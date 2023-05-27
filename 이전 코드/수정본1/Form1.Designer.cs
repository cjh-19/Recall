
namespace Team1
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.axKHOpenAPI1 = new AxKHOpenAPILib.AxKHOpenAPI();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.로그인ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.로그아웃ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.accountbox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.idbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.deletebtn = new System.Windows.Forms.Button();
            this.alterbtn = new System.Windows.Forms.Button();
            this.insertbtn = new System.Windows.Forms.Button();
            this.searchbtn = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.seq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jongmok_cd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jongmok_nm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.priority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buy_amt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buy_price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.target_price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cut_loss_price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buy_trd_yn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.sell_trd_yn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.check = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.autostartbtn = new System.Windows.Forms.Button();
            this.autostopbtn = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.messagelog = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.errorlog = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.ResultList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_Relavance = new System.Windows.Forms.RadioButton();
            this.btn_date = new System.Windows.Forms.RadioButton();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_Update = new System.Windows.Forms.Button();
            this.Searchbox = new System.Windows.Forms.TextBox();
            this.labelDisplayCounts = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_DisplayCount = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_DisplayCount)).BeginInit();
            this.SuspendLayout();
            // 
            // axKHOpenAPI1
            // 
            this.axKHOpenAPI1.Enabled = true;
            this.axKHOpenAPI1.Location = new System.Drawing.Point(530, 78);
            this.axKHOpenAPI1.Name = "axKHOpenAPI1";
            this.axKHOpenAPI1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axKHOpenAPI1.OcxState")));
            this.axKHOpenAPI1.Size = new System.Drawing.Size(201, 25);
            this.axKHOpenAPI1.TabIndex = 0;
            this.axKHOpenAPI1.Visible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.로그인ToolStripMenuItem,
            this.로그아웃ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1632, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 로그인ToolStripMenuItem
            // 
            this.로그인ToolStripMenuItem.Name = "로그인ToolStripMenuItem";
            this.로그인ToolStripMenuItem.Size = new System.Drawing.Size(68, 26);
            this.로그인ToolStripMenuItem.Text = "로그인";
            this.로그인ToolStripMenuItem.Click += new System.EventHandler(this.로그인ToolStripMenuItem_Click);
            // 
            // 로그아웃ToolStripMenuItem
            // 
            this.로그아웃ToolStripMenuItem.Name = "로그아웃ToolStripMenuItem";
            this.로그아웃ToolStripMenuItem.Size = new System.Drawing.Size(83, 26);
            this.로그아웃ToolStripMenuItem.Text = "로그아웃";
            this.로그아웃ToolStripMenuItem.Click += new System.EventHandler(this.로그아웃ToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.accountbox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.idbox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(0, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(800, 67);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "접속정보창";
            // 
            // accountbox
            // 
            this.accountbox.FormattingEnabled = true;
            this.accountbox.Location = new System.Drawing.Point(328, 25);
            this.accountbox.Name = "accountbox";
            this.accountbox.Size = new System.Drawing.Size(165, 23);
            this.accountbox.TabIndex = 6;
            this.accountbox.SelectedIndexChanged += new System.EventHandler(this.accountbox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 10F);
            this.label2.ForeColor = System.Drawing.Color.Fuchsia;
            this.label2.Location = new System.Drawing.Point(246, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "계좌번호";
            // 
            // idbox
            // 
            this.idbox.Location = new System.Drawing.Point(71, 24);
            this.idbox.Name = "idbox";
            this.idbox.Size = new System.Drawing.Size(128, 25);
            this.idbox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 10F);
            this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "아이디";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.deletebtn);
            this.groupBox2.Controls.Add(this.alterbtn);
            this.groupBox2.Controls.Add(this.insertbtn);
            this.groupBox2.Controls.Add(this.searchbtn);
            this.groupBox2.Controls.Add(this.dataGridView1);
            this.groupBox2.Location = new System.Drawing.Point(0, 109);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(841, 316);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "거래종목 창";
            // 
            // deletebtn
            // 
            this.deletebtn.Location = new System.Drawing.Point(436, 22);
            this.deletebtn.Name = "deletebtn";
            this.deletebtn.Size = new System.Drawing.Size(110, 35);
            this.deletebtn.TabIndex = 4;
            this.deletebtn.Text = "삭제";
            this.deletebtn.UseVisualStyleBackColor = true;
            this.deletebtn.Click += new System.EventHandler(this.deletebtn_Click);
            // 
            // alterbtn
            // 
            this.alterbtn.Location = new System.Drawing.Point(295, 24);
            this.alterbtn.Name = "alterbtn";
            this.alterbtn.Size = new System.Drawing.Size(107, 34);
            this.alterbtn.TabIndex = 3;
            this.alterbtn.Text = "수정";
            this.alterbtn.UseVisualStyleBackColor = true;
            this.alterbtn.Click += new System.EventHandler(this.alterbtn_Click);
            // 
            // insertbtn
            // 
            this.insertbtn.Location = new System.Drawing.Point(146, 24);
            this.insertbtn.Name = "insertbtn";
            this.insertbtn.Size = new System.Drawing.Size(107, 31);
            this.insertbtn.TabIndex = 2;
            this.insertbtn.Text = "삽입";
            this.insertbtn.UseVisualStyleBackColor = true;
            this.insertbtn.Click += new System.EventHandler(this.insertbtn_Click);
            // 
            // searchbtn
            // 
            this.searchbtn.Location = new System.Drawing.Point(12, 24);
            this.searchbtn.Name = "searchbtn";
            this.searchbtn.Size = new System.Drawing.Size(100, 31);
            this.searchbtn.TabIndex = 1;
            this.searchbtn.Text = "조회";
            this.searchbtn.UseVisualStyleBackColor = true;
            this.searchbtn.Click += new System.EventHandler(this.searchbtn_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.seq,
            this.jongmok_cd,
            this.jongmok_nm,
            this.priority,
            this.buy_amt,
            this.buy_price,
            this.target_price,
            this.cut_loss_price,
            this.buy_trd_yn,
            this.sell_trd_yn,
            this.check});
            this.dataGridView1.Location = new System.Drawing.Point(0, 61);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 27;
            this.dataGridView1.Size = new System.Drawing.Size(821, 242);
            this.dataGridView1.TabIndex = 0;
            // 
            // seq
            // 
            this.seq.HeaderText = "순번";
            this.seq.MinimumWidth = 6;
            this.seq.Name = "seq";
            this.seq.Width = 60;
            // 
            // jongmok_cd
            // 
            this.jongmok_cd.HeaderText = "종목코드";
            this.jongmok_cd.MinimumWidth = 6;
            this.jongmok_cd.Name = "jongmok_cd";
            this.jongmok_cd.Width = 80;
            // 
            // jongmok_nm
            // 
            this.jongmok_nm.HeaderText = "종목명";
            this.jongmok_nm.MinimumWidth = 6;
            this.jongmok_nm.Name = "jongmok_nm";
            this.jongmok_nm.Width = 80;
            // 
            // priority
            // 
            this.priority.HeaderText = "우선순위";
            this.priority.MinimumWidth = 6;
            this.priority.Name = "priority";
            this.priority.Width = 80;
            // 
            // buy_amt
            // 
            this.buy_amt.HeaderText = "매수금액";
            this.buy_amt.MinimumWidth = 6;
            this.buy_amt.Name = "buy_amt";
            this.buy_amt.Width = 80;
            // 
            // buy_price
            // 
            this.buy_price.HeaderText = "매수가";
            this.buy_price.MinimumWidth = 6;
            this.buy_price.Name = "buy_price";
            this.buy_price.Width = 70;
            // 
            // target_price
            // 
            this.target_price.HeaderText = "목표가";
            this.target_price.MinimumWidth = 6;
            this.target_price.Name = "target_price";
            this.target_price.Width = 70;
            // 
            // cut_loss_price
            // 
            this.cut_loss_price.HeaderText = "손절가";
            this.cut_loss_price.MinimumWidth = 6;
            this.cut_loss_price.Name = "cut_loss_price";
            this.cut_loss_price.Width = 70;
            // 
            // buy_trd_yn
            // 
            this.buy_trd_yn.HeaderText = "매수여부";
            this.buy_trd_yn.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.buy_trd_yn.MinimumWidth = 6;
            this.buy_trd_yn.Name = "buy_trd_yn";
            this.buy_trd_yn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.buy_trd_yn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.buy_trd_yn.Width = 80;
            // 
            // sell_trd_yn
            // 
            this.sell_trd_yn.HeaderText = "매도여부";
            this.sell_trd_yn.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.sell_trd_yn.MinimumWidth = 6;
            this.sell_trd_yn.Name = "sell_trd_yn";
            this.sell_trd_yn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.sell_trd_yn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.sell_trd_yn.Width = 80;
            // 
            // check
            // 
            this.check.HeaderText = "체크";
            this.check.MinimumWidth = 6;
            this.check.Name = "check";
            this.check.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.check.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.check.Width = 60;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.autostartbtn);
            this.groupBox3.Controls.Add(this.autostopbtn);
            this.groupBox3.Location = new System.Drawing.Point(1, 431);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(843, 65);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "자동매매";
            // 
            // autostartbtn
            // 
            this.autostartbtn.Location = new System.Drawing.Point(95, 23);
            this.autostartbtn.Name = "autostartbtn";
            this.autostartbtn.Size = new System.Drawing.Size(259, 33);
            this.autostartbtn.TabIndex = 1;
            this.autostartbtn.Text = "자동매매 시작";
            this.autostartbtn.UseVisualStyleBackColor = true;
            this.autostartbtn.Click += new System.EventHandler(this.autostartbtn_Click);
            // 
            // autostopbtn
            // 
            this.autostopbtn.Location = new System.Drawing.Point(448, 24);
            this.autostopbtn.Name = "autostopbtn";
            this.autostopbtn.Size = new System.Drawing.Size(282, 31);
            this.autostopbtn.TabIndex = 0;
            this.autostopbtn.Text = "자동매매 중지";
            this.autostopbtn.UseVisualStyleBackColor = true;
            this.autostopbtn.Click += new System.EventHandler(this.autostopbtn_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.messagelog);
            this.groupBox4.Location = new System.Drawing.Point(0, 502);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(389, 185);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "메시지 로그";
            // 
            // messagelog
            // 
            this.messagelog.BackColor = System.Drawing.Color.Black;
            this.messagelog.ForeColor = System.Drawing.Color.Yellow;
            this.messagelog.Location = new System.Drawing.Point(1, 24);
            this.messagelog.Multiline = true;
            this.messagelog.Name = "messagelog";
            this.messagelog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.messagelog.Size = new System.Drawing.Size(382, 160);
            this.messagelog.TabIndex = 5;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.errorlog);
            this.groupBox5.Location = new System.Drawing.Point(406, 502);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(400, 189);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "에러 로그";
            // 
            // errorlog
            // 
            this.errorlog.BackColor = System.Drawing.Color.Black;
            this.errorlog.ForeColor = System.Drawing.Color.Yellow;
            this.errorlog.Location = new System.Drawing.Point(5, 26);
            this.errorlog.Multiline = true;
            this.errorlog.Name = "errorlog";
            this.errorlog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.errorlog.Size = new System.Drawing.Size(428, 157);
            this.errorlog.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 707);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1632, 26);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(179, 20);
            this.toolStripStatusLabel1.Text = "자동매매 프로그램입니다";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.ResultList);
            this.groupBox6.Location = new System.Drawing.Point(847, 144);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox6.Size = new System.Drawing.Size(747, 368);
            this.groupBox6.TabIndex = 9;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "검색 결과";
            // 
            // ResultList
            // 
            this.ResultList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.ResultList.FullRowSelect = true;
            this.ResultList.HideSelection = false;
            this.ResultList.Location = new System.Drawing.Point(10, 25);
            this.ResultList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ResultList.MultiSelect = false;
            this.ResultList.Name = "ResultList";
            this.ResultList.Size = new System.Drawing.Size(726, 326);
            this.ResultList.TabIndex = 6;
            this.ResultList.UseCompatibleStateImageBehavior = false;
            this.ResultList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "No.";
            this.columnHeader1.Width = 35;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Title";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Main Text";
            this.columnHeader3.Width = 350;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Link";
            this.columnHeader4.Width = 188;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label3);
            this.groupBox7.Controls.Add(this.btn_Relavance);
            this.groupBox7.Controls.Add(this.btn_date);
            this.groupBox7.Controls.Add(this.groupBox8);
            this.groupBox7.Controls.Add(this.label4);
            this.groupBox7.Controls.Add(this.btn_Update);
            this.groupBox7.Controls.Add(this.Searchbox);
            this.groupBox7.Controls.Add(this.labelDisplayCounts);
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.Controls.Add(this.btn_DisplayCount);
            this.groupBox7.Location = new System.Drawing.Point(847, 34);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox7.Size = new System.Drawing.Size(747, 102);
            this.groupBox7.TabIndex = 8;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "검색 옵션";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(297, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "정렬 방법";
            // 
            // btn_Relavance
            // 
            this.btn_Relavance.AutoSize = true;
            this.btn_Relavance.Location = new System.Drawing.Point(299, 69);
            this.btn_Relavance.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Relavance.Name = "btn_Relavance";
            this.btn_Relavance.Size = new System.Drawing.Size(73, 19);
            this.btn_Relavance.TabIndex = 8;
            this.btn_Relavance.Text = "관련성";
            this.btn_Relavance.UseVisualStyleBackColor = true;
            // 
            // btn_date
            // 
            this.btn_date.AutoSize = true;
            this.btn_date.Checked = true;
            this.btn_date.Location = new System.Drawing.Point(299, 45);
            this.btn_date.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_date.Name = "btn_date";
            this.btn_date.Size = new System.Drawing.Size(58, 19);
            this.btn_date.TabIndex = 8;
            this.btn_date.TabStop = true;
            this.btn_date.Text = "날짜";
            this.btn_date.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Location = new System.Drawing.Point(0, 110);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox8.Size = new System.Drawing.Size(754, 366);
            this.groupBox8.TabIndex = 7;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "groupBox8";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(411, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(142, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "검색 결과 출력 개수";
            // 
            // btn_Update
            // 
            this.btn_Update.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btn_Update.Location = new System.Drawing.Point(587, 18);
            this.btn_Update.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(149, 70);
            this.btn_Update.TabIndex = 0;
            this.btn_Update.Text = "업데이트";
            this.btn_Update.UseVisualStyleBackColor = false;
            this.btn_Update.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // Searchbox
            // 
            this.Searchbox.Location = new System.Drawing.Point(50, 41);
            this.Searchbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Searchbox.Name = "Searchbox";
            this.Searchbox.Size = new System.Drawing.Size(239, 25);
            this.Searchbox.TabIndex = 1;
            this.Searchbox.Text = "원하는 검색 키워드를 입력하세요";
            // 
            // labelDisplayCounts
            // 
            this.labelDisplayCounts.AutoSize = true;
            this.labelDisplayCounts.Location = new System.Drawing.Point(547, 22);
            this.labelDisplayCounts.Name = "labelDisplayCounts";
            this.labelDisplayCounts.Size = new System.Drawing.Size(23, 15);
            this.labelDisplayCounts.TabIndex = 4;
            this.labelDisplayCounts.Text = "10";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "검색: ";
            // 
            // btn_DisplayCount
            // 
            this.btn_DisplayCount.Location = new System.Drawing.Point(413, 42);
            this.btn_DisplayCount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_DisplayCount.Maximum = 100;
            this.btn_DisplayCount.Minimum = 10;
            this.btn_DisplayCount.Name = "btn_DisplayCount";
            this.btn_DisplayCount.Size = new System.Drawing.Size(168, 56);
            this.btn_DisplayCount.TabIndex = 3;
            this.btn_DisplayCount.TickFrequency = 10;
            this.btn_DisplayCount.Value = 10;
            this.btn_DisplayCount.Scroll += new System.EventHandler(this.trackBarDisplayCounts_Scroll);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1632, 733);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.axKHOpenAPI1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "c##team";
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_DisplayCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 로그인ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 로그아웃ToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox accountbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox idbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button deletebtn;
        private System.Windows.Forms.Button alterbtn;
        private System.Windows.Forms.Button insertbtn;
        private System.Windows.Forms.Button searchbtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn seq;
        private System.Windows.Forms.DataGridViewTextBoxColumn jongmok_cd;
        private System.Windows.Forms.DataGridViewTextBoxColumn jongmok_nm;
        private System.Windows.Forms.DataGridViewTextBoxColumn priority;
        private System.Windows.Forms.DataGridViewTextBoxColumn buy_amt;
        private System.Windows.Forms.DataGridViewTextBoxColumn buy_price;
        private System.Windows.Forms.DataGridViewTextBoxColumn target_price;
        private System.Windows.Forms.DataGridViewTextBoxColumn cut_loss_price;
        private System.Windows.Forms.DataGridViewComboBoxColumn buy_trd_yn;
        private System.Windows.Forms.DataGridViewComboBoxColumn sell_trd_yn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn check;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button autostartbtn;
        private System.Windows.Forms.Button autostopbtn;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox messagelog;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox errorlog;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ListView ResultList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton btn_Relavance;
        private System.Windows.Forms.RadioButton btn_date;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_Update;
        private System.Windows.Forms.TextBox Searchbox;
        private System.Windows.Forms.Label labelDisplayCounts;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar btn_DisplayCount;
    }
}

