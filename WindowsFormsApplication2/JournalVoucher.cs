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
    public partial class JournalVoucher : Form
    {
        public JournalVoucher()
        {
            InitializeComponent();
        }

        //=================================================================
        //              DECLARATION
        //=================================================================
        Global global = new Global();
        Classes.clsJournalVoucher clsJournalVoucher = new Classes.clsJournalVoucher();
        Classes.clsGeneralVoucher clsGeneral = new Classes.clsGeneralVoucher();
     
        SqlConnection con;
        SqlDataAdapter adapter;
        CrystalDecisions.Shared.TableLogOnInfo li;
        //=================================================================

        public string getTransaction { get; set; }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        int selected,selectedRow;

        bool frmCancel = false;
        private void btnClose_Click(object sender, EventArgs e)
        {
            if(btnClose.Text == "CLOSE")
            {
                if(Classes.clsJournalVoucher.fromReplenishment == true)
                {
                    Alert.show("Please finish this transaction first!", Alert.AlertType.error);
                    return;
                }
                this.Close();

            }
            else
            {
                ForCancel();
                txtCredit.Text = "";
                txtDebit.Text = "";
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

        private void JournalVoucher_Load(object sender, EventArgs e)
        {
            //Load Datas
            clsJournalVoucher.loadComboBox(cmbTransaction);

            //load
            populateDatagridCombobox();

            //combobox transaction
            if(getTransaction == "TRAN001")
            {
                cmbTransaction.SelectedIndex = 1;
            }
            else
            {
                cmbTransaction.SelectedIndex = -1;
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
        private void panelHeader_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(JournalVoucher))
                {
                    form.Activate();
                    return;
                }
            }

            JournalVoucher frm = new JournalVoucher();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
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
                    panel22.Visible = true;
                    //disable panel subs
                    panel11.Visible = false;

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
                else if(e.ColumnIndex == 1)
                {
                    //Initialized a new DateTimePicker Control  
                    ////Adding DateTimePicker control into DataGridView   
                    //dataGridView1.Controls.Add(panel1);

                    //// It returns the retangular area that represents the Display area for a cell  
                    Rectangle oRectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    ////Setting area for DateTimePicker Control  
                    //// Setting Location  
                    panel11.Location = new Point(oRectangle.X + 3, oRectangle.Y + 25);
                    selected = dataGridView1.CurrentCell.ColumnIndex;
                    selectedRow = dataGridView1.CurrentRow.Index;
                    // Now make it visible  
                    panel11.Visible = true;
                    //disable panel account description
                    panel22.Visible = false;
                    //load database
                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();

                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 25 userID,EmployeeID,CASE WHEN Suffix is not null THEN (LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1) + Suffix) WHEN Suffix is null THEN (LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1)) END as Name From Membership where IsActive = 1 and IsApprove = 1", con);
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
                    panel22.Visible = false;
                    panel11.Visible = false;
                }
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                if (selected == 0)
                {
                    dataGridView1.Enabled = false;
                    dataGridView1.GetNextControl(dataGridView1, true).Focus();
                    dataGridView1.Enabled = true;
                    e.Handled = true;
                }

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            panel22.Visible = false;
            txtSearch.Text = "";
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if(txtSearch.Text != "")
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

        private void btnAutoEntry_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            //{
            //    dataGridView1.Rows.RemoveAt(i);
            //    i--;
            //    while (dataGridView1.Rows.Count == 0)
            //        continue;
            //}

            //dataGridView1.Rows[0].Cells[0].Value = "1";



            //int rowId = dataGridView1.Rows.Add();

            //// Grab the new row!
            //DataGridViewRow row = dataGridView1.Rows[rowId];

            //// Add the data
            //row.Cells[0].Value = "10";

            ////compute
            //compute();
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

        private void button5_Click(object sender, EventArgs e)
        {

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

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[selectedRow].Cells[selected].Value = dataGridView2.SelectedRows[0].Cells["Account_code"].Value.ToString();
            panel22.Visible = false;

            dataGridView1.CurrentRow.Cells[3].ReadOnly = false;
            dataGridView1.CurrentRow.Cells[4].ReadOnly = false;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (btnNew.Text == "NEW")
            {
                enableButtonsAfterClickingNew();
                forNew();
                btnSearchJV.Enabled = false;
                status.Visible = false;
                frmCancel = false;

                txtPostedBy.Text = "";
                txtCancelled.Text = "";
                //Check if Search is Online
                foreach (Form form in Application.OpenForms)
                {
                    if (form.GetType() == typeof(searchJournal))
                    {
                        form.Close();
                        return;
                    }
                }

            }
            else if (btnNew.Text == "SAVE")
            {
                if (cmbTransaction.Text == "" || cmbTransaction.Text == " - ")
                {
                    Alert.show("Transaction Type is Required!", Alert.AlertType.error);
                    return;
                }

                if (dataGridView1.Rows.Count <= 1)
                {
                    Alert.show("Details Information is required.", Alert.AlertType.error);
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


                //SAVE
                //Save Header First
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmdHeader = new SqlCommand();
                    cmdHeader.Connection = con;
                    cmdHeader.CommandText = "sp_InsertJournalHeader";
                    cmdHeader.CommandType = CommandType.StoredProcedure;
                    cmdHeader.Parameters.AddWithValue("@JV_Date", dtJVDate.Text);

                    //Check if Theres a Member or Client
                    if (Classes.clsJournalVoucher.userId.ToString() == "")
                    {
                        cmdHeader.Parameters.AddWithValue("@userID", DBNull.Value);

                    }
                    else
                    {
                        cmdHeader.Parameters.AddWithValue("@userID", Classes.clsJournalVoucher.userId.ToString());
                    }

                    cmdHeader.Parameters.AddWithValue("@AdjTo", txtMember.Text);
                    cmdHeader.Parameters.AddWithValue("@Particular", txtParticulars.Text);
                    cmdHeader.Parameters.AddWithValue("@Loan_No", txtLoanNumber.Text);
                    cmdHeader.Parameters.AddWithValue("@Posted", false);
                    cmdHeader.Parameters.AddWithValue("@Transaction_Type", cmbTransaction.SelectedValue);
                    cmdHeader.Parameters.AddWithValue("@summarize", checkBox1.Checked);
                    cmdHeader.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);
                    cmdHeader.Parameters.AddWithValue("@Poseted_By", DBNull.Value);
                    cmdHeader.ExecuteNonQuery();

                    //Get JV#

                    SqlCommand cmdJV = new SqlCommand();
                    cmdJV.Connection = con;
                    cmdJV.CommandText = "sp_GetJVNoAfterSaving";
                    cmdJV.CommandType = CommandType.StoredProcedure;
                    cmdJV.Parameters.AddWithValue("@jv_date", dtJVDate.Text);
                    cmdJV.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmdJV);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        txtJVNumber.Text = dt.Rows[0].ItemArray[0].ToString();
                    }
                    else
                    {
                        return;
                    }

                    //SAVE DETAILS ============================================================================================
                    if (dataGridView1.Rows.Count > 0)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells[0].Value != null)
                            {
                                SqlCommand cmd = new SqlCommand();
                                cmd.Connection = con;
                                cmd.CommandText = "sp_InsertJournalDetail";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@JV_No", txtJVNumber.Text);
                                cmd.Parameters.AddWithValue("@Account_Code", row.Cells[0].Value);

                                if (Convert.ToInt32(row.Cells[5].Value) == 0)
                                {
                                    cmd.Parameters.AddWithValue("@userID", DBNull.Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@userID", Convert.ToInt32(row.Cells[5].Value));
                                }

                                if (row.Cells[1].Value != null)
                                {
                                    cmd.Parameters.AddWithValue("@Subsidiary_Code", row.Cells[1].Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@Subsidiary_Code", DBNull.Value);
                                }


                                if (row.Cells[2].Value != null)
                                {
                                    cmd.Parameters.AddWithValue("@Loan_No", row.Cells[2].Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                                }

                                cmd.Parameters.AddWithValue("@Debit", Convert.ToDecimal(row.Cells[3].Value));
                                cmd.Parameters.AddWithValue("@Credit", Convert.ToDecimal(row.Cells[4].Value));
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                //SAVE MESSAGEBOX HERE
                Alert.show("Journal Voucher Successfully Created!", Alert.AlertType.success);

                //Disable all fields 
                txtParticulars.ReadOnly = true;
                dataGridView1.Enabled = false;
                btnUpload.Enabled = false;

                //Button Save change to NEW
                btnNew.Text = "NEW";
                btnEdit.Enabled = true;
                

                //enable button 
                btnCancel.Enabled = true;
                btnPrint.Enabled = true;
                btnPost.Enabled = true;
                btnSearchJV.Enabled = true;
                
                btnClose.Text = "CLOSE";
               
            }
            
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[selectedRow].Cells[selected].Value = dataGridView2.SelectedRows[0].Cells["Account_code"].Value.ToString();

            dataGridView1.CurrentRow.Cells[3].ReadOnly = false;
            dataGridView1.CurrentRow.Cells[4].ReadOnly = false;
        }


        //Enable after clicking NEW Button
        public void enableButtonsAfterClickingNew()
        {
            btnNew.Text = "SAVE";

            //Header Information
            btnSearchMember.Enabled = true;
            btnSearchLoan.Enabled = true;
            txtParticulars.ReadOnly = false;

            //Details Information
            dataGridView1.Enabled = true;
            btnAutoEntry.Enabled = true;

            //Command Buttons
            btnUpload.Enabled = true;
            btnClose.Text = "CANCEL";
        }

        private void btnSearchMember_Click(object sender, EventArgs e)
        {
            //Call Look Up
            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(JournalMemberClientLookUp))
                {
                    form.Activate();
                    return;
                }
            }
            
            JournalMemberClientLookUp LookUp = new JournalMemberClientLookUp();
            LookUp.Show();
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            
                //For Saving of Manual Entry 
                //===================================================================================
                //                              JOURNAL MANUAL ENTRY
                //===================================================================================

                //===================================================================================
                //                              JOURNAL POSTING
                //===================================================================================

                if (clsJournalVoucher.checkIfCancelled(txtJVNumber.Text) == true)
                {
                    //If Voucher already cancelled
                    Alert.show("Journal Voucher Already Cancelled!", Alert.AlertType.error);
                    return;
                }

                if (clsJournalVoucher.checkIfPosted(txtJVNumber.Text) == true)
                {
                    //If Voucher already Posted
                    Alert.show("Journal voucher already posted.", Alert.AlertType.error);
                    return;
                }

            if(Convert.ToDecimal(txtDebit.Text) != Convert.ToDecimal(txtCredit.Text))
            {
                Alert.show("Debit / Credit not Equal!", Alert.AlertType.error);
                return;
            }


                //===================================================================================
                //                       Are you sure you want to post this?
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
                        cmd.CommandText = "sp_PostingJournal";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@JV_No", txtJVNumber.Text);
                        cmd.Parameters.AddWithValue("@Posted_By", Classes.clsUser.Username);
                        cmd.ExecuteNonQuery();

                        //Success Message
                        Alert.show("Journal Voucher Successfully Posted!", Alert.AlertType.success);

                        //Display Message
                        status.Visible = true;
                        status.Text = "POSTED";

                        //Get Whos Posted IT
                        if (txtPostedBy.Text == "")
                        {
                            SqlDataAdapter adapterPosted = new SqlDataAdapter("SELECT Posted_By FROM Journal_Header WHERE JV_No ='" + txtJVNumber.Text + "'", con);
                            DataTable dtPosted = new DataTable();
                            adapterPosted.Fill(dtPosted);

                            txtPostedBy.Text = dtPosted.Rows[0].ItemArray[0].ToString();
                        }
                    }       
                }
                else
                {
                    return;
                }
            
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

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            panel11.Visible = false;
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[selectedRow].Cells[selected].Value = dataGridView3.SelectedRows[0].Cells["EmployeeID"].Value.ToString() + " - " + dataGridView3.SelectedRows[0].Cells["Name"].Value.ToString();
            dataGridView1.Rows[selectedRow].Cells[5].Value = dataGridView3.SelectedRows[0].Cells["userID"].Value.ToString();

            textBox1.Text = "";
            panel11.Visible = false;
        }

        private void dataGridView3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[selectedRow].Cells[selected].Value = dataGridView3.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
            dataGridView1.Rows[selectedRow].Cells[5].Value = dataGridView3.SelectedRows[0].Cells["userID"].Value.ToString();

            textBox1.Text = "";
            panel11.Visible = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();


                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT userID,EmployeeID,CASE WHEN Suffix is not null THEN (LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1) + Suffix) WHEN Suffix is null THEN (LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1)) END as Name From Membership where IsActive = 1 and IsApprove = 1 and EmployeeID like '%" + textBox1.Text + "%' or LastName like '%" + textBox1.Text + "%' or FirstName like '%" + textBox1.Text + "%' or MiddleName like '%" + textBox1.Text + "%'", con);
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

                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 25 userID,EmployeeID,(LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1) + Suffix) as Name From Membership where IsActive = 1 and IsApprove = 1", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView3.DataSource = dt;

                    dataGridView3.Columns["EmployeeID"].HeaderText = "ID";
                    dataGridView3.Columns["EmployeeID"].FillWeight = 30;
                    textBox1.Focus();
                }
                    
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            CrystalDecisions.Shared.TableLogOnInfo li;
            if(txtJVNumber.Text != "")
            {
                //Print Purposes
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    if (cmbTransaction.SelectedValue.ToString() == "TRAN002")
                    {
                        //Check first if he has a other deduction
                        SqlDataAdapter adapterCheck = new SqlDataAdapter("SELECT * FROM vw_GeneralJournalVoucher WHERE Loan_No ='" + txtLoanNumber.Text + "'", con);
                        DataTable dtCheck = new DataTable();
                        adapterCheck.Fill(dtCheck);

                        if (dtCheck.Rows.Count > 0)
                        {
                            //Call vw_GeneralJournal
                            //Generate GENERAL VOUCHER REPORT FOR LOANS
                            SqlCommand cmdGeneral = new SqlCommand();
                            cmdGeneral.Connection = con;
                            cmdGeneral.CommandText = "SELECT * FROM vw_GeneralJournalVoucher WHERE Loan_No = '" + txtLoanNumber.Text + "'";
                            cmdGeneral.CommandType = CommandType.Text;
                            cmdGeneral.ExecuteNonQuery();

                            SqlDataAdapter adapterGeneral = new SqlDataAdapter(cmdGeneral);



                            DataTable dtGeneral = new DataTable();
                            DataSet dsGeneral = new DataSet();

                            ReportsForms.JournalGeneralVoucher cr = new ReportsForms.JournalGeneralVoucher();
                            ReportsForms.GeneralVoucherJournal rpt = new ReportsForms.GeneralVoucherJournal();

                            li = new TableLogOnInfo();


                            adapterGeneral.Fill(dsGeneral, "vw_GeneralJournalVoucher");
                            dtGeneral = dsGeneral.Tables["vw_GeneralJournalVoucher"];
                            cr.SetDataSource(dsGeneral.Tables["vw_GeneralJournalVoucher"]);

                            //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                            cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

                            cr.SetParameterValue("savings", clsGeneral.returnSavings(txtLoanNumber.Text));
                            cr.SetParameterValue("sharecapital", clsGeneral.returnShareCapital(txtLoanNumber.Text));
                            cr.SetParameterValue("totalDeduction", clsGeneral.returnTotalDeduction(txtLoanNumber.Text));

                            SqlDataAdapter adapterBillDate = new SqlDataAdapter("SELECT top 3 Schedule_Payment FROM Loan_Details WHERE Loan_No = '" + txtLoanNumber.Text + "' ORDER BY Schedule_Payment ASC", con);
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
                            cmdGeneral.CommandText = "SELECT * FROM vw_GeneralJournalVoucherNoLoanDeduction WHERE Loan_No = '" + txtLoanNumber.Text + "'";
                            cmdGeneral.CommandType = CommandType.Text;
                            cmdGeneral.ExecuteNonQuery();

                            SqlDataAdapter adapterGeneral = new SqlDataAdapter(cmdGeneral);



                            DataTable dtGeneral = new DataTable();
                            DataSet dsGeneral = new DataSet();

                            ReportsForms.JournalGeneralVoucherNoDeduction cr = new ReportsForms.JournalGeneralVoucherNoDeduction();
                            ReportsForms.GeneralVoucherJournal rpt = new ReportsForms.GeneralVoucherJournal();

                            li = new TableLogOnInfo();


                            adapterGeneral.Fill(dsGeneral, "vw_GeneralJournalVoucherNoLoanDeduction");
                            dtGeneral = dsGeneral.Tables["vw_GeneralJournalVoucherNoLoanDeduction"];
                            cr.SetDataSource(dsGeneral.Tables["vw_GeneralJournalVoucherNoLoanDeduction"]);

                            //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                            cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

                            cr.SetParameterValue("savings", clsGeneral.returnSavings(txtLoanNumber.Text));
                            cr.SetParameterValue("sharecapital", clsGeneral.returnShareCapital(txtLoanNumber.Text));
                            cr.SetParameterValue("totalDeduction", clsGeneral.returnTotalDeduction(txtLoanNumber.Text));

                            SqlDataAdapter adapterBillDate = new SqlDataAdapter("SELECT top 3 Schedule_Payment FROM Loan_Details WHERE Loan_No = '" + txtLoanNumber.Text + "' ORDER BY Schedule_Payment ASC", con);
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
                        //=====================================================================================
                        //                   THIS IS FOR JOURNAL NOT CREATED IN LOAN
                        //=====================================================================================
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "sp_GetJournalDetailSummaryReport";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@JV_No", txtJVNumber.Text);

                        adapter = new SqlDataAdapter(cmd);



                        DataTable dt = new DataTable();
                        DataSet ds = new DataSet();

                        ReportsForms.crJournal cr = new ReportsForms.crJournal();
                        Reports.rptJournal rpt = new Reports.rptJournal();

                        li = new TableLogOnInfo();

                        li.ConnectionInfo.IntegratedSecurity = false;

                        adapter.Fill(ds, "vw_Journal");
                        dt = ds.Tables["vw_Journal"];
                        cr.SetDataSource(ds.Tables["vw_Journal"]);

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
                Alert.show("Please Select Journal Before Printing!", Alert.AlertType.warning);
                return;
            }
        }

        public void ForCancel()
        {
            btnClose.Text = "CLOSE";
            btnNew.Text = "NEW";
            btnEdit.Text = "EDIT";
            txtJVNumber.Text = "";
            txtLoanNumber.Text = "";
            txtParticulars.Text = "";
            txtParticulars.ReadOnly = true;
            txtMember.Text = "";
            btnSearchMember.Enabled = false;
            btnSearchLoan.Enabled = false;
            btnAutoEntry.Enabled = false;
            btnSearchJV.Enabled = true;
            btnNew.Enabled = true;



            Control control = new Control();
            //=====================================================================
            //                      Header Information
            //=====================================================================
            foreach (var c in panel5.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
                if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
            }

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
            dataGridView1.Enabled = false;

            //COmmand Buttons
            btnEdit.Enabled = false;
            btnPost.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            btnUpload.Enabled = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if(btnEdit.Text == "EDIT")
            {
                frmCancel = false;
                //Check if Search is Online
                foreach (Form form in Application.OpenForms)
                {
                    if (form.GetType() == typeof(searchJournal))
                    {
                        form.Close();
                        return;
                    }
                }

                //Check if Posted or Cancelled

                if (clsJournalVoucher.checkIfCancelled(txtJVNumber.Text) == true)
                {
                    //If Voucher already cancelled
                    Alert.show("Journal Voucher Already Cancelled!", Alert.AlertType.error);
                    return;
                }

                if (clsJournalVoucher.checkIfPosted(txtJVNumber.Text) == true)
                {
                    //If Voucher already Posted
                    Alert.show("Journal voucher already posted.", Alert.AlertType.error);
                    return;
                }

                //Change TEXT
                btnEdit.Text = "UPDATE";
                btnClose.Text = "CANCEL";

                //Disable Buttons not required
                btnNew.Enabled = false;
                btnPost.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = false;
                btnSearchJV.Enabled = false;
                btnUpload.Enabled = false;

                //Enable Fields
                dataGridView1.Enabled = true;
                txtParticulars.ReadOnly = false;
            }
            else
            {
                //Code For Validation
                if (cmbTransaction.Text == "" || cmbTransaction.Text == " - ")
                {
                    Alert.show("Transaction Type is Required!", Alert.AlertType.error);
                    return;
                }

                if (dataGridView1.Rows.Count <= 1)
                {
                    Alert.show("Details Information is required.", Alert.AlertType.error);
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

                //Coding For Updating The JV

                //==============================================================================================
                //                          CALL CONNECTION & SET PARAMETERS
                //==============================================================================================

                //==============================================================================================
                //                          FOR JOURNAL HEADER
                //==============================================================================================
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_UpdateJournalHeader";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@JV_No", txtJVNumber.Text);
                    cmd.Parameters.AddWithValue("@JV_Date", dtJVDate.Text);

                    //Check if Theres a Member or Client
                    if (Classes.clsJournalVoucher.userId.ToString() == "")
                    {
                        cmd.Parameters.AddWithValue("@userID", DBNull.Value);

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@userID", Classes.clsJournalVoucher.userId.ToString());
                    }

                    cmd.Parameters.AddWithValue("@AdjTo", txtMember.Text);
                    cmd.Parameters.AddWithValue("@Particulars", txtParticulars.Text);
                    cmd.Parameters.AddWithValue("@Loan_No", txtLoanNumber.Text);
                    cmd.Parameters.AddWithValue("@Transaction_Type", cmbTransaction.SelectedValue);
                    cmd.Parameters.AddWithValue("@summarize", checkBox1.Checked);
                    cmd.ExecuteNonQuery();

                    //==============================================================================================
                    //                          FOR JOURNAL DETAILS
                    //==============================================================================================

                    SqlCommand cmdDelete = new SqlCommand();
                    cmdDelete.Connection = con;
                    cmdDelete.CommandText = "sp_DeleteJournalDetail";
                    cmdDelete.CommandType = CommandType.StoredProcedure;
                    cmdDelete.Parameters.AddWithValue("@JV_No", txtJVNumber.Text);
                    cmdDelete.ExecuteNonQuery();

                    //==============================================================================================
                    //                          FOR JOURNAL DETAILS UPDATING
                    //==============================================================================================
                    if (dataGridView1.Rows.Count > 0)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells[0].Value != null)
                            {
                                SqlCommand cmdJD = new SqlCommand();
                                cmdJD.Connection = con;
                                cmdJD.CommandText = "sp_InsertJournalDetail";
                                cmdJD.CommandType = CommandType.StoredProcedure;
                                cmdJD.Parameters.AddWithValue("@JV_No", txtJVNumber.Text);
                                cmdJD.Parameters.AddWithValue("@Account_Code", row.Cells[0].Value);

                                if (DBNull.Value.Equals(row.Cells[5]))
                                {
                                    //NULL
                                    cmdJD.Parameters.AddWithValue("@userID", DBNull.Value);
                                }
                                else
                                {
                                    //Not Null
                                    cmdJD.Parameters.AddWithValue("@userID", Convert.ToInt32(row.Cells[5].Value));
                                }

                                if (row.Cells[1].Value != null)
                                {
                                    cmdJD.Parameters.AddWithValue("@Subsidiary_Code", row.Cells[1].Value);
                                }
                                else
                                {
                                    cmdJD.Parameters.AddWithValue("@Subsidiary_Code", DBNull.Value);
                                }


                                if (row.Cells[2].Value != null)
                                {
                                    cmdJD.Parameters.AddWithValue("@Loan_No", row.Cells[2].Value);
                                }
                                else
                                {
                                    cmdJD.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                                }

                                cmdJD.Parameters.AddWithValue("@Debit", Convert.ToDecimal(row.Cells[3].Value));
                                cmdJD.Parameters.AddWithValue("@Credit", Convert.ToDecimal(row.Cells[4].Value));
                                cmdJD.ExecuteNonQuery();
                            }
                        }
                    }
                }

                //Message Success Below
                Alert.show("Journal Voucher Successfully Updated.", Alert.AlertType.success);

                btnEdit.Text = "EDIT";

                //Disable all fields 
                txtParticulars.ReadOnly = true;
                dataGridView1.Enabled = false;
                btnUpload.Enabled = false;

                //Button Save change to NEW
                btnNew.Text = "NEW";
                btnEdit.Enabled = true;


                //enable button 
                btnCancel.Enabled = true;
                btnPrint.Enabled = true;
                btnPost.Enabled = true;
                btnNew.Enabled = true;
                btnSearchJV.Enabled = true;

                btnClose.Text = "CLOSE";
            }
        }

        private void btnSearchJV_Click(object sender, EventArgs e)
        {
            searchJournal frm = new searchJournal();
            frm.ShowDialog();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //For Saving of Manual Entry 
            //===================================================================================
            //                              JOURNAL MANUAL ENTRY
            //===================================================================================

            //===================================================================================
            //                              JOURNAL Cancellation
            //===================================================================================

            if (clsJournalVoucher.checkIfCancelled(txtJVNumber.Text) == true)
            {
                //If Voucher already cancelled
                Alert.show("Journal Voucher Already Cancelled!", Alert.AlertType.error);
                return;
            }

            if (clsJournalVoucher.checkIfPosted(txtJVNumber.Text) == true)
            {
                //If Voucher already Posted
                Alert.show("Journal voucher already posted.", Alert.AlertType.error);
                return;
            }

            if(frmCancel == false)
            {
                //enable particulars for cancelation note
                txtParticulars.ReadOnly = false;

                //remove particulars first
                txtParticulars.Text = "";
            }
                
            if(txtParticulars.Text == "")
            {
                Alert.show("Please put note on particulars before cancellation!", Alert.AlertType.error);
                txtParticulars.BackColor = Color.FromArgb(245, 149, 70);
                frmCancel = true;
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
                    cmd.CommandText = "sp_CancellationOfJournal";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@JV_No", txtJVNumber.Text);
                    cmd.Parameters.AddWithValue("@Cancel_Note", txtParticulars.Text);
                    cmd.Parameters.AddWithValue("@Cancelled_By", Classes.clsUser.Username);
                    cmd.ExecuteNonQuery();

                    //Success Message
                    Alert.show("Journal Voucher Successfully Cancelled!", Alert.AlertType.success);

                    //Display Message
                    status.Visible = true;
                    status.Text = "CANCELLED";

                    //Change particulars to readonly
                    txtParticulars.ReadOnly = true;
                    frmCancel = false;

                    //Get Cancelled By 
                    if (txtCancelled.Text == "")
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT Cancelled_By FROM Journal_Header WHERE JV_No = '" + txtJVNumber.Text + "'", con);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        txtCancelled.Text = dt.Rows[0].ItemArray[0].ToString();
                    }
                }
            }
            else
            {
                return;
            }

        }

        private void txtParticulars_TextChanged(object sender, EventArgs e)
        {
            txtParticulars.BackColor = Color.White;
        }

        private void panel16_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtLoanNumber_TextChanged(object sender, EventArgs e)
        {
            if(txtLoanNumber.Text != "")
            {
                txtLoanType.Text = clsJournalVoucher.returnLoanTypeDescription(txtLoanNumber.Text);
            }
            else
            {
                txtLoanType.Text = "";
            }
        }

        public void forNew()
        {
            Control control = new Control();
            //=====================================================================
            //                      Header Information
            //=====================================================================
            foreach (var c in panel5.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
                if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
            }

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
            

            txtParticulars.ReadOnly = false;
            btnEdit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            txtCredit.Text = "";
            txtDebit.Text = "";
            btnPost.Enabled = false;
            btnNew.Enabled = true;
        }
    }

}
