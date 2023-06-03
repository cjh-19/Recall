
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.logoutbtn = new System.Windows.Forms.Button();
            this.loginbtn = new System.Windows.Forms.Button();
            this.accountbox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.idbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
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
            this.searchbtn = new System.Windows.Forms.Button();
            this.autostopbtn = new System.Windows.Forms.Button();
            this.insertbtn = new System.Windows.Forms.Button();
            this.autostartbtn = new System.Windows.Forms.Button();
            this.deletebtn = new System.Windows.Forms.Button();
            this.alterbtn = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.messagelog = new System.Windows.Forms.TextBox();
            this.axKHOpenAPI1 = new AxKHOpenAPILib.AxKHOpenAPI();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.errorlog = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.ResultList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_Relavance = new System.Windows.Forms.RadioButton();
            this.btn_date = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_Update = new System.Windows.Forms.Button();
            this.Searchbox = new System.Windows.Forms.TextBox();
            this.labelDisplayCounts = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_DisplayCount = new System.Windows.Forms.TrackBar();
            this.clocklabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_DisplayCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.logoutbtn);
            this.groupBox1.Controls.Add(this.loginbtn);
            this.groupBox1.Controls.Add(this.accountbox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.idbox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(800, 68);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "사용자 접속 정보";
            // 
            // logoutbtn
            // 
            this.logoutbtn.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.logoutbtn.Location = new System.Drawing.Point(674, 22);
            this.logoutbtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.logoutbtn.Name = "logoutbtn";
            this.logoutbtn.Size = new System.Drawing.Size(106, 32);
            this.logoutbtn.TabIndex = 8;
            this.logoutbtn.Text = "종료";
            this.logoutbtn.UseVisualStyleBackColor = false;
            this.logoutbtn.Click += new System.EventHandler(this.logoutbtn_Click);
            // 
            // loginbtn
            // 
            this.loginbtn.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.loginbtn.Location = new System.Drawing.Point(537, 22);
            this.loginbtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.loginbtn.Name = "loginbtn";
            this.loginbtn.Size = new System.Drawing.Size(106, 32);
            this.loginbtn.TabIndex = 7;
            this.loginbtn.Text = "로그인";
            this.loginbtn.UseVisualStyleBackColor = false;
            this.loginbtn.Click += new System.EventHandler(this.loginbtn_Click);
            // 
            // accountbox
            // 
            this.accountbox.FormattingEnabled = true;
            this.accountbox.Location = new System.Drawing.Point(328, 25);
            this.accountbox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.accountbox.Name = "accountbox";
            this.accountbox.Size = new System.Drawing.Size(165, 23);
            this.accountbox.TabIndex = 6;
            this.accountbox.SelectedIndexChanged += new System.EventHandler(this.accountbox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.Sienna;
            this.label2.Location = new System.Drawing.Point(246, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "계좌번호";
            // 
            // idbox
            // 
            this.idbox.Location = new System.Drawing.Point(71, 24);
            this.idbox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.idbox.Name = "idbox";
            this.idbox.Size = new System.Drawing.Size(127, 25);
            this.idbox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label1.Location = new System.Drawing.Point(8, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "아이디";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridView1);
            this.groupBox2.Controls.Add(this.searchbtn);
            this.groupBox2.Controls.Add(this.autostopbtn);
            this.groupBox2.Controls.Add(this.insertbtn);
            this.groupBox2.Controls.Add(this.autostartbtn);
            this.groupBox2.Controls.Add(this.deletebtn);
            this.groupBox2.Controls.Add(this.alterbtn);
            this.groupBox2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox2.Location = new System.Drawing.Point(588, 87);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(1143, 491);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "거래종목이 표시됩니다";
            // 
            // dataGridView1
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.Silver;
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
            this.dataGridView1.Location = new System.Drawing.Point(14, 24);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 27;
            this.dataGridView1.Size = new System.Drawing.Size(972, 450);
            this.dataGridView1.TabIndex = 0;
            // 
            // seq
            // 
            this.seq.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.seq.FillWeight = 85.3747F;
            this.seq.HeaderText = "순번";
            this.seq.MinimumWidth = 6;
            this.seq.Name = "seq";
            // 
            // jongmok_cd
            // 
            this.jongmok_cd.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.jongmok_cd.FillWeight = 115.493F;
            this.jongmok_cd.HeaderText = "종목코드";
            this.jongmok_cd.MinimumWidth = 6;
            this.jongmok_cd.Name = "jongmok_cd";
            // 
            // jongmok_nm
            // 
            this.jongmok_nm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.jongmok_nm.FillWeight = 95.3685F;
            this.jongmok_nm.HeaderText = "종목명";
            this.jongmok_nm.MinimumWidth = 6;
            this.jongmok_nm.Name = "jongmok_nm";
            // 
            // priority
            // 
            this.priority.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.priority.FillWeight = 116.0443F;
            this.priority.HeaderText = "우선순위";
            this.priority.MinimumWidth = 6;
            this.priority.Name = "priority";
            // 
            // buy_amt
            // 
            this.buy_amt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.buy_amt.FillWeight = 117.9032F;
            this.buy_amt.HeaderText = "투자금액";
            this.buy_amt.MinimumWidth = 6;
            this.buy_amt.Name = "buy_amt";
            // 
            // buy_price
            // 
            this.buy_price.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.buy_price.FillWeight = 95.3685F;
            this.buy_price.HeaderText = "매수가";
            this.buy_price.MinimumWidth = 6;
            this.buy_price.Name = "buy_price";
            // 
            // target_price
            // 
            this.target_price.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.target_price.FillWeight = 95.3685F;
            this.target_price.HeaderText = "목표가";
            this.target_price.MinimumWidth = 6;
            this.target_price.Name = "target_price";
            // 
            // cut_loss_price
            // 
            this.cut_loss_price.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cut_loss_price.FillWeight = 95.3685F;
            this.cut_loss_price.HeaderText = "손절가";
            this.cut_loss_price.MinimumWidth = 6;
            this.cut_loss_price.Name = "cut_loss_price";
            // 
            // buy_trd_yn
            // 
            this.buy_trd_yn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.buy_trd_yn.FillWeight = 94.22467F;
            this.buy_trd_yn.HeaderText = "매수유무";
            this.buy_trd_yn.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.buy_trd_yn.MinimumWidth = 6;
            this.buy_trd_yn.Name = "buy_trd_yn";
            this.buy_trd_yn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.buy_trd_yn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // sell_trd_yn
            // 
            this.sell_trd_yn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.sell_trd_yn.FillWeight = 94.11765F;
            this.sell_trd_yn.HeaderText = "매도유무";
            this.sell_trd_yn.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.sell_trd_yn.MinimumWidth = 6;
            this.sell_trd_yn.Name = "sell_trd_yn";
            this.sell_trd_yn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.sell_trd_yn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // check
            // 
            this.check.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.check.FillWeight = 95.3685F;
            this.check.HeaderText = "체크";
            this.check.MinimumWidth = 6;
            this.check.Name = "check";
            this.check.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.check.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // searchbtn
            // 
            this.searchbtn.BackColor = System.Drawing.Color.Silver;
            this.searchbtn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.searchbtn.Location = new System.Drawing.Point(992, 24);
            this.searchbtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.searchbtn.Name = "searchbtn";
            this.searchbtn.Size = new System.Drawing.Size(143, 70);
            this.searchbtn.TabIndex = 1;
            this.searchbtn.Text = "불러오기";
            this.searchbtn.UseVisualStyleBackColor = false;
            this.searchbtn.Click += new System.EventHandler(this.searchbtn_Click);
            // 
            // autostopbtn
            // 
            this.autostopbtn.BackColor = System.Drawing.Color.Red;
            this.autostopbtn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.autostopbtn.Location = new System.Drawing.Point(992, 404);
            this.autostopbtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.autostopbtn.Name = "autostopbtn";
            this.autostopbtn.Size = new System.Drawing.Size(145, 70);
            this.autostopbtn.TabIndex = 0;
            this.autostopbtn.Text = "자동매매 중지";
            this.autostopbtn.UseVisualStyleBackColor = false;
            this.autostopbtn.Click += new System.EventHandler(this.autostopbtn_Click);
            // 
            // insertbtn
            // 
            this.insertbtn.BackColor = System.Drawing.Color.Gray;
            this.insertbtn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.insertbtn.Location = new System.Drawing.Point(992, 102);
            this.insertbtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.insertbtn.Name = "insertbtn";
            this.insertbtn.Size = new System.Drawing.Size(145, 70);
            this.insertbtn.TabIndex = 2;
            this.insertbtn.Text = "종목 삽입";
            this.insertbtn.UseVisualStyleBackColor = false;
            this.insertbtn.Click += new System.EventHandler(this.insertbtn_Click);
            // 
            // autostartbtn
            // 
            this.autostartbtn.BackColor = System.Drawing.Color.Gray;
            this.autostartbtn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.autostartbtn.Location = new System.Drawing.Point(992, 330);
            this.autostartbtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.autostartbtn.Name = "autostartbtn";
            this.autostartbtn.Size = new System.Drawing.Size(145, 70);
            this.autostartbtn.TabIndex = 1;
            this.autostartbtn.Text = "자동매매 예약";
            this.autostartbtn.UseVisualStyleBackColor = false;
            this.autostartbtn.Click += new System.EventHandler(this.autostartbtn_Click);
            // 
            // deletebtn
            // 
            this.deletebtn.BackColor = System.Drawing.Color.Gray;
            this.deletebtn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.deletebtn.Location = new System.Drawing.Point(992, 176);
            this.deletebtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.deletebtn.Name = "deletebtn";
            this.deletebtn.Size = new System.Drawing.Size(143, 70);
            this.deletebtn.TabIndex = 4;
            this.deletebtn.Text = "종목 제거";
            this.deletebtn.UseVisualStyleBackColor = false;
            this.deletebtn.Click += new System.EventHandler(this.deletebtn_Click);
            // 
            // alterbtn
            // 
            this.alterbtn.BackColor = System.Drawing.Color.Gray;
            this.alterbtn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.alterbtn.Location = new System.Drawing.Point(992, 252);
            this.alterbtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.alterbtn.Name = "alterbtn";
            this.alterbtn.Size = new System.Drawing.Size(143, 70);
            this.alterbtn.TabIndex = 3;
            this.alterbtn.Text = "종목 수정";
            this.alterbtn.UseVisualStyleBackColor = false;
            this.alterbtn.Click += new System.EventHandler(this.alterbtn_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.messagelog);
            this.groupBox4.Controls.Add(this.axKHOpenAPI1);
            this.groupBox4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox4.Location = new System.Drawing.Point(12, 87);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox4.Size = new System.Drawing.Size(570, 211);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "실시간 메시지 정보창";
            // 
            // messagelog
            // 
            this.messagelog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messagelog.BackColor = System.Drawing.Color.Black;
            this.messagelog.Font = new System.Drawing.Font("굴림", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.messagelog.ForeColor = System.Drawing.Color.Yellow;
            this.messagelog.Location = new System.Drawing.Point(7, 21);
            this.messagelog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.messagelog.Multiline = true;
            this.messagelog.Name = "messagelog";
            this.messagelog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.messagelog.Size = new System.Drawing.Size(551, 173);
            this.messagelog.TabIndex = 5;
            // 
            // axKHOpenAPI1
            // 
            this.axKHOpenAPI1.Enabled = true;
            this.axKHOpenAPI1.Location = new System.Drawing.Point(232, -14);
            this.axKHOpenAPI1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.axKHOpenAPI1.Name = "axKHOpenAPI1";
            this.axKHOpenAPI1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axKHOpenAPI1.OcxState")));
            this.axKHOpenAPI1.Size = new System.Drawing.Size(251, 31);
            this.axKHOpenAPI1.TabIndex = 0;
            this.axKHOpenAPI1.Visible = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.errorlog);
            this.groupBox5.Location = new System.Drawing.Point(12, 302);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox5.Size = new System.Drawing.Size(570, 185);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "에러 창";
            // 
            // errorlog
            // 
            this.errorlog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errorlog.BackColor = System.Drawing.Color.Black;
            this.errorlog.ForeColor = System.Drawing.Color.Red;
            this.errorlog.Location = new System.Drawing.Point(7, 22);
            this.errorlog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.errorlog.Multiline = true;
            this.errorlog.Name = "errorlog";
            this.errorlog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.errorlog.Size = new System.Drawing.Size(551, 150);
            this.errorlog.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 1029);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1796, 26);
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
            this.groupBox6.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox6.Location = new System.Drawing.Point(11, 668);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox6.Size = new System.Drawing.Size(921, 277);
            this.groupBox6.TabIndex = 9;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "최신 뉴스 검색 결과";
            // 
            // ResultList
            // 
            this.ResultList.BackColor = System.Drawing.Color.CadetBlue;
            this.ResultList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.ResultList.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ResultList.FullRowSelect = true;
            this.ResultList.HideSelection = false;
            this.ResultList.Location = new System.Drawing.Point(6, 18);
            this.ResultList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ResultList.MultiSelect = false;
            this.ResultList.Name = "ResultList";
            this.ResultList.Size = new System.Drawing.Size(904, 251);
            this.ResultList.TabIndex = 6;
            this.ResultList.UseCompatibleStateImageBehavior = false;
            this.ResultList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Link";
            this.columnHeader1.Width = 70;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Title";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Main Text";
            this.columnHeader3.Width = 630;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label3);
            this.groupBox7.Controls.Add(this.btn_Relavance);
            this.groupBox7.Controls.Add(this.btn_date);
            this.groupBox7.Controls.Add(this.label4);
            this.groupBox7.Controls.Add(this.btn_Update);
            this.groupBox7.Controls.Add(this.Searchbox);
            this.groupBox7.Controls.Add(this.labelDisplayCounts);
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.Controls.Add(this.btn_DisplayCount);
            this.groupBox7.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox7.Location = new System.Drawing.Point(11, 493);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox7.Size = new System.Drawing.Size(571, 167);
            this.groupBox7.TabIndex = 8;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "뉴스 검색옵션";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(201, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "정렬 방법";
            // 
            // btn_Relavance
            // 
            this.btn_Relavance.AutoSize = true;
            this.btn_Relavance.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Relavance.Location = new System.Drawing.Point(203, 131);
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
            this.btn_date.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_date.Location = new System.Drawing.Point(203, 107);
            this.btn_date.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_date.Name = "btn_date";
            this.btn_date.Size = new System.Drawing.Size(58, 19);
            this.btn_date.TabIndex = 8;
            this.btn_date.TabStop = true;
            this.btn_date.Text = "날짜";
            this.btn_date.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(6, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(147, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "검색 결과 출력 개수:";
            // 
            // btn_Update
            // 
            this.btn_Update.BackColor = System.Drawing.Color.CadetBlue;
            this.btn_Update.Location = new System.Drawing.Point(328, 74);
            this.btn_Update.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(231, 84);
            this.btn_Update.TabIndex = 0;
            this.btn_Update.Text = "검색";
            this.btn_Update.UseVisualStyleBackColor = false;
            this.btn_Update.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // Searchbox
            // 
            this.Searchbox.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Searchbox.ForeColor = System.Drawing.Color.Silver;
            this.Searchbox.Location = new System.Drawing.Point(50, 41);
            this.Searchbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Searchbox.Name = "Searchbox";
            this.Searchbox.Size = new System.Drawing.Size(509, 25);
            this.Searchbox.TabIndex = 1;
            this.Searchbox.Text = "News Search.";
            this.Searchbox.Enter += new System.EventHandler(this.Searchbox_Enter);
            this.Searchbox.Leave += new System.EventHandler(this.Searchbox_Leave);
            // 
            // labelDisplayCounts
            // 
            this.labelDisplayCounts.AutoSize = true;
            this.labelDisplayCounts.Font = new System.Drawing.Font("굴림", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDisplayCounts.Location = new System.Drawing.Point(149, 83);
            this.labelDisplayCounts.Name = "labelDisplayCounts";
            this.labelDisplayCounts.Size = new System.Drawing.Size(25, 15);
            this.labelDisplayCounts.TabIndex = 4;
            this.labelDisplayCounts.Text = "10";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "검색: ";
            // 
            // btn_DisplayCount
            // 
            this.btn_DisplayCount.Location = new System.Drawing.Point(8, 102);
            this.btn_DisplayCount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_DisplayCount.Maximum = 100;
            this.btn_DisplayCount.Minimum = 10;
            this.btn_DisplayCount.Name = "btn_DisplayCount";
            this.btn_DisplayCount.Size = new System.Drawing.Size(181, 56);
            this.btn_DisplayCount.TabIndex = 3;
            this.btn_DisplayCount.TickFrequency = 10;
            this.btn_DisplayCount.Value = 10;
            this.btn_DisplayCount.Scroll += new System.EventHandler(this.trackBarDisplayCounts_Scroll);
            // 
            // clocklabel
            // 
            this.clocklabel.AutoSize = true;
            this.clocklabel.BackColor = System.Drawing.Color.Linen;
            this.clocklabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.clocklabel.ForeColor = System.Drawing.Color.White;
            this.clocklabel.Location = new System.Drawing.Point(837, 28);
            this.clocklabel.Name = "clocklabel";
            this.clocklabel.Size = new System.Drawing.Size(78, 44);
            this.clocklabel.TabIndex = 10;
            this.clocklabel.Text = "시계";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label6.Font = new System.Drawing.Font("궁서", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label6.Location = new System.Drawing.Point(67, 835);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 30);
            this.label6.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(1458, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(273, 36);
            this.label7.TabIndex = 12;
            this.label7.Text = " 7조 다시들으시려면# ";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Team1.Properties.Resources.다운로드;
            this.pictureBox1.Location = new System.Drawing.Point(961, 600);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(770, 345);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Linen;
            this.ClientSize = new System.Drawing.Size(1796, 1055);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.clocklabel);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox1);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "c##team";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_DisplayCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI1;
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
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton btn_Relavance;
        private System.Windows.Forms.RadioButton btn_date;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_Update;
        private System.Windows.Forms.TextBox Searchbox;
        private System.Windows.Forms.Label labelDisplayCounts;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar btn_DisplayCount;
        private System.Windows.Forms.Button logoutbtn;
        private System.Windows.Forms.Button loginbtn;
        private System.Windows.Forms.Label clocklabel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ColumnHeader columnHeader1;
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
        private System.Windows.Forms.BindingSource bindingSource1;
    }
}

