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
    public partial class CashReplenishment : Form
    {
        public CashReplenishment()
        {
            InitializeComponent();
        }

        SqlConnection con;
        SqlDataAdapter adapter;
        decimal sum;

        Global global = new Global();
        Classes.clsCashReplenishment clsCashReplenishment = new Classes.clsCashReplenishment();
        Classes.clsJournalVoucher clsjv = new Classes.clsJournalVoucher();
        Classes.clsAccessControl clsAccess = new Classes.clsAccessControl();


        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

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

        private void button6_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForViewingRestriction(lblTitle.Text, Classes.clsUser.Username) != true)
            {
                return;
            }

            if (txtRangingFrom.Text == "" || txtRangingTo.Text == "")
            {
                Alert.show("All fields with (*) are required.", Alert.AlertType.warning);
                return;
            }

            if(Convert.ToInt32(txtRangingFrom.Text) > Convert.ToInt32(txtRangingTo.Text))
            {
                Alert.show("Slip No 'From' must not be greater than Slip No 'To'.", Alert.AlertType.warning);
                return;
            }

            if(Convert.ToDateTime(dtFrom.Text) > Convert.ToDateTime(dtTo.Text))
            {
                Alert.show("Date ' From' must not be greater than Date 'To'.", Alert.AlertType.warning);
                return;
            }
            clsCashReplenishment.loadReplenishment(dataGridView1, Convert.ToInt32(txtRangingFrom.Text), Convert.ToInt32(txtRangingTo.Text), dtFrom.Text, dtTo.Text,txtTotalNoSlips,txtAmount);
                      
        }


        private void btnReplenish_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForInsertRestriction(lblTitle.Text, Classes.clsUser.Username) != true)
            {
                return;
            }

            if (dataGridView1.Rows.Count == 0)
            {
                Alert.show("No record(s) found.", Alert.AlertType.warning);
                return;
            }

            string msg = Environment.NewLine + "Are you sure you want to proceed?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //=================================================================================
                //COnnection
                //=================================================================================
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

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
                    cmdHeader.Parameters.AddWithValue("@userID", DBNull.Value);
                    cmdHeader.Parameters.AddWithValue("@AdjTo", DBNull.Value);
                    cmdHeader.Parameters.AddWithValue("@Particular", "REPLENISHMENT OF SAVINGS WITHDRAWAL");
                    cmdHeader.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                    cmdHeader.Parameters.AddWithValue("@Posted", true);
                    cmdHeader.Parameters.AddWithValue("@Transaction_Type", "TRAN001");
                    cmdHeader.Parameters.AddWithValue("@summarize", true);
                    cmdHeader.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);
                    cmdHeader.Parameters.AddWithValue("@Poseted_By", Classes.clsUser.Username);
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
                    //JOURNAL DETAILS
                    //=================================================================================
                    if (dataGridView1.Rows.Count > 0)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            //Adding all the Debit 300.1 code
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = con;
                            cmd.CommandText = "sp_InsertJournalDetail";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@JV_No", jvNo);
                            cmd.Parameters.AddWithValue("@Account_Code", "300.1");
                            cmd.Parameters.AddWithValue("@userID", Convert.ToInt32(row.Cells["userID"].Value));
                            cmd.Parameters.AddWithValue("@Subsidiary_Code", row.Cells["EmployeeID"].Value);
                            cmd.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Debit", Convert.ToDecimal(row.Cells["AmtWithdrawn"].Value));
                            cmd.Parameters.AddWithValue("@Credit", Convert.ToDecimal("0.00"));
                            cmd.ExecuteNonQuery();
                        }

                        SqlCommand cmd1 = new SqlCommand();
                        cmd1.Connection = con;
                        cmd1.CommandText = "sp_InsertJournalDetail";
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@JV_No", jvNo);
                        cmd1.Parameters.AddWithValue("@Account_Code", "104");
                        cmd1.Parameters.AddWithValue("@userID", DBNull.Value);
                        cmd1.Parameters.AddWithValue("@Subsidiary_Code", DBNull.Value);
                        cmd1.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                        cmd1.Parameters.AddWithValue("@Debit", Convert.ToDecimal("0.00"));
                        cmd1.Parameters.AddWithValue("@Credit", Convert.ToDecimal(txtAmount.Text));
                        cmd1.ExecuteNonQuery();

                    }

                    //=================================================================================
                    //Replenish Withdrawal
                    //=================================================================================

                    if (dataGridView1.Rows.Count > 0)
                    {
                        //Get all details of withdrawal slip in temp table
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            //For Saving of Replenishment and JVNO
                            SqlCommand cmd2 = new SqlCommand();
                            cmd2.Connection = con;
                            cmd2.CommandText = "sp_UpdateReplenishAndJVNoWithdrawalSlip";
                            cmd2.CommandType = CommandType.StoredProcedure;
                            cmd2.Parameters.AddWithValue("@Withdrawal_Slip_No", row.Cells["Withdrawal_Slip_No"].Value);
                            cmd2.Parameters.AddWithValue("@JV_No", jvNo);
                            cmd2.Parameters.AddWithValue("@Posted_By", Classes.clsUser.Username);
                            cmd2.ExecuteNonQuery();
                        }
                    }

                    //=================================================================================
                    //Call Disbursement
                    //=================================================================================

                    DisbursementVoucher cv = new DisbursementVoucher();
                    cv.ForReplenishment();

                    foreach (Form form in Application.OpenForms)
                    {

                        if (form.GetType() == typeof(DisbursementVoucher))
                        {
                            form.Activate();
                            DisbursementVoucher cv1 = new DisbursementVoucher();
                            cv1 = (DisbursementVoucher)Application.OpenForms["DisbursementVoucher"];
                            cv1.dataGridView1.Rows[0].Cells[3].Value = txtAmount.Text; //==Debit
                            cv1.dataGridView1.Rows[1].Cells[4].Value = txtAmount.Text; //==Credit
                            cv1.cmbTransaction.SelectedValue = "TRAN001";
                            cv1.cmbBank.SelectedValue = "PCIB";
                            cv1.txtAmount.Text = txtAmount.Text;
                            cv1.txtParticular.Text = "REPLENISHMENT OF SAVINGS WITHDRAWAL - IN REFERENCE TO JV#" + jvNo;
                            cv1.radioClient.Checked = true;
                            cv1.txtPayee.Text = "CASHIER";
                            cv1.txtPayeeName.Text = clsCashReplenishment.getCashier();
                            return;
                        }
                    }

                    cv.dataGridView1.Rows.Add(1);

                    //for (int i = 0; i < 1; i++)
                    //{
                    //    cv.dataGridView1.Rows[i].Cells[3].Value = txtAmount.Text; //==Debit
                    //    cv.dataGridView1.Rows[1].Cells[4].Value = txtAmount.Text; //==Credit

                    //}

                    cv.getBankFromWithdrawal = "";
                    cv.getTransaction = "TRAN001";
                    cv.cmbBank.SelectedValue = "PCIB";
                    cv.txtAmount.Text = txtAmount.Text;
                    cv.txtParticular.Text = "REPLENISHMENT OF SAVINGS WITHDRAWAL - IN REFERENCE TO JV#" + jvNo;
                    cv.radioClient.Checked = true;
                    cv.txtPayee.Text = "CASHIER";
                    cv.txtPayeeName.Text = clsCashReplenishment.getCashier();

                    cv.dataGridView1.Rows[0].Cells[0].Value = "104";
                    cv.dataGridView1.Rows[0].Cells[3].Value = txtAmount.Text;

                    int rowId = cv.dataGridView1.Rows.Add();
                    // Grab the new row!
                    DataGridViewRow row2 = cv.dataGridView1.Rows[rowId];

                    // Add the data
                    row2.Cells[0].Value = "102.17";
                    row2.Cells[4].Value = txtAmount.Text;

                    //put jv to disbursement
                    cv.jvno = jvNo;

                    //=================================================================================
                    //Load Data Replenish 
                    //=================================================================================
                    clsCashReplenishment.loadReplenishment(dataGridView1, Convert.ToInt32(txtRangingFrom.Text), Convert.ToInt32(txtRangingTo.Text), dtFrom.Text, dtTo.Text, txtTotalNoSlips, txtAmount);
                    //=================================================================================
                    //MessageBox Before Showing Disbursement
                    //=================================================================================
                    this.Close();

                    cv.Show();
                }
                Alert.show("Cash withdrawal successfully replenished.", Alert.AlertType.success);

            } //User Click No For Return
            else
            {
                return;
            }           

        }

        private void txtRangingFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForDeleteRestriction(lblTitle.Text, Classes.clsUser.Username) != true)
            {
                return;
            }

            if (dataGridView1.Rows.Count == 0)
            {
                Alert.show("No record(s) found", Alert.AlertType.error);
                return;
            }
            
            string msg = Environment.NewLine + "Are you sure you want to remove this item(s)?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                var selectedRows = dataGridView1.SelectedRows
            .OfType<DataGridViewRow>()
            .Where(row => !row.IsNewRow)
            .ToArray();

                //Cancel Status if Remove
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_CancellationOfWithdrawal";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Withdrawal_Slip_No", dataGridView1.SelectedRows[0].Cells["Withdrawal_Slip_No"].Value.ToString());
                    cmd.Parameters.AddWithValue("@CancelledlNote", "Cancel From Replenishment");
                    cmd.Parameters.AddWithValue("@Cancelled_By", Classes.clsUser.Username);

                    cmd.ExecuteNonQuery();

                    foreach (var row in selectedRows)
                        dataGridView1.Rows.Remove(row);

                    dataGridView1.ClearSelection();

                    txtTotalNoSlips.Text = dataGridView1.Rows.Count.ToString();

                    //Compute Amount

                    if (dataGridView1.Rows.Count >= 1)
                    {
                        for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                        {
                            sum += Convert.ToDecimal(dataGridView1.Rows[i].Cells["AmtWithdrawn"].Value);
                        }
                    }

                    txtAmount.Text = sum.ToString();
                }                    
            }            
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(CashReplenishment))
                {
                    form.Activate();
                    return;
                }
            }

            CashReplenishment frm = new CashReplenishment();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }
        
    }
}
