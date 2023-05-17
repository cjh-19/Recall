namespace SearchboxPage
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
            this.btn_Update = new System.Windows.Forms.Button();
            this.Searchbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_DisplayCount = new System.Windows.Forms.TrackBar();
            this.labelDisplayCounts = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Relavance = new System.Windows.Forms.RadioButton();
            this.btn_date = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ResultList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.btn_DisplayCount)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
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
            this.Searchbox.TextChanged += new System.EventHandler(this.textBoxKeyword_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "검색: ";
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
            // labelDisplayCounts
            // 
            this.labelDisplayCounts.AutoSize = true;
            this.labelDisplayCounts.Location = new System.Drawing.Point(547, 22);
            this.labelDisplayCounts.Name = "labelDisplayCounts";
            this.labelDisplayCounts.Size = new System.Drawing.Size(23, 15);
            this.labelDisplayCounts.TabIndex = 4;
            this.labelDisplayCounts.Text = "10";
            this.labelDisplayCounts.Click += new System.EventHandler(this.labelDisplayCounts_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(411, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "검색 결과 출력 개수";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btn_Relavance);
            this.groupBox1.Controls.Add(this.btn_date);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btn_Update);
            this.groupBox1.Controls.Add(this.Searchbox);
            this.groupBox1.Controls.Add(this.labelDisplayCounts);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btn_DisplayCount);
            this.groupBox1.Location = new System.Drawing.Point(14, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(747, 102);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "검색 옵션";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(297, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "정렬 방법";
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
            this.btn_Relavance.CheckedChanged += new System.EventHandler(this.radioButtonSim_CheckedChanged);
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
            this.btn_date.CheckedChanged += new System.EventHandler(this.radioButtonDate_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(0, 110);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(754, 366);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
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
            this.ResultList.SelectedIndexChanged += new System.EventHandler(this.listViewResults_SelectedIndexChanged);
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ResultList);
            this.groupBox3.Location = new System.Drawing.Point(14, 125);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Size = new System.Drawing.Size(747, 368);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "검색 결과";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 500);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.btn_DisplayCount)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Update;
        private System.Windows.Forms.TextBox Searchbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar btn_DisplayCount;
        private System.Windows.Forms.Label labelDisplayCounts;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView ResultList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton btn_Relavance;
        private System.Windows.Forms.RadioButton btn_date;
    }
}

