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
    public partial class QueryLoanBalances : UserControl
    {
        public QueryLoanBalances()
        {
            InitializeComponent();
            lblMember.Text = "";
            lblMember.Text = clsQuery.returMembersNameAndID(Classes.clsQuery.searchUserID);
        }

        Global global = new Global();
        Classes.clsQuery clsQuery = new Classes.clsQuery();
        private void QueryLoanBalances_Load(object sender, EventArgs e)
        {

        }

        public void returnName()
        {
            lblMember.Text = "";
            lblMember.Text = clsQuery.returMembersNameAndID(Classes.clsQuery.searchUserID);
        }

        public void loadLoanBalance()
        {
            clsQuery.loadLoanBalances(Classes.clsQuery.searchUserID, dataGridView1);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                //Move to Membersprofile
                foreach (Form form in Application.OpenForms)
                {
                    if (form.GetType() == typeof(Query))
                    {
                        form.Activate();
                        Query query = new Query();
                        query = (Query)Application.OpenForms["Query"];

                        try
                        {
                            query.loadLoanDetails(dataGridView1.SelectedRows[0].Cells["Loan_No"].Value.ToString());
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }
    }
}
