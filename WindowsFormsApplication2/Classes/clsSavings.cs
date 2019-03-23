using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Extensions.DateTime;

namespace WindowsFormsApplication2.Classes
{
    class clsSavings
    {

        SqlConnection con;
        SqlDataAdapter adapter;
        Global global = new Global();

        #region DASHBOARD NUMBERS
        //Get Total ATM

        public void loadDash(Label lblATM, Label lblCheque, Label lblCash)
        {
            lblATM.Text = totalNumberATM();
            lblCheque.Text = totalNumberCheque();
            lblCash.Text = totalNumberCash();
        }

        public void loadDashReleaseCancel(Label cashRelease, Label chequeRelease, Label cancelTotal)
        {
            cashRelease.Text = totalNumberOfReleaseCash();
            chequeRelease.Text = totalNumberOfReleaseCheque();
            cancelTotal.Text = totalNumberOfCancelledTransaction();
        }

        public string totalNumberOfReleaseCash()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetSavingsReleaseDaily";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Mode", "CA");

                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
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
        public string totalNumberOfCancelledTransaction()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_GetSavingsCancelledTotal";

                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
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

        public string totalNumberOfReleaseCheque()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetSavingsReleaseDaily";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Mode", "CH");

                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
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
        public string totalNumberATM()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetSavingsTotalATM";
                cmd.CommandType = CommandType.StoredProcedure;


                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
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

        public string totalNumberCheque()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetSavingsTotalCheque";
                cmd.CommandType = CommandType.StoredProcedure;

                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
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

        public string totalNumberCash()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetSavingsTotalCash";
                cmd.CommandType = CommandType.StoredProcedure;

                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
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

        #endregion

        public void returnBankAtm(TextBox bankcode,TextBox atmNo,int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Bank_Code,Atm_Account_No FROM Membership WHERE userID ='" + userid + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                bankcode.Text = dt.Rows[0].ItemArray[0].ToString();
                atmNo.Text = dt.Rows[0].ItemArray[1].ToString();
            }
        }
        public void loadSavings(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetSavingsWithdrawalDaily";
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

                dgv.Columns["wdDate"].Visible = true;
                dgv.Columns["wdDate"].HeaderText = "Date";
                dgv.Columns["wdDate"].FillWeight = 50;

                dgv.Columns["Name"].Visible = true;
                dgv.Columns["Name"].HeaderText = "Name";
                dgv.Columns["Name"].FillWeight = 140;

                dgv.Columns["Withdrawal_Slip_No"].Visible = true;
                dgv.Columns["Withdrawal_Slip_No"].HeaderText = "Withdrawal Slip No";

                dgv.Columns["Withdrawal_Mode"].Visible = true;
                dgv.Columns["Withdrawal_Mode"].HeaderText = "Mode";
                dgv.Columns["Withdrawal_Mode"].FillWeight = 50;

                dgv.Columns["Posted"].Visible = true;
                dgv.Columns["Posted"].HeaderText = "Posted";
                dgv.Columns["Posted"].FillWeight = 60;

                dgv.Columns["Cancelled"].Visible = true;
                dgv.Columns["Cancelled"].HeaderText = "Cancelled";
                dgv.Columns["Cancelled"].FillWeight = 60;

                dgv.Columns["ReleaseDate"].Visible = true;
                dgv.Columns["ReleaseDate"].HeaderText = "Release Date";
                dgv.Columns["ReleaseDate"].FillWeight = 80;

                dgv.Columns["Prepared_By"].Visible = true;
                dgv.Columns["Prepared_By"].HeaderText = "Prepared By";
                dgv.Columns["Prepared_By"].FillWeight = 80;
            }
        }

        public string returnCompanyDescription(int userID)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT Company_Code FROM Membership WHERE userID ='" + userID + "'", con);
                DataTable dt1 = new DataTable();
                adapter1.Fill(dt1);


                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Description FROM Company WHERE Company_Code='" + dt1.Rows[0].ItemArray[0].ToString() + "'", con);
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

        public string returnSDMonthly(int userID)
        {
            using(SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Savings_Deposit FROM Membership WHERE userID ='" + userID + "'", con);
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

        public string returnBankDescription(string bankCode)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Bank_Name FROM Bank WHERE Bank_Code ='" + bankCode + "'", con);
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

        public string returnLastWithdrawalDate(int userID)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetLastDateWithdrawal";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userID", userID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return Convert.ToDateTime(dt.Rows[0].ItemArray[0].ToString()).ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

        public string returnDepositedChequeAmount(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                var now = DateTime.Now;
                var dt2 = now.AddBusinessDays(-3);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetTotalDepositedAmountSavingsCheck";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userID", userid);
                cmd.Parameters.AddWithValue("@dt", dt2.ToShortDateString());

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
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
        public string returnDepositedChequeDate(int userid)
        {
            //Date
            var now = DateTime.Now;
            var dt2 = now.AddBusinessDays(-3);

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetLastDepositedCheckSavings";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userID", userid);
                cmd.Parameters.AddWithValue("@dt", dt2.ToShortDateString());

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return Convert.ToDateTime(dt.Rows[0].ItemArray[0].ToString()).ToShortDateString();
                }
                else
                {
                    return "";
                }
            }
        }

    }
}

namespace Extensions.DateTime
{
    public static class BusinessDays
    {
        public static System.DateTime AddBusinessDays(this System.DateTime source, int businessDays)
        {
            var dayOfWeek = businessDays < 0
                                ? ((int)source.DayOfWeek - 12) % 7
                                : ((int)source.DayOfWeek + 6) % 7;

            switch (dayOfWeek)
            {
                case 6:
                    businessDays--;
                    break;
                case -6:
                    businessDays++;
                    break;
            }

            return source.AddDays(businessDays + ((businessDays + dayOfWeek) / 5) * 2);
        }
    }
}
