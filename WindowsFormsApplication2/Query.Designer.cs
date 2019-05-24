namespace WindowsFormsApplication2
{
    partial class Query
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Query));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.PictureBox();
            this.btnMin = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnMembership = new System.Windows.Forms.Button();
            this.PanelMainContainer = new System.Windows.Forms.Panel();
            this.queryLoanDetails1 = new WindowsFormsApplication2.QueryLoanDetails();
            this.queryLoanBalances1 = new WindowsFormsApplication2.QueryLoanBalances();
            this.queryShareCapital1 = new WindowsFormsApplication2.QueryShareCapital();
            this.querySavingsDeposit1 = new WindowsFormsApplication2.QuerySavingsDeposit();
            this.queryMemberProfile1 = new WindowsFormsApplication2.QueryMemberProfile();
            this.querySearchMember1 = new WindowsFormsApplication2.QuerySearchMember();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMin)).BeginInit();
            this.panel5.SuspendLayout();
            this.PanelMainContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SeaGreen;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnMin);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1789, 43);
            this.panel1.TabIndex = 58;
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(1747, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(36, 36);
            this.btnClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnClose.TabIndex = 6;
            this.btnClose.TabStop = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMin
            // 
            this.btnMin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMin.Image = ((System.Drawing.Image)(resources.GetObject("btnMin.Image")));
            this.btnMin.Location = new System.Drawing.Point(1708, 3);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(36, 36);
            this.btnMin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnMin.TabIndex = 5;
            this.btnMin.TabStop = false;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(8, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "Query";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.SeaGreen;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(1792, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(3, 952);
            this.panel3.TabIndex = 60;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.SeaGreen;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(3, 952);
            this.panel2.TabIndex = 59;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.SeaGreen;
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 952);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1795, 3);
            this.panel4.TabIndex = 61;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(45)))), ((int)(((byte)(50)))));
            this.panel5.Controls.Add(this.button7);
            this.panel5.Controls.Add(this.button6);
            this.panel5.Controls.Add(this.button5);
            this.panel5.Controls.Add(this.button4);
            this.panel5.Controls.Add(this.button3);
            this.panel5.Controls.Add(this.button2);
            this.panel5.Controls.Add(this.button1);
            this.panel5.Controls.Add(this.btnMembership);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(3, 43);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(266, 909);
            this.panel5.TabIndex = 62;
            this.panel5.Paint += new System.Windows.Forms.PaintEventHandler(this.panel5_Paint);
            // 
            // button7
            // 
            this.button7.FlatAppearance.BorderSize = 0;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.ForeColor = System.Drawing.Color.White;
            this.button7.Image = ((System.Drawing.Image)(resources.GetObject("button7.Image")));
            this.button7.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button7.Location = new System.Drawing.Point(12, 493);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(254, 61);
            this.button7.TabIndex = 38;
            this.button7.Text = "Members Profile";
            this.button7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.FlatAppearance.BorderSize = 0;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.ForeColor = System.Drawing.Color.White;
            this.button6.Image = ((System.Drawing.Image)(resources.GetObject("button6.Image")));
            this.button6.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button6.Location = new System.Drawing.Point(12, 426);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(254, 61);
            this.button6.TabIndex = 37;
            this.button6.Text = "Dividend Information";
            this.button6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Image = ((System.Drawing.Image)(resources.GetObject("button5.Image")));
            this.button5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button5.Location = new System.Drawing.Point(12, 359);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(254, 61);
            this.button5.TabIndex = 36;
            this.button5.Text = "Payroll Deduction";
            this.button5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
            this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button4.Location = new System.Drawing.Point(12, 292);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(254, 61);
            this.button4.TabIndex = 35;
            this.button4.Text = "Loan Listing";
            this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.Location = new System.Drawing.Point(12, 225);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(254, 61);
            this.button3.TabIndex = 34;
            this.button3.Text = "Loan Balances";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.Location = new System.Drawing.Point(12, 158);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(254, 61);
            this.button2.TabIndex = 33;
            this.button2.Text = "Share Capital";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.Location = new System.Drawing.Point(12, 91);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(254, 61);
            this.button1.TabIndex = 32;
            this.button1.Text = "Savings Deposit";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnMembership
            // 
            this.btnMembership.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(45)))), ((int)(((byte)(50)))));
            this.btnMembership.FlatAppearance.BorderSize = 0;
            this.btnMembership.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMembership.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMembership.ForeColor = System.Drawing.Color.White;
            this.btnMembership.Image = ((System.Drawing.Image)(resources.GetObject("btnMembership.Image")));
            this.btnMembership.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnMembership.Location = new System.Drawing.Point(12, 24);
            this.btnMembership.Name = "btnMembership";
            this.btnMembership.Size = new System.Drawing.Size(254, 61);
            this.btnMembership.TabIndex = 31;
            this.btnMembership.Text = "Search Member";
            this.btnMembership.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMembership.UseVisualStyleBackColor = false;
            this.btnMembership.Click += new System.EventHandler(this.btnMembership_Click);
            // 
            // PanelMainContainer
            // 
            this.PanelMainContainer.Controls.Add(this.queryLoanDetails1);
            this.PanelMainContainer.Controls.Add(this.queryLoanBalances1);
            this.PanelMainContainer.Controls.Add(this.queryShareCapital1);
            this.PanelMainContainer.Controls.Add(this.querySavingsDeposit1);
            this.PanelMainContainer.Controls.Add(this.queryMemberProfile1);
            this.PanelMainContainer.Controls.Add(this.querySearchMember1);
            this.PanelMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelMainContainer.Location = new System.Drawing.Point(269, 43);
            this.PanelMainContainer.Name = "PanelMainContainer";
            this.PanelMainContainer.Size = new System.Drawing.Size(1523, 909);
            this.PanelMainContainer.TabIndex = 63;
            // 
            // queryLoanDetails1
            // 
            this.queryLoanDetails1.BackColor = System.Drawing.Color.White;
            this.queryLoanDetails1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryLoanDetails1.Location = new System.Drawing.Point(0, 0);
            this.queryLoanDetails1.Name = "queryLoanDetails1";
            this.queryLoanDetails1.Size = new System.Drawing.Size(1523, 909);
            this.queryLoanDetails1.TabIndex = 5;
            // 
            // queryLoanBalances1
            // 
            this.queryLoanBalances1.BackColor = System.Drawing.Color.White;
            this.queryLoanBalances1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryLoanBalances1.Location = new System.Drawing.Point(0, 0);
            this.queryLoanBalances1.Name = "queryLoanBalances1";
            this.queryLoanBalances1.Size = new System.Drawing.Size(1523, 909);
            this.queryLoanBalances1.TabIndex = 4;
            // 
            // queryShareCapital1
            // 
            this.queryShareCapital1.BackColor = System.Drawing.Color.White;
            this.queryShareCapital1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryShareCapital1.Location = new System.Drawing.Point(0, 0);
            this.queryShareCapital1.Name = "queryShareCapital1";
            this.queryShareCapital1.Size = new System.Drawing.Size(1523, 909);
            this.queryShareCapital1.TabIndex = 3;
            // 
            // querySavingsDeposit1
            // 
            this.querySavingsDeposit1.BackColor = System.Drawing.Color.White;
            this.querySavingsDeposit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.querySavingsDeposit1.Location = new System.Drawing.Point(0, 0);
            this.querySavingsDeposit1.Name = "querySavingsDeposit1";
            this.querySavingsDeposit1.Size = new System.Drawing.Size(1523, 909);
            this.querySavingsDeposit1.TabIndex = 2;
            // 
            // queryMemberProfile1
            // 
            this.queryMemberProfile1.BackColor = System.Drawing.Color.White;
            this.queryMemberProfile1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryMemberProfile1.Location = new System.Drawing.Point(0, 0);
            this.queryMemberProfile1.Name = "queryMemberProfile1";
            this.queryMemberProfile1.Size = new System.Drawing.Size(1523, 909);
            this.queryMemberProfile1.TabIndex = 1;
            // 
            // querySearchMember1
            // 
            this.querySearchMember1.BackColor = System.Drawing.Color.White;
            this.querySearchMember1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.querySearchMember1.Location = new System.Drawing.Point(0, 0);
            this.querySearchMember1.Name = "querySearchMember1";
            this.querySearchMember1.Size = new System.Drawing.Size(1523, 909);
            this.querySearchMember1.TabIndex = 0;
            // 
            // Query
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1795, 955);
            this.Controls.Add(this.PanelMainContainer);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Query";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Query";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMin)).EndInit();
            this.panel5.ResumeLayout(false);
            this.PanelMainContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel PanelMainContainer;
        private System.Windows.Forms.Button btnMembership;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private QuerySearchMember querySearchMember1;
        private System.Windows.Forms.PictureBox btnClose;
        private System.Windows.Forms.PictureBox btnMin;
        public QueryMemberProfile queryMemberProfile1;
        private QuerySavingsDeposit querySavingsDeposit1;
        private QueryShareCapital queryShareCapital1;
        private QueryLoanDetails queryLoanDetails1;
        private QueryLoanBalances queryLoanBalances1;
    }
}