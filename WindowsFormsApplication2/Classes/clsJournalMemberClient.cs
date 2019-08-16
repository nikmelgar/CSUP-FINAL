using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsJournalMemberClient
    {
        SqlConnection con;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();

        public void loadLookUpQuery(string tableName, DataGridView dgv)
        {
            //Open Connection
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM " + tableName, con);
                dt = new DataTable();
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
        }

        public void loadClient(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();


                adapter = new SqlDataAdapter("SELECT * FROM Client WHERE isActive = 1", con);
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

                dgv.Columns["Client_Code"].Visible = true;
                dgv.Columns["Client_Code"].HeaderText = "Code";
                dgv.Columns["Client_Code"].FillWeight = 30;

                dgv.Columns["Name"].Visible = true;
                dgv.Columns["Name"].HeaderText = "Client Name";
                dgv.Columns["Name"].FillWeight = 100;
            }

        }

        public void SearchMember(string employeeid, string firstname, string lastname, DataGridView dgv)
        {
            //Open Connection
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE EmployeeID like '%" + employeeid + "%' and LastName like '%" + lastname + "%' and FirstName like '%" + firstname + "%' and isActive = '1' and isApprove = '1'", con);
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
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.warning);
                    return;
                }
            }
        }

        public void SearchClient(DataGridView dgv,string ClientId,string Name)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();


                adapter = new SqlDataAdapter("SELECT * FROM Client WHERE  Client_Code like '%" + ClientId + "%' and [Name] like '%" + Name + "%' and  isActive = 1", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;


                if (dt.Rows.Count > 0)
                {
                    int colCnt = dt.Columns.Count;
                    int x = 0;


                    while (x != colCnt)
                    {
                        dgv.Columns[x].Visible = false;
                        x = x + 1;
                    }

                    dgv.Columns["Client_Code"].Visible = true;
                    dgv.Columns["Client_Code"].HeaderText = "Code";
                    dgv.Columns["Client_Code"].FillWeight = 30;

                    dgv.Columns["Name"].Visible = true;
                    dgv.Columns["Name"].HeaderText = "Client Name";
                    dgv.Columns["Name"].FillWeight = 100;
                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.warning);
                    return;
                }
            }   
        }
    }
}
