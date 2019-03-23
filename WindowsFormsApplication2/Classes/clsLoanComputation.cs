using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsLoanComputation
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();


        public void loadLoanApprovedDetails(DataGridView dgv,Label lblCount)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from Loan WHERE Status = '2' ORDER BY Loan_Date,Loan_No ASC";
                cmd.CommandType = CommandType.Text;

                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;

                lblCount.Text = dt.Rows.Count.ToString();

                int colCnt = dt.Columns.Count;
                int x = 0;


                while (x != colCnt)
                {
                    dgv.Columns[x].Visible = false;
                    x = x + 1;
                }

                dgv.Columns["Loan_No"].Visible = true;
                dgv.Columns["Loan_No"].HeaderText = "Loan No";
                dgv.Columns["Loan_No"].FillWeight = 170;

                dgv.Columns["Loan_Type"].Visible = true;
                dgv.Columns["Loan_Type"].HeaderText = "Type";
                dgv.Columns["Loan_Type"].FillWeight = 100;
            }
        }

        //Search Loan No
        public void SearchLoanNo(DataGridView dgv, Label lblCount,string LoanNo,TextBox txtloan)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from Loan WHERE Status = '2' and Loan_No like '%" + LoanNo + "%' ORDER BY Loan_Date,Loan_No ASC";
                cmd.CommandType = CommandType.Text;

                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;

                lblCount.Text = dt.Rows.Count.ToString();

                int colCnt = dt.Columns.Count;
                int x = 0;

                if (dgv.Rows.Count == 0)
                {
                    Alert.show("No record/s found.", Alert.AlertType.error);
                    txtloan.Text = "";
                }

                while (x != colCnt)
                {
                    dgv.Columns[x].Visible = false;
                    x = x + 1;
                }

                dgv.Columns["Loan_No"].Visible = true;
                dgv.Columns["Loan_No"].HeaderText = "Loan No";
                dgv.Columns["Loan_No"].FillWeight = 170;

                dgv.Columns["Loan_Type"].Visible = true;
                dgv.Columns["Loan_Type"].HeaderText = "Type";
                dgv.Columns["Loan_Type"].FillWeight = 100;
            }
            
        }

        public string returnName(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT (LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1) + Suffix) as FullName From Membership WHERE userID = '" + userid + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }

        public string returnCompanyName(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Company_Code FROM Membership WHERE userID = '" + userid + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT Description FROM Company WHERE Company_Code = '" + dt.Rows[0].ItemArray[0].ToString() + "'", con);
                DataTable dt2 = new DataTable();
                adapter2.Fill(dt2);

                return dt2.Rows[0].ItemArray[0].ToString();
            }
        }

        public string returnDateHired(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Date_Hired FROM Membership WHERE userID = '" + userid + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                return Convert.ToString(Convert.ToDateTime(dt.Rows[0].ItemArray[0].ToString()).ToShortDateString());
            }
        }

        public string returnMembershipDate(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                adapter = new SqlDataAdapter("SELECT Date_Of_Membership FROM Membership WHERE userID = '" + userid + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                return Convert.ToString(Convert.ToDateTime(dt.Rows[0].ItemArray[0].ToString()).ToShortDateString());
            }
        }

        public string returnSalary(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Salary FROM Membership WHERE userID = '" + userid + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows[0].ItemArray[0].ToString() != "")
                {
                    return Convert.ToString(Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString()).ToString("#,0.00"));
                }
                else
                {
                    return "0.00";
                }
            }
        }

        public string returnPrincipalID(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT PrincipalID FROM Membership WHERE userID = '" + userid + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows[0].ItemArray[0].ToString() != "")
                {
                    return dt.Rows[0].ItemArray[0].ToString();
                }
                else
                {
                    return "";
                }
            }
        }


        //========================================================================
        //                        LOAN COMPUTATION
        //========================================================================

        public string returnTotalOtherDeduction(string loan_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT SUM(Applied_Amount) as OtherDeduct FROM Loan_Deductions WHERE Loan_No = '" + loan_no + "' and Deduction_Type = 'OTHER DEDUCTION'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows[0].ItemArray[0].ToString() != "")
                {
                    return Convert.ToString(Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString()).ToString("#,0.00"));
                }
                else
                {
                    return "0.00";
                }
            }              
        }


        public string returnTotalDeferred(string loan_no,string loanType)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT SUM(Applied_Amount) FROM Loan_Deductions WHERE loan_no = '" + loan_no + "' and Deduction_Type = 'LOAN' and Loan_Type <> '" + loanType + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows[0].ItemArray[0].ToString() != "")
                {
                    return Convert.ToString(Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString()).ToString("#,0.00"));
                }
                else
                {
                    return "0.00";
                }
            }  
        }


        //CREATING JV
        public string returnChartAccountCode(string loan_type)
        {
            //Get Current Loan
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Account_Dr FROM Loan_Type WHERE Loan_Type = '" + loan_type + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }

        //Return BankCode and BankName
        public string returnMemberBankCodeAccount(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Bank_Code FROM Membership WHERE userID = '" + userid + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT Bank_Account_Code FROM Bank WHERE Bank_Code ='" + dt.Rows[0].ItemArray[0].ToString() + "'", con);
                DataTable dt2 = new DataTable();
                adapter2.Fill(dt2);

                return dt2.Rows[0].ItemArray[0].ToString();
            } 
        }


        public string returnMemberBankCode(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                adapter = new SqlDataAdapter("SELECT Bank_Code FROM Membership WHERE userID = '" + userid + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }

    }

}


