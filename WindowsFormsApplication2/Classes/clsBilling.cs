using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Globalization;

namespace WindowsFormsApplication2.Classes
{
    class clsBilling
    {
        Global global = new Global();
        clsParameter clsParameter = new clsParameter();

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        //======================================
        //      DECLARATION / VARIABLES
        //======================================
        public static Boolean ClearAllLoans { get; set; }

        //======================================
        //      GET ALL LOAN BALANCES
        //======================================

        public void generateBillingAccndgCompRank(DataGridView dgv,string company,string rank,DateTimePicker dtBillDate)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_SetupBilling";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Payroll_Code", rank);
                cmd.Parameters.AddWithValue("@Company_Code", company);

                adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                
                //Reconsturct loans with no loan details
                //Came from inserting only the loan header and the outstanding balance from journal or other means of entry

                int noCnt;
                int val = 0;
                double interest, principal, fnalInterest;   

                //Check if theres a Breakdown of loan details for billing purposes
                for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                {
                    SqlDataAdapter adapterCheckLoanDetails = new SqlDataAdapter("SELECT top 1 * from Loan_Details where loan_no = '" + ds.Tables[0].Rows[x]["Loan_No"].ToString() + "' and isDone = '0' Order by PaymentNoSemi", con);
                    DataTable dt = new DataTable();
                    adapterCheckLoanDetails.Fill(dt);

                    /*
                     *  This is for reconstruct of loans details 
                     *  breakdown of loan per semi monthly
                     *  interest and principal recompute
                    */

                    if(dt.Rows.Count == 0)
                    {
                        //No Record or All the loan details is used
                        //We Will Create Record for this loan
                        
                        //Check first the last count if theres a loan before or not
                        SqlDataAdapter adapterCnt = new SqlDataAdapter("SELECT TOP 1 PaymentNoSemi FROM Loan_Details where loan_no = '" + ds.Tables[0].Rows[x]["Loan_No"].ToString() + "' and isDone = 1 Order by PaymentNoSemi DESC", con);
                        DataSet dsCnt = new DataSet();
                        adapterCnt.Fill(dsCnt);

                        if(dsCnt.Tables[0].Rows.Count  > 0)
                        {
                            noCnt = Convert.ToInt32(dsCnt.Tables[0].Rows[0]["PaymentNoSemi"].ToString()) + 1;
                        }
                        else
                        {
                            noCnt = 1;
                        }

                        interest = Convert.ToDouble(ds.Tables[0].Rows[x]["Balance"].ToString()) * Convert.ToDouble(ds.Tables[0].Rows[x]["Interest"].ToString());
                        fnalInterest = interest / 2;

                        principal = Convert.ToDouble(ds.Tables[0].Rows[x]["Semi_Monthly_Amort"].ToString()) - fnalInterest;


                        string str = Convert.ToString(dtBillDate.Value.ToString("MM/dd/yyyy"));
                        string outputDate;
                        str = str.Replace("/", "-");
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        DateTime dateTime13 = DateTime.ParseExact(str, "MM-d-yyyy", provider);

                        int year, month, day;

                        year = Convert.ToInt32(dateTime13.Year.ToString());
                        month = Convert.ToInt32(dateTime13.Month.ToString());
                        day = Convert.ToInt32(dateTime13.Day.ToString());

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

                        //re-compute and insert two records for 1 month
                        for (int i = 0; i < 1; i++)
                        {
                                                    
                            SqlCommand cmdInsert = new SqlCommand();
                            cmdInsert.Connection = con;
                            cmdInsert.CommandText = "sp_InsertLoanDetailsReconstruct";
                            cmdInsert.CommandType = CommandType.StoredProcedure;
                            cmdInsert.Parameters.AddWithValue("@Loan_No", ds.Tables[0].Rows[x]["Loan_No"].ToString());
                            cmdInsert.Parameters.AddWithValue("@Schedule_Payment", dateVal.ToShortDateString());
                            cmdInsert.Parameters.AddWithValue("@PaymentNoSemi", noCnt);
                            cmdInsert.Parameters.AddWithValue("@Payment", ds.Tables[0].Rows[x]["Semi_Monthly_Amort"].ToString()); //Semi Monthly
                            cmdInsert.Parameters.AddWithValue("@Interest", Convert.ToString(decimal.Round(Convert.ToDecimal(fnalInterest), 2)));
                            cmdInsert.Parameters.AddWithValue("@Principal", Convert.ToString(decimal.Round(Convert.ToDecimal(principal), 2)));
                            cmdInsert.Parameters.AddWithValue("@isDOne", false);
                            cmdInsert.ExecuteNonQuery();

                            noCnt = noCnt + 1;


                            if(day == 15)
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

                            DateTime dateVal2 = DateTime.ParseExact(outputDate, "yyyy-MM-dd", provider);

                            SqlCommand cmdInsert2 = new SqlCommand();
                            cmdInsert2.Connection = con;
                            cmdInsert2.CommandText = "sp_InsertLoanDetailsReconstruct";
                            cmdInsert2.CommandType = CommandType.StoredProcedure;
                            cmdInsert2.Parameters.AddWithValue("@Loan_No", ds.Tables[0].Rows[x]["Loan_No"].ToString());
                            cmdInsert2.Parameters.AddWithValue("@Schedule_Payment", dateVal2.ToShortDateString());
                            cmdInsert2.Parameters.AddWithValue("@PaymentNoSemi", noCnt);
                            cmdInsert2.Parameters.AddWithValue("@Payment", ds.Tables[0].Rows[x]["Semi_Monthly_Amort"].ToString()); //Semi Monthly
                            cmdInsert2.Parameters.AddWithValue("@Interest", Convert.ToString(decimal.Round(Convert.ToDecimal(fnalInterest), 2)));
                            cmdInsert2.Parameters.AddWithValue("@Principal", Convert.ToString(decimal.Round(Convert.ToDecimal(principal), 2)));
                            cmdInsert2.Parameters.AddWithValue("@isDOne", false);
                            cmdInsert2.ExecuteNonQuery();
                        }
                    }

                } //END Checking of loans Details [Per Loan]


