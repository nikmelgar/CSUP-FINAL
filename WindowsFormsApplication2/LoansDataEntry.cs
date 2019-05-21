using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;

namespace WindowsFormsApplication2
{
    public partial class LoansDataEntry : Form
    {
        public LoansDataEntry()
        {
            InitializeComponent();
        }

        //For PLAR Renew

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        Classes.clsLoan clsLoan = new Classes.clsLoan();
        Classes.clsLoanDataEntry clsLoanDataEntry = new Classes.clsLoanDataEntry();
        Classes.clsLoanLookUp clsLookUp = new Classes.clsLoanLookUp();
        Classes.clsLoanApproval clsApproval = new Classes.clsLoanApproval();
        Classes.clsLoanBalancesFromDataEntry clsLoanBalances = new Classes.clsLoanBalancesFromDataEntry();
        Classes.clsParameter clsParameter = new Classes.clsParameter();

        Global global = new Global();
        Loans loans = new Loans();

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        private void panelHeader_MouseMove(object sender, MouseEventArgs e)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            if(btnClose.Text == "CLOSE")
            {
                btnCancel.Visible = false;
                this.Close();
            }
            else
            {
                string msg = Environment.NewLine + "Are you sure you want to cancel this?";
                DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    clearFieldsForCancel();
                }
                else
                {
                    return;
                }
            }
        }

        private void LoansDataEntry_Load(object sender, EventArgs e)
        {
            //Set Default 
            txtEncodedBy.Text = Classes.clsUser.Username;          
            txtDateEncoded.Text = DateTime.Now.ToString("MM/dd/yyyy");

            //Loan Type
            clsLoan.loadComboBox(cmbLoanType);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            loanFrms.lookUpMember frm = new loanFrms.lookUpMember();
            frm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {          
            LoanApproval loanApproval= new LoanApproval();

            //=============================================================================
            //                      MEMBERS INFORMATION
            //=============================================================================

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanMembersInformation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userID", Classes.clsLoanDataEntry.userID);

                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    loanApproval.txtEmployeeID.Text = dt.Rows[0].ItemArray[1].ToString();

                    if (dt.Rows[0].ItemArray[2].ToString() == "False")
                    {
                        loanApproval.txtPrincipal.Text = clsLookUp.returnPrincipal(dt.Rows[0].ItemArray[1].ToString());
                    }

                    loanApproval.txtName.Text = dt.Rows[0].ItemArray[3].ToString();

                    if (dt.Rows[0].ItemArray[4].ToString() != "")
                    {
                        loanApproval.txtSalary.Text = Convert.ToDecimal(dt.Rows[0].ItemArray[4].ToString()).ToString("#,0.00");
                    }

                    loanApproval.txtDateHired.Text = Convert.ToString(Convert.ToDateTime(dt.Rows[0].ItemArray[5].ToString()).ToShortDateString());
                    loanApproval.txtMemberShipDate.Text = Convert.ToString(Convert.ToDateTime(dt.Rows[0].ItemArray[6].ToString()).ToShortDateString());

                    try
                    {
                        loanApproval.txtPMSDate.Text = Convert.ToString(Convert.ToDateTime(dt.Rows[0].ItemArray[7].ToString()).ToShortDateString());
                    }
                    catch
                    {

                    }


                    //For Getting the years in service
                    var yrsDate = new DateTime();
                    int yrsFrmDateHire, yrsToday, total;
                    yrsDate = Convert.ToDateTime(loanApproval.txtDateHired.Text);
                    yrsFrmDateHire = Convert.ToInt32(yrsDate.Date.Year.ToString());
                    yrsToday = Convert.ToInt32(DateTime.Today.Year.ToString());
                    total = yrsToday - yrsFrmDateHire;
                    loanApproval.txtYrsInService.Text = total.ToString();

                    loanApproval.txtCompany.Text = clsLoanDataEntry.returnCompanyDescription(Classes.clsLoanDataEntry.userID);

                    //Share Capital Balance
                    loanApproval.txtShareCapitalBalance.Text = clsLoanDataEntry.returnShareCapital(Classes.clsLoanDataEntry.userID).ToString("#,0.00");
                }

                //==================================================================================================================
                //                              END MEMBERS INFORMATION DETAIL
                //==================================================================================================================

                //============================================================
                //              DETAILS OF LOAN
                //============================================================
                loanApproval.txtLoanNo.Text = txtLoanNo.Text;
                loanApproval.cmbLoanType.Text = cmbLoanType.SelectedValue.ToString();
                loanApproval.txtLoanAmount.Text = txtLoanAmount.Text;
                loanApproval.txtTermsInMonth.Text = txtTermsInMonth.Text;
                loanApproval.txtMonthlyAmort.Text = txtMonthlyAmort.Text;
                loanApproval.txtSemiMonthlyAmort.Text = txtSemiMonthlyAmort.Text;
                loanApproval.txtInterest.Text = txtInterest.Text;
                //============================================================
                //              END DETAILS OF LOAN
                //============================================================

                //============================================================
                //              END DETAILS OF LOAN
                //============================================================

                //Return Reason for Cancellation if Status = 7 //Cancelled
                if (status.Text == "CANCELLED" || status.Text == "RELEASED")
                {
                    SqlDataAdapter adapterCancelled = new SqlDataAdapter("SELECT Note,Cancelled_By,Cancelled_Date,Approved_By,Approved_Date,Disapproved_By,Disapproved_Date FROM Loan WHERE Loan_No = '" + txtLoanNo.Text + "'", con);
                    DataTable dt2 = new DataTable();
                    adapterCancelled.Fill(dt2);

                    loanApproval.txtReason.Text = dt2.Rows[0].ItemArray[0].ToString();
                    if (dt2.Rows[0].ItemArray[2].ToString() != "")
                    {
                        loanApproval.txtDateCancelled.Text = Convert.ToDateTime(dt2.Rows[0].ItemArray[2].ToString()).ToShortDateString();
                    }
                    loanApproval.txtCancelledBy.Text = dt2.Rows[0].ItemArray[1].ToString();

                    if (dt2.Rows[0].ItemArray[4].ToString() != "")
                    {
                        //Approved
                        loanApproval.txtApprovedBy.Text = dt2.Rows[0].ItemArray[3].ToString();
                        loanApproval.txtAppDissDate.Text = Convert.ToDateTime(dt2.Rows[0].ItemArray[4].ToString()).ToShortDateString();
                    }

                    if (dt2.Rows[0].ItemArray[6].ToString() != "")
                    {
                        //Disapproved
                        loanApproval.txtDisapprovedBy.Text = dt2.Rows[0].ItemArray[5].ToString();
                        loanApproval.txtAppDissDate.Text = Convert.ToDateTime(dt2.Rows[0].ItemArray[6].ToString()).ToShortDateString();
                    }

                }
                else if (status.Text == "APPROVED")
                {
                    SqlDataAdapter adapterCancelled = new SqlDataAdapter("SELECT Note,Approved_By,Approved_Date FROM Loan WHERE Loan_No = '" + txtLoanNo.Text + "'", con);
                    DataTable dt2 = new DataTable();
                    adapterCancelled.Fill(dt2);

                    loanApproval.txtReason.Text = dt2.Rows[0].ItemArray[0].ToString();
                    loanApproval.txtAppDissDate.Text = Convert.ToDateTime(dt2.Rows[0].ItemArray[2].ToString()).ToShortDateString();
                    loanApproval.txtApprovedBy.Text = dt2.Rows[0].ItemArray[1].ToString();
                }
                else if (status.Text == "DISAPPROVED")
                {
                    SqlDataAdapter adapterCancelled = new SqlDataAdapter("SELECT Note,Disapproved_By,Disapproved_Date FROM Loan WHERE Loan_No = '" + txtLoanNo.Text + "'", con);
                    DataTable dt2 = new DataTable();
                    adapterCancelled.Fill(dt2);

                    loanApproval.txtReason.Text = dt2.Rows[0].ItemArray[0].ToString();
                    loanApproval.txtAppDissDate.Text = Convert.ToDateTime(dt2.Rows[0].ItemArray[2].ToString()).ToShortDateString();
                    loanApproval.txtDisapprovedBy.Text = dt2.Rows[0].ItemArray[1].ToString();
                }

                ///Load Balances

                clsApproval.loadLoanBalances(Classes.clsLoanDataEntry.userID, loanApproval.dataGridView1, cmbLoanType.SelectedValue.ToString());


                /*
                *   This is for the automation of the loan deferred
                *   20% Net
                */

                //Compute for 20 net 
                //Check first if theres a deferred loan (Selected Member by User)
                if(clsLoanDataEntry.hasDeferredLoan(Classes.clsLoanDataEntry.userID,cmbLoanType.SelectedValue.ToString()) == true)
                {
                    //Delete first before adding to temp table
                    SqlCommand cmdDelete = new SqlCommand();
                    cmdDelete.Connection = con;
                    cmdDelete.CommandText = "Delete loan_AutoCompute";
                    cmdDelete.CommandType = CommandType.Text;
                    cmdDelete.ExecuteNonQuery();

                    //First step add to table
                    clsLoanDataEntry.insertLoanDeferred(Classes.clsLoanDataEntry.userID, cmbLoanType.SelectedValue.ToString(), txtLoanNo.Text);

                    //Check if 20% of net is Greater than or Less Than

                    //check if plar
                    string loanAmnt = txtLoanAmount.Text;
                    if(cmbLoanType.SelectedValue.ToString() == "PLAR" && txtPrevOutstandingBalance.Text != "")
                    {
                        //Minus the balance in loan amount
                        decimal amt;
                        amt = Convert.ToDecimal(txtLoanAmount.Text) - Convert.ToDecimal(txtPrevOutstandingBalance.Text);

                        loanAmnt = amt.ToString();
                    }

                    if (clsLoanDataEntry.getTotalAmountMinusServiceFee(Convert.ToDecimal(loanAmnt),Convert.ToDecimal(loanApproval.txtShareCapital.Text),Convert.ToDecimal(loanApproval.txtSavings.Text),cmbLoanType.SelectedValue.ToString()) > clsLoanDataEntry.totalDeferredAmount(Classes.clsLoanDataEntry.userID,cmbLoanType.SelectedValue.ToString()))
                    {
                        //if Greater than then as ease the value
                        SqlDataAdapter adapterGet = new SqlDataAdapter("select * from loan_AutoCompute where loan_no = '"+ txtLoanNo.Text +"'", con);
                        DataSet ds = new DataSet();
                        adapterGet.Fill(ds);

                        if(ds.Tables[0].Rows.Count > 0)
                        {
                            for(int xx =0; xx < ds.Tables[0].Rows.Count; xx++)
                            {
                                foreach (DataGridViewRow row in loanApproval.dataGridView1.Rows)
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
                        clsLoanDataEntry.updateLoanDeferredPercent(txtLoanNo.Text, clsLoanDataEntry.totalDeferredAmount(Classes.clsLoanDataEntry.userID, cmbLoanType.SelectedValue.ToString()));

                        //Update Amount if The 20% of Net is less than the deferred amount
                        SqlDataAdapter adapterGet = new SqlDataAdapter("select * from loan_AutoCompute where loan_no = '" + txtLoanNo.Text + "'", con);
                        DataSet ds = new DataSet();
                        adapterGet.Fill(ds);
                        string amt;
                        decimal net = clsLoanDataEntry.getTotalAmountMinusServiceFee(Convert.ToDecimal(loanAmnt), Convert.ToDecimal(loanApproval.txtShareCapital.Text), Convert.ToDecimal(loanApproval.txtSavings.Text), cmbLoanType.SelectedValue.ToString());
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for(int x = 0; x < ds.Tables[0].Rows.Count; x++)
                            {
                                SqlCommand cmd2 = new SqlCommand();
                                cmd2.Connection = con;
                                amt = Convert.ToString(Convert.ToDecimal(net) * Convert.ToDecimal(ds.Tables[0].Rows[x]["Percentage"].ToString()));
                                cmd2.CommandText = "UPDATE loan_AutoCompute set Amount = " + Convert.ToDecimal(decimal.Round(Convert.ToDecimal(amt), 2)) + " WHERE loan_type = '" + ds.Tables[0].Rows[x]["Loan_Type"].ToString() + "' and loan_no = '" + txtLoanNo.Text + "'";
                                cmd2.CommandType = CommandType.Text;
                                cmd2.ExecuteNonQuery();
                            }

                            //SELECT THE UPDATED VALUES IN TABLE
                            SqlDataAdapter adpterUpdate = new SqlDataAdapter("select * from loan_AutoCompute where loan_no = '" + txtLoanNo.Text + "'", con);
                            DataSet ds2 = new DataSet();
                            adpterUpdate.Fill(ds2);

                            for (int xx = 0; xx < ds2.Tables[0].Rows.Count; xx++)
                            {
                                foreach (DataGridViewRow row in loanApproval.dataGridView1.Rows)
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
            this.Close();
            loanApproval.ShowDialog();
        }

        private void cmbLoanType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbLoanType.Text != "" && txtEmployeeID.Text != "")
            {
                if(btnSave.Text == "SAVE")
                {
                    //Check first if existing 
                    if(status.Text != "FOR RELEASE")
                    {
                        if (clsLoanDataEntry.checkIfSameLoanType(cmbLoanType.SelectedValue.ToString(), Classes.clsLoanDataEntry.userID) == true)
                        {
                            return;
                        }
                    }
                }
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    adapter = new SqlDataAdapter("SELECT Interest FROM Loan_Type WHERE Loan_Type = '" + cmbLoanType.SelectedValue.ToString() + "'", con);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    txtInterest.Text = dt.Rows[0].ItemArray[0].ToString();
                }
                //enable the fields
                txtLoanAmount.Enabled = true;
                txtTermsInMonth.Enabled = true;

                //SET FOR PREVIOUS LOAN
                clsLoanBalances.loadPrevLoanBalance(Classes.clsLoanDataEntry.userID, cmbLoanType.SelectedValue.ToString(), txtPrevLoanNo, txtPrevLoanType, txtPrevGrossAmount, txtPrevReleasedDate, txtPrevOutstandingBalance, txtPrevDeferred, txtPrevTerms, txtPrevMonthly,txtLoanNo);
                
            }
            else
            {
                //Disable the fields
                txtLoanAmount.Enabled = false;
                txtTermsInMonth.Enabled = false;
            }
        }

        public void compute(double pv, double rt, double trm)
        {
            if (txtLoanAmount.Text == "0.00" || txtLoanAmount.Text == "0" || txtLoanAmount.Text == "")
            {
                txtMonthlyAmort.Text = "0.00";
                txtSemiMonthlyAmort.Text = "0.00";
                return;
            }

            if(txtTermsInMonth.Text == "0" || txtTermsInMonth.Text == "")
            {
                txtMonthlyAmort.Text = "0.00";
                txtSemiMonthlyAmort.Text = "0.00";
                return;
            }
            // Roadmap for building the function
            //( Pv  *  R ) / (1 - ( 1 + R )^(-n)
            double PV = pv;
            double rate = (double)rt;
            double term = trm;
            double val1 = 1 + rate;
            double val2 = -term;
            double powResult = Math.Pow(val1, val2);
            double rightSide = 1 - powResult;
            double leftSide = PV * rate;
            double finalResult = leftSide / rightSide;

            double finalResult2 = (PV * rate) / (1 - (Math.Pow((1 + rate), -term)));
            decimal dec = Convert.ToDecimal(finalResult2);
            decimal sem = dec / 2;

            txtMonthlyAmort.Text = Convert.ToString(dec.ToString("#,0.00"));
            txtSemiMonthlyAmort.Text = Convert.ToString(sem.ToString("#,0.00"));
        }

        private void txtLoanAmount_Validated(object sender, EventArgs e)
        {
            if (txtLoanAmount.Text != "" && txtTermsInMonth.Text != "")
            {
                compute(Convert.ToDouble(txtLoanAmount.Text), Convert.ToDouble(txtInterest.Text), Convert.ToDouble(txtTermsInMonth.Text));
                txtLoanAmount.Text = Convert.ToString(Convert.ToDecimal(txtLoanAmount.Text).ToString("#,0.00"));
            }
        }

        private void txtTermsInMonth_Validated(object sender, EventArgs e)
        {
            if (txtLoanAmount.Text != "" && txtTermsInMonth.Text != "")
            {
                compute(Convert.ToDouble(txtLoanAmount.Text), Convert.ToDouble(txtInterest.Text), Convert.ToDouble(txtTermsInMonth.Text));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loanFrms.searchCoMaker coMaker = new loanFrms.searchCoMaker();
            Classes.clsSearchCoMaker.getCompanyCode = txtCompany.Text;
            loanFrms.searchCoMaker.getLoanType = cmbLoanType.SelectedValue.ToString();
            coMaker.ShowDialog();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                if (dataGridView1.SelectedRows.Count >= 1)
                {
                    string msg = Environment.NewLine + "Are you sure you want to delete this co-maker?";
                    DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            if (row.Cells[0].Value != null)
                            {
                                dataGridView1.Rows.Remove(row);
                                lblTotalCntMakers.Text = dataGridView1.Rows.Count.ToString();
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch
            {

            }
        }

        //Clear All fields
        public void clearFieldsForCancel()
        {
            status.Visible = false;
            //For Members Information
            foreach (var c in panel1.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
            }
            
            //Details of Loan
            foreach( var c in panel11.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
                if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
            }

            //For Co Makers
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                try
                {
                    dataGridView1.Rows.Clear();
                    lblTotalCntMakers.Text = dataGridView1.Rows.Count.ToString();
                }
                catch
                {

                }
            }

            //Prev Loan
            foreach(var c in panel23.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
            }

            //Disable Button
            btnSearch.Enabled = false;
            button1.Enabled = false; //Co Maker

            //Change Button Text
            btnClose.Text = "CLOSE";
            btnSave.Text = "NEW";

            //Cancel
            txtCancel.Visible = false;
            lblReason.Visible = false;
        }

        
        private void btnSave_Click(object sender, EventArgs e)
        {
            //Validation 
            if(btnSave.Text == "SAVE" || btnSave.Text == "UPDATE")
            {
                if(txtEmployeeID.Text == "")
                {
                    Alert.show("Please select member first!", Alert.AlertType.error);
                    return;
                }
                if(txtLoanAmount.Text == "")
                {
                    Alert.show("Loan Amount is required!", Alert.AlertType.error);
                    return;
                }
                if(txtTermsInMonth.Text == "")
                {
                    Alert.show("Loan Terms in Mos is required!", Alert.AlertType.error);
                    return;
                }
                if(cmbPaymentOption.Text == "")
                {
                    Alert.show("Please select Payment Option", Alert.AlertType.error);
                    return;
                }
                if(cmbReleaseOption.Text == "")
                {
                    Alert.show("Please select Release Option", Alert.AlertType.error);
                    return;
                }
                
                if (cmbReleaseOption.Text == "ATM")
                {
                    if(clsLoan.invalidBankAccount(Classes.clsLoanDataEntry.userID) == true)
                    {
                        return;
                    }
                }
                
                //Check if Max loanable amount for Prev.
                if(txtPrevOutstandingBalance.Text != "")
                {
                    if (clsLoanBalances.checkIfEqualMaxAndBalance(cmbLoanType.SelectedValue.ToString(), Convert.ToDecimal(txtPrevOutstandingBalance.Text)) == true)
                    {
                        return;
                    }
                }

                //Check Max Amount if > to Loan input amount
                if (txtLoanAmount.Text != "")
                {
                    clsLoanDataEntry.checkIfGreaterThanLoanableAmount(txtLoanAmount, cmbLoanType.SelectedValue.ToString());
                    if(clsLoanDataEntry.checkIfGreaterThanLoanableAmount(txtLoanAmount, cmbLoanType.SelectedValue.ToString()) == true)
                    {
                        return;
                    }
                }

                //Disable If Company is Non payroll
                if (clsLookUp.returnCompanyCode(txtCompany.Text) != "COMP010")
                {
                    //NON-PAYROLL
                    //Validation For Co Makers
                    if (clsLoanDataEntry.isShortTerm(cmbLoanType.SelectedValue.ToString()) == true)
                    {
                        //Short Term
                        if (dataGridView1.Rows.Count < 6)
                        {
                            Alert.show("Insufficient required number of co-makers (6)", Alert.AlertType.error);
                            return;
                        }
                        else if(dataGridView1.Rows.Count > 6)
                        {
                            Alert.show("Already exceeds the required number of co-makers.", Alert.AlertType.error);
                            return;
                        }

                    }
                    else
                    {
                        //Long Term
                        string str = txtLoanAmount.Text;
                        str = str.Replace(",", "");
                        decimal answer;
                        double ttalCO = Convert.ToDouble(txtLoanAmount.Text) / 500000.00;
                        answer = Convert.ToDecimal(ttalCO);

                        if ((answer % 1) > 0)
                        {
                            //is decimal
                            answer = clsLoanDataEntry.GetDot(Convert.ToString(ttalCO)) * 6;
                        }
                        else
                        {
                            answer = answer * 6;
                        }


                        if (dataGridView1.Rows.Count != answer && dataGridView1.Rows.Count < answer)
                        {
                            Alert.show("Insufficient required number of co-makers (" + answer.ToString() + ")", Alert.AlertType.error);
                            return;
                        }

                        if (dataGridView1.Rows.Count > answer)
                        {
                            Alert.show("Already exceeds the required number of co-makers.", Alert.AlertType.error);
                            return;
                        }
                    }
                }
               
            }

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                if (btnSave.Text == "SAVE")
                {
                    if (status.Text != "FOR RELEASE")
                    {
                        //Check first if existing 
                        if (clsLoanDataEntry.checkIfSameLoanType(cmbLoanType.SelectedValue.ToString(), Classes.clsLoanDataEntry.userID) == true)
                        {
                            return;
                        }
                    }

                    //========================================================================================
                    //                      CREATION OF LOAN HEADER
                    //========================================================================================

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_InsertLoanHeader";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Loan_Type", cmbLoanType.SelectedValue);
                    cmd.Parameters.AddWithValue("@Payment_Option", cmbPaymentOption.Text);
                    cmd.Parameters.AddWithValue("@Loan_Amount", txtLoanAmount.Text.Replace(",", ""));
                    cmd.Parameters.AddWithValue("@Terms", txtTermsInMonth.Text);
                    cmd.Parameters.AddWithValue("@No_Payment", Convert.ToInt32(txtTermsInMonth.Text) * 2);
                    cmd.Parameters.AddWithValue("@Monthly_Amort", txtMonthlyAmort.Text.Replace(",", ""));
                    cmd.Parameters.AddWithValue("@Semi_Monthly_Amort", txtSemiMonthlyAmort.Text.Replace(",", ""));
                    cmd.Parameters.AddWithValue("@Interest", txtInterest.Text);
                    cmd.Parameters.AddWithValue("@userID", Classes.clsLoanDataEntry.userID); //Stores the USERID of Subs or Members
                    cmd.Parameters.AddWithValue("@EmployeeID", txtEmployeeID.Text);

                    if (txtPrevLoanNo.Text != "")
                    {
                        cmd.Parameters.AddWithValue("@Gross_Amount", txtPrevGrossAmount.Text.Replace(",", ""));
                        cmd.Parameters.AddWithValue("@Outstanding_Balance", txtPrevOutstandingBalance.Text.Replace(",", ""));
                        cmd.Parameters.AddWithValue("@Prev_Loan_No", txtPrevLoanNo.Text);

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Gross_Amount", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Outstanding_Balance", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Prev_Loan_No", DBNull.Value);
                    }

                    if (txtPrevDeferred.Text != "")
                    {
                        cmd.Parameters.AddWithValue("@Deferred_Balance", txtPrevDeferred.Text.Replace(",", ""));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Deferred_Balance", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@Encoded_By", txtEncodedBy.Text);
                    cmd.Parameters.AddWithValue("@Status", Convert.ToInt32(1));
                    cmd.Parameters.AddWithValue("@ReleaseOption", cmbReleaseOption.Text);
                    cmd.ExecuteNonQuery();

                    //====================================================================================
                    //              END OF CREATING HEADER
                    //====================================================================================

                    //====================================================================================
                    //              CALL LOAN NO ACCORDING TO THE USER INPUT
                    //====================================================================================
                    adapter = new SqlDataAdapter("SELECT top 1 loan_no FROM Loan where encoded_by = '" + txtEncodedBy.Text + "' ORDER BY loan_no DESC", con);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    txtLoanNo.Text = dt.Rows[0].ItemArray[0].ToString();

                    //====================================================================================

                    //====================================================================================
                    //              SAVE CO MAKERS
                    //====================================================================================
                    if (dataGridView1.Rows.Count >= 1)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells[0].Value != null)
                            {
                                SqlCommand cmdCo = new SqlCommand();
                                cmdCo.Connection = con;
                                cmdCo.CommandType = CommandType.StoredProcedure;
                                cmdCo.CommandText = "sp_InsertLoanCoMakers";
                                cmdCo.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                                cmdCo.Parameters.AddWithValue("@Co_Maker_ID", row.Cells[0].Value);
                                cmdCo.Parameters.AddWithValue("@Co_Maker_EmployeeID", row.Cells[1].Value);
                                cmdCo.ExecuteNonQuery(); //SAVE CO MAKER
                            }
                        }
                    }

                    //====================================================================================
                    //              IF PDC OR PAYROLL PAYMENT
                    //====================================================================================
                    if (cmbPaymentOption.Text == "PDC")
                    {
                        //Generate Loan Details for PDC

                        //====================================================================================
                        //              CREATE LOAN DETAILS FOR [PAYMENT || INTEREST || PRINCIPAL]
                        //====================================================================================
                        //CREATING LOAN DETAILS
                        double PV = Convert.ToDouble(txtLoanAmount.Text.Replace(",", ""));
                        double rate = Convert.ToDouble(txtInterest.Text);
                        double term = Convert.ToDouble(txtTermsInMonth.Text);
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

                        string str = clsParameter.returnBonusLoanDate(cmbLoanType.SelectedValue.ToString(), txtDateEncoded);
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
                        if(clsParameter.deductionMonth(cmbLoanType.SelectedValue.ToString()) != 0)
                        {
                            month = month + clsParameter.deductionMonth(cmbLoanType.SelectedValue.ToString());
                        }


                        if (day <= 15)
                        {
                            if (month == 2)
                            {
                                day = 28;
                            }
                            else
                            {
                                day = 30;
                            }
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

                        int cnt = Convert.ToInt32(txtTermsInMonth.Text);
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
                            cmdDetail1.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
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
                        double PV = Convert.ToDouble(txtLoanAmount.Text.Replace(",", ""));
                        double rate = Convert.ToDouble(txtInterest.Text);
                        double term = Convert.ToDouble(txtTermsInMonth.Text);
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

                        string str = clsParameter.returnBonusLoanDate(cmbLoanType.SelectedValue.ToString(), txtDateEncoded);
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
                        if (clsParameter.deductionMonth(cmbLoanType.SelectedValue.ToString()) != 0)
                        {
                            month = month + clsParameter.deductionMonth(cmbLoanType.SelectedValue.ToString());
                        }

                        int cnt = Convert.ToInt32(txtTermsInMonth.Text);
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
                                    day = 30;
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
                                    if (month == 2)
                                    {
                                        day = 28;
                                    }
                                    else
                                    {
                                        day = 30;
                                    }
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
                            cmdDetail1.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
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
                                    day = 30;
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
                                    if (month == 2)
                                    {
                                        day = 28;
                                    }
                                    else
                                    {
                                        day = 30;
                                    }
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
                            cmd1.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
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

                    //===============================================================================
                    //              FUNCTIONS AFTER SAVING
                    //===============================================================================

                    //==================================================
                    //          REFRESH THE GRID FROM MAIN
                    //==================================================
                    try
                    {
                        loans = (Loans)Application.OpenForms["Loans"];
                        loans.refreshData();
                    }
                    catch
                    {

                    }
                    Alert.show("Loan No. "+ txtLoanNo.Text +" successfully created.", Alert.AlertType.success);

                    //===========================
                    //  RETURN STATUTS
                    //===========================
                    SqlDataAdapter adapterStatus = new SqlDataAdapter("SELECT [Status] FROM Loan WHERE Loan_No ='" + txtLoanNo.Text + "'", con);
                    DataTable dtStat = new DataTable();
                    adapterStatus.Fill(dtStat);

                    if (dtStat.Rows.Count > 0)
                    {
                        status.Text = clsLoanDataEntry.returnStatusDescription(Convert.ToInt32(dtStat.Rows[0].ItemArray[0].ToString()));
                        status.Visible = true;
                    }

                    btnSave.Text = "NEW";
                    btnClose.Text = "CLOSE";
                    btnSearch.Enabled = false;
                    button1.Enabled = false;
                    txtLoanAmount.Enabled = false;
                    txtTermsInMonth.Enabled = false;
                    btnForward.Enabled = true;
                    btnForward.Visible = true;


                }
                else if (btnSave.Text == "UPDATE")
                {
                    //========================================================================================
                    //                      UPDATING OF LOAN HEADER
                    //========================================================================================

                    //Test First if have a Loan No
                    if (txtLoanNo.Text == "")
                    {
                        Alert.show("Loan No is required!", Alert.AlertType.error);
                        return;
                    }


                    if (Classes.clsLoanDataEntry.loan_amount != Convert.ToDecimal(txtLoanAmount.Text))
                    {
                        string msg = Environment.NewLine + "Are you sure you want to change loan amount";
                        msg = msg + Environment.NewLine + "FROM P" + Classes.clsLoanDataEntry.loan_amount.ToString("#,0.00") + " TO P" + txtLoanAmount.Text + "?";
                        DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.No)
                        {
                            txtLoanAmount.Text = Classes.clsLoanDataEntry.loan_amount.ToString("#,0.00");
                            txtLoanAmount_Validated(sender, e); //Recall for the monthly and semi amort
                            return;
                        }
                    }

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_UpdateLoanHeader";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                    cmd.Parameters.AddWithValue("@Payment_Option", cmbPaymentOption.Text);
                    cmd.Parameters.AddWithValue("@Loan_Amount", txtLoanAmount.Text.Replace(",", ""));
                    cmd.Parameters.AddWithValue("@Terms", txtTermsInMonth.Text);
                    cmd.Parameters.AddWithValue("@Monthly_Amort", txtMonthlyAmort.Text.Replace(",", ""));
                    cmd.Parameters.AddWithValue("@Semi_Monthly_Amort", txtSemiMonthlyAmort.Text.Replace(",", ""));
                    cmd.Parameters.AddWithValue("@ReleaseOption", cmbReleaseOption.Text);
                    cmd.ExecuteNonQuery();

                    //=========================================================================================
                    //                      END UPDATING HEADER
                    //=========================================================================================


                    //===================================================
                    //           UPDATING CO MAKERS
                    //===================================================

                    //DELETE FIRST THEN SAVE IT AGAIN
                    SqlCommand cmdDelete = new SqlCommand();
                    cmdDelete.Connection = con;
                    cmdDelete.CommandText = "DELETE [Loan_Co-Maker] WHERE Loan_No = '" + txtLoanNo.Text + "'";
                    cmdDelete.CommandType = CommandType.Text;
                    cmdDelete.ExecuteNonQuery();

                    if (dataGridView1.Rows.Count >= 1)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells[0].Value != null)
                            {
                                SqlCommand cmdCo = new SqlCommand();
                                cmdCo.Connection = con;
                                cmdCo.CommandType = CommandType.StoredProcedure;
                                cmdCo.CommandText = "sp_InsertLoanCoMakers";
                                cmdCo.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                                cmdCo.Parameters.AddWithValue("@Co_Maker_ID", row.Cells[0].Value);
                                cmdCo.Parameters.AddWithValue("@Co_Maker_EmployeeID", row.Cells[1].Value);
                                cmdCo.ExecuteNonQuery(); //SAVE CO MAKERS
                            }
                        }
                    }

                    //===================================================
                    //           END CO MAKERS
                    //===================================================


                    //===================================================
                    //          CREATE LOAN DETAILS AGAIN
                    //===================================================

                    //DELETE FIRST THEN SAVE AGAIN
                    SqlCommand cmdDetail = new SqlCommand();
                    cmdDetail.Connection = con;
                    cmdDetail.CommandText = "DELETE Loan_Details WHERE Loan_No = '" + txtLoanNo.Text + "'";
                    cmdDetail.CommandType = CommandType.Text;
                    cmdDetail.ExecuteNonQuery();

                    //====================================================================================
                    //              IF PDC OR PAYROLL PAYMENT
                    //====================================================================================
                    if (cmbPaymentOption.Text == "PDC")
                    {
                        //Generate Loan Details for PDC

                        //====================================================================================
                        //              CREATE LOAN DETAILS FOR [PAYMENT || INTEREST || PRINCIPAL]
                        //====================================================================================
                        //CREATING LOAN DETAILS
                        double PV = Convert.ToDouble(txtLoanAmount.Text.Replace(",", ""));
                        double rate = Convert.ToDouble(txtInterest.Text);
                        double term = Convert.ToDouble(txtTermsInMonth.Text);
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



                        string str = clsParameter.returnBonusLoanDate(cmbLoanType.SelectedValue.ToString(), txtDateEncoded);
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
                        if (clsParameter.deductionMonth(cmbLoanType.SelectedValue.ToString()) != 0)
                        {
                            month = month + clsParameter.deductionMonth(cmbLoanType.SelectedValue.ToString());
                        }


                        if (day <= 15)
                        {
                            if (month == 2)
                            {
                                day = 28;
                            }
                            else
                            {
                                day = 30;
                            }
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

                        int cnt = Convert.ToInt32(txtTermsInMonth.Text);
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
                            cmdDetail1.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
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
                        double PV = Convert.ToDouble(txtLoanAmount.Text.Replace(",", ""));
                        double rate = Convert.ToDouble(txtInterest.Text);
                        double term = Convert.ToDouble(txtTermsInMonth.Text);
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

                        string str = clsParameter.returnBonusLoanDate(cmbLoanType.SelectedValue.ToString(), txtDateEncoded);
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
                        if (clsParameter.deductionMonth(cmbLoanType.SelectedValue.ToString()) != 0)
                        {
                            month = month + clsParameter.deductionMonth(cmbLoanType.SelectedValue.ToString());
                        }

                        int cnt = Convert.ToInt32(txtTermsInMonth.Text);
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
                                    day = 30;
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
                                    if (month == 2)
                                    {
                                        day = 28;
                                    }
                                    else
                                    {
                                        day = 30;
                                    }
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
                            cmdDetail1.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
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
                                    day = 30;
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
                                    if (month == 2)
                                    {
                                        day = 28;
                                    }
                                    else
                                    {
                                        day = 30;
                                    }
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
                            cmd1.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
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

                    //===============================================================================
                    //              FUNCTIONS AFTER SAVING
                    //===============================================================================

                    //==================================================
                    //          REFRESH THE GRID FROM MAIN
                    //==================================================
                    try
                    {
                        loans = (Loans)Application.OpenForms["Loans"];
                        loans.refreshData();
                    }
                    catch
                    {

                    }
                    Alert.show("Successfully updated.", Alert.AlertType.success);

                    //Put the updated amount 
                    Classes.clsLoanDataEntry.loan_amount = Convert.ToDecimal(txtLoanAmount.Text);

                    //CHange button TEXT
                    btnSave.Text = "EDIT";
                    btnClose.Text = "CLOSE";
                    button1.Enabled = false;
                    txtLoanAmount.Enabled = false;
                    txtTermsInMonth.Enabled = false;
                    btnForward.Enabled = true;
                    cmbPaymentOption.Enabled = false;
                    cmbReleaseOption.Enabled = false;

                }
                else if (btnSave.Text == "EDIT")
                {
                    btnSave.Text = "UPDATE";
                    btnClose.Text = "CANCEL";
                    if (txtCompany.Text != "NON PAYROLL")
                    {
                        button1.Enabled = true;
                        cmbPaymentOption.Enabled = true;
                        cmbReleaseOption.Enabled = true;
                    }
                    else
                    {
                        cmbPaymentOption.Enabled = false;
                        cmbReleaseOption.Enabled = false;
                    }
                    txtLoanAmount.Enabled = true;
                    txtTermsInMonth.Enabled = true;
                    btnForward.Enabled = false;


                }
                else
                {
                    clearFieldsForCancel();

                    //For New 
                    button1.Enabled = true;
                    btnSearch.Enabled = true;
                    cmbPaymentOption.Enabled = true;

                    //Change button From NEw = Save
                    btnSave.Text = "SAVE";
                    btnClose.Text = "CANCEL";
                    cmbLoanType.Enabled = true;
                    btnCancel.Visible = false;
                    lblReason.Visible = false;
                    txtCancel.Visible = false;
                }
            }
        }

        private void txtLoanAmount_Leave(object sender, EventArgs e)
        {
            if(txtLoanAmount.Text != "")
            {
                try
                {
                    txtLoanAmount.Text = Convert.ToDecimal(txtLoanAmount.Text).ToString("#,0.00");
                }
                catch
                {

                }   
            }
        }

        private void txtCompany_TextChanged(object sender, EventArgs e)
        {
            if(btnSave.Text == "SAVE")
            {
                if (txtCompany.Text == "NON PAYROLL")
                {
                    cmbPaymentOption.SelectedIndex = 1;
                    cmbPaymentOption.Enabled = false;
                }
                else
                {
                    cmbPaymentOption.SelectedIndex = -1;
                    cmbPaymentOption.Enabled = true;
                }
            }
        }

        private void txtLoanAmount_TextChanged(object sender, EventArgs e)
        {
            if(cmbLoanType.Text != "")
            {
                if(txtLoanAmount.Text != "")
                {
                    try
                    {
                        clsLoanDataEntry.checkIfGreaterThanLoanableAmount(txtLoanAmount, cmbLoanType.SelectedValue.ToString());
                    }
                    catch
                    {

                    }
                    
                }

                if (txtLoanAmount.Text != "" && txtTermsInMonth.Text != "")
                {
                    try
                    {
                        compute(Convert.ToDouble(txtLoanAmount.Text.Replace(",", "")), Convert.ToDouble(txtInterest.Text), Convert.ToDouble(txtTermsInMonth.Text.Replace(",", "")));
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void txtTermsInMonth_TextChanged(object sender, EventArgs e)
        {
            if(cmbLoanType.Text != "")
            {
                if (txtLoanAmount.Text != "" && txtTermsInMonth.Text != "")
                {
                    try
                    {
                        compute(Convert.ToDouble(txtLoanAmount.Text.Replace(",", "")), Convert.ToDouble(txtInterest.Text), Convert.ToDouble(txtTermsInMonth.Text.Replace(",", "")));
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void txtLoanAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void txtTermsInMonth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "5")
            {
                //DISAPPROVED
                Alert.show("This Loan already Released!", Alert.AlertType.error);
                return;
            }

            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "7")
            {
                //DISAPPROVED
                Alert.show("Loan already cancelled.", Alert.AlertType.error);
                return;
            }

            string msg = Environment.NewLine + "Are you sure you want to cancel this loan?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (txtCancel.Text == "")
                {
                    Alert.show("Reason for cancellation of loan is required.", Alert.AlertType.error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "UPDATE LOAN Set Status = '7',Cancelled_Date = GETDATE(), Cancelled_By = '" + Classes.clsUser.Username + "' ,Note = '" + txtCancel.Text + "' WHERE Loan_No ='" + txtLoanNo.Text + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();

                    Alert.show("Loan " + txtLoanNo.Text + " successfully cancelled.", Alert.AlertType.success);

                    status.Text = "CANCELLED";
                    //Refresh Grid
                    try
                    {
                        Loans frm = new Loans();
                        frm = (Loans)Application.OpenForms["Loans"];
                        frm.refreshData();
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void cmbReleaseOption_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbReleaseOption.Text != "")
            {
                if(cmbReleaseOption.Text == "ATM")
                {
                    clsLoan.invalidBankAccount(Classes.clsLoanDataEntry.userID);
                }
            }
        }
    }
}
