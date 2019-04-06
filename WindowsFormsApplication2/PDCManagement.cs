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
    public partial class PDCManagement : Form
    {
        public PDCManagement()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        Global global = new Global();
        clsMembershipEntry clsMembershipEntry = new clsMembershipEntry();
        clsMembership clsMembership = new clsMembership();
        Classes.clsPDCManagement clsPDCManagement = new Classes.clsPDCManagement();
        Classes.clsLoan clsLoan = new Classes.clsLoan();

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearchPayee_Click(object sender, EventArgs e)
        {
            PDC.searchMember frm = new PDC.searchMember();
            frm.ShowDialog();
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

        private void button3_Click(object sender, EventArgs e)
        {
            if(txtEmployeeID.Text == "")
            {
                Alert.show("Please select member first.", Alert.AlertType.error);
                return;
            }

            PDCFolder.loanPerMember frm = new PDCFolder.loanPerMember();
            frm.ShowDialog();

        }

        private void PDCManagement_Load(object sender, EventArgs e)
        {
            clsMembershipEntry.loadComboBox(cmbBank, "Bank", "Bank_Name", "Bank_Code");

            //Search Panel
            clsMembershipEntry.loadComboBox(cmbSearchBank, "Bank", "Bank_Name", "Bank_Code");
            clsLoan.loadComboBox(cmbLoanType);

            cmbLoanType.SelectedIndex = -1;
            cmbSearchBank.SelectedIndex = -1;
            cmbBank.SelectedIndex = -1;

            clsPDCManagement.loadPDC(dataGridView1);
        }

        private void btnRemoveCheck_Click(object sender, EventArgs e)
        {
            if(btnClear.Text == "CLEAR")
            {
                txtEmployeeID.Text = "";
                txtName.Text = "";
                txtLoanNumber.Text = "";
                txtLoanType.Text = "";
                dtDatePrepared.Value = DateTime.Today;
                cmbBank.SelectedIndex = -1;
                dtChequeDate.Value = DateTime.Today;
                txtChequeNo.Text = "";
                txtAmount.Text = "";
            }
            else
            {
                txtEmployeeID.Text = "";
                txtName.Text = "";
                txtLoanNumber.Text = "";
                txtLoanType.Text = "";
                dtDatePrepared.Value = DateTime.Today;
                cmbBank.SelectedIndex = -1;
                dtChequeDate.Value = DateTime.Today;
                txtChequeNo.Text = "";
                txtAmount.Text = "";

                btnAddCheck.Enabled = true;
                btnEdit.Enabled = false;
                btnEdit.Text = "EDIT";
                btnClear.Text = "CLEAR";
                btnSearchPayee.Enabled = true;
                button3.Enabled = true;
                dtDatePrepared.Enabled = true;
                dtChequeDate.Enabled = true;
            }
            
        }

        private void btnAddCheck_Click(object sender, EventArgs e)
        {
            if(txtEmployeeID.Text == "")
            {
                Alert.show("Please select member first.", Alert.AlertType.error);
                return;
            }

            if(txtLoanType.Text == "")
            {
                Alert.show("Please select loan first.", Alert.AlertType.error);
                return;
            }

            if(cmbBank.Text == "")
            {
                Alert.show("Please select bank first.", Alert.AlertType.error);
                return;
            }

            if(txtChequeNo.Text == "")
            {
                Alert.show("Cheque number is required.", Alert.AlertType.error);
                return;
            }

            if(txtAmount.Text == "")
            {
                Alert.show("Amount is required.", Alert.AlertType.error);
                return;
            }


            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_InsertPDC";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", Classes.clsPDCManagement.userid);
                cmd.Parameters.AddWithValue("@EmployeeID", txtEmployeeID.Text);
                cmd.Parameters.AddWithValue("@EmpName", txtName.Text);
                cmd.Parameters.AddWithValue("@LoanType", txtLoanType.Text);
                cmd.Parameters.AddWithValue("@LoanNumber", txtLoanNumber.Text);
                cmd.Parameters.AddWithValue("@Bank", cmbBank.SelectedValue);
                cmd.Parameters.AddWithValue("@ChequeDate", dtChequeDate.Text);
                cmd.Parameters.AddWithValue("@ChequeNo", txtChequeNo.Text);
                cmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Replace(",",""));
                cmd.Parameters.AddWithValue("@DatePrepared", dtDatePrepared.Text);
                cmd.Parameters.AddWithValue("@PreparedBy", Classes.clsUser.Username);
                cmd.ExecuteNonQuery();

                Alert.show("Successfully added.", Alert.AlertType.success);

                //Remove Cheque Number and Amount
                txtChequeNo.Text = "";
                txtAmount.Text = "";

                //Refresh Datagridview
                clsPDCManagement.loadPDC(dataGridView1);
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

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            clsPDCManagement.loadPDC(dataGridView1);
            cmbSearchBy.SelectedIndex = -1;
            cmbLoanType.SelectedIndex = -1;
            dtChequeDateTO.Value = DateTime.Today;
            dtChequeDateFrom.Value = DateTime.Today;
            dtChequeDateFrom.Checked = false;
            dtChequeDateTO.Checked = false;
            txtKeyword.Text = "";
            cmbSearchBank.SelectedIndex = -1;
            
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                //No Data to be edit
                Alert.show("Please select you want to edit.", Alert.AlertType.warning);
                return;
            }

            if(dataGridView1.SelectedRows[0].Cells["ORNumber"].Value.ToString() != "")
            {
                return;
            }

            Classes.clsPDCManagement.userid = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userid"].Value.ToString());
            txtEmployeeID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
            txtName.Text = dataGridView1.SelectedRows[0].Cells["EmpName"].Value.ToString();
            txtLoanType.Text = dataGridView1.SelectedRows[0].Cells["LoanType"].Value.ToString();
            txtLoanNumber.Text = dataGridView1.SelectedRows[0].Cells["LoanNumber"].Value.ToString();
            dtDatePrepared.Text = dataGridView1.SelectedRows[0].Cells["DatePrepared"].Value.ToString();
            cmbBank.Text = clsMembership.returnBankName(dataGridView1.SelectedRows[0].Cells["Bank"].Value.ToString());
            dtChequeDate.Text = dataGridView1.SelectedRows[0].Cells["ChequeDate"].Value.ToString();
            txtChequeNo.Text = dataGridView1.SelectedRows[0].Cells["ChequeNo"].Value.ToString();
            txtAmount.Text = dataGridView1.SelectedRows[0].Cells["Amount"].Value.ToString();

            //Put The last Value
            Classes.clsPDCManagement.LastValueChequeNo = dataGridView1.SelectedRows[0].Cells["ChequeNo"].Value.ToString();
            Classes.clsPDCManagement.LastValueAmount = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Amount"].Value.ToString());

            //Disable Buttons
            btnAddCheck.Enabled = false;
            btnEdit.Enabled = true;
            btnEdit.Text = "UPDATE";
            btnClear.Text = "CANCEL";
            btnSearchPayee.Enabled = false;
            button3.Enabled = false;
            dtDatePrepared.Enabled = false;
            dtChequeDate.Enabled = false;

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if(btnEdit.Text == "UPDATE")
            {
                if (cmbBank.Text == "")
                {
                    Alert.show("Please select bank first.", Alert.AlertType.error);
                    return;
                }

                if (txtChequeNo.Text == "")
                {
                    Alert.show("Cheque number is required.", Alert.AlertType.error);
                    return;
                }

                if (txtAmount.Text == "")
                {
                    Alert.show("Amount is required.", Alert.AlertType.error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_UpdatePDC";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", Classes.clsPDCManagement.userid);
                    cmd.Parameters.AddWithValue("@DatePrepared", dtDatePrepared.Text);
                    cmd.Parameters.AddWithValue("@loanType", txtLoanType.Text);
                    cmd.Parameters.AddWithValue("@LoanNumber", txtLoanNumber.Text);
                    cmd.Parameters.AddWithValue("@Bank", cmbBank.SelectedValue.ToString());
                    cmd.Parameters.AddWithValue("@ChequeDate", dtChequeDate.Text);
                    cmd.Parameters.AddWithValue("@ChequeNo", txtChequeNo.Text);
                    cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                    cmd.Parameters.AddWithValue("@LastValueChequeNo", Classes.clsPDCManagement.LastValueChequeNo);
                    cmd.Parameters.AddWithValue("@LastValueAmount", Classes.clsPDCManagement.LastValueAmount);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", Classes.clsUser.Username);
                    cmd.ExecuteNonQuery();

                    Alert.show("Successfully updated.", Alert.AlertType.success);

                    //RESET TOOLS
                    txtEmployeeID.Text = "";
                    txtName.Text = "";
                    txtLoanNumber.Text = "";
                    txtLoanType.Text = "";
                    dtDatePrepared.Value = DateTime.Today;
                    cmbBank.SelectedIndex = -1;
                    dtChequeDate.Value = DateTime.Today;
                    txtChequeNo.Text = "";
                    txtAmount.Text = "";

                    btnAddCheck.Enabled = true;
                    btnEdit.Enabled = false;
                    btnEdit.Text = "EDIT";
                    btnClear.Text = "CLEAR";
                    btnSearchPayee.Enabled = true;
                    button3.Enabled = true;
                    dtDatePrepared.Enabled = true;
                    dtChequeDate.Enabled = true;

                    //Refresh Datagridview
                    clsPDCManagement.loadPDC(dataGridView1);
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str;
            if(cmbSearchBy.Text == "OR Number")
            {
                str = "SELECT * FROM vw_PDCManagement WHERE ORNumber = '" + txtKeyword.Text +"'";
            }
            else if(cmbSearchBy.Text == "EmployeeID")
            {
                str = "SELECT * FROM vw_PDCManagement WHERE EmployeeID = '" + txtKeyword.Text +"'";
            }
            else if(cmbSearchBy.Text == "Name")
            {
                if(dtChequeDateFrom.Checked == true && dtChequeDateTO.Checked == true)
                {
                    str = "SELECT * FROM vw_PDCManagement WHERE EmpName like '%" + txtKeyword.Text + "%' and LoanType like '%" + cmbLoanType.SelectedValue + "%' and Bank like '%" + cmbSearchBank.SelectedValue + "%' and ChequeDate Between '"+ dtChequeDateFrom.Text +"' and '"+ dtChequeDateTO.Text +"'";
                }
                else
                {
                    str = "SELECT * FROM vw_PDCManagement WHERE EmpName like '%" + txtKeyword.Text + "%' and LoanType like '%" + cmbLoanType.SelectedValue + "%' and Bank like '%" + cmbSearchBank.SelectedValue + "%'";
                }
               
            }
            else
            {
                //NO SELECTED CRITERIA
                if (dtChequeDateFrom.Checked == true && dtChequeDateTO.Checked == true)
                {
                    str = "SELECT * FROM vw_PDCManagement WHERE ChequeDate Between '" + dtChequeDateFrom.Text + "' and '" + dtChequeDateTO.Text + "' ";


                    if (cmbLoanType.Text != "")
                    {
                        if (cmbSearchBank.Text != "")
                        {
                            str = str + "and LoanType = '" + cmbLoanType.SelectedValue.ToString() + "' and Bank = '" + cmbSearchBank.SelectedValue.ToString() + "'";
                        }
                        else
                        {
                            str = str + "and LoanType = '" + cmbLoanType.SelectedValue.ToString() + "'";
                        }
                    }
                    else
                    {
                        if (cmbSearchBank.Text != "")
                        {
                            str = str + "and Bank = '" + cmbSearchBank.SelectedValue.ToString() + "'";
                        }
                    }

                }
                else
                {
                    str = "SELECT * FROM vw_PDCManagement ";

                    if (cmbLoanType.Text != "")
                    {
                        if(cmbSearchBank.Text != "")
                        {
                            str = str + "WHERE LoanType = '" + cmbLoanType.SelectedValue.ToString() + "' and Bank = '"+ cmbSearchBank.SelectedValue.ToString() +"'";
                        }
                        else
                        {
                            str = str + "WHERE LoanType = '" + cmbLoanType.SelectedValue.ToString() + "'";
                        }
                    }
                    else
                    {
                        if (cmbSearchBank.Text != "")
                        {
                            str = str + "WHERE Bank = '" + cmbSearchBank.SelectedValue.ToString() + "'";
                        }
                        else
                        {
                            str = "SELECT TOP 50 * FROM vw_PDCManagement ORDER BY DatePrepared DESC";
                        }
                    }
                }
            }

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(str, con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;

                //Datagridview Custom EDIT
                dataGridView1.Columns["DatePrepared"].Visible = false;
                dataGridView1.Columns["userid"].Visible = false;
                dataGridView1.Columns["LoanNumber"].Visible = false;

                dataGridView1.Columns["isCheck"].HeaderText = "";
                dataGridView1.Columns["ORNumber"].HeaderText = "OR #";
                dataGridView1.Columns["EmployeeID"].HeaderText = "ID No.";
                dataGridView1.Columns["EmpName"].HeaderText = "Name";
                dataGridView1.Columns["ChequeDate"].HeaderText = "Date of Cheque";
                dataGridView1.Columns["LoanType"].HeaderText = "Type";
                dataGridView1.Columns["Bank"].HeaderText = "Bank";
                dataGridView1.Columns["ChequeNo"].HeaderText = "Cheque No";
                dataGridView1.Columns["Amount"].HeaderText = "Amount";

                dataGridView1.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns["Amount"].DefaultCellStyle.Format = "#,0.00";

                dataGridView1.Columns["isCheck"].FillWeight = 20;
                dataGridView1.Columns["ORNumber"].FillWeight = 60;
                dataGridView1.Columns["EmployeeID"].FillWeight = 50;
                dataGridView1.Columns["EmpName"].FillWeight = 260;
                dataGridView1.Columns["LoanType"].FillWeight = 50;
                dataGridView1.Columns["Bank"].FillWeight = 50;

                dataGridView1.Columns["isCheck"].ReadOnly = false;

                dataGridView1.Columns["ORNumber"].ReadOnly = true;
                dataGridView1.Columns["EmployeeID"].ReadOnly = true;
                dataGridView1.Columns["EmpName"].ReadOnly = true;
                dataGridView1.Columns["ChequeDate"].ReadOnly = true;
                dataGridView1.Columns["LoanType"].ReadOnly = true;
                dataGridView1.Columns["Bank"].ReadOnly = true;
                dataGridView1.Columns["ChequeNo"].ReadOnly = true;
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
                Alert.show("No record found.", Alert.AlertType.error);
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
                Alert.show("No record found.", Alert.AlertType.error);
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            int getIfCnt;
            getIfCnt = 0;

            if (dataGridView1.Rows.Count > 0)
            {
                //if theres a data
                
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if(row.Cells["isCheck"].Value.ToString() == "True")
                    {
                        getIfCnt = getIfCnt + 1;
                    }
                }
            }

            if(getIfCnt > 0)
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
                                if(row.Cells["ORNumber"].Value.ToString() == "")
                                {
                                    //It's checked!
                                    //Code for saving all the check members approve
                                    using (SqlConnection con = new SqlConnection(global.connectString()))
                                    {
                                        con.Open();

                                        SqlCommand cmd = new SqlCommand();
                                        cmd.Connection = con;
                                        cmd.CommandText = "sp_DeletePDC";
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@userid", row.Cells["userid"].Value.ToString());
                                        cmd.Parameters.AddWithValue("@DatePrepared", row.Cells["DatePrepared"].Value.ToString());
                                        cmd.Parameters.AddWithValue("@loanType", row.Cells["loanType"].Value.ToString());
                                        cmd.Parameters.AddWithValue("@LoanNumber", row.Cells["LoanNumber"].Value.ToString());
                                        cmd.Parameters.AddWithValue("@ChequeDate", row.Cells["ChequeDate"].Value.ToString());
                                        cmd.ExecuteNonQuery();
                                    }

                                }
                                else
                                {
                                    //Uncheck the Record with OR Number already
                                    //For History record
                                    row.Cells["isCheck"].Value = "false";
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
                Alert.show("Please check record you want to delete.", Alert.AlertType.error);
                return;
            }
        }
    }
}
