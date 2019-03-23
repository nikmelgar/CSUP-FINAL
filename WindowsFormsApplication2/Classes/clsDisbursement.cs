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

        public void loadBank(ComboBox cmb)
        {
            cmb.DataSource = null;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT (Bank_Code + ' - '+ Bank_Name) as Bank_Name,Bank_Code FROM Bank WHERE isActive = 1", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmb.DisplayMember = "Bank_Name";
                cmb.ValueMember = "Bank_Code";
                cmb.DataSource = dt;
            }

        }

        public string accountCodeFromBanks(string Bank_Code)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Bank_Account_Code FROM Bank WHERE Bank_Code = '" + Bank_Code + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0].ItemArray[0].ToString();
                }
                else
                {
                    return "";
                }
            }

        }

        public Boolean checkIfCancelled(string cv_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

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
        }

        public Boolean checkIfPosted(string cv_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

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
        }

        //Get Whos Posted
        public string getPostedBy(string cv_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Posted_By FROM Disbursement_Header WHERE CV_No = '" + cv_no + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }

        //Get Whos Cancel
        public string getCancelBy(string cv_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Cancelled_By FROM Disbursement_Header WHERE CV_No = '" + cv_no + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }

              
        }

       public void ReleaseDisbursement(string CV_No,Label status)
       {
            if(status.Text == "POSTED and RELEASED")
            {
                Alert.show("Disbursement voucher already released.", Alert.AlertType.error);
                return;
            }

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //Check CV if POSTED FIRST BEFORE CONTINUE TO RELEASE

                SqlDataAdapter adapterCheckCV = new SqlDataAdapter("SELECT Posted,wd_slip_no FROM Disbursement_Header WHERE CV_No = '" + CV_No + "'", con);
                DataTable dtCheckCV = new DataTable();
                adapterCheckCV.Fill(dtCheckCV);

                if (dtCheckCV.Rows[0].ItemArray[0].ToString() == "True")
                {
                    //Proceed to next Step
                    //Put Release Date and Release by
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_ReleasedDisbursement";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CV_No", CV_No);
                    cmd.Parameters.AddWithValue("@Released_By", Classes.clsUser.Username);
                    cmd.ExecuteNonQuery();

                    //Next Check if this Voucher is came from Withdrawal of Savings
                    //If Yes then Tag the Status of The Withdrawal Slip [Release Date]

                    if (dtCheckCV.Rows[0].ItemArray[1].ToString() != "")
                    {
                        //TRUE [FROM SAVINGS]
                        SqlCommand cmdSD = new SqlCommand();
                        cmdSD.Connection = con;
                        cmdSD.CommandText = "UPDATE Withdrawal_Slip SET ReleaseDate = '" + DateTime.Today.ToShortDateString() + "' WHERE Withdrawal_Slip_No = '" + dtCheckCV.Rows[0].ItemArray[1].ToString() + "'";
                        cmdSD.CommandType = CommandType.Text;
                        cmdSD.ExecuteNonQuery();
                    }


                    //DONE Updating Status of Voucher Proceed to Status [Visible]
                    Alert.show("Disbursement voucher successfully released.", Alert.AlertType.success);

                    status.Text = "POSTED and RELEASED";
                }
                else
                {
                    //ERROR 
                    Alert.show("Please Post this voucher first to continue.", Alert.AlertType.error);
                    return;
                }
            }
       }
    }
}
