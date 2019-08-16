using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;



namespace WindowsFormsApplication2.loanFrms
{
    class clsLookUpAmort
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();

        public void loaddefaultitems(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 50 * FROM membership where Date_Resigned_From_Pecci is null and isActive = '1' and isApprove = '1'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgv.DataSource = dt;

                    int colCnt = dt.Columns.Count;
                    int x = 0;


                    while (x != colCnt)
                    {
                        dgv.Columns[x].Visible = false;
                        x = x + 1;
                    }

                    dgv.Columns["EmployeeID"].Visible = true;
                    dgv.Columns["EmployeeID"].HeaderText = "ID";
                    dgv.Columns["EmployeeID"].FillWeight = 40;

                    dgv.Columns["LastName"].Visible = true;
                    dgv.Columns["LastName"].HeaderText = "Last Name";

                    dgv.Columns["FirstName"].Visible = true;
                    dgv.Columns["FirstName"].HeaderText = "First Name";

                    dgv.Columns["MiddleName"].Visible = true;
                    dgv.Columns["MiddleName"].HeaderText = "Middle Name";

                    dgv.Columns["Date_Of_Birth"].Visible = true;
                    dgv.Columns["Date_Of_Birth"].HeaderText = "Birthday";
                    dgv.Columns["Date_Of_Birth"].FillWeight = 45;


                }
            }
        }

        public void search(string employeeid, string firstname, string lastname, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 50 * FROM membership where Date_Resigned_From_Pecci is null and EmployeeID like '%" + employeeid + "%' and LastName like '%" + lastname + "%' and FirstName like '%" + firstname + "%' and isActive = '1' and isApprove = '1'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgv.DataSource = dt;

                    int colCnt = dt.Columns.Count;
                    int x = 0;


                    while (x != colCnt)
                    {
                        dgv.Columns[x].Visible = false;
                        x = x + 1;
                    }

                    dgv.Columns["EmployeeID"].Visible = true;
                    dgv.Columns["EmployeeID"].HeaderText = "ID";
                    dgv.Columns["EmployeeID"].FillWeight = 40;

                    dgv.Columns["LastName"].Visible = true;
                    dgv.Columns["LastName"].HeaderText = "Last Name";

                    dgv.Columns["FirstName"].Visible = true;
                    dgv.Columns["FirstName"].HeaderText = "First Name";

                    dgv.Columns["MiddleName"].Visible = true;
                    dgv.Columns["MiddleName"].HeaderText = "Middle Name";

                    dgv.Columns["Date_Of_Birth"].Visible = true;
                    dgv.Columns["Date_Of_Birth"].HeaderText = "Birthday";
                    dgv.Columns["Date_Of_Birth"].FillWeight = 45;


                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }
            }
        }
    }
}
