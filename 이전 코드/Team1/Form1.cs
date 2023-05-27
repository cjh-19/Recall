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
                    // 계좌조회, 계죄정보 조회, 보유종목 매도주문 수행
                    // 챕터10 이후부터 구현 파트
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

                        delay(200); // 첫 번째 무한루프 지연

                    }
                }

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
