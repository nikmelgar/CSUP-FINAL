using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class QueryLoanDetails : UserControl
    {
        public QueryLoanDetails()
        {
            InitializeComponent();
            lblMember.Text = "";
            lblMember.Text = clsQuery.returMembersNameAndID(Classes.clsQuery.searchUserID);
        }

        Global global = new Global();
        Classes.clsQuery clsQuery = new Classes.clsQuery();
        Classes.clsQueryLoanDetails clsQyeryLoanDetails = new Classes.clsQueryLoanDetails();
        private void QueryLoanDetails_Load(object sender, EventArgs e)
        {

        }
   
        public void returnName()
        {
            lblMember.Text = "";
            lblMember.Text = clsQuery.returMembersNameAndID(Classes.clsQuery.searchUserID);
        }

        public void loadTrans()
        {
            clsQyeryLoanDetails.loadTransactionCodes(dgvTrans);
        }

        public void loadLoanDetails(string loan_no)
        {
            clsQyeryLoanDetails.displayLoanDetails(loan_no,lblLoanNo,lblLoanType,lblLoanDate,lblReleasedDate,lblLoanStatus,lblLoanGrossAmount,lblServiceFee,lblPrevBalance,lblNetProceed,lblTerms,lblMonthlyAMort,lblFirstDeductionPayment,lblSecondDeductionPayment,lblThirdDeductionPayment,lblSuccedingPayment,lblFirstDeduction,lblSecondDeduction,lblThirdDeduction,dgvCoMakers,dgvOtherDeduct,dgvSubsTrans);
            clsQyeryLoanDetails.loadSubsTransactiob(loan_no, dgvSubsTrans);
            clsQyeryLoanDetails.loadSubsTransactioPastDue(loan_no, dgvPastDue);
        }
    }
}
