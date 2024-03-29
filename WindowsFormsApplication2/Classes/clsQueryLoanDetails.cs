﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace WindowsFormsApplication2.Classes
{
    class clsQueryLoanDetails
    {

        Global global = new Global();
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataSet ds;
        /*
        *  FOR LOAN DETAILS
        */
        string refNo;
        string dtDate;
        string tempBalance;

        public void loadTransactionCodes(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT SUBSTRING(Transaction_Code, 5, 3) as tranCode,Description FROM Transaction_Type WHERE Transaction_Code <> '' or Transaction_Code <> NULL", con);
                ds = new DataSet();
                adapter.Fill(ds);

                dgv.Rows.Clear();

                for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                {
                    dgv.Rows.Add(ds.Tables[0].Rows[x]["tranCode"].ToString(), ds.Tables[0].Rows[x]["Description"].ToString());
                }

            }
        }

        //Return Loan Status
        public string returnLoanStatus(string stat)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT status_description FROM Status_Loan WHERE id = '" + stat.ToString() + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }
        
        //Display Deduction Dates
        public void displayDates(string loan_no,Label lblFirstDeduction,Label lblSecondDeduction,Label lblThirdDeduction)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT TOP 3 Schedule_Payment FROM Loan_Details WHERE Loan_No = '"+ loan_no +"' Order By PaymentNoSemi", con);
                dt = new DataTable();
                adapter.Fill(dt);

                lblFirstDeduction.Text = Convert.ToDateTime(dt.Rows[0].ItemArray[0].ToString()).ToString("MM/dd/yyyy");
                lblSecondDeduction.Text = Convert.ToDateTime(dt.Rows[1].ItemArray[0].ToString()).ToString("MM/dd/yyyy");
                lblThirdDeduction.Text = Convert.ToDateTime(dt.Rows[2].ItemArray[0].ToString()).ToString("MM/dd/yyyy");
            }
        }

        //Dispaly Co Makers
        public void loadComakers(string loan_no,DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Name +' - ('+ EmployeeID +')' FROM vw_CoMakers WHERE Loan_No = '"+ loan_no +"'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                dgv.Rows.Clear();

                for(int c = 0; c < dt.Rows.Count; c++)
                {
                    dgv.Rows.Add(dt.Rows[c].ItemArray[0].ToString());
                }
            }
        }

        //Display Other Deductions
        public void loadOtherDeductions(string loan_no,DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT (Account_Description+'   '+Loan_Type_Loan_No) as Account_Description,Applied_Amount FROM vw_LoanDeductionQuery WHERE Loan_No = '" + loan_no + "' and Other_Deduction <> '314' and Other_Deduction <> '401.1'", con);
                ds = new DataSet();
                adapter.Fill(ds);

                dgv.Rows.Clear();

                for(int o = 0; o < ds.Tables[0].Rows.Count; o++)
                {
                    dgv.Rows.Add(ds.Tables[0].Rows[o]["Account_Description"].ToString(), Convert.ToDecimal(ds.Tables[0].Rows[o]["Applied_Amount"].ToString()).ToString("#,0.00"));
                }
            }
        }

        //Subsidiary Transaction
        public void loadSubsTransactiob(string loan_no, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanDetailsToQuery";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@loan_no", loan_no);

                adapter = new SqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds);

                dgv.Rows.Clear();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    decimal balance = 0;
                    //Load to datagridview

                    string presentRef;
                    string presentDate;
                    string deb, cred;

                    dtDate = "";
                    refNo = "";
                    

                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        if(ds.Tables[0].Rows[x]["Account_Code"].ToString() != "401.1" && ds.Tables[0].Rows[x]["Account_Code"].ToString() != "314")
                        {
                            balance = balance + Convert.ToDecimal(ds.Tables[0].Rows[x]["Debit"].ToString()) - Convert.ToDecimal(ds.Tables[0].Rows[x]["Credit"].ToString());
                        }

                        presentRef = ds.Tables[0].Rows[x]["Reference"].ToString();
                        presentDate = Convert.ToDateTime(ds.Tables[0].Rows[x]["Date"].ToString()).ToShortDateString();
                        deb = ds.Tables[0].Rows[x]["Debit"].ToString();
                        cred = ds.Tables[0].Rows[x]["Credit"].ToString();
                        //Date
                        if (dtDate != presentDate)
                        {
                            presentDate = Convert.ToDateTime(ds.Tables[0].Rows[x]["Date"].ToString()).ToShortDateString();
                        }
                        else
                        {
                            presentDate = "";
                        }

                        //Reference
                        if (refNo != presentRef)
                        {
                            presentRef = ds.Tables[0].Rows[x]["Reference"].ToString();
                        }
                        else
                        {
                            presentRef = "";
                        }

                        //Debit & Credit
                        if(deb == "0.00")
                        {
                            deb = "";
                        }
                        else
                        {
                            deb = Convert.ToDecimal(deb.ToString()).ToString("#,0.00");
                        }

                        if (cred == "0.00")
                        {
                            cred = "";
                        }
                        else
                        {
                            cred = Convert.ToDecimal(cred.ToString()).ToString("#,0.00");
                        }


                        //Balance
                        if (x != ds.Tables[0].Rows.Count-1)
                        {
                            if(ds.Tables[0].Rows[x]["Reference"].ToString() == ds.Tables[0].Rows[x+1]["Reference"].ToString())
                            {
                                tempBalance = "";
                            }
                            else
                            {
                                tempBalance = balance.ToString("#,0.00");
                            }
                        }
                        else
                        {
                            //Last ROW
                            tempBalance = balance.ToString("#,0.00");
                        }

                        
                        dgv.Rows.Add(presentDate,
                            presentRef,
                            ds.Tables[0].Rows[x]["Transaction_Type"].ToString(),
                            ds.Tables[0].Rows[x]["Account_Description"].ToString(),
                            deb,
                            cred,
                            tempBalance);

                        dtDate = Convert.ToDateTime(ds.Tables[0].Rows[x]["Date"].ToString()).ToShortDateString();
                        refNo = ds.Tables[0].Rows[x]["Reference"].ToString();
                    }
                }
            }
        }

        //Load PastDue if he has
        public void loadSubsTransactioPastDue(string loan_no, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanDetailsToQueryPastDue";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@loan_no", loan_no);

                adapter = new SqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds);

                dgv.Rows.Clear();

                if(ds.Tables[0].Rows.Count > 0)
                {
                    decimal balance = 0;
                    string deb, cred;
                    

                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        deb = ds.Tables[0].Rows[x]["Debit"].ToString();
                        cred = ds.Tables[0].Rows[x]["Credit"].ToString();

                        balance = balance + Convert.ToDecimal(ds.Tables[0].Rows[x]["Debit"].ToString()) - Convert.ToDecimal(ds.Tables[0].Rows[x]["Credit"].ToString());

                        //Debit & Credit
                        if (deb == "0.00")
                        {
                            deb = "";
                        }
                        else
                        {
                            deb = Convert.ToDecimal(deb.ToString()).ToString("#,0.00");
                        }

                        if (cred == "0.00")
                        {
                            cred = "";
                        }
                        else
                        {
                            cred = Convert.ToDecimal(cred.ToString()).ToString("#,0.00");
                        }

                        dgv.Rows.Add(Convert.ToDateTime(ds.Tables[0].Rows[x]["Date"].ToString()).ToShortDateString(),
                        ds.Tables[0].Rows[x]["Reference"].ToString(),
                        ds.Tables[0].Rows[x]["Transaction_Type"].ToString(),
                        ds.Tables[0].Rows[x]["Account_Description"].ToString(),
                        deb,
                        cred,
                        balance.ToString("#,0.00"));
                    }
                }
            }
        }

        //Display Selected Loan
        public void displayLoanDetails(string loan_no,Label l_loanNo,Label l_loanType,Label l_loanDate,Label l_ReleasedDate,Label l_LoanStatus,Label l_GrossAmount,Label l_ServiceFee,Label l_PrevBalance,Label l_NetProceed,Label l_Terms,Label l_MonthylAmort,Label l_FirstDeductionPayment,Label l_SecondDeductionPayment,Label l_ThirdDeductionPayment,Label l_SuccedingPayment,Label l_firstDate,Label l_secondDate,Label l_thirdDate,DataGridView dgvCoMakers,DataGridView dgvOtherDeduction,DataGridView dgvSubs)
        {
            if(loan_no != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_ReturnLoanBalancesQuerySelected";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", Classes.clsQuery.searchUserID);
                    cmd.Parameters.AddWithValue("@loan_no", loan_no);

                    adapter = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    adapter.Fill(ds);

                    if(ds.Tables[0].Rows.Count > 0)
                    {
                        l_loanNo.Text = ds.Tables[0].Rows[0]["Loan_No"].ToString();
                        l_loanType.Text = ds.Tables[0].Rows[0]["Loan_Type"].ToString();
                        l_loanDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["Loan_Date"].ToString()).ToString("MM/dd/yyyy");
                        l_ReleasedDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["ReleaseDate"].ToString()).ToString("MM/dd/yyyy");
                        l_LoanStatus.Text = returnLoanStatus(ds.Tables[0].Rows[0]["Status"].ToString()); //Convert to text
                        l_GrossAmount.Text = Convert.ToDecimal(ds.Tables[0].Rows[0]["Loan_Amount"].ToString()).ToString("#,0.00");
                        l_ServiceFee.Text = Convert.ToDecimal(ds.Tables[0].Rows[0]["Service_Fee"].ToString()).ToString("#,0.00");
                        if(Convert.ToString(ds.Tables[0].Rows[0]["Outstanding_Balance"].ToString()) != "")
                        {
                            l_PrevBalance.Text = Convert.ToDecimal(ds.Tables[0].Rows[0]["Outstanding_Balance"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            l_PrevBalance.Text = "";
                        }
                        l_NetProceed.Text = Convert.ToDecimal(ds.Tables[0].Rows[0]["NetProceeds"].ToString()).ToString("#,0.00");
                        l_Terms.Text = ds.Tables[0].Rows[0]["Terms"].ToString();
                        l_MonthylAmort.Text = Convert.ToDecimal(ds.Tables[0].Rows[0]["Monthly_Amort"].ToString()).ToString("#,0.00");


                        //GETTING THE PAYMENT

                        SqlDataAdapter adapterPayment = new SqlDataAdapter("SELECT top 1 Payment FROM Loan_Details WHERE loan_No = '"+loan_no+"'", con);
                        DataTable dt = new DataTable();
                        adapterPayment.Fill(dt);

                        //Payment
                        l_FirstDeductionPayment.Text = Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString()).ToString("#,0.00");
                        l_SecondDeductionPayment.Text = l_FirstDeductionPayment.Text;
                        l_ThirdDeductionPayment.Text = l_FirstDeductionPayment.Text;
                        l_SuccedingPayment.Text = l_FirstDeductionPayment.Text;

                        displayDates(ds.Tables[0].Rows[0]["Loan_No"].ToString(), l_firstDate, l_secondDate, l_thirdDate);

                        //Display Co Makers
                        loadComakers(ds.Tables[0].Rows[0]["Loan_No"].ToString(), dgvCoMakers);

                        //Display Other Deduction
                        loadOtherDeductions(ds.Tables[0].Rows[0]["Loan_No"].ToString(), dgvOtherDeduction);
                       
                    }
                }
            }
        }
    }
}
