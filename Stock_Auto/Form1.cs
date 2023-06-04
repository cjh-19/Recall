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
        string g_user_id = null; // 키움증권 아이디
        string g_accnt_no = null; // 증권 계좌번호
        int g_scr_no = 0; //OpenAPI 요청번호

        int g_is_thread = 0; // 0이면 스레드 미생성, 1이면 스레드 생성
        Thread thread1 = null; // 생성된 스레드 객체를 담을 변수

        string g_rqname = null; // 요청명 저장 변수

        const string _apiUrl = "https://openapi.naver.com/v1/search/news";   //추가기능 뉴스 api
        const string _clientId = "I74lzNbMOpmIlEsfaWRO";
        const string _clientSecret = "73vhwkbPBc";

        int g_is_next = 0; // 다음 조회 데이터가 있는지 확인하는 플래그
        int g_flag_1 = 0; // 1이면 요청에 대한 응답 완료
        int g_flag_2 = 0; // 1이면 요청에 대한 응답 완료
        int g_flag_3 = 0; // 매수주문 응답 플래그
        int g_flag_4 = 0; // 매도주문 응답 플래그
        int g_flag_5 = 0; // 매도취소주문 응답 플래그
        int g_cur_price = 0; // 현재가
        int g_flag_6 = 0; // 1이면 조회 완료
        int g_buy_hoga = 0; // 최우선 매수호가 저장
        int g_flag_7 = 0; // 최우선 매수호가 플래그 변수가 1이면 조회 완료

        int g_ord_amt_possible; //총 매수 가능 금액
        public Form1()
        {
            InitializeComponent();
            // 데이터 수신 요청에 대한 응답을 받는 대기 이벤트 메서드 선언
            this.axKHOpenAPI1.OnReceiveTrData += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEventHandler(this.axKHOpenAPI1_OnReceiveTrData);
            this.axKHOpenAPI1.OnReceiveMsg += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveMsgEventHandler(this.axKHOpenAPI1_OnReceiveMsg);
            this.axKHOpenAPI1.OnReceiveChejanData += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEventHandler(this.axKHOpenAPI1_OnReceiveChejanData);
            ResultList.MouseDoubleClick += new MouseEventHandler(ResultList_MouseDoubleClick);
        }
        
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
                return;
            }

            if (e.sRQName == "증거금세부내역조회요청") //응답받은 요청명이 증거금세부내역조회요청이라면
            {
                g_ord_amt_possible = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "100주문가능금액").Trim()); //주문 가능 금액을 저장
                axKHOpenAPI1.DisconnectRealData(e.sScrNo);
                g_flag_1 = 1;
            }

            if (e.sRQName == "계좌평가현황요청") //응답받은 요청명이 '계좌평가현황요청'이라면
            {
                int repeat_cnt = 0;
                int ii = 0;
                String user_id = null;
                String jongmok_cd = null;
                String jongmok_nm = null;

                int own_stock_cnt = 0;
                int buy_price = 0;
                int own_amt = 0;

                repeat_cnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName); // 보유 종목수 가져오기

                write_msg_log("TB_ACCNT_INFO 테이블 설정 시작\n", 0);
                write_msg_log("보유종목수 : " + repeat_cnt.ToString() + "\n", 0);

                for (ii = 0; ii < repeat_cnt; ii++)
                {
                    user_id = "";
                    jongmok_cd = "";
                    own_stock_cnt = 0;
                    buy_price = 0;
                    own_amt = 0;

                    user_id = g_user_id;
                    jongmok_cd = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, ii, "종목코드").Trim().Substring(1, 6);
                    jongmok_nm = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, ii, "종목명").Trim();
                    own_stock_cnt = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, ii, "보유수량").Trim());
                    buy_price = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, ii, "평균단가").Trim());
                    own_amt = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, ii, "매입금액").Trim());

                    write_msg_log("종목코드 : " + jongmok_cd + "\n", 0);
                    write_msg_log("종목명 : " + jongmok_nm + "\n", 0);
                    write_msg_log("보유주식수 : " + own_stock_cnt.ToString() + "\n", 0);

                    if (own_stock_cnt == 0)//보유주식수가 0이라면 저장하지 않음
                    {
                        continue;
                    }

                    insert_tb_accnt_info(jongmok_cd, jongmok_nm, buy_price, own_stock_cnt, own_amt); // 계좌정보에 테이블 저장
                }
                write_msg_log("TB_ACCNT_INFO 테이블 설정 완료\n", 0);
                axKHOpenAPI1.DisconnectRealData(e.sScrNo);

                if (e.sPrevNext.Length == 0)
                {
                    g_is_next = 0;
                }
                else
                {
                    g_is_next = int.Parse(e.sPrevNext);
                }
                g_flag_2 = 1;
            }

            if (e.sRQName == "호가조회")
            {
                int cnt = 0;
                int ii = 0;
                int l_buy_hoga = 0;

                cnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                for (ii = 0; ii < cnt; ii++)
                {
                    l_buy_hoga = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, ii, "매수최우선호가").Trim());
                    l_buy_hoga = System.Math.Abs(l_buy_hoga);
                }

                g_buy_hoga = l_buy_hoga;

                axKHOpenAPI1.DisconnectRealData(e.sScrNo);
                g_flag_7 = 1;
            }

            if (e.sRQName == "현재가조회") // 응답받은 요청명이 현재가조회
            {
                g_cur_price = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "현재가").Trim());
                g_cur_price = System.Math.Abs(g_cur_price);
                axKHOpenAPI1.DisconnectRealData(e.sScrNo);
                g_flag_6 = 1;
            }
        } //axKHOpenAPI1_OnReceiveTrData 매서드 종료

        // 주식주문을 요청할 때 해당 주식 주문의 응답을 수신하는 이벤트 메서드
        private void axKHOpenAPI1_OnReceiveMsg(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            if (e.sRQName == "매수주문")
            {
                write_msg_log("\n========매수주문 원장 응답정보 출력 시작========\n", 0);
                write_msg_log("sScrNo : [" + e.sScrNo + "]" + "\n", 0);
                write_msg_log("sRQName : [" + e.sRQName + "]" + "\n", 0);
                write_msg_log("sTrCode : [" + e.sTrCode + "]" + "\n", 0);
                write_msg_log("sMsg : [" + e.sMsg + "]" + "\n", 0);
                write_msg_log("=========매수주문 원장 응답정보 출력 종료========\n", 0);
                g_flag_3 = 1; //매수주문 응답완료 설정
            }

            if (e.sRQName == "매도주문")
            {
                write_msg_log("\n========매도주문 원장 응답정보 출력 시작========\n", 0);
                write_msg_log("sScrNo : [" + e.sScrNo + "]" + "\n", 0);
                write_msg_log("sRQName : [" + e.sRQName + "]" + "\n", 0);
                write_msg_log("sTrCode : [" + e.sTrCode + "]" + "\n", 0);
                write_msg_log("sMsg : [" + e.sMsg + "]" + "\n", 0);
                write_msg_log("=========매도주문 원장 응답정보 출력 종료========\n", 0);
                g_flag_4 = 1; //매수주문 응답완료 설정
            }
            if (e.sRQName == "매도취소주문")
            {
                write_msg_log("\n========매도취소주문 원장 응답정보 출력 시작========\n", 0);
                write_msg_log("sScrNo : [" + e.sScrNo + "]" + "\n", 0);
                write_msg_log("sRQName : [" + e.sRQName + "]" + "\n", 0);
                write_msg_log("sTrCode : [" + e.sTrCode + "]" + "\n", 0);
                write_msg_log("sMsg : [" + e.sMsg + "]" + "\n", 0);
                write_msg_log("=========매도취소주문 원장 응답정보 출력 종료========\n", 0);
                g_flag_5 = 1; //매도취소주문 응답완료 설정
            }
        }
        // 주식주문을 요청한 후 주문내역과 체결내역 데이터를 수신하는 이벤트 메서드
        private void axKHOpenAPI1_OnReceiveChejanData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            if (e.sGubun == "0")//sGubun의 값이 "0" 이라면 주문내역 및 체결내역 수신
            {
                String chejan_gb = "";
                chejan_gb = axKHOpenAPI1.GetChejanData(913).Trim(); //주문내역인지 체결내역인지 가져옴
                if (chejan_gb == "접수") //chejan_gb 의 값이 "접수"라면 주문내역
                {
                    String user_id = null;
                    String jongmok_cd = null;
                    String jongmok_nm = null;
                    String ord_gb = null;
                    String ord_no = null;
                    String org_ord_no = null;
                    string ref_dt = null;
                    int ord_price = 0;
                    int ord_stock_cnt = 0;
                    int ord_amt = 0;
                    String ord_dtm = null;

                    user_id = g_user_id;
                    jongmok_cd = axKHOpenAPI1.GetChejanData(9001).Trim().Substring(1, 6);
                    jongmok_nm = get_jongmok_nm(jongmok_cd);
                    ord_gb = axKHOpenAPI1.GetChejanData(907).Trim();
                    ord_no = axKHOpenAPI1.GetChejanData(9203).Trim();
                    org_ord_no = axKHOpenAPI1.GetChejanData(904).Trim();
                    ord_price = int.Parse(axKHOpenAPI1.GetChejanData(901).Trim());
                    ord_stock_cnt = int.Parse(axKHOpenAPI1.GetChejanData(900).Trim());
                    ord_amt = ord_price * ord_stock_cnt;

                    DateTime CurTime;
                    String CurDt;
                    CurTime = DateTime.Now;
                    CurDt = CurTime.ToString("yyyy") + CurTime.ToString("MM") + CurTime.ToString("dd");

                    ref_dt = CurDt;

                    ord_dtm = CurDt + axKHOpenAPI1.GetChejanData(908).Trim();

                    write_msg_log("종목코드 : [" + jongmok_cd + "]" + "\n", 0);
                    write_msg_log("종목명 : [" + jongmok_nm + "]" + "\n", 0);
                    write_msg_log("주문구분 : [" + ord_gb + "]" + "\n", 0);
                    write_msg_log("주문번호 : [" + ord_no + "]" + "\n", 0);
                    write_msg_log("원주문번호 : [" + org_ord_no + "]" + "\n", 0);
                    write_msg_log("주문금액 : [" + ord_price.ToString() + "]" + "\n", 0);
                    write_msg_log("주문주식수 : [" + ord_stock_cnt.ToString() + "]" + "\n", 0);
                    write_msg_log("주문금액 : [" + ord_amt.ToString() + "]" + "\n", 0);
                    write_msg_log("주문일시 : [" + ord_dtm + "]" + "\n", 0);

                    insert_tb_ord_lst(ref_dt, jongmok_cd, jongmok_nm, ord_gb, ord_no,
                        org_ord_no, ord_price, ord_stock_cnt, ord_amt, ord_dtm); //주문내역 저장

                    if (ord_gb == "2") //매수주문일경우
                    {
                        update_tb_accnt(ord_gb, ord_amt);
                    }

                } //"if(chejan_gb == "접수")" 종료
                else if (chejan_gb == "체결") //chejan_gb의 값이 "체결"이라면 체결내역
                {
                    String user_id = null;
                    String jongmok_cd = null;
                    String jongmok_nm = null;
                    String chegyul_gb = null;
                    int chegyul_no = 0;
                    int chegyul_price = 0;
                    int chegyul_cnt = 0;
                    int chegyul_amt = 0;
                    String chegyul_dtm = null;
                    String ord_no = null;
                    String org_ord_no = null;
                    string ref_dt = null;

                    user_id = g_user_id;
                    jongmok_cd = axKHOpenAPI1.GetChejanData(9001).Trim().Substring(1, 6);
                    jongmok_nm = get_jongmok_nm(jongmok_cd);
                    chegyul_gb = axKHOpenAPI1.GetChejanData(907).Trim(); //2:매수 1:매도
                    chegyul_no = int.Parse(axKHOpenAPI1.GetChejanData(909).Trim());
                    chegyul_price = int.Parse(axKHOpenAPI1.GetChejanData(910).Trim());
                    chegyul_cnt = int.Parse(axKHOpenAPI1.GetChejanData(911).Trim());
                    chegyul_amt = chegyul_price * chegyul_cnt;
                    org_ord_no = axKHOpenAPI1.GetChejanData(904).Trim();

                    DateTime CurTime;
                    String CurDt;
                    CurTime = DateTime.Now;
                    CurDt = CurTime.ToString("yyyy") + CurTime.ToString("MM") + CurTime.ToString("dd");

                    ref_dt = CurDt;

                    chegyul_dtm = CurDt + axKHOpenAPI1.GetChejanData(908).Trim();
                    ord_no = axKHOpenAPI1.GetChejanData(9203).Trim();

                    write_msg_log("종목코드 : [" + jongmok_cd + "]" + "\n", 0);
                    write_msg_log("종목명 : [" + jongmok_nm + "]" + "\n", 0);
                    write_msg_log("체결구분 : [" + chegyul_gb + "]" + "\n", 0);
                    write_msg_log("체결번호 : [" + chegyul_no.ToString() + "]" + "\n", 0);
                    write_msg_log("체결가 : [" + chegyul_price.ToString() + "]" + "\n", 0);
                    write_msg_log("체결주식수 : [" + chegyul_cnt.ToString() + "]" + "\n", 0);
                    write_msg_log("체결금액 : [" + chegyul_amt.ToString() + "]" + "\n", 0);
                    write_msg_log("체결일시 : [" + chegyul_dtm + "]" + "\n", 0);
                    write_msg_log("주문번호 : [" + ord_no + "]" + "\n", 0);
                    write_msg_log("원주문번호 : [" + org_ord_no + "]" + "\n", 0);

                    insert_tb_chegyul_lst(ref_dt, jongmok_cd, jongmok_nm, chegyul_gb,
                        chegyul_no, chegyul_price, chegyul_cnt, chegyul_amt, chegyul_dtm, ord_no, org_ord_no); //체결내역 저장
                    if (chegyul_gb == "1") //매도체결이라면 계좌테이블의 매수가능 금액을 늘려줌
                    {
                        update_tb_accnt(chegyul_gb, chegyul_amt);
                    }

                }//else if (chejan_gb = "체결") 종료
            } //if(e.Gubun == "0")종료

            if (e.sGubun == "1")  // sGubun 이 1이면 계좌정보 수신
            {
                String user_id = null;
                String jongmok_cd = null;

                int boyu_cnt = 0;
                int boyu_price = 0;
                int boyu_amt = 0;

                user_id = g_user_id;
                jongmok_cd = axKHOpenAPI1.GetChejanData(9001).Trim().Substring(1, 6);
                boyu_cnt = int.Parse(axKHOpenAPI1.GetChejanData(930).Trim());
                boyu_price = int.Parse(axKHOpenAPI1.GetChejanData(931).Trim());
                boyu_amt = int.Parse(axKHOpenAPI1.GetChejanData(932).Trim());

                String l_jongmok_nm = null;
                l_jongmok_nm = get_jongmok_nm(jongmok_cd);

                write_msg_log("종목코드 : [" + jongmok_cd + "]" + "\n", 0);
                write_msg_log("보유주식수 : [" + boyu_cnt.ToString() + "]" + "\n", 0);
                write_msg_log("보유가 : [" + boyu_price.ToString() + "]" + "\n", 0);
                write_msg_log("보유금액 : [" + boyu_amt.ToString() + "]" + "\n", 0);

                merge_tb_accnt_info(jongmok_cd, l_jongmok_nm, boyu_cnt, boyu_price, boyu_amt); //계좌정보(보유종목) 저장
            }// if(e.sGubun==1) 종료
        } // 메서드 종료


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

        

        private void accountbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            g_accnt_no = accountbox.SelectedItem.ToString().Trim();
            write_msg_log("사용할 증권계좌 번호는 : [" + g_accnt_no + "] 입니다. \n", 0);
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
            int l_target_price;
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
            l_target_price = 0;
            l_cut_loss_price = 0;
            l_buy_trd_yn = "";
            l_sell_trd_yn = "";
            //l_seq = 0; 

            while (reader.Read())
            {
                l_seq++;
                l_jongmok_cd = "";
                l_jongmok_nm = "";
                l_priority = 0;
                l_buy_amt = 0;
                l_buy_price = 0;
                l_target_price = 0;
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
                l_target_price = int.Parse(reader[5].ToString().Trim());
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
                    l_target_price.ToString(),
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
            int l_target_price;
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

                    l_target_price = int.Parse(Row.Cells[6].Value.ToString());
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
                        + l_target_price + ","
                        + l_cut_loss_price + "," +
                        "'" + l_buy_trd_yn + "'" + "," +
                        "'" + l_sell_trd_yn + "'" + "," +
                        "'" + g_user_id + "'" + "," +
                        "sysdate " + "," +
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
            int l_target_price;
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

                    l_target_price = int.Parse(Row.Cells[6].Value.ToString());
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
                        " TARGET_PRICE = " + l_target_price + "," +
                        " CUT_LOSS_PRICE = " + l_cut_loss_price + "," +
                        " BUY_TRD_YN = " + "'" + l_buy_trd_yn + "'" + "," +
                        " SELL_TRD_YN = " + "'" + l_sell_trd_yn + "'" + "," +
                        " UPDT_ID = " + "'" + g_user_id + "'" + "," +
                        " UPDT_DTM = SYSDATE" + " WHERE JONGMOK_CD = " + "'" + l_jongmok_cd + "'" +
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
        public void m_thread1() // 스레드 매서드
        {
            string l_cur_tm = null;

            int l_set_tb_accnt_flag = 0; // 1이면 호출 완료
            int l_set_tb_accnt_info_flag = 0; // 1이면 호출 완료
            int l_sell_ord_first_flag = 0; //1이면 호출 완료

            // 최초 스레드 생성 파트
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
                    // 계좌조회, 계좌정보 조회, 보유종목 매도주문 수행
                    // 계좌조회
                    if (l_set_tb_accnt_flag == 0) // 계좌조회 호출 전
                    {
                        set_tb_accnt(); // 호출
                        l_set_tb_accnt_flag = 1; //호출로 설정
                    }
                    // 계좌정보 조회
                    if (l_set_tb_accnt_info_flag == 0)
                    {
                        set_tb_accnt_info(); // 계좌정보 테이블 설정
                        l_set_tb_accnt_info_flag = 1;
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
                        real_buy_ord(); // 실시간 매수 주문 매서드 호출

                        delay(200); // 두 번째 무한루프 지연, 0.2초 딜레이
                        real_sell_ord(); // 실시간 매도 주문 매서드 호출

                        delay(200); // 0.2초 딜레이
                        real_cut_loss_ord(); // 실시간 손절 주문 매서드 호출

                        delay(200); // 일단 시험삼아 추가
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
                thread1.Abort(); // 스레드 중지
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

        public void set_tb_accnt() //계좌 테이블 세팅 메서드
        {
            int l_for_cnt = 0;
            int l_for_flag = 0;

            write_msg_log("TB_ACCNT 테이블 세팅 시작\n", 0);

            g_ord_amt_possible = 0; // 매수 가능 금액

            l_for_flag = 0;
            for (; ; )
            {
                // 계좌 정보를 입력
                axKHOpenAPI1.SetInputValue("계좌번호", g_accnt_no);
                axKHOpenAPI1.SetInputValue("비밀번호", "");

                // 요청명을 설정하고, 요청중 상태를 표시하는 변수를 초기화
                g_rqname = "";
                g_rqname = "증거금세부내역조회요청"; // 요청명 정의
                g_flag_1 = 0; //요청중

                String l_scr_no = null; //화면번호를 담을 변수 선언
                l_scr_no = "";
                l_scr_no = get_scr_no();//화면번호 채번
                axKHOpenAPI1.CommRqData("증거금세부내역조회요청", "opw00013", 0, l_scr_no); // Open API로 데이터 요청

                l_for_cnt = 0;
                for (; ; ) // 요청후 대기시작
                {
                    if (g_flag_1 == 1) //요청에 대한 응답이 완료되면 루프를 빠져나옴
                    {
                        delay(1000);
                        axKHOpenAPI1.DisconnectRealData(l_scr_no);
                        l_for_flag = 1;
                        break;
                    }
                    else //아직 요청에 대한 응답이 오지 않을 경우
                    {
                        write_msg_log("'증거금세부내역조회' 완료 대기 중...\n", 0);
                        delay(1000);
                        l_for_cnt++;
                        if (l_for_cnt == 1) // 한번이라도 실패하면 무한루프를 빠져나감(증권계좌 비밀번호 오류 방지)
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
                axKHOpenAPI1.DisconnectRealData(l_scr_no);//화면 접속 해제
                if (l_for_flag == 1) //요청에 대한 응답을 받았으므로 무한루프에서 빠져나옴
                {
                    break;
                }
                else if (l_for_flag == 0) //요청에 대한 응답을 받지 못해도 비번 5회 오류 방지를 위해 무한 루프에서 빠져나옴
                {
                    delay(1000);
                    break; //비밀번호 5회 오류 방지
                }
                delay(1000);
            }
            write_msg_log("주문가능금액 : [" + g_ord_amt_possible.ToString() + "]\n", 0);
            merge_tb_accnt(g_ord_amt_possible);
        }//set_tb_accnt 종료

        public void merge_tb_accnt(int g_ord_amt_possible) //계좌정보 테이블 세팅 메서드
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String l_sql = null;

            l_sql = null;
            cmd = null;
            conn = null;
            conn = connect_db(); // DBMS 연결 설정

            // 연결 성공한 경우 다음 단계로 넘어감
            if (conn != null)
            {
                cmd = new OracleCommand();
                cmd.Connection = conn; // OracleCommand 객체에 연결 설정
                cmd.CommandType = CommandType.Text;

                // 쿼리문 수정
                l_sql = @"merge into tb_accnt a
            using(
            select nvl(max(user_id), ' ') user_id, nvl(max(accnt_no), ' ') accnt_no, nvl(max(ref_dt),' ') ref_dt " +
                    " from tb_accnt " +
                    " where user_id = '" + g_user_id + "'" +
                    " and accnt_no = " + "'" + g_accnt_no + "'" +
                    " and ref_dt = to_char(sysdate, 'yyyymmdd') " +
                    " ) b " +
                    " on ( a.user_id = b.user_id and a.accnt_no = b.accnt_no and a.ref_dt = b.ref_dt) " +
                    " when matched then update " +
                    " set ord_possible_amt = " + g_ord_amt_possible + "," +
                    " updt_dtm = SYSDATE" + "," +
                    " updt_id = 'c##team'" +
                    " when not matched then insert (a.user_id, a.accnt_no, a.ref_dt, a.ord_possible_amt, a.inst_dtm, a.inst_id) values ( " +
                    "'" + g_user_id + "'" + ", " +
                    "'" + g_accnt_no + "'" + ", " +
                    " to_char(sysdate, 'yyyymmdd') " + ", "
                    + g_ord_amt_possible + ", " +
                    " SYSDATE, " +
                    "'c##team'" +
                    " )";
                cmd.CommandText = l_sql;

                // SQL 명령문 실행 중 예외가 발생할 경우 처리
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    write_err_log("merge_tb_accnt() ex : [" + ex.Message + "]\n", 0);
                }
                finally
                {
                    conn.Close(); // DB 연결 종료
                }
            }
            else
            {
                // DB 연결 실패 시 로그 기록
                write_msg_log("db connection check!\n", 0);
            }
        }

        public void set_tb_accnt_info() //계좌정보 테이블 설정
        // 이 메서드는 TB_ACCnT_INFO 테이블을 삭제한뒤
        // Open API 의 setInputValue 함수를 이용하여 입력값을 설정하고
        // CommRqData 함수를 호출햐여 계좌정보를 요청한다.
        {
            OracleCommand cmd;
            OracleConnection conn;
            String sql;
            int l_for_cnt = 0;
            int l_for_flag = 0;

            sql = null;
            cmd = null;

            conn = null;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            sql = @"delete from tb_accnt_info where ref_dt = to_char(sysdate, 'yyyymmdd') and user_id = " + "'" + g_user_id + "'"; //당일 기준 계좌 정보 삭제

            cmd.CommandText = sql;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                write_err_log("delete tb_accnt_info ex.Message : [" + ex.Message + "]\n", 0);
            }
            conn.Close();

            g_is_next = 0;
            for (; ; ) // 계좌 평가 현황 정보 요청 및 수신 루프 시작
            {
                l_for_flag = 0;
                for (; ; )
                {
                    // 계좌 평가 현황 정보 요청
                    axKHOpenAPI1.SetInputValue("계좌번호", g_accnt_no);
                    axKHOpenAPI1.SetInputValue("비밀번호", "");
                    axKHOpenAPI1.SetInputValue("상장폐지조회구분", "1");
                    axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");

                    g_flag_2 = 0;
                    g_rqname = "계좌평가현황요청";

                    String l_scr_no = get_scr_no();

                    axKHOpenAPI1.CommRqData("계좌평가현황요청", "OPW00004", g_is_next, l_scr_no); //axKHOpenAPI1.OnRecieveData 호출

                    // 계좌 평가 현황 정보 수신 대기 루프 시작
                    l_for_cnt = 0;
                    for (; ; )
                    {
                        //데이터 수신이 완료되면 일정 시간을 지연시키고,
                        //데이터 스트림을 끊어서 이 루프를 종료하고
                        //다음 페이지의 데이터를 요청하도록 설정한다.
                        if (g_flag_2 == 1)
                        {
                            delay(1000);
                            axKHOpenAPI1.DisconnectRealData(l_scr_no);
                            l_for_flag = 1;
                            break;
                        }
                        else
                        {
                            delay(1000);
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
                    delay(1000);
                    axKHOpenAPI1.DisconnectRealData(l_scr_no);

                    // 다음 페이지 데이터 요청
                    if (l_for_flag == 1)
                    {
                        break;
                    }
                    else if (l_for_flag == 0)
                    {
                        delay(1000);
                        continue;
                    }
                }

                // 모든 페이지 데이터 요청 완료
                if (g_is_next == 0)
                {
                    break;
                }
                delay(1000);
            }
        }

        public void insert_tb_accnt_info(string i_jongmok_cd, string i_jongmok_nm, int i_buy_price, int i_own_stock_cnt, int i_own_amt) //계좌정보 테이블 삽입
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String l_sql = null;

            l_sql = null;
            cmd = null;
            conn = null;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            //계좌정보 테이블 삽입
            l_sql = @" insert into tb_accnt_info values ( " +
                "'" + g_user_id + "'" + "," +
                "'" + g_accnt_no + "'" + "," +
                "to_char(sysdate, 'yyyymmdd')" + ", " +
                "'" + i_jongmok_cd + "'" + ", " +
                "'" + i_jongmok_nm + "'" + ", "
                +i_buy_price + ", "
                +i_own_stock_cnt + ", "
                +i_own_amt + ", " +
                "'c##team'" + ", " +
                " SYSDATE" + "," + "null" + "," + "null" + ") ";

            cmd.CommandText = l_sql;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                write_err_log("insert tb_accnt_info() insert tb_accnt_info ex.Message : [" + ex.Message + "]\n", 0);
            }
            conn.Close();
        }

        public void insert_tb_ord_lst(string i_ref_dt, String i_jongmok_cd, String i_jongmok_nm, String i_ord_gb, String i_ord_no, String i_org_ord_no,
            int i_ord_price, int i_ord_stock_cnt, int i_ord_amt, String i_ord_dtm) // 주문 내역 저장 매서드
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String l_sql = null;

            l_sql = null;
            cmd = null;
            conn = null;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            //주문내역 저장
            l_sql = @" insert into tb_ord_lst values ( " +
                "'" + g_user_id + "'" + "," +
                "'" + g_accnt_no + "'" + "," +
                "'" + i_ref_dt + "'" + "," +
                "'" + i_jongmok_cd + "'" + "," +
                "'" + i_jongmok_nm + "'" + "," +
                "'" + i_ord_gb + "'" + "," +
                "'" + i_ord_no + "'" + "," +
                "'" + i_org_ord_no + "'" + ","
                + i_ord_price + ","
                + i_ord_stock_cnt + ","
                + i_ord_amt + "," +
                "'" + i_ord_dtm + "'" + "," +
                "'c##team'" + "," +
                "SYSDATE" + "," +
                "null" + "," +
                "null" + ") ";

            cmd.CommandText = l_sql;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                write_err_log("insert tb_ord_lst ex : [" + ex.Message + "] \n", 0);
            }

            conn.Close();

        }

        public void update_tb_accnt(String i_chegyul_gb, int i_chegyul_amt) //계좌테이블 수정 메서드
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String l_sql = null;

            l_sql = null;

            cmd = null;
            conn = null;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            if (i_chegyul_gb == "2") // 매수일떄 주문 가능 금액에서 체결금액 빼기
            {
                l_sql = @" update TB_ACCNT set ORD_POSSIBLE_AMT = ord_possible_amt - "
                        + i_chegyul_amt + ", updt_dtm = SYSDATE, updt_id = 'c##team' " +
                        " where user_id = " + "'" + g_user_id + "'" +
                        " and accnt_no = " + "'" + g_accnt_no + "'" +
                        " and ref_dt = to_char(sysdate, 'yyyymmdd') ";
            }
            else if (i_chegyul_gb == "1") //매도인 경우 주문가능금액에서 체결금액을 더하기
            {
                l_sql = @" update TB_ACCNT set ORD_POSSIBLE_AMT = ord_possible_amt + "
                        + i_chegyul_amt + ", updt_dtm = SYSDATE, updt_id = 'c##team' " +
                        " where user_id = " + "'" + g_user_id + "'" +
                        " and accnt_no = " + "'" + g_accnt_no + "'" +
                        " and ref_dt = to_char(sysdate, 'yyyymmdd') ";
            }

            cmd.CommandText = l_sql;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                write_err_log("update TB_ACCNT ex.Message: [" + ex.Message + "]\n", 0);
            }
            conn.Close();
        }

        public void insert_tb_chegyul_lst(string i_ref_dt, String i_jongmok_cd, String i_jongmok_nm,
            String i_chegyul_gb, int i_chegyul_no, int i_chegyul_price, int i_chegyul_stock_cnt,
            int i_chegyul_amt, String i_chegyul_dtm, String i_ord_no, String i_org_ord_no) //체결내역저장 메서드
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String l_sql = null;
            l_sql = null;
            cmd = null;
            conn = null;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            //체결내역 테이블 삽입
            l_sql = @" insert into tb_chegyul_lst values ( " +
                "'" + g_user_id + "'" + "," +
                "'" + g_accnt_no + "'" + "," +
                "'" + i_ref_dt + "'" + "," +
                "'" + i_jongmok_cd + "'" + "," +
                "'" + i_jongmok_nm + "'" + "," +
                "'" + i_chegyul_gb + "'" + "," +
                "'" + i_ord_no + "'" + "," +
                "'" + i_chegyul_gb + "'" + ","
                + i_chegyul_no + ","
                + i_chegyul_price + ","
                + i_chegyul_stock_cnt + ","
                +i_chegyul_amt + "," +
                "'" + i_chegyul_dtm + "'" + "," +
                "'c##team'" + "," +
                "SYSDATE" + "," +
                "null" + "," +
                "null" + ") ";

            cmd.CommandText = l_sql;

            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                write_err_log("insert tb_chegyul_lst ex : [" + ex.Message + "]\n", 0);
            }
            conn.Close();

        }

        public void merge_tb_accnt_info(String i_jongmok_cd, String i_jongmok_nm, int i_boyu_cnt, int i_boyu_price, int i_boyu_amt) //계좌정보 테이블 세팅 매서드
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String l_sql = null;

            l_sql = null;
            cmd = null;
            conn = null;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            //계좌정보 테이블 세팅, 기존에 보유한 종목이면 갱신, 보유하지 않았으면 신규로 삽입
            l_sql = @"merge into TB_ACCNT_INFO a
           using(
              select nvl(max(user_id),'0') user_id, nvl(max(ref_dt),'0') ref_dt, nvl(max(jongmok_cd),'0') jongmok_cd, nvl(max(jongmok_nm), '0') jongmok_nm
              from TB_ACCNT_INFO
              where user_id= '" + g_user_id + "'" + "and ACCNT_NO = '" + g_accnt_no + "'" +
              " and jongmok_cd = '" + i_jongmok_cd + "'" +
              " and ref_dt = to_char(sysdate, 'yyyymmdd')"
              + " ) b " +
                      " on( a.user_id = b.user_id and a.jongmok_cd = b.jongmok_cd and a.ref_dt = b.ref_dt ) " +
                      " when matched then update " +
                      " set OWN_STOCK_CNT = " + i_boyu_cnt + "," +
                      " BUY_PRICE = " + i_boyu_price + "," +
                      " OWN_AMT = " + i_boyu_amt + "," +
                      " updt_dtm = SYSDATE" + "," +
                      " updt_id = 'c##team'" +
                      " when not matched then insert (a.user_id, a.accnt_no, a.ref_dt, a.jongmok_cd, a.jongmok_nm, a.BUY_PRICE, a.OWN_STOCK_CNT, a.OWN_AMT, a.inst_dtm, a.inst_id) values ( " +
                      "'" + g_user_id + "'" + "," +
                      "'" + g_accnt_no + "'" + "," +
                      "to_char(sysdate, 'yyyymmdd') , " +
                      "'" + i_jongmok_cd + "'" + ", " +
                      "'" + i_jongmok_nm + "'" + ", "
                      + i_boyu_price + ","
                      + i_boyu_cnt + ","
                      + i_boyu_amt + "," +
                      "SYSDATE, " +
                      "'c##team'" +
                    " ) ";
            cmd.CommandText = l_sql;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                write_err_log("merge TB_ACCNT_INFO ex : [" + ex.Message + "]\n", 0);
            }
            conn.Close();

        }

        // TB_TRD_JONGMOK 테이블과 TB_ACCNT_INFO 테이블을 JOIN하여 매도대상 종목을 조회
        // 조회 결과로 매도대상 종목의 종목코드, 매수가, 보유주식수, 목표가를 가져온다
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

            conn = null;
            sql = null;
            cmd = null;
            reader = null;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            // TB_ACCNT_INFO, TB_TRD_JONGMOK table을 join하여 매도 대상 종목을 조회
            sql = @" SELECT " +
                "    A.JONGMOK_CD, " +
                "    A.BUY_PRICE, " +
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

                // 매도주문 요청 후 g_flag_4 = 1 이 될 때까지 대기한다 -> 주문완료
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
            } // while (reader.Read()) 종료

            reader.Close();
            conn.Close();
        } //sell_ord_first 종료

        // 2023년 5월 기준 주식시장의 호가가격단위에 따른 유효한 호가 가격단위를 구한다.
        // 유효하지 않은 호가가격 때문에 매도주문에 실패하는것을 방지하기 위해 매도 주문 전 유효한 호가가격단위를 구한다.
        public int get_hoga_unit_price(int i_price, String i_jongmok_cd, int i_hoga_unit_jump) // 호가가격단위 가져오기
        {
            int l_market_type = 0; // 0:코스피 , 10:코스닥
            int l_rest;

            try
            {
                // 시장 구분 가져오기
                l_market_type = int.Parse(axKHOpenAPI1.GetMarketType(i_jongmok_cd).ToString());
            } catch (Exception ex)
            {
                write_err_log("get_hoga_unit_price() ex.Message : [" + ex.Message + "]\n", 0);
            }

            if (i_price < 2000) // 기준가 < 2000
            {
                return i_price + (i_hoga_unit_jump * 1);
            }
            else if (i_price >= 2000 && i_price < 5000)
            {
                l_rest = i_price % 5;
                if (l_rest == 0)
                {
                    return i_price + (i_hoga_unit_jump * 5);
                }
                else if (l_rest < 3)
                {
                    return (i_price - l_rest) + (i_hoga_unit_jump * 5);
                }
                else
                {
                    return (i_price + (5 - l_rest)) + (i_hoga_unit_jump * 5);
                }
            }
            else if (i_price >= 5000 && i_price < 20000) // 5000 <= 기준가 < 20000
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
            else if (i_price >= 20000 && i_price < 50000) // 20000 <= 기준가 < 50000
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
            else if (i_price >= 50000 && i_price < 200000) // 50000 <= 기준가 200000
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
            else if (i_price >= 200000 && i_price < 500000) // 200000 <= 기준가 < 500000
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
            else if (i_price >= 500000) // 기준가 > 500000
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

        // 매수대상 거래종목 조회
        public void real_buy_ord() // TB_TRD_JONGMOK 테이블을 조회하고 실시간 매수주문
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            string l_jongmok_cd = null;
            int l_buy_price = 0;
            int l_buy_amt = 0;

            conn = null;
            sql = null;
            cmd = null;
            reader = null;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            // TB_ACCNT_INFO, TB_TRD_JONGMOK table을 join하여 매수 대상 종목을 조회
            // 거래종목 테이블 조회
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
                // 참고 소스에는 1이 아니라 0으로 함
                // 0으로 변경
                l_buy_price_tmp = get_hoga_unit_price(l_buy_price, l_jongmok_cd, 0); // 매수호가 구하기

                int l_buy_ord_stock_cnt = 0;
                l_buy_ord_stock_cnt = (int)(l_buy_amt / l_buy_price_tmp); // 매수주문 주식 수 구하기

                write_msg_log("종목코드 : [" + l_jongmok_cd.ToString() + "]\n", 0);
                write_msg_log("종목명 : [" + get_jongmok_nm(l_jongmok_cd) + "]\n", 0);
                write_msg_log("매수금액 : [" + l_buy_amt.ToString() + "]\n", 0);
                write_msg_log("매수가 : [" + l_buy_price_tmp.ToString() + "]\n", 0);

                int l_own_stock_cnt = 0;
                l_own_stock_cnt = get_own_stock_cnt(l_jongmok_cd); // 해당 종목 보유주식 수 구하기
                write_msg_log("보유주식수 : [" + l_own_stock_cnt.ToString() + "]\n", 0);

                if (l_own_stock_cnt > 0) // 해당 종목이 보유중이라면 매수하지 않음
                {
                    write_msg_log("해당 종목을 보유 중이므로 매수하지 않음\n", 0);
                    continue;
                }

                string l_buy_not_chegyul_yn = null;
                l_buy_not_chegyul_yn = get_buy_not_chegyul_yn(l_jongmok_cd); // 미체결 매수주문 여부 확인

                if (l_buy_not_chegyul_yn == "Y") // 미체결 매수주문이 있으므로 매수하지 않음
                {
                    write_msg_log("해당 종목에 미체결 매수주문이 있으므로 매수하지 않음 \n", 0);
                    continue;
                }

                // 매수주문 전 최우선 매수호가 조회
                int l_for_flag = 0;
                int l_for_cnt = 0;
                g_buy_hoga = 0;

                for (; ; )
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
                        for (; ; )
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

                if(l_buy_price > g_buy_hoga)
                {
                    write_msg_log("해당 종목의 매수가가 최우선 매수호가보다 크므로 매수주문하지 않음 \n", 0);
                    continue;
                }

                // 매수대상 종목의 종목코드, 매수가, 매수주문 주식수대로 매수주문 한다
                // 매수주문 후 주문응답이 정상일시 주문 요청을 완료한다
                g_flag_3 = 0;
                g_rqname = "매수주문";

                String l_scr_no = null;
                l_scr_no = "";
                l_scr_no = get_scr_no();

                int ret = 0;

                //매수주문 요청
                ret = axKHOpenAPI1.SendOrder("매수주문", l_scr_no, g_accnt_no, 1, l_jongmok_cd, l_buy_ord_stock_cnt, l_buy_price, "00", "");
                if (ret == 0)
                {
                    write_msg_log("매수주문 SendOrder() 호출 성공\n", 0);
                    write_msg_log("종목코드 : [" + l_jongmok_cd + "]\n", 0);
                }
                else
                {
                    write_msg_log("매수주문 SendOrder() 호출 실패\n", 0);
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
            } // while (reader.Read()) 종료
            reader.Close();
            conn.Close();
        }

        // 매수대상 종목을 조회하고 매수주문 하기 전에 주문하려는 종목이 이미 보유한 종목이라면 매수주문을 시행하지않음
        // 종목코드를 입력받고 보유주식수를 반환한다
        public int get_own_stock_cnt(string i_jongmok_cd) // 보유주식수 가져오기
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            int l_own_stock_cnt = 0;

            conn = null;
            sql = null;
            cmd = null;
            reader = null;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            // 계좌정보 테이블 조회
            sql = @"
                SELECT
                    NVL(MAX(OWN_STOCK_CNT), 0) OWN_STOCK_CNT
                    FROM
                    TB_ACCNT_INFO
                    WHERE USER_ID = " + "'" + g_user_id + "'" +
                    " AND JONGMOK_CD = " + "'" + i_jongmok_cd + "'" +
                    " AND ACCNT_NO = " + "'" + g_accnt_no + "'" +
                    " AND REF_DT = TO_CHAR(SYSDATE, 'YYYYMMDD') ";

            cmd.CommandText = sql;
            reader = cmd.ExecuteReader();
            reader.Read();

            l_own_stock_cnt = int.Parse(reader[0].ToString()); // 보유주식 수 구하기

            reader.Close();
            conn.Close();

            return l_own_stock_cnt;
        }

        // TB_ORD_LST 테이블과 TB_CHEGYUL_LST 테이블을 JOIN하고, 이 테이블을 조회하여 미체결된 매수주문이 있는지 확인한다
        // 미체결 매수주문이 있다면 매수주문을 하지 않는다
        public string get_buy_not_chegyul_yn(string i_jongmok_cd) // 미체결 매수주문 여부 확인
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            int l_buy_not_chegyul_ord_stock_cnt = 0;
            string l_buy_not_chegyul_yn = null;

            conn = null;
            conn = connect_db();

            sql = null;
            cmd = null;
            reader = null;

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            // 주문내역과 체결내역 테이블 조회
            sql = @"
                SELECT
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
                    "                  AND B.ORG_ORD_NO = A.ORD_NO " +
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

        // 매수주문 체결 후 갱신된 TB_ACCNT_INFO 테이블과 TB_TRD_JONGMOK 테이블을 조인하여 실시간 매도주문한다.
        public void real_sell_ord() // 실시간 매도주문
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            string l_jongmok_cd = null;
            int l_target_price = 0;
            int l_own_stock_cnt = 0;
            write_msg_log("real_sell_ord 시작\n", 0);

            conn = null;
            sql = null;
            cmd = null;
            reader = null;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            
            // 거래종목 및 계좌정보 테이블 조회
            sql = @" SELECT " +
                "    A.JONGMOK_CD, " +
                "    A.TARGET_PRICE, " +
                "    B.OWN_STOCK_CNT " +
                " FROM " + 
                "    TB_TRD_JONGMOK A, " +
                "    TB_ACCNT_INFO B " +
                " WHERE A.USER_ID = " + "'" + g_user_id + "'" +
                " AND A.JONGMOK_CD = B.JONGMOK_CD " +
                " AND B.ACCNT_NO = " + "'" + g_accnt_no + "'" +
                " AND B.REF_DT = TO_CHAR(SYSDATE, 'YYYYMMDD') " +
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

                // 보유주식수와 미체결 매도주문 주식수가 같으면 이미 매도주문이 들어간 종목이므로 매도주문하지 않는다.
                // 다르다면 (주식수) = (보유주식수) - (미체결 매도주문 주식수) 로 설정한다.
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

                    if (l_sell_ord_stock_cnt_tmp <= 0) // 매도대상 주식수가 0 이하라면 매도하지 않음
                    {
                        continue;
                    }

                    // 보유주식수와 미체결 매도주문 주식수가 같지 않은 조건일 때 get_hoga_unit_price()를 호출하여 매도주문가의 호가가격단위를 구한다.
                    int l_new_target_price = 0;
                    l_new_target_price = get_hoga_unit_price(l_target_price, l_jongmok_cd, 0);

                    // 매도 호가를 구함
                    g_flag_4 = 0;
                    g_rqname = "매도주문";

                    String l_scr_no = null;
                    l_scr_no = "";
                    l_scr_no = get_scr_no();

                    int ret = 0;

                    // 매도주문 요청 후 g_flag_4 = 1 이 될 때까지 대기한다 -> 주문완료
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
            } // while (reader.Read()) 종료
            reader.Close();
            conn.Close();
        }

        // TB_ACCNT_INFO 테이블과 TB_TRD_JONGMOK 테이블을 JOIN하여 현재 보유한 종목에 대한 종목코드, 목표가, 보유주식수를 가져온다.
        // 이미 매도주문한 종목인지 확인하고 미체결 매도주문또한 확인한다.
        public int get_sell_not_chegyul_ord_stock_cnt(string i_jongmok_cd) // 미체결 매도주문 주식 수 가져오기
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            int l_sell_not_chegyul_ord_stock_cnt = 0;

            conn = null;
            conn = connect_db();

            sql = null;
            cmd = null;
            reader = null;

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            // 주문내역과 체결내역 테이블 조회
            sql = @"
                SELECT
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

        // 보유한 종목들의 현재가를 조회하여 현재가가 손절가를 이탈한다면 시장가로 손절주문 한다.
        public void real_cut_loss_ord() // 실시간 손절주문
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String sql = null;
            OracleDataReader reader = null;

            string l_jongmok_cd = null;
            int l_cut_loss_price = 0;
            int l_own_stock_cnt = 0;

            int l_for_flag = 0;
            int l_for_cnt = 0;

            write_msg_log("real_cut_loss_ord 시작\n", 0);

            conn = null;
            sql = null;
            cmd = null;
            reader = null;
            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            //거래종목 및 계좌정보 테이블 조회
            sql = @" SELECT " +
                "    A.JONGMOK_CD, " +
                "    A.CUT_LOSS_PRICE, " +
                "    B.OWN_STOCK_CNT " +
                " FROM " +
                "    TB_TRD_JONGMOK A, " +
                "    TB_ACCNT_INFO B " +
                " WHERE A.USER_ID = " + "'" + g_user_id + "'" +
                " AND A.JONGMOK_CD = B.JONGMOK_CD " +
                " AND B.ACCNT_NO = " + "'" + g_accnt_no + "'" +
                " AND B.REF_DT = TO_CHAR(SYSDATE, 'YYYYMMDD') " +
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

                l_for_flag = 0;
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
                } // 현재가 조회 완료

                write_msg_log("현재가 : [" + g_cur_price.ToString() + "]\n", 0);

                if (g_cur_price < l_cut_loss_price) // 현재가가 손절가 이탈 시
                {
                    write_msg_log("현재가격이 손절가격을 이탈\n", 0);
                    
                    write_msg_log("sell_canc_ord() 시작\n", 0);
                    sell_canc_ord(l_jongmok_cd);
                    write_msg_log("sell_canc_ord() 완료\n", 0);

                    g_flag_4 = 0;
                    g_rqname = "매도주문";

                    String l_scr_no = null;
                    l_scr_no = "";
                    l_scr_no = get_scr_no();

                    int ret = 0;

                    // 매도주문 요청
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
                    update_tb_trd_jongmok(l_jongmok_cd);
                }
            } // while (reader.Read()) 종료
            reader.Close();
            conn.Close();
        }

        // 손절주문 대상 종목의 현재가가 손절가를 이탈하면 해당 종목의 미체결 매도주문을 매도취소하고 시장가로 손절 매도주문을 한다
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

            conn = null;
            conn = connect_db();

            sql = null;
            cmd = null;
            reader = null;

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            // 주문내역과 체결내역 테이블 조회
            sql = @"select
	                    rowid rid,
	                    jongmok_cd,
	                    (ord_stock_cnt -
	                    ( select nvl(max(b.CHEGYUL_STOCK_CNT), 0) CHEGYUL_STOCK_CNT
	                      from tb_chegyul_lst b
	                      where b.user_id = a.user_id
	                      and b.accnt_no = a.accnt_no
	                      and b.ref_dt = a.ref_dt
	                      and b.jongmok_cd = a.jongmok_cd
	                      and b.ord_gb = a.ord_gb
	                      and b.ord_no = a.ord_no
                      )) sell_not_chegyul_ord_stock_cnt,
	                    ord_price,
	                    ord_no,
	                    org_ord_no
                    from
                    TB_ORD_LST a
                    where a.ref_dt = TO_CHAR(SYSDATE, 'YYYYMMDD')
                    and a.user_id =     " + "'" + g_user_id + "'" +
                    " and a.accnt_no =     " + "'" + g_accnt_no + "'" +
                    " and a.jongmok_cd =     " + "'" + i_jongmok_cd + "'" +
                    " and a.ord_gb = '1' " +
                    " and a.org_ord_no = '0000000' " +
                    " and not exists ( 	select '1' " +
                    " 				    from TB_ORD_LST b " +
                    " 				    where b.user_id = a.user_id " +
                    " 				    and b.accnt_no = a.accnt_no " +
                    " 				    and b.ref_dt = a.ref_dt " +
                    " 				    and b.jongmok_cd = a.jongmok_cd " +
                    " 				    and b.ord_gb = a.ord_gb " +
                    " 				    and b.org_ord_no = a.ord_no " +
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

        // 시장가로 매도주문하여 손절주문을 완료하면 손절한 종목을 매수주문하지 않도록 TB_TRD_JONGMOK 테이블의 BUY_TRD_YN을 'N'으로 설정한다
        public void update_tb_trd_jongmok(String i_jongmok_cd)
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String l_sql = null;

            conn = connect_db();

            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            l_sql = @" update TB_TRD_JONGMOK set buy_trd_yn = 'N', updt_dtm = SYSDATE, updt_id = 'c##team' " +
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

        private void buttonUpdate_Click(object sender, EventArgs e)
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
                    var title = parseJson["items"][i]["title"].ToString();
                    title = title.Replace("&quot;", "\"");

                    var description = parseJson["items"][i]["description"].ToString();
                    description = description.Replace("&quot;", "\"");

                    var link = parseJson["items"][i]["link"].ToString();

                    // 생성 순서를 바꿉니다.
                    ListViewItem item = new ListViewItem(link);
                    item.SubItems.Add(title);
                    item.SubItems.Add(description);


                    ResultList.Items.Add(item);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
        }
        private void ResultList_MouseDoubleClick(object sender, MouseEventArgs e) //listbox에 마우스 더블클릭 적용 함수임
        {
            ListViewItem item = ResultList.GetItemAt(e.X, e.Y);

            if (item != null && item.SubItems.Count > 0)
            {
                string url = item.SubItems[0].Text;

                if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true }); //더블클릭한 곳 listbox에서 이 url일경우 웹으로 연다.
                }
            }
        }


        private string getResults() // 뉴스 기사 업데이트에 쓰이는 결과값 받아오기 함수임
        {
            string keyword = Searchbox.Text;
            string display = btn_DisplayCount.Value.ToString();
            string sort = "sim";
            if (btn_date.Checked == true)
                sort = "date";


            string query = string.Format("?query={0}&display={1}&sort={2}", keyword, display, sort);


            WebRequest request = WebRequest.Create(_apiUrl + query);
            request.Headers.Add("X-Naver-Client-Id", "I74lzNbMOpmIlEsfaWRO");
            request.Headers.Add("X-Naver-Client-Secret", "73vhwkbPBc");

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

        private void loginbtn_Click(object sender, EventArgs e)
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

        private void logoutbtn_Click(object sender, EventArgs e)
        {
            Close();
            // Form 종료로 로그아웃 진행
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            clocklabel.Text = DateTime.Now.ToString();
            clocklabel.ForeColor = Color.Red;
            clocklabel.BackColor = Color.Linen;

        }

        private void Searchbox_Enter(object sender, EventArgs e) // 뉴스 검색란 워터마크 클릭시 사라짐
        {
            if ( Searchbox.ForeColor == Color.Silver)
            {
                Searchbox.Text = "";
                Searchbox.ForeColor = Color.Black;
            }
        }

        private void Searchbox_Leave(object sender, EventArgs e) // 뉴스 검색란 워터마크 표시
        {
            if (Searchbox.Text == "")
            {
                Searchbox.Text = "News Search.";
                Searchbox.ForeColor = Color.Silver;
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.kiwoom.com/wm/myk/ac000/myAsetView?dummyVal=0");
        }
    }
}
