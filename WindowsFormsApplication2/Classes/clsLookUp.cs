using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsLookUp
    {
        SqlConnection con;
        Global global = new Global();
        clsSavingsDataEntry clsSavingsDataEntry = new clsSavingsDataEntry();

        //0 = Savings
        public static string whosLookUp { get; set; }

        public void loadLookUpQuery(string tableName,DataGridView dgv)
        {
            //Open Connection
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //check whos form trigger the lookup
                if (whosLookUp.ToString() == "0" || whosLookUp.ToString() == "1" || whosLookUp.ToString() == "2")
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 50 * FROM " + tableName, con);
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
                }
            }

        }

        public void SearchSavings(string employeeid,string firstname,string lastname, DataGridView dgv)
        {
            //Open Connection
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //check whos form trigger the lookup
                if (whosLookUp.ToString() == "0")
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 50 * FROM Membership WHERE EmployeeID like '%" + employeeid + "%' and LastName like '%" + lastname + "%' and FirstName like '%" + firstname + "%' and isActive = '1' and isApprove = '1'", con);
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
                        Alert.show("No record/s found.", Alert.AlertType.warning);
                        return;
                    }
                }
            }

                
        }

        public void SearchMemberDisbursement(string employeeid, string firstname, string lastname, DataGridView dgv)
        {
            //Open Connection
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //check whos form trigger the lookup
                if (whosLookUp.ToString() == "1")
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 50 * FROM Membership WHERE EmployeeID like '%" + employeeid + "%' and LastName like '%" + lastname + "%' and FirstName like '%" + firstname + "%' and isActive = '1' and isApprove = '1'", con);
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
                        Alert.show("No record/s found.", Alert.AlertType.warning);
                        return;
                    }
                }
            }
        }

        public void SearchMemberCashReceipt(string employeeid, string firstname, string lastname, DataGridView dgv)
        {
            //Open Connection
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //check whos form trigger the lookup
                if (whosLookUp.ToString() == "2")
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 50 * FROM Membership WHERE EmployeeID like '%" + employeeid + "%' and LastName like '%" + lastname + "%' and FirstName like '%" + firstname + "%' and isActive = '1' and isApprove = '1'", con);
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
                        Alert.show("No record/s found.", Alert.AlertType.warning);
                        return;
                    }
                }
            }
        }

    }
}
