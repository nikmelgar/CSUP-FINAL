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
            if(dataGridView1.SelectedRows.Count > 0)
            {
                Classes.clsQuery.searchUserID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                //Move to Membersprofile
                foreach (Form form in Application.OpenForms)
                {
                    if (form.GetType() == typeof(Query))
                    {
                        form.Activate();
                        Query query = new Query();
                        query = (Query)Application.OpenForms["Query"];

                        query.LoadDefault();
                    }
                }
            }    
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button2_Click(sender, e);
        }
    }
}
