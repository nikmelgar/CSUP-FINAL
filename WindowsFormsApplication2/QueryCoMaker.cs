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
    public partial class QueryCoMaker : UserControl
    {
        public QueryCoMaker()
        {
            InitializeComponent();
        }

        Classes.clsQuery clsQuery = new Classes.clsQuery();
        Classes.clsQueryCoMaker clsComaker = new Classes.clsQueryCoMaker();

        private void QueryCoMaker_Load(object sender, EventArgs e)
        {

        }
        public void returnName()
        {
            lblMember.Text = "";
            lblMember.Text = clsQuery.returMembersNameAndID(Classes.clsQuery.searchUserID);
        }

        public void loadCoMaker()
        {
            clsComaker.getAllLoanMakers(dataGridView1);
        }
    }
}
