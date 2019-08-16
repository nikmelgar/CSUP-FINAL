namespace WindowsFormsApplication2.Dashboard
{
    partial class DashboardTH
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashboardTH));
            this.panel28 = new System.Windows.Forms.Panel();
            this.lblWorkCnt = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.myWorkList = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.lblTeamCnt = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.myTeamWorklist = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.lblIncoming = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel10 = new System.Windows.Forms.Panel();
            this.lblDailyCnt = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.timerRefreshAccntg = new System.Windows.Forms.Timer(this.components);
            this.timerAudit = new System.Windows.Forms.Timer(this.components);
            this.worklist2 = new WindowsFormsApplication2.Dashboard.DashboardControls.Worklist();
            this.teamsWorklist1 = new WindowsFormsApplication2.Dashboard.DashboardControls.teamsWorklist();
            this.incomingWorklist1 = new WindowsFormsApplication2.Dashboard.DashboardControls.IncomingWorklist();
            this.dailyProductivity1 = new WindowsFormsApplication2.Dashboard.DashboardControls.DailyProductivity();
            this.panel28.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.panelContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            this.panel28.TabIndex = 9;
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
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(173)))), ((int)(((byte)(78)))));
            this.panel6.Controls.Add(this.lblTeamCnt);
            this.panel6.Controls.Add(this.panel7);
            this.panel6.Controls.Add(this.pictureBox1);
            this.panel6.Controls.Add(this.myTeamWorklist);
            this.panel6.Location = new System.Drawing.Point(411, 52);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(357, 79);
            this.panel6.TabIndex = 10;
            // 
            // lblTeamCnt
            // 
            this.lblTeamCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTeamCnt.AutoSize = true;
            this.lblTeamCnt.Font = new System.Drawing.Font("Century Gothic", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTeamCnt.ForeColor = System.Drawing.Color.White;
            this.lblTeamCnt.Location = new System.Drawing.Point(262, 21);
            this.lblTeamCnt.Name = "lblTeamCnt";
            this.lblTeamCnt.Size = new System.Drawing.Size(74, 40);
            this.lblTeamCnt.TabIndex = 23;
            this.lblTeamCnt.Text = "100";
            // 
            // panel7
            // 
            this.panel7.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel7.Location = new System.Drawing.Point(342, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(15, 79);
            this.panel7.TabIndex = 22;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(86, 79);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 20;
            this.pictureBox1.TabStop = false;
            // 
            // myTeamWorklist
            // 
            this.myTeamWorklist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myTeamWorklist.AutoSize = true;
            this.myTeamWorklist.Cursor = System.Windows.Forms.Cursors.Hand;
            this.myTeamWorklist.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.myTeamWorklist.ForeColor = System.Drawing.Color.White;
            this.myTeamWorklist.Location = new System.Drawing.Point(95, 8);
            this.myTeamWorklist.Name = "myTeamWorklist";
            this.myTeamWorklist.Size = new System.Drawing.Size(135, 56);
            this.myTeamWorklist.TabIndex = 19;
            this.myTeamWorklist.Text = "My Team\'s\r\nWorklists";
            this.myTeamWorklist.Click += new System.EventHandler(this.myTeamWorklist_Click);
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(83)))), ((int)(((byte)(79)))));
            this.panel8.Controls.Add(this.lblIncoming);
            this.panel8.Controls.Add(this.panel9);
            this.panel8.Controls.Add(this.pictureBox2);
            this.panel8.Controls.Add(this.label6);
            this.panel8.Location = new System.Drawing.Point(784, 52);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(357, 79);
            this.panel8.TabIndex = 11;
            // 
            // lblIncoming
            // 
            this.lblIncoming.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIncoming.AutoSize = true;
            this.lblIncoming.Font = new System.Drawing.Font("Century Gothic", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIncoming.ForeColor = System.Drawing.Color.White;
            this.lblIncoming.Location = new System.Drawing.Point(262, 21);
            this.lblIncoming.Name = "lblIncoming";
            this.lblIncoming.Size = new System.Drawing.Size(74, 40);
            this.lblIncoming.TabIndex = 23;
            this.lblIncoming.Text = "100";
            // 
            // panel9
            // 
            this.panel9.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel9.Location = new System.Drawing.Point(342, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(15, 79);
            this.panel9.TabIndex = 22;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(86, 79);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 20;
            this.pictureBox2.TabStop = false;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label6.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(95, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(122, 56);
            this.label6.TabIndex = 19;
            this.label6.Text = "Incoming\r\nWorklists";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.SeaGreen;
            this.panel10.Controls.Add(this.lblDailyCnt);
            this.panel10.Controls.Add(this.panel11);
            this.panel10.Controls.Add(this.pictureBox3);
            this.panel10.Controls.Add(this.label8);
            this.panel10.Location = new System.Drawing.Point(1157, 52);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(357, 79);
            this.panel10.TabIndex = 12;
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
            // panelContainer
            // 
            this.panelContainer.Controls.Add(this.worklist2);
            this.panelContainer.Controls.Add(this.teamsWorklist1);
            this.panelContainer.Controls.Add(this.incomingWorklist1);
            this.panelContainer.Controls.Add(this.dailyProductivity1);
            this.panelContainer.Location = new System.Drawing.Point(38, 137);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(1476, 760);
            this.panelContainer.TabIndex = 17;
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
            this.btnClose.TabIndex = 18;
            this.btnClose.Text = "CLOSE";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click_1);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(141)))), ((int)(((byte)(188)))));
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(3, 956);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1543, 3);
            this.panel4.TabIndex = 22;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(141)))), ((int)(((byte)(188)))));
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(1546, 43);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(3, 916);
            this.panel3.TabIndex = 21;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(141)))), ((int)(((byte)(188)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 43);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(3, 916);
            this.panel2.TabIndex = 20;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(141)))), ((int)(((byte)(188)))));
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1549, 43);
            this.panel1.TabIndex = 19;
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
            // timerRefreshAccntg
            // 
            this.timerRefreshAccntg.Interval = 5000;
            this.timerRefreshAccntg.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // timerAudit
            // 
            this.timerAudit.Interval = 5000;
            this.timerAudit.Tick += new System.EventHandler(this.timerAudit_Tick);
            // 
            // worklist2
            // 
            this.worklist2.BackColor = System.Drawing.Color.White;
            this.worklist2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.worklist2.Location = new System.Drawing.Point(0, 0);
            this.worklist2.Name = "worklist2";
            this.worklist2.Size = new System.Drawing.Size(1476, 760);
            this.worklist2.TabIndex = 3;
            // 
            // teamsWorklist1
            // 
            this.teamsWorklist1.BackColor = System.Drawing.Color.White;
            this.teamsWorklist1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.teamsWorklist1.Location = new System.Drawing.Point(0, 0);
            this.teamsWorklist1.Name = "teamsWorklist1";
            this.teamsWorklist1.Size = new System.Drawing.Size(1476, 760);
            this.teamsWorklist1.TabIndex = 2;
            // 
            // incomingWorklist1
            // 
            this.incomingWorklist1.BackColor = System.Drawing.Color.White;
            this.incomingWorklist1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.incomingWorklist1.Location = new System.Drawing.Point(0, 0);
            this.incomingWorklist1.Name = "incomingWorklist1";
            this.incomingWorklist1.Size = new System.Drawing.Size(1476, 760);
            this.incomingWorklist1.TabIndex = 1;
            // 
            // dailyProductivity1
            // 
            this.dailyProductivity1.BackColor = System.Drawing.Color.White;
            this.dailyProductivity1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dailyProductivity1.Location = new System.Drawing.Point(0, 0);
            this.dailyProductivity1.Name = "dailyProductivity1";
            this.dailyProductivity1.Size = new System.Drawing.Size(1476, 760);
            this.dailyProductivity1.TabIndex = 0;
            // 
            // DashboardTH
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1549, 959);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panelContainer);
            this.Controls.Add(this.panel10);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel28);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "DashboardTH";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dashboard";
            this.Load += new System.EventHandler(this.DashboardTH_Load);
            this.panel28.ResumeLayout(false);
            this.panel28.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.panelContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel28;
        private System.Windows.Forms.Label myWorkList;
        private System.Windows.Forms.Label lblWorkCnt;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label lblTeamCnt;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label myTeamWorklist;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label lblIncoming;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Label lblDailyCnt;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private DashboardControls.Worklist worklist1;
        public System.Windows.Forms.Timer timerRefreshAccntg;
        public System.Windows.Forms.Timer timerAudit;
        private DashboardControls.teamsWorklist teamsWorklist1;
        private DashboardControls.IncomingWorklist incomingWorklist1;
        private DashboardControls.DailyProductivity dailyProductivity1;
        private DashboardControls.Worklist worklist2;
    }
}