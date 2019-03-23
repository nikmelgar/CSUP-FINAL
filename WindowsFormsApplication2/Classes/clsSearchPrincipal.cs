using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsSearchPrincipal
    {
        Global global = new Global();
        SqlConnection con;
        
        public void loadAllPrincipal(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_SelectAllPrincipal";
                cmd.CommandType = CommandType.StoredProcedure;


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;

                int colCnt = dt.Columns.Count;
                int x = 0;


                while (x != colCnt)
                {
                    dgv.Columns[x].Visible = false;
                    x = x + 1;
                }

                dgv.Columns["EmployeeID"].Visible = true;
                dgv.Columns["EmployeeID"].HeaderText = "Employee ID";

                dgv.Columns["LastName"].Visible = true;
                dgv.Columns["LastName"].HeaderText = "Last Name";

                dgv.Columns["FirstName"].Visible = true;
                dgv.Columns["FirstName"].HeaderText = "First Name";

                dgv.Columns["MiddleName"].Visible = true;
                dgv.Columns["MiddleName"].HeaderText = "Middle Name";

                dgv.Columns["Date_Of_Birth"].Visible = true;
                dgv.Columns["Date_Of_Birth"].HeaderText = "Birthday";
            }
        }

        public void SearchMembers(DataGridView dgv, string EmployeeID, string lastName, string firstName)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //Search Principal Only
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE isActive = '1' and isApprove = '1' and Principal = '1' and EmployeeID like '%" + EmployeeID + "%' and LastName like '%" + lastName + "%' and FirstName like '%" + firstName + "%' and Principal ='1'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgv.DataSource = dt;
                }
                else
                {
                    Alert.show("No Records found on Principal", Alert.AlertType.warning);
                    return;
                }
            }
        }
    }
}
