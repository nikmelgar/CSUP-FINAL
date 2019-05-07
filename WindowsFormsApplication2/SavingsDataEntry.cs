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
    public partial class SavingsDataEntry : Form
    {
        public SavingsDataEntry()
        {
            InitializeComponent();
        }

        string slip;
        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        Classes.clsLookUp clsLookUp = new Classes.clsLookUp();
        Classes.clsSavings clsSavings = new Classes.clsSavings();
        Classes.clsOpenTransaction clsOpen = new Classes.clsOpenTransaction();

        Savings savings = new Savings();


        SqlConnection con = new SqlConnection();
        Global global = new Global();
        Classes.clsSavingsDataEntry clsSavingsDataEntry = new Classes.clsSavingsDataEntry();
        Classes.clsHoldAccounts clsHoldAccount = new Classes.clsHoldAccounts();
        Classes.clsParameter clsParam = new Classes.clsParameter();

        private void button5_Click(object sender, EventArgs e)
        {
            /*
            *   05/06/2019
            *   as per maam vangie request
            *   Remove  the dialog box once button clicked
            */

            //if (status.Text == "FOR RELEASE")
            //{
            //    if(txtEmployeeID.Text != "" || txtAmountWithdrawn.Text != "")
            //    {
            //        string msg = Environment.NewLine + "Are you sure you want to Cancel this Entry?";
            //        DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //        if (result == DialogResult.Yes)
            //        {

            //            //============================================================
            //            //              REMOVE OPEN FORM
            //            //============================================================
            //            clsOpen.deleteTransaction("Savings", txtWithdrawalSlipNo.Text);

            //            this.Close();
            //        }
            //        else
            //        {
            //            return;
            //        }
            //    }
            //}
            //else
            //{
            //    //============================================================
            //    //              REMOVE OPEN FORM
            //    //============================================================
            //    clsOpen.deleteTransaction("Savings", txtWithdrawalSlipNo.Text);

            //    this.Close();
            //}


            //============================================================
            //              REMOVE OPEN FORM
            //============================================================
            clsOpen.deleteTransaction("Savings", txtWithdrawalSlipNo.Text);

            this.Close();
        }

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

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if(radioCheque.Checked == true)
            {
                cmbBankName.Enabled = true;
                txtCheque.Enabled = true;
                btnRelease.Enabled = true;
                btnRelease.Text = "PREPARE CHEQUE";

                //loadBank
                clsSavingsDataEntry.loadComboBox(cmbBankName);
            }
            else
            {
                cmbBankName.Enabled = false;
                txtCheque.Enabled = false;
                cmbBankName.Text = "";
                txtCheque.Text = "";
                cmbBankName.DataSource = null;
                btnRelease.Text = "RELEASE";
            }

            txtAmountWithdrawn.Focus();
        }

        private void radioCash_CheckedChanged(object sender, EventArgs e)
        {
            txtAmountWithdrawn.Focus();
            btnRelease.Enabled = true;
            btnRelease.Text = "RELEASE";
        }

        private void txtAmountWithdrawn_TextChanged(object sender, EventArgs e)
        {
            try
            {

                ////Check function if exceeded to the withdrawable amount
                //Decimal remaining, Total;
                //remaining = 100;
                //if(Convert.ToDecimal(txtAmountWithdrawn.Text) >= Convert.ToDecimal(txtWithdrawalBalance.Text))
                //{
                //    MessageBox.Show("Exceeds allowed maintaining balance of Php 100.00");
                //    txtAmountInWords.Text = "";
                //    return;
                //}
                //else if(Convert.ToDecimal(txtAmountWithdrawn.Text) < Convert.ToDecimal(txtWithdrawalBalance.Text))
                //{
                //    Total = Convert.ToDecimal(txtAmountWithdrawn.Text) + remaining;
                //    if(Total > Convert.ToDecimal(txtWithdrawalBalance.Text))
                //    {
                //        MessageBox.Show("Exceeds allowed maintaining balance of Php 100.00");
                //        txtAmountInWords.Text = "";
                //        return;
                //    }
                //}


                string num = txtAmountWithdrawn.Text;


                if (txtAmountWithdrawn.Text == "")
                {
                    txtAmountInWords.Text = "";
                }
                else
                {
                    decimal number = decimal.Parse(num.ToString());

                    if (number.ToString() == "0")
                    {
                        MessageBox.Show("The number in currency fomat is \nZero Only");
                    }
                    else
                    {
                        txtAmountInWords.Text = Classes.clsSavingsDataEntry.ConvertToWords(number.ToString());
                    }
                }


                Console.ReadKey();
            }
            catch (System.Exception ex)
            {


            }
        }

        private void txtAmountWithdrawn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&(e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(LookUp))
                {
                    form.Activate();
                    Classes.clsLookUp.whosLookUp = "0";
                    return;
                }
            }

            Classes.clsLookUp.whosLookUp = "0";
            LookUp LookUp = new LookUp();
            LookUp.Show();
        }

        public string returnMode()
        {
            if (radioCash.Checked == true)
            {
                return "CA";
            }
            else if (radioCheque.Checked == true)
            {
                return "CH";
            }
            else
            {
                return "AT";
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {

            if(btnSave.Text == "SAVE" || btnSave.Text == "UPDATE")
            {
                //Check if theres a employeeid and amount
                if (txtEmployeeID.Text == "" || txtAmountWithdrawn.Text == "")
                {
                    Alert.show("All fields with (*) are required.", Alert.AlertType.warning);
                    return;
                }

                //Check if already resigned member
                if(global.checkMemberIfResignedTRUE(Classes.clsSavingsDataEntry.userID) == true)
                {
                    Alert.show("This Member is Already Resigned!", Alert.AlertType.error);
                    return;
                }
                //Cash limit for 50k
                if (returnMode() == "CA")
                {
                    //Put the Param here
                    if (Convert.ToDecimal(txtAmountWithdrawn.Text) > clsParam.cashLimit())
                    {
                        Alert.show("Exceeds maximum daily cash withdrawal of "+ clsParam.cashLimit().ToString("#,0.00") +"!", Alert.AlertType.warning);
                        return;
                    }
                }

                if (returnMode() == "CH")
                {
                    if (txtCheque.Text == "")
                    {
                        Alert.show("Cheque Number is required!", Alert.AlertType.warning);
                        txtCheque.Focus();
                        return;
                    }
                }

                if(returnMode() == "AT")
                {
                    //Check if BPI BDO MBTC
                    if(txtBankCode.Text != "BDO" && txtBankCode.Text != "BPI" && txtBankCode.Text != "MBTC")
                    {
                        Alert.show("Members Bank is Invalid!", Alert.AlertType.error);
                        return;
                    }
                }

                //Check function if exceeded to the withdrawable amount
                Decimal Total;
                if (Convert.ToDecimal(txtAmountWithdrawn.Text) >= Convert.ToDecimal(txtWithdrawalBalance.Text))
                {
                    Alert.show("Exceeds minimum maintaining balance of Php " + clsParam.remainingBalance().ToString("#,0.00"), Alert.AlertType.warning);
                    txtAmountInWords.Text = "";
                    return;
                }
                else if (Convert.ToDecimal(txtAmountWithdrawn.Text) < Convert.ToDecimal(txtWithdrawalBalance.Text))
                {
                    Total = Convert.ToDecimal(txtAmountWithdrawn.Text) + clsParam.remainingBalance();
                    if (Total > Convert.ToDecimal(txtWithdrawalBalance.Text))
                    {
                        Alert.show("Exceeds minimum maintaining balance of Php " + clsParam.remainingBalance().ToString("#,0.00"), Alert.AlertType.warning);
                        txtAmountInWords.Text = "";
                        return;
                    }
                }


                //Check for hold accounts
                if(clsHoldAccount.checkIfHoldAccount(Classes.clsSavingsDataEntry.userID) == true)
                {
                    Alert.show("Member's account is on hold.", Alert.AlertType.error);
                    return;
                }

                if (clsHoldAccount.checkIfTHeresADependent(txtEmployeeID.Text) == true)
                {
                    Alert.show("Member's account is on hold.", Alert.AlertType.error);
                    return;
                }
                //for dependent purposes


                //END VALIDATION FIRST ============================================================
            }

            //Save or Update Function
            if (btnSave.Text == "SAVE")
            {
                //Check for withdrawal limit For Saving
                if (clsSavingsDataEntry.getCountWithdrawal() == true)
                {
                    Alert.show("Exceed allowed withdrawal transaction per day.", Alert.AlertType.error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_InsertWithdrawal";
                    cmd.Parameters.AddWithValue("@userID", Classes.clsSavingsDataEntry.userID);
                    cmd.Parameters.AddWithValue("@EmployeeID", txtEmployeeID.Text);
                    cmd.Parameters.AddWithValue("@wdDate", dtDateWithdrawal.Text);
                    cmd.Parameters.AddWithValue("@Balance_Before_Withdrawal", Convert.ToDecimal(txtCurrentBalanceBeforeWithdrawal.Text));
                    cmd.Parameters.AddWithValue("@Withdrawal_Mode", returnMode());
                    cmd.Parameters.AddWithValue("@AmtWithdrawn", Convert.ToDecimal(txtAmountWithdrawn.Text));


                    if (cmbBankName.Text == "")
                    {
                        cmd.Parameters.AddWithValue("@Bank_Code", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Bank_Code", cmbBankName.SelectedValue);
                    }

                    cmd.Parameters.AddWithValue("@Check_No", txtCheque.Text);
                    cmd.Parameters.AddWithValue("@Prepared_By", txtPreparedBy.Text);
                    if (radioPerea.Checked == true)
                    {
                        cmd.Parameters.AddWithValue("@WDFrom", '1');
                    }
                    else if (radioDansalan.Checked == true)
                    {
                        cmd.Parameters.AddWithValue("@WDFrom", '0');
                    }

                    cmd.ExecuteNonQuery();

                    Alert.show("Successfully Added.", Alert.AlertType.success);

                    //Get Withdrawal Slip Number
                    txtWithdrawalSlipNo.Text = clsSavingsDataEntry.returnWithdrawalSlipNo();

                    if (clsSavingsDataEntry.returnReleaseDate(txtWithdrawalSlipNo.Text).ToString() == "" || clsSavingsDataEntry.returnReleaseDate(txtWithdrawalSlipNo.Text).ToString() == string.Empty)
                    {
                        dtReleaseDate.Text = "";
                    }

                    //refresh
                    try
                    {
                        savings = (Savings)Application.OpenForms["Savings"];
                        savings.refreshData();

                    }
                    catch
                    {

                    }
                    //Visible Status
                    status.Visible = true;
                    status.Text = "FOR RELEASE";


                    //Lock for this user
                    //Insert here for register the open form and reference
                    clsOpen.insertTransaction("Savings", txtWithdrawalSlipNo.Text);


                    btnSave.Text = "NEW";
                }
            }
            else if(btnSave.Text == "UPDATE") //UPDATE
            {
                //=============================================================
                //              UPDATE CODE
                //=============================================================
                if (status.Text == "POSTED")
                {
                    Alert.show("This Withdrawal Already Posted!", Alert.AlertType.warning);
                    return;
                }

                if (dtReleaseDate.Text != "")
                {
                    //If withdrawal already released cannot update cancel first and recreate
                    Alert.show("Withdrawal Already been Released!", Alert.AlertType.error);
                    return;
                }
                if (txtCancelledBy.Text != "")
                {
                    Alert.show("Withdrawal Already been Cancelled!", Alert.AlertType.error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_UpdateWithdrawalAmout";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Withdrawal_Slip_No", txtWithdrawalSlipNo.Text);
                    cmd.Parameters.AddWithValue("@AmtWithdrawn", Convert.ToDecimal(txtAmountWithdrawn.Text));

                    if (cmbBankName.Text == "")
                    {
                        cmd.Parameters.AddWithValue("@Bank_Code", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Bank_Code", cmbBankName.SelectedValue);
                    }
                    cmd.Parameters.AddWithValue("@check_no", txtCheque.Text);

                    cmd.ExecuteNonQuery();
                }
                //refresh
                savings = (Savings)Application.OpenForms["Savings"];
                savings.refreshData();

                Alert.show("Successfully updated.", Alert.AlertType.success);

            }
            else
            {

                //Remove Lock In
                clsOpen.deleteTransaction("Savings", txtWithdrawalSlipNo.Text);

                //FOR NEW Button
                clearFields();
                btnSave.Text = "SAVE";
                status.Text = "";
                dtDateWithdrawal.Text = DateTime.Today.Date.ToShortDateString();

            }
    

        }

        public void btnRelease_Click(object sender, EventArgs e)
        {
            if(txtWithdrawalSlipNo.Text == "")
            {
                Alert.show("No withdrawal for release.", Alert.AlertType.error);
                return;
            }

            if (status.Text == "POSTED")
            {
                Alert.show("This Withdrawal Already Posted!", Alert.AlertType.warning);
                return;
            }

            if (txtCancelledBy.Text != "")
            {
                Alert.show("This Withdrawal Already Cancelled!", Alert.AlertType.error);
                return;
            }

            if (dtReleaseDate.Text != "")
            {
                Alert.show("This Withdrawal Already Released!", Alert.AlertType.error);
                return;
            }

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                if (radioCheque.Checked == true)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT CV_No FROM Withdrawal_Slip WHERE Withdrawal_Slip_No = '" + txtWithdrawalSlipNo.Text + "'", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows[0].ItemArray[0].ToString() != "")
                    {

                    }
                }


                string msg = Environment.NewLine + "Are you sure you want to release this?";
                DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (btnRelease.Text == "RELEASE")
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "sp_ReleaseWithdrawalDate";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Withdrawal_Slip_No", txtWithdrawalSlipNo.Text);
                        cmd.ExecuteNonQuery();

                        //Put the date to the textbox
                        dtReleaseDate.Text = Convert.ToDateTime(clsSavingsDataEntry.returnReleaseDate(txtWithdrawalSlipNo.Text)).ToShortDateString();

                        status.Visible = true;
                        status.Text = "RELEASED";

                        //refresh
                        savings = (Savings)Application.OpenForms["Savings"];
                        savings.refreshData();

                        btnSave.Text = "NEW";

                        //=================================================================================
                        //MessageBox Before Showing Disbursement
                        //=================================================================================
                        this.Close();
                        Alert.show("Successfully Released!", Alert.AlertType.success);
                    }

                    //CHECK IF WITHDRAWABLE METHOD IS CHECK 
                    //IF CHECK GO TO DISBURSEMENT AND SAVE THE RELEASED VALUE
                    //===========================================================================================================

                    if (radioCheque.Checked == true)
                    {
                        //Put true for disbursement posting if withdrawal from check
                        Classes.clsDisbursement.releaseCashWithdrawal = true;
                        Classes.clsDisbursement.slipFromWithdrawal = txtWithdrawalSlipNo.Text;

                        //=================================================================================
                        //Call Disbursement
                        //=================================================================================

                        DisbursementVoucher cv = new DisbursementVoucher();
                        //03-10-2019
                        cv.ForReplenishment();
                        cv.fromWithdrawal = true;

                        cv.dataGridView1.Rows.Add(1);


                        cv.getTransaction = "TRAN001";
                        cv.getBankFromWithdrawal = cmbBankName.SelectedValue.ToString();
                        Classes.clsDisbursement.userID = Classes.clsSavingsDataEntry.userID;
                        cv.txtChequeNo.Text = txtCheque.Text;
                        cv.txtAmount.Text = txtAmountWithdrawn.Text;
                        slip = txtWithdrawalSlipNo.Text;
                        slip = slip.Substring(12, 4);
                        cv.txtParticular.Text = "Withdrawal of Savings per Slip receipt no. " + slip + " Dated : " + DateTime.Now.ToShortDateString();
                        cv.radioMember.Checked = true;
                        cv.txtPayee.Text = txtEmployeeID.Text;
                        cv.txtPayeeName.Text = txtName.Text;

                        cv.dataGridView1.Rows[0].Cells[0].Value = "300.1";
                        cv.dataGridView1.Rows[0].Cells[3].Value = txtAmountWithdrawn.Text;
                        //Put the Subsidiary or the Member ID and Name in Disbursement Details
                        cv.dataGridView1.Rows[0].Cells[1].Value = txtEmployeeID.Text + " - " + txtName.Text;
                        cv.dataGridView1.Rows[0].Cells[5].Value = Classes.clsSavingsDataEntry.userID;

                        int rowId = cv.dataGridView1.Rows.Add();
                        // Grab the new row!
                        DataGridViewRow row2 = cv.dataGridView1.Rows[rowId];

                        //Added March 10 , 2019 as Per Sir Manny Request For Cheque Release
                        SqlDataAdapter adapterBank = new SqlDataAdapter("SELECT Bank_Account_Code FROM Bank WHERE Bank_Code ='" + cmbBankName.SelectedValue.ToString() + "'", con);
                        DataTable dtBank = new DataTable();
                        adapterBank.Fill(dtBank);


                        // Add the data
                        row2.Cells[0].Value = dtBank.Rows[0].ItemArray[0].ToString();
                        row2.Cells[4].Value = txtAmountWithdrawn.Text;


                        cv.btnEdit.Enabled = false;
                        cv.btnPost.Enabled = true;
                        cv.btnCancel.Enabled = true;
                        cv.btnPrint.Enabled = true;
                        cv.btnPrintCheque.Enabled = true;
                        cv.btnSearch.Enabled = false;
                        cv.dataGridView1.Enabled = false;
                        cv.txtChequeNo.Enabled = false;
                        cv.dtChequeDate.Enabled = false;
                        cv.txtAmount.Enabled = false;
                        cv.txtParticular.Enabled = false;
                        cv.btnSearchPayee.Enabled = false;
                        cv.btnSearchLoan.Enabled = false;
                        cv.btnNew.Enabled = false;

                        cv.btnNew.Text = "NEW";



                        //BEFORE NEXT WINDOW REMOVE THE OPEN TRANSACTION
                        //============================================================
                        //              REMOVE OPEN FORM
                        //============================================================
                        clsOpen.deleteTransaction("Savings", txtWithdrawalSlipNo.Text);

                        cv.Show();
                        //=================================================================================
                        //SAVING DISBURSEMENT
                        //=================================================================================
                        SqlCommand cmdHeader = new SqlCommand();
                        cmdHeader.Connection = con;
                        cmdHeader.CommandText = "sp_InsertDisbursementHeader";
                        cmdHeader.CommandType = CommandType.StoredProcedure;
                        cmdHeader.Parameters.AddWithValue("@CVDate", DateTime.Now.ToShortDateString());

                        //FOr Payee Type 
                        //Member = 0 Client = 1
                        if (cv.radioMember.Checked == true)
                        {
                            cmdHeader.Parameters.AddWithValue("@Payee_Type", "0");
                        }
                        else
                        {
                            cmdHeader.Parameters.AddWithValue("@Payee_Type", "1");
                        }

                        //Check if Theres a Member or Client
                        if (Classes.clsDisbursement.userID.ToString() == "")
                        {
                            cmdHeader.Parameters.AddWithValue("@userID", DBNull.Value);

                        }
                        else if (Classes.clsDisbursement.userID == 0)
                        {
                            cmdHeader.Parameters.AddWithValue("@userID", DBNull.Value);
                        }
                        else
                        {
                            cmdHeader.Parameters.AddWithValue("@userID", Classes.clsDisbursement.userID.ToString());
                        }

                        cmdHeader.Parameters.AddWithValue("@Payee", cv.txtPayee.Text);
                        cmdHeader.Parameters.AddWithValue("@Payee_Name", cv.txtPayeeName.Text);
                        cmdHeader.Parameters.AddWithValue("@Particulars", cv.txtParticular.Text);
                        cmdHeader.Parameters.AddWithValue("@Loan_No", cv.txtLoanNo.Text);
                        cmdHeader.Parameters.AddWithValue("@Bank_Code", cv.cmbBank.SelectedValue);
                        cmdHeader.Parameters.AddWithValue("@Check_No", cv.txtChequeNo.Text);
                        cmdHeader.Parameters.AddWithValue("@Check_Date", cv.dtChequeDate.Text);
                        cmdHeader.Parameters.AddWithValue("@Amount", Convert.ToDecimal(cv.txtAmount.Text));
                        cmdHeader.Parameters.AddWithValue("@Transaction_Type", cv.cmbTransaction.SelectedValue);
                        cmdHeader.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);
                        cmdHeader.ExecuteNonQuery();

                        //Get The CV NO.

                        SqlCommand cmdCV = new SqlCommand();
                        cmdCV.Connection = con;
                        cmdCV.CommandText = "sp_GetCVNoAfterSaving";
                        cmdCV.CommandType = CommandType.StoredProcedure;
                        cmdCV.Parameters.AddWithValue("@CV_Date", DateTime.Now.ToShortDateString());
                        cmdCV.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmdCV);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            cv.txtCVNo.Text = dt.Rows[0].ItemArray[0].ToString();
                        }
                        else
                        {
                            return;
                        }

                        //Insert CV Details
                        //SAVE DETAILS ============================================================================================
                        if (cv.dataGridView1.Rows.Count > 0)
                        {
                            foreach (DataGridViewRow row in cv.dataGridView1.Rows)
                            {
                                if (row.Cells[0].Value != null)
                                {
                                    SqlCommand cmdDetail = new SqlCommand();
                                    cmdDetail.Connection = con;
                                    cmdDetail.CommandText = "sp_InsertDisbursementDetail";
                                    cmdDetail.CommandType = CommandType.StoredProcedure;
                                    cmdDetail.Parameters.AddWithValue("@CV_No", cv.txtCVNo.Text);
                                    cmdDetail.Parameters.AddWithValue("@Account_Code", row.Cells[0].Value);

                                    if (Convert.ToInt32(row.Cells[5].Value) == 0)
                                    {
                                        cmdDetail.Parameters.AddWithValue("@userID", DBNull.Value);
                                    }
                                    else
                                    {
                                        cmdDetail.Parameters.AddWithValue("@userID", Convert.ToInt32(row.Cells[5].Value));
                                    }

                                    if (row.Cells[1].Value != null)
                                    {
                                        cmdDetail.Parameters.AddWithValue("@Subsidiary_Code", row.Cells[1].Value);
                                    }
                                    else
                                    {
                                        cmdDetail.Parameters.AddWithValue("@Subsidiary_Code", DBNull.Value);
                                    }


                                    if (row.Cells[2].Value != null)
                                    {
                                        cmdDetail.Parameters.AddWithValue("@Loan_No", row.Cells[2].Value);
                                    }
                                    else
                                    {
                                        cmdDetail.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                                    }

                                    cmdDetail.Parameters.AddWithValue("@Debit", Convert.ToDecimal(row.Cells[3].Value));
                                    cmdDetail.Parameters.AddWithValue("@Credit", Convert.ToDecimal(row.Cells[4].Value));
                                    cmdDetail.ExecuteNonQuery();
                                }
                            }
                        }

                        //=================================================================================
                        //                     SAVE WITHDRAWAL SLIP FOR CHANGING PURPOSES
                        //=================================================================================
                        SqlCommand cmdWD = new SqlCommand();
                        cmdWD.Connection = con;
                        cmdWD.CommandText = "UPDATE Disbursement_Header SET wd_slip_no = '" + txtWithdrawalSlipNo.Text + "' WHERE CV_No = '" + cv.txtCVNo.Text + "'";
                        cmdWD.CommandType = CommandType.Text;
                        cmdWD.ExecuteNonQuery();

                        //=================================================================================
                        //                     SAVE CV NO FOR WITHDRAWAL SLIP
                        //=================================================================================
                        SqlCommand cmdWDCV = new SqlCommand();
                        cmdWDCV.Connection = con;
                        cmdWDCV.CommandText = "UPDATE Withdrawal_Slip SET cv_no = '" + cv.txtCVNo.Text + "' WHERE Withdrawal_Slip_No = '" + txtWithdrawalSlipNo.Text + "'";
                        cmdWDCV.CommandType = CommandType.Text;
                        cmdWDCV.ExecuteNonQuery();

                        Alert.show("Withdrawal voucher successfully prepared.", Alert.AlertType.success);


                        SqlDataAdapter adapterchck = new SqlDataAdapter("SELECT CV_No FROM Withdrawal_Slip WHERE Withdrawal_Slip_No = '" + txtWithdrawalSlipNo.Text + "'", con);
                        DataTable dtchck = new DataTable();
                        adapterchck.Fill(dtchck);

                        status.Text = "FOR RELEASE - CV#" + dtchck.Rows[0].ItemArray[0].ToString();
                        status.Visible = true;
                        btnRelease.Enabled = false;

                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void SavingsDataEntry_Load(object sender, EventArgs e)
        {
            //GET DATE TODAY
            dtDateWithdrawal.Text = DateTime.Today.Date.ToShortDateString();
            dtReleaseDate.Text = DateTime.Today.Date.ToShortDateString();

            //Display 
            txtPreparedBy.Text = Classes.clsUser.Username;
            txtAmountWithdrawn.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //check if cancel note already fill up
            txtNote.ReadOnly = false;

            if (txtWithdrawalSlipNo.Text == "")
            {
                Alert.show("No withdrawal for cancellation.", Alert.AlertType.error);
                return;
            }

            
            if(status.Text == "CANCELLED")
            {
                Alert.show("This Withdrawal Already Cancelled!", Alert.AlertType.warning);
                return;
            }

            if(status.Text == "POSTED")
            {
                Alert.show("This Withdrawal Already Posted!", Alert.AlertType.warning);
                return;
            }

            if (txtNote.Text == "")
            {
                Alert.show("Cancel note is required!", Alert.AlertType.warning);
                txtNote.Focus();
                return;
            }

            string msg = Environment.NewLine + "Are you sure you want to cancel this?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_CancellationOfWithdrawal";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Withdrawal_Slip_No", txtWithdrawalSlipNo.Text);
                    cmd.Parameters.AddWithValue("@CancelledlNote", txtNote.Text);
                    cmd.Parameters.AddWithValue("@Cancelled_By", Classes.clsUser.Username);

                    cmd.ExecuteNonQuery();

                    if (txtCancelledBy.Text == "")
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT Cancelled_By FROM Withdrawal_Slip WHERE Withdrawal_Slip_No = '" + txtWithdrawalSlipNo.Text + "'", con);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        txtCancelledBy.Text = dt.Rows[0].ItemArray[0].ToString();
                    }
                }
                Alert.show("Successfully cancelled.", Alert.AlertType.success);

                status.Visible = true;
                status.Text = "CANCELLED";
                dtReleaseDate.Text = "";


                //refresh
                savings = (Savings)Application.OpenForms["Savings"];
                savings.refreshData();

                btnSave.Text = "NEW";
            }
            else
            {
                return;
            }

        }

        public void clearFields()
        {
            Control control = new Control();

            //=====================================================================
            //                      Members ID and Name
            //=====================================================================
            txtEmployeeID.Text = "";
            txtName.Text = "";

            //=====================================================================
            //                      Savings Account Information
            //=====================================================================
            foreach (var c in panel10.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
            }

            //=====================================================================
            //                      Withdrawal Transaction
            //=====================================================================
            foreach (var c in panel16.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
            }

            //=====================================================================
            //                    Bank Information
            //=====================================================================
            foreach (var c in panelCheckInformation.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
                if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
            }


            //Set Prepared By
            txtPreparedBy.Text = Classes.clsUser.Username;

            FieldControls(true, radioPerea, radioDansalan, radioCash, radioATM, radioCheque, txtAmountWithdrawn, cmbBankName, txtCheque);

            if(radioCheque.Checked == false)
            {
                cmbBankName.Enabled = false;
                txtCheque.Enabled = false;
            }

            btnSearch.Enabled = true;
            btnRelease.Enabled = true;

            //Status Remove
            status.Visible = false;
        }

        private void txtAmountWithdrawn_Leave(object sender, EventArgs e)
        {
            if(txtAmountWithdrawn.Text != "")
            {
                txtAmountWithdrawn.Text = Convert.ToDecimal(txtAmountWithdrawn.Text).ToString("#,0.00");
            }
        }

        private void txtCheque_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        //Disable Upon Edit Mode
        public void FieldControls(bool fldTrueFalse, RadioButton perea,RadioButton dansalan,RadioButton cash,RadioButton atm,RadioButton check,TextBox amount,ComboBox bank,TextBox checkNo)
        {
            perea.Enabled = fldTrueFalse;
            dansalan.Enabled = fldTrueFalse;
            cash.Enabled = fldTrueFalse;
            atm.Enabled = fldTrueFalse;
            check.Enabled = fldTrueFalse;
            amount.Enabled = fldTrueFalse;
            bank.Enabled = fldTrueFalse;
            checkNo.Enabled = fldTrueFalse;
        }

        private void radioATM_Click(object sender, EventArgs e)
        {
            txtAmountWithdrawn.Focus();
            btnRelease.Enabled = false;
        }

        private void radioPerea_Click(object sender, EventArgs e)
        {
            txtAmountWithdrawn.Focus();
        }

        private void radioDansalan_Click(object sender, EventArgs e)
        {
            txtAmountWithdrawn.Focus();
        }

        private void radioATM_CheckedChanged(object sender, EventArgs e)
        {
            btnRelease.Enabled = false;
        }
    }
}
