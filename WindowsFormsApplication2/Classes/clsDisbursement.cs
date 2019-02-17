using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsDisbursement
    {
        SqlConnection con;
        SqlDataAdapter adapter;

        Global global = new Global();
        public static int userID { get; set; }
        public static bool releaseCashWithdrawal { get; set; }

        public static string slipFromWithdrawal { get; set; }
        public void loadComboBox(ComboBox cmb)
        {
            cmb.DataSource = null;

            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT REPLACE(transaction_code, 'TRAN', '') +' - '+ [Description] as [Transaction] ,Transaction_Code From Transaction_Type where isActive = 1", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            cmb.DisplayMember = "Transaction";
            cmb.ValueMember = "Transaction_Code";
            cmb.DataSource = dt;

        }

        public void loadBank(ComboBox cmb)
        {
            cmb.DataSource = null;

            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT (Bank_Code + ' - '+ Bank_Name) as Bank_Name,Bank_Code FROM Bank WHERE isActive = 1", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            cmb.DisplayMember = "Bank_Name";
            cmb.ValueMember = "Bank_Code"; 
            cmb.DataSource = dt;
        }

        public string accountCodeFromBanks(string Bank_Code)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Bank_Account_Code FROM Bank WHERE Bank_Code = '"+ Bank_Code +"'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray[0].ToString();
            }
            else
            {
                return "";
            }
        }

        public Boolean checkIfCancelled(string cv_no)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Disbursement_Header WHERE CV_No = '" + cv_no + "' and Cancelled = 1", con);
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

        public Boolean checkIfPosted(string cv_no)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Disbursement_Header WHERE cV_No = '" + cv_no + "' and Posted = 1", con);
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

        //Get Whos Posted
        public string getPostedBy(string cv_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Posted_By FROM Disbursement_Header WHERE CV_No = '"+ cv_no +"'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt.Rows[0].ItemArray[0].ToString();
        }

        //Get Whos Cancel
        public string getCancelBy(string cv_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Cancelled_By FROM Disbursement_Header WHERE CV_No = '" + cv_no + "'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt.Rows[0].ItemArray[0].ToString();
        }

       
    }
}
