using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;


namespace WindowsFormsApplication2.Classes
{
    class clsPDCManagement
    {
        Global global = new Global();
        public static int userid { get; set; }
        public static int id { get; set; }

        public static string LastValueChequeNo { get; set; }
        public static decimal LastValueAmount { get; set; }
        public static string typeFromChecking { get; set; }

        Classes.clsCollection clsCollection = new clsCollection();
        Classes.clsBilling clsBilling = new clsBilling();

        public void loadPDC(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT TOP 50 * FROM vw_PDCManagement ORDER BY ChequeDate,EmpName ASC", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;

                dgv.Columns["DatePrepared"].Visible = false;
                dgv.Columns["userid"].Visible = false;
                dgv.Columns["id"].Visible = false;

                dgv.Columns["isCheck"].HeaderText = "";
                dgv.Columns["ORNumber"].HeaderText = "OR #";
                dgv.Columns["EmployeeID"].HeaderText = "ID No.";
                dgv.Columns["EmpName"].HeaderText = "Name";
                dgv.Columns["ChequeDate"].HeaderText = "Date of Cheque";
                dgv.Columns["isMultiple"].HeaderText = "Multiple transaction";
                dgv.Columns["Bank"].HeaderText = "Bank";
                dgv.Columns["ChequeNo"].HeaderText = "Cheque No";
                dgv.Columns["TotalAmount"].HeaderText = "Amount";

                dgv.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns["TotalAmount"].DefaultCellStyle.Format = "#,0.00";

                dgv.Columns["isCheck"].FillWeight = 20;
                dgv.Columns["ORNumber"].FillWeight = 60;
                dgv.Columns["EmployeeID"].FillWeight = 50;
                dgv.Columns["EmpName"].FillWeight = 220;
                dgv.Columns["Bank"].FillWeight = 50;

                dgv.Columns["isCheck"].ReadOnly = false;
                dgv.Columns["isMultiple"].ReadOnly = true;
                dgv.Columns["ORNumber"].ReadOnly = true;
                dgv.Columns["EmployeeID"].ReadOnly = true;
                dgv.Columns["EmpName"].ReadOnly = true;
                dgv.Columns["ChequeDate"].ReadOnly = true;
                dgv.Columns["Bank"].ReadOnly = true;
                dgv.Columns["ChequeNo"].ReadOnly = true;
            }
        }

