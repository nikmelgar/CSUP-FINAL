using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;

namespace WindowsFormsApplication2
{
    public partial class CashReceiptVoucher : Form
    {
        public CashReceiptVoucher()
        {
            InitializeComponent();
        }

        //===============================================================
        //                      Class and Forms
        //===============================================================
        Global global = new Global();
        Classes.clsCashReceipt clsCash = new Classes.clsCashReceipt();
        Classes.clsSearchCashReceipt clsSearchCash = new Classes.clsSearchCashReceipt();

        //===============================================================
        //                      MOUSE MOVE PANEL
        //===============================================================
        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        string part;
        //===============================================================
        //                      SQL PARAM
        //===============================================================
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;

        //===============================================================
        //                      Other Dec.
        //===============================================================
        int selected, selectedRow;


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
                if (form.GetType() == typeof(CashReceiptVoucher))
                {
                    form.Activate();
                    return;
                }
            }

            CashReceiptVoucher frm = new CashReceiptVoucher();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if(btnClose.Text == "CANCEL")
            {
                ForCancel();
            }
            else
            {
                this.Close();
            }
        }

        private void datagridviewTransaction_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 0)
                {
                    //Hide Panel loan type
                    panel38.Visible = false;
                    //// It returns the retangular area that represents the Display area for a cell  
                    Rectangle oRectangle = datagridviewTransaction.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    datagridviewTransaction.SelectedCells[e.ColumnIndex].ReadOnly = true;
                    ////Setting area for DateTimePicker Control  
                    //// Setting Location  
                    panelTrans.Location = new Point(oRectangle.X + 691, oRectangle.Y + 215);

                    selected = datagridviewTransaction.CurrentCell.ColumnIndex;
                    selectedRow = datagridviewTransaction.CurrentRow.Index;
                    // Now make it visible
                    panelTrans.Visible = true;

                    //load database
                    con = new SqlConnection();
                    global.connection(con);
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT Transaction_Code, REPLACE(transaction_code, 'TRAN', '') as Code,[Description] From Transaction_Type where isActive = 1 and transaction_code ='TRAN021' or (transaction_Code between 'TRAN001' and 'TRAN007')", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvTrans.DataSource = dt;
                    dgvTrans.Columns["Transaction_Code"].Visible = false;
                    dgvTrans.Columns["Code"].FillWeight = 30;

                }
                else if(e.ColumnIndex == 1)
                {
                    try
                    {
                        //Stored object of user id Classes.clsCashReceipt.userID;
                        if (datagridviewTransaction.Rows[selectedRow].Cells[0].Value.ToString() == "002")
                        {
                            //Hide panel transaction
                            panelTrans.Visible = false;
                            //// It returns the retangular area that represents the Display area for a cell  
                            Rectangle oRectangle = datagridviewTransaction.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                            ////Setting area for DateTimePicker Control  
                            //// Setting Location  
                            panel38.Location = new Point(oRectangle.X + 691, oRectangle.Y + 215);

                            selected = datagridviewTransaction.CurrentCell.ColumnIndex;
                            selectedRow = datagridviewTransaction.CurrentRow.Index;


                            //load database
                            con = new SqlConnection();
                            global.connection(con);

                            if (txtPayorID.Text != "")
                            {
                                cmd = new SqlCommand();
                                cmd.Connection = con;
                                cmd.CommandText = "sp_ReturnLoanTypesPerUser";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@userid", Classes.clsCashReceipt.userID);

                                //Put in datatable
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                DataTable dt = new DataTable();
                                adapter.Fill(dt);

                                //If Theres a loan then make it visible
                                // Now make it visible
                                if (dt.Rows.Count > 0)
                                {
                                    panel38.Visible = true;
                                }
                                else
                                {
                                    panel38.Visible = false;
                                }

                                dataGridView2.DataSource = dt;
                                dataGridView2.Columns["Loan_Type"].FillWeight = 30;
                                dataGridView2.Columns["Loan_Type"].HeaderText = "Type";
                                dataGridView2.Columns["Loan_Description"].HeaderText = "Description";

                                //Hide other columns
                                dataGridView2.Columns["Loan_No"].Visible = false;
                                dataGridView2.Columns["userID"].Visible = false;
                                dataGridView2.Columns["CurrentDr"].Visible = false;
                                dataGridView2.Columns["PastDueDr"].Visible = false;
                                dataGridView2.Columns["Balance"].Visible = false;
                                dataGridView2.Columns["Deferred"].Visible = false;
                            }
                        }
                    }
                    catch
                    {

                    }                               

                }
                else
                {
                    selected = datagridviewTransaction.CurrentCell.ColumnIndex;
                    panelTrans.Visible = false;
                    panel38.Visible = false;
                }
            }
        }

        public void populateDatagridCombobox()
        {
            DataGridViewComboBoxColumn cbCell = (DataGridViewComboBoxColumn)datagridviewTransaction.Columns[0];

            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT Transaction_Code, REPLACE(transaction_code, 'TRAN', '') as Code,[Description] From Transaction_Type where isActive = 1 and transaction_code ='TRAN021' or (transaction_Code between 'TRAN001' and 'TRAN007')", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);


            cbCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            cbCell.AutoComplete = true;
            cbCell.DisplayMember = "Code";
            cbCell.ValueMember = "Code";
            cbCell.DataSource = dt;
        }

        private void CashReceiptVoucher_Load(object sender, EventArgs e)
        {
            populateDatagridCombobox();//Trans
            populateDatagridComboboxDetails();//Details
            clsCash.loadBank(cmbBank);
            
            txtPreparedBy.Text = Classes.clsUser.Username;
        }

        private void datagridviewTransaction_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not Valid.")
            {
                object value = dgvChecks.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dgvChecks.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dgvChecks.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        private void dgvTrans_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            datagridviewTransaction.Rows[selectedRow].Cells[selected].Value = dgvTrans.SelectedRows[0].Cells["Code"].Value.ToString();
            panelTrans.Visible = false;
        }

        private void datagridviewTransaction_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewTransaction.Columns[e.ColumnIndex].Name == "Amount")
            {
                //your code goes here
                datagridviewTransaction.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Convert.ToDecimal(datagridviewTransaction.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString("#,0.00");
            }
        }

        private void datagridviewTransaction_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

            if (String.IsNullOrEmpty(datagridviewTransaction.CurrentRow.Cells[0].Value as String))
            {
                datagridviewTransaction.CurrentRow.Cells[2].ReadOnly = true;
            }
            else
            {
                datagridviewTransaction.CurrentRow.Cells[2].ReadOnly = false;
            }

            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (datagridviewTransaction.CurrentCell.ColumnIndex == 2) //Desired Column
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
                if (datagridviewTransaction.CurrentRow.IsNewRow)
                {
                    Alert.show("New row cannot be deleted!", Alert.AlertType.error);
                    return;
                }

                

                string msg = Environment.NewLine + "Are you sure you want to delete this entry?";
                DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    foreach(DataGridViewCell cell in datagridviewTransaction.SelectedCells)
                    {
                        int row = datagridviewTransaction.CurrentCell.RowIndex;
                        datagridviewTransaction.Rows.RemoveAt(row);
                        compute();
                    }         
                }
                else
                {
                    return;
                }
            }
            catch
            {

            }
        }

        private void dgvTrans_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            datagridviewTransaction.Rows[selectedRow].Cells[selected].Value = dgvTrans.SelectedRows[0].Cells["Code"].Value.ToString();

            datagridviewTransaction.CurrentRow.Cells[2].ReadOnly = false;
            panelTrans.Visible = false;
        }

        private void datagridviewTransaction_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewTransaction.Columns[e.ColumnIndex].Name == "Amount")
            {
                //your code goes here
                compute();
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
                        Classes.clsLookUp.whosLookUp = "2";
                        return;
                    }
                }

                Classes.clsLookUp.whosLookUp = "2";
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
                        ClientLookUp.whosLookUp = "2";
                        return;
                    }
                }

                ClientLookUp LookUp = new ClientLookUp();
                ClientLookUp.whosLookUp = "2";
                LookUp.ShowDialog();
            }
        }

        private void radioMember_CheckedChanged(object sender, EventArgs e)
        {
            txtPayorID.Text = "";
            txtPayorCompany.Text = "";
            txtPayorName.Text = "";
        }

        private void radioClient_CheckedChanged(object sender, EventArgs e)
        {
            txtPayorID.Text = "";
            txtPayorCompany.Text = "";
            txtPayorName.Text = "";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioCash.Checked == true)
            {
                cmbBank.Enabled = false;
                txtCheckAmount.Enabled = false;
                txtCheckNumber.Enabled = false;
                dtCheckDate.Enabled = false;
                btnAddCheck.Enabled = false;
                btnRemoveCheck.Enabled = false;
                dgvChecks.Enabled = false;
            }
            else
            {
                cmbBank.Enabled = true;
                txtCheckAmount.Enabled = true;
                txtCheckNumber.Enabled = true;
                dtCheckDate.Enabled = true;
                btnAddCheck.Enabled = true;
                btnRemoveCheck.Enabled = true;
                dgvChecks.Enabled = true;
            }
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 0)
                {
                    //Initialized a new DateTimePicker Control  
                    ////Adding DateTimePicker control into DataGridView   
                    //dataGridView1.Controls.Add(panel1);

                    //// It returns the retangular area that represents the Display area for a cell  
                    Rectangle oRectangle = dataGridView3.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    ////Setting area for DateTimePicker Control  
                    //// Setting Location  
                    panelDetails.Location = new Point(oRectangle.X + 3, oRectangle.Y + 25);
                   
                    selected = dataGridView3.CurrentCell.ColumnIndex;
                    selectedRow = dataGridView3.CurrentRow.Index;

                    dataGridView3.SelectedCells[selected].ReadOnly = true;
                    // Now make it visible  
                    panelDetails.Visible = true;
                    panel14.Visible = false;

                    //load database
                    con = new SqlConnection();
                    global.connection(con);
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 25 Account_code,Account_Description from chart_of_Accounts", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvDetails.DataSource = dt;

                    dgvDetails.Columns["Account_code"].HeaderText = "Code";
                    dgvDetails.Columns["Account_code"].FillWeight = 30;

                    dgvDetails.Columns["Account_Description"].HeaderText = "Description";
                    txtSearch.Focus();
                }
                else if (e.ColumnIndex == 1)
                {
                    //Initialized a new DateTimePicker Control  
                    ////Adding DateTimePicker control into DataGridView   
                    //dataGridView1.Controls.Add(panel1);

                    //// It returns the retangular area that represents the Display area for a cell  
                    Rectangle oRectangle = dataGridView3.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    ////Setting area for DateTimePicker Control  
                    //// Setting Location  
                    panel14.Location = new Point(oRectangle.X + 3, oRectangle.Y + 25);
                    selected = dataGridView3.CurrentCell.ColumnIndex;
                    selectedRow = dataGridView3.CurrentRow.Index;
                    // Now make it visible  
                    panel14.Visible = true;
                    //disable panel account description
                    panelDetails.Visible = false;
                    //load database
                    con = new SqlConnection();
                    global.connection(con);
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 25 userID,EmployeeID,(LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1) + Suffix) as Name From Membership where IsActive = 1 and IsApprove = 1", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;

                    dataGridView1.Columns["userID"].Visible = false;

                    dataGridView1.Columns["EmployeeID"].HeaderText = "ID";
                    dataGridView1.Columns["EmployeeID"].FillWeight = 30;


                    textBox1.Focus();
                }
                else
                {
                    selected = dataGridView3.CurrentCell.ColumnIndex;
                    panelDetails.Visible = false;
                    panel14.Visible = false;
                }
            }
        }

        private void dataGridView3_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not Valid.")
            {
                object value = dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dataGridView3.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dataGridView3.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        public void compute()
        {
            decimal sumTransAmount = 0;
            //Check if theres a beneficiary
            if (datagridviewTransaction.Rows.Count > 0)
            {

                for (int i = 0; i < datagridviewTransaction.Rows.Count; ++i)
                {
                    sumTransAmount += Convert.ToDecimal(datagridviewTransaction.Rows[i].Cells[2].Value);
                }
            }

            txtTransAmount.Text = sumTransAmount.ToString("#,0.00");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            panelDetails.Visible = false;
        }

        private void dgvDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView3.Rows[selectedRow].Cells[selected].Value = dgvDetails.SelectedRows[0].Cells["Account_code"].Value.ToString();

            dataGridView3.CurrentRow.Cells[3].ReadOnly = false;
            dataGridView3.CurrentRow.Cells[4].ReadOnly = false;
            panelDetails.Visible = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchCashReceipt searchCash = new searchCashReceipt();
            searchCash.ShowDialog();
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView3.CurrentRow.IsNewRow)
                {
                    Alert.show("New row cannot be deleted!", Alert.AlertType.error);
                    return;
                }



                string msg = Environment.NewLine + "Are you sure you want to delete this entry?";
                DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewCell cell in dataGridView3.SelectedCells)
                    {
                        int row = dataGridView3.CurrentCell.RowIndex;
                        dataGridView3.Rows.RemoveAt(row);
                        computeDetails();
                    }
                }
                else
                {
                    return;
                }
            }
            catch
            {

            }
        }

        private void dataGridView3_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView3.Columns[e.ColumnIndex].Name == "Debit")
            {
                //your code goes here
                dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Convert.ToDecimal(dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString("#,0.00");
            }
            else if (dataGridView3.Columns[e.ColumnIndex].Name == "Credit")
            {
                //Credit Code goes here
                dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Convert.ToDecimal(dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString("#,0.00");
            }
        }

        private void dataGridView3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView3.Columns[e.ColumnIndex].Name == "Debit")
            {
                //your code goes here
                computeDetails();
            }
            else if (dataGridView3.Columns[e.ColumnIndex].Name == "Credit")
            {
                //Credit Code goes here
                computeDetails();
            }
        }

        public void populateDatagridComboboxDetails()
        {
            DataGridViewComboBoxColumn cbCell = (DataGridViewComboBoxColumn)dataGridView3.Columns[0];

            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT account_Code,account_Description From chart_of_Accounts", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);


            cbCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            cbCell.AutoComplete = true;
            cbCell.DisplayMember = "account_Description";
            cbCell.ValueMember = "account_code";
            cbCell.DataSource = dt;
        }

        private void dataGridView3_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (String.IsNullOrEmpty(dataGridView3.CurrentRow.Cells[0].Value as String))
            {
                dataGridView3.CurrentRow.Cells[3].ReadOnly = true;
                dataGridView3.CurrentRow.Cells[4].ReadOnly = true;
            }
            else
            {
                dataGridView3.CurrentRow.Cells[3].ReadOnly = false;
                dataGridView3.CurrentRow.Cells[4].ReadOnly = false;
            }


            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dataGridView3.CurrentCell.ColumnIndex == 3) //Desired Column
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
            else if (dataGridView3.CurrentCell.ColumnIndex == 4)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text != "")
            {
                con = new SqlConnection();
                global.connection(con);

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Account_Code,Account_Description from chart_of_accounts where Account_Code like '%" + txtSearch.Text + "%' or account_Description like '%" + txtSearch.Text + "%'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvDetails.DataSource = dt;

                dgvDetails.Columns["Account_code"].HeaderText = "Code";
                dgvDetails.Columns["Account_code"].FillWeight = 30;

                dgvDetails.Columns["Account_Description"].HeaderText = "Description";
                txtSearch.Focus();
            }
            else
            {
                con = new SqlConnection();
                global.connection(con);
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 25 Account_code,Account_Description from chart_of_Accounts", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvDetails.DataSource = dt;

                dgvDetails.Columns["Account_code"].HeaderText = "Code";
                dgvDetails.Columns["Account_code"].FillWeight = 30;

                dgvDetails.Columns["Account_Description"].HeaderText = "Description";
                txtSearch.Focus();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (datagridviewTransaction.Rows.Count >= 1)
            {
                if(txtPayorID.Text == "")
                {
                    Alert.show("Please select Member first!", Alert.AlertType.warning);
                    return;
                }

                if (txtTransAmount.Text == "0.00" || txtTransAmount.Text == "")
                {
                    Alert.show("Please select transaction first and put Amount!", Alert.AlertType.warning);
                    return;
                }

                if(radioMember.Checked == true)
                {
                    con = new SqlConnection();
                    global.connection(con);

                    dataGridView3.Rows.Clear();
                    int x, y;
                    x = datagridviewTransaction.Rows.Count - 1;
                    y = 0; //Set Count

                    while (y != x)
                    {
                        //Savings || Loans || Share Capital
                        if (datagridviewTransaction.Rows[y].Cells[0].Value.ToString() == "001" || datagridviewTransaction.Rows[y].Cells[0].Value.ToString() == "002" || datagridviewTransaction.Rows[y].Cells[0].Value.ToString() == "003")
                        {
                            ArrayList row = new ArrayList();
                            switch (datagridviewTransaction.Rows[y].Cells[0].Value.ToString())
                            {
                                case "001":
                                    //Savings
                                    row.Add("300.1");
                                    row.Add(txtPayorID.Text + " - " + txtPayorName.Text);
                                    row.Add("");
                                    row.Add("0.00");
                                    row.Add(datagridviewTransaction.Rows[y].Cells[2].Value.ToString());
                                    row.Add(Classes.clsCashReceipt.userID.ToString());
                                    dataGridView3.Rows.Add(row.ToArray());
                                    break;
                                case "002":
                                    //Loans

                                    cmd = new SqlCommand();
                                    cmd.Connection = con;
                                    cmd.CommandText = "sp_ReturnLoanTypesPerUserSEARCH";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@userid", Classes.clsCashReceipt.userID);
                                    cmd.Parameters.AddWithValue("@description", datagridviewTransaction.Rows[y].Cells[1].Value.ToString());

                                    //Put in datatable
                                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                    DataTable dt = new DataTable();
                                    adapter.Fill(dt);

                                    //Declare Double first
                                    Double balance, deferred, amountInput, Excess;

                                    //Set Variable Values
                                    balance = Convert.ToDouble(dt.Rows[0].ItemArray[6].ToString());
                                    deferred = Convert.ToDouble(dt.Rows[0].ItemArray[7].ToString());
                                    amountInput = Convert.ToDouble(datagridviewTransaction.Rows[y].Cells[2].Value.ToString());

                                    //Has a deferred loan
                                    if(deferred != 0.00 || deferred != 0)
                                    {
                                        if(amountInput > deferred)
                                        {
                                            //Payment is greater than deferred 
                                            //Past Due First
                                            Excess = amountInput - deferred;

                                            row.Add(dt.Rows[0].ItemArray[5].ToString()); //PastDue Account
                                            row.Add(txtPayorID.Text + " - " + txtPayorName.Text);
                                            row.Add(dt.Rows[0].ItemArray[0].ToString()); //Loan Number
                                            row.Add("0.00");
                                            row.Add(Convert.ToDecimal(deferred).ToString("#,0.00"));
                                            row.Add(Classes.clsCashReceipt.userID.ToString());
                                            dataGridView3.Rows.Add(row.ToArray());

                                            //Current Loan
                                            row.Add(dt.Rows[0].ItemArray[4].ToString()); //Current Account
                                            row.Add(txtPayorID.Text + " - " + txtPayorName.Text);
                                            row.Add(dt.Rows[0].ItemArray[0].ToString()); //Loan Number
                                            row.Add("0.00");
                                            row.Add(Convert.ToDecimal(Excess).ToString("#,0.00"));
                                            row.Add(Classes.clsCashReceipt.userID.ToString());
                                            dataGridView3.Rows.Add(row.ToArray());
                                        }
                                        else //For exact and Less than the amount
                                        {
                                            //Exact Amount 
                                            row.Add(dt.Rows[0].ItemArray[5].ToString()); //PastDue Account
                                            row.Add(txtPayorID.Text + " - " + txtPayorName.Text);
                                            row.Add(dt.Rows[0].ItemArray[0].ToString()); //Loan Number
                                            row.Add("0.00");
                                            row.Add(datagridviewTransaction.Rows[y].Cells[2].Value.ToString());
                                            row.Add(Classes.clsCashReceipt.userID.ToString());
                                            dataGridView3.Rows.Add(row.ToArray());
                                        }
                                    }
                                    else
                                    {
                                        //No Deferred Loan
                                        //Current Loan
                                        row.Add(dt.Rows[0].ItemArray[4].ToString()); //Current Account
                                        row.Add(txtPayorID.Text + " - " + txtPayorName.Text);
                                        row.Add(dt.Rows[0].ItemArray[0].ToString()); //Loan Number
                                        row.Add("0.00");
                                        row.Add(Convert.ToDecimal(datagridviewTransaction.Rows[y].Cells[2].Value.ToString()).ToString("#,0.00"));
                                        row.Add(Classes.clsCashReceipt.userID.ToString());
                                        dataGridView3.Rows.Add(row.ToArray());
                                    }


                                    //Restore Variables to 0;
                                    balance = 0;
                                    deferred = 0;
                                    amountInput = 0;
                                    Excess = 0;
                                    break;
                                case "003":
                                    //Share Capital
                                    row.Add("363");
                                    row.Add(txtPayorID.Text + " - " + txtPayorName.Text);
                                    row.Add("");
                                    row.Add("0.00");
                                    row.Add(datagridviewTransaction.Rows[y].Cells[2].Value.ToString());
                                    row.Add(Classes.clsCashReceipt.userID.ToString());
                                    dataGridView3.Rows.Add(row.ToArray());
                                    break;
                            }

                        }

                        y = y + 1;
                    }
                    //Last Record for Payment
                    ArrayList row1 = new ArrayList();
                    if (radioCash.Checked == true || radioPecciCheck.Checked == true)
                    {
                        //Cash
                        row1.Add("101");
                        row1.Add("");
                        row1.Add("");
                        row1.Add(txtTransAmount.Text);
                        row1.Add("0.00");
                        row1.Add("0");
                        dataGridView3.Rows.Add(row1.ToArray());
                    }
                    else
                    {
                        //COCI
                        row1.Add("105");
                        row1.Add("");
                        row1.Add("");
                        row1.Add(txtTransAmount.Text);
                        row1.Add("0.00");
                        row1.Add("0");
                        dataGridView3.Rows.Add(row1.ToArray());
                    }

                    //var index = dataGridView3.Rows.Add();
                    //dataGridView3.Rows[index].Cells[0].Value = "101";
                    //dataGridView3.Rows[index].Cells[3].Value = txtTransAmount.Text;
                    computeDetails();
                }
            }

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            panel14.Visible = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView3.Rows[selectedRow].Cells[selected].Value = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString() + " - " + dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString();
            dataGridView3.Rows[selectedRow].Cells[5].Value = dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString();

            textBox1.Text = "";
            panel14.Visible = false;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //For Creating Cash Receipt and Saving
            if(btnNew.Text == "NEW")
            {
                InitializedFromNew();
            }
            else
            {
                //SAVING CREATED CASH RECEIPTS

                //Validation First Before Saving
                if(clsCash.CashValidation(txtORNo,txtPayorID,txtTransAmount,txtDebit,txtCredit,datagridviewTransaction,dataGridView3,dgvChecks,radioCash,radioPecciCheck,radioNonPecciCheck) == false)
                {
                    //Continue to saving
                    //=================================================================================================
                    //                              SQL CONNECTION
                    //=================================================================================================
                    con = new SqlConnection();
                    global.connection(con);


                    //=================================================================================================
                    //                              FOR CASH TRANSACTION
                    //=================================================================================================

                    //=================================================================================================
                    //                              CASH RECEIPT HEADER
                    //=================================================================================================
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_InsertCashReceiptsHeader";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Or_No", txtORNo.Text);
                    cmd.Parameters.AddWithValue("@Or_Date", dtOrDate.Text);

                    //FOr Payee Type 
                    //Member = 0 Client = 1
                    if (radioMember.Checked == true)
                    {
                        cmd.Parameters.AddWithValue("@Payor_Type", "0");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Payor_Type", "1");
                    }

                    //Check if Theres a Member or Client
                    if (Classes.clsCashReceipt.userID.ToString() == "")
                    {
                        cmd.Parameters.AddWithValue("@userID", DBNull.Value);

                    }
                    else if (Classes.clsCashReceipt.userID == 0)
                    {
                        cmd.Parameters.AddWithValue("@userID", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@userID", Classes.clsCashReceipt.userID.ToString());
                    }

                    cmd.Parameters.AddWithValue("@Payor", txtPayorID.Text);
                    cmd.Parameters.AddWithValue("@Particulars", txtParticulars.Text);

                    //============= FOr Collection Type
                    //0 = Cash
                    //1 = Pecci Check
                    //2 = Non-Pecci Check
                    if(radioCash.Checked == true)
                    {
                        //Cash
                        cmd.Parameters.AddWithValue("@Collection_Type", "0");
                        cmd.Parameters.AddWithValue("@Bank", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Check_No", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Check_Date", DBNull.Value);
                    }
                    else if(radioPecciCheck.Checked == true)
                    {
                        //Pecci Check
                        cmd.Parameters.AddWithValue("@Collection_Type", "1");
                        cmd.Parameters.AddWithValue("@Bank", dgvChecks.Rows[0].Cells[0].Value.ToString());
                        cmd.Parameters.AddWithValue("@Check_Date", dgvChecks.Rows[0].Cells[2].Value.ToString());
                        cmd.Parameters.AddWithValue("@Check_No", dgvChecks.Rows[0].Cells[3].Value.ToString());
                        
                    }
                    else
                    {
                        //Non-Pecci
                        cmd.Parameters.AddWithValue("@Collection_Type", "2");
                        cmd.Parameters.AddWithValue("@Bank", dgvChecks.Rows[0].Cells[0].Value.ToString());
                        cmd.Parameters.AddWithValue("@Check_Date", dgvChecks.Rows[0].Cells[2].Value.ToString());
                        cmd.Parameters.AddWithValue("@Check_No", dgvChecks.Rows[0].Cells[3].Value.ToString());
                    }

                    cmd.Parameters.AddWithValue("@Prepared_By", txtPreparedBy.Text);

                    cmd.ExecuteNonQuery(); //SAVING CASH HEADER


                    //=================================================================================================
                    //                              CASH RECEIPT DETAIL
                    //=================================================================================================
                    if (dataGridView3.Rows.Count > 0) //DETAIL GRID
                    {
                        foreach (DataGridViewRow row in dataGridView3.Rows)
                        {
                            if (row.Cells[0].Value != null) //Not New ROW
                            {
                                SqlCommand cmdDetail = new SqlCommand();
                                cmdDetail.Connection = con;
                                cmdDetail.CommandText = "sp_InsertCashReceiptsDetail";
                                cmdDetail.CommandType = CommandType.StoredProcedure;
                                cmdDetail.Parameters.AddWithValue("@Or_No", txtORNo.Text);
                                cmdDetail.Parameters.AddWithValue("@Account_Code", row.Cells[0].Value);

                                if (Convert.ToInt32(row.Cells[5].Value) == 0 || row.Cells[5].Value.ToString() == "")
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
                                cmdDetail.ExecuteNonQuery(); //SAVING CASH DETAILS
                            }
                        }
                    }



                    //=================================================================================================
                    //                              CASH TRANSACTION
                    //=================================================================================================
                    if(datagridviewTransaction.Rows.Count > 0)
                    {
                        foreach (DataGridViewRow row in datagridviewTransaction.Rows)
                        {
                            if(row.Cells[0].Value != null)
                            {
                                SqlCommand cmdTrans = new SqlCommand();
                                cmdTrans.Connection = con;
                                cmdTrans.CommandText = "sp_InsertCashReceiptsTrans";
                                cmdTrans.CommandType = CommandType.StoredProcedure;
                                cmdTrans.Parameters.AddWithValue("@Or_No", txtORNo.Text);
                                cmdTrans.Parameters.AddWithValue("@Transaction_Type", row.Cells[0].Value);

                                if (row.Cells[1].Value != null)
                                {
                                    cmdTrans.Parameters.AddWithValue("@Loan_No", row.Cells[1].Value);
                                }
                                else
                                {
                                    cmdTrans.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                                }

                                cmdTrans.Parameters.AddWithValue("@Amount", Convert.ToDecimal(row.Cells[2].Value));
                                cmdTrans.ExecuteNonQuery(); //SAVING TRANSACTION
                            }
                        }
                    }

                    //=================================================================================================
                    //                         BANK FOR CHECK IF COLLECTION MODE IS NOT CASH
                    //=================================================================================================

                    if(radioPecciCheck.Checked == true || radioNonPecciCheck.Checked == true)
                    {
                        if (dgvChecks.Rows.Count > 0)
                        {
                            foreach (DataGridViewRow row in dgvChecks.Rows) 
                            {
                                if (row.Cells[0].Value != null)
                                {
                                    SqlCommand cmdchek = new SqlCommand();
                                    cmdchek.Connection = con;
                                    cmdchek.CommandText = "sp_InsertCashReceiptsChecks";
                                    cmdchek.CommandType = CommandType.StoredProcedure;
                                    cmdchek.Parameters.AddWithValue("@Or_No", txtORNo.Text);
                                    cmdchek.Parameters.AddWithValue("@Bank", row.Cells[0].Value);
                                    cmdchek.Parameters.AddWithValue("@Amount", row.Cells[1].Value);
                                    cmdchek.Parameters.AddWithValue("@Check_Date", row.Cells[2].Value);
                                    cmdchek.Parameters.AddWithValue("@Check_No", row.Cells[3].Value);
                                    cmdchek.ExecuteNonQuery(); //SAVING TRANSACTION
                                }
                            }
                        }
                    }

                    Alert.show("Cash Receipts Successfully Created!", Alert.AlertType.success);
                    //RESTORE COMMANDS AND TEXT
                    AfterSavingOrUpdating();
                }
            }
        }

        public void computeDetails()
        {
            decimal sumDR = 0;
            decimal sumCR = 0;
            //Check if theres a beneficiary
            if (dataGridView3.Rows.Count > 0)
            {

                for (int i = 0; i < dataGridView3.Rows.Count; ++i)
                {
                    sumDR += Convert.ToDecimal(dataGridView3.Rows[i].Cells[3].Value);
                    sumCR += Convert.ToDecimal(dataGridView3.Rows[i].Cells[4].Value);
                }
            }

            txtDebit.Text = sumDR.ToString("#,0.00");
            txtCredit.Text = sumCR.ToString("#,0.00");
        }

        private void btnAddCheck_Click(object sender, EventArgs e)
        {
            var index = dgvChecks.Rows.Add();
            dgvChecks.Rows[index].Cells[0].Value = cmbBank.Text;
            dgvChecks.Rows[index].Cells[1].Value = txtCheckAmount.Text;
            dgvChecks.Rows[index].Cells[2].Value = dtCheckDate.Text;
            dgvChecks.Rows[index].Cells[3].Value = txtCheckNumber.Text;

            //Clear For Adding Additional Checks
            cmbBank.Text = "";
            txtCheckAmount.Text = "";
            txtCheckNumber.Text = "";
            dtCheckDate.Value = DateTime.Today;
        }

        private void btnRemoveCheck_Click(object sender, EventArgs e)
        {
            string msg = Environment.NewLine + "Are you sure you want to delete this check?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dgvChecks.SelectedRows)
                {
                    dgvChecks.Rows.Remove(row);
                }
            }
            else
            {
                return;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                con = new SqlConnection();
                global.connection(con);

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT userID,EmployeeID,(LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1) + Suffix) as Name From Membership where IsActive = 1 and IsApprove = 1 and EmployeeID like '%" + textBox1.Text + "%' or LastName like '%" + textBox1.Text + "%' or FirstName like '%" + textBox1.Text + "%' or MiddleName like '%" + textBox1.Text + "%'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;

                dataGridView1.Columns["userID"].Visible = false;

                dataGridView1.Columns["EmployeeID"].HeaderText = "ID";
                dataGridView1.Columns["EmployeeID"].FillWeight = 30;
                textBox1.Focus();
            }
            else
            {
                con = new SqlConnection();
                global.connection(con);
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 25 userID,EmployeeID,(LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1) + Suffix) as Name From Membership where IsActive = 1 and IsApprove = 1", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;

                dataGridView1.Columns["EmployeeID"].HeaderText = "ID";
                dataGridView1.Columns["EmployeeID"].FillWeight = 30;
                textBox1.Focus();
            }
        }

        //================================================================================================================
        //                                           INITIALIZED FROM NEW
        //================================================================================================================
        public void InitializedFromNew()
        {
            //=======================================
            //          Command buttons
            //=======================================
            btnEdit.Enabled = false;
            btnPost.Enabled = false;
            btnCancel.Enabled = false;
            btnSearch.Enabled = false;
            btnClose.Text = "CANCEL";
            btnNew.Text = "SAVE";

            status.Visible = false;
            //=======================================
            //          Enable Fields
            //=======================================
            txtORNo.Enabled = true;
            txtParticulars.Enabled = true;
            datagridviewTransaction.Enabled = true;
            dataGridView3.Enabled = true;

            radioCash.Checked = true;
            //=======================================
            //          Clear All Fields
            //=======================================
            txtParticulars.Text = "";

            //=======================================
            //          Header Information
            //=======================================

            txtORNo.Text = "";
            dtOrDate.Value = DateTime.Today;
            txtPayorID.Text = "";
            txtPayorName.Text = "";
            txtPayorCompany.Text = "";

            //=======================================
            //       Bank / Check Information
            //=======================================
            cmbBank.Text = "";
            txtCheckAmount.Text = "";
            txtCheckNumber.Text = "";
            dtCheckDate.Value = DateTime.Today;
            dgvChecks.Rows.Clear();

            //=======================================
            //       Transaction Information
            //=======================================
            datagridviewTransaction.Rows.Clear();
            txtTransAmount.Text = "";

            //=======================================
            //       Details Information
            //=======================================
            dataGridView3.Rows.Clear();
            txtCredit.Text = "";
            txtDebit.Text = "";

            //=======================================
            //       Footer Information
            //=======================================
            txtPostedBy.Text = "";
            txtCancelledBy.Text = "";

        }

        public void ForCancel()
        {
            //=======================================
            //          Command buttons
            //=======================================
            btnEdit.Enabled = false;
            btnPost.Enabled = false;
            btnCancel.Enabled = false;
            btnSearch.Enabled = true;
            btnNew.Enabled = true;
            btnClose.Text = "CLOSE";
            btnEdit.Text = "EDIT";
            btnNew.Text = "NEW";

            radioCash.Checked = true;

            //=======================================
            //          Disable Fields
            //=======================================
            txtORNo.Enabled = false;
            txtParticulars.Enabled = false;
            datagridviewTransaction.Enabled = false;
            dataGridView3.Enabled = false;

            //=======================================
            //          Header Information
            //=======================================

            txtORNo.Text = "";
            dtOrDate.Value = DateTime.Today;
            txtPayorID.Text = "";
            txtPayorName.Text = "";
            txtPayorCompany.Text = "";

            //=======================================
            //       Bank / Check Information
            //=======================================
            cmbBank.Text = "";
            txtCheckAmount.Text = "";
            txtCheckNumber.Text = "";
            dtCheckDate.Value = DateTime.Today;
            dgvChecks.Rows.Clear();

            //=======================================
            //       Transaction Information
            //=======================================
            datagridviewTransaction.Rows.Clear();
            txtTransAmount.Text = "";

            //=======================================
            //       Details Information
            //=======================================
            dataGridView3.Rows.Clear();
            txtCredit.Text = "";
            txtDebit.Text = "";

            //=======================================
            //       Footer Information
            //=======================================
            txtPostedBy.Text = "";
            txtCancelledBy.Text = "";

        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            //===================================================================================
            //                          CASH RECEIPTS POSTING
            //===================================================================================

            if(clsSearchCash.checkIfPosted(txtORNo.Text) == true)
            {
                //If Voucher already cancelled
                Alert.show("Cash Receipt Already Posted!", Alert.AlertType.error);
                return;
            }

            if (clsSearchCash.checkIfCancelled(txtORNo.Text) == true)
            {
                //If Voucher already cancelled
                Alert.show("Cash Receipt Already Cancelled!", Alert.AlertType.error);
                return;
            }

            string msg = Environment.NewLine + "Are you sure you want to continue?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //Code for posting
                con = new SqlConnection();
                global.connection(con);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_PostingCashReceipt";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Or_No", txtORNo.Text);
                cmd.Parameters.AddWithValue("@Posted_By", Classes.clsUser.Username);
                cmd.ExecuteNonQuery();


                //Success Message
                Alert.show("Cash Receipts Successfully Posted!", Alert.AlertType.success);

                //Display Message
                status.Visible = true;
                status.Text = "POSTED";

                //Get Whos Posted IT
                if (txtPostedBy.Text == "")
                {
                    txtPostedBy.Text = clsSearchCash.getPostedBy(txtORNo.Text);
                }
            }
            else
            {
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            part = txtParticulars.Text;
            //===================================================================================
            //                          CASH RECEIPTS POSTING
            //===================================================================================

            if (clsSearchCash.checkIfPosted(txtORNo.Text) == true)
            {
                //If Voucher already cancelled
                Alert.show("Cash Receipt Already Posted!", Alert.AlertType.error);
                return;
            }

            if (clsSearchCash.checkIfCancelled(txtORNo.Text) == true)
            {
                //If Voucher already cancelled
                Alert.show("Cash Receipt Already Cancelled!", Alert.AlertType.error);
                return;
            }

            //ENABLE PARTICULARS FOR NOTE
            txtParticulars.Enabled = true;
            if (txtParticulars.Text == "")
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


                //Code for posting
                con = new SqlConnection();
                global.connection(con);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_CancellationOfCashReceipt";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Or_No", txtORNo.Text);
                cmd.Parameters.AddWithValue("@Cancel_Note", txtParticulars.Text);
                cmd.Parameters.AddWithValue("@Cancelled_By", Classes.clsUser.Username);
                cmd.ExecuteNonQuery();

                //Success Message
                Alert.show("Cash Receipt Successfully Cancelled!", Alert.AlertType.success);

                //Display Message
                status.Visible = true;
                status.Text = "CANCELLED";

                //Get Cancelled By 
                if (txtCancelledBy.Text == "")
                {
                    txtCancelledBy.Text = clsSearchCash.getCancelled(txtORNo.Text);
                }

                //Disable After Cancel
                txtParticulars.Enabled = false;
            }
            else
            {
                txtParticulars.Enabled = false;
                part = txtParticulars.Text;
                return;
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

            //=========================================================================
            //                      Check if Posted or Cancelled
            //=========================================================================

            if (clsSearchCash.checkIfPosted(txtORNo.Text) == true)
            {
                //If Voucher already cancelled
                Alert.show("Cash Receipt Already Posted!", Alert.AlertType.error);
                return;
            }

            if (clsSearchCash.checkIfCancelled(txtORNo.Text) == true)
            {
                //If Voucher already cancelled
                Alert.show("Cash Receipt Already Cancelled!", Alert.AlertType.error);
                return;
            }

            //=========================================================================
            //                      Code to Edit Cash Receipt
            //=========================================================================


            //=========================================================================
            //                      Validation Before Updating
            //=========================================================================

           
            if(btnEdit.Text == "EDIT")
            {
                forUpdating();
            }
            else
            {
                if (clsCash.CashValidationUpdate(txtORNo, txtPayorID, txtTransAmount, txtDebit, txtCredit, datagridviewTransaction, dataGridView3, dgvChecks, radioCash, radioPecciCheck, radioNonPecciCheck) == false)
                {
                    //UPDATE CODE;

                    // Continue to saving
                    //=================================================================================================
                    //                              SQL CONNECTION
                    //=================================================================================================
                    con = new SqlConnection();
                    global.connection(con);


                    //=================================================================================================
                    //                              FOR CASH TRANSACTION
                    //=================================================================================================

                    //=================================================================================================
                    //                              CASH RECEIPT HEADER
                    //=================================================================================================
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_UpdateCashReceiptsHeader";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Or_No", txtORNo.Text);
                    cmd.Parameters.AddWithValue("@Or_Date", dtOrDate.Text);

                    //FOr Payee Type 
                    //Member = 0 Client = 1
                    if (radioMember.Checked == true)
                    {
                        cmd.Parameters.AddWithValue("@Payor_Type", "0");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Payor_Type", "1");
                    }

                    //Check if Theres a Member or Client
                    if (Classes.clsCashReceipt.userID.ToString() == "")
                    {
                        cmd.Parameters.AddWithValue("@userID", DBNull.Value);

                    }
                    else if (Classes.clsCashReceipt.userID == 0)
                    {
                        cmd.Parameters.AddWithValue("@userID", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@userID", Classes.clsCashReceipt.userID.ToString());
                    }

                    cmd.Parameters.AddWithValue("@Payor", txtPayorID.Text);
                    cmd.Parameters.AddWithValue("@Particulars", txtParticulars.Text);

                    //============= FOr Collection Type
                    //0 = Cash
                    //1 = Pecci Check
                    //2 = Non-Pecci Check
                    if (radioCash.Checked == true)
                    {
                        //Cash
                        cmd.Parameters.AddWithValue("@Collection_Type", "0");
                        cmd.Parameters.AddWithValue("@Bank", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Check_No", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Check_Date", DBNull.Value);
                    }
                    else if (radioPecciCheck.Checked == true)
                    {
                        //Pecci Check
                        cmd.Parameters.AddWithValue("@Collection_Type", "1");
                        cmd.Parameters.AddWithValue("@Bank", dgvChecks.Rows[0].Cells[0].Value.ToString());
                        cmd.Parameters.AddWithValue("@Check_Date", dgvChecks.Rows[0].Cells[2].Value.ToString());
                        cmd.Parameters.AddWithValue("@Check_No", dgvChecks.Rows[0].Cells[3].Value.ToString());

                    }
                    else
                    {
                        //Non-Pecci
                        cmd.Parameters.AddWithValue("@Collection_Type", "2");
                        cmd.Parameters.AddWithValue("@Bank", dgvChecks.Rows[0].Cells[0].Value.ToString());
                        cmd.Parameters.AddWithValue("@Check_Date", dgvChecks.Rows[0].Cells[2].Value.ToString());
                        cmd.Parameters.AddWithValue("@Check_No", dgvChecks.Rows[0].Cells[3].Value.ToString());
                    }

                    cmd.Parameters.AddWithValue("@Prepared_By", txtPreparedBy.Text);

                    cmd.ExecuteNonQuery(); //SAVING CASH HEADER

                    //=================================================================================================
                    //                              Delete Details, Trans, Cheque
                    //=================================================================================================

                    SqlCommand cmdDeleteDetails = new SqlCommand();
                    cmdDeleteDetails.Connection = con;
                    cmdDeleteDetails.CommandText = "sp_DeleteCashReceiptDetail";
                    cmdDeleteDetails.CommandType = CommandType.StoredProcedure;
                    cmdDeleteDetails.Parameters.AddWithValue("@Or_No", txtORNo.Text);
                    cmdDeleteDetails.ExecuteNonQuery();

                    SqlCommand cmdDeleteTrans = new SqlCommand();
                    cmdDeleteTrans.Connection = con;
                    cmdDeleteTrans.CommandText = "sp_DeleteCashReceiptTrans";
                    cmdDeleteTrans.CommandType = CommandType.StoredProcedure;
                    cmdDeleteTrans.Parameters.AddWithValue("@Or_No", txtORNo.Text);
                    cmdDeleteTrans.ExecuteNonQuery();

                    SqlCommand cmdDeleteCheck = new SqlCommand();
                    cmdDeleteCheck.Connection = con;
                    cmdDeleteCheck.CommandText = "sp_DeleteCashReceiptChecks";
                    cmdDeleteCheck.CommandType = CommandType.StoredProcedure;
                    cmdDeleteCheck.Parameters.AddWithValue("@Or_No", txtORNo.Text);
                    cmdDeleteCheck.ExecuteNonQuery();



                    //=================================================================================================
                    //                              CASH RECEIPT DETAIL
                    //=================================================================================================
                    if (dataGridView3.Rows.Count > 0) //DETAIL GRID
                    {
                        foreach (DataGridViewRow row in dataGridView3.Rows)
                        {
                            if (row.Cells[0].Value != null) //Not New ROW
                            {
                                SqlCommand cmdDetail = new SqlCommand();
                                cmdDetail.Connection = con;
                                cmdDetail.CommandText = "sp_InsertCashReceiptsDetail";
                                cmdDetail.CommandType = CommandType.StoredProcedure;
                                cmdDetail.Parameters.AddWithValue("@Or_No", txtORNo.Text);
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
                                cmdDetail.ExecuteNonQuery(); //SAVING CASH DETAILS
                            }
                        }
                    }



                    //=================================================================================================
                    //                              CASH TRANSACTION
                    //=================================================================================================
                    if (datagridviewTransaction.Rows.Count > 0)
                    {
                        foreach (DataGridViewRow row in datagridviewTransaction.Rows)
                        {
                            if (row.Cells[0].Value != null)
                            {
                                SqlCommand cmdTrans = new SqlCommand();
                                cmdTrans.Connection = con;
                                cmdTrans.CommandText = "sp_InsertCashReceiptsTrans";
                                cmdTrans.CommandType = CommandType.StoredProcedure;
                                cmdTrans.Parameters.AddWithValue("@Or_No", txtORNo.Text);
                                cmdTrans.Parameters.AddWithValue("@Transaction_Type", row.Cells[0].Value);

                                if (row.Cells[1].Value != null)
                                {
                                    cmdTrans.Parameters.AddWithValue("@Loan_No", row.Cells[1].Value);
                                }
                                else
                                {
                                    cmdTrans.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                                }

                                cmdTrans.Parameters.AddWithValue("@Amount", Convert.ToDecimal(row.Cells[2].Value));
                                cmdTrans.ExecuteNonQuery(); //SAVING TRANSACTION
                            }
                        }
                    }

                    //=================================================================================================
                    //                         BANK FOR CHECK IF COLLECTION MODE IS NOT CASH
                    //=================================================================================================

                    if (radioPecciCheck.Checked == true || radioNonPecciCheck.Checked == true)
                    {
                        if (dgvChecks.Rows.Count > 0)
                        {
                            foreach (DataGridViewRow row in dgvChecks.Rows)
                            {
                                if (row.Cells[0].Value != null)
                                {
                                    SqlCommand cmdchek = new SqlCommand();
                                    cmdchek.Connection = con;
                                    cmdchek.CommandText = "sp_InsertCashReceiptsChecks";
                                    cmdchek.CommandType = CommandType.StoredProcedure;
                                    cmdchek.Parameters.AddWithValue("@Or_No", txtORNo.Text);
                                    cmdchek.Parameters.AddWithValue("@Bank", row.Cells[0].Value);
                                    cmdchek.Parameters.AddWithValue("@Amount", row.Cells[1].Value);
                                    cmdchek.Parameters.AddWithValue("@Check_Date", row.Cells[2].Value);
                                    cmdchek.Parameters.AddWithValue("@Check_No", row.Cells[3].Value);
                                    cmdchek.ExecuteNonQuery(); //SAVING TRANSACTION
                                }
                            }
                        }
                    }

                    Alert.show("Cash Receipts Successfully Updated!", Alert.AlertType.success);
                    //RESTORE COMMANDS AND TEXT
                    AfterSavingOrUpdating();

                }
            }

        }

        public void AfterSavingOrUpdating()
        {
            //=======================================
            //          Command buttons
            //=======================================
            btnEdit.Enabled = true;
            btnPost.Enabled = true;
            btnCancel.Enabled = true;
            btnSearch.Enabled = true;
            btnNew.Enabled = true;
            btnClose.Text = "CLOSE";
            btnEdit.Text = "EDIT";
            btnNew.Text = "NEW";

            //=======================================
            //          Disable Fields
            //=======================================
            txtORNo.Enabled = false;
            txtParticulars.Enabled = false;
            datagridviewTransaction.Enabled = false;
            dataGridView3.Enabled = false;
        }

        private void txtCheckAmount_Leave(object sender, EventArgs e)
        {
            if (txtCheckAmount.Text != "")
            {
                txtCheckAmount.Text = Convert.ToDecimal(txtCheckAmount.Text).ToString("#,0.00");
            }
        }

        private void txtLoanTypeSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtPayorID.Text != "")
            {
                con = new SqlConnection();
                global.connection(con);

                if (txtLoanTypeSearch.Text == "")
                {
                    //Return all Loan Types possible for this user
                    
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_ReturnLoanTypesPerUser";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", Classes.clsCashReceipt.userID);

                    //Put in datatable
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView2.DataSource = dt;
                    dataGridView2.Columns["Loan_Type"].FillWeight = 30;
                    dataGridView2.Columns["Loan_Type"].HeaderText = "Type";
                    dataGridView2.Columns["Loan_Description"].HeaderText = "Description";

                    //Hide other columns
                    dataGridView2.Columns["Loan_No"].Visible = false;
                    dataGridView2.Columns["userID"].Visible = false;
                    dataGridView2.Columns["CurrentDr"].Visible = false;
                    dataGridView2.Columns["PastDueDr"].Visible = false;
                    dataGridView2.Columns["Balance"].Visible = false;
                    dataGridView2.Columns["Deferred"].Visible = false;

                }
                else
                {
                    //Return the search loan
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_ReturnLoanTypesPerUserSEARCH";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", Classes.clsCashReceipt.userID);
                    cmd.Parameters.AddWithValue("@description", txtLoanTypeSearch.Text);

                    //Put in datatable
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView2.DataSource = dt;
                    dataGridView2.Columns["Loan_Type"].FillWeight = 30;
                    dataGridView2.Columns["Loan_Type"].HeaderText = "Type";
                    dataGridView2.Columns["Loan_Description"].HeaderText = "Description";

                    //Hide other columns
                    dataGridView2.Columns["Loan_No"].Visible = false;
                    dataGridView2.Columns["userID"].Visible = false;
                    dataGridView2.Columns["CurrentDr"].Visible = false;
                    dataGridView2.Columns["PastDueDr"].Visible = false;
                    dataGridView2.Columns["Balance"].Visible = false;
                    dataGridView2.Columns["Deferred"].Visible = false;
                }
            }

        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            datagridviewTransaction.Rows[selectedRow].Cells[selected].Value = dataGridView2.SelectedRows[0].Cells["Loan_Description"].Value.ToString();
            
            panel38.Visible = false;
        }

        public void forUpdating()
        {
            //=======================================
            //          Command buttons
            //=======================================
            btnClose.Text = "CANCEL";
            btnEdit.Text = "UPDATE";
            btnNew.Enabled = false;
            btnPost.Enabled = false;
            btnCancel.Enabled = false;
            btnSearch.Enabled = false;

            //=======================================
            //         PAYOR INFORMATION
            //=======================================
            txtParticulars.Enabled = true;

            //=======================================
            //       Transaction INFORMATION
            //=======================================
            datagridviewTransaction.Enabled = true;

            //=======================================
            //       Details INFORMATION
            //=======================================
            dataGridView3.Enabled = true;

        }

    }

}
