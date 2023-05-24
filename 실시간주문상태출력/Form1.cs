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

   
        const string _apiUrl = "https://openapi.naver.com/v1/search/news";   //추가기능 뉴스 api
        const string _clientId = "I74lzNbMOpmIlEsfaWRO";
        const string _clientSecret = "jgntupzVD8";

        int g_flag_1 = 0; // 1이면 요청에 대한 응답 완료
        string g_rqname = null;
        int g_ord_amt_possible; //총 매수 가능 금엑

        int g_flag_2 = 0;
        int g_is_next = 0;

        int g_flag_3 = 0; // 매수주문 응답 플래그
        int g_flag_4 = 0; //매도주문 응답 플래그
        int g_flag_5 = 0; // 매도취소주문 응답 플래그

        public Form1()
        {
            InitializeComponent();
            // 데이터 수신 요청에 대한 응답을 받는 대기 이벤트 메서드 선언
            this.axKHOpenAPI1.OnReceiveTrData += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEventHandler(this.axKHOpenAPI1_OnReceiveTrData);
            this.axKHOpenAPI1.OnReceiveMsg += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveMsgEventHandler(this.axKHOpenAPI1_OnReceiveMsg);
            this.axKHOpenAPI1.OnReceiveChejanData += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEventHandler(this.axKHOpenAPI1_OnReceiveChejanData);
            
        }

        public void set_tb_accnt_info() //계좌정보 테이블 설정
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
                cmd.CommandText = sql;
            }
            catch (Exception ex)
            {
                write_err_log("delete tb_accnt_info ex.Message : [" + ex.Message + "]\n", 0);
            }
            conn.Close();

            g_is_next = 0;
            for(; ;)
            {
                l_for_flag = 0;
                for(; ; )
                {
                    axKHOpenAPI1.SetInputValue("계좌번호", g_accnt_no);
                    axKHOpenAPI1.SetInputValue("비밀번호", "");
                    axKHOpenAPI1.SetInputValue("상장폐지조회구분", "1");
                    axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");

                    g_flag_2 = 0;
                    g_rqname = "계좌평가현황요청";

                    String l_scr_no = get_scr_no();

                    axKHOpenAPI1.CommRqData("계좌평가현황요청", "OPW0004", g_is_next, l_scr_no); //axKHOpenAPI1.OnRecieveData 호출

                    l_for_cnt = 0;
                    for (; ; )
                    {
                        if(g_flag_2 == 1)
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
                        delay(1000);
                        axKHOpenAPI1.DisconnectRealData(l_scr_no);

                        if(l_for_flag == 1)
                        {
                            break;
                        }
                        else if (l_for_flag==0)
                        {
                            delay(1000);
                            continue;
                        }
                    }
                    if (g_is_next == 0)
                    {
                        break;
                    }
                    delay(1000);
                }
            }


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
                axKHOpenAPI1.SetInputValue("계좌번호", g_accnt_no);
                axKHOpenAPI1.SetInputValue("비밀번호", "");

                g_rqname = "";
                g_rqname = "증거금세부내역조회요청"; // 요청명 정의
                g_flag_1 = 0; //요청중

                String l_scr_no = null; //화면번호를 담을 변수 선언
                l_scr_no = "";
                l_scr_no = get_scr_no();//화면번호 채번
                axKHOpenAPI1.CommRqData("증거금세부내역조회요청", "opw00013", 0, l_scr_no); // Open API로 데이터 요청

                l_for_cnt = 0;
                for (; ; )
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
                        if (l_for_cnt == 1) // 한번이라도 실패하면 무한루프를 빠져나감(증권계좌 비밀번호 오류 방지
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



        // 10장에서 정의 : 투자정보를 요청할 때 데이터 수신 요청에 대한 응답을 받는 이벤트 메서드
        private void axKHOpenAPI1_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (g_rqname.CompareTo(e.sRQName) == 0)//요청한 요청명과 OPEN API로부터 응답받은 요청명이 같다면
            {
                ;//다음으로 진행
            }
            else
            {
                write_err_log("요청한 TR  :[" + g_rqname + "]\n", 0);
                write_err_log("응답받은 TR  :[" + e.sRQName + "]\n", 0);

                switch (g_rqname)
                {
                    case "증거금세부내역조회요청":
                        g_flag_1 = 1; //g_flag_1을 1로 설정하여 요청하는 쪽이 무한루프에서 빠지지않게 방지
                        break;
                    case "계좌평가현황요청":
                        g_flag_2 = 1; //g_flag_2을 1 로 설정 하여 요청하는 쪽에서 무한 루프에 빠지지 않게 함
                        break;

                    default: break;
                }
                return;
            }
            if (e.sRQName == "증거금세부내역조회요청")
            {
                g_ord_amt_possible = int.Parse(axKHOpenAPI1.CommGetData(e.sTrCode, "", e.sRQName, 0, "100주문가능금액").Trim()); //주문 가능 금액을 저장
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

                repeat_cnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);

                write_msg_log("TB_ACCNT_INFO 테이블 설정 시작\n", 0);
                write_msg_log("보유종목수 : " + repeat_cnt.ToString() + "\n", 0);

                for(ii = 0; ii < repeat_cnt; ii++)
                {
                    user_id = "";
                    jongmok_cd = "";
                    own_stock_cnt = 0;
                    buy_price = 0;
                    own_amt = 0;

                    user_id = g_user_id;
                    jongmok_cd = axKHOpenAPI1.CommGetData(e.sTrCode, "", e.sRQName, ii, "종목코드").Trim().Substring(1, 6);
                    jongmok_nm = axKHOpenAPI1.CommGetData(e.sTrCode, "", e.sRQName, ii, "종목명").Trim();
                    own_stock_cnt = int.Parse(axKHOpenAPI1.CommGetData(e.sTrCode, "", e.sRQName, ii, "보유수량").Trim());
                    buy_price = int.Parse(axKHOpenAPI1.CommGetData(e.sTrCode, "", e.sRQName, ii, "평균단가").Trim());
                    own_amt = int.Parse(axKHOpenAPI1.CommGetData(e.sTrCode, "", e.sRQName, ii, "매입금액").Trim());

                    write_msg_log("종목코드 : " + jongmok_cd + "\n", 0);
                    write_msg_log("종목명 : " + jongmok_nm + "\n", 0);
                    write_msg_log("보유주식수 : " + own_stock_cnt.ToString() + "\n", 0);

                    if(own_stock_cnt == 0)//보유주식수가 0이라면 저장하지 않음
                    {
                        continue;
                    }

                    insert_tb_accnt_info(jongmok_cd, jongmok_nm, buy_price, own_stock_cnt, own_amt); // 계좌정보에 테이블 저장
                }
                write_msg_log("TB_ACCNT_INFO 테이블 설정 완료\n", 0);
                axKHOpenAPI1.DisconnectRealData(e.sScrNo);

                if(e.sPrevNext.Length == 0)
                {
                    g_is_next = 0;
                }
                else
                {
                    g_is_next = int.Parse(e.sPrevNext);
                }
                g_flag_2 = 1;
            }
        }//axKHOpenAPI_OnReceiveTrData 메서드 종료

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
            l_sql = @" insert into tb_accnt_info values (" +
                "'" + g_user_id + "'" + "," +
                "'" + g_accnt_no + "'" + "," + 
                "to_char(sysdate, 'yyyymmdd')" + "," + 
                "'" + i_jongmok_cd + "'" + "," + 
                "'" + i_jongmok_nm + "'" + "," +
                +i_buy_price + "," + 
                +i_own_stock_cnt + "," +
                +i_own_amt + "," + 
                "'ats'" + "," +
                "SYSDATE" + "," + "null" + "," + "null" + ")";

            cmd.CommandText = l_sql;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                write_err_log("inser tb_accnt_info() inssert tb_accnt_info ex.Message : [" + ex.Message + "]\n", 0);
            }
            conn.Close();
        }

        public void merge_tb_accnt(int g_ord_amt_possible) //계좌정보 테이블 세팅 메서드
        {
            OracleCommand cmd = null;
            OracleConnection conn = null;
            String l_sql = null;

            l_sql = null;
            cmd = null;
            conn = null;
            conn = connect_db();

            if (conn != null)
            {
                cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;

                l_sql = @"merge into tb_accnt a
            using(
            select nvl(max(user_id),'')user_id, nvl(max(accnt_no),' ')accnt_no, nvl(max(red_dt),' ')ref_dt " +
                    "from tb_accnt" +
                    "where user_id = '" + g_user_id + "'" +
                    "and accnt_no = " + "'" + g_accnt_no + "'" +
                    "and ref_dt = to_char(sysdate, yyyymmdd')" +
                    ") b " +
                    "on ( a.user_id = b.user_id and a.accnt_no = b.accnt_no and a.ref_dt = b.ref.dt) " +
                    "when matched then update" +
                    "set ord_possible_amt = " + g_ord_amt_possible + "," +
                    "updt_dtm = 'ats'" +
                    "when not matched then insert (a.user_id, a.accnt_no, a.ref_dt, a.ord_possible_amt, a.inst_dtm, a.inst_id) values ( " +
                    "'" + g_user_id + "'" + "," +
                    "'" + g_accnt_no + "'" + "," +
                    " to_char(sysdate, 'yyyymmdd')" + "," +
                    +g_ord_amt_possible + "," +
                    "SYSDATE, " +
                    "'ats'" +
                    ")";
                cmd.CommandText = l_sql;

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
                    conn.Close();
                }
            }
            else
            {
                write_msg_log("db connection check!\n", 0);
            }
        }



        // 12장에서 정의 : 주식주문을 요청할 때 해당 주식 주문의 응답을 수신하는 이벤트 메서드
        private void axKHOpenAPI1_OnReceiveMsg(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            if(e.sRQName == "매수주문")
            {
                write_msg_log("\n========매수주문 원장 응답정보 출력 시작========\n", 0);
                write_msg_log("sScrNo : [" + e.sScrNo + "]" + "\n", 0);
                write_msg_log("sRQName : [" + e.sRQName + "]" + "\n", 0);
                write_msg_log("sTrCode : [" + e.sTrCode + "]" + "\n", 0);
                write_msg_log("sMsg : [" + e.sMsg + "]" + "\n", 0);
                write_msg_log("=========매수주문 원장 응답정보 출력 종료========\n", 0);
                g_flag_3 = 1; //매수주문 응답완료 설정
            }

            if(e.sRQName == "매도주문")
            {
                write_msg_log("\n========매수주문 원장 응답정보 출력 시작========\n", 0);
                write_msg_log("sScrNo : [" + e.sScrNo + "]" + "\n", 0);
                write_msg_log("sRQName : [" + e.sRQName + "]" + "\n", 0);
                write_msg_log("sTrCode : [" + e.sTrCode + "]" + "\n", 0);
                write_msg_log("sMsg : [" + e.sMsg + "]" + "\n", 0);
                write_msg_log("=========매수주문 원장 응답정보 출력 종료========\n", 0);
                g_flag_3 = 1; //매수주문 응답완료 설정
            }
            if(e.sRQName == "매도취소주문")
            {
                write_msg_log("\n========매수주문 원장 응답정보 출력 시작========\n", 0);
                write_msg_log("sScrNo : [" + e.sScrNo + "]" + "\n", 0);
                write_msg_log("sRQName : [" + e.sRQName + "]" + "\n", 0);
                write_msg_log("sTrCode : [" + e.sTrCode + "]" + "\n", 0);
                write_msg_log("sMsg : [" + e.sMsg + "]" + "\n", 0);
                write_msg_log("=========매수주문 원장 응답정보 출력 종료========\n", 0);
                g_flag_3 = 1; //매수주문 응답완료 설정
            }
        }

        // 12장에서 정의 : 주식주문을 요청한 후 주문내역과 체결내역 데이터를 수신하는 이벤트 메서드
        private void axKHOpenAPI1_OnReceiveChejanData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            if(e.sGubun == "0")//sGubun의 값이 "0" 이라면 주문내역 및 체결내역 수신
            {
                String chejan_gb = "";
                chejan_gb = axKHOpenAPI1.GetChejanData(913).Trim(); //주문내역인지 체결내역인지 가져옴
                if(chejan_gb == "접수") //chejan_gb 의 값이 "접수"라면 주문내역
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

                    if(ord_gb == "2") //매수주문일경우
                    {
                        update_tb_accnt(ord_gb, ord_amt);
                    }

                } //"if(chejan_gb == "접수")" 종료
                else if(chejan_gb == "체결") //chejan_gb의 값이 "체결"이라면 체결내역
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
                    chegyul_no = int.Parse(axKHOpenAPI1.GetChejanData(910).Trim());
                    chegyul_price = int.Parse(axKHOpenAPI1.GetChejanData(911).Trim());
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
                    if(chegyul_gb == "1") //매도체결이라면 계좌테이블의 매수가능 금액을 늘려줌
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
        } //메서드종료

        public void merge_tb_accnt_info(String i_jongmok_cd, String i_jongmok_nm, int i_boyu_cnt, int i_boyu_price, int i_boyu_amt) //계좌정보 테이블 세팅 함수
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
              select nvl(max(user_id),'0') user_id,nvl(max(ref_dt),'0') ref_dt, nvl(max(jongmok_cd),'0') jongmok_cd, nvl(max(jongmok_nm),'0') jongmok_nm
              from TB_ACCNT_INFO
              where user_id= '" + g_user_id + "'" + "and ACCNT_NO = '" + g_accnt_no + "'" + 
              "and jongmok_cd = '" + i_jongmok_cd + "'" + 
              "and ref_dt = to_char(sysdate, 'yyyymmdd')" 
              + ")b " +
                      "on(a.user_id=b.user_id and a.jongmok_cd =b.jongmok_cd and a.ref_dt =b.ref_dt)" +
                      " when matched then update " +
                      " set OWN_STOCK_CNT = " + i_boyu_cnt + "," +
                      " BUY_PRICE = " + i_boyu_price + "," +
                      " OWN_AMT = " + i_boyu_amt + "," +
                      " updt_dtm = SYSDATE" + "," +
                      " updt_id = 'c##team'" + //id 일단 우리걸로 바꿈 
                      " when not matched then insert (a.user_id,a.accnt_no,a.ref_dt,a.jongmok_cd,a.jongmok_nm,a.BUY_PRICE,a.OWN_STOCK_CNT, a.OWN_AMT, a.inst_dtm,a.inst_id) values ( " +
                      "'" + g_user_id + "'" + "," +
                      "'" + g_accnt_no + "'" + "," +
                      "to_char(sysdate, 'yyyymmdd'), " +
                      "'" + i_jongmok_cd + "'" + "," +
                      "'" + i_jongmok_nm + "'" + "," +
                      +i_boyu_price + "," +
                      +i_boyu_cnt + "," +
                      +i_boyu_amt + "," +
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
                "'" + i_chegyul_gb + "'" + "," +
                +i_chegyul_no + "," +
                +i_chegyul_price + "," +
                +i_chegyul_stock_cnt + "," +
                "'ats" + "," +
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
                write_err_log ("insert tb_chegyul_lst ex : [" + ex.Message + "]\n", 0);
            }
            conn.Close();

        }


        public void insert_tb_ord_lst(string i_ref_dt, String i_jongmok_cd, String i_jongmok_nm, String i_ord_gb, String i_ord_no, String i_org_ord_no, 
            int i_ord_price, int i_ord_stock_cnt, int i_ord_amt, String i_ord_dtm)
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
            l_sql = @" insert into tb_ord_lst values (" +
                "'" + g_user_id + "'" + "," +
                "'" + g_accnt_no + "'" + "," +
                "'" + i_ref_dt + "'" + "," +
                "'" + i_jongmok_cd + "'" + "," +
                "'" + i_jongmok_nm + "'" + "," +
                "'" + i_ord_gb + "'" + "," +
                "'" + i_ord_no + "'" + "," +
                "'" + i_org_ord_no + "'" + "," +
                +i_ord_price + "," +
                +i_ord_stock_cnt + "," +
                +i_ord_amt + "," +
                "'" + i_ord_dtm + "," +
                "'ats'" + "," +
                "SYSDATE" + "," +
                "null" + "," +
                "null" + ")";

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

        public void update_tb_accnt(String i_chegyul_gb, int i_chegyul_amt)//계좌테이블 수정 메서드
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

            if(i_chegyul_gb == "2") // 매수일떄 주문 가능 금액에서 체결금액 뺴기
            {
                l_sql = @" update TB_ACCNT set ORD_POSSIBLE_AMT = ord_possile_amt - "
+ i_chegyul_amt + ", updt_dtm = SYSDATE, updt_id = 'ats' " +
" where user_id = " + "'" + g_user_id + "'" + 
" and accnt_no = " + "'" +g_accnt_no + "'" +
" and ref_dt = to_char(sysdate, 'yyyymmdd') ";
            }

            cmd.CommandText = l_sql;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                write_err_log("update TB_ACCNT ex.Message: [" + ex.Message + "\n", 0);
            }
            conn.Close();
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
            int l_set_tb_accnt_flag = 0; //1이면 호출 완료
            int l_set_tb_accnt_info_flag = 0; //1이면 호출 완료

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
                    if(l_set_tb_accnt_flag == 0) //호출전
                    {
                        l_set_tb_accnt_flag = 1; //호출로 설정
                        set_tb_accnt(); //호출
                    }
                }
                if(l_set_tb_accnt_info_flag == 0)
                {
                    set_tb_accnt_info(); //계좌정보 테이블 설정
                    l_set_tb_accnt_info_flag = 1;
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
