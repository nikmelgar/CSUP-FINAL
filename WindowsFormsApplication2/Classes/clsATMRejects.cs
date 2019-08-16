using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsATMRejects
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();

        
        public void searchInvalidAccount(TextBox txt, string search,DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM ATM WHERE Deposited is null and Account_No = '" + search + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgv.DataSource = dt;

                    dgv.Columns["userID"].Visible = false;
                    dgv.Columns["Principal"].Visible = false;
                    dgv.Columns["Deposited"].Visible = false;
                    dgv.Columns["Frm"].Visible = false;
                    dgv.Columns["wd_loan_slip"].Visible = false;
                    dgv.Columns["jv_no"].Visible = false;

                    dgv.Columns["EmployeeID"].HeaderText = "ID";
                    dgv.Columns["Account_No"].HeaderText = "Account No";
                    dgv.Columns["Bank_Code"].HeaderText = "Bank";

                    txt.Text = "";

                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    dgv.DataSource = null;
                    txt.Text = "";
                    return;
                }
            }    
        }

        //==============================================================================================
        //                              SD WITHDRAWAL STEPS INVALID ATM
        //==============================================================================================
        public void tagSDCancel(string slipNo)
        {
            //TAG AS CANCEL
            //REMOVE JV
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE Withdrawal_Slip SET Cancelled = 1, Cancelled_By = '" + Classes.clsUser.Username + "', Cancelled_Note = 'Invalid Account Number', JV_No = NULL, JV_Date = NULL WHERE Withdrawal_Slip_No = '" + slipNo + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }

            
        }
       
        public void updateJVandRecompute(string jvno,string selectedUserID)
        {
            //check if she/he is the only one in jv
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Journal_Detail WHERE JV_No ='" + jvno + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 2) //Only One in JV
                {
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "UPDATE Journal_Header SET Posted = 0, Posted_By = NULL,Cancelled = 1, Cancelled_By ='" + Classes.clsUser.Username + "', Cancel_Note ='Invalid Account Number', Particulars = 'Invalid Account Number' WHERE JV_No = '" + jvno + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    decimal selectedDebit = 0;
                    decimal selectedCredit = 0;
                    decimal updateCredit = 0;

                    //Get the Amount [Debit] of Selected UserID
                    SqlDataAdapter adapGetAmount = new SqlDataAdapter("SELECT Debit FROM Journal_Detail WHERE JV_No ='" + jvno + "' and userID ='" + selectedUserID + "'", con);
                    DataTable dtGetAmount = new DataTable();
                    adapGetAmount.Fill(dtGetAmount);

                    //Get the Amount [Credit] of Selected UserID
                    SqlDataAdapter adapGetAmountCreidt = new SqlDataAdapter("SELECT Credit FROM Journal_Detail WHERE JV_No ='" + jvno + "' and Account_Code like '102.%'", con);
                    DataTable dtGetAmountCredit = new DataTable();
                    adapGetAmountCreidt.Fill(dtGetAmountCredit);


                    selectedDebit = Convert.ToDecimal(dtGetAmount.Rows[0].ItemArray[0].ToString());
                    selectedCredit = Convert.ToDecimal(dtGetAmountCredit.Rows[0].ItemArray[0].ToString());

                    updateCredit = selectedCredit - selectedDebit;

                    //Recompute
                    SqlCommand cmdUpdateCredit = new SqlCommand();
                    cmdUpdateCredit.Connection = con;
                    cmdUpdateCredit.CommandText = "UPDATE Journal_Detail SET Credit = '" + updateCredit + "' WHERE JV_No = '" + jvno + "' and Account_Code like '102.%'";
                    cmdUpdateCredit.CommandType = CommandType.Text;
                    cmdUpdateCredit.ExecuteNonQuery();

                    //Remove and Recompute Journal Details
                    SqlCommand cmdDeleteDetails = new SqlCommand();
                    cmdDeleteDetails.Connection = con;
                    cmdDeleteDetails.CommandText = "DELETE Journal_Detail WHERE JV_No = '" + jvno + "' and userID ='" + selectedUserID + "'";
                    cmdDeleteDetails.CommandType = CommandType.Text;
                    cmdDeleteDetails.ExecuteNonQuery();
                }
            }

        }

        public void removeATM(string wd_loan_slip)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "DELETE ATM WHERE wd_loan_slip = '" + wd_loan_slip + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            } 
        }

    }
}
