using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsLoan
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();

        public void loadComboBox(ComboBox cmb)
        {
            cmb.DataSource = null;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("select (loan_type +' - '+ loan_Description) as LoanDisplay,Loan_Type from Loan_Type", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmb.DisplayMember = "LoanDisplay";
                cmb.ValueMember = "Loan_Type";
                cmb.DataSource = dt;
            }

        }

        public void loadLoanDefault(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();


                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from vw_LoanDashboard";
                cmd.CommandType = CommandType.Text;

                adapter = new SqlDataAdapter(cmd);
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

                dgv.Columns["Loan_No"].Visible = true;
                dgv.Columns["Loan_No"].HeaderText = "Loan No";
                dgv.Columns["Loan_No"].FillWeight = 50;

                dgv.Columns["Loan_Type"].Visible = true;
                dgv.Columns["Loan_Type"].HeaderText = "Type";
                dgv.Columns["Loan_Type"].FillWeight = 150;

                dgv.Columns["Loan_Date"].Visible = true;
                dgv.Columns["Loan_Date"].HeaderText = "Date";
                dgv.Columns["Loan_Date"].FillWeight = 50;

                dgv.Columns["Name"].Visible = true;
                dgv.Columns["Name"].HeaderText = "Name";
                dgv.Columns["Name"].FillWeight = 140;

                dgv.Columns["VoucherNo"].Visible = true;
                dgv.Columns["VoucherNo"].HeaderText = "Voucher";
                dgv.Columns["VoucherNo"].FillWeight = 70;

                dgv.Columns["status_description"].Visible = true;
                dgv.Columns["status_description"].HeaderText = "Status";
                dgv.Columns["status_description"].FillWeight = 70;

                dgv.Columns["Encoded_By"].Visible = true;
                dgv.Columns["Encoded_By"].HeaderText = "Encoded By";
                dgv.Columns["Encoded_By"].FillWeight = 60;
            }
        }

        //========================================================
        //              START FOR DASHBOARD
        //========================================================

        public string loadLoansApproved()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM vw_LoanDashboard WHERE Status = '2'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows.Count.ToString();
                }
                else
                {
                    return "0";
                }
            }
        }

        public string loadLoansDisapproved()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM vw_LoanDashboard WHERE Status = '3'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows.Count.ToString();
                }
                else
                {
                    return "0";
                }
            }             
        }

        public string loadLoansForApproval()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM vw_LoanDisplay WHERE Status in ('1','6')", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows.Count.ToString();
                }
                else
                {
                    return "0";
                }
            }    
        }

        public string loadLoansReleased()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM vw_LoanDisplay WHERE Status ='5'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows.Count.ToString();
                }
                else
                {
                    return "0";
                }
            }
        }

        //DETAILS IN DASHBOARD
        public void loadForApprovalDetails(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from vw_LoanDisplay WHERE Status in ('1','6') ORDER BY Loan_Date,Loan_No ASC";
                cmd.CommandType = CommandType.Text;

                adapter = new SqlDataAdapter(cmd);
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

                dgv.Columns["Loan_No"].Visible = true;
                dgv.Columns["Loan_No"].HeaderText = "Loan No";
                dgv.Columns["Loan_No"].FillWeight = 80;

                dgv.Columns["Loan_Type"].Visible = true;
                dgv.Columns["Loan_Type"].HeaderText = "Type";
                dgv.Columns["Loan_Type"].FillWeight = 150;

                dgv.Columns["Loan_Date"].Visible = true;
                dgv.Columns["Loan_Date"].HeaderText = "Date";
                dgv.Columns["Loan_Date"].FillWeight = 50;

                dgv.Columns["Name"].Visible = true;
                dgv.Columns["Name"].HeaderText = "Name";
                dgv.Columns["Name"].FillWeight = 150;

                dgv.Columns["status_description"].Visible = true;
                dgv.Columns["status_description"].HeaderText = "Status";
                dgv.Columns["status_description"].FillWeight = 80;

                dgv.Columns["Encoded_By"].Visible = true;
                dgv.Columns["Encoded_By"].HeaderText = "Encoded By";
            }
        }

        //LOAD LOAN APPROVED
        public void loadLoanApprovedDetails(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from vw_LoanDashboard WHERE Status = '2' ORDER BY Loan_Date,Loan_No ASC";
                cmd.CommandType = CommandType.Text;

                adapter = new SqlDataAdapter(cmd);
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

                dgv.Columns["Loan_No"].Visible = true;
                dgv.Columns["Loan_No"].HeaderText = "Loan No";
                dgv.Columns["Loan_No"].FillWeight = 80;

                dgv.Columns["Loan_Type"].Visible = true;
                dgv.Columns["Loan_Type"].HeaderText = "Type";
                dgv.Columns["Loan_Type"].FillWeight = 150;

                dgv.Columns["Loan_Date"].Visible = true;
                dgv.Columns["Loan_Date"].HeaderText = "Date";
                dgv.Columns["Loan_Date"].FillWeight = 50;

                dgv.Columns["Name"].Visible = true;
                dgv.Columns["Name"].HeaderText = "Name";
                dgv.Columns["Name"].FillWeight = 150;

                dgv.Columns["status_description"].Visible = true;
                dgv.Columns["status_description"].HeaderText = "Status";
                dgv.Columns["status_description"].FillWeight = 80;

                dgv.Columns["Encoded_By"].Visible = true;
                dgv.Columns["Encoded_By"].HeaderText = "Encoded By";
            }
        }

        //LOAD LOAN DISAPPROVED
        public void loadLoanDisapprovedDetails(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from vw_LoanDashboard WHERE Status = '3' ORDER BY Loan_Date,Loan_No ASC";
                cmd.CommandType = CommandType.Text;

                adapter = new SqlDataAdapter(cmd);
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

                dgv.Columns["Loan_No"].Visible = true;
                dgv.Columns["Loan_No"].HeaderText = "Loan No";
                dgv.Columns["Loan_No"].FillWeight = 80;

                dgv.Columns["Loan_Type"].Visible = true;
                dgv.Columns["Loan_Type"].HeaderText = "Type";
                dgv.Columns["Loan_Type"].FillWeight = 150;

                dgv.Columns["Loan_Date"].Visible = true;
                dgv.Columns["Loan_Date"].HeaderText = "Date";
                dgv.Columns["Loan_Date"].FillWeight = 50;

                dgv.Columns["Name"].Visible = true;
                dgv.Columns["Name"].HeaderText = "Name";
                dgv.Columns["Name"].FillWeight = 150;

                dgv.Columns["status_description"].Visible = true;
                dgv.Columns["status_description"].HeaderText = "Status";
                dgv.Columns["status_description"].FillWeight = 80;

                dgv.Columns["Encoded_By"].Visible = true;
                dgv.Columns["Encoded_By"].HeaderText = "Encoded By";
            }
        }

        //LOAD LOAN RELEASED
        public void loadLoanReleasedDetails(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from vw_LoanDashboard WHERE Status = '5' ORDER BY Loan_Date,Loan_No ASC";
                cmd.CommandType = CommandType.Text;

                adapter = new SqlDataAdapter(cmd);
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

                dgv.Columns["Loan_No"].Visible = true;
                dgv.Columns["Loan_No"].HeaderText = "Loan No";
                dgv.Columns["Loan_No"].FillWeight = 80;

                dgv.Columns["Loan_Type"].Visible = true;
                dgv.Columns["Loan_Type"].HeaderText = "Type";
                dgv.Columns["Loan_Type"].FillWeight = 150;

                dgv.Columns["Loan_Date"].Visible = true;
                dgv.Columns["Loan_Date"].HeaderText = "Date";
                dgv.Columns["Loan_Date"].FillWeight = 50;

                dgv.Columns["Name"].Visible = true;
                dgv.Columns["Name"].HeaderText = "Name";
                dgv.Columns["Name"].FillWeight = 150;

                dgv.Columns["status_description"].Visible = true;
                dgv.Columns["status_description"].HeaderText = "Status";
                dgv.Columns["status_description"].FillWeight = 80;

                dgv.Columns["Encoded_By"].Visible = true;
                dgv.Columns["Encoded_By"].HeaderText = "Encoded By";
            }
        }

        //SEARCHING OF LOANS
        public void SearchLoan(DataGridView dgv, TextBox loan_no, TextBox name, TextBox employeeid)
        {
            if (loan_no.Text == "" && name.Text == "" && employeeid.Text == "")
            {
                Alert.show("Please put keyword to be search!", Alert.AlertType.error);
                return;
            }

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from vw_LoanSearch WHERE loan_no like '%" + loan_no.Text + "%'" + " and Name like '%" + name.Text + "%'" + " and EmployeeID like '%" + employeeid.Text + "%'" + " ORDER BY Loan_Date,Loan_No ASC";
                cmd.CommandType = CommandType.Text;

                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    Alert.show("No record/s found.", Alert.AlertType.error);
                    return;
                }

                dgv.DataSource = dt;

                int colCnt = dt.Columns.Count;
                int x = 0;


                while (x != colCnt)
                {
                    dgv.Columns[x].Visible = false;
                    x = x + 1;
                }

                dgv.Columns["Loan_No"].Visible = true;
                dgv.Columns["Loan_No"].HeaderText = "Loan No";
                dgv.Columns["Loan_No"].FillWeight = 50;

                dgv.Columns["Loan_Type"].Visible = true;
                dgv.Columns["Loan_Type"].HeaderText = "Type";
                dgv.Columns["Loan_Type"].FillWeight = 150;

                dgv.Columns["Loan_Date"].Visible = true;
                dgv.Columns["Loan_Date"].HeaderText = "Date";
                dgv.Columns["Loan_Date"].FillWeight = 50;

                dgv.Columns["Name"].Visible = true;
                dgv.Columns["Name"].HeaderText = "Name";
                dgv.Columns["Name"].FillWeight = 140;

                dgv.Columns["VoucherNo"].Visible = true;
                dgv.Columns["VoucherNo"].HeaderText = "Voucher";
                dgv.Columns["VoucherNo"].FillWeight = 70;

                dgv.Columns["status_description"].Visible = true;
                dgv.Columns["status_description"].HeaderText = "Status";
                dgv.Columns["status_description"].FillWeight = 70;

                dgv.Columns["Encoded_By"].Visible = true;
                dgv.Columns["Encoded_By"].HeaderText = "Encoded By";
                dgv.Columns["Encoded_By"].FillWeight = 60;
            }
        }

        public Boolean invalidBankAccount(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Bank_Code,Atm_Account_No FROM Membership WHERE userID ='"+ userid +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                //dt.Rows[0].ItemArray[0].ToString() - Bank Code
                //dt.Rows[0].ItemArray[1].ToString() - Atm Account No

                if(dt.Rows[0].ItemArray[0].ToString() == "" || dt.Rows[0].ItemArray[1].ToString() == "")
                {
                    Alert.show("Please update members bank account.", Alert.AlertType.error);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
