using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsLoanBalancesFromDataEntry
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();

        public void loadPrevLoanBalance(int userid,string loan_type,TextBox txtOldLoanNo,TextBox txtOldLoanType,TextBox txtGrossAmount,TextBox txtReleasedDate,TextBox txtExistingBalance,TextBox txtDeferredBalances,TextBox txtTermsinMos, TextBox txtMonthlyAmort, TextBox txtLoanNo)
        {
            if(userid.ToString() != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_ReturnLoanBalancesForPrevLoan";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@Loan_Type", loan_type);
                    cmd.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);

                    adapter = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        //Prev Loan for Renewal

                        txtOldLoanNo.Text = dt.Rows[0].ItemArray[0].ToString();
                        txtOldLoanType.Text = dt.Rows[0].ItemArray[1].ToString();
                        txtGrossAmount.Text = Convert.ToDecimal(dt.Rows[0].ItemArray[3].ToString()).ToString("#,0.00");
                        txtReleasedDate.Text = Convert.ToDateTime(dt.Rows[0].ItemArray[33].ToString().ToString()).ToString("MM/dd/yyyy");
                        txtExistingBalance.Text = Convert.ToDecimal(dt.Rows[0].ItemArray[38].ToString()).ToString("#,0.00");
                        txtDeferredBalances.Text = Convert.ToDecimal(dt.Rows[0].ItemArray[39].ToString()).ToString("#,0.00");
                        txtTermsinMos.Text = dt.Rows[0].ItemArray[5].ToString();
                        txtMonthlyAmort.Text = Convert.ToDecimal(dt.Rows[0].ItemArray[7].ToString()).ToString("#,0.00");

                        if(checkIfEqualMaxAndBalance(loan_type, Convert.ToDecimal(txtExistingBalance.Text)) == true)
                        {
                            return;
                        }

                    }

                    else
                    {
                        txtOldLoanNo.Text = "";
                        txtOldLoanType.Text = "";
                        txtGrossAmount.Text = "";
                        txtReleasedDate.Text = "";
                        txtExistingBalance.Text = "";
                        txtDeferredBalances.Text = "";
                        txtTermsinMos.Text = "";
                        txtMonthlyAmort.Text = "";
                    }
                }
            }
        }
        
        public Boolean checkIfEqualMaxAndBalance(string loan_Type,decimal Balance)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //first get the max amount of that loan and compare to the balance of that loan if equal to the max amount then decline
                SqlDataAdapter adapterGetMax = new SqlDataAdapter("SELECT Max_Loan_Amount FROM Loan_Type WHERE Loan_Type = '" + loan_Type + "'", con);
                DataTable dtGetMax = new DataTable();
                adapterGetMax.Fill(dtGetMax);

                if(Balance >= Convert.ToDecimal(dtGetMax.Rows[0].ItemArray[0].ToString()))
                {
                    Alert.show("Maximum loanable amount reached.", Alert.AlertType.error);
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
