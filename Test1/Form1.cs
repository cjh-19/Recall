using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.loginButton.Click += this.ButtonClicked; // 로그인 버튼 클릭 시 
            /*this.logoutButton.Click += this.ButtonClicked;*/
            this.inquireButton.Click += this.ButtonClicked; // 조회 버튼 클릭 시
            this.dailyButton.Click += this.ButtonClicked; // 일봉데이터 버튼 클릭 시
            /*this.axKHOpenAPI1.OnEventConnect += this.axKHOpenAPI1_OnEventConnect;*/
            this.axKHOpenAPI1.OnReceiveTrData += this.axKHOpenAPI1_OnReceiveTrData;
           /*this.axKHOpenAPI1.OnReceiveRealData += this.axKHOpenAPI1_OnReceiveRealData;*/

        }

        private void ButtonClicked(object sender, EventArgs e)
        {
            if (sender.Equals(this.loginButton))
            {
                if (axKHOpenAPI1.CommConnect() == 0) // API 실행
                    this.listBox1.Items.Add("로그인 시작");
                else
                    this.listBox1.Items.Add("로그인 실패");
            }
            else if (sender.Equals(this.inquireButton)) // 조회 버튼 클릭
            {
                axKHOpenAPI1.SetInputValue("종목코드", this.itemCodeTextBox.Text.Trim()); // 주식 조회 요청시 TR의 Input값 지정

                int nRet = axKHOpenAPI1.CommRqData("주식기본정보", "OPT10001", 0, "1001"); // TR설정 후 요청시 주식 서버에서 원하는 데이터를 Request 함

                if (nRet == 0)
                    this.listBox1.Items.Add("주식 정보요청 성공");
                else
                    this.listBox1.Items.Add("주식 정보요청 실패");
            }
            else if (sender.Equals(this.dailyButton)) // 기준일자 버튼 클릭
            {
                axKHOpenAPI1.SetInputValue("종목코드", this.itemCodeTextBox.Text.Trim());
                axKHOpenAPI1.SetInputValue("기준일자", this.dateTextBox.Text.Trim());
                axKHOpenAPI1.SetInputValue("수정주가구분", "1");

                int nRet;
                if (!this.checkBox1.Checked)
                    nRet = axKHOpenAPI1.CommRqData("주식일봉차트조회", "OPT10081", 0, "1002"); // 신규조회
                else
                    nRet = axKHOpenAPI1.CommRqData("주식일봉차트조회", "OPT10081", 2, "1002"); // 연속조회

                if (nRet == 0)
                    this.listBox1.Items.Add("주식 일봉 정보요청 성공");
                else
                    this.listBox1.Items.Add("주식 일봉 정보요청 실패");
            }
        }

       /* private void axKHOpenAPI1_OnEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (e.nErrCode == 0)
            {
                this.listBox1.Items.Add("로그인 성공");
                if (this.axKHOpenAPI1.GetConnectState() == 1)
                    this.listBox1.Items.Add("---연결중---");

                else if (this.axKHOpenAPI1.GetConnectState() == 0)
                    this.listBox1.Items.Add("--연결실패--");

                getUserInfo();
                return;
            }
            else
                this.listBox1.Items.Add("로그인 실패");
        }
       */

        /*public void getUserInfo()
        {
            this.idLabel.Text += axKHOpenAPI1.GetLoginInfo("USER_ID");
            this.nameLabel.Text += axKHOpenAPI1.GetLoginInfo("USER_NAME");

            string[] acountArray = axKHOpenAPI1.GetLoginInfo("ACCNO").Split(',');
            this.comboBox.Items.AddRange(acountArray);
            this.comboBox.SelectedIndex = 0;
        }*/

        private void axKHOpenAPI1_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (e.sRQName == "주식기본정보") // 일반조회
            {
                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName); // 개수 확인 후 index별 데이터 출력
                for (int nidx = 0; nidx < nCnt; nidx++)
                {
                    this.listBox2.Items.Add("종목코드" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "종목코드").Trim()); // 상세 정보를 얻는다.
                    this.listBox2.Items.Add("종목명" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "종목명").Trim());
                    this.listBox2.Items.Add("거래량" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "거래량").Trim());
                    this.listBox2.Items.Add("시가" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "시가").Trim());
                    this.listBox2.Items.Add("최고가" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "최고가").Trim());
                    this.listBox2.Items.Add("최저가" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "최저가").Trim());
                    this.listBox2.Items.Add("현재가" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "현재가").Trim());
                    this.listBox2.Items.Add("등락율" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "등락율").Trim());
                }
            }
            else if (e.sRQName == "주식일봉차트조회") // 일봉데이터 조회
            {
                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName); // 멀티데이터 개수 확인 후 index별 데이터 출력
                for (int nidx = 0; nidx < nCnt; nidx++)
                {
                    this.listBox2.Items.Add("일자" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "일자").Trim());
                    this.listBox2.Items.Add("현재가" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "현재가").Trim());
                    this.listBox2.Items.Add("거래량" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "거래량").Trim());
                    this.listBox2.Items.Add("시가" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "시가").Trim());
                    this.listBox2.Items.Add("고가" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "고가").Trim());
                    this.listBox2.Items.Add("저가" + axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nidx, "저가").Trim());
                    this.listBox2.Items.Add("--------------------------------------");
                    this.listBox2.SelectedIndex = this.listBox2.Items.Count - 1;
                }
            }
        }

        /*private void axKHOpenAPI1_OnReceiveRealData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            this.listBox3.Items.Add(e.sRealKey);
            this.listBox3.Items.Add(e.sRealType);
            this.listBox3.Items.Add(e.sRealData);

            if (e.sRealType == "주식시세")
            {
                this.listBox3.Items.Add("현재가" + axKHOpenAPI1.GetCommRealData(e.sRealType, 10).Trim());
                this.listBox3.Items.Add("전일대비" + axKHOpenAPI1.GetCommRealData(e.sRealType, 11).Trim());
                this.listBox3.Items.Add("등락율" + axKHOpenAPI1.GetCommRealData(e.sRealType, 12).Trim());
                this.listBox3.Items.Add("매도호가" + axKHOpenAPI1.GetCommRealData(e.sRealType, 27).Trim());
                this.listBox3.Items.Add("매수호가" + axKHOpenAPI1.GetCommRealData(e.sRealType, 28).Trim());
                this.listBox3.Items.Add("누적거래량" + axKHOpenAPI1.GetCommRealData(e.sRealType, 13).Trim());
                this.listBox3.Items.Add("누적거래대금" + axKHOpenAPI1.GetCommRealData(e.sRealType, 14).Trim());
                this.listBox3.Items.Add("시가" + axKHOpenAPI1.GetCommRealData(e.sRealType, 16).Trim());
                this.listBox3.Items.Add("고가" + axKHOpenAPI1.GetCommRealData(e.sRealType, 17).Trim());
                this.listBox3.Items.Add("저가" + axKHOpenAPI1.GetCommRealData(e.sRealType, 18).Trim());
                this.listBox3.Items.Add("전일대비기호" + axKHOpenAPI1.GetCommRealData(e.sRealType, 25).Trim());
                this.listBox3.Items.Add("전일거래량대비" + axKHOpenAPI1.GetCommRealData(e.sRealType, 26).Trim());
                this.listBox3.Items.Add("거래대금증감" + axKHOpenAPI1.GetCommRealData(e.sRealType, 29).Trim());
                this.listBox3.Items.Add("전일거래량대비(비율)" + axKHOpenAPI1.GetCommRealData(e.sRealType, 30).Trim());
            }
        }
        */
    }
}
