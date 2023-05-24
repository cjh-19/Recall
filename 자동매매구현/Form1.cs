// 구현 환경
// Visual Studio Community 2019
// Oracle DBMS
// KIWOOM Open API
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client; // MS에서 oracle developer tools for visual studio 다운로드
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading; // 스레드 라이브러리 참조
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; //네이버 api 받아오기 위한 매개체
using System.Diagnostics;
using System.Net;
using System.IO;

namespace Team1
{
    public partial class Form1 : Form
    {
        string g_user_id = null;
        string g_accnt_no = null;
        int g_scr_no = 0; //OpenAPI 요청번호

        int g_is_thread = 0; // 0이면 스레드 미생성, 1이면 스레드 생성
        Thread thread1 = null; // 생성된 스레드 객체를 담을 변수

        string g_rqname = null;

        const string _apiUrl = "https://openapi.naver.com/v1/search/news";   //추가기능 뉴스 api
        const string _clientId = "I74lzNbMOpmIlEsfaWRO";
        const string _clientSecret = "jgntupzVD8";

        int g_flag_1 = 0; // 1이면 요청에 대한 응답 완료
        int g_flag_2 = 0;
        int g_flag_3 = 0; // 매수주문 응답 플래그
        int g_flag_4 = 0; // 매도주문 응답 플래그
        int g_flag_5 = 0; // 매도취소주문 응답 플래그
        int g_cur_price = 0; // 현재가
        int g_flag_6 = 0; // 1이면 조회 완료
        int g_buy_hoga = 0; // 최우선 매수호가 저장
        int g_flag_7 = 0; // 1이면 조회 완료
        public Form1()
        {
            InitializeComponent();
            // 데이터 수신 요청에 대한 응답을 받는 대기 이벤트 메서드 선언
            this.axKHOpenAPI1.OnReceiveTrData += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEventHandler(this.axKHOpenAPI1_OnReceiveTrData);
            this.axKHOpenAPI1.OnReceiveMsg += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveMsgEventHandler(this.axKHOpenAPI1_OnReceiveMsg);
            this.axKHOpenAPI1.OnReceiveChejanData += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEventHandler(this.axKHOpenAPI1_OnReceiveChejanData);
        }
        // 10장에서 정의 : 투자정보를 요청할 때 데이터 수신 요청에 대한 응답을 받는 이벤트 메서드
        private void axKHOpenAPI1_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (g_rqname.CompareTo(e.sRQName) == 0) // 요청한 요청명과 API로부터 응답받은 요청명이 같다면
            {
                ; // 다음으로 진행
            }
            else // 같지 않다면
            {
                write_err_log("요청한 TR : [" + g_rqname + "]\n", 0);
                write_err_log("응답받은 TR : [" + e.sRQName + "]\n", 0);

                switch (g_rqname)
                {
                    case "증거금세부내역조회요청":
                        g_flag_1 = 1; // 요청하는 쪽이 무한루프에 빠지지 않도록 방지
                        break;
                    case "계좌평가현황요청":
                        g_flag_2 = 1; // 요청하는 쪽이 무한루프에 빠지지 않도록 방지
                        break;
                    case "호가조회":
                        g_flag_7 = 1;
                        break;
                    case "현재가조회":
                        g_flag_6 = 1;
                        break;
                    default: break;
                }
            }

            if (e.sRQName == "호가조회")
            {
                int cnt = 0;
                int ii = 0;
                int l_buy_hoga = 0;

                cnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                for (ii = 0; ii < cnt; ii++)
                {
                    l_buy_hoga = int.Parse(axKHOpenAPI1.CommGetData(e.sTrCode, "", e.sRQName, ii, "매수최우선호가").Trim());
                    l_buy_hoga = System.Math.Abs(l_buy_hoga);
                }

                g_buy_hoga = l_buy_hoga;

                axKHOpenAPI1.DisconnectRealData(e.sScrNo);
                g_flag_7 = 1;
            }

