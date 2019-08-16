using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Query : Form
    {
        public Query()
        {
            InitializeComponent();
            querySearchMember1.BringToFront();
        }

        Global global = new Global();
        Classes.clsQuery clsQuery = new Classes.clsQuery();
        Classes.clsQueryMember clsQueryMember = new Classes.clsQueryMember();

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMembership_Click(object sender, EventArgs e)
        {
            querySearchMember1.BringToFront();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            querySavingsDeposit1.minmax();
            querySavingsDeposit1.BringToFront();
            
        }
        private void button7_Click(object sender, EventArgs e)
        {
            clsQueryMember.loadMembersProfile(Classes.clsQuery.searchUserID);
            queryMemberProfile1.loadMembersInfo();
            queryMemberProfile1.BringToFront();
        }

        public void LoadDefault()
        {
            clsQueryMember.loadMembersProfile(Classes.clsQuery.searchUserID);
            queryMemberProfile1.loadMembersInfo();
            queryMemberProfile1.BringToFront();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            queryShareCapital1.minmax();
            queryShareCapital1.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            queryLoanBalances1.returnName();
            queryLoanBalances1.loadLoanBalance();
            queryLoanBalances1.BringToFront();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            queryLoanListing1.returnName();
            queryLoanListing1.loadLoanListing();
            queryLoanListing1.BringToFront();
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        //For loan balances to loan details
        public void loadLoanDetails(string loan_no)
        {
            //Return Name first
            queryLoanDetails1.returnName();
            queryLoanDetails1.loadTrans();
            queryLoanDetails1.loadLoanDetails(loan_no);
            queryLoanDetails1.BringToFront();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            queryPayrollDeduction1.dataGridView1.Rows.Clear();
            queryPayrollDeduction1.returnName();
            queryPayrollDeduction1.BringToFront();
        }

        private void btnCoMaker_Click(object sender, EventArgs e)
        {
            queryCoMaker1.returnName();
            queryCoMaker1.loadCoMaker();
            queryCoMaker1.BringToFront();
        }
    }
}
