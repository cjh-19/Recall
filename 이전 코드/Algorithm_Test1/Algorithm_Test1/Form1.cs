// 자동매매는 윈폼 프록램이 생성한 스레드에서 이루어짐.
// 자동매매 시작 버튼을 누르면 새로운 스레드가 생성되고
// 주식시장 운영시간중에 가동 되며 자동으로 매수와 매도 시작
// "스레드"를 생성하는 것이 키


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading; // 스레드 라이브러리 참조
using System.Runtime.ExceptionServices; // [HandleProcessCorruptedStateExceptions]  사용
using System.Security; // [SecurityCritical] 사용

namespace Algorithm_Test1
{
    public partial class Form1 : Form
    {
        // Form 클래스 영역에 스레드 생성 여부를 관리하는 클래스 변수와
        // 스레드 객체를 저장한 클래스 변수 선언
        int g_is_thread = 0; // 0이면 스레드 미생성, 1이면 스레드 생성
        Thread thread1 = null; // 생성된 스레드 객체를 담을 변수

        // Open API에서 특정 데이터를 수신하려면 자동매매 스레드에서 API에 수신 요청을 하고
        // API가 키움증권 Open API 서버에 데이터를 요청한다.
        // API서버는 키움증권의 원장 서버에 데이터를 다시 요청하고 응답받은 뒤
        // 그 데이터를 사용자의 이벤트 메서드를 통해 수신을 받는다.
        // 이를 위해 이벤트 메서드 선언이 필요하다
        // 윈폼이 시작될 때 가장먼저 되기 위해서 Form1 클래스의 생성자인 Form1 메서드 생성자에서 같이 선언
        public Form1()
        {
            InitializeComponent();
            // 데이터 수신 요청에 대한 응답을 받는 대기 이벤트 메서드 선언
            this.axKHOpenAPI1.OnReceiveTrData += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEventHandler(this.axKHOpenAPI1_OnReceiveTrData);
            this.axKHOpenAPI1.OnReceiveMsg += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveMsgEventHandler(this.axKHOpenAPI1_OnReceiveMsg);
            this.axKHOpenAPI1.OnReceiveChejanData += new AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEventHandler(this.axKHOpenAPI1_OnReceiveChejanData);
        }
        // 이벤트 명 : OnReceiveTrData / OnReceiveMsg / OnReceiveChejanData
        // 메서드 명 : axKHOpenAPI1_OnReceiveTrData / axKHOpenAPI1_OnReceiveMsg / axKHOpenAPI1_OnReceiveChejanData
        // OnReceiveTrData : 투자정보를 요청할 때 데이터 수신 요청에 대한 응답을 받는 이벤트 메서드
        // 현재가, 최우선 매수호가, 추정자산, 매입금액 등
        // OnReceiveMsg : 주식주문을 요청할 때 해당 주식 주문의 응답을 수신하는 이벤트 메서드
        // 매수주문, 매도주문 등의 응답 여부
        // OnReceiveChejanData : 주식주문을 요청한 후 주문내역과 체결내역 데이터를 수신하는 이벤트 메서드
        // 주문내역, 잔고내역, 체결내역

        // 10장에서 정의
        private void axKHOpenAPI1_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {

        }
        // 12장에서 정의
        private void axKHOpenAPI1_OnReceiveMsg(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {

        }
        // 12장에서 정의
        private void axKHOpenAPI1_OnReceiveChejanData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {

        }

        [HandleProcessCorruptedStateExceptions]  // 손상된 프로레스 상태를 나타내는 예외 처리
        [SecurityCritical] // 코드나 어셈블리가 중요한 작업을 수행함을 지정

        // Thread.sleep을 쓰지 않고 Task.delay 이유는?
        // sleep은 동기함수이며 스레드를 차단하므로 비동기 코드에서는 sleep을 사용하는 것을 절제해야함
        // delay는 비동기함수로 현재 스레드를 차단하지 않고 논리적으로 지연시킴
        // 스레드를 차단하지 않는 것이 키
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
                catch (Exception) //(AccessViolationException e)
                {
                    // 지연 로그 메세지 출력
                    //write_err_log("delay() e.Message : [" + e.Message + "]\n", 0);
                }
                ThisMoment = DateTime.Now;
            }
            return DateTime.Now;

        }

