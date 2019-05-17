using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2.Classes
{
    class clsLoanDataEntry
    {
        public static int userID { get; set; }
        public static decimal loan_amount { get; set; }

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();
        Classes.clsParameter clsParameter = new clsParameter();
        public string returnCompanyDescription(int userID)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Company_Code FROM Membership WHERE userID='" + userID + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                //Return Company Description

                SqlDataAdapter CompanyAdapter = new SqlDataAdapter("SELECT Description FROM Company WHERE Company_Code = '" + dt.Rows[0].ItemArray[0].ToString() + "'", con);
                DataTable dtCompany = new DataTable();
                CompanyAdapter.Fill(dtCompany);

                if (dtCompany.Rows.Count > 0)
                {
                    return dtCompany.Rows[0].ItemArray[0].ToString();
                }
                else
                {
                    return "";
                }

            }

        }

        public string returnStatusDescription(int id)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM status_loan WHERE id = '" + id + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0].ItemArray[1].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public Boolean isShortTerm(string loan_Type)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT isShortTerm FROM loan_co_maker_no WHERE loan_type = '" + loan_Type + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows[0].ItemArray[0].ToString() == "True")
                {
                    return true; //Short Term Loan = 6 Co Makers
                }
                else
                {
                    return false; //Long Term Depends on AMOUNT
                }
            }
             
        }

        public int GetDot(string text, string stopAt = ".")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    string str = text.Substring(0, charLocation);
                    
                    int ans = Convert.ToInt32(str);
                    return ans + 1;
                }
            }

            return 0;
        }

        public decimal returnShareCapital(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetShareCapitalBalancePerMember";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userID", userid);

                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0].ItemArray[0] != DBNull.Value)
                    {
                        return Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        //With existing Loan for approval
        public Boolean checkIfSameLoanType(string loan_type,int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM vw_LoanDisplay WHERE userID = '" + userID + "' and type = '" + loan_type + "' and Status not in ('3','5','7')", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //Already has a exisiting loan with same type
                    if (dt.Rows[0].ItemArray[29].ToString() == "8")
                    {
                        Alert.show("Account with the same Loan type waiting for release", Alert.AlertType.error);
                    }
                    else if (dt.Rows[0].ItemArray[29].ToString() == "1")
                    {
                        Alert.show("Account with the same Loan type pending for approval", Alert.AlertType.error);
                    }
                    else if (dt.Rows[0].ItemArray[29].ToString() == "6")
                    {
                        Alert.show("Account with the same Loan type for board approval", Alert.AlertType.error);
                    }
                    else
                    {
                        Alert.show("Account with the same Loan type already approved", Alert.AlertType.error);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Boolean checkIfGreaterThanLoanableAmount(TextBox amount,string loan_Type)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Max_loan_Amount FROM Loan_Type WHERE Loan_Type = '" + loan_Type + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                decimal maxAmnt;

                maxAmnt = Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());

                if (Convert.ToDecimal(amount.Text) > maxAmnt)
                {
                    Alert.show("Please input valid amount!", Alert.AlertType.error);
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        //For Automatic computation of deferred and 20% net
        #region Automate_Computation
        //Check first if he/she has a deferred loan
        public Boolean hasDeferredLoan(int userid,string presentLoan_Type)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanBalancesDeferredForAutomaticComp";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@loan_Type", presentLoan_Type);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if(ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //Get Total Sum Amount of Deferred loans
        public decimal totalDeferredAmount(int userid,string presentLoan_Type)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanBalancesDeferredForAutomaticComp";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@loan_Type", presentLoan_Type);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                decimal sumttal = 0;

                for(int x = 0; x < ds.Tables[0].Rows.Count; x++)
                {
                    sumttal = sumttal + Convert.ToDecimal(ds.Tables[0].Rows[x]["Deferred"].ToString());
                }

                return sumttal;
            }
        }

        //Insert to temp Table 
        public void insertLoanDeferred(int userid,string presentLoan_Type,string loan_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanBalancesDeferredForAutomaticComp";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@loan_Type", presentLoan_Type);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                
                for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                {
                    SqlCommand cmdInsert = new SqlCommand();
                    cmdInsert.Connection = con;
                    cmdInsert.CommandText = "sp_InsertLoanAutoCompute";
                    cmdInsert.CommandType = CommandType.StoredProcedure;
                    cmdInsert.Parameters.AddWithValue("@loan_No", loan_no);
                    cmdInsert.Parameters.AddWithValue("@loan_Type", ds.Tables[0].Rows[x]["Loan_Type"].ToString());
                    cmdInsert.Parameters.AddWithValue("@Amount", Convert.ToDecimal(ds.Tables[0].Rows[x]["Deferred"].ToString()));
                    cmdInsert.ExecuteNonQuery();
                }
            }
        }

        //Get the percentage of deferred amount and update in table
        public void updateLoanDeferredPercent(string loan_no,decimal ttalDefAmount)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM loan_AutoCompute WHERE Loan_No ='"+ loan_no +"'", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if(ds.Tables[0].Rows.Count > 0)
                {
                    for(int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE loan_AutoCompute SET Percentage = '"+ Convert.ToDouble(ds.Tables[0].Rows[x]["Amount"].ToString()) / Convert.ToDouble(ttalDefAmount) + "' WHERE Loan_No = '"+ loan_no +"' and Loan_Type = '"+ ds.Tables[0].Rows[x]["Loan_Type"].ToString() +"'";
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        //Get the net amount 
        public decimal getTotalAmountMinusServiceFee(decimal loan_amount,decimal share,decimal savings,string loan_Type)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT val FROM Parameter WHERE val = '"+ loan_Type + "' and Description = 'No Service Fee'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                decimal ret = 0;
                if (dt.Rows.Count > 0)
                {
                    ret = loan_amount * clsParameter.serviceFee();
                }
                else
                {
                    ret = loan_amount * clsParameter.serviceFee();
                }
                ret = loan_amount - ret;
                ret = ret - share;
                ret = ret - savings;

                ret = ret * clsParameter.net20Percent();

                return ret;
            }
        }

        //Delete first before updating
        public void deleteLoanAuto(string loan_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "DELETE loan_AutoCompute Where Loan_No = '" + loan_no + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

        public void updateForShareandSavings(string loan_No,string loanAmount,string shareCapital,string savings,string loan_Type,DataGridView dgvLoanApproval)
        {
            deleteLoanAuto(loan_No);

            insertLoanDeferred(Classes.clsLoanDataEntry.userID, loan_Type, loan_No);

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();


                //Check if 20% of net is Greater than or Less Than
                if (getTotalAmountMinusServiceFee(Convert.ToDecimal(loanAmount), Convert.ToDecimal(shareCapital), Convert.ToDecimal(savings), loan_Type) > totalDeferredAmount(Classes.clsLoanDataEntry.userID, loan_Type))
                {
                    //if Greater than then as ease the value
                    SqlDataAdapter adapterGet = new SqlDataAdapter("select * from loan_AutoCompute where loan_no = '" + loan_No + "'", con);
                    DataSet ds = new DataSet();
                    adapterGet.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int xx = 0; xx < ds.Tables[0].Rows.Count; xx++)
                        {
                            foreach (DataGridViewRow row in dgvLoanApproval.Rows)
                            {
                                if (row.Cells[7].Value.ToString().Contains(ds.Tables[0].Rows[xx]["Loan_Type"].ToString()))
                                {
                                    row.Cells[4].Value = Convert.ToDecimal(ds.Tables[0].Rows[xx]["Amount"].ToString()).ToString("#,0.00");
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Move to percentage value and allocate
                    //Second compute percentage
                    updateLoanDeferredPercent(loan_No, totalDeferredAmount(Classes.clsLoanDataEntry.userID, loan_Type));

                    //Update Amount if The 20% of Net is less than the deferred amount
                    SqlDataAdapter adapterGet = new SqlDataAdapter("select * from loan_AutoCompute where loan_no = '" + loan_No + "'", con);
                    DataSet ds = new DataSet();
                    adapterGet.Fill(ds);
                    string amt;
                    decimal net = getTotalAmountMinusServiceFee(Convert.ToDecimal(loanAmount), Convert.ToDecimal(shareCapital), Convert.ToDecimal(savings), loan_Type);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                        {
                            SqlCommand cmd2 = new SqlCommand();
                            cmd2.Connection = con;
                            amt = Convert.ToString(Convert.ToDecimal(net) * Convert.ToDecimal(ds.Tables[0].Rows[x]["Percentage"].ToString()));
                            cmd2.CommandText = "UPDATE loan_AutoCompute set Amount = " + Convert.ToDecimal(decimal.Round(Convert.ToDecimal(amt), 2)) + " WHERE loan_type = '" + ds.Tables[0].Rows[x]["Loan_Type"].ToString() + "' and loan_no = '" + loan_No + "'";
                            cmd2.CommandType = CommandType.Text;
                            cmd2.ExecuteNonQuery();
                        }

                        //SELECT THE UPDATED VALUES IN TABLE
                        SqlDataAdapter adpterUpdate = new SqlDataAdapter("select * from loan_AutoCompute where loan_no = '" + loan_No + "'", con);
                        DataSet ds2 = new DataSet();
                        adpterUpdate.Fill(ds2);

                        for (int xx = 0; xx < ds2.Tables[0].Rows.Count; xx++)
                        {
                            foreach (DataGridViewRow row in dgvLoanApproval.Rows)
                            {
                                if (row.Cells[7].Value.ToString().Contains(ds2.Tables[0].Rows[xx]["Loan_Type"].ToString()))
                                {
                                    row.Cells[4].Value = Convert.ToDecimal(ds2.Tables[0].Rows[xx]["Amount"].ToString()).ToString("#,0.00");
                                }
                            }
                        }

                    }

                }
            }
        }

        #endregion
    }
}
