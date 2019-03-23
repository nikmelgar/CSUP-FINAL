using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

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
                dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;

                //Reconsturct loans with no loan details
                //
                //Came from inserting only the loan header and the outstanding balance from journal or other means of entry

                int noCnt;
                int val = 0;
                double interest, principal, fnalInterest;

                //Checking if theres a loan details
                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    SqlDataAdapter adapterCheckDetails = new SqlDataAdapter("SELECT top 1 * from Loan_Details where loan_no = '" + dt.Rows[x].ItemArray[0].ToString() + "' and isDone = '0' Order by PaymentNoSemi", con);
                    DataTable dtCheckDetails = new DataTable();
                    adapterCheckDetails.Fill(dtCheckDetails);

                    if (dtCheckDetails.Rows.Count > 0)
                    {
                        //Proceed [RETURN TRUE]
                    }
                    else
                    {
                        //GET LAST COUNT OF PAYMENT 
                        SqlDataAdapter adapterCount = new SqlDataAdapter("SELECT TOP 1 PaymentNoSemi FROM Loan_Details where loan_no = '" + dt.Rows[x].ItemArray[0].ToString() + "' and isDone = 1 Order by PaymentNoSemi", con);
                        DataTable dtCount = new DataTable();
                        adapterCount.Fill(dtCount);

                        if (dtCount.Rows.Count > 0)
                        {
                            noCnt = Convert.ToInt32(dtCount.Rows[0].ItemArray[0].ToString()) + 1;
                        }
                        else
                        {
                            noCnt = 1;
                        }
                        //39 = balance 9 = interest
                        interest = Convert.ToDouble(dt.Rows[x].ItemArray[39].ToString()) * Convert.ToDouble(dt.Rows[x].ItemArray[9].ToString());
                        fnalInterest = interest / 2;

                        principal = Convert.ToDouble(dt.Rows[x].ItemArray[8].ToString()) - fnalInterest;

                        //re-compute and insert two records for 1 month
                        for (int i = 0; i < 2; i++)
                        {
                            SqlCommand cmdInsert = new SqlCommand();
                            cmdInsert.Connection = con;
                            cmdInsert.CommandText = "sp_InsertLoanDetailsReconstruct";
                            cmdInsert.CommandType = CommandType.StoredProcedure;
                            cmdInsert.Parameters.AddWithValue("@Loan_No", dt.Rows[x].ItemArray[0].ToString());
                            cmdInsert.Parameters.AddWithValue("@PaymentNoSemi", noCnt);
                            cmdInsert.Parameters.AddWithValue("@Payment", dt.Rows[x].ItemArray[8].ToString()); //Semi Monthly
                            cmdInsert.Parameters.AddWithValue("@Interest", Convert.ToString(decimal.Round(Convert.ToDecimal(fnalInterest), 2)));
                            cmdInsert.Parameters.AddWithValue("@Principal", Convert.ToString(decimal.Round(Convert.ToDecimal(principal), 2)));
                            cmdInsert.Parameters.AddWithValue("@isDOne", false);
                            cmdInsert.ExecuteNonQuery();

                            noCnt = noCnt + 1;
                        }
                    }


                    //===================================================================
                    //  After Creating Loan Details of Loans
                    //  Then Select top 1 of all loan details 
                    //  if PaymentNo is ODD then Recompute Interest 
                    //  and Insert two Records
                    //===================================================================
                    //IF ODD VALUES RECOMPUTE FOR INTEREST
                    //[ MEANING THIS ODD NUMBER IS HIS/HER NEW MONTH  ]

                    //RETURN PRIORTY NUMBER
                    SqlDataAdapter adapterPriority = new SqlDataAdapter("SELECT [Priority] FROM LOan_Type WHERE Loan_Type = '" + dt.Rows[x].ItemArray[1].ToString() + "'", con);
                    DataTable dtPriority = new DataTable();
                    adapterPriority.Fill(dtPriority);


                    val = 0;
                    val = Convert.ToInt32(dtCheckDetails.Rows[0].ItemArray[2].ToString());
                    if (val % 2 == 0)
                    {
                        //EVEN
                        //2nd COllection
                        //MOVE AS EASE
                        SqlCommand cmdMove = new SqlCommand();
                        cmdMove.Connection = con;
                        cmdMove.CommandText = "sp_InsertBilling";
                        cmdMove.CommandType = CommandType.StoredProcedure;
                        cmdMove.Parameters.AddWithValue("@userid", dt.Rows[x].ItemArray[10].ToString());
                        cmdMove.Parameters.AddWithValue("@EmployeeID", dt.Rows[x].ItemArray[11].ToString());
                        cmdMove.Parameters.AddWithValue("@Company_Code", dt.Rows[x].ItemArray[35].ToString());
                        cmdMove.Parameters.AddWithValue("@Payroll_Code", dt.Rows[x].ItemArray[34].ToString());
                        cmdMove.Parameters.AddWithValue("@Loan_No", dt.Rows[x].ItemArray[0].ToString());
                        cmdMove.Parameters.AddWithValue("@Deduction_Code", "LOAN");
                        cmdMove.Parameters.AddWithValue("@Priority", dtPriority.Rows[0].ItemArray[0].ToString());
                        cmdMove.Parameters.AddWithValue("@Interest", Convert.ToDecimal(dtCheckDetails.Rows[0].ItemArray[4].ToString())); //THIS WILL GET FROM LOAN DETAILS
                        cmdMove.Parameters.AddWithValue("@Principal", Convert.ToDecimal(dtCheckDetails.Rows[0].ItemArray[5].ToString())); //THIS WILL GET FROM LOAN DETAILS
                        cmdMove.Parameters.AddWithValue("@Deferred", "0.00"); //THIS WILL GET FROM LOAN DETAILS
                        cmdMove.Parameters.AddWithValue("@TotalDueAmount", Convert.ToDecimal(dtCheckDetails.Rows[0].ItemArray[3].ToString())); //THIS WILL GET FROM LOAN DETAILS
                        cmdMove.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                        cmdMove.ExecuteNonQuery();

                        //Check if has a deferred then if true re insert for Deferred Deduction not more than 50k

                        if (dt.Rows[x].ItemArray[40].ToString() != "0.00")
                        {
                            SqlCommand cmdMoveDeferred = new SqlCommand();
                            cmdMoveDeferred.Connection = con;
                            cmdMoveDeferred.CommandText = "sp_InsertBilling";
                            cmdMoveDeferred.CommandType = CommandType.StoredProcedure;
                            cmdMoveDeferred.Parameters.AddWithValue("@userid", dt.Rows[x].ItemArray[10].ToString());
                            cmdMoveDeferred.Parameters.AddWithValue("@EmployeeID", dt.Rows[x].ItemArray[11].ToString());
                            cmdMoveDeferred.Parameters.AddWithValue("@Company_Code", dt.Rows[x].ItemArray[35].ToString());
                            cmdMoveDeferred.Parameters.AddWithValue("@Payroll_Code", dt.Rows[x].ItemArray[34].ToString());
                            cmdMoveDeferred.Parameters.AddWithValue("@Loan_No", dt.Rows[x].ItemArray[0].ToString());
                            cmdMoveDeferred.Parameters.AddWithValue("@Deduction_Code", "DEFERRED");
                            cmdMoveDeferred.Parameters.AddWithValue("@Priority", dtPriority.Rows[0].ItemArray[0].ToString());
                            cmdMoveDeferred.Parameters.AddWithValue("@Interest", "0.00"); //THIS WILL GET FROM LOAN DETAILS
                            cmdMoveDeferred.Parameters.AddWithValue("@Principal", "0.00"); //THIS WILL GET FROM LOAN DETAILS
                            if (Convert.ToDecimal(dt.Rows[x].ItemArray[40].ToString()) > clsParameter.DeferredAmount())
                            {
                                //Greater than deferred value in parameter
                                cmdMoveDeferred.Parameters.AddWithValue("@Deferred", clsParameter.DeferredAmount()); //THIS WILL GET FROM LOAN DETAILS
                                cmdMoveDeferred.Parameters.AddWithValue("@TotalDueAmount", clsParameter.DeferredAmount()); //THIS WILL GET FROM LOAN DETAILS
                            }
                            else
                            {
                                cmdMoveDeferred.Parameters.AddWithValue("@Deferred", dt.Rows[x].ItemArray[40].ToString()); //THIS WILL GET FROM LOAN DETAILS
                                cmdMoveDeferred.Parameters.AddWithValue("@TotalDueAmount", dt.Rows[x].ItemArray[40].ToString()); //THIS WILL GET FROM LOAN DETAILS
                            }
                            cmdMoveDeferred.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                            cmdMoveDeferred.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        //ODD
                        //FIrst Collection for the Month 
                        //RECOMPUTE FIRST THEN 

                        //GET FIRST THE OUTSTANDING BALANCE

                        //39 = balance 9 = interest
                        interest = Convert.ToDouble(dt.Rows[x].ItemArray[39].ToString()) * Convert.ToDouble(dt.Rows[x].ItemArray[9].ToString());
                        fnalInterest = interest / 2;

                        principal = Convert.ToDouble(dt.Rows[x].ItemArray[8].ToString()) - fnalInterest;

                        //UPDATE THE 2 RECORDS
                        //CRITERIA = PAYMENTNOSEMI + 1

                        for (int upd = 0; upd < 2; upd++)
                        {
                            SqlCommand cmdUpdate = new SqlCommand();
                            cmdUpdate.Connection = con;
                            cmdUpdate.CommandText = "sp_UpdateLoanDetailsReconstruct";
                            cmdUpdate.CommandType = CommandType.StoredProcedure;
                            cmdUpdate.Parameters.AddWithValue("@Loan_No", dt.Rows[x].ItemArray[0].ToString());
                            cmdUpdate.Parameters.AddWithValue("@PaymentNoSemi", val);
                            cmdUpdate.Parameters.AddWithValue("@Interest", Convert.ToString(decimal.Round(Convert.ToDecimal(fnalInterest), 2)));
                            cmdUpdate.Parameters.AddWithValue("@Principal", Convert.ToString(decimal.Round(Convert.ToDecimal(principal), 2)));
                            cmdUpdate.ExecuteNonQuery();

                            val = val + 1;
                        }

                        //MOVE TO BILLING TABLE
                        SqlCommand cmdMove = new SqlCommand();
                        cmdMove.Connection = con;
                        cmdMove.CommandText = "sp_InsertBilling";
                        cmdMove.CommandType = CommandType.StoredProcedure;
                        cmdMove.Parameters.AddWithValue("@userid", dt.Rows[x].ItemArray[10].ToString());
                        cmdMove.Parameters.AddWithValue("@EmployeeID", dt.Rows[x].ItemArray[11].ToString());
                        cmdMove.Parameters.AddWithValue("@Company_Code", dt.Rows[x].ItemArray[35].ToString());
                        cmdMove.Parameters.AddWithValue("@Payroll_Code", dt.Rows[x].ItemArray[34].ToString());
                        cmdMove.Parameters.AddWithValue("@Loan_No", dt.Rows[x].ItemArray[0].ToString());
                        cmdMove.Parameters.AddWithValue("@Deduction_Code", "LOAN");
                        cmdMove.Parameters.AddWithValue("@Priority", dtPriority.Rows[0].ItemArray[0].ToString());
                        cmdMove.Parameters.AddWithValue("@Interest", Convert.ToDecimal(dtCheckDetails.Rows[0].ItemArray[4].ToString())); //THIS WILL GET FROM LOAN DETAILS
                        cmdMove.Parameters.AddWithValue("@Principal", Convert.ToDecimal(dtCheckDetails.Rows[0].ItemArray[5].ToString())); //THIS WILL GET FROM LOAN DETAILS
                        cmdMove.Parameters.AddWithValue("@Deferred", "0.00"); //THIS WILL GET FROM LOAN DETAILS
                        cmdMove.Parameters.AddWithValue("@TotalDueAmount", Convert.ToDecimal(dtCheckDetails.Rows[0].ItemArray[3].ToString())); //THIS WILL GET FROM LOAN DETAILS
                        cmdMove.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                        cmdMove.ExecuteNonQuery();

                        //Check if has a deferred then if true re insert for Deferred Deduction not more than 50k

                        if (dt.Rows[x].ItemArray[40].ToString() != "0.00")
                        {
                            SqlCommand cmdMoveDeferred = new SqlCommand();
                            cmdMoveDeferred.Connection = con;
                            cmdMoveDeferred.CommandText = "sp_InsertBilling";
                            cmdMoveDeferred.CommandType = CommandType.StoredProcedure;
                            cmdMoveDeferred.Parameters.AddWithValue("@userid", dt.Rows[x].ItemArray[10].ToString());
                            cmdMoveDeferred.Parameters.AddWithValue("@EmployeeID", dt.Rows[x].ItemArray[11].ToString());
                            cmdMoveDeferred.Parameters.AddWithValue("@Company_Code", dt.Rows[x].ItemArray[35].ToString());
                            cmdMoveDeferred.Parameters.AddWithValue("@Payroll_Code", dt.Rows[x].ItemArray[34].ToString());
                            cmdMoveDeferred.Parameters.AddWithValue("@Loan_No", dt.Rows[x].ItemArray[0].ToString());
                            cmdMoveDeferred.Parameters.AddWithValue("@Deduction_Code", "DEFERRED");
                            cmdMoveDeferred.Parameters.AddWithValue("@Priority", dtPriority.Rows[0].ItemArray[0].ToString());
                            cmdMoveDeferred.Parameters.AddWithValue("@Interest", "0.00"); //THIS WILL GET FROM LOAN DETAILS
                            cmdMoveDeferred.Parameters.AddWithValue("@Principal", "0.00"); //THIS WILL GET FROM LOAN DETAILS
                            if (Convert.ToDecimal(dt.Rows[x].ItemArray[40].ToString()) > clsParameter.DeferredAmount())
                            {
                                //Greater than deferred value in parameter
                                cmdMoveDeferred.Parameters.AddWithValue("@Deferred", clsParameter.DeferredAmount()); //THIS WILL GET FROM LOAN DETAILS
                                cmdMoveDeferred.Parameters.AddWithValue("@TotalDueAmount", clsParameter.DeferredAmount()); //THIS WILL GET FROM LOAN DETAILS
                            }
                            else
                            {
                                cmdMoveDeferred.Parameters.AddWithValue("@Deferred", dt.Rows[x].ItemArray[40].ToString()); //THIS WILL GET FROM LOAN DETAILS
                                cmdMoveDeferred.Parameters.AddWithValue("@TotalDueAmount", dt.Rows[x].ItemArray[40].ToString()); //THIS WILL GET FROM LOAN DETAILS
                            }

                            cmdMoveDeferred.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                            cmdMoveDeferred.ExecuteNonQuery();
                        }

                    }

                }
            }


            generateMembershipFee(dtBillDate, company, rank);
            generateShareCapital(dtBillDate, company, rank);
        }


        //GENERATE BILLING FOR MEMBERSHIP
        public void generateMembershipFee(DateTimePicker dtBillDate,string Company,string rank)
        {

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM membersFee WHERE isDone ='0' and Company_Code = '" + Company + "' and Payroll_Code = '" + rank + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    //MOVE TO BILLING TABLE
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_InsertBilling";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", dt.Rows[x].ItemArray[0].ToString());
                    cmd.Parameters.AddWithValue("@EmployeeID", dt.Rows[x].ItemArray[1].ToString());
                    cmd.Parameters.AddWithValue("@Company_Code", dt.Rows[x].ItemArray[2].ToString());
                    cmd.Parameters.AddWithValue("@Payroll_Code", dt.Rows[x].ItemArray[3].ToString());
                    cmd.Parameters.AddWithValue("@Loan_No", "");
                    cmd.Parameters.AddWithValue("@Deduction_Code", "MEMBERSHIP FEE");
                    cmd.Parameters.AddWithValue("@Priority", clsParameter.MembershipFeePriority());
                    cmd.Parameters.AddWithValue("@Interest", "0.00");
                    cmd.Parameters.AddWithValue("@Principal", "0.00");
                    cmd.Parameters.AddWithValue("@Deferred", "0.00");
                    cmd.Parameters.AddWithValue("@TotalDueAmount", clsParameter.MembershipFee());
                    cmd.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void generateShareCapital(DateTimePicker dtBillDate, string Company, string rank)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT userid,employeeid,Company_code,payroll_code,Share_Capital FROM Membership WHERE Date_Resigned_From_Pecci is null and Company_Code = '" + Company + "' and Payroll_Code = '" + rank + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);


                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    //MOVE TO BILLING TABLE
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_InsertBilling";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", dt.Rows[x].ItemArray[0].ToString());
                    cmd.Parameters.AddWithValue("@EmployeeID", dt.Rows[x].ItemArray[1].ToString());
                    cmd.Parameters.AddWithValue("@Company_Code", dt.Rows[x].ItemArray[2].ToString());
                    cmd.Parameters.AddWithValue("@Payroll_Code", dt.Rows[x].ItemArray[3].ToString());
                    cmd.Parameters.AddWithValue("@Loan_No", "");
                    cmd.Parameters.AddWithValue("@Deduction_Code", "SHARE CAPITAL");
                    cmd.Parameters.AddWithValue("@Priority", clsParameter.ShareCapitalPriority());
                    cmd.Parameters.AddWithValue("@Interest", "0.00");
                    cmd.Parameters.AddWithValue("@Principal", "0.00");
                    cmd.Parameters.AddWithValue("@Deferred", "0.00");
                    cmd.Parameters.AddWithValue("@TotalDueAmount", dt.Rows[x].ItemArray[4].ToString());
                    cmd.Parameters.AddWithValue("@Billdate", dtBillDate.Text);
                    cmd.ExecuteNonQuery();

                }
            }
        }

    }
}