        public string get_cur_tm()  //시간 가져오기 함수
        {
            DateTime l_cur_time;
            string l_cur_tm;

            l_cur_time = DateTime.Now; //현재시각 저장
            l_cur_tm = l_cur_time.ToString("HHmmss"); //시분초 저장

            return l_cur_tm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(g_is_thread == 1) // 스레드가 이미 생성된 상태라면
            {
                // 이미 실행됐다고 출력
                // write_msg_log("Auto Tradng이 이미 시작되었습니다.\n", 0);
                return; // 이벤트 메서드 종료
            }
            // 스레드 생성 시작
            g_is_thread = 1; // 스레드 생성으로 값 설정
            thread1 = new Thread(new ThreadStart(m_thread1)); // 스레드 생성
            // 스레드가 생성되면 m_thread1 이라는 메서드를 실행한다
            // 이 메서드가 바로 신규로 생성된 스레드이다.
            thread1.Start(); // 스레드 시작
        }
        // 자동매매 시작 버튼을 누르면 새로운 스레드(m_thread1)가 생성되면서
        // m_thread1 메서드에 구현된 로직을 실행
        // 이 메서드는 주식시장 운영시간(9시부터 15시30분까지)에 매수와 매도하는 것을 무한 반복한다.
        // 08시 30분부터는 재동매매를 위한 준비를 시작
        // 이때 무한루프안에 지연시간이 없다면 프록램은 엄청난 부하 발생
        // 따라서 delay()함수를 호출하여 적절한 지연시간 추가
        public void m_thread1()
        {
            string l_cur_tm = null;

            // 스레드 생성 파트
            if(g_is_thread == 0) // 최초 스레드 생성
            {
                g_is_thread = 1; // 중복 스레드 생성 방지를 위해 스레드 값 1로 변경
                // write_msg_log("자동매매가 시작되었습니다.\n", 0); // 시작 로그 메세지 출력
            }

            // while문으로 안돌리고 for문으로 돌리는 이유는?
            // 추후에 while문으로 실행해보자
            for(; ; ) // 첫 번째 무한루프 시작
            {
                l_cur_tm = get_cur_tm(); // 현재 시각 조회
                if (l_cur_tm.CompareTo("083001") >= 0) // 8시 30분 이후라면
                {
                    // 계좌조회, 계죄정보 조회, 보유종목 매도주문 수행
                    // 챕터10 이후부터 구현 파트
                }

                // 장이 열리면 장이 닫힐 때까지 for문안에서 다시 무한루프를 시작한다.
                if(l_cur_tm.CompareTo("090001") >= 0) // 09시 이후라면
                {
                    for(; ; ) // 두 번째 무한루프 시작
                    {
                        l_cur_tm = get_cur_tm(); // 현재시각 조회
                        if(l_cur_tm.CompareTo("153001") >= 0) // 15시 30분 이후라면
                        {
                            break; // 장이 닫히면 두 번째 무한루프를 빠져나간다.
                        }
                        // 장 운영 시간 중이므로 매수나 매도 주문

                        // Thread.sleep을 쓰지 않고 Task.delay 이유는?
                        // sleep은 동기함수이며 스레드를 차단하므로 비동기 코드에서는 sleep을 사용하는 것을 절제해야함
                        // delay는 비동기함수로 현재 스레드를 차단하지 않고 논리적으로 지연시킴
                        // 스레드를 차단하지 않는 것이 키
                        delay(200); // 첫 번째 무한루프 지연
                        
                    }
                }

            }
        }

        // 자동매매 시스템 중지
        // 스레드가 중지되며 더는 매매하지 않음
        private void button2_Click(object sender, EventArgs e)
        {
            // write_msg_log("\n 자동매매 중지 시작\n", 0);

            try
            {
                thread1.Abort();
            }
            catch(Exception ex)
            {
                // write_err_log("자동매매 중지 ex.Message : " + ex.Message + "\n", 0);
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

            // write_msg_log("\n 자동매매 중지 완료\n", 0);
        }
    }
}
