using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;

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
                        Alert.show("Account with the same Loan type waiting for release.", Alert.AlertType.error);
                    }
                    else if (dt.Rows[0].ItemArray[29].ToString() == "1")
                    {
                        Alert.show("Account with the same Loan type pending for approval.", Alert.AlertType.error);
                    }
                    else if (dt.Rows[0].ItemArray[29].ToString() == "6")
                    {
                        Alert.show("Account with the same Loan type for board approval.", Alert.AlertType.error);
                    }
                    else
                    {
                        Alert.show("Account with the same Loan type already approved.", Alert.AlertType.error);
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
                    Alert.show("Please enter valid amount.", Alert.AlertType.error);
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

        //===================================================================================
        //            LOAN DETAIL SCHEDULE PAYMENT [PDC / PAYROLL DEDUCTION]
        //===================================================================================

        public void createSchedule(string loanNo,string PaymentOption,double loanAmount,double interestRate,double termsInMonth,string loanType,string dateReleased)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //=================================================
                //       DELETE FIRST BEFORE INSERTING NEW
                //=================================================

                SqlCommand cmdDelete = new SqlCommand();
                cmdDelete.Connection = con;
                cmdDelete.CommandText = "DELETE Loan_Details WHERE Loan_No ='"+ loanNo +"'";
                cmdDelete.CommandType = CommandType.Text;
                cmdDelete.ExecuteNonQuery();

                //====================================================================================
                //              IF PDC OR PAYROLL PAYMENT
                //              INSERT LOAN DETAILS [FOR SCHEDULING OF PAYMENT]
                //====================================================================================
                if (PaymentOption == "PDC" || PaymentOption == "Savings Deduction")
                {
                    //Generate Loan Details for PDC

                    //====================================================================================
                    //              CREATE LOAN DETAILS FOR [PAYMENT || INTEREST || PRINCIPAL]
                    //====================================================================================
                    //CREATING LOAN DETAILS
                    double PV = loanAmount;
                    double rate = interestRate;
                    double term = termsInMonth;
                    double val1 = 1 + rate;
                    double val2 = -term;
                    double powResult = Math.Pow(val1, val2);
                    double rightSide = 1 - powResult;
                    double leftSide = PV * rate;
                    double finalResult = leftSide / rightSide;

                    double finalResult2 = (PV * rate) / (1 - (Math.Pow((1 + rate), -term)));
                    decimal dec = Convert.ToDecimal(finalResult2);


                    double z, i, ob;
                    double interest, principal;

                    //=============================================
                    //         DECLARATION FOR DATE
                    //=============================================

                    string str = returnBonusLoan(loanType, dateReleased);
                    string outputDate;
                    str = str.Replace("/", "-");
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    DateTime dateTime13 = DateTime.ParseExact(str, "MM-d-yyyy", provider);

                    int year, month, day;

                    year = Convert.ToInt32(dateTime13.Year.ToString());
                    month = Convert.ToInt32(dateTime13.Month.ToString());
                    day = Convert.ToInt32(dateTime13.Day.ToString());

                    //=============================================


                    //Getting the mons first deduction first
                    if (clsParameter.deductionMonth(loanType) != 0)
                    {
                        month = month + clsParameter.deductionMonth(loanType);
                    }


                    if (day <= 15)
                    {
                        day = DateTime.DaysInMonth(year, month);
                    }
                    else
                    {
                        day = 15;
                        month = month + 1;
                    }

                    if (Convert.ToInt32(Convert.ToString(month).Length) == 1)
                    {
                        outputDate = year.ToString() + "-0" + month.ToString() + "-" + day.ToString();
                    }
                    else
                    {
                        outputDate = year.ToString() + "-" + month.ToString() + "-" + day.ToString();
                    }

                    IFormatProvider culture = new CultureInfo("en-US", true);
                    DateTime dateVal = DateTime.ParseExact(outputDate, "yyyy-MM-dd", provider);

                    int cnt = Convert.ToInt32(termsInMonth);
                    for (int a = 0; a < cnt; a++)
                    {
                        z = PV * rate;
                        i = finalResult2 - z;



                        interest = z;
                        principal = i;
                        ob = PV - principal;
                        //================================================================

                        SqlCommand cmdDetail1 = new SqlCommand();
                        cmdDetail1.Connection = con;
                        cmdDetail1.CommandText = "sp_InsertLoanDetails";
                        cmdDetail1.CommandType = CommandType.StoredProcedure;
                        cmdDetail1.Parameters.AddWithValue("@Loan_No", loanNo);
                        cmdDetail1.Parameters.AddWithValue("@PaymentNoSemi", a);
                        cmdDetail1.Parameters.AddWithValue("@Payment", dec);
                        cmdDetail1.Parameters.AddWithValue("@Interest", Convert.ToString(decimal.Round(Convert.ToDecimal(interest), 2)));
                        cmdDetail1.Parameters.AddWithValue("@Principal", Convert.ToString(decimal.Round(Convert.ToDecimal(principal), 2)));
                        cmdDetail1.Parameters.AddWithValue("@Outstanding_Balance", Convert.ToString(Convert.ToDecimal(PV)));
                        cmdDetail1.Parameters.AddWithValue("@Original_Balance", Convert.ToString(Convert.ToDecimal(ob)));
                        cmdDetail1.Parameters.AddWithValue("@Schedule_Payment", dateVal.AddMonths(a).ToShortDateString());
                        cmdDetail1.ExecuteNonQuery();

                        PV = ob;
                        ob = 0;
                    }
                }
                else
                {
                    //Generatge Loan Details for Payroll Deduction
                    //====================================================================================
                    //              CREATE LOAN DETAILS FOR [PAYMENT || INTEREST || PRINCIPAL]
                    //====================================================================================
                    //CREATING LOAN DETAILS
                    double PV = loanAmount;
                    double rate = interestRate;
                    double term = termsInMonth;
                    double val1 = 1 + rate;
                    double val2 = -term;
                    double powResult = Math.Pow(val1, val2);
                    double rightSide = 1 - powResult;
                    double leftSide = PV * rate;
                    double finalResult = leftSide / rightSide;

                    double finalResult2 = (PV * rate) / (1 - (Math.Pow((1 + rate), -term)));
                    decimal dec = Convert.ToDecimal(finalResult2);


                    double z, i, ob;
                    double interest, principal;
                    dec = dec / 2;
                    int noPay = 1;


                    //=============================================
                    //         DECLARATION FOR DATE
                    //=============================================

                    string str = returnBonusLoan(loanType, dateReleased);
                    string outputDate;
                    str = str.Replace("/", "-");
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    DateTime dateTime13 = DateTime.ParseExact(str, "MM-d-yyyy", provider);

                    int year, month, day;

                    year = Convert.ToInt32(dateTime13.Year.ToString());
                    month = Convert.ToInt32(dateTime13.Month.ToString());
                    day = Convert.ToInt32(dateTime13.Day.ToString());

                    //=============================================

                    //Getting the mons first deduction first
                    if (clsParameter.deductionMonth(loanType) != 0)
                    {
                        month = month + clsParameter.deductionMonth(loanType);
                    }

                    int cnt = Convert.ToInt32(termsInMonth);
                    for (int a = 0; a < cnt; a++)
                    {
                        z = PV * rate;
                        i = finalResult2 - z;



                        interest = z / 2;
                        principal = i / 2;
                        ob = PV - principal;


                        //=====================
                        //for date
                        //=====================
                        if (month == 12)
                        {
                            if (day <= 15)
                            {
                                day = DateTime.DaysInMonth(year, month);
                            }
                            else
                            {
                                day = 15;
                                month = 1;
                                year = year + 1;
                            }
                        }
                        else
                        {
                            if (day <= 15)
                            {
                                day = DateTime.DaysInMonth(year, month);
                            }
                            else
                            {
                                day = 15;
                                month = month + 1;
                            }
                        }
                        if (Convert.ToInt32(Convert.ToString(month).Length) == 1)
                        {
                            outputDate = year.ToString() + "-0" + month.ToString() + "-" + day.ToString();
                        }
                        else
                        {
                            outputDate = year.ToString() + "-" + month.ToString() + "-" + day.ToString();
                        }
                        IFormatProvider culture = new CultureInfo("en-US", true);
                        DateTime dateVal = DateTime.ParseExact(outputDate, "yyyy-MM-dd", provider);
                        //================================================================

                        SqlCommand cmdDetail1 = new SqlCommand();
                        cmdDetail1.Connection = con;
                        cmdDetail1.CommandText = "sp_InsertLoanDetails";
                        cmdDetail1.CommandType = CommandType.StoredProcedure;
                        cmdDetail1.Parameters.AddWithValue("@Loan_No", loanNo);
                        cmdDetail1.Parameters.AddWithValue("@PaymentNoSemi", noPay);
                        cmdDetail1.Parameters.AddWithValue("@Payment", dec);
                        cmdDetail1.Parameters.AddWithValue("@Interest", Convert.ToString(decimal.Round(Convert.ToDecimal(interest), 2)));
                        cmdDetail1.Parameters.AddWithValue("@Principal", Convert.ToString(decimal.Round(Convert.ToDecimal(principal), 2)));
                        cmdDetail1.Parameters.AddWithValue("@Outstanding_Balance", Convert.ToString(Convert.ToDecimal(PV)));
                        cmdDetail1.Parameters.AddWithValue("@Original_Balance", Convert.ToString(Convert.ToDecimal(ob)));
                        cmdDetail1.Parameters.AddWithValue("@Schedule_Payment", dateVal.ToShortDateString());
                        cmdDetail1.ExecuteNonQuery();

                        if (month == 12)
                        {
                            if (day <= 15)
                            {
                                day = DateTime.DaysInMonth(year, month);
                            }
                            else
                            {
                                day = 15;
                                month = 1;
                                year = year + 1;
                            }
                        }
                        else
                        {
                            if (day <= 15)
                            {
                                day = DateTime.DaysInMonth(year, month);
                            }
                            else
                            {
                                day = 15;
                                month = month + 1;
                            }
                        }
                        if (Convert.ToInt32(Convert.ToString(month).Length) == 1)
                        {
                            outputDate = year.ToString() + "-0" + month.ToString() + "-" + day.ToString();
                        }
                        else
                        {
                            outputDate = year.ToString() + "-" + month.ToString() + "-" + day.ToString();
                        }
                        DateTime dateVal2 = DateTime.ParseExact(outputDate, "yyyy-MM-dd", provider);


                        noPay = noPay + 1;
                        SqlCommand cmd1 = new SqlCommand();
                        cmd1.Connection = con;
                        cmd1.CommandText = "sp_InsertLoanDetails";
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@Loan_No", loanNo);
                        cmd1.Parameters.AddWithValue("@PaymentNoSemi", noPay);
                        cmd1.Parameters.AddWithValue("@Payment", dec);
                        cmd1.Parameters.AddWithValue("@Interest", Convert.ToString(decimal.Round(Convert.ToDecimal(interest), 2)));
                        cmd1.Parameters.AddWithValue("@Principal", Convert.ToString(decimal.Round(Convert.ToDecimal(principal), 2)));
                        cmd1.Parameters.AddWithValue("@Outstanding_Balance", Convert.ToString(decimal.Round(Convert.ToDecimal(ob), 2)));
                        ob = ob - principal;
                        cmd1.Parameters.AddWithValue("@Original_Balance", Convert.ToString(decimal.Round(Convert.ToDecimal(ob), 2)));
                        cmd1.Parameters.AddWithValue("@Schedule_Payment", dateVal2.ToShortDateString());
                        cmd1.ExecuteNonQuery();

                        noPay = noPay + 1;
                        z = 0;
                        PV = ob;
                        ob = 0;
                    }
                }
            }
        }

        //==============================================================
        //          RETURN PAYMENT OPTION
        //==============================================================
        public string returnPaymentOption(string loanNo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Payment_Option FROM Loan WHERE Loan_No = '"+ loanNo +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }

        public double loanAmountGross(string loanNo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Loan_Amount FROM Loan Where Loan_No = '"+ loanNo +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString());
            }
        }

        public double returnInterest(string loanNo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Interest FROM Loan Where Loan_No = '"+ loanNo +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString());
            }
        }

        public double returnTermsInMonth(string loanNo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Terms FROM Loan WHERE Loan_No = '"+ loanNo +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString());
            }
        }

        public string returnBonusLoan(string loan_type, string dtToday)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT val FROM Parameter WHERE Description = 'Bonus Loans' and val ='" + loan_type + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                DateTime dtTodayYear = DateTime.Today;

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0].ItemArray[0].ToString() == "BALMY")
                    {
                        return "05/15/" + dtTodayYear.Year;
                    }
                    else if (dt.Rows[0].ItemArray[0].ToString() == "BAL")
                    {
                        return "11/15/" + dtTodayYear.Year;
                    }
                    else
                    {
                        return "12/15/" + dtTodayYear.Year;
                    }
                }
                else
                {
                    return dtToday;
                }
            }
        }
    }
}
