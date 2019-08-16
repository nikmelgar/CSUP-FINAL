using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2.Dashboard.DashboardControls
{
    public partial class Worklist : UserControl
    {
        public Worklist()
        {
            InitializeComponent();
        }

        DashboardClasses.clsWorklists clsWorklists = new DashboardClasses.clsWorklists();
        Classes.clsSearchJournal clsSearchJournal = new Classes.clsSearchJournal();
        Classes.clsSearchDisbursement clsDisbursement = new Classes.clsSearchDisbursement();
        Classes.clsSearchCashReceipt clsSearchCash = new Classes.clsSearchCashReceipt();
        Classes.clsCashReceipt clsCash = new Classes.clsCashReceipt();
        Classes.clsOpenTransaction clsOpenTransaction = new Classes.clsOpenTransaction();

        Global global = new Global();

        private void button1_Click(object sender, EventArgs e)
        {
            clsWorklists.loadJournalVoucher(lblCnt, dgvList);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clsWorklists.loadDisbursementVoucher(lblCnt, dgvList);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            clsWorklists.loadReceiptVoucher(lblCnt, dgvList);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            clsWorklists.loadAllVoucher(lblCnt, dgvList);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dgvList.SelectedRows.Count > 0)
            {
                switch (dgvList.SelectedRows[0].Cells["frm"].Value.ToString())
                {
                    case "JV":
                        //Check if open
                        if (clsOpenTransaction.checkOpenFormsAndTransaction("Journal Voucher", dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString()) == true)
                        {
                            //Messagebox here for open form with user whos using the form and reference
                            Alert.show(clsOpenTransaction.returnUserOnlineAndReference("Journal Voucher", dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString(), "Journal Voucher"), Alert.AlertType.error);
                            return;
                        }
                        else
                        {
                            //Insert here for register the open form and reference
                            clsOpenTransaction.insertTransaction("Journal Voucher", dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString());
                            useJournal();
                        }
                        break;
                    case "CV":
                        //Check if open
                        if (clsOpenTransaction.checkOpenFormsAndTransaction("Disbursement Voucher", dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString()) == true)
                        {
                            //Messagebox here for open form with user whos using the form and reference
                            Alert.show(clsOpenTransaction.returnUserOnlineAndReference("Disbursement Voucher", dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString(), "Disbursement Voucher"), Alert.AlertType.error);
                            return;
                        }
                        else
                        {
                            //Insert here for register the open form and reference
                            clsOpenTransaction.insertTransaction("Disbursement Voucher", dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString());
                            useDisbursement();
                        }
                        break;
                    case "OR":
                        //Check if open
                        if (clsOpenTransaction.checkOpenFormsAndTransaction("Receipt Voucher", dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString()) == true)
                        {
                            //Messagebox here for open form with user whos using the form and reference
                            Alert.show(clsOpenTransaction.returnUserOnlineAndReference("Receipt Voucher", dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString(), "Receipt Voucher"), Alert.AlertType.error);
                            return;
                        }
                        else
                        {
                            //Insert here for register the open form and reference
                            clsOpenTransaction.insertTransaction("Receipt Voucher", dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString());
                            useReceipt();
                        }
                        break;
                }
            }
        }


        //===================================================================
        //                  USE JOURNAL VOUCHER
        //===================================================================
        public void useJournal()
        {
            if (dgvList.SelectedRows.Count > 0)
            {                
                JournalVoucher jv = new JournalVoucher();

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Journal_Header WHERE JV_No = '"+ dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString() + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    if(ds.Tables[0].Rows.Count > 0)
                    {
                        //=========================================================================================
                        //                              Header Information
                        //=========================================================================================

                        jv.txtJVNumber.Text = dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString(); //JV_No
                        jv.txtMember.Text = ds.Tables[0].Rows[0]["AdjTo"].ToString();
                        jv.dtJVDate.Text = ds.Tables[0].Rows[0]["JV_Date"].ToString();

                        if (ds.Tables[0].Rows[0]["userID"].ToString() != "" || ds.Tables[0].Rows[0]["userID"].ToString() != string.Empty)
                        {
                            jv.txtName.Text = clsSearchJournal.fullName(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString()));
                        }
                        jv.txtParticulars.Text = ds.Tables[0].Rows[0]["Particulars"].ToString();
                        if (ds.Tables[0].Rows[0]["summarize"].ToString() == "True")
                        {
                            jv.checkBox1.Checked = true;
                        }
                        else
                        {
                            jv.checkBox1.Checked = false;
                        }

                        JournalVoucher.transactionFromTH = ds.Tables[0].Rows[0]["Transaction_Type"].ToString();
                        jv.txtLoanNumber.Text = ds.Tables[0].Rows[0]["Loan_No"].ToString();

                        //=========================================================================================
                        //                              Footer Information
                        //=========================================================================================

                        jv.txtPreparedBy.Text = ds.Tables[0].Rows[0]["Prepared_By"].ToString();
                        jv.txtPostedBy.Text = ds.Tables[0].Rows[0]["Posted_By"].ToString();  
                        jv.txtCancelled.Text = ds.Tables[0].Rows[0]["Cancelled_By"].ToString(); 
                        jv.txtAudited.Text = ds.Tables[0].Rows[0]["Audited_By"].ToString();

                        //=========================================================================================
                        //                              Status Information
                        //=========================================================================================

                        if (ds.Tables[0].Rows[0]["Posted"].ToString() == "True")
                        {
                            jv.status.Visible = true;
                            jv.status.Text = "POSTED";
                        }
                        else if (ds.Tables[0].Rows[0]["Cancelled"].ToString() == "True")
                        {
                            jv.txtParticulars.Text = dgvList.SelectedRows[0].Cells["Cancel_Note"].Value.ToString();
                            jv.status.Visible = true;
                            jv.status.Text = "CANCELLED";
                        }
                        else
                        {
                            jv.status.Visible = false;
                            jv.status.Text = "";
                        }


                        //=========================================================================================
                        //                              Details Information
                        //=========================================================================================

                        jv.dataGridView1.Rows.Clear();

                        //Check first if summarize or not
                        if (ds.Tables[0].Rows[0]["summarize"].ToString() == "True")
                        {
                            clsSearchJournal.loadDetailSummary(jv.dataGridView1, dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString());
                        }
                        else
                        {
                            clsSearchJournal.loadDetailsNotSummarize(jv.dataGridView1, dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString());
                        }

                        //=========================================================================================
                        //                              Compute
                        //=========================================================================================
                        clsSearchJournal.loadTotalDebitCredit(jv.txtDebit, jv.txtCredit, dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString());


                        //=========================================================================================
                        //                              Enable Buttons
                        //=========================================================================================
                        jv.btnEdit.Enabled = true;
                        jv.btnPost.Enabled = true;
                        jv.btnCancel.Enabled = true;
                        jv.btnPrint.Enabled = true;
                        jv.btnAuditted.Enabled = true;
                        jv.txtParticulars.BackColor = SystemColors.Control;

                        jv.fromTh = true;

                        jv.ShowDialog();
                    }
                }
            }
        }


        //===================================================================
        //                  USE DISBURSEMENT VOUCHER
        //===================================================================
        public void useDisbursement()
        {
            if (dgvList.SelectedRows.Count > 0)
            {
                DisbursementVoucher cv = new DisbursementVoucher();

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Disbursement_Header WHERE CV_No = '" + dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString() + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        //=========================================================================================
                        //                              Header Information
                        //=========================================================================================

                        cv.txtCVNo.Text = dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString();
                        cv.dtCVDate.Text = ds.Tables[0].Rows[0]["CV_Date"].ToString();


                        if (ds.Tables[0].Rows[0]["Payee_Type"].ToString() == "True")
                        {
                            cv.radioClient.Checked = true;
                        }
                        else
                        {
                            cv.radioMember.Checked = true;
                        }
                        cv.txtPayee.Text = ds.Tables[0].Rows[0]["Payee"].ToString();

                        if (ds.Tables[0].Rows[0]["userID"].ToString() != "" || ds.Tables[0].Rows[0]["userID"].ToString() != string.Empty)
                        {
                            Classes.clsDisbursement.userID = Convert.ToInt32(dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString());
                            cv.txtPayeeName.Text = clsDisbursement.fullName(Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString()));
                        }
                        else
                        {
                            cv.txtPayeeName.Text = clsDisbursement.ClientName(ds.Tables[0].Rows[0]["Payee"].ToString());
                        }



                        cv.txtLoanNo.Text = ds.Tables[0].Rows[0]["Loan_No"].ToString();

                        DisbursementVoucher.transactionFromTH = ds.Tables[0].Rows[0]["Transaction_Type"].ToString();
                        cv.cmbBank.SelectedValue = ds.Tables[0].Rows[0]["Bank_Code"].ToString();
                        cv.txtChequeNo.Text = ds.Tables[0].Rows[0]["Check_No"].ToString();
                        cv.dtChequeDate.Text = ds.Tables[0].Rows[0]["Check_Date"].ToString();
                        cv.txtAmount.Text = Convert.ToDecimal(ds.Tables[0].Rows[0]["Amount"].ToString()).ToString("#,0.00");
                        cv.txtParticular.Text = ds.Tables[0].Rows[0]["Particulars"].ToString();

                        //=========================================================================================
                        //                              Footer Information
                        //=========================================================================================

                        cv.txtPreparedBy.Text = ds.Tables[0].Rows[0]["Prepared_By"].ToString();
                        cv.txtPostedBy.Text = ds.Tables[0].Rows[0]["Posted_By"].ToString();
                        cv.txtCancelledBy.Text = ds.Tables[0].Rows[0]["Cancelled_By"].ToString();
                        cv.txtAuditedBy.Text = ds.Tables[0].Rows[0]["Audited_By"].ToString();

                        //=========================================================================================
                        //                              Status Information
                        //=========================================================================================

                        if (ds.Tables[0].Rows[0]["Posted"].ToString() == "True")
                        {
                            if (ds.Tables[0].Rows[0]["Released_By"].ToString() != "")
                            {
                                cv.status.Visible = true;
                                cv.status.Text = "POSTED and RELEASED";
                            }
                            else
                            {
                                cv.status.Visible = true;
                                cv.status.Text = "POSTED";
                            }

                        }
                        else if (ds.Tables[0].Rows[0]["Cancelled"].ToString() == "True")
                        {
                            cv.status.Visible = true;
                            cv.status.Text = "CANCELLED";
                        }
                        else
                        {
                            cv.status.Visible = false;
                            cv.status.Text = "";
                        }

                        //=========================================================================================
                        //                              Details Information
                        //=========================================================================================

                        cv.dataGridView1.Rows.Clear();
                        clsDisbursement.loadDetails(cv.dataGridView1, dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString());

                        //=========================================================================================
                        //                              Compute
                        //=========================================================================================
                        clsDisbursement.loadTotalDebitCredit(cv.txtDebit, cv.txtCredit, dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString());


                        //=========================================================================================
                        //                              Enable Buttons
                        //=========================================================================================
                        cv.btnEdit.Enabled = true;
                        cv.btnPost.Enabled = true;
                        cv.btnCancel.Enabled = true;
                        cv.btnPrint.Enabled = true;
                        cv.btnPrintCheque.Enabled = true;
                        cv.btnRelease.Enabled = true;
                        cv.btnAuditted.Enabled = true;

                        cv.fromTH = true;

                        cv.ShowDialog();
                    }
                }

            }
        }


        //===================================================================
        //                  USE RECEIPT VOUCHER
        //===================================================================
        public void useReceipt()
        {
            if (dgvList.SelectedRows.Count > 0)
            {
                CashReceiptVoucher or = new CashReceiptVoucher();

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Cash_Receipts_Header WHERE Or_No = '" + dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString() + "'", con);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        //=========================================================================================
                        //                              Header Information
                        //=========================================================================================

                        Classes.clsCashReceipt.userID = Convert.ToInt32(ds.Tables[0].Rows[0]["userID"].ToString());
                        or.txtORNo.Text = dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString();
                        or.dtOrDate.Text = ds.Tables[0].Rows[0]["Or_Date"].ToString();

                        if (ds.Tables[0].Rows[0]["Payor_Type"].ToString() == "True")
                        {
                            or.radioClient.Checked = true;
                            //Return Company Name According to ID
                            or.txtPayorName.Text = clsSearchCash.returnClientName(ds.Tables[0].Rows[0]["Payor"].ToString());
                        }
                        else
                        {
                            or.radioMember.Checked = true;
                            //Return Member Name According to ID
                            or.txtPayorName.Text = clsSearchCash.returnMembersName(ds.Tables[0].Rows[0]["userID"].ToString());
                            or.txtPayorCompany.Text = clsCash.returnCompanyDescription(clsSearchCash.GetCompanyPerMember(ds.Tables[0].Rows[0]["userID"].ToString()));
                        }

                        or.txtPayorID.Text = ds.Tables[0].Rows[0]["Payor"].ToString();
                        or.txtParticulars.Text = ds.Tables[0].Rows[0]["Particulars"].ToString();

                        //=========================================================================================
                        //                      COLLECTION TYPE
                        //=========================================================================================
                        //0 = cash
                        //1 = pecci check
                        //2 = non-pecci check
                        if (ds.Tables[0].Rows[0]["Collection_Type"].ToString() == "0")
                        {
                            or.radioCash.Checked = true;
                        }
                        else if (ds.Tables[0].Rows[0]["Collection_Type"].ToString() == "1")
                        {
                            or.radioPecciCheck.Checked = true;
                        }
                        else
                        {
                            or.radioNonPecciCheck.Checked = true;
                        }

                        //=========================================================================================
                        //                      Transaction Header
                        //=========================================================================================
                        clsSearchCash.loadTransaction(or.datagridviewTransaction, dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString());

                        //=========================================================================================
                        //                      Bank Details
                        //=========================================================================================
                        if (ds.Tables[0].Rows[0]["Collection_Type"].ToString() == "1" || ds.Tables[0].Rows[0]["Collection_Type"].ToString() == "2")
                        {
                            clsSearchCash.loadBanksCheck(or.dgvChecks, dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString());

                            //Disable Bank Grid and buttons
                            or.dgvChecks.Enabled = false;
                            or.btnAddCheck.Enabled = false;
                            or.btnRemoveCheck.Enabled = false;

                        }
                        else
                        {
                            or.dgvChecks.Rows.Clear();
                        }

                        //=========================================================================================
                        //                      Cash Receipt Details
                        //=========================================================================================

                        clsSearchCash.loadCashReceiptsDetails(or.dataGridView3, dgvList.SelectedRows[0].Cells["voucherRealNo"].Value.ToString());

                        //=========================================================================================
                        //                      Cash Receipt Footer
                        //=========================================================================================
                        or.txtPostedBy.Text = ds.Tables[0].Rows[0]["Posted_By"].ToString();
                        or.txtCancelledBy.Text = ds.Tables[0].Rows[0]["Cancelled_By"].ToString();

                        if (ds.Tables[0].Rows[0]["Posted"].ToString() == "True" || ds.Tables[0].Rows[0]["Posted"].ToString() == "1")
                        {
                            or.status.Visible = true;
                            or.status.Text = "POSTED";
                        }
                        else if (ds.Tables[0].Rows[0]["Cancelled"].ToString() == "True" || ds.Tables[0].Rows[0]["Cancelled"].ToString() == "1")
                        {
                            or.status.Visible = true;
                            or.status.Text = "CANCELLED";
                        }
                        else
                        {
                            or.status.Visible = false;
                        }


                        //Location of OR as Per Maam Diane Request
                        if (ds.Tables[0].Rows[0]["Location"].ToString() == "PEREA")
                        {
                            or.radioLocPerea.Checked = true;
                        }
                        else
                        {
                            or.radioLocTeltech.Checked = true;
                        }

                        //Put Prepared By
                        or.txtPreparedBy.Text = ds.Tables[0].Rows[0]["Prepared_By"].ToString();
                        or.txtAuditedBy.Text = ds.Tables[0].Rows[0]["Audited_By"].ToString();

                        //Enable Commands
                        or.btnEdit.Enabled = true;
                        or.btnPost.Enabled = true;
                        or.btnCancel.Enabled = true;
                        or.btnAuditted.Enabled = true;

                        or.fromTH = true;

                        or.ShowDialog();
                    }
                }
            }
        }

        private void Worklist_Load(object sender, EventArgs e)
        {
            
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (dgvList.Rows.Count > 0)
            {
                //if theres a data
                foreach (DataGridViewRow row in dgvList.Rows)
                {
                    if(row.Cells[2].GetType().ToString() == "System.Windows.Forms.DataGridViewCheckBoxCell")
                    {
                        DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[2];
                        chk.Value = true;
                    }
                }
            }
            else
            {
                Alert.show("No record(s) found.", Alert.AlertType.error);
                return;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dgvList.Rows.Count > 0)
            {
                //if theres a data
                foreach (DataGridViewRow row in dgvList.Rows)
                {
                    if (row.Cells[2].GetType().ToString() == "System.Windows.Forms.DataGridViewCheckBoxCell")
                    {
                        DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[2];
                        chk.Value = false;
                    }
                }
            }
            else
            {
                Alert.show("No record(s) found.", Alert.AlertType.error);
                return;
            }
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            if (Classes.clsUser.department.ToString() == "1" && Classes.clsUser.role.ToString() == "3")
            {
                //Accounting
                if (dgvList.Rows.Count > 0)
                {
                    int x = 0;
                    int y = 0;
                    foreach (DataGridViewRow row in dgvList.Rows)
                    {
                        if (row.Cells[2].GetType().ToString() == "System.Windows.Forms.DataGridViewCheckBoxCell")
                        {
                            DataGridViewCheckBoxCell cell = row.Cells[2] as DataGridViewCheckBoxCell;
                            //We don't want a null exception!
                            if (cell.Value != null)
                            {
                                if (cell.Value.ToString() == "" || cell.Value.ToString() == "False")
                                {
                                    //Get Total Count of False
                                    x = x + 1;
                                }
                            }
                            y = y + 1;
                        }
                    }

                    //=========================================
                    //      CHECK IF THERES A CHECK VALUE
                    //=========================================

                    if (x == y)
                    {
                        //No data check
                        Alert.show("Please select voucher you want to post.", Alert.AlertType.error);
                        return;
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show(this, "Are you sure you want to proceed?", "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            foreach (DataGridViewRow row in dgvList.Rows)
                            {
                                if (row.Cells[2].GetType().ToString() == "System.Windows.Forms.DataGridViewCheckBoxCell")
                                {
                                    DataGridViewCheckBoxCell cell = row.Cells[2] as DataGridViewCheckBoxCell;
                                    //We don't want a null exception!
                                    if (cell.Value != null)
                                    {
                                        if (cell.Value.ToString() == "True")
                                        {
                                            //Check Columns
                                            //row.Cells[0].Value.ToString() = VOUCHER NUMBER
                                            //row.Cells[1].Value.ToString() = FORM [JV,CV,OR]
                                            postVoucher(row.Cells[1].Value.ToString(), row.Cells[0].Value.ToString());
                                        }
                                    }
                                }
                            }

                            Alert.show("Successfully posted.", Alert.AlertType.success);
                        }
                    }
                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }
            }
            else
            {
                //Audit 
                Alert.show("Error : Access denied.", Alert.AlertType.error);
                return;
            }
        }

        public void postVoucher(string frm, string voucherNo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                switch (frm)
                {
                    case "JV":
                        SqlCommand cmdJV = new SqlCommand();
                        cmdJV.Connection = con;
                        cmdJV.CommandText = "sp_PostingJournal";
                        cmdJV.CommandType = CommandType.StoredProcedure;
                        cmdJV.Parameters.AddWithValue("@JV_No", voucherNo);
                        cmdJV.Parameters.AddWithValue("@Posted_By", Classes.clsUser.Username);
                        cmdJV.ExecuteNonQuery();
                        break;
                    case "OR":
                        SqlCommand cmdOR = new SqlCommand();
                        cmdOR.Connection = con;
                        cmdOR.CommandText = "sp_PostingCashReceipt";
                        cmdOR.CommandType = CommandType.StoredProcedure;
                        cmdOR.Parameters.AddWithValue("@Or_No", voucherNo);
                        cmdOR.Parameters.AddWithValue("@Posted_By", Classes.clsUser.Username);
                        cmdOR.ExecuteNonQuery();
                        break;
                    case "CV":
                        SqlCommand cmdCV = new SqlCommand();
                        cmdCV.Connection = con;
                        cmdCV.CommandText = "sp_PostingDisbursement";
                        cmdCV.CommandType = CommandType.StoredProcedure;
                        cmdCV.Parameters.AddWithValue("@CV_No", voucherNo);
                        cmdCV.Parameters.AddWithValue("@Posted_By", Classes.clsUser.Username);
                        cmdCV.ExecuteNonQuery();
                        break;
                }
            }
        }
    }
}