            if(e.sRQName == "현재가조회") // 응답받은 요청명이 현재가조회
            {
                g_cur_price = int.Parse(axKHOpenAPI1.CommGetData(e.sTrCode, "", e.sRQName, 0, "현재가").Trim());
                g_cur_price = System.Math.Abs(g_cur_price);
                axKHOpenAPI1.DisconnectRealData(e.sScrNo);
                g_flag_6 = 1;
            }
        }
        // 12장에서 정의 : 주식주문을 요청할 때 해당 주식 주문의 응답을 수신하는 이벤트 메서드
        private void axKHOpenAPI1_OnReceiveMsg(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {

        }
        // 12장에서 정의 : 주식주문을 요청한 후 주문내역과 체결내역 데이터를 수신하는 이벤트 메서드
        private void axKHOpenAPI1_OnReceiveChejanData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {

        }


        public string get_cur_tm()  //시간 가져오기 함수
        {
            DateTime l_cur_time;
            string l_cur_tm;

            l_cur_time = DateTime.Now; //현재시각 저장
            l_cur_tm = l_cur_time.ToString("HHmmss"); //시분초 저장

            return l_cur_tm;
        }

        public string get_jongmok_nm(string i_jongmok_cd)   // 종목명 불러오기 함수 (입력값은 종목코드)
        {
            string l_jongmok_nm = null;

            l_jongmok_nm = axKHOpenAPI1.GetMasterCodeName(i_jongmok_cd); //코드로 종목명 가져오기

            return l_jongmok_nm;
        }

        public void write_msg_log(String text, int is_clear)  // 메시지 로그 출력 함수
        {
            DateTime l_cur_time;
            String l_cur_dt;
            String l_cur_tm;
            String l_cur_dtm;

            l_cur_dt = "";
            l_cur_tm = "";
            l_cur_time = DateTime.Now;
            l_cur_dt = l_cur_time.ToString("yyyy-") + l_cur_time.ToString("MM-") + l_cur_time.ToString("dd");

            l_cur_tm = l_cur_time.ToString("HH:mm:ss");
            l_cur_dtm = "[" + l_cur_dt + " " + l_cur_tm + "]";

            if (is_clear == 1)
            {
                if (this.messagelog.InvokeRequired)
                {
                    messagelog.BeginInvoke(new Action(() => messagelog.Clear()));
                }
                else
                    this.messagelog.Clear();
            }
            else
            {
                if (this.messagelog.InvokeRequired)
                {
                    messagelog.BeginInvoke(new Action(() => messagelog.AppendText(l_cur_dtm + text)));
                }
                else
                {
                    this.messagelog.AppendText(l_cur_dtm + text);
                }
            }
        }
        public void write_err_log(String text, int is_clear) //에러로그 출력함수
        {
            DateTime l_cur_time;
            String l_cur_dt;
            String l_cur_tm;
            String l_cur_dtm;

            l_cur_dt = "";
            l_cur_tm = "";
            l_cur_time = DateTime.Now;
            l_cur_dt = l_cur_time.ToString("yyyy-") + l_cur_time.ToString("MM-") + l_cur_time.ToString("dd");

            l_cur_tm = l_cur_time.ToString("HH:mm:ss");
            l_cur_dtm = "[" + l_cur_dt + " " + l_cur_tm + "]";

            if (is_clear == 1)
            {
                if (this.errorlog.InvokeRequired)
                {
                    errorlog.BeginInvoke(new Action(() => errorlog.Clear()));
                }
                else
                    this.errorlog.Clear();
            }
            else
            {
                if (this.errorlog.InvokeRequired)
                {
                    errorlog.BeginInvoke(new Action(() => errorlog.AppendText(l_cur_dtm + text)));
                }
                else
                {
                    this.errorlog.AppendText(l_cur_dtm + text);
                }
            }
        }

        [HandleProcessCorruptedStateExceptions]  // 손상된 프로레스 상태를 나타내는 예외 처리
        [SecurityCritical] // 코드나 어셈블리가 중요한 작업을 수행함을 지정
        public DateTime delay(int MS) // 지연메서드 (키움증권 api는 초당 5회로 요청횟수 제한하고 있음으로) MS는 밀리초
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                try
                {
                    unsafe  //안전하지 않은 코드허용 덕분
                    {
                        System.Windows.Forms.Application.DoEvents();   //메세지 큐의 모든 메시지 처리
                    }
                }
                catch (AccessViolationException e)
                {
                    write_err_log("delay() e.Message : [" + e.Message + "]\n", 0);
                }
                ThisMoment = DateTime.Now;
            }
            return DateTime.Now;

        }

        private string get_scr_no()   //OpenAPI 화면번호 가져오기 함수
        {
            if (g_scr_no < 9999)
                g_scr_no++;
            else
                g_scr_no = 1000;

            return g_scr_no.ToString();

        }
        private OracleConnection connect_db()  //오라클 접속 (성공)
        {
            // user id, password, data source 모두 string 형태로 받아서 DBMS로 보내 연결하는 형태
            // cmd에서 "$lsnrctl status" 를 통해서 host와 port 부분을 자신것으로 수정해야함
            // 위처럼 안하고 오라클dbms에서 c##team 의 속성에서 확인 가능
            // service name과 리스너를 설정하기 위해 "listener.ora" 와 "tnsnames.ora"수정 필요
            // listener.ora 에서는
            // "SID_LIST_LISTENER = "에 다음과 같이 추가 host와 port 추가
            //     (SID_DESC =
            //          (SID_NAME = xe)
            //          (ORACLE_HOME = F:\oracleDB\app\oracle\product\11.2.0\server)
            //      )
            // "LISTENER = " 에 다음과 같이 추가
            //     (DESCRIPTION =
            //          (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
            //      )
            //
            // tnsnames.ora 에서는 xe(sid)를 추가한다 sid 와 service_name은 같아야한다.
            // xe =
            //  (DESCRIPTION =
            //          (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
            //          (CONNECT_DATA =
            //              (SERVER = DEDICATED)
            //              (SERVICE_NAME = xe)
            //          )
            //  )


            String conninfo = "User Id = c##team;" +
                "Password = 1234;" +
                "Data Source = (DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=xe)) );";

            OracleConnection conn = new OracleConnection(conninfo); //오라클 연결 인스턴스

            try
            {
                conn.Open(); //접속
            }
            catch (Exception e)
            {
                MessageBox.Show("connect_db() Failed " + e.Message, "오류발생");
                conn = null;
            }
            return conn;
        }

        private void 로그인ToolStripMenuItem_Click(object sender, EventArgs e)  //로그인 버튼 눌렀을 시
        {
            int ret = 0;
            int ret2 = 0;

            String l_accno = null; //계좌번호
            String l_accno_cnt = null; //소유한 계좌번호 수
            String[] l_accno_arr = null; //N개의 계좌번호 저장배열

            ret = axKHOpenAPI1.CommConnect();//로그인 창 호출 api함수

            if (ret == 0)
            {
                toolStripStatusLabel1.Text = "로그인 하는 중...";

                for (; ; )
                {
                    ret2 = axKHOpenAPI1.GetConnectState(); //로그인 완료 여부 api함수
                    if (ret2 == 1)
                    {    //로그인 완료시
                        break;
                    }
                    else
                    {
                        delay(1000); //1초지연 지연메서드 구현요망
                    }
                }

                toolStripStatusLabel1.Text = "로그인 완료";

                g_user_id = "";
                g_user_id = axKHOpenAPI1.GetLoginInfo("USER_ID").Trim(); //사용자 아이디 클라스 변수에 저장

                idbox.Text = g_user_id;

                l_accno_cnt = "";
                l_accno_cnt = axKHOpenAPI1.GetLoginInfo("ACCOUNT_CNT").Trim();  //사용자 계좌번호 수 를 저장

                l_accno_arr = new string[int.Parse(l_accno_cnt)];

                l_accno = "";
                l_accno = axKHOpenAPI1.GetLoginInfo("ACCNO").Trim(); //사용자 계좌번호 저장

                l_accno_arr = l_accno.Split(';');

                accountbox.Items.Clear();
                accountbox.Items.AddRange(l_accno_arr); //N개의 계좌번호 콤보박스에 저장
                accountbox.SelectedIndex = 0;  //초기선택
                g_accnt_no = accountbox.SelectedItem.ToString().Trim(); //설정된 계좌번호 클래스 변수에 저장
            }
        }

        private void accountbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            g_accnt_no = accountbox.SelectedItem.ToString().Trim();
            write_msg_log("사용할 증권계좌 번호는 : [" + g_accnt_no + "] 입니다. \n", 0);
        }

        private void 로그아웃ToolStripMenuItem_Click(object sender, EventArgs e) //로그아웃 버튼 클릭시
        {
            axKHOpenAPI1.CommTerminate();
            toolStripStatusLabel1.Text = "로그아웃 완료";
        }

        //종목조회버튼 기능
        private void searchbtn_Click(object sender, EventArgs e)
        // 오라클 DBMS 접속 > TB_TRD_JONG SELECT 문 작성 AND 수행 >
        // 존재하는 행만큼 반복하면서 결과값 배열에 저장 후 데이터그리드뷰에 출력 > 오라클 DBMS 접속
        {
            OracleCommand cmd;
            OracleConnection conn;
            OracleDataReader reader = null;

            string sql;

            string l_jongmok_cd;
            string l_jongmok_nm;
            int l_priority;
            int l_buy_amt;
            int l_buy_price;
            int l_tager_price;
            int l_cut_loss_price;
            string l_buy_trd_yn;
            string l_sell_trd_yn;
            int l_seq = 0;
            string[] l_arr = null;

            conn = null;
            conn = connect_db(); //DB접속

            cmd = null;

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            sql = null;
            // DB 쿼리문 작업
            sql = " SELECT " + // 거래종목 테이블 조회 SQL 작성
                    "JONGMOK_CD , " +
                    "JONGMOk_NM , " +
                    "PRIORITY , " +
                    "BUY_AMT , " +
                    "BUY_PRICE , " +
                    "TARGET_PRICE , " +
                    "CUT_LOSS_PRICE , " +
                    "BUY_TRD_YN , " +
                    "SELL_TRD_YN" +
                    " FROM " +
                    "TB_TRD_JONGMOK" +
                    " WHERE USER_ID = " + "'" + g_user_id + "'" + " order by PRIORITY ";

            cmd.CommandText = sql;

            this.Invoke(new MethodInvoker(
                delegate ()
                {
                    dataGridView1.Rows.Clear(); // 그리드뷰 초기화
                }
                ));

            try
            {
                reader = cmd.ExecuteReader(); // SQL 수행
            }
            catch (Exception ex)
            {
                write_err_log("SELECT TB_TRD_JONGMOK ex.MESSAGE : [" + ex.Message + "]\n", 0);
            }

            l_jongmok_cd = "";
            l_jongmok_nm = "";
            l_priority = 0;
            l_buy_amt = 0;
            l_buy_price = 0;
            l_tager_price = 0;
            l_cut_loss_price = 0;
            l_buy_trd_yn = "";
            l_sell_trd_yn = "";

            while (reader.Read())
            {
                l_seq++;
                l_jongmok_cd = "";
                l_jongmok_nm = "";
                l_priority = 0;
                l_buy_amt = 0;
                l_buy_price = 0;
                l_tager_price = 0;
                l_cut_loss_price = 0;
                l_buy_trd_yn = "";
                l_sell_trd_yn = "";
                l_seq = 0;

                // 각 컬럼 값 저장
                l_jongmok_cd = reader[0].ToString().Trim();
                l_jongmok_nm = reader[1].ToString().Trim();
                l_priority = int.Parse(reader[2].ToString().Trim());
                l_buy_amt = int.Parse(reader[3].ToString().Trim());
                l_buy_price = int.Parse(reader[4].ToString().Trim());
                l_tager_price = int.Parse(reader[5].ToString().Trim());
                l_cut_loss_price = int.Parse(reader[6].ToString().Trim());
                l_buy_trd_yn = reader[7].ToString().Trim();
                l_sell_trd_yn = reader[8].ToString().Trim();

                l_arr = null;
                l_arr = new String[] // 가져온 결과를 문자열 배열에 저장
                {
                    l_seq.ToString(),
                    l_jongmok_cd,
                    l_jongmok_nm,
                    l_priority.ToString(),
                    l_buy_amt.ToString(),
                    l_buy_price.ToString(),
                    l_tager_price.ToString(),
                    l_cut_loss_price.ToString(),
                    l_buy_trd_yn,
                    l_sell_trd_yn
                };
                this.Invoke(new MethodInvoker(
                    delegate ()
                    {
                        dataGridView1.Rows.Add(l_arr); // 데이터그리드뷰에 추가
                    }));
            }
            write_msg_log("TB_TRD_JONGMOK 테이블이 조회되었습니다.\n", 0);
        }

        // 종목삽입 버튼 기능
        private void insertbtn_Click(object sender, EventArgs e)
        {
            OracleCommand cmd;
            OracleConnection conn;

            string sql;

            string l_jongmok_cd;
            string l_jongmok_nm;
            int l_priority;
            int l_buy_amt;
            int l_buy_price;
            int l_tager_price;
            int l_cut_loss_price;
            string l_buy_trd_yn;
            string l_sell_trd_yn;

            foreach (DataGridViewRow Row in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(Row.Cells[check.Name].Value) != true)
                {
                    continue;
                }
                if (Convert.ToBoolean(Row.Cells[check.Name].Value) == true)
                {
                    l_jongmok_cd = Row.Cells[1].Value.ToString();
                    l_jongmok_nm = Row.Cells[2].Value.ToString();
                    l_priority = int.Parse(Row.Cells[3].Value.ToString());
                    l_buy_amt = int.Parse(Row.Cells[4].Value.ToString());
                    l_buy_price = int.Parse(Row.Cells[5].Value.ToString());

                    l_tager_price = int.Parse(Row.Cells[6].Value.ToString());
                    l_cut_loss_price = int.Parse(Row.Cells[7].Value.ToString());

                    l_buy_trd_yn = Row.Cells[8].Value.ToString();
                    l_sell_trd_yn = Row.Cells[9].Value.ToString();

                    conn = null;
                    conn = connect_db();

                    cmd = null;
                    cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    sql = null;
                    // DB 쿼리문 작업
                    sql = @"insert into TB_TRD_JONGMOK values " +
                        "(" +
                        "'" + g_user_id + "'" + "," +
                        "'" + l_jongmok_cd + "'" + "," +
                        "'" + l_jongmok_nm + "'" + ","
                        + l_priority + ","
                        + l_buy_amt + ","
                        + l_buy_price + ","
                        + l_tager_price + ","
                        + l_cut_loss_price + "," +
                        "'" + l_buy_trd_yn + "'" + "," +
                        "'" + l_sell_trd_yn + "'" + "," +
                        "'" + g_user_id + "'" + "," +
                        "sysdata " + "," +
                        "NULL" + "," +
                        "NULL" + ")";

                    cmd.CommandText = sql;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        write_err_log("insert TB_TRD_JONGMOK ex.Message : [" + ex.Message + "]\n", 0);
                    }
                    write_msg_log("종목코드 : [" + l_jongmok_cd + "]가 삽입되었습니다.\n", 0);
                    conn.Close();
                }
            }
        }

        // 종목수정 버튼 기능
        private void alterbtn_Click(object sender, EventArgs e)
        {
            OracleCommand cmd;
            OracleConnection conn;

            string sql;

            string l_jongmok_cd;
            string l_jongmok_nm;
            int l_priority;
            int l_buy_amt;
            int l_buy_price;
            int l_tager_price;
            int l_cut_loss_price;
            string l_buy_trd_yn;
            string l_sell_trd_yn;

            foreach (DataGridViewRow Row in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(Row.Cells[check.Name].Value) != true)
                {
                    continue;
                }
                if (Convert.ToBoolean(Row.Cells[check.Name].Value) == true)
                {
                    l_jongmok_cd = Row.Cells[1].Value.ToString();
                    l_jongmok_nm = Row.Cells[2].Value.ToString();
                    l_priority = int.Parse(Row.Cells[3].Value.ToString());
                    l_buy_amt = int.Parse(Row.Cells[4].Value.ToString());
                    l_buy_price = int.Parse(Row.Cells[5].Value.ToString());

                    l_tager_price = int.Parse(Row.Cells[6].Value.ToString());
                    l_cut_loss_price = int.Parse(Row.Cells[7].Value.ToString());

                    l_buy_trd_yn = Row.Cells[8].Value.ToString();
                    l_sell_trd_yn = Row.Cells[9].Value.ToString();

                    conn = null;
                    conn = connect_db();

                    cmd = null;
                    cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    sql = null;
                    // DB 쿼리문 작업
                    sql = @" UPDATE TB_TRD_JONGMOK SET JONGMOK_NM = " +
                        "'" + l_jongmok_nm + "'" + "," +
                        " PRIORITY = " + l_priority + "," +
                        " BUY_AMT = " + l_buy_amt + "," +
                        " BUY_PRICE = " + l_buy_price + "," +
                        " TARGET_PRICE = " + l_tager_price + "," +
                        " CUT_LOSS_PRICE" + l_cut_loss_price + "," +
                        " BUY_TRD_YN = " + "'" + l_buy_trd_yn + "'" + "," +
                        " SELL_TRD_YN = " + "'" + l_sell_trd_yn + "'" + "," +
                        " UPDT_ID = " + "'" + g_user_id + "'" + "," +
                        " UPDT_DTM = SYSDAATE" + " WHERE JONGMOK_CD = " + "'" + l_jongmok_cd + "'" +
                        " AND USER_ID = " + "'" + g_user_id + "'";

                    cmd.CommandText = sql;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        write_err_log("UPDATE TB_TRD_JONGMOK ex.Message : [" + ex.Message + "]\n", 0);
                    }
                    write_msg_log("종목코드 : [" + l_jongmok_cd + "]가 수정되었습니다.\n", 0);
                    conn.Close();
                }
            }
        }

        //종목삭제 버튼 기능
        private void deletebtn_Click(object sender, EventArgs e)
        {
            OracleCommand cmd;
            OracleConnection conn;

            string sql;

            string l_jongmok_cd = null;

            foreach (DataGridViewRow Row in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(Row.Cells[check.Name].Value) != true)
                {
                    continue;
                }
                if (Convert.ToBoolean(Row.Cells[check.Name].Value) == true)
                {
                    l_jongmok_cd = Row.Cells[1].Value.ToString();

                    conn = null;
                    conn = connect_db();

                    cmd = null;
                    cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    sql = null;
                    // DB 쿼리문 작업
                    sql = @" DELETE FROM TB_TRD_JONGMOK " +
                        " WHERE JONGMOK_CD = " + "'" + l_jongmok_cd + "'" +
                        " AND USER_ID = " + "'" + g_user_id + "'";

                    cmd.CommandText = sql;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        write_err_log("DELETE TB_TRD_JONGMOK ex.Message : [" + ex.Message + "]\n", 0);
                    }
                    write_msg_log("종목코드 : [" + l_jongmok_cd + "]가 삭제되었습니다.\n", 0);
                    conn.Close();
                }
            }
        }

        private void autostartbtn_Click(object sender, EventArgs e)
        {
            if (g_is_thread == 1) // 스레드가 이미 생성된 상태라면
            {
                write_msg_log("Auto Tradng이 이미 시작되었습니다.\n", 0);
                return; // 이벤트 메서드 종료
            }
            // 스레드 생성 시작
            g_is_thread = 1; // 스레드 생성으로 값 설정
            thread1 = new Thread(new ThreadStart(m_thread1)); // 스레드 생성
            thread1.Start(); // 스레드 시작
        }
        public void m_thread1()
        {
            string l_cur_tm = null;

            int l_set_tb_accnt_flag = 0; // 1이면 호출 완료
            int l_set_tb_eccnt_info_flag = 0; // 1이면 호출 완료
            int l_sell_ord_first_flag = 0; //1이면 호출 완료

            // 스레드 생성 파트
            if (g_is_thread == 0) // 최초 스레드 생성
            {
                g_is_thread = 1; // 중복 스레드 생성 방지를 위해 스레드 값 1로 변경
                write_msg_log("자동매매가 시작되었습니다.\n", 0);
            }

            for (; ; ) // 첫 번째 무한루프 시작
            {
                l_cur_tm = get_cur_tm(); // 현재 시각 조회
                if (l_cur_tm.CompareTo("083001") >= 0) // 8시 30분 이후라면
                {
                    if (l_set_tb_accnt_flag == 0)
                    {
                        l_set_tb_eccnt_info_flag = 1; //호출로 설정
                        set_tb_accnt(); // 호출 //이전 선언 필요
                    }
                    if (l_set_tb_eccnt_info_flag == 0)
                    {
                        set_tb_accnt_info(); // 이전 선언 필요
                        l_set_tb_eccnt_info_flag = 1;
                    }
                    if (l_sell_ord_first_flag == 0)
                    {
                        sell_ord_first(); // 보유종목 매도
                        l_sell_ord_first_flag = 1;
                    }
                }

                // 장이 열리면 장이 닫힐 때까지 for문안에서 다시 무한루프를 시작한다.
                if (l_cur_tm.CompareTo("090001") >= 0) // 09시 이후라면
                {
                    for (; ; ) // 두 번째 무한루프 시작
                    {
                        l_cur_tm = get_cur_tm(); // 현재시각 조회
                        if (l_cur_tm.CompareTo("153001") >= 0) // 15시 30분 이후라면
                        {
                            break; // 장이 닫히면 두 번째 무한루프를 빠져나간다.
                        }
                        // 장 운영 시간 중이므로 매수나 매도 주문
                        real_buy_ord();

                        delay(200); // 두 번째 무한루프 지연
                        real_sell_ord(); // 실시간 매도주문 호출

                        delay(200);
                        real_cut_loss_ord(); // 실시간 손절주문 호출
                    }
                }
                delay(200); // 첫 번째 무한루프 지연
            }
        }

        private void autostopbtn_Click(object sender, EventArgs e)
        {
            write_msg_log("\n 자동매매 중지 시작\n", 0);

            try
            {
                thread1.Abort();
            }
            catch (Exception ex)
            {
                write_err_log("자동매매 중지 ex.Message : " + ex.Message + "\n", 0);
            }

            this.Invoke(new MethodInvoker(() =>
            {
                if (thread1 != null)
                {
                    // 인터럽트를 실행하여 스레드를 중단한다.
                    thread1.Interrupt();
                    thread1 = null;
                }
            }));
            g_is_thread = 0; // 스레드를 중단했다는 값
            // 추후에 자동매매를 다시 시작하기 위해 0으로 설정

            write_msg_log("\n 자동매매 중지 완료\n", 0);
        }

        public void set_tb_accnt()
        {
            // 10장 계좌 조회
        }

        public void set_tb_accnt_info()
        {
            // 11장 계좌정보 조회
        }

        // 매도대상 종목 조회
        public void sell_ord_first() // 계좌정보 보유종목의 매도주문
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            string l_jongmok_cd = null;
            int l_buy_price = 0;
            int l_own_stock_cnt = 0;
            int l_target_price = 0;

            // conn = null;
            // sql = null;
            // cmd = null;
            // reader = cull;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            // TB_ACCNT_INFO, TB_TRD_JONGMOK table을 join하여 매도 대상 종목을 조회
            sql = @" SELECT " +
                "    A.JONGMOK_CD, " +
                "    A.BUT_PRICE, " +
                "    A.OWN_STOCK_CNT, " +
                "    B.TARGET_PRICE " +
                " FROM TB_ACCNT_INFO A, " +
                "    TB_TRD_JONGMOK B " +
                " WHERE A.USER_ID = " + "'" + g_user_id + "'" +
                " AND A.ACCNT_NO = " + "'" + g_accnt_no + "'" +
                " AND A.REF_DT = TO_CHAR(SYSDATE, 'YYYYMMDD') " +
                " AND A.USER_ID = B.USER_ID " +
                " AND A.JONGMOK_CD = B.JONGMOK_CD " +
                " AND B.SELL_TRD_YN = 'Y' AND A.OWN_STOCK_CNT > 0 ";

            cmd.CommandText = sql;
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                l_jongmok_cd = "";
                l_buy_price = 0;
                l_own_stock_cnt = 0;
                l_target_price = 0;

                l_jongmok_cd = reader[0].ToString().Trim();
                l_buy_price = int.Parse(reader[1].ToString().Trim());
                l_own_stock_cnt = int.Parse(reader[2].ToString().Trim());
                l_target_price = int.Parse(reader[3].ToString().Trim());

                write_msg_log("종목코드 : [" + l_jongmok_cd + "]\n", 0);
                write_msg_log("매입가 : [" + l_buy_price.ToString() + "]\n", 0);
                write_msg_log("보유주식수 : [" + l_own_stock_cnt.ToString() + "]\n", 0);
                write_msg_log("목표가 : [" + l_target_price.ToString() + "]\n", 0);

                int l_new_target_price = 0;
                l_new_target_price = get_hoga_unit_price(l_target_price, l_jongmok_cd, 0);

                g_flag_4 = 0; // 이전 int 선언 필요
                g_rqname = "매도주문";

                String l_scr_no = null;
                l_scr_no = "";
                l_scr_no = get_scr_no();

                int ret = 0;

                //매도주문 요청
                ret = axKHOpenAPI1.SendOrder("매도주문", l_scr_no, g_accnt_no, 2, l_jongmok_cd, l_own_stock_cnt, l_new_target_price, "00", "");
                if (ret == 0)
                {
                    write_msg_log("매도주문 Sendord() 호출 성공\n", 0);
                    write_msg_log("종목코드 : [" + l_jongmok_cd + "]\n", 0);
                }
                else
                {
                    write_msg_log("매도주문 Sendord() 호출 실패\n", 0);
                    write_msg_log("i_jongmok_cd : [" + l_jongmok_cd + "]\n", 0);
                }

                delay(200);

                for (; ; )
                {
                    if (g_flag_4 == 1)
                    {
                        delay(200);
                        axKHOpenAPI1.DisconnectRealData(l_scr_no);
                        break;
                    }
                    else
                    {
                        write_msg_log("'매도주문' 완료 대기 중...\n", 0);
                        delay(200);
                        break;
                    }
                }
                axKHOpenAPI1.DisconnectRealData(l_scr_no);
            }

            reader.Close();
            conn.Close();
        }

        public int get_hoga_unit_price(int i_price, String i_jongmok_cd, int i_hoga_unit_jump) // 유효한 호가가격단위를 구함
        {
            int l_market_type = 0; // 0:코스피 , 10:코스닥
            int l_rest;

            try
            {
                l_market_type = int.Parse(axKHOpenAPI1.GetMarketType(i_jongmok_cd).ToString());
            } catch (Exception ex)
            {
                write_err_log("get_hoga_unit_price() ex.Message : [" + ex.Message + "]\n", 0);
            }

            if (i_price < 2000)
            {
                return i_price + (i_hoga_unit_jump * 1);
            }
            else if (i_price >= 2000 && i_price < 5000)
            {
                l_rest = i_price % 5;
                if (l_rest == 0)
                {
                    return i_price + (i_hoga_unit_jump * 5);
                } else if (l_rest < 3)
                {
                    return (i_price - l_rest) + (i_hoga_unit_jump * 5);
                } else
                {
                    return (i_price + (5 - l_rest)) + (i_hoga_unit_jump * 5);
                }
            }
            else if (i_price >= 5000 && i_price < 20000)
            {
                l_rest = i_price % 10;
                if (l_rest == 0)
                {
                    return i_price + (i_hoga_unit_jump * 10);
                }
                else if (l_rest < 5)
                {
                    return (i_price - l_rest) + (i_hoga_unit_jump * 10);
                }
                else
                {
                    return (i_price + (10 - l_rest)) + (i_hoga_unit_jump * 10);
                }
            }
            else if (i_price >= 20000 && i_price < 50000)
            {
                l_rest = i_price % 50;
                if (l_rest == 0)
                {
                    return i_price + (i_hoga_unit_jump * 50);
                }
                else if (l_rest < 25)
                {
                    return (i_price - l_rest) + (i_hoga_unit_jump * 50);
                }
                else
                {
                    return (i_price + (50 - l_rest)) + (i_hoga_unit_jump * 50);
                }
            }
            else if (i_price >= 50000 && i_price < 200000)
            {
                l_rest = i_price % 100;
                if (l_rest == 0)
                {
                    return i_price + (i_hoga_unit_jump * 100);
                }
                else if (l_rest < 50)
                {
                    return (i_price - l_rest) + (i_hoga_unit_jump * 100);
                }
                else
                {
                    return (i_price + (100 - l_rest)) + (i_hoga_unit_jump * 100);
                }
            }
            else if (i_price >= 200000 && i_price < 500000)
            {
                if (l_market_type == 10)
                {

                    l_rest = i_price % 100;
                    if (l_rest == 0)
                    {
                        return i_price + (i_hoga_unit_jump * 100);
                    }
                    else if (l_rest < 50)
                    {
                        return (i_price - l_rest) + (i_hoga_unit_jump * 100);
                    }
                    else
                    {
                        return (i_price + (100 - l_rest)) + (i_hoga_unit_jump * 100);
                    }
                }
                else
                {
                    l_rest = i_price % 500;
                    if (l_rest == 0)
                    {
                        return i_price + (i_hoga_unit_jump * 500);
                    }
                    else if (l_rest < 250)
                    {
                        return (i_price - l_rest) + (i_hoga_unit_jump * 500);
                    }
                    else
                    {
                        return (i_price + (500 - l_rest)) + (i_hoga_unit_jump * 500);
                    }
                }
            }
            else if (i_price >= 500000)
            {
                if (l_market_type == 10)
                {

                    l_rest = i_price % 100;
                    if (l_rest == 0)
                    {
                        return i_price + (i_hoga_unit_jump * 100);
                    }
                    else if (l_rest < 50)
                    {
                        return (i_price - l_rest) + (i_hoga_unit_jump * 100);
                    }
                    else
                    {
                        return (i_price + (100 - l_rest)) + (i_hoga_unit_jump * 100);
                    }
                }
                else
                {
                    l_rest = i_price % 1000;
                    if (l_rest == 0)
                    {
                        return i_price + (i_hoga_unit_jump * 1000);
                    }
                    else if (l_rest < 500)
                    {
                        return (i_price - l_rest) + (i_hoga_unit_jump * 1000);
                    }
                    else
                    {
                        return (i_price + (1000 - l_rest)) + (i_hoga_unit_jump * 1000);
                    }
                }
            }
            return 0;
        }

        public void real_buy_ord() // 매수대상 거래종목 조회
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            string l_jongmok_cd = null;
            int l_buy_price = 0;
            int l_buy_amt = 0;

            // conn = null;
            // sql = null;
            // cmd = null;
            // reader = cull;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            // TB_ACCNT_INFO, TB_TRD_JONGMOK table을 join하여 매도 대상 종목을 조회

            sql = @"                    " +
                   " SELECT             " +
                   "    A.JONGMOK_CD,   " +
                   "    A.BUY_AMT,      " +
                   "    A.BUY_PRICE     " +
                   " FROM TB_TRD_JONGMOK A " +
                   " WHERE A.USER_ID = " + "'" + g_user_id + "'" +
                   " AND A.BUY_TRD_YN = 'Y' " +
                   " ORDER BY A.PRIORITY ";

            cmd.CommandText = sql;
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                l_jongmok_cd = "";
                l_buy_amt = 0;
                l_buy_price = 0;


                l_jongmok_cd = reader[0].ToString().Trim(); // 종목코드
                l_buy_amt = int.Parse(reader[1].ToString().Trim()); // 매수금액
                l_buy_price = int.Parse(reader[2].ToString().Trim()); // 매수가

                int l_buy_price_tmp = 0;
                l_buy_price_tmp = get_hoga_unit_price(l_buy_price, l_jongmok_cd, 1); // 매수호가 구하기

                int l_buy_ord_stock_cnt = 0;
                l_buy_ord_stock_cnt = (int)(l_buy_amt / l_buy_price_tmp); // 매수주문 주식 수 구하기

                write_msg_log("종목코드 : [" + l_jongmok_cd + "]\n", 0);
                write_msg_log("종목명 : [" + get_jongmok_nm(l_jongmok_cd) + "]\n", 0);
                write_msg_log("매수금액 : [" + l_buy_amt.ToString() + "]\n", 0);
                write_msg_log("매수가 : [" + l_buy_price_tmp.ToString() + "]\n", 0);

                int l_own_stock_cnt = 0;
                l_own_stock_cnt = get_own_stock_cnt(l_jongmok_cd); // 해당 종목 보유주식 수 구하기
                write_msg_log("보유주식수 : [" + l_own_stock_cnt.ToString() + "]\n", 0);

                if(l_own_stock_cnt > 0)
                {
                    write_msg_log("해당 종목을 보유 중이므로 매수하지 않음\n", 0);
                    continue;
                }

                string l_buy_not_chegyul_yn = null;
                l_buy_not_chegyul_yn = get_buy_not_chegyul_yn(l_jongmok_cd); // 미체결 매수주문 여부 확인

                // 매수주문 전 최우선 매수호가 조회
                int l_for_flag = 0;
                int l_for_cnt = 0;
                g_buy_hoga = 0;

                for(; ;)
                {
                    g_rqname = "";
                    g_rqname = "호가조회";
                    g_flag_7 = 0;
                    axKHOpenAPI1.SetInputValue("종목코드", l_jongmok_cd);

                    string l_scr_no_2 = null;
                    l_scr_no_2 = "";
                    l_scr_no_2 = get_scr_no();

                    axKHOpenAPI1.CommRqData("호가조회", "OPT10004", 0, l_scr_no_2);

                    try
                    {
                        l_for_cnt = 0;
                        for(; ; )
                        {
                            if (g_flag_7 == 1)
                            {
                                delay(200);
                                axKHOpenAPI1.DisconnectRealData(l_scr_no_2);
                                l_for_flag = 1;
                                break;
                            }
                            else
                            {
                                write_msg_log("'호가조회' 완료 대기 중...\n", 0);
                                delay(200);
                                l_for_cnt++;
                                if(l_for_cnt == 5)
                                {
                                    l_for_flag = 0;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    } catch (Exception ex)
                    {
                        write_err_log("real_buy_ord() 호가조회 ex.Message : [" + ex.Message + "]\n", 0);
                    }

                    axKHOpenAPI1.DisconnectRealData(l_scr_no_2);

                    if (l_for_flag == 1)
                    {
                        break;
                    }
                    else if (l_for_flag == 0)
                    {
                        delay(200);
                        continue;
                    }
                    delay(200);
                }

                if (l_buy_not_chegyul_yn == "Y") // 미체결 매수주문이 있으므로 매수하지 않음
                {
                    write_msg_log("해당 종목에 미체결 매수주문이 있으므로 매수하지 않음 \n", 0);
                    continue;
                }

                if(l_buy_price > g_buy_hoga)
                {
                    write_msg_log("해당 종목의 매수가가 최우선 매수호가보다 크므로 매수주문하지 않음 \n", 0);
                    continue;
                }

                //매수대상 매수주문
                g_flag_3 = 0; // 이전 선언 필요
                g_rqname = "매수주문";

                String l_scr_no = null;
                l_scr_no = "";
                l_scr_no = get_scr_no();

                int ret = 0;

                //매도주문 요청
                ret = axKHOpenAPI1.SendOrder("매수주문", l_scr_no, g_accnt_no, 1, l_jongmok_cd, l_buy_ord_stock_cnt, l_buy_price, "00", "");
                if (ret == 0)
                {
                    write_msg_log("매수주문 Sendord() 호출 성공\n", 0);
                    write_msg_log("종목코드 : [" + l_jongmok_cd + "]\n", 0);
                }
                else
                {
                    write_msg_log("매수주문 Sendord() 호출 실패\n", 0);
                    write_msg_log("i_jongmok_cd : [" + l_jongmok_cd + "]\n", 0);
                }

                delay(200);

                for (; ; )
                {
                    if (g_flag_3 == 1)
                    {
                        delay(200);
                        axKHOpenAPI1.DisconnectRealData(l_scr_no);
                        break;
                    }
                    else
                    {
                        write_msg_log("'매수주문' 완료 대기 중...\n", 0);
                        delay(200);
                        break;
                    }
                }
                axKHOpenAPI1.DisconnectRealData(l_scr_no);
                delay(1000);
            }
            reader.Close();
            conn.Close();
        }

        public int get_own_stock_cnt(string i_jongmok_cd)
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            int l_own_stock_cnt = 0;

            // conn = null;
            // sql = null;
            // cmd = null;
            // reader = cull;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            sql = @"
                SELECT
                    NVL(MAX(OWN_STOCK_CNT), 0) OWN_STOCK_CNT
                    FROM
                    TB_ACCNT_INFO
                    WHERE USER_ID = " + "'" + g_user_id + "'" +
                    "AND JONGMOK_CD = " + "'" + i_jongmok_cd + "'" +
                    "AND ACCNT_NO = " + "'" + g_accnt_no + "'" +
                    "AND REF_DT = TO_CHAR(SYSDATE, 'YYYYMMDD') ";

            cmd.CommandText = sql;
            reader = cmd.ExecuteReader();
            reader.Read();

            l_own_stock_cnt = int.Parse(reader[0].ToString()); // 보유주식 수 구하기

            reader.Close();
            conn.Close();

            return l_own_stock_cnt;
        }

        public string get_buy_not_chegyul_yn(string i_jongmok_cd)
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            int l_buy_not_chegyul_ord_stock_cnt = 0;
            string l_buy_not_chegyul_yn = null;

            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            // 주문내역과 체결내역 테이블 조회
            sql = @"
                SECLET
                    NVL(SUM(ORD_STOCK_CNT - CHEGYUL_STOCK_CNT), 0) BUY_NOT_CHEGYUL_ORD_STOCK_CNT
                FROM
                (
                    SELECT ORD_STOCK_CNT ORD_STOCK_CNT,
                    ( SELECT NVL(MAX(B.CHEGYUL_STOCK_CNT), 0) CHEGYUL_STOCK_CNT
                      FROM TB_CHEGYUL_LST B
                      WHERE B.USER_ID = A.USER_ID
                      AND B.ACCNT_NO = A.ACCNT_NO
                      AND B.REF_DT = A.REF_DT
                      AND B.JONGMOK_CD = A.JONGMOK_CD
                      AND B.ORD_GB = A.ORD_GB
                      AND B.ORD_NO = A.ORD_NO
                    ) CHEGYUL_STOCK_CNT
                    FROM TB_ORD_LST A
                    WHERE A.REF_DT = TO_CHAR(SYSDATE, 'YYYYMMDD')
                    AND A.USER_ID = " + "'" + g_user_id + "'" +
                    " AND A.ACCNT_NO = " + "'" + g_accnt_no + "'" +
                    " AND A.JONGMOK_CD = " + "'" + i_jongmok_cd + "'" +
                    " AND A.ORD_GB = '2' " +
                    " AND A.ORG_ORD_NO = '0000000' " +
                    " AND NOT EXISTS ( SELECT '1' " +
                    "                  FROM TB_ORD_LST B " +
                    "                  WHERE B.USER_ID = A.USER_ID " +
                    "                  AND B.ACCNT_NO = A.ACCNT_NO " +
                    "                  AND B.REF_DT = A.REF_DT " +
                    "                  AND B.JONGMOK_CD = A.JONGMOK_CD " +
                    "                  AND B.ORD_GB = A.ORD_GB " +
                    "                  AND B.ORD_NO = A.ORD_NO " +
                " ) " +
            ") x ";


            cmd.CommandText = sql;
            reader = cmd.ExecuteReader();
            reader.Read();

            l_buy_not_chegyul_ord_stock_cnt = int.Parse(reader[0].ToString()); // 미체결 매수주문 주식 수 구하기

            reader.Close();
            conn.Close();

            if (l_buy_not_chegyul_ord_stock_cnt > 0)
            {
                l_buy_not_chegyul_yn = "Y";
            }
            else
            {
                l_buy_not_chegyul_yn = "N";
            }

            return l_buy_not_chegyul_yn;
        }

        public void real_sell_ord() // 매수대상 거래종목 조회
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            string l_jongmok_cd = null;
            int l_target_price = 0;
            int l_own_stock_cnt = 0;
            write_msg_log("real_sell_ord 시작\n", 0);

            // conn = null;
            // sql = null;
            // cmd = null;
            // reader = cull;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            
            //거래종목 및 계좌정보 테이블 조회
            sql = @" SELECT " +
                "    A.JONGMOK_CD, " +
                "    A.TARGET_PRICE, " +
                "    B.OWN_STOCK_CNT, " +
                " FROM " + 
                "    TB_TRD_JONGMOK A " +
                "    TB_ACCNT_INFO B, " +
                " WHERE A.USER_ID = " + "'" + g_user_id + "'" +
                " AND A.JONGMOK_CD = B.JONGMOK_CD " +
                " AND B.ACCNT_NO = " + "'" + g_accnt_no + "'" +
                " AND A.REF_DT = TO_CHAR(SYSDATE, 'YYYYMMDD') " +
                " AND A.SELL_TRD_YN = 'Y' AND B.OWN_STOCK_CNT > 0 ";

            cmd.CommandText = sql;
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                l_jongmok_cd = "";
                l_target_price = 0;

                l_jongmok_cd = reader[0].ToString().Trim();
                l_target_price = int.Parse(reader[1].ToString().Trim());
                l_own_stock_cnt = int.Parse(reader[2].ToString().Trim());

                write_msg_log("종목코드 : [" + l_jongmok_cd + "]\n", 0);
                write_msg_log("종목명 : [" + get_jongmok_nm(l_jongmok_cd) + "]\n", 0);
                write_msg_log("목표가 : [" + l_target_price.ToString() + "]\n", 0);
                write_msg_log("보유주식수 : [" + l_own_stock_cnt.ToString() + "]\n", 0);

                int l_sell_not_chegyul_ord_stock_cnt = 0;
                l_sell_not_chegyul_ord_stock_cnt = get_sell_not_chegyul_ord_stock_cnt(l_jongmok_cd); // 미체결 매도주문 주식 수 구하기

                if (l_sell_not_chegyul_ord_stock_cnt == l_own_stock_cnt) // 미체결 매도주문 주식 수와 보유주식 수가 같으면 기 주문종목이므로 매도주문하지 않음
                {
                    continue;
                }
                else // 같지 않으면 아직 매도하지 않은 종목
                {
                    int l_sell_ord_stock_cnt_tmp = 0;
                    l_sell_ord_stock_cnt_tmp = l_own_stock_cnt - l_sell_not_chegyul_ord_stock_cnt; // 보유 주식수 - 미체결 매도주문 주식 수

                    if(l_sell_ord_stock_cnt_tmp <= 0) // 매도대상 주식수가 0 이하라면 매도하지 않음
                    {
                        continue;
                    }

                    int l_new_target_price = 0;
                    l_new_target_price = get_hoga_unit_price(l_target_price, l_jongmok_cd, 0);

                    // 매도 호가를 구함
                    g_flag_4 = 0;
                    g_rqname = "매도주문";

                    String l_scr_no = null;
                    l_scr_no = "";
                    l_scr_no = get_scr_no();

                    int ret = 0;

                    //매도주문 요청
                    ret = axKHOpenAPI1.SendOrder("매도주문", l_scr_no, g_accnt_no, 2, l_jongmok_cd, l_sell_ord_stock_cnt_tmp, l_new_target_price, "00", "");
                    if (ret == 0)
                    {
                        write_msg_log("매도주문 Sendord() 호출 성공\n", 0);
                        write_msg_log("종목코드 : [" + l_jongmok_cd + "]\n", 0);
                    }
                    else
                    {
                        write_msg_log("매도주문 Sendord() 호출 실패\n", 0);
                        write_msg_log("i_jongmok_cd : [" + l_jongmok_cd + "]\n", 0);
                    }

                    delay(200);

                    for (; ; )
                    {
                        if (g_flag_4 == 1)
                        {
                            delay(200);
                            axKHOpenAPI1.DisconnectRealData(l_scr_no);
                            break;
                        }
                        else
                        {
                            write_msg_log("'매도주문' 완료 대기 중...\n", 0);
                            delay(200);
                            break;
                        }
                    }
                    axKHOpenAPI1.DisconnectRealData(l_scr_no);
                }
            }
            reader.Close();
            conn.Close();
        }

        public int get_sell_not_chegyul_ord_stock_cnt(string i_jongmok_cd) // 미체결 매도주문 주식 수 가져오기
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            int l_sell_not_chegyul_ord_stock_cnt = 0;

            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            // 주문내역과 체결내역 테이블 조회
            sql = @"
                SECLET
                    NVL(SUM(ORD_STOCK_CNT - CHEGYUL_STOCK_CNT), 0) SELL_NOT_CHEGYUL_ORD_STOCK_CNT
                FROM
                (
                    SELECT ORD_STOCK_CNT ORD_STOCK_CNT,
                    ( SELECT NVL(MAX(B.CHEGYUL_STOCK_CNT), 0) CHEGYUL_STOCK_CNT
                      FROM TB_CHEGYUL_LST B
                      WHERE B.USER_ID = A.USER_ID
                      AND B.ACCNT_NO = A.ACCNT_NO
                      AND B.REF_DT = A.REF_DT
                      AND B.JONGMOK_CD = A.JONGMOK_CD
                      AND B.ORD_GB = A.ORD_GB
                      AND B.ORD_NO = A.ORD_NO
                    ) CHEGYUL_STOCK_CNT
                    FROM TB_ORD_LST A
                    WHERE A.REF_DT = TO_CHAR(SYSDATE, 'YYYYMMDD')
                    AND A.USER_ID = " + "'" + g_user_id + "'" +
                    " AND A.JONGMOK_CD = " + "'" + i_jongmok_cd + "'" +
                    " AND A.ACCNT_NO = " + "'" + g_accnt_no + "'" +
                    " AND A.ORD_GB = '1' " +
                    " AND A.ORG_ORD_NO = '0000000' " +
                    " AND NOT EXISTS ( SELECT '1' " +
                    "                  FROM TB_ORD_LST B " +
                    "                  WHERE B.USER_ID = A.USER_ID " +
                    "                  AND B.ACCNT_NO = A.ACCNT_NO " +
                    "                  AND B.REF_DT = A.REF_DT " +
                    "                  AND B.JONGMOK_CD = A.JONGMOK_CD " +
                    "                  AND B.ORD_GB = A.ORD_GB " +
                    "                  AND B.ORG_ORD_NO = A.ORD_NO " +
                    "                   )) ";


            cmd.CommandText = sql;
            reader = cmd.ExecuteReader();
            reader.Read();

            l_sell_not_chegyul_ord_stock_cnt = int.Parse(reader[0].ToString()); // 미체결 매도주문 주식 수 가져오기

            reader.Close();
            conn.Close();

            return l_sell_not_chegyul_ord_stock_cnt;
        }

        public void real_cut_loss_ord() // 실시간 손절주문
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            string l_jongmok_cd = null;
            int l_cut_loss_price = 0;
            int l_own_stock_cnt = 0;
            write_msg_log("real_cut_loss_ord 시작\n", 0);

            // conn = null;
            // sql = null;
            // cmd = null;
            // reader = cull;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            //거래종목 및 계좌정보 테이블 조회
            sql = @" SELECT " +
                "    A.JONGMOK_CD, " +
                "    A.CUT_LOSS_PRICE, " +
                "    B.OWN_STOCK_CNT, " +
                " FROM " +
                "    TB_TRD_JONGMOK A " +
                "    TB_ACCNT_INFO B, " +
                " WHERE A.USER_ID = " + "'" + g_user_id + "'" +
                " AND A.JONGMOK_CD = B.JONGMOK_CD " +
                " AND B.ACCNT_NO = " + "'" + g_accnt_no + "'" +
                " AND A.REF_DT = TO_CHAR(SYSDATE, 'YYYYMMDD') " +
                " AND A.SELL_TRD_YN = 'Y' AND B.OWN_STOCK_CNT > 0 ";

            cmd.CommandText = sql;
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                l_jongmok_cd = "";
                l_cut_loss_price = 0;

                l_jongmok_cd = reader[0].ToString().Trim();
                l_cut_loss_price = int.Parse(reader[1].ToString().Trim());
                l_own_stock_cnt = int.Parse(reader[2].ToString().Trim());

                write_msg_log("종목코드 : [" + l_jongmok_cd + "]\n", 0);
                write_msg_log("종목명 : [" + get_jongmok_nm(l_jongmok_cd) + "]\n", 0);
                write_msg_log("손절가 : [" + l_cut_loss_price.ToString() + "]\n", 0);
                write_msg_log("보유주식수 : [" + l_own_stock_cnt.ToString() + "]\n", 0);

                int l_for_flag = 0;
                int l_for_cnt = 0;
                g_cur_price = 0;

                for (; ; )
                {
                    g_rqname = "";
                    g_rqname = "현재가조회";
                    g_flag_6 = 0;
                    axKHOpenAPI1.SetInputValue("종목코드", l_jongmok_cd);

                    string l_scr_no = null;
                    l_scr_no = "";
                    l_scr_no = get_scr_no();

                    //현재가 조회 요청
                    axKHOpenAPI1.CommRqData(g_rqname, "OPT10001", 0, l_scr_no);
                    
                    if (g_cur_price < l_cut_loss_price) // 현재가가 손절가 이탈 시
                    {
                        sell_canc_ord(l_jongmok_cd);

                        g_flag_4 = 0;
                        g_rqname = "매도주문";

                        l_scr_no = "";
                        l_scr_no = get_scr_no();

                        int ret = 0;
                        ret = axKHOpenAPI1.SendOrder("매도주문", l_scr_no, g_accnt_no, 2, l_jongmok_cd, l_own_stock_cnt, 0, "03", "");

                        if (ret == 0)
                        {
                            write_msg_log("매도주문 Sendord() 호출 성공\n", 0);
                            write_msg_log("종목코드 : [" + l_jongmok_cd + "]\n", 0);
                        }
                        else
                        {
                            write_msg_log("매도주문 Sendord() 호출 실패\n", 0);
                            write_msg_log("i_jongmok_cd : [" + l_jongmok_cd + "]\n", 0);
                        }
                        delay(200);

                        for(; ; )
                        {
                            if (g_flag_4 == 1)
                            {
                                delay(200);
                                axKHOpenAPI1.DisconnectRealData(l_scr_no);
                                break;
                            }
                            else
                            {
                                write_msg_log("'매도주문' 완료 대기 중...\n", 0);
                                delay(200);
                                break;
                            }
                        }
                        axKHOpenAPI1.DisconnectRealData(l_scr_no);
                        update_tb_trd_jongmok(l_jongmok_cd);
                    }
                    try
                    {
                        l_for_cnt = 0;
                        for (; ; )
                        {
                            if (g_flag_6 == 1)
                            {
                                delay(200);
                                axKHOpenAPI1.DisconnectRealData(l_scr_no);
                                l_for_flag = 1;
                                break;
                            }
                            else
                            {
                                write_msg_log("'현재가조회' 완료 대기 중...\n", 0);
                                delay(200);
                                l_for_cnt++;
                                if (l_for_cnt == 5)
                                {
                                    l_for_flag = 0;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        write_err_log("real_cut_loss_ord() 현재가조회 ex.Message : [" + ex.Message + "]\n", 0);
                    }

                    axKHOpenAPI1.DisconnectRealData(l_scr_no);

                    if (l_for_flag == 1)
                    {
                        break;
                    }
                    else if (l_for_flag == 0)
                    {
                        delay(200);
                        continue;
                    }
                    delay(200);
                }
            }
            reader.Close();
            conn.Close();
        }

        public void sell_canc_ord(string i_jongmok_cd)
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            string l_rid = null;
            string l_jongmok_cd = null;
            int l_ord_stock_cnt = 0;
            int l_ord_price = 0;
            string l_ord_no = null;
            string l_org_ord_no = null;

            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            // 주문내역과 체결내역 테이블 조회
            sql = @"
                SECLET
                    ROWID RID, JONGMOK_CD, (ORD_STOCK_CNT -
                    ( SELECT NVL(MAX(B.CHEGYUL_STOCK_CNT), 0) CHEGYUL_STOCK_CNT
                      FROM TB_CHEGYUL_LST B
                      WHERE B.USER_ID = A.USER_ID
                      AND B.ACCNT_NO = A.ACCNT_NO
                      AND B.REF_DT = A.REF_DT
                      AND B.JONGMOK_CD = A.JONGMOK_CD
                      AND B.ORD_GB = A.ORD_GB
                      AND B.ORD_NO = A.ORD_NO
                    ) ) SELL_NOT_CHEGYUL_ORD_STOCK_CNT, ORD_PRICE, ORD_N0, ORG_ORD_NO
                FROM TB_ORD_LST A
                WHERE A.REF_DT = TO_CHAR(SYSDATE, 'YYYYMMDD')
                AND A.USER_ID = " + "'" + g_user_id + "'" +
                " AND A.ACCNT_NO = " + "'" + g_accnt_no + "'" +
                " AND A.JONGMOK_CD = " + "'" + i_jongmok_cd + "'" +
                " AND A.ORD_GB = '1' " +
                " AND A.ORG_ORD_NO = '0000000' " +
                " AND NOT EXISTS ( SELECT '1' " +
                "                  FROM TB_ORD_LST B " +
                "                  WHERE B.USER_ID = A.USER_ID " +
                "                  AND B.ACCNT_NO = A.ACCNT_NO " +
                "                  AND B.REF_DT = A.REF_DT " +
                "                  AND B.JONGMOK_CD = A.JONGMOK_CD " +
                "                  AND B.ORD_GB = A.ORD_GB " +
                "                  AND B.ORD_NO = A.ORD_NO " +
                "                )";


            cmd.CommandText = sql;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                l_rid = "";
                l_jongmok_cd = "";
                l_ord_stock_cnt = 0;
                l_ord_price = 0;
                l_ord_no = "";
                l_org_ord_no = "";

                l_rid = reader[0].ToString().Trim();
                l_jongmok_cd = reader[1].ToString().Trim();
                l_ord_stock_cnt = int.Parse(reader[2].ToString().Trim());
                l_ord_price = int.Parse(reader[3].ToString().Trim());
                l_ord_no = reader[4].ToString().Trim();
                l_org_ord_no = reader[5].ToString().Trim();

                g_flag_5 = 0;
                g_rqname = "매도취소주문";

                String l_scr_no = null;
                l_scr_no = "";
                l_scr_no = get_scr_no();

                int ret = 0;

                //매도취소주문 요청
                ret = axKHOpenAPI1.SendOrder("매도취소주문", l_scr_no, g_accnt_no, 4, l_jongmok_cd, l_ord_stock_cnt, 0, "03", l_ord_no);
                if (ret == 0)
                {
                    write_msg_log("매도취소주문 Sendord() 호출 성공\n", 0);
                    write_msg_log("종목코드 : [" + l_jongmok_cd + "]\n", 0);
                }
                else
                {
                    write_msg_log("매도취소주문 Sendord() 호출 실패\n", 0);
                    write_msg_log("i_jongmok_cd : [" + l_jongmok_cd + "]\n", 0);
                }

                delay(200);

                for (; ; )
                {
                    if (g_flag_5 == 1)
                    {
                        delay(200);
                        axKHOpenAPI1.DisconnectRealData(l_scr_no);
                        break;
                    }
                    else
                    {
                        write_msg_log("'매도취소주문' 완료 대기 중...\n", 0);
                        delay(200);
                        break;
                    }
                }
                axKHOpenAPI1.DisconnectRealData(l_scr_no);
                delay(1000);
            }

            reader.Close();
            conn.Close();
        }

        public void update_tb_trd_jongmok(String i_jongmok_cd)
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String l_sql = null;

            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            l_sql = @" update TB_TRD_JONGMOK set buy_trd_yn = 'N', updt_dtm = SYSDATE, updt_id = 'ats' " +
                " WHERE user_id = " + "'" + g_user_id + "'" +
                " AND jongmok_cd = " + "'" + i_jongmok_cd + "'";

            cmd.CommandText = l_sql;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                write_err_log("update TB_TRD_JONGMOK ex.Message : [" + ex.Message + "]\n", 0);
            }
            conn.Close();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)  // 추가기능 뉴스기사 업데이트 버튼 클릭시
        {
            try
            {
                string results = getResults();
                results = results.Replace("<b>", "");
                results = results.Replace("</b>", "");
                results = results.Replace("&lt;", "<");
                results = results.Replace("&gt;", ">");

                var parseJson = JObject.Parse(results);
                var countsOfDisplay = Convert.ToInt32(parseJson["display"]);
                var countsOfResults = Convert.ToInt32(parseJson["total"]);

                ResultList.Items.Clear();
                for (int i = 0; i < countsOfDisplay; i++)
                {
                    ListViewItem item = new ListViewItem((i + 1).ToString());

                    var title = parseJson["items"][i]["title"].ToString();
                    title = title.Replace("&quot;", "\"");

                    var description = parseJson["items"][i]["description"].ToString();
                    description = description.Replace("&quot;", "\"");

                    var link = parseJson["items"][i]["link"].ToString();

                    item.SubItems.Add(title);
                    item.SubItems.Add(description);
                    item.SubItems.Add(link);

                    ResultList.Items.Add(item);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
        }

        private string getResults()                                          // 뉴스 기사 업데이트에 쓰이는 결과값 받아오기 함수임
        {
            string keyword = Searchbox.Text;
            string display = btn_DisplayCount.Value.ToString();
            string sort = "sim";
            if (btn_date.Checked == true)
                sort = "date";

            string query = string.Format("?query={0}&display={1}sort={2}", keyword, display, sort);

            WebRequest request = WebRequest.Create(_apiUrl + query);
            request.Headers.Add("X-Naver-Client-Id", "I74lzNbMOpmIlEsfaWRO");
            request.Headers.Add("X-Naver-Client-Secret", "jgntupzVD8");

            string requestResult = "";
            using (var response = request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        requestResult = reader.ReadToEnd();
                    }
                }
            }

            return requestResult;
        }

        private void trackBarDisplayCounts_Scroll(object sender, EventArgs e) // 이벤트 속성 추가 완료
        {
            labelDisplayCounts.Text = btn_DisplayCount.Value.ToString();
        }

    }
}
