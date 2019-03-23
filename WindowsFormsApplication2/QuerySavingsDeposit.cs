using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace WindowsFormsApplication2
{
    public partial class QuerySavingsDeposit : UserControl
    {
        public QuerySavingsDeposit()
        {
            InitializeComponent();
            clsQuery.loadSavingsByUserID(Classes.clsQuery.searchUserID, dataGridView1);
            lblMember.Text = "";
            lblMember.Text = clsQuery.returMembersNameAndID(Classes.clsQuery.searchUserID);
        }

        Global global = new Global();
        Classes.clsQuery clsQuery = new Classes.clsQuery();

                
        public void QuerySavingsDeposit_Load(object sender, EventArgs e)
        {

        }

        public void minmax()
        {
            clsQuery.loadSavingsByUserID(Classes.clsQuery.searchUserID, dataGridView1);
            lblMember.Text = "";
            lblMember.Text = clsQuery.returMembersNameAndID(Classes.clsQuery.searchUserID);
        }
    }
}
