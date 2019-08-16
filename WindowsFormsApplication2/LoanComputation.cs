using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2
{
    public partial class LoanComputation : Form
    {
        public LoanComputation()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        public static int userID { get; set; }

        Global global = new Global();
        Classes.clsLoanComputation clsLoanComputation = new Classes.clsLoanComputation();
        Classes.clsLoanDataEntry clsLoanDataEntry = new Classes.clsLoanDataEntry();
        Classes.clsSearchDisbursement clsDisbursement = new Classes.clsSearchDisbursement();
        Classes.clsParameter clsParameter = new Classes.clsParameter();
        Classes.clsCollection clsCollection = new Classes.clsCollection();

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_firstClick == false)
                {
                    m_firstClick = true;
                    m_firstClickLoc = new Point(e.X, e.Y);
                }

                this.Location = new Point(
                    this.Location.X + e.X - m_firstClickLoc.X,
                    this.Location.Y + e.Y - m_firstClickLoc.Y
                    );
            }
            else
            {
                m_firstClick = false;
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(LoanComputation))
                {
                    form.Activate();
                    return;
                }
            }

            LoanComputation frm = new LoanComputation();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }

        private void LoanComputation_Load(object sender, EventArgs e)
        {
            clsLoanComputation.loadLoanApprovedDetails(dataGridView1,lblTotalCount);

            //Default
            txtProcessedBy.Text = Classes.clsUser.Username;
            txtProcessdDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(txtSearchLoanNo.Text == "")
            {
                Alert.show("Please enter valid Loan number.", Alert.AlertType.error);
                return;
            }

            clsLoanComputation.SearchLoanNo(dataGridView1, lblTotalCount, txtSearchLoanNo.Text, txtSearchLoanNo);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearchLoanNo.Text = "";
            clsLoanComputation.loadLoanApprovedDetails(dataGridView1, lblTotalCount);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                //No Data to be edit
                Alert.show("Please select transaction you want to release.", Alert.AlertType.warning);
                return;
            }
            txtSearchLoanNo.Text = "";

            userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
            txtEmployeeID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
            txtName.Text = clsLoanComputation.returnName(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
            txtCompany.Text = clsLoanComputation.returnCompanyName(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
            txtDateHired.Text = clsLoanComputation.returnDateHired(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
            txtMemberShipDate.Text = clsLoanComputation.returnMembershipDate(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
            txtShareCapitalBalance.Text = clsLoanDataEntry.returnShareCapital(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString())).ToString("#,0.00");
            txtSalary.Text = clsLoanComputation.returnSalary(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));


            //=====================================================================
            //              LOAN DETAILS 
            //=====================================================================
            txtLoanNo.Text = dataGridView1.SelectedRows[0].Cells["Loan_No"].Value.ToString();
            cmbLoanType.Text = dataGridView1.SelectedRows[0].Cells["Loan_Type"].Value.ToString();
            txtLoanAmount.Text = Convert.ToString(Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Loan_Amount"].Value.ToString()).ToString("#,0.00"));
            txtTermsInMonth.Text = dataGridView1.SelectedRows[0].Cells["Terms"].Value.ToString();
            txtMonthlyAmort.Text = Convert.ToString(Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Monthly_Amort"].Value.ToString()).ToString("#,0.00"));
            txtSemiMonthlyAmort.Text = Convert.ToString(Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Semi_Monthly_Amort"].Value.ToString()).ToString("#,0.00"));
            txtInterest.Text = dataGridView1.SelectedRows[0].Cells["Interest"].Value.ToString();
            txtPrincipal.Text = clsLoanComputation.returnPrincipalID(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));

            /*
            *   THIS WILL ENABLE ATM OR CHEQUE BUTTON
            *   ENHANCEMENT FOR LOAN COMPUTATION
            */

            if(dataGridView1.SelectedRows[0].Cells["ReleaseOption"].Value.ToString() == "ATM")
            {
                //ATM OPTION
                button1.Enabled = false;
                button2.Enabled = true;
            }
            else
            {
                //CHEQUE
                button1.Enabled = true;
                button2.Enabled = false;
            }

            //=====================================================================
            //              LOAN COMPUTATION 
            //=====================================================================
            txtLoanReceivable.Text = Convert.ToString(Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Loan_Amount"].Value.ToString()).ToString("#,0.00"));
            

            if(dataGridView1.SelectedRows[0].Cells["Outstanding_Balance"].Value.ToString() != "")
            {
                txtExistingBalance.Text = Convert.ToString(Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Outstanding_Balance"].Value.ToString()).ToString("#,0.00"));
            }
            else
            {
                txtExistingBalance.Text = "0.00";   
            }

            txtOtherDeduction.Text = clsLoanComputation.returnTotalOtherDeduction(txtLoanNo.Text);
            txtTotalDeferred.Text = clsLoanComputation.returnTotalDeferred(txtLoanNo.Text,cmbLoanType.Text);


            //FOR PLAR RENEW 
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                if (cmbLoanType.Text == "PLAR")
                {

                    adapter = new SqlDataAdapter("select Prev_Loan_No from loan WHERE Loan_No = '" + txtLoanNo.Text + "'", con);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    if ((!DBNull.Value.Equals(dt.Rows[0].ItemArray[0].ToString())))
                    {
                        //Get first the Net Amount before we compute the service Fee
                        //COmputation for Total deduction and net
                        double ttalDeduction, ttalNet;
                        double srvcFromNet;
                        ttalDeduction = Convert.ToDouble(txtExistingBalance.Text.Replace(",", "")) + Convert.ToDouble(txtTotalDeferred.Text.Replace(",", "")) + Convert.ToDouble(txtOtherDeduction.Text.Replace(",", ""));
                        ttalNet = Convert.ToDouble(txtLoanReceivable.Text.Replace(",", "")) - ttalDeduction;

                        //Charge Service fee for the net amount 
                        srvcFromNet = Convert.ToDouble(txtLoanAmount.Text.Replace(",", "")) - Convert.ToDouble(txtExistingBalance.Text.Replace(",", ""));
                        srvcFromNet = srvcFromNet * Convert.ToDouble(clsParameter.serviceFee());

                        txtServiceFee.Text = Convert.ToDecimal(srvcFromNet).ToString("#,0.00");
                        ttalDeduction = ttalDeduction + srvcFromNet;
                        ttalNet = Convert.ToDouble(txtLoanReceivable.Text.Replace(",", "")) - ttalDeduction;

                        txtTotalDeduction.Text = Convert.ToString(Convert.ToDouble(ttalDeduction).ToString("#,0.00"));
                        txtNetAmount.Text = Convert.ToString(Convert.ToDouble(ttalNet).ToString("#,0.00"));
                    }
                    else
                    {
                        //PLAR NEW APPLICATION
                        txtServiceFee.Text = Convert.ToString(Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Service_Fee"].Value.ToString()).ToString("#,0.00"));

                        //COmputation for Total deduction and net
                        double ttalDeduction, ttalNet;

                        ttalDeduction = Convert.ToDouble(txtServiceFee.Text.Replace(",", "")) + Convert.ToDouble(txtExistingBalance.Text.Replace(",", "")) + Convert.ToDouble(txtTotalDeferred.Text.Replace(",", "")) + Convert.ToDouble(txtOtherDeduction.Text.Replace(",", ""));
                        ttalNet = Convert.ToDouble(txtLoanReceivable.Text.Replace(",", "")) - ttalDeduction;

                        txtTotalDeduction.Text = Convert.ToString(Convert.ToDouble(ttalDeduction).ToString("#,0.00"));
                        txtNetAmount.Text = Convert.ToString(Convert.ToDouble(ttalNet).ToString("#,0.00"));
                    }
                }
                else
                {
                    txtServiceFee.Text = Convert.ToString(Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Service_Fee"].Value.ToString()).ToString("#,0.00"));

                    //COmputation for Total deduction and net
                    double ttalDeduction, ttalNet;

                    ttalDeduction = Convert.ToDouble(txtServiceFee.Text.Replace(",", "")) + Convert.ToDouble(txtExistingBalance.Text.Replace(",", "")) + Convert.ToDouble(txtTotalDeferred.Text.Replace(",", "")) + Convert.ToDouble(txtOtherDeduction.Text.Replace(",", ""));
                    ttalNet = Convert.ToDouble(txtLoanReceivable.Text.Replace(",", "")) - ttalDeduction;

                    txtTotalDeduction.Text = Convert.ToString(Convert.ToDouble(ttalDeduction).ToString("#,0.00"));
                    txtNetAmount.Text = Convert.ToString(Convert.ToDouble(ttalNet).ToString("#,0.00"));
                }

                txtProcessedBy.Text = Classes.clsUser.Username;
                txtProcessdDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if(btnClose.Text == "CLOSE")
            {
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string msg = Environment.NewLine + "Are you sure you want to continue?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    //=====================================================
                    //         LOAN DETAIL INSERT HERE
                    //=====================================================

                    clsLoanDataEntry.createSchedule(txtLoanNo.Text, clsLoanDataEntry.returnPaymentOption(txtLoanNo.Text), clsLoanDataEntry.loanAmountGross(txtLoanNo.Text), Convert.ToDouble(txtInterest.Text), Convert.ToDouble(txtTermsInMonth.Text), cmbLoanType.Text, txtProcessdDate.Text);

                    string jvNo;

                    //=================================================================================
                    //Create JV For Replenished Withdrawal
                    //=================================================================================

                    //=================================================================================
                    //JOURNAL HEADER
                    //=================================================================================
                    SqlCommand cmdHeader = new SqlCommand();
                    cmdHeader.Connection = con;
                    cmdHeader.CommandText = "sp_InsertJournalHeader";
                    cmdHeader.CommandType = CommandType.StoredProcedure;
                    cmdHeader.Parameters.AddWithValue("@JV_Date", DateTime.Today.ToShortDateString());
                    cmdHeader.Parameters.AddWithValue("@userID", userID);
                    cmdHeader.Parameters.AddWithValue("@AdjTo", txtEmployeeID.Text);
                    cmdHeader.Parameters.AddWithValue("@Particular", txtParticular.Text);
                    cmdHeader.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                    cmdHeader.Parameters.AddWithValue("@Posted", false);
                    cmdHeader.Parameters.AddWithValue("@Transaction_Type", "TRAN002");
                    cmdHeader.Parameters.AddWithValue("@summarize", false);
                    cmdHeader.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);
                    cmdHeader.Parameters.AddWithValue("@Poseted_By", "");
                    cmdHeader.ExecuteNonQuery();
                    //=================================================================================
                    //GET JV
                    //=================================================================================

                    SqlCommand cmdJV = new SqlCommand();
                    cmdJV.Connection = con;
                    cmdJV.CommandText = "sp_GetJVNoAfterSaving";
                    cmdJV.CommandType = CommandType.StoredProcedure;
                    cmdJV.Parameters.AddWithValue("@jv_date", DateTime.Today.ToShortDateString());
                    cmdJV.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmdJV);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        jvNo = dt.Rows[0].ItemArray[0].ToString();
                    }
                    else
                    {
                        return;
                    }

                    //=================================================================================
                    //JOURNAL DETAILS FIRST CURRENT LOAN AND BANK OF MEMBERS
                    //=================================================================================

                    //FOR CURRENT LOAN - LOAN AVAIL BY THE MEMBER
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_InsertJournalDetail";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@JV_No", jvNo);
                    cmd.Parameters.AddWithValue("@Account_Code", clsLoanComputation.returnChartAccountCode(cmbLoanType.Text));
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@Subsidiary_Code", clsCollection.GetSubsidiary(userID));
                    cmd.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                    cmd.Parameters.AddWithValue("@Debit", Convert.ToDecimal(txtLoanReceivable.Text.Replace(",", "")));
                    cmd.Parameters.AddWithValue("@Credit", Convert.ToDecimal("0.00"));
                    cmd.ExecuteNonQuery();

                    //FOR MEMBERS BANK INFORMATION
                    SqlCommand cmd1 = new SqlCommand();
                    cmd1.Connection = con;
                    cmd1.CommandText = "sp_InsertJournalDetail";
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@JV_No", jvNo);
                    cmd1.Parameters.AddWithValue("@Account_Code", clsLoanComputation.returnMemberBankCodeAccount(userID));
                    cmd1.Parameters.AddWithValue("@userID", userID);
                    cmd1.Parameters.AddWithValue("@Subsidiary_Code", clsCollection.GetSubsidiary(userID));
                    cmd1.Parameters.AddWithValue("@Loan_No", "");
                    cmd1.Parameters.AddWithValue("@Debit", Convert.ToDecimal("0.00"));
                    cmd1.Parameters.AddWithValue("@Credit", Convert.ToDecimal(txtNetAmount.Text.Replace(",", "")));
                    cmd1.ExecuteNonQuery();

                    //FOR SERVICE FEE
                    if (txtServiceFee.Text != "0.00")
                    {
                        SqlCommand cmd2 = new SqlCommand();
                        cmd2.Connection = con;
                        cmd2.CommandText = "sp_InsertJournalDetail";
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@JV_No", jvNo);
                        cmd2.Parameters.AddWithValue("@Account_Code", "405");
                        cmd2.Parameters.AddWithValue("@userID", userID);
                        cmd2.Parameters.AddWithValue("@Subsidiary_Code", clsCollection.GetSubsidiary(userID));
                        cmd2.Parameters.AddWithValue("@Loan_No", "");
                        cmd2.Parameters.AddWithValue("@Debit", Convert.ToDecimal("0.00"));
                        cmd2.Parameters.AddWithValue("@Credit", Convert.ToDecimal(txtServiceFee.Text.Replace(",", "")));
                        cmd2.ExecuteNonQuery();
                    }

                    //===================================================================================================
                    //                  IF EVER HE HAS OTHER DEDUCTION MADE
                    //===================================================================================================

                    SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT * FROM vw_loanDeductionToVoucher WHERE Loan_No = '" + txtLoanNo.Text + "'", con);
                    DataTable dt2 = new DataTable();
                    adapter2.Fill(dt2);

                    if (dt2.Rows.Count > 0)
                    {
                        int x = 0;
                        while (x != dt2.Rows.Count)
                        {
                            SqlCommand cmdDeduction = new SqlCommand();
                            cmdDeduction.Connection = con;
                            cmdDeduction.CommandText = "sp_InsertJournalDetail";
                            cmdDeduction.CommandType = CommandType.StoredProcedure;
                            cmdDeduction.Parameters.AddWithValue("@JV_No", jvNo);
                            cmdDeduction.Parameters.AddWithValue("@Account_Code", dt2.Rows[x].ItemArray[7].ToString());
                            cmdDeduction.Parameters.AddWithValue("@userID", userID);
                            cmdDeduction.Parameters.AddWithValue("@Subsidiary_Code", clsCollection.GetSubsidiary(userID));

                            if (dt2.Rows[x].ItemArray[4].ToString() != "")
                            {
                                cmdDeduction.Parameters.AddWithValue("@Loan_No", dt2.Rows[x].ItemArray[4].ToString());
                            }
                            else
                            {
                                cmdDeduction.Parameters.AddWithValue("@Loan_No", "");
                            }

                            //check if unearned or earned
                            //if unearned then go debit else credit
                            if(dt2.Rows[x].ItemArray[7].ToString() == "314")
                            {
                                cmdDeduction.Parameters.AddWithValue("@Debit", Convert.ToDecimal(dt2.Rows[x].ItemArray[5].ToString()));
                                cmdDeduction.Parameters.AddWithValue("@Credit", Convert.ToDecimal("0.00"));

                            }
                            else
                            {
                                cmdDeduction.Parameters.AddWithValue("@Debit", Convert.ToDecimal("0.00"));
                                cmdDeduction.Parameters.AddWithValue("@Credit", Convert.ToDecimal(dt2.Rows[x].ItemArray[5].ToString()));
                            }
                            
                            cmdDeduction.ExecuteNonQuery();

                            x = x + 1;//Increment
                        }
                    }


                    //Update loan net proceeds loan header
                    SqlCommand cmdLoanHeader = new SqlCommand();
                    cmdLoanHeader.Connection = con;
                    cmdLoanHeader.CommandText = "UPDATE Loan SET NetProceeds = '" + txtNetAmount.Text.Replace(",", "") + "', jv_no = '" + jvNo + "', Status = '8' WHERE loan_no = '" + txtLoanNo.Text + "'";
                    cmdLoanHeader.CommandType = CommandType.Text;
                    cmdLoanHeader.ExecuteNonQuery();

                    Alert.show("Loan# " + txtLoanNo.Text + " For ATM Preparation", Alert.AlertType.success);

                    //Refresh the grid
                    clsLoanComputation.loadLoanApprovedDetails(dataGridView1, lblTotalCount);

                    //Set Loan Released Date
                    SqlCommand cmdRelease = new SqlCommand();
                    cmdRelease.Connection = con;
                    cmdRelease.CommandText = "sp_LoanReleasedDate";
                    cmdRelease.CommandType = CommandType.StoredProcedure;
                    cmdRelease.Parameters.AddWithValue("@LoanNo", txtLoanNo.Text);
                    cmdRelease.ExecuteNonQuery();
                }
                clearTextFIelds();

                //RESET TO DEFAULT
                button2.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string msg = Environment.NewLine + "Are you sure you want to continue?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    //=====================================================
                    //         LOAN DETAIL INSERT HERE
                    //=====================================================

                    clsLoanDataEntry.createSchedule(txtLoanNo.Text, clsLoanDataEntry.returnPaymentOption(txtLoanNo.Text), clsLoanDataEntry.loanAmountGross(txtLoanNo.Text), Convert.ToDouble(txtInterest.Text), Convert.ToDouble(txtTermsInMonth.Text), cmbLoanType.Text, txtProcessdDate.Text);


                    string cvno;

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_InsertDisbursementHeader";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CVDate", DateTime.Today.ToShortDateString());

                    //FOr Payee Type 
                    //Member = 0 Client = 1

                    cmd.Parameters.AddWithValue("@Payee_Type", "1");
                    cmd.Parameters.AddWithValue("@userID", userID);

                    cmd.Parameters.AddWithValue("@Payee", txtEmployeeID.Text);
                    cmd.Parameters.AddWithValue("@Payee_Name", txtName.Text);
                    cmd.Parameters.AddWithValue("@Particulars", txtParticular.Text);
                    cmd.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                    cmd.Parameters.AddWithValue("@Bank_Code", "BDO");
                    cmd.Parameters.AddWithValue("@Check_No", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Check_Date", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtNetAmount.Text));
                    cmd.Parameters.AddWithValue("@Transaction_Type", "TRAN002");
                    cmd.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);
                    cmd.ExecuteNonQuery();

                    //Get The CV NO.

                    SqlCommand cmdCV = new SqlCommand();
                    cmdCV.Connection = con;
                    cmdCV.CommandText = "sp_GetCVNoAfterSaving";
                    cmdCV.CommandType = CommandType.StoredProcedure;
                    cmdCV.Parameters.AddWithValue("@CV_Date", DateTime.Today.ToShortDateString());
                    cmdCV.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmdCV);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        cvno = dt.Rows[0].ItemArray[0].ToString();
                    }
                    else
                    {
                        return;
                    }

                    //=================================================================================
                    //DISBURSEMENT DETAILS FIRST CURRENT LOAN AND BANK OF MEMBERS
                    //=================================================================================

                    //FOR CURRENT LOAN - LOAN AVAIL BY THE MEMBER
                    SqlCommand cmd1 = new SqlCommand();
                    cmd1.Connection = con;
                    cmd1.CommandText = "sp_InsertDisbursementDetail";
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@CV_No", cvno);
                    cmd1.Parameters.AddWithValue("@Account_Code", clsLoanComputation.returnChartAccountCode(cmbLoanType.Text));
                    cmd1.Parameters.AddWithValue("@userID", userID);
                    cmd1.Parameters.AddWithValue("@Subsidiary_Code", clsCollection.GetSubsidiary(userID));
                    cmd1.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                    cmd1.Parameters.AddWithValue("@Debit", Convert.ToDecimal(txtLoanReceivable.Text.Replace(",", "")));
                    cmd1.Parameters.AddWithValue("@Credit", Convert.ToDecimal("0.00"));
                    cmd1.ExecuteNonQuery();

                    //FOR MEMBERS BANK INFORMATION
                    SqlCommand cmd2 = new SqlCommand();
                    cmd2.Connection = con;
                    cmd2.CommandText = "sp_InsertDisbursementDetail";
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@CV_No", cvno);
                    cmd2.Parameters.AddWithValue("@Account_Code", "102.02");
                    cmd2.Parameters.AddWithValue("@userID", userID);
                    cmd2.Parameters.AddWithValue("@Subsidiary_Code", clsCollection.GetSubsidiary(userID));
                    cmd2.Parameters.AddWithValue("@Loan_No", "");
                    cmd2.Parameters.AddWithValue("@Debit", Convert.ToDecimal("0.00"));
                    cmd2.Parameters.AddWithValue("@Credit", Convert.ToDecimal(txtNetAmount.Text.Replace(",", "")));
                    cmd2.ExecuteNonQuery();

                    //FOR SERVICE FEE
                    if (txtServiceFee.Text != "0.00")
                    {
                        SqlCommand cmd3 = new SqlCommand();
                        cmd3.Connection = con;
                        cmd3.CommandText = "sp_InsertDisbursementDetail";
                        cmd3.CommandType = CommandType.StoredProcedure;
                        cmd3.Parameters.AddWithValue("@CV_No", cvno);
                        cmd3.Parameters.AddWithValue("@Account_Code", "405");
                        cmd3.Parameters.AddWithValue("@userID", userID);
                        cmd3.Parameters.AddWithValue("@Subsidiary_Code", clsCollection.GetSubsidiary(userID));
                        cmd3.Parameters.AddWithValue("@Loan_No", "");
                        cmd3.Parameters.AddWithValue("@Debit", Convert.ToDecimal("0.00"));
                        cmd3.Parameters.AddWithValue("@Credit", Convert.ToDecimal(txtServiceFee.Text.Replace(",", "")));
                        cmd3.ExecuteNonQuery();
                    }

                    //===================================================================================================
                    //                  IF EVER HE HAS OTHER DEDUCTION MADE
                    //===================================================================================================

                    SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT * FROM vw_loanDeductionToVoucher WHERE Loan_No = '" + txtLoanNo.Text + "'", con);
                    DataTable dt2 = new DataTable();
                    adapter2.Fill(dt2);

                    if (dt2.Rows.Count > 0)
                    {
                        int x = 0;
                        while (x != dt2.Rows.Count)
                        {
                            SqlCommand cmdDeduction = new SqlCommand();
                            cmdDeduction.Connection = con;
                            cmdDeduction.CommandText = "sp_InsertDisbursementDetail";
                            cmdDeduction.CommandType = CommandType.StoredProcedure;
                            cmdDeduction.Parameters.AddWithValue("@CV_No", cvno);
                            cmdDeduction.Parameters.AddWithValue("@Account_Code", dt2.Rows[x].ItemArray[7].ToString());
                            cmdDeduction.Parameters.AddWithValue("@userID", userID);
                            cmdDeduction.Parameters.AddWithValue("@Subsidiary_Code", clsCollection.GetSubsidiary(userID));

                            if (dt2.Rows[x].ItemArray[4].ToString() != "")
                            {
                                cmdDeduction.Parameters.AddWithValue("@Loan_No", dt2.Rows[x].ItemArray[4].ToString());
                            }
                            else
                            {
                                cmdDeduction.Parameters.AddWithValue("@Loan_No", "");
                            }

                            cmdDeduction.Parameters.AddWithValue("@Debit", Convert.ToDecimal("0.00"));
                            cmdDeduction.Parameters.AddWithValue("@Credit", Convert.ToDecimal(dt2.Rows[x].ItemArray[5].ToString()));
                            cmdDeduction.ExecuteNonQuery();

                            x = x + 1;//Increment
                        }
                    }


                    //=====================================================================================================================
                    //Display Disbursement Screen

                    DisbursementVoucher cv = new DisbursementVoucher();
                    cv.Show();
                    //=========================================================================================
                    //                              Header Information
                    //=========================================================================================

                    SqlDataAdapter adapter3 = new SqlDataAdapter("SELECT * FROM Disbursement_Header WHERE CV_No = '" + cvno + "'", con);
                    DataTable dt3 = new DataTable();
                    adapter3.Fill(dt3);


                    cv.txtCVNo.Text = cvno;
                    cv.dtCVDate.Text = dt3.Rows[0].ItemArray[1].ToString();


                    if (dt3.Rows[0].ItemArray[3].ToString() == "True")
                    {
                        cv.radioClient.Checked = true;
                    }
                    else
                    {
                        cv.radioMember.Checked = true;
                    }
                    cv.txtPayee.Text = dt3.Rows[0].ItemArray[5].ToString();

                    if (dt3.Rows[0].ItemArray[4].ToString() != "" || dt3.Rows[0].ItemArray[4].ToString() != string.Empty)
                    {
                        Classes.clsDisbursement.userID = Convert.ToInt32(dt3.Rows[0].ItemArray[4].ToString());
                        cv.txtPayeeName.Text = clsDisbursement.fullName(Convert.ToInt32(dt3.Rows[0].ItemArray[4].ToString()));
                    }
                    else
                    {
                        cv.txtPayeeName.Text = clsDisbursement.ClientName(dt3.Rows[0].ItemArray[5].ToString());
                    }



                    cv.txtLoanNo.Text = dt3.Rows[0].ItemArray[8].ToString();

                    cv.cmbTransaction.SelectedValue = dt3.Rows[0].ItemArray[13].ToString();
                    cv.cmbBank.SelectedValue = dt3.Rows[0].ItemArray[9].ToString();
                    cv.txtChequeNo.Text = dt3.Rows[0].ItemArray[10].ToString();
                    cv.dtChequeDate.Text = dt3.Rows[0].ItemArray[11].ToString();
                    cv.txtAmount.Text = Convert.ToDecimal(dt3.Rows[0].ItemArray[12].ToString()).ToString("#,0.00");
                    cv.txtParticular.Text = dt3.Rows[0].ItemArray[7].ToString();


                    //================================
                    //      DISPLAY DETAILS
                    //================================

                    cv.dataGridView1.Rows.Clear();
                    clsDisbursement.loadDetails(cv.dataGridView1, cv.txtCVNo.Text);

                    //=========================================================================================
                    //                              Enable Buttons
                    //=========================================================================================
                    cv.btnEdit.Enabled = true;
                    cv.btnPost.Enabled = true;
                    cv.btnCancel.Enabled = true;
                    cv.btnPrint.Enabled = true;
                    cv.btnPrintCheque.Enabled = true;

                    Alert.show("Loan# " + txtLoanNo.Text + " For Cheque Release", Alert.AlertType.success);

                    //Update loan net proceeds loan header
                    SqlCommand cmdLoanHeader = new SqlCommand();
                    cmdLoanHeader.Connection = con;
                    cmdLoanHeader.CommandText = "UPDATE Loan SET NetProceeds = '" + txtNetAmount.Text.Replace(",", "") + "', cv_no = '" + cvno + "', Status = '5' WHERE loan_no = '" + txtLoanNo.Text + "'";
                    cmdLoanHeader.CommandType = CommandType.Text;
                    cmdLoanHeader.ExecuteNonQuery();

                    //Refresh the grid
                    clsLoanComputation.loadLoanApprovedDetails(dataGridView1, lblTotalCount);

                    //Set Loan Released Date
                    SqlCommand cmdRelease = new SqlCommand();
                    cmdRelease.Connection = con;
                    cmdRelease.CommandText = "sp_LoanReleasedDate";
                    cmdRelease.CommandType = CommandType.StoredProcedure;
                    cmdRelease.Parameters.AddWithValue("@LoanNo", txtLoanNo.Text);
                    cmdRelease.ExecuteNonQuery();
                }
                clearTextFIelds();

                //RESET TO DEFAULT
                button1.Enabled = false;
            }
        }

        //============================================
        //  CLEAR ALL TEXTFIELDS
        //============================================
        public void clearTextFIelds()
        {
            foreach (var c in panel5.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
            }

            foreach (var c in panel17.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
            }

            foreach (var c in panel23.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            loanFrms.loanComputationDetails loanCompdetail = new loanFrms.loanComputationDetails();
            Classes.clsLoanComputationDetails clsLoanDetail = new Classes.clsLoanComputationDetails();

            clsLoanDetail.loadLoansDetails(txtLoanNo.Text,cmbLoanType.Text, loanCompdetail.dataGridView1);

            loanCompdetail.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            loanFrms.loanComputationDetails loanCompdetail = new loanFrms.loanComputationDetails();
            Classes.clsLoanComputationDetails clsLoanDetail = new Classes.clsLoanComputationDetails();

            clsLoanDetail.loadLoansDetailsOthers(txtLoanNo.Text, loanCompdetail.dataGridView1);

            loanCompdetail.ShowDialog();
        }
    }
}
