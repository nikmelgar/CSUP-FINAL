using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;


namespace WindowsFormsApplication2
{
    public partial class DisbursementVoucher : Form
    {
        public DisbursementVoucher()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        public string getTransaction { get; set; }
        public string getBankFromWithdrawal { get; set; }

        public bool fromWithdrawal = false;

        public bool fromReplenishment = false;
        decimal number;
        //from replenishment
        public string jvno { get; set; }

        int selected, selectedRow;
        string part;
        Global global = new Global();
        Classes.clsDisbursement clsDisbursement = new Classes.clsDisbursement();
        Classes.clsJournalVoucher clsJournalVoucher = new Classes.clsJournalVoucher();
        Classes.clsGeneralVoucher clsGeneral = new Classes.clsGeneralVoucher();

        SqlConnection con;

        CrystalDecisions.Shared.TableLogOnInfo li;
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (btnClose.Text == "CANCEL")
            {
                InitializeFromLoading();
            }
            else
            {
                if (fromReplenishment == true)
                {
                    if(fromWithdrawal == true)
                    {
                        this.Close();
                        return;
                    }

                    Alert.show("Please save this disbursement before closing!", Alert.AlertType.error);
                    return;
                }
                this.Close();
            }

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

        private void panelHeader_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(DisbursementVoucher))
                {
                    form.Activate();
                    return;
                }
            }

            DisbursementVoucher frm = new DisbursementVoucher();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }

        private void DisbursementVoucher_Load(object sender, EventArgs e)
        {
            //load 
            clsDisbursement.loadComboBox(cmbTransaction);
            clsDisbursement.loadBank(cmbBank);


            //load
            populateDatagridCombobox();

            if(fromReplenishment == true)
            {
                cmbTransaction.SelectedValue = getTransaction;
                if(getBankFromWithdrawal == "")
                {
                    cmbBank.SelectedValue = "PCIB";
                }
                else
                {
                    cmbBank.SelectedValue = getBankFromWithdrawal;
                }
                
            }
            else
            {
                InitializeFromLoading();
            }
            

            txtPreparedBy.Text = Classes.clsUser.Username;
        }

        public void populateDatagridCombobox()
        {
            DataGridViewComboBoxColumn cbCell = (DataGridViewComboBoxColumn)dataGridView1.Columns[0];

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT account_Code,account_Description From chart_of_Accounts", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);


                cbCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                cbCell.AutoComplete = true;
                cbCell.DisplayMember = "account_Description";
                cbCell.ValueMember = "account_code";
                cbCell.DataSource = dt;
            }

               
        }
        private void btnSearchPayee_Click(object sender, EventArgs e)
        {
            if (radioMember.Checked == true)
            {
                //Call Look Up
                //controls
                foreach (Form form in Application.OpenForms)
                {


                    if (form.GetType() == typeof(LookUp))
                    {
                        form.Activate();
                        Classes.clsLookUp.whosLookUp = "1";
                        return;
                    }
                }

                Classes.clsLookUp.whosLookUp = "1";
                LookUp LookUp = new LookUp();
                LookUp.ShowDialog();
            }
            else
            {
                //Call Client Look Up
                foreach (Form form in Application.OpenForms)
                {


                    if (form.GetType() == typeof(ClientLookUp))
                    {
                        form.Activate();
                        ClientLookUp.whosLookUp = "0";
                        return;
                    }
                }

                ClientLookUp LookUp = new ClientLookUp();
                ClientLookUp.whosLookUp = "0";
                LookUp.ShowDialog();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            panel22.Visible = false;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT Account_Code,Account_Description from chart_of_accounts where Account_Code like '%" + txtSearch.Text + "%' or account_Description like '%" + txtSearch.Text + "%'", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView2.DataSource = dt;

                    dataGridView2.Columns["Account_code"].HeaderText = "Code";
                    dataGridView2.Columns["Account_code"].FillWeight = 30;

                    dataGridView2.Columns["Account_Description"].HeaderText = "Description";
                    txtSearch.Focus();
                } 
            }
            else
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 25 Account_code,Account_Description from chart_of_Accounts", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView2.DataSource = dt;

                    dataGridView2.Columns["Account_code"].HeaderText = "Code";
                    dataGridView2.Columns["Account_code"].FillWeight = 30;

                    dataGridView2.Columns["Account_Description"].HeaderText = "Description";
                    txtSearch.Focus();
                }   
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[selectedRow].Cells[selected].Value = dataGridView2.SelectedRows[0].Cells["Account_code"].Value.ToString();
            dataGridView1.CurrentRow.Cells[3].ReadOnly = false;
            dataGridView1.CurrentRow.Cells[4].ReadOnly = false;
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[selectedRow].Cells[selected].Value = dataGridView2.SelectedRows[0].Cells["Account_code"].Value.ToString();
            panel22.Visible = false;
            dataGridView1.CurrentRow.Cells[3].ReadOnly = false;
            dataGridView1.CurrentRow.Cells[4].ReadOnly = false;
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not Valid.")
            {
                object value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow.IsNewRow)
                {
                    Alert.show("New row cannot be deleted!", Alert.AlertType.error);
                    return;
                }

                if (dataGridView1.SelectedCells.Count >= 1)
                {
                    string msg = Environment.NewLine + "Are you sure you want to delete this entry?";
                    DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                        {
                            int row = dataGridView1.CurrentCell.RowIndex;
                            dataGridView1.Rows.RemoveAt(row);
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

        private void cmbBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Code to Display in datagridview automatic depends on bank
            if(btnNew.Text != "NEW")
            {
                if (cmbBank.Text != "" && fromReplenishment == false)
                {
                    dataGridView1.Rows[0].Cells[0].Value = clsDisbursement.accountCodeFromBanks(cmbBank.SelectedValue.ToString());
                }
            }
            //==========================================================================
            //object[] rowData = new object[dataGridView1.Columns.Count];
            //rowData[0] = dataGridView1.Rows.Count;
            //dataGridView1.Rows.Add(rowData);
        }

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Debit")
            {
                //your code goes here
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString("#,0.00");
            }
            else if (dataGridView1.Columns[e.ColumnIndex].Name == "Credit")
            {
                //Credit Code goes here
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString("#,0.00");
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (String.IsNullOrEmpty(dataGridView1.CurrentRow.Cells[0].Value as String))
            {
                dataGridView1.CurrentRow.Cells[3].ReadOnly = true;
                dataGridView1.CurrentRow.Cells[4].ReadOnly = true;
            }
            else
            {
                dataGridView1.CurrentRow.Cells[3].ReadOnly = false;
                dataGridView1.CurrentRow.Cells[4].ReadOnly = false;
            }


            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dataGridView1.CurrentCell.ColumnIndex == 3) //Desired Column
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
            else if (dataGridView1.CurrentCell.ColumnIndex == 4)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }
        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 0)
                {
                    //Initialized a new DateTimePicker Control  
                    ////Adding DateTimePicker control into DataGridView   
                    //dataGridView1.Controls.Add(panel1);

                    //// It returns the retangular area that represents the Display area for a cell  
                    Rectangle oRectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    dataGridView1.SelectedCells[e.ColumnIndex].ReadOnly = true;
                    ////Setting area for DateTimePicker Control  
                    //// Setting Location  
                    panel22.Location = new Point(oRectangle.X + 3, oRectangle.Y + 25);

                    selected = dataGridView1.CurrentCell.ColumnIndex;
                    selectedRow = dataGridView1.CurrentRow.Index;
                    // Now make it visible  
                    panelSubs.Visible = false;
                    panel22.Visible = true;

                    //load database
                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 25 Account_code,Account_Description from chart_of_Accounts", con);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView2.DataSource = dt;

                        dataGridView2.Columns["Account_code"].HeaderText = "Code";
                        dataGridView2.Columns["Account_code"].FillWeight = 30;

                        dataGridView2.Columns["Account_Description"].HeaderText = "Description";
                        txtSearch.Focus();

                    }
                    
                }
                else if (e.ColumnIndex == 1)
                {
                    //Initialized a new DateTimePicker Control  
                    ////Adding DateTimePicker control into DataGridView   
                    //dataGridView1.Controls.Add(panel1);

                    //// It returns the retangular area that represents the Display area for a cell  
                    Rectangle oRectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);                    
                    ////Setting area for DateTimePicker Control  
                    //// Setting Location  
                    panelSubs.Location = new Point(oRectangle.X + 3, oRectangle.Y + 25);
                    selected = dataGridView1.CurrentCell.ColumnIndex;
                    selectedRow = dataGridView1.CurrentRow.Index;
                    // Now make it visible  
                    panelSubs.Visible = true;
                    //disable panel account description
                    panel22.Visible = false;
                    //load database
                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();

                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 25 userID,EmployeeID,(CASE WHEN Suffix IS NOT NULL THEN LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1) + Suffix ELSE LastName+', '+ FirstName + SPACE(1) + MiddleName END) as Name From Membership where IsActive = 1 and IsApprove = 1", con);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView3.DataSource = dt;

                        dataGridView3.Columns["userID"].Visible = false;

                        dataGridView3.Columns["EmployeeID"].HeaderText = "ID";
                        dataGridView3.Columns["EmployeeID"].FillWeight = 30;


                        textBox1.Focus();
                    }
                      
                }
                else
                {
                    selected = dataGridView1.CurrentCell.ColumnIndex;
                    panelSubs.Visible = false;
                    panel22.Visible = false;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (btnNew.Text == "NEW")
            {
                InitializeFromLoading();

                //Set Default
                Classes.clsDisbursement.releaseCashWithdrawal = false;

                btnNew.Text = "SAVE";
                btnClose.Text = "CANCEL";

                //Enable All Disable fields 
                btnSearch.Enabled = false;

                //Header Information
                dtCVDate.Enabled = true;
                cmbTransaction.Enabled = true;
                cmbBank.Enabled = true;

                txtChequeNo.Enabled = true;
                dtChequeDate.Enabled = true;
                txtAmount.Enabled = true;
                txtParticular.Enabled = true;

                //Details Information
                dataGridView1.Enabled = true;

                fromReplenishment = false;
                
                
            }
            else
            {
                //SAVING DISBURSEMENT VOUCHER

                //Validation First
                if (checkIfNoValueOnRequiredField(cmbTransaction,cmbBank,txtPayee,txtChequeNo,txtAmount,dataGridView1) == false)
                {
                    //Check First if Total Debit = Total Credit
                    if (Convert.ToDecimal(txtDebit.Text) != Convert.ToDecimal(txtCredit.Text))
                    {
                        Alert.show("Debit / Credit not Equal!", Alert.AlertType.error);
                        return;
                    }

                    //If Equal Continue to save
                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();

                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "sp_InsertDisbursementHeader";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CVDate", dtCVDate.Text);

                        //FOr Payee Type 
                        //Member = 0 Client = 1
                        if (radioMember.Checked == true)
                        {
                            cmd.Parameters.AddWithValue("@Payee_Type", "0");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Payee_Type", "1");
                        }

                        //Check if Theres a Member or Client
                        if (Classes.clsDisbursement.userID.ToString() == "")
                        {
                            cmd.Parameters.AddWithValue("@userID", DBNull.Value);

                        }
                        else if (Classes.clsDisbursement.userID == 0)
                        {
                            cmd.Parameters.AddWithValue("@userID", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@userID", Classes.clsDisbursement.userID.ToString());
                        }

                        cmd.Parameters.AddWithValue("@Payee", txtPayee.Text);
                        cmd.Parameters.AddWithValue("@Payee_Name", txtPayeeName.Text);
                        cmd.Parameters.AddWithValue("@Particulars", txtParticular.Text);
                        cmd.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                        cmd.Parameters.AddWithValue("@Bank_Code", cmbBank.SelectedValue);
                        cmd.Parameters.AddWithValue("@Check_No", txtChequeNo.Text);
                        cmd.Parameters.AddWithValue("@Check_Date", dtChequeDate.Text);
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtAmount.Text));
                        cmd.Parameters.AddWithValue("@Transaction_Type", cmbTransaction.SelectedValue);
                        cmd.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);
                        cmd.ExecuteNonQuery();

                        //Get The CV NO.

                        SqlCommand cmdCV = new SqlCommand();
                        cmdCV.Connection = con;
                        cmdCV.CommandText = "sp_GetCVNoAfterSaving";
                        cmdCV.CommandType = CommandType.StoredProcedure;
                        cmdCV.Parameters.AddWithValue("@CV_Date", dtCVDate.Text);
                        cmdCV.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmdCV);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            txtCVNo.Text = dt.Rows[0].ItemArray[0].ToString();
                        }
                        else
                        {
                            return;
                        }

                        //Insert CV Details
                        //SAVE DETAILS ============================================================================================
                        if (dataGridView1.Rows.Count > 0)
                        {
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (row.Cells[0].Value != null)
                                {
                                    SqlCommand cmdDetail = new SqlCommand();
                                    cmdDetail.Connection = con;
                                    cmdDetail.CommandText = "sp_InsertDisbursementDetail";
                                    cmdDetail.CommandType = CommandType.StoredProcedure;
                                    cmdDetail.Parameters.AddWithValue("@CV_No", txtCVNo.Text);
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
                    }

                   

                    //SAVE MESSAGEBOX HERE
                    Alert.show("Disbursement voucher successfully created.", Alert.AlertType.success);

                    //Button Save = New / Button Cancel = Close
                    btnNew.Text = "NEW";
                    btnClose.Text = "CLOSE";

                    //Button Enable [Commands]
                    btnEdit.Enabled = true;
                    btnPost.Enabled = true;
                    btnCancel.Enabled = true;
                    btnPrint.Enabled = true;
                    btnPrintCheque.Enabled = true;
                    btnSearch.Enabled = true;

                    //Disable Fields
                    CommandControls(false, dtCVDate, cmbTransaction, cmbBank, txtChequeNo, dtChequeDate, txtAmount, txtParticular, dataGridView1);

                    if(fromReplenishment == true)
                    {
                        fromReplenishment = false;
                    }
                }
            }
        }

        //Disbursement Load From Clicking Menu
        public void InitializeFromLoading()
        {
            //Header Information
            dtCVDate.Enabled = false;
            cmbTransaction.Enabled = false;
            cmbBank.Enabled = false;

            txtChequeNo.Enabled = false;
            dtChequeDate.Enabled = false;
            txtAmount.Enabled = false;
            txtParticular.Enabled = false;

            //Details Information
            dataGridView1.Enabled = false;

            //Commands
            btnNew.Enabled = true;
            btnEdit.Enabled = false;
            btnPost.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            btnPrintCheque.Enabled = false;
            btnSearch.Enabled = true;
            btnRelease.Enabled = false;


            //Button Naming Convension
            btnChangeCheque.Text = "CHANGE NUMBER";
            btnNew.Text = "NEW";
            btnEdit.Text = "EDIT";
            btnClose.Text = "CLOSE";

            //Status
            status.Visible = false;


            Control control = new Control();
            //=====================================================================
            //                      Header Information
            //=====================================================================
            foreach (var c in panel5.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
                if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
            }

            txtDebit.Text = "";
            txtCredit.Text = "";

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                try
                {
                    dataGridView1.Rows.Clear();
                }
                catch
                {

                }
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Debit")
            {
                //your code goes here
                compute();
            }
            else if (dataGridView1.Columns[e.ColumnIndex].Name == "Credit")
            {
                //Credit Code goes here
                compute();
            }
        }

        public void compute()
        {
            decimal sumDR = 0;
            decimal sumCR = 0;
            //Check if theres a beneficiary
            if (dataGridView1.Rows.Count > 0)
            {

                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    sumDR += Convert.ToDecimal(dataGridView1.Rows[i].Cells[3].Value);
                    sumCR += Convert.ToDecimal(dataGridView1.Rows[i].Cells[4].Value);
                }
            }

            txtDebit.Text = sumDR.ToString("#,0.00");
            txtCredit.Text = sumCR.ToString("#,0.00");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchDisbursement frm = new searchDisbursement();
            fromReplenishment = false;
            frm.ShowDialog();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (btnEdit.Text == "EDIT")
            {

                //===========================================
                //              CHECK STATUS FIRST
                //===========================================

                fromReplenishment = false;

                if (clsDisbursement.checkIfCancelled(txtCVNo.Text) == true)
                {
                    //If Voucher already cancelled
                    Alert.show("Disbursement Voucher Already Cancelled!", Alert.AlertType.error);
                    return;
                }

                if (clsDisbursement.checkIfPosted(txtCVNo.Text) == true)
                {
                    //If Voucher already Posted
                    Alert.show("Disbursement voucher already posted.", Alert.AlertType.error);
                    return;
                }


                //=============================================
                //              Change button Name
                //=============================================
                btnEdit.Text = "UPDATE";
                btnClose.Text = "CANCEL";
                btnChangeCheque.Text = "CHANGE NUMBER";
                //=============================================
                //              Disable buttons
                //=============================================

                btnNew.Enabled = false;
                btnPost.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = false;
                btnPrintCheque.Enabled = false;
                btnSearch.Enabled = false;
                btnRelease.Enabled = false; 

                //=============================================
                //              Enable Fields
                //=============================================
                //Header Information
                dtCVDate.Enabled = true;
                cmbTransaction.Enabled = true;
                cmbBank.Enabled = true;

                txtChequeNo.Enabled = true;
                dtChequeDate.Enabled = true;
                txtAmount.Enabled = true;
                txtParticular.Enabled = true;

                //Details Information
                dataGridView1.Enabled = true;

            }
            else
            {
                //SAVING CODE HERE =====================================================================================================
                if (checkIfNoValueOnRequiredField(cmbTransaction, cmbBank, txtPayee, txtChequeNo, txtAmount, dataGridView1) == false)
                {
                    if (cmbTransaction.Text == "" || cmbTransaction.Text == " - ")
                    {
                        Alert.show("Transaction Type is Required!", Alert.AlertType.error);
                        return;
                    }

                    if (dataGridView1.Rows.Count <= 1)
                    {
                        Alert.show("Details Information is Required!", Alert.AlertType.error);
                        return;
                    }
                    else
                    {
                        if (Convert.ToDecimal(txtDebit.Text) != Convert.ToDecimal(txtCredit.Text))
                        {
                            Alert.show("Debit / Credit not Equal!", Alert.AlertType.error);
                            return;
                        }
                    }

                    //===================================================================================================================
                    //                                      SAVING CODE HERE
                    //===================================================================================================================

                    //===================================================================================================================
                    //                                      Disbursement Header
                    //===================================================================================================================
                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();

                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "sp_UpdateDisbursementHeader";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@cv_No", txtCVNo.Text);
                        cmd.Parameters.AddWithValue("@cv_Date", dtCVDate.Text);

                        //FOr Payee Type 
                        //Member = 0 Client = 1
                        if (radioMember.Checked == true)
                        {
                            cmd.Parameters.AddWithValue("@Payee_Type", "0");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Payee_Type", "1");
                        }

                        //Check if Theres a Member or Client
                        if (Classes.clsDisbursement.userID.ToString() == "")
                        {
                            cmd.Parameters.AddWithValue("@userID", DBNull.Value);

                        }
                        else if (Classes.clsDisbursement.userID == 0)
                        {
                            cmd.Parameters.AddWithValue("@userID", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@userID", Classes.clsDisbursement.userID.ToString());
                        }

                        cmd.Parameters.AddWithValue("@Payee", txtPayee.Text);
                        cmd.Parameters.AddWithValue("@Payee_Name", txtPayeeName.Text);
                        cmd.Parameters.AddWithValue("@Particulars", txtParticular.Text);
                        cmd.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                        cmd.Parameters.AddWithValue("@Bank_Code", cmbBank.SelectedValue);
                        cmd.Parameters.AddWithValue("@Check_No", txtChequeNo.Text);
                        cmd.Parameters.AddWithValue("@Check_Date", dtChequeDate.Text);
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtAmount.Text));
                        cmd.Parameters.AddWithValue("@Transaction_Type", cmbTransaction.SelectedValue);
                        cmd.ExecuteNonQuery();

                        //==============================================================================================
                        //                          FOR DISBURSEMENT DETAILS
                        //==============================================================================================

                        SqlCommand cmdDelete = new SqlCommand();
                        cmdDelete.Connection = con;
                        cmdDelete.CommandText = "sp_DeleteDisbursementDetail";
                        cmdDelete.CommandType = CommandType.StoredProcedure;
                        cmdDelete.Parameters.AddWithValue("@CV_No", txtCVNo.Text);
                        cmdDelete.ExecuteNonQuery();

                        //==============================================================================================
                        //                          FOR DISBURSEMENT DETAILS UPDATING
                        //==============================================================================================
                        if (dataGridView1.Rows.Count > 0)
                        {
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (row.Cells[0].Value != null)
                                {
                                    SqlCommand cmdDetail = new SqlCommand();
                                    cmdDetail.Connection = con;
                                    cmdDetail.CommandText = "sp_InsertDisbursementDetail";
                                    cmdDetail.CommandType = CommandType.StoredProcedure;
                                    cmdDetail.Parameters.AddWithValue("@CV_No", txtCVNo.Text);
                                    cmdDetail.Parameters.AddWithValue("@Account_Code", row.Cells[0].Value);

                                    if (DBNull.Value.Equals(row.Cells[5]))
                                    {
                                        //NULL
                                        cmdDetail.Parameters.AddWithValue("@userID", DBNull.Value);
                                    }
                                    else
                                    {
                                        //Not Null
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
                    }
                    //Messagebox here
                    Alert.show("Disbursement Voucher Successfully Updated.", Alert.AlertType.success);

                    //return all buttons
                    //Button Enable [Commands]
                    btnEdit.Enabled = true;
                    btnPost.Enabled = true;
                    btnCancel.Enabled = true;
                    btnPrint.Enabled = true;
                    btnPrintCheque.Enabled = true;
                    btnSearch.Enabled = true;
                    btnNew.Enabled = true;

                    //Disable Fields
                    CommandControls(false, dtCVDate, cmbTransaction, cmbBank, txtChequeNo, dtChequeDate, txtAmount, txtParticular, dataGridView1);

                    //Button change name
                    btnClose.Text = "CLOSE";
                    btnEdit.Text = "EDIT";

                    //FREEZE ALL

                }
            }
        }

        public void CommandControls(bool DisableEnalbe,DateTimePicker cvdate, ComboBox cmbtransaction, ComboBox cmbBank,TextBox txtCheque, DateTimePicker chequedate,TextBox amount,TextBox particular,DataGridView dgv)
        {
            cvdate.Enabled = DisableEnalbe;
            cmbtransaction.Enabled = DisableEnalbe;
            cmbBank.Enabled = DisableEnalbe;
            txtCheque.Enabled = DisableEnalbe;
            chequedate.Enabled = DisableEnalbe;
            amount.Enabled = DisableEnalbe;
            particular.Enabled = DisableEnalbe;
            dgv.Enabled = DisableEnalbe;
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            //===================================================================================
            //                              DISBURSEMENT POSTING
            //===================================================================================

            fromReplenishment = false;

            if (clsDisbursement.checkIfCancelled(txtCVNo.Text) == true)
            {
                //If Voucher already cancelled
                Alert.show("Disbursement Voucher Already Cancelled!", Alert.AlertType.error);
                return;
            }

            if (clsDisbursement.checkIfPosted(txtCVNo.Text) == true)
            {
                //If Voucher already Posted
                Alert.show("Disbursement voucher already posted.", Alert.AlertType.error);
                return;
            }

            btnChangeCheque.Text = "CHANGE NUMBER";


            if(txtChequeNo.Text == "")
            {
                Alert.show("Cheque Number is Required!", Alert.AlertType.error);
                return;
            }
            //===================================================================================
            //                       Are you sure you want to post this?
            //===================================================================================

            string msg = Environment.NewLine + "Are you sure you want to continue?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //Code for posting
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_PostingDisbursement";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CV_No", txtCVNo.Text);
                    cmd.Parameters.AddWithValue("@Posted_By", Classes.clsUser.Username);
                    cmd.ExecuteNonQuery();


                    //REFLECT TO WITHDRAWAL FROM REPLENISHMENT CASH
                    if (fromReplenishment == true)
                    {
                        SqlCommand cmd2 = new SqlCommand();
                        cmd2.Connection = con;
                        cmd2.CommandText = "sp_UpdateReplenishAndCVNoWithdrawalSlip";
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@CV_No", txtCVNo.Text);
                        cmd2.Parameters.AddWithValue("@JV_No", jvno);
                        cmd2.ExecuteNonQuery();

                        jvno = null;
                    }

                    //Posting for Withdrawal Table
                    if (Classes.clsDisbursement.releaseCashWithdrawal == true)
                    {
                        SqlCommand cmdSD = new SqlCommand();
                        cmdSD.Connection = con;
                        cmdSD.CommandText = "UPDATE Withdrawal_Slip SET Posted = 1, Posted_By = '" + Classes.clsUser.Username + "', CV_No = '" + txtCVNo.Text + "', CV_Date = '" + DateTime.Today + "' WHERE Withdrawal_Slip_No = '" + Classes.clsDisbursement.slipFromWithdrawal + "'";
                        cmdSD.CommandType = CommandType.Text;
                        cmdSD.ExecuteNonQuery();
                        //After posting set to false 
                        Classes.clsDisbursement.releaseCashWithdrawal = false;
                        Classes.clsDisbursement.slipFromWithdrawal = string.Empty;

                        //Refresh Savings Grid
                        Savings frm = new Savings();
                        frm = (Savings)Application.OpenForms["Savings"];
                        frm.refreshData();
                    }

                    //Check if the voucher is from withdrawal of savings 
                    SqlDataAdapter adapterCheck = new SqlDataAdapter("SELECT wd_slip_no FROM Disbursement_Header WHERE CV_No = '" + txtCVNo.Text + "'", con);
                    DataTable dtCheck = new DataTable();
                    adapterCheck.Fill(dtCheck);

                    if (dtCheck.Rows[0].ItemArray[0].ToString() != "")
                    {
                        //TRUE [FROM SAVINGS]
                        SqlCommand cmdSD = new SqlCommand();
                        cmdSD.Connection = con;
                        cmdSD.CommandText = "UPDATE Withdrawal_Slip SET Posted = 1, Posted_By = '" + Classes.clsUser.Username + "', CV_No = '" + txtCVNo.Text + "', CV_Date = '" + DateTime.Today.ToShortDateString() + "' WHERE Withdrawal_Slip_No = '" + dtCheck.Rows[0].ItemArray[0].ToString() + "'";
                        cmdSD.CommandType = CommandType.Text;
                        cmdSD.ExecuteNonQuery();
                    }
                }
                //Success Message
                Alert.show("Disbursement voucher successfully posted.", Alert.AlertType.success);

                //Display Message
                status.Visible = true;
                status.Text = "POSTED";

                //Get Whos Posted IT
                if (txtPostedBy.Text == "")
                {
                    txtPostedBy.Text = clsDisbursement.getPostedBy(txtCVNo.Text);
                }
            }
            else
            {
                return;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            part = txtParticular.Text;
            btnChangeCheque.Text = "CHANGE NUMBER";
            //===================================================================================
            //                              DISBURSEMENT Cancellation
            //===================================================================================

            fromReplenishment = false;

            if (clsDisbursement.checkIfCancelled(txtCVNo.Text) == true)
            {
                //If Voucher already cancelled
                Alert.show("Disbursement Voucher Already Cancelled!", Alert.AlertType.error);
                return;
            }

            if (clsDisbursement.checkIfPosted(txtCVNo.Text) == true)
            {
                //If Voucher already Posted
                Alert.show("Disbursement voucher already posted.", Alert.AlertType.error);
                return;
            }

            //ENABLE PARTICULARS FOR NOTE
            txtParticular.Enabled = true;
            if (txtParticular.Text == "")
            {
                Alert.show("Please put note on particulars before cancellation!", Alert.AlertType.error);
                return;
            }


            //===================================================================================
            //                       Are you sure you want to cancel this?
            //===================================================================================

            string msg = Environment.NewLine + "Are you sure you want to continue?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {


                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_CancellationOfDisbursement";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CV_No", txtCVNo.Text);
                    cmd.Parameters.AddWithValue("@Cancel_Note", txtParticular.Text);
                    cmd.Parameters.AddWithValue("@Cancelled_By", Classes.clsUser.Username);
                    cmd.ExecuteNonQuery();
                }
                //Success Message
                Alert.show("Disbursement Voucher Successfully Cancelled!", Alert.AlertType.success);

                //Display Message
                status.Visible = true;
                status.Text = "CANCELLED";

                //Get Cancelled By 
                if (txtCancelledBy.Text == "")
                {
                    txtCancelledBy.Text = clsDisbursement.getCancelBy(txtCVNo.Text);
                }

                //Disable After Cancel
                txtParticular.Enabled = false;
            }
            else
            {
                txtParticular.Enabled = false;
                part = txtParticular.Text;
                return;
            }


        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnChangeCheque.Text = "CHANGE NUMBER";
            CrystalDecisions.Shared.TableLogOnInfo li;
            fromReplenishment = false;
            if (txtCVNo.Text != "")
            {
                //Print Purposes
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    if (cmbTransaction.SelectedValue.ToString() == "TRAN002")
                    {
                        //Check first if he has a other deduction
                        SqlDataAdapter adapterCheck = new SqlDataAdapter("SELECT * FROM vw_GeneralDisbursementVoucher WHERE Loan_No ='" + txtLoanNo.Text + "'", con);
                        DataTable dtCheck = new DataTable();
                        adapterCheck.Fill(dtCheck);

                        if (dtCheck.Rows.Count > 0)
                        {
                            //Call vw_GeneralJournal
                            //Generate GENERAL VOUCHER REPORT FOR LOANS
                            SqlCommand cmdGeneral = new SqlCommand();
                            cmdGeneral.Connection = con;
                            cmdGeneral.CommandText = "SELECT * FROM vw_GeneralDisbursementVoucher WHERE Loan_No = '" + txtLoanNo.Text + "'";
                            cmdGeneral.CommandType = CommandType.Text;
                            cmdGeneral.ExecuteNonQuery();

                            SqlDataAdapter adapterGeneral = new SqlDataAdapter(cmdGeneral);



                            DataTable dtGeneral = new DataTable();
                            DataSet dsGeneral = new DataSet();

                            ReportsForms.DisbursementGeneralVoucher cr = new ReportsForms.DisbursementGeneralVoucher();
                            ReportsForms.GeneralVoucherJournal rpt = new ReportsForms.GeneralVoucherJournal();

                            li = new TableLogOnInfo();


                            adapterGeneral.Fill(dsGeneral, "vw_GeneralJournalVoucher");
                            dtGeneral = dsGeneral.Tables["vw_GeneralJournalVoucher"];
                            cr.SetDataSource(dsGeneral.Tables["vw_GeneralJournalVoucher"]);

                            //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                            cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

                            cr.SetParameterValue("cv_no", txtCVNo.Text);
                            cr.SetParameterValue("savings", clsGeneral.returnSavings(txtLoanNo.Text));
                            cr.SetParameterValue("sharecapital", clsGeneral.returnShareCapital(txtLoanNo.Text));
                            cr.SetParameterValue("totalDeduction", clsGeneral.returnTotalDeduction(txtLoanNo.Text));

                            SqlDataAdapter adapterBillDate = new SqlDataAdapter("SELECT top 3 Schedule_Payment FROM Loan_Details WHERE Loan_No = '" + txtLoanNo.Text + "' ORDER BY Schedule_Payment ASC", con);
                            DataTable dtBillDate = new DataTable();
                            adapterBillDate.Fill(dtBillDate);

                            cr.SetParameterValue("billDate1", Convert.ToString(Convert.ToDateTime(dtBillDate.Rows[0].ItemArray[0].ToString()).ToShortDateString()));
                            cr.SetParameterValue("billDate2", Convert.ToString(Convert.ToDateTime(dtBillDate.Rows[1].ItemArray[0].ToString()).ToShortDateString()));
                            cr.SetParameterValue("billDate3", Convert.ToString(Convert.ToDateTime(dtBillDate.Rows[2].ItemArray[0].ToString()).ToShortDateString()));

                            rpt.crystalReportViewer1.ReportSource = cr;
                            rpt.ShowDialog();
                        }
                        else
                        {
                            //Call vw_GeneralJournalVoucherNoLoanDeduction
                            //Generate GENERAL VOUCHER REPORT FOR LOANS
                            SqlCommand cmdGeneral = new SqlCommand();
                            cmdGeneral.Connection = con;
                            cmdGeneral.CommandText = "SELECT * FROM vw_GeneralDisbursementVoucherNoLoanDeduction WHERE Loan_No = '" + txtLoanNo.Text + "'";
                            cmdGeneral.CommandType = CommandType.Text;
                            cmdGeneral.ExecuteNonQuery();

                            SqlDataAdapter adapterGeneral = new SqlDataAdapter(cmdGeneral);



                            DataTable dtGeneral = new DataTable();
                            DataSet dsGeneral = new DataSet();

                            ReportsForms.DisbursementGeneralVoucherNoDeduction cr = new ReportsForms.DisbursementGeneralVoucherNoDeduction();
                            ReportsForms.GeneralVoucherJournal rpt = new ReportsForms.GeneralVoucherJournal();

                            li = new TableLogOnInfo();

                            adapterGeneral.Fill(dsGeneral, "vw_GeneralDisbursementVoucherNoLoanDeduction");
                            dtGeneral = dsGeneral.Tables["vw_GeneralDisbursementVoucherNoLoanDeduction"];
                            cr.SetDataSource(dsGeneral.Tables["vw_GeneralDisbursementVoucherNoLoanDeduction"]);

                            //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                            cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

                            cr.SetParameterValue("cv_no", txtCVNo.Text);
                            cr.SetParameterValue("savings", clsGeneral.returnSavings(txtLoanNo.Text));
                            cr.SetParameterValue("sharecapital", clsGeneral.returnShareCapital(txtLoanNo.Text));
                            cr.SetParameterValue("totalDeduction", clsGeneral.returnTotalDeduction(txtLoanNo.Text));

                            SqlDataAdapter adapterBillDate = new SqlDataAdapter("SELECT top 3 Schedule_Payment FROM Loan_Details WHERE Loan_No = '" + txtLoanNo.Text + "' ORDER BY Schedule_Payment ASC", con);
                            DataTable dtBillDate = new DataTable();
                            adapterBillDate.Fill(dtBillDate);

                            cr.SetParameterValue("billDate1", Convert.ToString(Convert.ToDateTime(dtBillDate.Rows[0].ItemArray[0].ToString()).ToShortDateString()));
                            cr.SetParameterValue("billDate2", Convert.ToString(Convert.ToDateTime(dtBillDate.Rows[1].ItemArray[0].ToString()).ToShortDateString()));
                            cr.SetParameterValue("billDate3", Convert.ToString(Convert.ToDateTime(dtBillDate.Rows[2].ItemArray[0].ToString()).ToShortDateString()));

                            rpt.crystalReportViewer1.ReportSource = cr;
                            rpt.ShowDialog();
                        }
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "sp_GetDisbursementDetailSummaryReport";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CV_No", txtCVNo.Text);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);



                        DataTable dt = new DataTable();
                        DataSet ds = new DataSet();

                        ReportsForms.crDisbursement cr = new ReportsForms.crDisbursement();
                        ReportsForms.rptDisbursement rpt = new ReportsForms.rptDisbursement();

                        li = new TableLogOnInfo();

                        li.ConnectionInfo.IntegratedSecurity = false;

                        adapter.Fill(ds, "vw_DisbursementForReporting");
                        dt = ds.Tables["vw_DisbursementForReporting"];
                        cr.SetDataSource(ds.Tables["vw_DisbursementForReporting"]);

                        //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                        cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

                        rpt.crystalReportViewer1.ReportSource = cr;
                        rpt.crystalReportViewer1.RefreshReport();

                        rpt.ShowDialog();
                    }
                }
            }
            else
            {
                Alert.show("Please Select Disbursement Before Printing!", Alert.AlertType.warning);
                return;
            }
        }

        public bool checkIfNoValueOnRequiredField(ComboBox cmbTransaction, ComboBox cmbBank, TextBox payee ,TextBox checkNo, TextBox Amount, DataGridView dgv)
        {
            if (cmbTransaction.Text == "" || cmbTransaction.Text == " - " || cmbBank.Text == "" || checkNo.Text == "" || Amount.Text == "" || payee.Text == "")
            {
                Alert.show("All fields with ( * ) are required.", Alert.AlertType.warning);
                return true;
            }
            else if(dgv.Rows.Count <= 1)
            {
                Alert.show("Disbursement detail is required!", Alert.AlertType.warning);
                return true;
            }
            else
            {
                return false;
            }

        }

        private void txtAmount_Leave(object sender, EventArgs e)
        {
            if (txtAmount.Text != "")
            {
                txtAmount.Text = Convert.ToDecimal(txtAmount.Text).ToString("#,0.00");
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtChequeNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void radioMember_CheckedChanged(object sender, EventArgs e)
        {
            txtPayee.Text = "";
            txtPayeeName.Text = "";

        }

        private void radioClient_CheckedChanged(object sender, EventArgs e)
        {
            txtPayee.Text = "";
            txtPayeeName.Text = "";
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            panelSubs.Visible = false;
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[selectedRow].Cells[selected].Value = dataGridView3.SelectedRows[0].Cells["EmployeeID"].Value.ToString() +" - "+dataGridView3.SelectedRows[0].Cells["Name"].Value.ToString();
            dataGridView1.Rows[selectedRow].Cells[5].Value = dataGridView3.SelectedRows[0].Cells["userID"].Value.ToString();

            textBox1.Text = "";
            panelSubs.Visible = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT userID,EmployeeID,(CASE WHEN Suffix IS NOT NULL THEN LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1) + Suffix ELSE LastName+', '+ FirstName + SPACE(1) + MiddleName END) as Name From Membership where IsActive = 1 and IsApprove = 1 and EmployeeID like '%" + textBox1.Text + "%' or LastName like '%" + textBox1.Text + "%' or FirstName like '%" + textBox1.Text + "%' or MiddleName like '%" + textBox1.Text + "%'", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView3.DataSource = dt;

                    dataGridView3.Columns["userID"].Visible = false;

                    dataGridView3.Columns["EmployeeID"].HeaderText = "ID";
                    dataGridView3.Columns["EmployeeID"].FillWeight = 30;
                    textBox1.Focus();
                }

                
            }
            else
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 25 userID,EmployeeID,(CASE WHEN Suffix IS NOT NULL THEN LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1) + Suffix ELSE LastName+', '+ FirstName + SPACE(1) + MiddleName END) as Name From Membership where IsActive = 1 and IsApprove = 1", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView3.DataSource = dt;

                    dataGridView3.Columns["EmployeeID"].HeaderText = "ID";
                    dataGridView3.Columns["EmployeeID"].FillWeight = 30;
                    textBox1.Focus();
                }
                 
            }
        }

        private void btnPrintCheque_Click(object sender, EventArgs e)
        {
            //All Members
            btnChangeCheque.Text = "CHANGE NUMBER";
            CrystalDecisions.Shared.TableLogOnInfo li;
            li = new TableLogOnInfo();

            li.ConnectionInfo.IntegratedSecurity = false;

            //PRINTING CHECKS
            ReportsForms.crCheckPrinting cr = new ReportsForms.crCheckPrinting();
            ReportsForms.rptCheck rpt = new ReportsForms.rptCheck();

            //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
            cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

            cr.SetParameterValue("name", "** " + txtPayeeName.Text + " **");

            try
            {
                string num = txtAmount.Text;
                
                string word;

                if (txtAmount.Text == "")
                {
                    txtAmount.Text = "";
                }
                else
                {
                    number = decimal.Parse(num.ToString());

                    if (number.ToString() == "0")
                    {
                        MessageBox.Show("The number in currency fomat is \nZero Only");
                    }
                    else
                    {
                        word = Classes.clsSavingsDataEntry.ConvertToWords(number.ToString());
                    }
                }


                Console.ReadKey();
            }
            catch (System.Exception ex)
            {


            }
        
            cr.SetParameterValue("amountwords", "** " + Classes.clsSavingsDataEntry.ConvertToWords(number.ToString()) + " **");
            cr.SetParameterValue("amount", txtAmount.Text);
            
            rpt.crystalReportViewer1.ReportSource = cr;
            rpt.ShowDialog();
        }

        private void btnChangeCheque_Click(object sender, EventArgs e)
        {
            //==============================================================
            //              FOR CHANGING THE CHEQUE 
            //==============================================================
            if (txtCVNo.Text == "")
            {
                Alert.show("Please select Disbursement First!", Alert.AlertType.error);
                return;
            }

            if (clsDisbursement.checkIfCancelled(txtCVNo.Text) == true)
            {
                //If Voucher already cancelled
                Alert.show("Disbursement Voucher Already Cancelled!", Alert.AlertType.error);
                return;
            }

            if (clsDisbursement.checkIfPosted(txtCVNo.Text) == true)
            {
                //If Voucher already Posted
                Alert.show("Disbursement voucher already posted.", Alert.AlertType.error);
                return;
            }


            if(btnChangeCheque.Text == "CHANGE NUMBER")
            {
                btnChangeCheque.Text = "UPDATE";
                btnSearch.Enabled = false;
                btnNew.Enabled = false;
                btnEdit.Enabled = false;
                btnPost.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = false;
                btnPrintCheque.Enabled = false;
                btnRelease.Enabled = false;

                btnChangeCheque.Size = new Size(169, 37);

                button1.Visible = true;
                
                if(txtChequeNo.Enabled != true)
                {
                    txtChequeNo.Enabled = true;
                }


            }
            else
            {
                
                if (txtChequeNo.Text == "")
                {
                    Alert.show("Please fill up cheque no. field!", Alert.AlertType.error);
                    txtChequeNo.Focus();
                    return;
                }

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    //check if from withdrawal slip then change withdrawal slip also
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT wd_slip_no FROM Disbursement_Header WHERE CV_No = '" + txtCVNo.Text + "'", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (!DBNull.Value.Equals(dt.Rows[0].ItemArray[0].ToString()))
                    {
                        //not null = From Withdrawal Slip

                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE Withdrawal_Slip SET Check_No = '" + txtChequeNo.Text + "' WHERE Withdrawal_Slip_No ='" + dt.Rows[0].ItemArray[0].ToString() + "'";
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();

                        SqlCommand cmd2 = new SqlCommand();
                        cmd2.Connection = con;
                        cmd2.CommandText = "UPDATE Disbursement_Header SET Check_No = '" + txtChequeNo.Text + "' WHERE CV_No = '" + txtCVNo.Text + "'";
                        cmd2.CommandType = CommandType.Text;
                        cmd2.ExecuteNonQuery();

                    }
                    else
                    {
                        //null = Not From Withdrawal
                        SqlCommand cmd2 = new SqlCommand();
                        cmd2.Connection = con;
                        cmd2.CommandText = "UPDATE Disbursement_Header SET Check_No = '" + txtChequeNo.Text + "' WHERE CV_No = '" + txtCVNo.Text + "'";
                        cmd2.CommandType = CommandType.Text;
                        cmd2.ExecuteNonQuery();
                    }

                    txtChequeNo.Enabled = false;
                    btnSearch.Enabled = true;
                    btnChangeCheque.Text = "CHANGE NUMBER";
                    Alert.show("Cheque No. Successfully Updated.", Alert.AlertType.success);

                    //Restore to original after successfully updated
                    btnSearch.Enabled = true;
                    btnNew.Enabled = true;
                    btnEdit.Enabled = true;
                    btnPost.Enabled = true;
                    btnCancel.Enabled = true;
                    btnPrint.Enabled = true;
                    btnPrintCheque.Enabled = true;
                    btnRelease.Enabled = true;
                    btnChangeCheque.Text = "CHANGE NUMBER";
                    btnChangeCheque.Size = new Size(273, 37);
                    txtChequeNo.Enabled = false;
                    button1.Visible = false;
                }
            }

          }

        private void button1_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = true;
            btnNew.Enabled = true;
            btnEdit.Enabled = true;
            btnPost.Enabled = true;
            btnCancel.Enabled = true;
            btnPrint.Enabled = true;
            btnPrintCheque.Enabled = true;
            btnChangeCheque.Text = "CHANGE NUMBER";
            btnChangeCheque.Size = new Size(273, 37);
            txtChequeNo.Enabled = false;
            button1.Visible = false;
            btnRelease.Enabled = true;
        }

        private void txtLoanNo_TextChanged(object sender, EventArgs e)
        {
            if (txtLoanNo.Text != "")
            {
                txtLoanType.Text = clsJournalVoucher.returnLoanTypeDescription(txtLoanNo.Text);
            }
            else
            {
                txtLoanType.Text = "";
            }
        }

        private void btnRelease_Click(object sender, EventArgs e)
        {
            clsDisbursement.ReleaseDisbursement(txtCVNo.Text,status);
        }

        private void btnSearchLoan_Click(object sender, EventArgs e)
        {
            if (LoanLookUpProcess.clsLoanLookUpMember.userid != 0)
            {
                //has a value 
                LoanLookUpProcess.LoanLookUp frm = new LoanLookUpProcess.LoanLookUp();
                LoanLookUpProcess.clsLoanLookUpMember.frmPass = "Disbursement";
                frm.ShowDialog();
            }
            else
            {
                //No Record(s)9561473002
                Alert.show("Please select member first.", Alert.AlertType.error);
                return;
            }
        }

        public void ForReplenishment()
        {
            btnNew.Enabled = true;
            btnPost.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            btnPrintCheque.Enabled = false;
            btnSearch.Enabled = false;
            btnEdit.Enabled = false;

            btnNew.Text = "SAVE";

            //=============================================
            //              Enable Fields
            //=============================================
            //Header Information
            dtCVDate.Enabled = true;
            cmbTransaction.Enabled = true;
            cmbBank.Enabled = true;

            txtChequeNo.Enabled = true;
            dtChequeDate.Enabled = true;
            txtAmount.Enabled = true;
            txtParticular.Enabled = true;

            //Details Information
            dataGridView1.Enabled = true;

            fromReplenishment = true;
        }
    }
}
