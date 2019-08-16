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
    public partial class QueryPayrollDeduction : UserControl
    {
        public QueryPayrollDeduction()
        {
            InitializeComponent();
            dtPayroll.CustomFormat = "MM/dd/yyyy";
        }

        Classes.clsQueryPayrollDeduction clsPayroll = new Classes.clsQueryPayrollDeduction();
        Classes.clsQuery clsQuery = new Classes.clsQuery();
        private void QueryPayrollDeduction_Load(object sender, EventArgs e)
        {
            dtPayroll.CustomFormat = "MM/dd/yyyy";
        }
        public void returnName()
        {
            lblMember.Text = "";
            lblMember.Text = clsQuery.returMembersNameAndID(Classes.clsQuery.searchUserID);
        }
       
        private void btnSearch_Click(object sender, EventArgs e)
        {
            clsPayroll.displayPayrollDeduction(dataGridView1, Classes.clsQuery.searchUserID, dtPayroll.Text);
        }
    }
}