        public void loadLoanPerMemberPDC(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanBalancesPerMemberForPDCManagement";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;

                dgv.Columns["Loan_No"].HeaderText = "Loan No";
                dgv.Columns["Loan_Type"].HeaderText = "Type";
                dgv.Columns["Loan_Amount"].HeaderText = "Gross Amount";
                dgv.Columns["Monthly_Amort"].HeaderText = "Monthly";
                dgv.Columns["ReleaseDate"].HeaderText = "Released Date";

                dgv.Columns["Loan_Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns["Monthly_Amort"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns["ReleaseDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns["Loan_Type"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgv.Columns["Loan_Type"].FillWeight = 50;
            }
        }

        //FOR SAME USER ONLY
        public bool CheckChequeNoIfUsed(string chequeNo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM PDCManagement WHERE ChequeNo = '"+ chequeNo +"' and userid ='"+ userid +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    //Already Used                      
                    return true;
                }
                else
                {
                    //Not Use
                    return false;
                }
            }
        }

        //FOR ALL USERS
        public bool CheckChequeNoIfUsedByOthers(string chequeNo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM PDCManagementV2 WHERE ChequeNo = '" + chequeNo + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //Already Used                      
                    return true;
                }
                else
                {
                    //Not Use
                    return false;
                }
            }
        }

        public bool checkCategory(string category,string chequeNo,string loanType)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                string str = "";
                switch (category) {
                    case "Savings":
                        str = "SELECT * FROM PDCManagement WHERE ChequeNo = '" + chequeNo + "' and userid ='" + userid + "' and LoanType = 'SD'";
                        break;
                    case "Share Capital":
                        str = "SELECT * FROM PDCManagement WHERE ChequeNo = '" + chequeNo + "' and userid ='" + userid + "' and LoanType = 'SC'";
                        break;
                    case "Loan":
                        str = "SELECT * FROM PDCManagement WHERE ChequeNo = '" + chequeNo + "' and userid ='" + userid + "' and LoanType = '"+ loanType +"'";
                        break;
                }

                SqlDataAdapter adapter = new SqlDataAdapter(str, con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    //Stop cant be used twice for the same category
                    return true;
                }
                else
                {
                    //He/She can proceed
                    return false;
                }
            }
        }

        //==============================================================================================
        //                      CLASS FOR PDC MANAGEMENT V2
        //==============================================================================================
        public void loadTotals(DataGridView dgv,Label lblTotal)
        {
            double tempAMount = 0;
            //Check if theres a beneficiary
            if (dgv.Rows.Count > 0)
            {
                for (int i = 0; i < dgv.Rows.Count; ++i)
                {
                    tempAMount += Convert.ToDouble(dgv.Rows[i].Cells[3].Value);
                }
            }
            lblTotal.Text = Convert.ToDecimal(tempAMount).ToString("#,0.00");
        }

        public bool checkChequeNoUsedByOthers(string chequeNo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM PDCManagementV2 WHERE ChequeNo = '" + chequeNo + "' and id <> '" + id + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    Alert.show("Cheque number already used.", Alert.AlertType.error);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string returnLoanDescription(string loanType)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Loan_Description FROM Loan_Type WHERE Loan_Type = '"+ loanType+"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }

        public void getMonthlyDetails(DataGridView dgvORGridview, string loan_no,double amountInputed,DataGridView dgvTemp)
        {
            double interest, outstanding, amort,Principal,TotalBalance,remainingBal;
            interest = 0;
            outstanding = 0;
            amort = 0;
            Principal = 0;
            TotalBalance = 0;
            remainingBal = 0;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanBalancesForPDC";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@loan_No", loan_no);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                outstanding = Convert.ToDouble(ds.Tables[0].Rows[0]["Balance"].ToString());
                interest = outstanding * Convert.ToDouble(ds.Tables[0].Rows[0]["Interest"].ToString());
                amort = Convert.ToDouble(ds.Tables[0].Rows[0]["Monthly_Amort"].ToString());
                Principal = amort - interest;
                               

                dgvTemp.Rows.Clear();
                remainingBal = amountInputed;

                if (ds.Tables[0].Rows[0]["Deferred"].ToString() != "0.00")
                {
                    //HAS DEFERRED
                    //MOVE TO TEMP
                    //CHECK IF HAS UNEARNED 
                    if (Convert.ToString(global.getUnearnAmountGLOBAL(ds.Tables[0].Rows[0]["Loan_No"].ToString())) != "0.00")
                    {
                        //MOVE TO TEMP
                        dgvTemp.Rows.Add(
                            "DEFERRED INTEREST",
                            "401.1",
                            "314",
                            global.getUnearnAmountGLOBAL(ds.Tables[0].Rows[0]["Loan_No"].ToString()).ToString("#,0.00"),
                            "0.00",
                            "0.00"
                            );
                    }

                    dgvTemp.Rows.Add(
                        "DEFERRED PRINCIPAL",
                        ds.Tables[0].Rows[0]["PastDueDr"].ToString(),
                        ds.Tables[0].Rows[0]["PastDueDr"].ToString(),
                        clsBilling.getPrincipalDeferred(loan_no).ToString("#,0.00"),
                        "0.00",
                        "0.00"
                        );
                }

                //FOR LOAN CURRENT 
                //MOVE TO TEMP
                dgvTemp.Rows.Add(
                    "LOAN INTEREST",
                    "401.1",
                    "314",
                    Convert.ToDecimal(interest).ToString("#,0.00"),
                    "0.00",
                    "0.00"
                    );
                //PRINCIPAL
                dgvTemp.Rows.Add(
                    "LOAN PRINCIPAL",
                    "401.1",
                    "314",
                    Convert.ToDecimal(Principal).ToString("#,0.00"),
                    "0.00",
                    "0.00"
                    );

                //ALOCATE THE COLLECTION ON PDC

                for(int x=0; x<dgvTemp.Rows.Count; x++)
                {
                    if(remainingBal > 0)
                    {
                        if (remainingBal > Convert.ToDouble(dgvTemp.Rows[x].Cells[3].Value.ToString()))
                        {
                            //cmdDef.Parameters.AddWithValue("@AppliedAmount", ds.Tables[0].Rows[y]["DueAmount"].ToString());
                            //cmdDef.Parameters.AddWithValue("@DeferredAmount", "0.00");

                            dgvTemp.Rows[x].Cells[4].Value = Convert.ToDecimal(dgvTemp.Rows[x].Cells[3].Value.ToString()).ToString("#,0.00");
                            dgvTemp.Rows[x].Cells[5].Value = "0.00";
                        }
                        else
                        {
                            //cmdDef.Parameters.AddWithValue("@AppliedAmount", amnt);
                            //cmdDef.Parameters.AddWithValue("@DeferredAmount", Convert.ToDouble(ds.Tables[0].Rows[y]["DueAmount"].ToString()) - amnt);
                            dgvTemp.Rows[x].Cells[4].Value = Convert.ToDecimal(remainingBal).ToString("#,0.00");
                            dgvTemp.Rows[x].Cells[5].Value = Convert.ToDecimal(Convert.ToDouble(dgvTemp.Rows[x].Cells[3].Value.ToString()) - remainingBal);
                        }

                        remainingBal = remainingBal - Convert.ToDouble(dgvTemp.Rows[x].Cells[3].Value.ToString());
                    }

                    else
                    {
                        dgvTemp.Rows[x].Cells[4].Value = "0.00";
                        dgvTemp.Rows[x].Cells[5].Value = Convert.ToDecimal(dgvTemp.Rows[x].Cells[3].Value.ToString());
                    }
                }


                //FILL TO DATAGRIDVIEW WITH ACCOUNT CODES
                //===========================================================
                //           MOVE TO DATAGRIDVIEW
                //===========================================================
                //COL1 = account description
                //COL2 = subsidiary
                //COL3 = loan no
                //COL4 = debit
                //COL5 = credit
                //COL6 = userid
                
                for(int rowy = 0; rowy < dgvTemp.Rows.Count; rowy++)
                {
                    switch(dgvTemp.Rows[rowy].Cells[0].Value.ToString())
                    {
                        case "DEFERRED INTEREST":
                            if (Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[4].Value.ToString()).ToString("#,0.00") != "0.00")
                            {
                                dgvORGridview.Rows.Add(
                                   "401.1",
                                   clsCollection.GetSubsidiary(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString())),
                                   ds.Tables[0].Rows[0]["Loan_No"].ToString(),
                                   "0.00",
                                   Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[4].Value.ToString()).ToString("#,0.00"),
                                   ds.Tables[0].Rows[0]["userID"].ToString()
                                   );

                                dgvORGridview.Rows.Add(
                                   "314",
                                   clsCollection.GetSubsidiary(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString())),
                                   ds.Tables[0].Rows[0]["Loan_No"].ToString(),
                                   Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[4].Value.ToString()).ToString("#,0.00"),
                                   "0.00",
                                   ds.Tables[0].Rows[0]["userID"].ToString()
                                   );

                                dgvORGridview.Rows.Add(
                                   ds.Tables[0].Rows[0]["PastDueDr"].ToString(),
                                   clsCollection.GetSubsidiary(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString())),
                                   ds.Tables[0].Rows[0]["Loan_No"].ToString(),
                                   "0.00",
                                   Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[4].Value.ToString()).ToString("#,0.00"),
                                   ds.Tables[0].Rows[0]["userID"].ToString()
                                   );
                            }
                            break;
                        case "DEFERRED PRINCIPAL":
                            if (Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[4].Value.ToString()).ToString("#,0.00") != "0.00")
                            {
                                dgvORGridview.Rows.Add(
                                   ds.Tables[0].Rows[0]["PastDueDr"].ToString(),
                                   clsCollection.GetSubsidiary(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString())),
                                   ds.Tables[0].Rows[0]["Loan_No"].ToString(),
                                   "0.00",
                                   Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[4].Value.ToString()).ToString("#,0.00"),
                                   ds.Tables[0].Rows[0]["userID"].ToString()
                                   );
                            }
                            break;
                        case "LOAN INTEREST":
                            if (Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[5].Value.ToString()).ToString("#,0.00") != "0.00")
                            {
                                dgvORGridview.Rows.Add(
                                    "314",
                                    clsCollection.GetSubsidiary(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString())),
                                    ds.Tables[0].Rows[0]["Loan_No"].ToString(),
                                    "0.00",
                                    Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[5].Value.ToString()).ToString("#,0.00"),
                                    ds.Tables[0].Rows[0]["userID"].ToString()
                                    );
                                //PASTDUE
                                dgvORGridview.Rows.Add(
                                    ds.Tables[0].Rows[0]["PastDueDr"].ToString(),
                                    clsCollection.GetSubsidiary(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString())),
                                    ds.Tables[0].Rows[0]["Loan_No"].ToString(),
                                    Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[5].Value.ToString()).ToString("#,0.00"),
                                    "0.00",
                                    ds.Tables[0].Rows[0]["userID"].ToString()
                                    );

                                if (Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[4].Value.ToString()).ToString("#,0.00") != "0.00")
                                {
                                    dgvORGridview.Rows.Add(
                                    "401.1",
                                    clsCollection.GetSubsidiary(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString())),
                                    ds.Tables[0].Rows[0]["Loan_No"].ToString(),
                                    "0.00",
                                    Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[4].Value.ToString()).ToString("#,0.00"),
                                    ds.Tables[0].Rows[0]["userID"].ToString()
                                    );
                                }

                            }
                            else
                            {
                                dgvORGridview.Rows.Add(
                                   "401.1",
                                   clsCollection.GetSubsidiary(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString())),
                                   ds.Tables[0].Rows[0]["Loan_No"].ToString(),
                                   "0.00",
                                   Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[4].Value.ToString()).ToString("#,0.00"),
                                   ds.Tables[0].Rows[0]["userID"].ToString()
                                   );
                            }
                            break;
                        case "LOAN PRINCIPAL":
                            if (Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[5].Value.ToString()).ToString("#,0.00") != "0.00")
                            {
                                //PASTDUE
                                dgvORGridview.Rows.Add(
                                    ds.Tables[0].Rows[0]["PastDueDr"].ToString(),
                                    clsCollection.GetSubsidiary(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString())),
                                    ds.Tables[0].Rows[0]["Loan_No"].ToString(),
                                    Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[5].Value.ToString()).ToString("#,0.00"),
                                    "0.00",
                                    ds.Tables[0].Rows[0]["userID"].ToString()
                                    );

                                dgvORGridview.Rows.Add(
                                    ds.Tables[0].Rows[0]["CurrentDr"].ToString(),
                                    clsCollection.GetSubsidiary(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString())),
                                    ds.Tables[0].Rows[0]["Loan_No"].ToString(),
                                    "0.00",
                                    Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[3].Value.ToString()).ToString("#,0.00"),
                                    ds.Tables[0].Rows[0]["userID"].ToString()
                                    );
                            }
                            else
                            {
                                dgvORGridview.Rows.Add(
                                    ds.Tables[0].Rows[0]["CurrentDr"].ToString(),
                                    clsCollection.GetSubsidiary(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString())),
                                    ds.Tables[0].Rows[0]["Loan_No"].ToString(),
                                    "0.00",
                                    Convert.ToDecimal(dgvTemp.Rows[rowy].Cells[4].Value.ToString()).ToString("#,0.00"),
                                    ds.Tables[0].Rows[0]["userID"].ToString()
                                    );
                            }
                            break;
                    }
                }

                dgvORGridview.Enabled = true;
            }// END USING
        }


        //PDC CHECK OR VALIDATION
        public bool orNumberDuplicate(TextBox or)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM PDCManagementV2  WHERE ORNumber ='" + or.Text + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool checkSameORDIfferentID(string id,string ORNumber)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM PDCManagementV2 WHERE ORNumber = '" + ORNumber + "' and id <> '" + id + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    //OR NUMBER ALREADY USED
                    Alert.show("OR Number already used. ", Alert.AlertType.error);
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
