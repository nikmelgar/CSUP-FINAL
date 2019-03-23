using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsJournalVoucher
    {
        Global global = new Global();

        SqlConnection con;
        SqlDataAdapter adapter;

        public static bool fromReplenishment { get; set; }
        public static int userId { get; set; }
        public void loadComboBox(ComboBox cmb)
        {
            cmb.DataSource = null;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT REPLACE(transaction_code, 'TRAN', '') +' - '+ [Description] as [Transaction] ,Transaction_Code From Transaction_Type where isActive = 1", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmb.DisplayMember = "Transaction";
                cmb.ValueMember = "Transaction_Code";
                cmb.DataSource = dt;
            }

            

        }

        public Boolean checkIfCancelled(string jv_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Journal_Header WHERE JV_No = '" + jv_no + "' and Cancelled = 1", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public Boolean checkIfPosted(string jv_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Journal_Header WHERE JV_No = '" + jv_no + "' and Posted = 1", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string returnLoanTypeDescription(string loan_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                adapter = new SqlDataAdapter("SELECT Loan_Type FROM Loan WHERE Loan_No ='" + loan_no + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT Loan_Description FROM Loan_Type WHERE Loan_Type = '" + dt.Rows[0].ItemArray[0].ToString() + "'", con);
                DataTable dt2 = new DataTable();
                adapter2.Fill(dt2);

                return dt2.Rows[0].ItemArray[0].ToString();
            }
        }

    }
}
