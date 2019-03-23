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
    public partial class QuerySearchMember : UserControl
    {
        public QuerySearchMember()
        {
            InitializeComponent();
            txtEmployeeID.Text = "";
            txtLastName.Text = "";
            txtFirstName.Text = "";
            dataGridView1.DataSource = null;
        }

        Global global = new Global();
        Classes.clsQuery clsQuery = new Classes.clsQuery();
        private void btnSearch_Click(object sender, EventArgs e)
        {
            clsQuery.loadSearchMember(dataGridView1, txtLastName, txtFirstName, txtEmployeeID);
        }

        private void QuerySearchMember_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Classes.clsQuery.searchUserID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
        }
    }
}
