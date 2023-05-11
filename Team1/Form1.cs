using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using System.Runtime.ExceptionServices;
using System.Security;

namespace Team1
{
    public partial class Form1 : Form
    {
        string g_user_id = null;
        string g_accnt_no = null;
        int g_scr_no = 0; //OpenAPI 요청번호
        public Form1()
        {
            InitializeComponent();
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

        [HandleProcessCorruptedStateExceptions]  //처음 이용해보는 형태
        [SecurityCritical]
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
                catch(AccessViolationException e)
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
        private OracleConnection connect_db()  //오라클 접속 연결 함수(수정필요)
        {
            String conninfo = "User Id=c##team";
            String Password = "1234"; //이것도 타입 모르겠어서 일단 스트링 설정
            //Data Source = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=xe1)) );"; // 타입 알 수 없음

            OracleConnection conn = new OracleConnection(conninfo); //오라클 연결 인스턴스

            try
            {
                conn.Open(); //접속
            }
            catch(Exception e)
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
    }
}
