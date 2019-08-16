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
    public partial class SavingsWithdrawalATM : Form
    {
        public SavingsWithdrawalATM()
        {
            InitializeComponent();
        }

        //Global & Others
        Global global = new Global();
        Classes.clsATMWithdrawal clsATM = new Classes.clsATMWithdrawal();
        Classes.clsAccessControl clsAccess = new Classes.clsAccessControl();

        //SQl
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;

        decimal totalForAmout;
        string jvNo; //For JV No of Banks

        //Mousemove
        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        private void SavingsWithdrawalATM_Load(object sender, EventArgs e)
        {
            clsATM.loadATMWithdrawal(dataGridView2);
            clsATM.loadBank(cmbBank);

            cmbBank.SelectedIndex = -1;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //Moveable Forms / Screens
            //Nikko Melgar
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


                if (form.GetType() == typeof(SavingsWithdrawalATM))
                {
                    form.Activate();
                    return;
                }
            }

            SavingsWithdrawalATM frm = new SavingsWithdrawalATM();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtEmployeeID.Text != "" || txtFirstName.Text != "" || txtLastName.Text != "" || txtWithdrawalSlip.Text != "" || cmbBank.Text != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    string str;
                    if (cmbBank.Text != "")
                    {
                        str = "SELECT * FROM vw_SavingsWithdrawalATM where EmployeeID like '%" + txtEmployeeID.Text + "%' and Slip_No like '%" + txtWithdrawalSlip.Text + "%' and lastName like '%" + txtLastName.Text + "%' and FirstName like '%" + txtFirstName.Text + "%' and Bank_Code like  '%" + cmbBank.SelectedValue + "%'";
                    }
                    else
                    {
                        str = "SELECT * FROM vw_SavingsWithdrawalATM where EmployeeID like '%" + txtEmployeeID.Text + "%' and Slip_No like '%" + txtWithdrawalSlip.Text + "%' and lastName like '%" + txtLastName.Text + "%' and FirstName like '%" + txtFirstName.Text + "%'";
                    }

                    adapter = new SqlDataAdapter(str, con);
                    dt = new DataTable();
                    adapter.Fill(dt);



                    if (dt.Rows.Count == 0)
                    {
                        Alert.show("No record(s) found.", Alert.AlertType.warning);
                        clsATM.loadATMWithdrawal(dataGridView2);
                    }
                    else
                    {
                        dataGridView2.DataSource = dt;
                    }
                }
            }
            else
            {
                Alert.show("Please enter valid Keyword.", Alert.AlertType.warning);
                return;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtWithdrawalSlip.Text = "";
            cmbBank.SelectedIndex = -1;
            txtCancelNote.Text = "";

            clsATM.loadATMWithdrawal(dataGridView2);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForDeleteRestriction("ATM Preparation", Classes.clsUser.Username) != true)
            {
                return;
            }

            if (dataGridView2.Rows.Count == 0)
            {
                Alert.show("No record(s) found.", Alert.AlertType.error);
                return;
            }

            //Check if cancel note is null
            if(txtCancelNote.Text == "")
            {
                Alert.show("Cancel note is required.", Alert.AlertType.error);
                return;
            }

            string msg = Environment.NewLine + "Are you sure you want to remove this transaction?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                var selectedRows = dataGridView2.SelectedRows
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
                    cmd.Parameters.AddWithValue("@Withdrawal_Slip_No", dataGridView2.SelectedRows[0].Cells["Slip_No"].Value.ToString());
                    cmd.Parameters.AddWithValue("@CancelledlNote", txtCancelNote.Text);
                    cmd.Parameters.AddWithValue("@Cancelled_By", Classes.clsUser.Username);

                    cmd.ExecuteNonQuery();
                }
                foreach (var row in selectedRows)
                    dataGridView2.Rows.Remove(row);

                dataGridView2.ClearSelection();

                txtCancelNote.Text = "";
            }
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForInsertRestriction("ATM Preparation", Classes.clsUser.Username) != true)
            {
                return;
            }

            if (dataGridView2.Rows.Count >= 1)
            {
                string msg = Environment.NewLine + "Are you sure you want to proceed?";
                DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //Get Record First In vw_Savings
                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();

                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_SavingsWithdrawalATM ORDER BY Bank_Code", con);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            //Check first if theres a record
                            var first = dt.Rows[0].ItemArray[8].ToString(); //Bank Code
                                                                            //jvNo = Jv No For Diff. Banks
                                                                            //totalForAmout = total Amount Per Banks

                            //=================================================================================
                            //Create JV For Replenished Withdrawal
                            //=================================================================================

                            //=================================================================================
                            //JOURNAL HEADER = First Bank Header
                            //=================================================================================
                            SqlCommand cmdHeader = new SqlCommand();
                            cmdHeader.Connection = con;
                            cmdHeader.CommandText = "sp_InsertJournalHeader";
                            cmdHeader.CommandType = CommandType.StoredProcedure;
                            cmdHeader.Parameters.AddWithValue("@JV_Date", DateTime.Today.ToShortDateString());
                            cmdHeader.Parameters.AddWithValue("@userID", DBNull.Value);
                            cmdHeader.Parameters.AddWithValue("@AdjTo", DBNull.Value);
                            cmdHeader.Parameters.AddWithValue("@Particular", "SAVINGS WITHDRAWAL RELEASE [ " + first + " ]");
                            cmdHeader.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                            cmdHeader.Parameters.AddWithValue("@Posted", false);
                            cmdHeader.Parameters.AddWithValue("@Transaction_Type", "TRAN001");
                            cmdHeader.Parameters.AddWithValue("@summarize", true);
                            cmdHeader.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);
                            cmdHeader.Parameters.AddWithValue("@Poseted_By", DBNull.Value);
                            cmdHeader.ExecuteNonQuery();


                            for (int x = 0; x < dt.Rows.Count; x++)
                            {
                                if (dt.Rows[x].ItemArray[8].ToString() == first)
                                {
                                    //If same bank put to 1 jv no.

                                    //=================================================================================
                                    //GET JV 
                                    //=================================================================================

                                    SqlCommand cmdJV = new SqlCommand();
                                    cmdJV.Connection = con;
                                    cmdJV.CommandText = "sp_GetJVNoAfterSaving";
                                    cmdJV.CommandType = CommandType.StoredProcedure;
                                    cmdJV.Parameters.AddWithValue("@jv_date", DateTime.Today.ToShortDateString());
                                    cmdJV.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);

                                    SqlDataAdapter adapterjv = new SqlDataAdapter(cmdJV);
                                    DataTable dtjv = new DataTable();
                                    adapterjv.Fill(dtjv);

                                    if (dtjv.Rows.Count > 0)
                                    {
                                        jvNo = dtjv.Rows[0].ItemArray[0].ToString();
                                    }
                                    else
                                    {
                                        return;
                                    }

                                    //Adding all the Debit 300.1 code
                                    SqlCommand cmd = new SqlCommand();
                                    cmd.Connection = con;
                                    cmd.CommandText = "sp_InsertJournalDetail";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@JV_No", jvNo);
                                    cmd.Parameters.AddWithValue("@Account_Code", "300.1");
                                    cmd.Parameters.AddWithValue("@userID", Convert.ToInt32(dt.Rows[x].ItemArray[1].ToString()));
                                    cmd.Parameters.AddWithValue("@Subsidiary_Code", dt.Rows[x].ItemArray[2].ToString());
                                    cmd.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                                    cmd.Parameters.AddWithValue("@Debit", Convert.ToDecimal(dt.Rows[x].ItemArray[7].ToString()));
                                    cmd.Parameters.AddWithValue("@Credit", Convert.ToDecimal("0.00"));
                                    totalForAmout = totalForAmout + Convert.ToDecimal(dt.Rows[x].ItemArray[7].ToString());
                                    cmd.ExecuteNonQuery();

                                    //====================================================================================
                                    //          UPDATE TABLE = WITHDRAWAL SLIP [JV_NO | DATE]
                                    //====================================================================================

                                    SqlCommand cmd2 = new SqlCommand();
                                    cmd2.Connection = con;
                                    cmd2.CommandText = "sp_UpdateReplenishAndJVNoWithdrawalSlipATM";
                                    cmd2.CommandType = CommandType.StoredProcedure;
                                    cmd2.Parameters.AddWithValue("@Withdrawal_Slip_No", dt.Rows[x].ItemArray[0].ToString());
                                    cmd2.Parameters.AddWithValue("@Posted", false);
                                    cmd2.Parameters.AddWithValue("@JV_No", jvNo);
                                    cmd2.Parameters.AddWithValue("@Posted_By", DBNull.Value);
                                    cmd2.ExecuteNonQuery();

                                }//End of the same bank
                                else
                                {
                                    //This will Start to create the footer of the prev. JV No
                                    SqlCommand cmd1 = new SqlCommand();
                                    cmd1.Connection = con;
                                    cmd1.CommandText = "sp_InsertJournalDetail";
                                    cmd1.CommandType = CommandType.StoredProcedure;
                                    cmd1.Parameters.AddWithValue("@JV_No", jvNo);

                                    //CHECK WHAT BANK USED
                                    if (first == "MBTC")
                                    {
                                        cmd1.Parameters.AddWithValue("@Account_Code", "102.09");
                                    }
                                    else if (first == "BPI")
                                    {
                                        cmd1.Parameters.AddWithValue("@Account_Code", "102.10");
                                    }
                                    else
                                    {
                                        cmd1.Parameters.AddWithValue("@Account_Code", "102.17");
                                    }

                                    cmd1.Parameters.AddWithValue("@userID", DBNull.Value);
                                    cmd1.Parameters.AddWithValue("@Subsidiary_Code", DBNull.Value);
                                    cmd1.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                                    cmd1.Parameters.AddWithValue("@Debit", Convert.ToDecimal("0.00"));
                                    cmd1.Parameters.AddWithValue("@Credit", Convert.ToDecimal(totalForAmout));
                                    cmd1.ExecuteNonQuery();

                                    //========================================================================================
                                    //          Create First Header For the Next Bank 
                                    //========================================================================================

                                    //1. First Set the First Var to the next Bank
                                    first = dt.Rows[x].ItemArray[8].ToString();
                                    totalForAmout = 0;

                                    SqlCommand cmdHeaderNxtBank = new SqlCommand();
                                    cmdHeaderNxtBank.Connection = con;
                                    cmdHeaderNxtBank.CommandText = "sp_InsertJournalHeader";
                                    cmdHeaderNxtBank.CommandType = CommandType.StoredProcedure;
                                    cmdHeaderNxtBank.Parameters.AddWithValue("@JV_Date", DateTime.Today.ToShortDateString());
                                    cmdHeaderNxtBank.Parameters.AddWithValue("@userID", DBNull.Value);
                                    cmdHeaderNxtBank.Parameters.AddWithValue("@AdjTo", DBNull.Value);
                                    cmdHeaderNxtBank.Parameters.AddWithValue("@Particular", "SAVINGS WITHDRAWAL RELEASE [ " + first + " ]");
                                    cmdHeaderNxtBank.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                                    cmdHeaderNxtBank.Parameters.AddWithValue("@Posted", false);
                                    cmdHeaderNxtBank.Parameters.AddWithValue("@Transaction_Type", "TRAN001");
                                    cmdHeaderNxtBank.Parameters.AddWithValue("@summarize", true);
                                    cmdHeaderNxtBank.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);
                                    cmdHeaderNxtBank.Parameters.AddWithValue("@Poseted_By", DBNull.Value);
                                    cmdHeaderNxtBank.ExecuteNonQuery();


                                    //=================================================================================
                                    //GET JV 
                                    //=================================================================================

                                    SqlCommand cmdJV = new SqlCommand();
                                    cmdJV.Connection = con;
                                    cmdJV.CommandText = "sp_GetJVNoAfterSaving";
                                    cmdJV.CommandType = CommandType.StoredProcedure;
                                    cmdJV.Parameters.AddWithValue("@jv_date", DateTime.Today.ToShortDateString());
                                    cmdJV.Parameters.AddWithValue("@Prepared_By", Classes.clsUser.Username);

                                    SqlDataAdapter adapterjv = new SqlDataAdapter(cmdJV);
                                    DataTable dtjv = new DataTable();
                                    adapterjv.Fill(dtjv);

                                    if (dtjv.Rows.Count > 0)
                                    {
                                        jvNo = dtjv.Rows[0].ItemArray[0].ToString();
                                    }
                                    else
                                    {
                                        return;
                                    }

                                    //Adding all the Debit 300.1 code
                                    SqlCommand cmd = new SqlCommand();
                                    cmd.Connection = con;
                                    cmd.CommandText = "sp_InsertJournalDetail";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@JV_No", jvNo);
                                    cmd.Parameters.AddWithValue("@Account_Code", "300.1");
                                    cmd.Parameters.AddWithValue("@userID", Convert.ToInt32(dt.Rows[x].ItemArray[1].ToString()));
                                    cmd.Parameters.AddWithValue("@Subsidiary_Code", dt.Rows[x].ItemArray[2].ToString());
                                    cmd.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                                    cmd.Parameters.AddWithValue("@Debit", Convert.ToDecimal(dt.Rows[x].ItemArray[7].ToString()));
                                    cmd.Parameters.AddWithValue("@Credit", Convert.ToDecimal("0.00"));
                                    totalForAmout = totalForAmout + Convert.ToDecimal(dt.Rows[x].ItemArray[7].ToString());
                                    cmd.ExecuteNonQuery();

                                    //====================================================================================
                                    //          UPDATE TABLE = WITHDRAWAL SLIP [JV_NO | DATE]
                                    //====================================================================================

                                    SqlCommand cmd2 = new SqlCommand();
                                    cmd2.Connection = con;
                                    cmd2.CommandText = "sp_UpdateReplenishAndJVNoWithdrawalSlipATM";
                                    cmd2.CommandType = CommandType.StoredProcedure;
                                    cmd2.Parameters.AddWithValue("@Withdrawal_Slip_No", dt.Rows[x].ItemArray[0].ToString());
                                    cmd2.Parameters.AddWithValue("@Posted", false);
                                    cmd2.Parameters.AddWithValue("@JV_No", jvNo);
                                    cmd2.Parameters.AddWithValue("@Posted_By", DBNull.Value);
                                    cmd2.ExecuteNonQuery();
                                }

                            } //End of Loop

                            //==========================================================================================================
                            // This will create footer for the last record 
                            //==========================================================================================================

                            //This will Start to create the footer of the prev. JV No
                            SqlCommand cmdFooterLastRecord = new SqlCommand();
                            cmdFooterLastRecord.Connection = con;
                            cmdFooterLastRecord.CommandText = "sp_InsertJournalDetail";
                            cmdFooterLastRecord.CommandType = CommandType.StoredProcedure;
                            cmdFooterLastRecord.Parameters.AddWithValue("@JV_No", jvNo);

                            //CHECK WHAT BANK USED
                            if (first == "MBTC")
                            {
                                cmdFooterLastRecord.Parameters.AddWithValue("@Account_Code", "102.09");
                            }
                            else if (first == "BPI")
                            {
                                cmdFooterLastRecord.Parameters.AddWithValue("@Account_Code", "102.10");
                            }
                            else
                            {
                                cmdFooterLastRecord.Parameters.AddWithValue("@Account_Code", "102.17");
                            }

                            cmdFooterLastRecord.Parameters.AddWithValue("@userID", DBNull.Value);
                            cmdFooterLastRecord.Parameters.AddWithValue("@Subsidiary_Code", DBNull.Value);
                            cmdFooterLastRecord.Parameters.AddWithValue("@Loan_No", DBNull.Value);
                            cmdFooterLastRecord.Parameters.AddWithValue("@Debit", Convert.ToDecimal("0.00"));
                            cmdFooterLastRecord.Parameters.AddWithValue("@Credit", Convert.ToDecimal(totalForAmout));
                            cmdFooterLastRecord.ExecuteNonQuery();



                            //==========================================================================================================
                            // This will move to ATM table
                            //==========================================================================================================
                            //=============================================================================
                            //                      SAVE TO ATM TABLE
                            //=============================================================================
                            foreach (DataGridViewRow row in dataGridView2.Rows)
                            {
                                SqlCommand cmdATM = new SqlCommand();
                                cmdATM.Connection = con;
                                cmdATM.CommandText = "sp_InsertSavingsWithdrawalToATMTable";
                                cmdATM.CommandType = CommandType.StoredProcedure;
                                cmdATM.Parameters.AddWithValue("@userID", row.Cells["userID"].Value);
                                cmdATM.Parameters.AddWithValue("@EmployeeID", row.Cells["EmployeeID"].Value);
                                cmdATM.Parameters.AddWithValue("@Account_No", row.Cells["Atm_Account_No"].Value);
                                cmdATM.Parameters.AddWithValue("@Bank_Code", row.Cells["Bank_Code"].Value);
                                cmdATM.Parameters.AddWithValue("@Amount", row.Cells["AmtWithdrawn"].Value);
                                if (row.Cells["PrincipalID"].Value.ToString() != "")
                                {
                                    cmdATM.Parameters.AddWithValue("@Principal", false);
                                }
                                else
                                {
                                    cmdATM.Parameters.AddWithValue("@Principal", true);
                                }

                                cmdATM.Parameters.AddWithValue("@Purpose", "SD");
                                cmdATM.Parameters.AddWithValue("@Name", row.Cells["LastName"].Value + ", " + row.Cells["FirstName"].Value + " " + row.Cells["MiddleName"].Value + " " + row.Cells["Suffix"].Value);
                                cmdATM.Parameters.AddWithValue("@Frm", "WD");
                                cmdATM.Parameters.AddWithValue("@wd_loan_slip", row.Cells["Slip_No"].Value);


                                adapter = new SqlDataAdapter("SELECT JV_No FROM Withdrawal_Slip WHERE Withdrawal_Slip_No = '" + row.Cells["Slip_No"].Value + "'", con);
                                DataTable dt1 = new DataTable();
                                adapter.Fill(dt1);

                                cmdATM.Parameters.AddWithValue("@jv_no", dt1.Rows[0].ItemArray[0].ToString());
                                cmdATM.ExecuteNonQuery();
                            }

                            //CLEAR
                            clsATM.loadATMWithdrawal(dataGridView2);

                            //SUCCESS MESSAGE
                            Alert.show("ATM Withdrawal voucher successfully created.", Alert.AlertType.success);
                        }


                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                Alert.show("No record(s) for posting.", Alert.AlertType.error);
                return;
            }
        }
    }
}
