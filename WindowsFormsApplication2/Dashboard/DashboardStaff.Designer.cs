namespace WindowsFormsApplication2.Dashboard
{
    partial class DashboardStaff
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashboardStaff));
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel10 = new System.Windows.Forms.Panel();
            this.lblDailyCnt = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panel28 = new System.Windows.Forms.Panel();
            this.lblWorkCnt = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.myWorkList = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.worklistStaff1 = new WindowsFormsApplication2.Dashboard.DashboardControls.WorklistStaff();
            this.dailyProductivityStaff1 = new WindowsFormsApplication2.Dashboard.DashboardControls.DailyProductivityStaff();
            this.timerAccntg = new System.Windows.Forms.Timer(this.components);
            this.timerAudit = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.panel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.panel28.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(141)))), ((int)(((byte)(188)))));
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(3, 956);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1543, 3);
            this.panel4.TabIndex = 27;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(141)))), ((int)(((byte)(188)))));
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(1546, 43);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(3, 916);
            this.panel3.TabIndex = 26;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(141)))), ((int)(((byte)(188)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 43);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(3, 916);
            this.panel2.TabIndex = 25;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(141)))), ((int)(((byte)(188)))));
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1549, 43);
            this.panel1.TabIndex = 24;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 23);
            this.label2.TabIndex = 4;
            this.label2.Text = "Dashboard";
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
            this.btnClose.ForeColor = System.Drawing.Color.Red;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(1380, 905);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(134, 44);
            this.btnClose.TabIndex = 23;
            this.btnClose.Text = "CLOSE";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.SeaGreen;
            this.panel10.Controls.Add(this.lblDailyCnt);
            this.panel10.Controls.Add(this.panel11);
            this.panel10.Controls.Add(this.pictureBox3);
            this.panel10.Controls.Add(this.label8);
            this.panel10.Location = new System.Drawing.Point(411, 52);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(357, 79);
            this.panel10.TabIndex = 29;
            // 
            // lblDailyCnt
            // 
            this.lblDailyCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDailyCnt.AutoSize = true;
            this.lblDailyCnt.Font = new System.Drawing.Font("Century Gothic", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDailyCnt.ForeColor = System.Drawing.Color.White;
            this.lblDailyCnt.Location = new System.Drawing.Point(262, 21);
            this.lblDailyCnt.Name = "lblDailyCnt";
            this.lblDailyCnt.Size = new System.Drawing.Size(74, 40);
            this.lblDailyCnt.TabIndex = 23;
            this.lblDailyCnt.Text = "100";
            // 
            // panel11
            // 
            this.panel11.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel11.Location = new System.Drawing.Point(342, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(15, 79);
            this.panel11.TabIndex = 22;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(0, 0);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(86, 79);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 20;
            this.pictureBox3.TabStop = false;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label8.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(94, 8);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(146, 56);
            this.label8.TabIndex = 19;
            this.label8.Text = "Daily\r\nProductivity";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // panel28
            // 
            this.panel28.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.panel28.Controls.Add(this.lblWorkCnt);
            this.panel28.Controls.Add(this.panel5);
            this.panel28.Controls.Add(this.pictureBox4);
            this.panel28.Controls.Add(this.myWorkList);
            this.panel28.Location = new System.Drawing.Point(38, 52);
            this.panel28.Name = "panel28";
            this.panel28.Size = new System.Drawing.Size(357, 79);
            this.panel28.TabIndex = 28;
            // 
            // lblWorkCnt
            // 
            this.lblWorkCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWorkCnt.AutoSize = true;
            this.lblWorkCnt.Font = new System.Drawing.Font("Century Gothic", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWorkCnt.ForeColor = System.Drawing.Color.White;
            this.lblWorkCnt.Location = new System.Drawing.Point(264, 22);
            this.lblWorkCnt.Name = "lblWorkCnt";
            this.lblWorkCnt.Size = new System.Drawing.Size(74, 40);
            this.lblWorkCnt.TabIndex = 23;
            this.lblWorkCnt.Text = "100";
            // 
            // panel5
            // 
            this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel5.Location = new System.Drawing.Point(342, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(15, 79);
            this.panel5.TabIndex = 22;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(0, 0);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(86, 79);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 20;
            this.pictureBox4.TabStop = false;
            // 
            // myWorkList
            // 
            this.myWorkList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myWorkList.AutoSize = true;
            this.myWorkList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.myWorkList.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.myWorkList.ForeColor = System.Drawing.Color.White;
            this.myWorkList.Location = new System.Drawing.Point(89, 29);
            this.myWorkList.Name = "myWorkList";
            this.myWorkList.Size = new System.Drawing.Size(155, 28);
            this.myWorkList.TabIndex = 19;
            this.myWorkList.Text = "My Worklists";
            this.myWorkList.Click += new System.EventHandler(this.myWorkList_Click);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.worklistStaff1);
            this.panel6.Controls.Add(this.dailyProductivityStaff1);
            this.panel6.Location = new System.Drawing.Point(38, 137);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1476, 760);
            this.panel6.TabIndex = 30;
            // 
            // worklistStaff1
            // 
            this.worklistStaff1.BackColor = System.Drawing.Color.White;
            this.worklistStaff1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.worklistStaff1.Location = new System.Drawing.Point(0, 0);
            this.worklistStaff1.Name = "worklistStaff1";
            this.worklistStaff1.Size = new System.Drawing.Size(1476, 760);
            this.worklistStaff1.TabIndex = 1;
            this.worklistStaff1.Load += new System.EventHandler(this.worklistStaff1_Load);
            // 
            // dailyProductivityStaff1
            // 
            this.dailyProductivityStaff1.BackColor = System.Drawing.Color.White;
            this.dailyProductivityStaff1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dailyProductivityStaff1.Location = new System.Drawing.Point(0, 0);
            this.dailyProductivityStaff1.Name = "dailyProductivityStaff1";
            this.dailyProductivityStaff1.Size = new System.Drawing.Size(1476, 760);
            this.dailyProductivityStaff1.TabIndex = 0;
            // 
            // timerAccntg
            // 
            this.timerAccntg.Interval = 5000;
            this.timerAccntg.Tick += new System.EventHandler(this.timerAccntg_Tick);
            // 
            // timerAudit
            // 
            this.timerAudit.Interval = 5000;
            this.timerAudit.Tick += new System.EventHandler(this.timerAudit_Tick);
            // 
            // DashboardStaff
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1549, 959);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel10);
            this.Controls.Add(this.panel28);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DashboardStaff";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DashboardStaff";
            this.Load += new System.EventHandler(this.DashboardStaff_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.panel28.ResumeLayout(false);
            this.panel28.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Label lblDailyCnt;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel28;
        private System.Windows.Forms.Label lblWorkCnt;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label myWorkList;
        private System.Windows.Forms.Panel panel6;
        private DashboardControls.WorklistStaff worklistStaff1;
        private DashboardControls.DailyProductivityStaff dailyProductivityStaff1;
        public System.Windows.Forms.Timer timerAccntg;
        public System.Windows.Forms.Timer timerAudit;
    }
}