                //PUT THE LOAN DETAILS TO TEMP TALBE
                checkNinsertLoanDetails(ds);

                //Date Billing
                FilterLoansInTempDate(dtBillDate);

                if(company != "COMP021")
                {
                    //Insert to billing table
                    //NOT SMART 
                    insertToBilling(dtBillDate);
                }
                else
                {
                    insertToBillingSMART(dtBillDate);
                }

                

            }//End Connection
               
        }

        //==================================================================================
        //      CHECKING OF LOAN DETAILS AND INSERTION TO TEMP TABLE LOAN DETAILS
        //==================================================================================

        public void checkNinsertLoanDetails(DataSet ds)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //DELETE FIRST BEFORE WE UPLOAD TO TEMP TABLE
                SqlCommand cmdDelete = new SqlCommand();
                cmdDelete.Connection = con;
                cmdDelete.CommandText = "DELETE temp_Loan_Details";
                cmdDelete.CommandType = CommandType.Text;
                cmdDelete.ExecuteNonQuery();

                for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                {
                    SqlDataAdapter adapterCheckLoanDetails = new SqlDataAdapter("SELECT top 1 * from Loan_Details where loan_no = '" + ds.Tables[0].Rows[x]["Loan_No"].ToString() + "' and isDone = '0' Order by PaymentNoSemi", con);
                    DataSet dsInsert = new DataSet();
                    adapterCheckLoanDetails.Fill(dsInsert);

                    //Has a loan detail then insert to temp table loan details
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "sp_InsertTempLoanDetails";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Loan_No", dsInsert.Tables[0].Rows[0]["Loan_No"].ToString());
                        cmd.Parameters.AddWithValue("@userid", getUserid(dsInsert.Tables[0].Rows[0]["Loan_No"].ToString()));
                        cmd.Parameters.AddWithValue("@Schedule_Payment", Convert.ToDateTime(dsInsert.Tables[0].Rows[0]["Schedule_Payment"].ToString()).ToShortDateString());
                        cmd.Parameters.AddWithValue("@PaymentNoSemi", dsInsert.Tables[0].Rows[0]["PaymentNoSemi"].ToString());
                        cmd.Parameters.AddWithValue("@Payment", Convert.ToDecimal(dsInsert.Tables[0].Rows[0]["Payment"].ToString()));
                        cmd.Parameters.AddWithValue("@Interest", Convert.ToDecimal(dsInsert.Tables[0].Rows[0]["Interest"].ToString()));
                        cmd.Parameters.AddWithValue("@Principal", Convert.ToDecimal(dsInsert.Tables[0].Rows[0]["Principal"].ToString()));
                        cmd.ExecuteNonQuery();
                    }
                } // END CHECKING AND INSERTING TO TEMP
            }
        }

        //==================================================================================
        //      FILTER LOANS DETAIL AND DELETE LOANS DATE NOT EQUAL TO PARAMETER
        //==================================================================================

        public void FilterLoansInTempDate(DateTimePicker dtBillDate)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                
                SqlCommand cmdDelete = new SqlCommand();
                cmdDelete.Connection = con;
                cmdDelete.CommandText = "DELETE temp_Loan_Details WHERE Schedule_Payment <> '" + dtBillDate.Value.ToShortDateString() + "'";
                cmdDelete.CommandType = CommandType.Text;
                cmdDelete.ExecuteNonQuery();

                //After deleting the loan details not equal to the date selected
                //Then Select All ODD numbers and recompute for Interest

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM temp_Loan_Details", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                double interest, FinalInterest,principal;
                interest = 0;
                FinalInterest = 0;
                principal = 0;
                int payNo;
                payNo = 0;

                if(ds.Tables[0].Rows.Count > 0)
                {
                    for(int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        if(Convert.ToInt32(ds.Tables[0].Rows[x]["PaymentNoSemi"].ToString()) % 2 != 0)
                        {
                            //GETTING ALL ODD NUMBERS
                            //AND RECOMPUTE THE INTEREST AND PRINCIPAL OF THE LOANS

                            //Balance * Interest
                            interest = outstandingBalance(ds.Tables[0].Rows[x]["Loan_No"].ToString()) * 0.0075;
                            FinalInterest = interest / 2;
                            principal = semiMonthlyAmort(ds.Tables[0].Rows[x]["Loan_No"].ToString()) - FinalInterest;
                            payNo = Convert.ToInt32(ds.Tables[0].Rows[x]["PaymentNoSemi"].ToString());
                            //UPDATE FIRST IN LOAN DETAIL TABLE
                            //UPDATE 2 RECORDS FOR 1 MONTH INTEREST COMPUTATION
                            for (int upd = 0; upd < 2; upd++)
                            {
                                SqlCommand cmdUpdate = new SqlCommand();
                                cmdUpdate.Connection = con;
                                cmdUpdate.CommandText = "sp_UpdateLoanDetailsReconstruct";
                                cmdUpdate.CommandType = CommandType.StoredProcedure;
                                cmdUpdate.Parameters.AddWithValue("@Loan_No", ds.Tables[0].Rows[x]["Loan_No"].ToString());
                                cmdUpdate.Parameters.AddWithValue("@PaymentNoSemi", payNo);
                                cmdUpdate.Parameters.AddWithValue("@Interest", Convert.ToString(decimal.Round(Convert.ToDecimal(FinalInterest), 2)));
                                cmdUpdate.Parameters.AddWithValue("@Principal", Convert.ToString(decimal.Round(Convert.ToDecimal(principal), 2)));
                                cmdUpdate.ExecuteNonQuery();

                                payNo = payNo + 1;
                            }


                            //UPDATE ALSO THE TEMP TABLE FOR THE SAME LOAN NO
                            SqlCommand cmdTempUp = new SqlCommand();
                            cmdTempUp.Connection = con;
                            cmdTempUp.CommandText = "UPDATE temp_Loan_Details SET Interest = '"+ FinalInterest +"', Principal = '"+ principal +"' WHERE Loan_No = '"+ ds.Tables[0].Rows[x]["Loan_No"].ToString() + "'";
                            cmdTempUp.CommandType = CommandType.Text;
                            cmdTempUp.ExecuteNonQuery();


                            //Reset the values first
                            interest = 0;
                            FinalInterest = 0;
                            principal = 0;
                            payNo = 0;
                        }
                    }
                }
            }
        }

        public void insertToBilling(DateTimePicker dtBillDate)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM temp_Loan_Details ORDER BY userid,loan_no", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                
                if(ds.Tables[0].Rows.Count > 0)
                {
                    //INSERT INTO BILLING TABLE CURRENT AND DEFERRED + UNEARN
                    for(int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        //CHECK FIRST IF HAS A DEFERRED + UNEARN
                        if(hasDeferred(ds.Tables[0].Rows[x]["Loan_No"].ToString()) == true)
                        {
                            //Has a deferred 
                            //do the function insert here

                            //Check first for deferred interest or unearn
                            //then insert first the deferred unearn before the whole deferred amount
                            if(Convert.ToString(getUnearnAmount(ds.Tables[0].Rows[x]["Loan_No"].ToString())) != "0.00")
                            {
                                //Insert the unearn amount
                                SqlCommand cmd = new SqlCommand();
                                cmd.Connection = con;
                                cmd.CommandText = "sp_InsertBilling";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                                cmd.Parameters.AddWithValue("@EmployeeID", getEmpID(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                                cmd.Parameters.AddWithValue("@PrincipalID", getPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                                cmd.Parameters.AddWithValue("@isPrincipal", isPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                                cmd.Parameters.AddWithValue("@Company_Code", getCompanyCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                                cmd.Parameters.AddWithValue("@Payroll_Code", getPayrollCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                                cmd.Parameters.AddWithValue("@Loan_No", ds.Tables[0].Rows[x]["Loan_No"].ToString());
                                cmd.Parameters.AddWithValue("@Deduction_Code", "DEFERRED INTEREST");
                                cmd.Parameters.AddWithValue("@Priority", getLoanPriority(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                                cmd.Parameters.AddWithValue("@Interest", getUnearnAmount(ds.Tables[0].Rows[x]["Loan_No"].ToString())); //Amount of Interest
                                cmd.Parameters.AddWithValue("@Principal", 0.00); //Amount of Principal
                                cmd.Parameters.AddWithValue("@Deferred", getUnearnAmount(ds.Tables[0].Rows[x]["Loan_No"].ToString())); //Deferred Amount (Principal + Interest)
                                cmd.Parameters.AddWithValue("@TotalDueAmount", getUnearnAmount(ds.Tables[0].Rows[x]["Loan_No"].ToString())); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                                cmd.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                                cmd.ExecuteNonQuery();

                                //Insert Principal Amount
                                SqlCommand cmd2 = new SqlCommand();
                                cmd2.Connection = con;
                                cmd2.CommandText = "sp_InsertBilling";
                                cmd2.CommandType = CommandType.StoredProcedure;
                                cmd2.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                                cmd2.Parameters.AddWithValue("@EmployeeID", getEmpID(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                                cmd2.Parameters.AddWithValue("@PrincipalID", getPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                                cmd2.Parameters.AddWithValue("@isPrincipal", isPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                                cmd2.Parameters.AddWithValue("@Company_Code", getCompanyCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                                cmd2.Parameters.AddWithValue("@Payroll_Code", getPayrollCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                                cmd2.Parameters.AddWithValue("@Loan_No", ds.Tables[0].Rows[x]["Loan_No"].ToString());
                                cmd2.Parameters.AddWithValue("@Deduction_Code", "DEFERRED PRINCIPAL");
                                cmd2.Parameters.AddWithValue("@Priority", getLoanPriority(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                                cmd2.Parameters.AddWithValue("@Interest", 0.00); //Amount of Interest
                                cmd2.Parameters.AddWithValue("@Principal", getPrincipalDeferred(ds.Tables[0].Rows[x]["Loan_No"].ToString())); //Amount of Principal
                                cmd2.Parameters.AddWithValue("@Deferred", getPrincipalDeferred(ds.Tables[0].Rows[x]["Loan_No"].ToString())); //Deferred Amount (Principal + Interest)
                                cmd2.Parameters.AddWithValue("@TotalDueAmount", getPrincipalDeferred(ds.Tables[0].Rows[x]["Loan_No"].ToString())); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                                cmd2.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                                cmd2.ExecuteNonQuery();
                            }
                            else
                            {
                                //Just insert the deferred amount only and put the amount in principal
                                SqlCommand cmd2 = new SqlCommand();
                                cmd2.Connection = con;
                                cmd2.CommandText = "sp_InsertBilling";
                                cmd2.CommandType = CommandType.StoredProcedure;
                                cmd2.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                                cmd2.Parameters.AddWithValue("@EmployeeID", getEmpID(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                                cmd2.Parameters.AddWithValue("@PrincipalID", getPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                                cmd2.Parameters.AddWithValue("@isPrincipal", isPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                                cmd2.Parameters.AddWithValue("@Company_Code", getCompanyCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                                cmd2.Parameters.AddWithValue("@Payroll_Code", getPayrollCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                                cmd2.Parameters.AddWithValue("@Loan_No", ds.Tables[0].Rows[x]["Loan_No"].ToString());
                                cmd2.Parameters.AddWithValue("@Deduction_Code", "DEFERRED PRINCIPAL");
                                cmd2.Parameters.AddWithValue("@Priority", getLoanPriority(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                                cmd2.Parameters.AddWithValue("@Interest", 0.00); //Amount of Interest
                                cmd2.Parameters.AddWithValue("@Principal", getPrincipalDeferred(ds.Tables[0].Rows[x]["Loan_No"].ToString())); //Amount of Principal
                                cmd2.Parameters.AddWithValue("@Deferred", getPrincipalDeferred(ds.Tables[0].Rows[x]["Loan_No"].ToString())); //Deferred Amount (Principal + Interest)
                                cmd2.Parameters.AddWithValue("@TotalDueAmount", getPrincipalDeferred(ds.Tables[0].Rows[x]["Loan_No"].ToString())); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                                cmd2.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                                cmd2.ExecuteNonQuery();
                            }
                            

                        }
                        //Insert for current amount of amort 
                        //according to the semi amort and created billing in temp table
                        SqlCommand cmdMove = new SqlCommand();
                        cmdMove.Connection = con;
                        cmdMove.CommandText = "sp_InsertBilling";
                        cmdMove.CommandType = CommandType.StoredProcedure;
                        cmdMove.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                        cmdMove.Parameters.AddWithValue("@EmployeeID", getEmpID(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                        cmdMove.Parameters.AddWithValue("@PrincipalID", getPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMove.Parameters.AddWithValue("@isPrincipal", isPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMove.Parameters.AddWithValue("@Company_Code", getCompanyCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMove.Parameters.AddWithValue("@Payroll_Code", getPayrollCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMove.Parameters.AddWithValue("@Loan_No", ds.Tables[0].Rows[x]["Loan_No"].ToString());
                        cmdMove.Parameters.AddWithValue("@Deduction_Code", "LOAN INTEREST");
                        cmdMove.Parameters.AddWithValue("@Priority", getLoanPriority(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                        cmdMove.Parameters.AddWithValue("@Interest", Convert.ToDecimal(ds.Tables[0].Rows[x]["Interest"].ToString())); //Amount of Interest
                        cmdMove.Parameters.AddWithValue("@Principal", 0.00); //Amount of Principal
                        cmdMove.Parameters.AddWithValue("@Deferred", 0.00); //Deferred Amount (Principal + Interest)
                        cmdMove.Parameters.AddWithValue("@TotalDueAmount", Convert.ToDecimal(ds.Tables[0].Rows[x]["Interest"].ToString())); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                        cmdMove.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                        cmdMove.ExecuteNonQuery();

                        //insert to principal only
                        SqlCommand cmdMovePrincipal = new SqlCommand();
                        cmdMovePrincipal.Connection = con;
                        cmdMovePrincipal.CommandText = "sp_InsertBilling";
                        cmdMovePrincipal.CommandType = CommandType.StoredProcedure;
                        cmdMovePrincipal.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                        cmdMovePrincipal.Parameters.AddWithValue("@EmployeeID", getEmpID(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                        cmdMovePrincipal.Parameters.AddWithValue("@PrincipalID", getPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMovePrincipal.Parameters.AddWithValue("@isPrincipal", isPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMovePrincipal.Parameters.AddWithValue("@Company_Code", getCompanyCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMovePrincipal.Parameters.AddWithValue("@Payroll_Code", getPayrollCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMovePrincipal.Parameters.AddWithValue("@Loan_No", ds.Tables[0].Rows[x]["Loan_No"].ToString());
                        cmdMovePrincipal.Parameters.AddWithValue("@Deduction_Code", "LOAN PRINCIPAL");
                        cmdMovePrincipal.Parameters.AddWithValue("@Priority", getLoanPriority(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                        cmdMovePrincipal.Parameters.AddWithValue("@Interest", 0.00); //Amount of Interest
                        cmdMovePrincipal.Parameters.AddWithValue("@Principal", Convert.ToDecimal(ds.Tables[0].Rows[x]["Principal"].ToString())); //Amount of Principal
                        cmdMovePrincipal.Parameters.AddWithValue("@Deferred", 0.00); //Deferred Amount (Principal + Interest)
                        cmdMovePrincipal.Parameters.AddWithValue("@TotalDueAmount", Convert.ToDecimal(ds.Tables[0].Rows[x]["Principal"].ToString())); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                        cmdMovePrincipal.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                        cmdMovePrincipal.ExecuteNonQuery();
                    }
                }
            }
        }

        //=====================================================================================
        //                  FOR SMART BILLING ONLY THE CURRENT AMORT
        //=====================================================================================
        public void insertToBillingSMART(DateTimePicker dtBillDate)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM temp_Loan_Details ORDER BY userid,loan_no", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    //INSERT INTO BILLING TABLE CURRENT ONLY
                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        //Insert for current amount of amort 
                        //according to the semi amort and created billing in temp table
                        SqlCommand cmdMove = new SqlCommand();
                        cmdMove.Connection = con;
                        cmdMove.CommandText = "sp_InsertBilling";
                        cmdMove.CommandType = CommandType.StoredProcedure;
                        cmdMove.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                        cmdMove.Parameters.AddWithValue("@EmployeeID", getEmpID(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                        cmdMove.Parameters.AddWithValue("@PrincipalID", getPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMove.Parameters.AddWithValue("@isPrincipal", isPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMove.Parameters.AddWithValue("@Company_Code", getCompanyCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMove.Parameters.AddWithValue("@Payroll_Code", getPayrollCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMove.Parameters.AddWithValue("@Loan_No", ds.Tables[0].Rows[x]["Loan_No"].ToString());
                        cmdMove.Parameters.AddWithValue("@Deduction_Code", "LOAN INTEREST");
                        cmdMove.Parameters.AddWithValue("@Priority", getLoanPriority(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                        cmdMove.Parameters.AddWithValue("@Interest", Convert.ToDecimal(ds.Tables[0].Rows[x]["Interest"].ToString())); //Amount of Interest
                        cmdMove.Parameters.AddWithValue("@Principal", 0.00); //Amount of Principal
                        cmdMove.Parameters.AddWithValue("@Deferred", 0.00); //Deferred Amount (Principal + Interest)
                        cmdMove.Parameters.AddWithValue("@TotalDueAmount", Convert.ToDecimal(ds.Tables[0].Rows[x]["Interest"].ToString())); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                        cmdMove.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                        cmdMove.ExecuteNonQuery();

                        //insert to principal only
                        SqlCommand cmdMovePrincipal = new SqlCommand();
                        cmdMovePrincipal.Connection = con;
                        cmdMovePrincipal.CommandText = "sp_InsertBilling";
                        cmdMovePrincipal.CommandType = CommandType.StoredProcedure;
                        cmdMovePrincipal.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                        cmdMovePrincipal.Parameters.AddWithValue("@EmployeeID", getEmpID(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                        cmdMovePrincipal.Parameters.AddWithValue("@PrincipalID", getPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMovePrincipal.Parameters.AddWithValue("@isPrincipal", isPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMovePrincipal.Parameters.AddWithValue("@Company_Code", getCompanyCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMovePrincipal.Parameters.AddWithValue("@Payroll_Code", getPayrollCode(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmdMovePrincipal.Parameters.AddWithValue("@Loan_No", ds.Tables[0].Rows[x]["Loan_No"].ToString());
                        cmdMovePrincipal.Parameters.AddWithValue("@Deduction_Code", "LOAN PRINCIPAL");
                        cmdMovePrincipal.Parameters.AddWithValue("@Priority", getLoanPriority(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                        cmdMovePrincipal.Parameters.AddWithValue("@Interest", 0.00); //Amount of Interest
                        cmdMovePrincipal.Parameters.AddWithValue("@Principal", Convert.ToDecimal(ds.Tables[0].Rows[x]["Principal"].ToString())); //Amount of Principal
                        cmdMovePrincipal.Parameters.AddWithValue("@Deferred", 0.00); //Deferred Amount (Principal + Interest)
                        cmdMovePrincipal.Parameters.AddWithValue("@TotalDueAmount", Convert.ToDecimal(ds.Tables[0].Rows[x]["Principal"].ToString())); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                        cmdMovePrincipal.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                        cmdMovePrincipal.ExecuteNonQuery();
                    }
                }
            }
        }

        //=====================================================================================
        //          GETTING THE PRINCIPAL PER USERID#
        //=====================================================================================
        public string getPrincipal(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                if(isPrincipal(userid) == true)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT PrincipalID FROM Membership WHERE userID = '" + userid + "'", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return dt.Rows[0].ItemArray[0].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public Boolean isPrincipal(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Principal FROM Membership WHERE userID = '"+ userid +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                
                if (dt.Rows[0].ItemArray[0].ToString() == "True")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //=====================================================================================
        //          GETTING THE LOANTYPE PER LOAN#
        //=====================================================================================
        public string getLoanType(string loan_No)
        {
            if (loan_No != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT loan_type from Loan WHERE Loan_No = '" + loan_No + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    return ds.Tables[0].Rows[0]["Loan_Type"].ToString();
                }
            }
            else
            {
                return "";
            }
        }

        //=====================================================================================
        //          GETTING THE LOAN PRIORITY PER LOAN TYPE
        //=====================================================================================
        public int getLoanPriority(string loan_No)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Priority FROM loan_Type WHERE Loan_Type = '"+ getLoanType(loan_No) +"'", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                return Convert.ToInt32(ds.Tables[0].Rows[0]["Priority"].ToString());
            }
        }


        //=====================================================================================
        //          CHECKING OF DEFERRED AND UNEARN PER LOAN#
        //=====================================================================================
        public Boolean hasDeferred(string loan_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanDeferredAmount";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Loan_no", loan_no);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //=====================================================================================
        //          GETTING THE UNEARN AMOUNT PER LOAN#
        //=====================================================================================
        public decimal getUnearnAmount(string loan_No)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT ISNULL(SUM(CREDIT - DEBIT), 0) as unearn from vw_Query_Total where loan_no = '" + loan_No + "' and account_code = '314'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());
            }
        }

        //=====================================================================================
        //          GETTING THE PRINCIPAL DEFERRED AMOUNT PER LOAN#
        //=====================================================================================
        public decimal getPrincipalDeferred(string loan_No)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanDeferredAmount";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Loan_no", loan_No);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                decimal deferred, Principal;

                deferred = Convert.ToDecimal(ds.Tables[0].Rows[0]["Deferred"].ToString());

                Principal = deferred - getUnearnAmount(loan_No);

                return Principal;
            }
        }


        //=====================================================================================
        //          GETTING THE USERID PER LOAN#
        //=====================================================================================
        public int getUserid(string loan_No)
        {
            if(loan_No != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT userID from Loan WHERE Loan_No = '"+ loan_No +"'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    return Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString());
                }
            }
            else
            {
                return 0;
            }
        }

        //=====================================================================================
        //          GETTING THE EMPLOYEEID PER LOAN#
        //=====================================================================================
        public string getEmpID(string loan_No)
        {
            if (loan_No != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT EmployeeID from Loan WHERE Loan_No = '" + loan_No + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    return ds.Tables[0].Rows[0]["EmployeeID"].ToString();
                }
            }
            else
            {
                return "";
            }
        }

        //=====================================================================================
        //          GETTING THE COMPANY PER LOAN#
        //=====================================================================================
        public string getCompanyCode(int userid)
        {
            if (userid.ToString() != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT Company_Code from Membership WHERE userID = '" + userid + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    return ds.Tables[0].Rows[0]["Company_Code"].ToString();
                }
            }
            else
            {
                return "";
            }
        }

        //=====================================================================================
        //          GETTING THE PAYROLL GROUP PER LOAN#
        //=====================================================================================
        public string getPayrollCode(int userid)
        {
            if (userid.ToString() != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT Payroll_Code from Membership WHERE userID = '" + userid + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    return ds.Tables[0].Rows[0]["Payroll_Code"].ToString();
                }
            }
            else
            {
                return "";
            }
        }

        //=====================================================================================
        //      GETTING THE LOAN INFORMATION FOR RECOMPUTATION OF LOAN DETAILS
        //=====================================================================================

        //GET THE OUTSTANDING BALANCE FOR RECOMPUTE
        public double outstandingBalance(string loan_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanBalancesOutstanding";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@loan_no", loan_no);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                return Convert.ToDouble(ds.Tables[0].Rows[0]["Balance"].ToString());
            }
        }

        public double semiMonthlyAmort(string loan_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanBalancesOutstanding";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@loan_no", loan_no);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                return Convert.ToDouble(ds.Tables[0].Rows[0]["Semi_Monthly_Amort"].ToString());
            }
        }


        //======================================================================================
        //                          END LOAN INFORMATION 
        //======================================================================================

        //GENERATE BILLING FOR MEMBERSHIP
        public void generateMembershipFee(DateTimePicker dtBillDate,string Company,string rank)
        {
            //Check if already has data in table
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Billing WHERE Deduction_Code = 'MEMBERSHIP FEE' and BillDate ='"+ dtBillDate.Value.ToShortDateString() +"' and Company_Code = '"+ Company +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    //Already generated
                    return;
                }
                else
                {
                    adapter = new SqlDataAdapter("SELECT * FROM membersFee WHERE isDone ='0' and Company_Code = '" + Company + "' and Payroll_Code = '" + rank + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        //MOVE TO BILLING TABLE
                        cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "sp_InsertBilling";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                        cmd.Parameters.AddWithValue("@EmployeeID", ds.Tables[0].Rows[x]["EmployeeID"].ToString());
                        cmd.Parameters.AddWithValue("@PrincipalID", getPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmd.Parameters.AddWithValue("@isPrincipal", isPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                        cmd.Parameters.AddWithValue("@Company_Code", ds.Tables[0].Rows[x]["Company_Code"].ToString());
                        cmd.Parameters.AddWithValue("@Payroll_Code", ds.Tables[0].Rows[x]["Payroll_Code"].ToString());
                        cmd.Parameters.AddWithValue("@Loan_No", "");
                        cmd.Parameters.AddWithValue("@Deduction_Code", "MEMBERSHIP FEE");
                        cmd.Parameters.AddWithValue("@Priority", "1");
                        cmd.Parameters.AddWithValue("@Interest", "0.00"); //Amount of Interest
                        cmd.Parameters.AddWithValue("@Principal", "0.00"); //Amount of Principal
                        cmd.Parameters.AddWithValue("@Deferred", "0.00"); //Deferred Amount (Principal + Interest)
                        cmd.Parameters.AddWithValue("@TotalDueAmount", "100.00"); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                        cmd.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void generateShareCapital(DateTimePicker dtBillDate, string Company, string rank)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Billing WHERE Deduction_Code = 'SHARE CAPITAL' and BillDate ='" + dtBillDate.Value.ToShortDateString() + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return;
                }
                else
                {
                    adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE Date_Resigned_From_Pecci is null and Company_Code = '" + Company + "' and Payroll_Code = '" + rank + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);


                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        //MOVE TO BILLING TABLE
                        cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "sp_InsertBilling";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                        cmd.Parameters.AddWithValue("@EmployeeID", ds.Tables[0].Rows[x]["EmployeeID"].ToString());
                        cmd.Parameters.AddWithValue("@PrincipalID", ds.Tables[0].Rows[x]["PrincipalID"].ToString());
                        cmd.Parameters.AddWithValue("@isPrincipal", ds.Tables[0].Rows[x]["Principal"].ToString());
                        cmd.Parameters.AddWithValue("@Company_Code", ds.Tables[0].Rows[x]["Company_Code"].ToString());
                        cmd.Parameters.AddWithValue("@Payroll_Code", ds.Tables[0].Rows[x]["Payroll_Code"].ToString());
                        cmd.Parameters.AddWithValue("@Loan_No", "");
                        cmd.Parameters.AddWithValue("@Deduction_Code", "SHARE CAPITAL");
                        cmd.Parameters.AddWithValue("@Priority", "2");
                        cmd.Parameters.AddWithValue("@Interest", "0.00"); //Amount of Interest
                        cmd.Parameters.AddWithValue("@Principal", "0.00"); //Amount of Principal
                        cmd.Parameters.AddWithValue("@Deferred", "0.00"); //Deferred Amount (Principal + Interest)
                        cmd.Parameters.AddWithValue("@TotalDueAmount", ds.Tables[0].Rows[x]["share_capital"].ToString()); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                        cmd.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                        cmd.ExecuteNonQuery();

                    }
                }
            }
        }

        public void generateSavings(DateTimePicker dtBillDate, string Company, string rank)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Billing WHERE Deduction_Code = 'SAVINGS DEPOSIT' and BillDate ='" + dtBillDate.Value.ToShortDateString() + "' and Company_Code = '"+ Company +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return;
                }
                else
                {
                    adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE Date_Resigned_From_Pecci is null and Company_Code = '" + Company + "' and Payroll_Code = '" + rank + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);


                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        //MOVE TO BILLING TABLE
                        cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "sp_InsertBilling";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                        cmd.Parameters.AddWithValue("@EmployeeID", ds.Tables[0].Rows[x]["EmployeeID"].ToString());
                        cmd.Parameters.AddWithValue("@PrincipalID", ds.Tables[0].Rows[x]["PrincipalID"].ToString());
                        cmd.Parameters.AddWithValue("@isPrincipal", ds.Tables[0].Rows[x]["Principal"].ToString());
                        cmd.Parameters.AddWithValue("@Company_Code", ds.Tables[0].Rows[x]["Company_Code"].ToString());
                        cmd.Parameters.AddWithValue("@Payroll_Code", ds.Tables[0].Rows[x]["Payroll_Code"].ToString());
                        cmd.Parameters.AddWithValue("@Loan_No", "");
                        cmd.Parameters.AddWithValue("@Deduction_Code", "SAVINGS DEPOSIT");
                        cmd.Parameters.AddWithValue("@Priority", savingsPriority());
                        cmd.Parameters.AddWithValue("@Interest", "0.00"); //Amount of Interest
                        cmd.Parameters.AddWithValue("@Principal", "0.00"); //Amount of Principal
                        cmd.Parameters.AddWithValue("@Deferred", "0.00"); //Deferred Amount (Principal + Interest)
                        cmd.Parameters.AddWithValue("@TotalDueAmount", ds.Tables[0].Rows[x]["Savings_Deposit"].ToString()); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                        cmd.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                        cmd.ExecuteNonQuery();

                    }
                }
            }
        }

        public int savingsPriority()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT val FROM Parameter WHERE frm ='Billing' and Description = 'Savings Deposit'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString());
            }
        }


        #region SMART BILLING
        public void generateMembershipFeeSMART(DateTimePicker dtBillDate, string Company, string rank)
        {
            //Check if already has data in table
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Billing WHERE Deduction_Code = 'MEMBERSHIP FEE' and BillDate ='" + dtBillDate.Value.ToShortDateString() + "' and Company_Code = '"+ Company +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //Already generated
                    return;
                }
                else
                {
                    adapter = new SqlDataAdapter("SELECT * FROM membersFee WHERE isDone ='0' and Company_Code = '" + Company + "' and Payroll_Code = '" + rank + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {

                        //CHECK FIRST IF HAS BILLINGNO FROM SMARTBILLING TABLE
                        if (checkHasBillingNo(ds.Tables[0].Rows[x]["EmployeeID"].ToString()) == true)
                        {
                            //MOVE TO BILLING TABLE
                            cmd = new SqlCommand();
                            cmd.Connection = con;
                            cmd.CommandText = "sp_InsertBilling";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                            cmd.Parameters.AddWithValue("@EmployeeID", ds.Tables[0].Rows[x]["EmployeeID"].ToString());
                            cmd.Parameters.AddWithValue("@PrincipalID", getPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                            cmd.Parameters.AddWithValue("@isPrincipal", isPrincipal(Convert.ToInt32(ds.Tables[0].Rows[x]["userid"].ToString())));
                            cmd.Parameters.AddWithValue("@Company_Code", ds.Tables[0].Rows[x]["Company_Code"].ToString());
                            cmd.Parameters.AddWithValue("@Payroll_Code", ds.Tables[0].Rows[x]["Payroll_Code"].ToString());
                            cmd.Parameters.AddWithValue("@Loan_No", "");
                            cmd.Parameters.AddWithValue("@Deduction_Code", "MEMBERSHIP FEE");
                            cmd.Parameters.AddWithValue("@Priority", "1");
                            cmd.Parameters.AddWithValue("@Interest", "0.00"); //Amount of Interest
                            cmd.Parameters.AddWithValue("@Principal", "0.00"); //Amount of Principal
                            cmd.Parameters.AddWithValue("@Deferred", "0.00"); //Deferred Amount (Principal + Interest)
                            cmd.Parameters.AddWithValue("@TotalDueAmount", "100.00"); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                            cmd.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public void generateShareCapitalSMART(DateTimePicker dtBillDate, string Company, string rank)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Billing WHERE Deduction_Code = 'SHARE CAPITAL' and BillDate ='" + dtBillDate.Value.ToShortDateString() + "' and Company_Code ='"+ Company +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return;
                }
                else
                {
                    adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE Date_Resigned_From_Pecci is null and Company_Code = '" + Company + "' and Payroll_Code = '" + rank + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);


                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {

                        //CHECK FIRST IF HAS BILLINGNO FROM SMARTBILLING TABLE
                        if (checkHasBillingNo(ds.Tables[0].Rows[x]["EmployeeID"].ToString()) == true)
                        {
                            //MOVE TO BILLING TABLE
                            cmd = new SqlCommand();
                            cmd.Connection = con;
                            cmd.CommandText = "sp_InsertBilling";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                            cmd.Parameters.AddWithValue("@EmployeeID", ds.Tables[0].Rows[x]["EmployeeID"].ToString());
                            cmd.Parameters.AddWithValue("@PrincipalID", ds.Tables[0].Rows[x]["PrincipalID"].ToString());
                            cmd.Parameters.AddWithValue("@isPrincipal", ds.Tables[0].Rows[x]["Principal"].ToString());
                            cmd.Parameters.AddWithValue("@Company_Code", ds.Tables[0].Rows[x]["Company_Code"].ToString());
                            cmd.Parameters.AddWithValue("@Payroll_Code", ds.Tables[0].Rows[x]["Payroll_Code"].ToString());
                            cmd.Parameters.AddWithValue("@Loan_No", "");
                            cmd.Parameters.AddWithValue("@Deduction_Code", "SHARE CAPITAL");
                            cmd.Parameters.AddWithValue("@Priority", "2");
                            cmd.Parameters.AddWithValue("@Interest", "0.00"); //Amount of Interest
                            cmd.Parameters.AddWithValue("@Principal", "0.00"); //Amount of Principal
                            cmd.Parameters.AddWithValue("@Deferred", "0.00"); //Deferred Amount (Principal + Interest)
                            cmd.Parameters.AddWithValue("@TotalDueAmount", ds.Tables[0].Rows[x]["share_capital"].ToString()); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                            cmd.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public void generateSavingsSMART(DateTimePicker dtBillDate, string Company, string rank)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Billing WHERE Deduction_Code = 'SAVINGS DEPOSIT' and BillDate ='" + dtBillDate.Value.ToShortDateString() + "' and Company_Code = '"+ Company +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return;
                }
                else
                {
                    adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE Date_Resigned_From_Pecci is null and Company_Code = '" + Company + "' and Payroll_Code = '" + rank + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);


                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        
                        //CHECK FIRST IF HAS BILLINGNO FROM SMARTBILLING TABLE
                        if (checkHasBillingNo(ds.Tables[0].Rows[x]["EmployeeID"].ToString()) == true)
                        {
                            //MOVE TO BILLING TABLE
                            cmd = new SqlCommand();
                            cmd.Connection = con;
                            cmd.CommandText = "sp_InsertBilling";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@userid", ds.Tables[0].Rows[x]["userid"].ToString());
                            cmd.Parameters.AddWithValue("@EmployeeID", ds.Tables[0].Rows[x]["EmployeeID"].ToString());
                            cmd.Parameters.AddWithValue("@PrincipalID", ds.Tables[0].Rows[x]["PrincipalID"].ToString());
                            cmd.Parameters.AddWithValue("@isPrincipal", ds.Tables[0].Rows[x]["Principal"].ToString());
                            cmd.Parameters.AddWithValue("@Company_Code", ds.Tables[0].Rows[x]["Company_Code"].ToString());
                            cmd.Parameters.AddWithValue("@Payroll_Code", ds.Tables[0].Rows[x]["Payroll_Code"].ToString());
                            cmd.Parameters.AddWithValue("@Loan_No", "");
                            cmd.Parameters.AddWithValue("@Deduction_Code", "SAVINGS DEPOSIT");
                            cmd.Parameters.AddWithValue("@Priority", savingsPriority());
                            cmd.Parameters.AddWithValue("@Interest", "0.00"); //Amount of Interest
                            cmd.Parameters.AddWithValue("@Principal", "0.00"); //Amount of Principal
                            cmd.Parameters.AddWithValue("@Deferred", "0.00"); //Deferred Amount (Principal + Interest)
                            cmd.Parameters.AddWithValue("@TotalDueAmount", ds.Tables[0].Rows[x]["Savings_Deposit"].ToString()); //Sum of all interest + principal amount if current // if deferred get the deferred amout only
                            cmd.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public bool checkHasBillingNo(string employeeID)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM SmartBilling WHERE EmployeeID = '"+ employeeID+"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion
    }
}
