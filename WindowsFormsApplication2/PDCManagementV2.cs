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
    public partial class PDCManagementV2 : Form
    {
        public PDCManagementV2()
        {
            InitializeComponent();
            str = "";
        }
        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        Global global = new Global();
        clsMembershipEntry clsMembershipEntry = new clsMembershipEntry();
        clsMembership clsMembership = new clsMembership();
        Classes.clsPDCManagement clsPDCManagement = new Classes.clsPDCManagement();
        Classes.clsLoan clsLoan = new Classes.clsLoan();
        Classes.clsSearchCashReceipt clsSearchCash = new Classes.clsSearchCashReceipt();
        Classes.clsCashReceipt clsOR = new Classes.clsCashReceipt();
        Classes.clsCashReceipt clsCash = new Classes.clsCashReceipt();
        Classes.clsOpenTransaction clsOpen = new Classes.clsOpenTransaction();
        Classes.clsAccessControl clsAccess = new Classes.clsAccessControl();

        public string str;
        public static string prevChequeNo { get; set; }


        private void btnSearchPayee_Click(object sender, EventArgs e)
        {
            PDC.searchMember frm = new PDC.searchMember();
            frm.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbCategory_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbCategory.Text == "Loan")
            {
                btnSearchLoan.Enabled = true;
                txtLoanNumber.Text = "";
                txtLoanType.Text = "";
            }
            else
            {
                btnSearchLoan.Enabled = false;
                txtLoanNumber.Text = "";
                txtLoanType.Text = "";
            }
        }

        private void btnSearchLoan_Click(object sender, EventArgs e)
        {
            if (txtEmployeeID.Text == "")
            {
                Alert.show("Please select Member first.", Alert.AlertType.error);
                return;
            }

            PDCFolder.loanPerMember frm = new PDCFolder.loanPerMember();
            frm.ShowDialog();
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            if(cmbCategory.Text == "")
            {
                Alert.show("Please select Category.", Alert.AlertType.error);
                return;
            }

            if(cmbCategory.Text == "Loan")
            {
                if(txtLoanType.Text == "")
                {
                    Alert.show("Please select Loan.", Alert.AlertType.error);
                    return;
                }
            }

            if(txtAmount.Text == "")
            {
                Alert.show("Please enter Amount.", Alert.AlertType.error);
                return;
            }


            //=======================================================
            //          ADD CATEGORY FIRST
            //=======================================================

            string typ;

            if(cmbCategory.Text == "Loan")
            {
                typ = txtLoanType.Text;
            }
            else if(cmbCategory.Text == "Savings")
            {
                typ = "SD";
            }
            else
            {
                typ = "SC";
            }

            dgvDetails.Rows.Add(
                cmbCategory.Text,
                typ,
                txtLoanNumber.Text,
                txtAmount.Text
                );

            clsPDCManagement.loadTotals(dgvDetails,lblTotalAmount);
        }

        private void btnAddCheck_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForInsertRestriction("PDC Management", Classes.clsUser.Username) != true)
            {
                return;
            }

            //=======================================================
            //          VALIDATION FIRST BEFORE SAVING
            //=======================================================

            txtOrNumber.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            if (txtOrNumber.Text != "")
            {
                TextBox tt = new TextBox();
                tt.Text = txtOrNumber.Text;
                //Check first OR Number if already used by
                if (clsPDCManagement.orNumberDuplicate(tt) == true)
                {
                    Alert.show("OR Number already used. ", Alert.AlertType.error);
                    return;
                }
            }

            if(txtEmployeeID.Text == "")
            {
                Alert.show("Please select Member first.", Alert.AlertType.error);
                return;
            }

            if(cmbBank.Text == "")
            {
                Alert.show("Please select / enter bank.", Alert.AlertType.error);
                return;
            }

            if(txtChequeNo.Text == "")
            {
                Alert.show("Please enter Cheque No.", Alert.AlertType.error);
                return;
            }
            else
            {
                //Check cheque no first if already been used.
                if(clsPDCManagement.CheckChequeNoIfUsedByOthers(txtChequeNo.Text) == true)
                {
                    Alert.show("Cheque number already used.", Alert.AlertType.error);
                    return;
                }
            }

            if(dgvDetails.Rows.Count == 0)
            {
                Alert.show("Please enter at least 1 category.", Alert.AlertType.error);
                return; 
            }


            //================================================================
            //                      SAVE PDCMANAGEMENT V2
            //================================================================

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_InsertPDCManagementV2";
                cmd.CommandType = CommandType.StoredProcedure;
                txtOrNumber.TextMaskFormat = MaskFormat.IncludeLiterals;
                cmd.Parameters.AddWithValue("@ORNumber", txtOrNumber.Text);
                cmd.Parameters.AddWithValue("@userID", Classes.clsPDCManagement.userid);
                cmd.Parameters.AddWithValue("@EmployeeID", txtEmployeeID.Text);
                cmd.Parameters.AddWithValue("@EmpName", txtEmployeeName.Text);
                cmd.Parameters.AddWithValue("@Bank", cmbBank.Text);
                cmd.Parameters.AddWithValue("@ChequeDate", dtChequeDate.Text);
                cmd.Parameters.AddWithValue("@ChequeNo", txtChequeNo.Text);
                //cmd.Parameters.AddWithValue("@PreparedBy", Classes.clsUser.Username);
                cmd.Parameters.AddWithValue("@PreparedBy", "nikko");
                cmd.Parameters.AddWithValue("@TotalAmount", Convert.ToDecimal(lblTotalAmount.Text));
                cmd.Parameters.AddWithValue("@DatePrepared", dtDatePrepared.Text);

                //check if multiple transaction
                if(dgvDetails.Rows.Count > 1)
                {
                    cmd.Parameters.AddWithValue("@isMultiple", true);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@isMultiple", false);
                }

                cmd.ExecuteNonQuery();

                //===========================
                //      SAVE DETAILS
                //===========================

                for(int x = 0; x < dgvDetails.Rows.Count; x++)
                {
                    SqlCommand cmdDetails = new SqlCommand();
                    cmdDetails.Connection = con;
                    cmdDetails.CommandText = "sp_InsertPDCManagementV2_Detail";
                    cmdDetails.CommandType = CommandType.StoredProcedure;
                    cmdDetails.Parameters.AddWithValue("@ChequeNo", txtChequeNo.Text);
                    cmdDetails.Parameters.AddWithValue("@Category", dgvDetails.Rows[x].Cells[0].Value.ToString());
                    cmdDetails.Parameters.AddWithValue("@Loan_Type", dgvDetails.Rows[x].Cells[1].Value.ToString());
                    cmdDetails.Parameters.AddWithValue("@Loan_No", dgvDetails.Rows[x].Cells[2].Value.ToString());
                    cmdDetails.Parameters.AddWithValue("@Amount", Convert.ToDecimal(dgvDetails.Rows[x].Cells[3].Value.ToString()));
                    cmdDetails.ExecuteNonQuery();
                }

                Alert.show("Successfully added.", Alert.AlertType.success);

                //Refresh for real time
                //Clear
                clearFields();
                clsPDCManagement.loadPDC(dataGridView1);

            }


        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDetails.CurrentRow.IsNewRow)
                {
                    Alert.show("New row cannot be deleted.", Alert.AlertType.error);
                    return;
                }

                if (dgvDetails.SelectedCells.Count >= 1)
                {
                    string msg = Environment.NewLine + "Are you sure you want to delete this entry?";
                    DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        foreach (DataGridViewCell cell in dgvDetails.SelectedCells)
                        {
                            int row = dgvDetails.CurrentCell.RowIndex;
                            dgvDetails.Rows.RemoveAt(row);
                            clsPDCManagement.loadTotals(dgvDetails, lblTotalAmount);
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        public void clearFields()
        {
            txtOrNumber.Text = "";
            txtEmployeeID.Text = "";
            txtEmployeeName.Text = "";
            cmbBank.Text = "";
            txtChequeNo.Text = "";
            cmbCategory.SelectedIndex = -1;
            txtLoanNumber.Text = "";
            txtLoanType.Text = "";
            txtGross.Text = "";
            txtAmount.Text = "";
            dgvDetails.Rows.Clear();
            lblTotalAmount.Text = "0.00";

            btnSearchLoan.Enabled = false;
            btnEdit.Enabled = false;
            btnAddCheck.Enabled = true;
            btnSearchPayee.Enabled = true;
            dtDatePrepared.Enabled = true;
            prevChequeNo = "";
            btnEdit.Text = "EDIT";
            dtDatePrepared.Value = DateTime.Today;
            dtChequeDate.Value = DateTime.Today;
        }

        private void panel19_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PDCManagementV2_Load(object sender, EventArgs e)
        {
            clsPDCManagement.loadPDC(dataGridView1);
            clsMembershipEntry.loadComboBox(cmbBank, "Bank", "Bank_Name", "Bank_Name");
            clsMembershipEntry.loadComboBox(cmbSearchBank,"Bank","Bank_Name", "Bank_Name");
            clsLoan.loadComboBox(cmbLoanType);

            cmbLoanType.SelectedIndex = -1;
            cmbBank.SelectedIndex = -1;
            cmbSearchBank.SelectedIndex = -1;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                //No Data to be edit
                Alert.show("Please select transaction you want to edit.", Alert.AlertType.warning);
                return;
            }

            /*
            /    FOR VARIABLES NOT VISIBLE TO OUR EYES
            */
            Classes.clsPDCManagement.id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value.ToString());
            Classes.clsPDCManagement.userid = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userid"].Value.ToString());

            /*
            /    CONTROLS 
            */
            txtOrNumber.Text = dataGridView1.SelectedRows[0].Cells["ORNumber"].Value.ToString();

            txtEmployeeID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
            txtEmployeeName.Text = dataGridView1.SelectedRows[0].Cells["EmpName"].Value.ToString();

            cmbBank.Text = dataGridView1.SelectedRows[0].Cells["Bank"].Value.ToString();
            dtChequeDate.Text = dataGridView1.SelectedRows[0].Cells["ChequeDate"].Value.ToString();
            txtChequeNo.Text = dataGridView1.SelectedRows[0].Cells["ChequeNo"].Value.ToString();
            lblTotalAmount.Text = dataGridView1.SelectedRows[0].Cells["TotalAmount"].Value.ToString();
            dtDatePrepared.Text = dataGridView1.SelectedRows[0].Cells["DatePrepared"].Value.ToString();

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_PDCManagementV2_Details WHERE ChequeNo = '" + dataGridView1.SelectedRows[0].Cells["ChequeNo"].Value.ToString() + "'", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                dgvDetails.Rows.Clear();

                for(int x = 0; x < ds.Tables[0].Rows.Count; x++)
                {
                    dgvDetails.Rows.Add(
                        ds.Tables[0].Rows[x]["Category"].ToString(),
                        ds.Tables[0].Rows[x]["Loan_Type"].ToString(),
                        ds.Tables[0].Rows[x]["Loan_No"].ToString(),
                        ds.Tables[0].Rows[x]["Amount"].ToString()
                        );
                }
            }

            //==================================
            //      BUTTON ENABLE/DISABLE
            //==================================
            btnAddCheck.Enabled = false;
            btnEdit.Enabled = true;
            btnSearchPayee.Enabled = false;

            dtDatePrepared.Enabled = false;

            prevChequeNo = dataGridView1.SelectedRows[0].Cells["ChequeNo"].Value.ToString();

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForEditRestriction("PDC Management", Classes.clsUser.Username) != true)
            {
                return;
            }

            if (btnEdit.Text == "EDIT")
            {
                btnEdit.Text = "UPDATE";
            }
            else
            {

                if(clsPDCManagement.checkSameORDIfferentID(Convert.ToString(Classes.clsPDCManagement.id), txtOrNumber.Text) == true)
                {
                    return;
                }

                //Update PDC
                if (cmbBank.Text == "")
                {
                    Alert.show("Please select / enter bank.", Alert.AlertType.error);
                    return;
                }

                if (txtChequeNo.Text == "")
                {
                    Alert.show("Please enter Cheque No.", Alert.AlertType.error);
                    return;
                }
                else
                {
                    //Check cheque no first if already been used.
                    if (clsPDCManagement.checkChequeNoUsedByOthers(txtChequeNo.Text) == true)
                    {
                        return;
                    }
                }

                if (dgvDetails.Rows.Count == 0)
                {
                    Alert.show("Please enter at least 1 category.", Alert.AlertType.error);
                    return;
                }

                //==========================
                //  UPDATE CODE PDC
                //==========================

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();
                    
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_UpdatePDCManagementV2";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", Classes.clsPDCManagement.id);
                    txtOrNumber.TextMaskFormat = MaskFormat.IncludeLiterals;
                    cmd.Parameters.AddWithValue("@ORNumber", txtOrNumber.Text);
                    cmd.Parameters.AddWithValue("@Bank", cmbBank.Text);
                    cmd.Parameters.AddWithValue("@ChequeDate", dtChequeDate.Text);
                    cmd.Parameters.AddWithValue("@ChequeNo", txtChequeNo.Text);
                    cmd.Parameters.AddWithValue("@TotalAmount", Convert.ToDecimal(lblTotalAmount.Text));
                    if (dgvDetails.Rows.Count > 1)
                    {
                        cmd.Parameters.AddWithValue("@isMultiple", true);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@isMultiple", false);
                    }

                    cmd.ExecuteNonQuery();


                    //DELETE FIRST THE DETAILS AND RE INSERT

                    SqlCommand cmdDelete = new SqlCommand();
                    cmdDelete.Connection = con;
                    cmdDelete.CommandText = "DELETE PDCManagementV2_Details WHERE ChequeNo = '"+ prevChequeNo +"'";
                    cmdDelete.CommandType = CommandType.Text;
                    cmdDelete.ExecuteNonQuery();

                    //INSERT DETAILS AGAIN
                    for (int x = 0; x < dgvDetails.Rows.Count; x++)
                    {
                        SqlCommand cmdDetails = new SqlCommand();
                        cmdDetails.Connection = con;
                        cmdDetails.CommandText = "sp_InsertPDCManagementV2_Detail";
                        cmdDetails.CommandType = CommandType.StoredProcedure;
                        cmdDetails.Parameters.AddWithValue("@ChequeNo", txtChequeNo.Text);
                        cmdDetails.Parameters.AddWithValue("@Category", dgvDetails.Rows[x].Cells[0].Value.ToString());
                        cmdDetails.Parameters.AddWithValue("@Loan_Type", dgvDetails.Rows[x].Cells[1].Value.ToString());
                        cmdDetails.Parameters.AddWithValue("@Loan_No", dgvDetails.Rows[x].Cells[2].Value.ToString());
                        cmdDetails.Parameters.AddWithValue("@Amount", Convert.ToDecimal(dgvDetails.Rows[x].Cells[3].Value.ToString()));
                        cmdDetails.ExecuteNonQuery();
                    }

                    Alert.show("Successfully updated.", Alert.AlertType.success);

                    //Refresh for real time
                    //Clear
                    clearFields();
                    clsPDCManagement.loadPDC(dataGridView1);
                }


            }

        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                //if theres a data
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // This will check the cell.
                    row.Cells["isCheck"].Value = "true";
                }
            }
            else
            {
                Alert.show("No record(s) found.", Alert.AlertType.error);
                return;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                //if theres a data
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // This will check the cell.
                    row.Cells["isCheck"].Value = "false";
                }
            }
            else
            {
                Alert.show("No record(s) found.", Alert.AlertType.error);
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForDeleteRestriction("PDC Management", Classes.clsUser.Username) != true)
            {
                return;
            }

            int getIfCnt;
            getIfCnt = 0;

            if (dataGridView1.Rows.Count > 0)
            {
                //if theres a data

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["isCheck"].Value.ToString() == "True")
                    {
                        getIfCnt = getIfCnt + 1;
                    }
                }
            }

            if (getIfCnt > 0)
            {
                string msg = Environment.NewLine + "Are you sure you want to continue?";
                DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        DataGridViewCheckBoxCell cell = row.Cells[0] as DataGridViewCheckBoxCell;

                        //We don't want a null exception!
                        if (cell.Value != null)
                        {
                            if (cell.Value.ToString() == "True")
                            {
                                using (SqlConnection con = new SqlConnection(global.connectString()))
                                {
                                    con.Open();

                                    //DELETE DETAILS
                                    SqlCommand cmdDelete = new SqlCommand();
                                    cmdDelete.Connection = con;
                                    cmdDelete.CommandText = "DELETE PDCManagementV2_Details WHERE ChequeNo ='" + row.Cells["ChequeNo"].Value.ToString() + "'";
                                    cmdDelete.CommandType = CommandType.Text;
                                    cmdDelete.ExecuteNonQuery();

                                    //DELETE HEADER
                                    SqlCommand cmd = new SqlCommand();
                                    cmd.Connection = con;
                                    cmd.CommandText = "DELETE PDCManagementV2 WHERE ChequeNo ='" + row.Cells["ChequeNo"].Value.ToString() + "'";
                                    cmd.CommandType = CommandType.Text;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    //Alert Message for success deletion
                    Alert.show("Successfully deleted.", Alert.AlertType.success);
                    //Load Records
                    clsPDCManagement.loadPDC(dataGridView1);
                }
            }
            else
            {
                Alert.show("Please select  Record you want to delete.", Alert.AlertType.error);
                return;
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            /*
               Getting the min and max of date for parameter
           */

            var dateTimes = dataGridView1.Rows.Cast<DataGridViewRow>()
            //.Select(x => (DateTime) x.Cells["ColumnName"].Value); //if column type datetime
            //or    
            .Select(x => Convert.ToDateTime(x.Cells["ChequeDate"].Value));

            var minValue = dateTimes.Min();
            var maxValue = dateTimes.Max();

            //ATM REPORT
            CrystalDecisions.Shared.TableLogOnInfo li;

            //Print Purposes
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                string qry;
                if (str == "")
                {
                    qry = "SELECT TOP 50 * FROM vw_PDCManagementReportV2 ORDER BY ChequeDate,EmpName ASC";
                }
                else
                {
                    qry = str;
                }

                SqlDataAdapter adapter = new SqlDataAdapter(qry, con);

                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                PDCFolder.PDCManagementCR cr = new PDCFolder.PDCManagementCR();
                PDCFolder.PDCReport rpt = new PDCFolder.PDCReport();

                li = new TableLogOnInfo();

                li.ConnectionInfo.IntegratedSecurity = false;

                adapter.Fill(ds, "vw_PDCManagementReportV2");
                dt = ds.Tables["vw_PDCManagementReportV2"];
                cr.SetDataSource(ds.Tables["vw_PDCManagementReportV2"]);

                if (str != "" && dtChequeDateFrom.Checked == true && dtChequeDateTO.Checked == true)
                {
                    cr.SetParameterValue("pdcDUe", "PDC Due From " + dtChequeDateFrom.Text + " To " + dtChequeDateTO.Text);
                }
                else
                {
                    cr.SetParameterValue("pdcDUe", "PDC Due From " + minValue.ToShortDateString() + " To " + maxValue.ToShortDateString());
                }

                //cr.SetParameterValue("printedBy", Classes.clsUser.Username);
                cr.SetParameterValue("printedBy", "Nikko");

                //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);


                rpt.crystalReportViewer1.ReportSource = cr;
                rpt.ShowDialog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch(cmbSearchBy.Text)
            {
                case "OR Number":
                    str = "SELECT * FROM vw_PDCManagement WHERE ORNumber = '"+ txtKeyword.Text +"'";
                    break;
                case "EmployeeID":
                    if(cmbLoanType.Text != "")
                    {
                        //Search with loan type
                        str = "SELECT * FROM vw_PDCManagementV2_SearchLoanType WHERE EmployeeID = '" + txtKeyword.Text + "' and Loan_Type like '%" + cmbLoanType.SelectedValue + "%'";
                    }
                    else
                    {
                        str = "SELECT * FROM vw_PDCManagement WHERE EmployeeID = '" + txtKeyword.Text + "' ";
                    }

                    if (cmbSearchBank.Text != "")
                    {
                        str = str + "and Bank = '" + cmbSearchBank.SelectedValue.ToString() + "'";
                    }

                    if (dtChequeDateFrom.Checked == true && dtChequeDateTO.Checked == true)
                    {
                        str = str + " and ChequeDate Between '" + dtChequeDateFrom.Text + "' and '" + dtChequeDateTO.Text + "' ";
                    }
                    break;
                case "Name":
                    if (cmbLoanType.Text != "")
                    {
                        //Search with loan type
                        str = "SELECT * FROM vw_PDCManagementV2_SearchLoanType WHERE EmpName like '%" + txtKeyword.Text + "%' and Loan_Type like '%" + cmbLoanType.SelectedValue + "%'";
                    }
                    else
                    {
                        str = "SELECT * FROM vw_PDCManagement WHERE EmpName like '%" + txtKeyword.Text + "%' ";
                    }

                    if (cmbSearchBank.Text != "")
                    {
                        str = str + "and Bank = '" + cmbSearchBank.SelectedValue.ToString() + "'";
                    }

                    if (dtChequeDateFrom.Checked == true && dtChequeDateTO.Checked == true)
                    {
                        str = str + " and ChequeDate Between '" + dtChequeDateFrom.Text + "' and '" + dtChequeDateTO.Text + "' ";
                    }
                    break;
            }

            if(str != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(str, con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if(dt.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dt;

                        dataGridView1.Columns["DatePrepared"].Visible = false;
                        dataGridView1.Columns["userid"].Visible = false;
                        dataGridView1.Columns["id"].Visible = false;
                        try
                        {
                            dataGridView1.Columns["Loan_No"].Visible = false;
                        }
                        catch
                        {

                        }

                        dataGridView1.Columns["isCheck"].HeaderText = "";
                        dataGridView1.Columns["ORNumber"].HeaderText = "OR #";
                        dataGridView1.Columns["EmployeeID"].HeaderText = "ID No.";
                        dataGridView1.Columns["EmpName"].HeaderText = "Name";
                        dataGridView1.Columns["ChequeDate"].HeaderText = "Date of Cheque";
                        
                        dataGridView1.Columns["Bank"].HeaderText = "Bank";
                        dataGridView1.Columns["ChequeNo"].HeaderText = "Cheque No";
                        dataGridView1.Columns["TotalAmount"].HeaderText = "Amount";

                        dataGridView1.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dataGridView1.Columns["TotalAmount"].DefaultCellStyle.Format = "#,0.00";
                        dataGridView1.Columns["isMultiple"].HeaderText = "Multiple transaction";
                        dataGridView1.Columns["isCheck"].FillWeight = 20;
                        dataGridView1.Columns["ORNumber"].FillWeight = 60;
                        dataGridView1.Columns["EmployeeID"].FillWeight = 50;
                        dataGridView1.Columns["EmpName"].FillWeight = 220;
                        dataGridView1.Columns["Bank"].FillWeight = 50;

                        dataGridView1.Columns["isCheck"].ReadOnly = false;
                        dataGridView1.Columns["isMultiple"].ReadOnly = true;
                        dataGridView1.Columns["ORNumber"].ReadOnly = true;
                        dataGridView1.Columns["EmployeeID"].ReadOnly = true;
                        dataGridView1.Columns["EmpName"].ReadOnly = true;
                        dataGridView1.Columns["ChequeDate"].ReadOnly = true;
                        dataGridView1.Columns["Bank"].ReadOnly = true;
                        dataGridView1.Columns["ChequeNo"].ReadOnly = true;
                    }
                    else
                    {
                        Alert.show("No record(s) found.", Alert.AlertType.error);
                        str = "";
                        return;
                    }
                }
            }
        }

        private void cmbSearchBy_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbSearchBy.Text == "OR Number")
            {
                cmbLoanType.Enabled = false;
                cmbSearchBank.Enabled = false;
                dtChequeDateFrom.Enabled = false;
                dtChequeDateTO.Enabled = false;
            }
            else
            {
                cmbLoanType.Enabled = true;
                cmbSearchBank.Enabled = true;
                dtChequeDateFrom.Enabled = true;
                dtChequeDateTO.Enabled = true;
            }
        }

        private void cmbSearchBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSearchBy.Text == "OR Number")
            {
                cmbLoanType.Enabled = false;
                cmbSearchBank.Enabled = false;
                dtChequeDateFrom.Enabled = false;
                dtChequeDateTO.Enabled = false;
            }
            else
            {
                cmbLoanType.Enabled = true;
                cmbSearchBank.Enabled = true;
                dtChequeDateFrom.Enabled = true;
                dtChequeDateTO.Enabled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            cmbLoanType.Enabled = false;
            cmbSearchBank.Enabled = false;
            dtChequeDateFrom.Enabled = false;
            dtChequeDateTO.Enabled = false;
            txtKeyword.Text = "";

            cmbSearchBy.SelectedIndex = -1;
            cmbLoanType.SelectedIndex = -1;
            cmbSearchBank.SelectedIndex = -1;

            clsPDCManagement.loadPDC(dataGridView1);
            dtChequeDateFrom.Value = DateTime.Today;
            dtChequeDateTO.Value = DateTime.Today;
            dtChequeDateFrom.Checked = false;
            dtChequeDateTO.Checked = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForViewingRestriction("PDC Management", Classes.clsUser.Username) != true)
            {
                return;
            }

            if (dataGridView1.Rows.Count > 0)
            {
                if(dataGridView1.SelectedRows[0].Cells["ORNumber"].Value.ToString() != "")
                {
                    CashReceiptVoucher or = new CashReceiptVoucher();

                    foreach (Form form in Application.OpenForms)
                    {

                        if (form.GetType() == typeof(CashReceiptVoucher))
                        {
                            form.Activate();
                            or = (CashReceiptVoucher)Application.OpenForms["CashReceiptVoucher"];
                        }
                    }
                    //REMOVE GRID FIRST
                    or.datagridviewTransaction.Rows.Clear();
                    or.dataGridView3.Rows.Clear();
                    or.dgvChecks.Rows.Clear();
                    
                    //REMOVE THE OPEN OR FIRST
                    if (or.txtORNo.Text != "")
                    {
                        clsOpen.deleteTransaction("Receipt Voucher", or.txtORNo.Text);
                    }

                    or.txtORNo.Text = dataGridView1.SelectedRows[0].Cells["ORNumber"].Value.ToString();
                    //========================================================
                    //          PAYOR INFORMATION
                    //========================================================
                    Classes.clsCashReceipt.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                    or.txtPayorID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                    or.txtPayorName.Text = dataGridView1.SelectedRows[0].Cells["EmpName"].Value.ToString();
                    or.txtPayorCompany.Text = clsCash.returnCompanyDescription(clsSearchCash.GetCompanyPerMember(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
                    or.radioMember.Checked = true;

                    //========================================================
                    //          PAYOR TRANSACTION 
                    //========================================================
                    or.radioPecciCheck.Checked = true;

                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();

                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM PDCManagementv2_details WHERE ChequeNo = '"+ dataGridView1.SelectedRows[0].Cells["ChequeNo"].Value.ToString() +"'", con);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);

                        //================================================
                        //          COCI FOR DETAILS
                        //================================================
                        or.dataGridView3.Rows.Add(
                            "105", "", "", Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["TotalAmount"].Value.ToString()).ToString("#,0.00"), "0.00", "0"
                            );

                        for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                        {
                            switch(ds.Tables[0].Rows[x]["Category"].ToString())
                            {
                                case "Savings":
                                    or.datagridviewTransaction.Rows.Add(
                                        "001", "SAVINGS DEPOSIT", Convert.ToDecimal(ds.Tables[0].Rows[x]["Amount"].ToString()).ToString("#,0.00")
                                        );

                                    //================================================
                                    //          SAVINGS ENTRY
                                    //================================================
                                    or.dataGridView3.Rows.Add(
                                        "300.1", "", "", "0.00", Convert.ToDecimal(ds.Tables[0].Rows[x]["Amount"].ToString()).ToString("#,0.00"), dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()
                                        );
                                    break;
                                case "Share Capital":
                                    or.datagridviewTransaction.Rows.Add(
                                        "003", "FIXED DEPOSIT", Convert.ToDecimal(ds.Tables[0].Rows[x]["Amount"].ToString()).ToString("#,0.00")
                                        );

                                    //================================================
                                    //          SHARE CAPITAL ENTRY
                                    //================================================
                                    or.dataGridView3.Rows.Add(
                                        "363", "", "", "0.00", Convert.ToDecimal(ds.Tables[0].Rows[x]["Amount"].ToString()).ToString("#,0.00"), dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()
                                        );
                                    break;
                                case "Loan":
                                    or.datagridviewTransaction.Rows.Add(
                                        "002",clsPDCManagement.returnLoanDescription(ds.Tables[0].Rows[x]["Loan_Type"].ToString()), Convert.ToDecimal(ds.Tables[0].Rows[x]["Amount"].ToString()).ToString("#,0.00")
                                        );
                                    //FOR LOAN SETUP
                                    clsPDCManagement.getMonthlyDetails(or.dataGridView3, ds.Tables[0].Rows[x]["Loan_No"].ToString(), Convert.ToDouble(ds.Tables[0].Rows[x]["Amount"].ToString()),or.dgvtempBilling);
                                    break;
                            }
                        }
                        or.compute();
                        or.computeDetails();
                    }

                    //========================================================
                    //              CHEQUE INFORMATION
                    //========================================================
                    or.dgvChecks.Rows.Add(
                        dataGridView1.SelectedRows[0].Cells["Bank"].Value.ToString(),
                        dataGridView1.SelectedRows[0].Cells["TotalAmount"].Value.ToString(),
                        Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["ChequeDate"].Value.ToString()).ToShortDateString(),
                        dataGridView1.SelectedRows[0].Cells["ChequeNo"].Value.ToString()
                        );


                    or.dataGridView3.Sort(or.dataGridView3.Columns["Debit"], ListSortDirection.Descending);

                    //Enable and Disable Buttons
                    or.btnEdit.Enabled = false;
                    or.btnPost.Enabled = false;
                    or.btnCancel.Enabled = false;
                    or.btnAuditted.Enabled = false;
                    or.btnSearch.Enabled = false;
                    or.btnNew.Enabled = true;

                    or.btnNew.Text = "SAVE";
                    or.btnEdit.Text = "EDIT";
                    or.btnClose.Text = "CANCEL";

                    //FROM PDCMANAGEMENT 
                    or.frmPDCManagement = true;
                    or.frmPDCManagementChequeNumber = dataGridView1.SelectedRows[0].Cells["ChequeNo"].Value.ToString();
                    or.Show();
                }
                else
                {
                    Alert.show("Please update or number.", Alert.AlertType.error);
                    return;
                }
            }
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
