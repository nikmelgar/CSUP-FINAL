using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;


namespace WindowsFormsApplication2.Classes
{
    class clsPDCManagement
    {
        Global global = new Global();
        public static int userid { get; set; }
        public static int id { get; set; }

        public static string LastValueChequeNo { get; set; }
        public static decimal LastValueAmount { get; set; }
        public static string typeFromChecking { get; set; }


        public void loadPDC(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT TOP 50 * FROM vw_PDCManagement ORDER BY ChequeDate,EmpName ASC", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;

                dgv.Columns["DatePrepared"].Visible = false;
                dgv.Columns["userid"].Visible = false;
                dgv.Columns["LoanNumber"].Visible = false;
                dgv.Columns["id"].Visible = false;

                dgv.Columns["isCheck"].HeaderText = "";
                dgv.Columns["ORNumber"].HeaderText = "OR #";
                dgv.Columns["EmployeeID"].HeaderText = "ID No.";
                dgv.Columns["EmpName"].HeaderText = "Name";
                dgv.Columns["ChequeDate"].HeaderText = "Date of Cheque";
                dgv.Columns["LoanType"].HeaderText = "Type";
                dgv.Columns["Bank"].HeaderText = "Bank";
                dgv.Columns["ChequeNo"].HeaderText = "Cheque No";
                dgv.Columns["Amount"].HeaderText = "Amount";

                dgv.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns["Amount"].DefaultCellStyle.Format = "#,0.00";

                dgv.Columns["isCheck"].FillWeight = 20;
                dgv.Columns["ORNumber"].FillWeight = 60;
                dgv.Columns["EmployeeID"].FillWeight = 50;
                dgv.Columns["EmpName"].FillWeight = 260;
                dgv.Columns["LoanType"].FillWeight = 50;
                dgv.Columns["Bank"].FillWeight = 50;

                dgv.Columns["isCheck"].ReadOnly = false;

                dgv.Columns["ORNumber"].ReadOnly = true;
                dgv.Columns["EmployeeID"].ReadOnly = true;
                dgv.Columns["EmpName"].ReadOnly = true;
                dgv.Columns["ChequeDate"].ReadOnly = true;
                dgv.Columns["LoanType"].ReadOnly = true;
                dgv.Columns["Bank"].ReadOnly = true;
                dgv.Columns["ChequeNo"].ReadOnly = true;
            }
        }

        public void loadLoanPerMemberPDC(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanBalancesPerMemberForPDCManagement";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;

                dgv.Columns["Loan_No"].HeaderText = "Loan No";
                dgv.Columns["Loan_Type"].HeaderText = "Type";
                dgv.Columns["Loan_Amount"].HeaderText = "Gross Amount";
                dgv.Columns["Monthly_Amort"].HeaderText = "Monthly";
                dgv.Columns["ReleaseDate"].HeaderText = "Released Date";

                dgv.Columns["Loan_Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns["Monthly_Amort"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns["ReleaseDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns["Loan_Type"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgv.Columns["Loan_Type"].FillWeight = 50;
            }
        }

        //FOR SAME USER ONLY
        public bool CheckChequeNoIfUsed(string chequeNo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM PDCManagement WHERE ChequeNo = '"+ chequeNo +"' and userid ='"+ userid +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    //Already Used                      
                    return true;
                }
                else
                {
                    //Not Use
                    return false;
                }
            }
        }

        //FOR ALL USERS
        public bool CheckChequeNoIfUsedByOthers(string chequeNo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM PDCManagement WHERE ChequeNo = '" + chequeNo + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //Already Used                      
                    return true;
                }
                else
                {
                    //Not Use
                    return false;
                }
            }
        }

        public bool checkCategory(string category,string chequeNo,string loanType)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                string str = "";
                switch (category) {
                    case "Savings":
                        str = "SELECT * FROM PDCManagement WHERE ChequeNo = '" + chequeNo + "' and userid ='" + userid + "' and LoanType = 'SD'";
                        break;
                    case "Share Capital":
                        str = "SELECT * FROM PDCManagement WHERE ChequeNo = '" + chequeNo + "' and userid ='" + userid + "' and LoanType = 'SC'";
                        break;
                    case "Loan":
                        str = "SELECT * FROM PDCManagement WHERE ChequeNo = '" + chequeNo + "' and userid ='" + userid + "' and LoanType = '"+ loanType +"'";
                        break;
                }

                SqlDataAdapter adapter = new SqlDataAdapter(str, con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    //Stop cant be used twice for the same category
                    return true;
                }
                else
                {
                    //He/She can proceed
                    return false;
                }
            }
        }
        
    }
}
