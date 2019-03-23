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
    public partial class QueryShareCapital : UserControl
    {
        public QueryShareCapital()
        {
            InitializeComponent();
        }

        Classes.clsQuery clsQuery = new Classes.clsQuery();

        private void QueryShareCapital_Load(object sender, EventArgs e)
        {

        }

        public void minmax()
        {
            clsQuery.loadShareCapitalByUserID(Classes.clsQuery.searchUserID, dataGridView1);
            lblMember.Text = "";
            lblMember.Text = clsQuery.returMembersNameAndID(Classes.clsQuery.searchUserID);
        }
    }
}
