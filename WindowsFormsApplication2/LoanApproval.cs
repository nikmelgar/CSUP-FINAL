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
    public partial class LoanApproval : Form
    {
        public LoanApproval()
        {
            InitializeComponent();
        }
        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        Global global = new Global();
        Classes.clsLoanApproval clsApproval = new Classes.clsLoanApproval();
        Classes.clsParameter clsParameter = new Classes.clsParameter();

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
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "2")
            {
                //APPROVED
                Alert.show("This Loan already Approved!", Alert.AlertType.error);
                return;
            }

            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "3")
            {
                //DISAPPROVED
                Alert.show("This Loan already Dispproved!", Alert.AlertType.error);
                return;
            }

            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "5")
            {
                //DISAPPROVED
                Alert.show("This Loan already Released!", Alert.AlertType.error);
                return;
            }

            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "7")
            {
                //DISAPPROVED
                Alert.show("This Loan already Cancelled!", Alert.AlertType.error);
                return;
            }



            string msg = Environment.NewLine + "Are you sure you want to Approve this loan?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {

                con = new SqlConnection();
                global.connection(con);


                double srvc;
                srvc = Convert.ToDouble(txtLoanAmount.Text) * Convert.ToDouble(clsParameter.serviceFee());

                //For Loans that not required a service fee
                SqlDataAdapter adapterSRV = new SqlDataAdapter("SELECT VAL FROM Parameter WHERE val = '"+ cmbLoanType.Text +"'", con);
                DataTable dtSRV = new DataTable();
                adapterSRV.Fill(dtSRV);

                if(dtSRV.Rows.Count > 0)
                {
                    srvc = 0.00;
                }


                //APPROVED
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ApprovedLoan";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                cmd.Parameters.AddWithValue("@Service_Fee", Convert.ToDecimal(decimal.Round(Convert.ToDecimal(srvc),2)));
                cmd.Parameters.AddWithValue("@Share_Capital", Convert.ToDecimal(txtShareCapital.Text));
                cmd.Parameters.AddWithValue("@Savings", Convert.ToDecimal(txtSavings.Text));
                cmd.Parameters.AddWithValue("@Approved_By", Classes.clsUser.Username);
                cmd.Parameters.AddWithValue("@Note", txtReason.Text);
                cmd.ExecuteNonQuery();


                //Move the Deduct amounts to loan and other deduction
                if(dataGridView1.Rows.Count >= 1)
                {
                    //SAVE ALL THAT HAS A VALUE
                    //DATAGRID LOAN BALANCES

                    //Declaration First
                    con = new SqlConnection();
                    global.connection(con);
                    int x = 0;

                    
                    while (x != dataGridView1.Rows.Count)
                    {
                        if(dataGridView1.Rows[x].Cells[4].Value == null)
                        {
                            //If No Applied Amount
                            x = x + 1;
                        }
                        else
                        {
                            //If has a deferred value
                            if (Convert.ToDecimal(dataGridView1.Rows[x].Cells[3].Value.ToString()) > 0) 
                            {
                                //Check if equal or greater than or less than 
                                if(Convert.ToDecimal(dataGridView1.Rows[x].Cells[4].Value.ToString()) == Convert.ToDecimal(dataGridView1.Rows[x].Cells[3].Value.ToString()))
                                {
                                    //CHECK IF APPLIED AMOUNT = DEFERRED AMOUNT
                                    SqlCommand cmd = new SqlCommand();
                                    cmd.Connection = con;
                                    cmd.CommandText = "sp_InsertLoanDeductions";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@userID", Classes.clsLoanDataEntry.userID);
                                    cmd.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                                    cmd.Parameters.AddWithValue("@Loan_Type", dataGridView1.Rows[x].Cells[7].Value.ToString());
                                    cmd.Parameters.AddWithValue("@Loan_Type_Loan_No", dataGridView1.Rows[x].Cells[0].Value.ToString());
                                    cmd.Parameters.AddWithValue("@Other_Deduction", dataGridView1.Rows[x].Cells[6].Value.ToString()); //Bill Deferred First
                                    cmd.Parameters.AddWithValue("@Applied_Amount", dataGridView1.Rows[x].Cells[3].Value.ToString().Replace(",", ""));
                                    cmd.Parameters.AddWithValue("@Deduction_Type", "LOAN");
                                    cmd.ExecuteNonQuery();
                                }

                                else if(Convert.ToDecimal(dataGridView1.Rows[x].Cells[4].Value.ToString()) > Convert.ToDecimal(dataGridView1.Rows[x].Cells[3].Value.ToString()))
                                {
                                    //IF APPLIED AMOUNT IS GREATER THAN THE DEFERRED AMOUNT
                                    SqlCommand cmd = new SqlCommand();
                                    cmd.Connection = con;
                                    cmd.CommandText = "sp_InsertLoanDeductions";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@userID", Classes.clsLoanDataEntry.userID);
                                    cmd.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                                    cmd.Parameters.AddWithValue("@Loan_Type", dataGridView1.Rows[x].Cells[7].Value.ToString());
                                    cmd.Parameters.AddWithValue("@Loan_Type_Loan_No", dataGridView1.Rows[x].Cells[0].Value.ToString());
                                    cmd.Parameters.AddWithValue("@Other_Deduction", dataGridView1.Rows[x].Cells[6].Value.ToString()); //Bill Deferred First
                                    cmd.Parameters.AddWithValue("@Applied_Amount", dataGridView1.Rows[x].Cells[3].Value.ToString().Replace(",", ""));
                                    cmd.Parameters.AddWithValue("@Deduction_Type", "LOAN");
                                    cmd.ExecuteNonQuery();

                                    SqlCommand cmd2 = new SqlCommand();
                                    cmd2.Connection = con;
                                    cmd2.CommandText = "sp_InsertLoanDeductions";
                                    cmd2.CommandType = CommandType.StoredProcedure;
                                    cmd2.Parameters.AddWithValue("@userID", Classes.clsLoanDataEntry.userID);
                                    cmd2.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                                    cmd2.Parameters.AddWithValue("@Loan_Type", dataGridView1.Rows[x].Cells[7].Value.ToString());
                                    cmd2.Parameters.AddWithValue("@Loan_Type_Loan_No", dataGridView1.Rows[x].Cells[0].Value.ToString());
                                    cmd2.Parameters.AddWithValue("@Other_Deduction", dataGridView1.Rows[x].Cells[5].Value.ToString()); //Bill Current First
                                    cmd2.Parameters.AddWithValue("@Applied_Amount", Convert.ToDecimal(Convert.ToDecimal(dataGridView1.Rows[x].Cells[4].Value.ToString()) - Convert.ToDecimal(dataGridView1.Rows[0].Cells[3].Value.ToString())).ToString().Replace(",", "."));
                                    cmd2.Parameters.AddWithValue("@Deduction_Type", "LOAN");
                                    cmd2.ExecuteNonQuery();
                                }
                                else if(Convert.ToDecimal(dataGridView1.Rows[x].Cells[4].Value.ToString()) < Convert.ToDecimal(dataGridView1.Rows[x].Cells[3].Value.ToString()))
                                {
                                    SqlCommand cmd = new SqlCommand();
                                    cmd.Connection = con;
                                    cmd.CommandText = "sp_InsertLoanDeductions";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@userID", Classes.clsLoanDataEntry.userID);
                                    cmd.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                                    cmd.Parameters.AddWithValue("@Loan_Type", dataGridView1.Rows[x].Cells[7].Value.ToString());
                                    cmd.Parameters.AddWithValue("@Loan_Type_Loan_No", dataGridView1.Rows[x].Cells[0].Value.ToString());
                                    cmd.Parameters.AddWithValue("@Other_Deduction", dataGridView1.Rows[x].Cells[6].Value.ToString()); //Bill Deferred First
                                    cmd.Parameters.AddWithValue("@Applied_Amount", dataGridView1.Rows[x].Cells[4].Value.ToString().Replace(",", ""));
                                    cmd.Parameters.AddWithValue("@Deduction_Type", "LOAN");
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            //IF NO DEFERRED
                            else
                            {
                                SqlCommand cmd2 = new SqlCommand();
                                cmd2.Connection = con;
                                cmd2.CommandText = "sp_InsertLoanDeductions";
                                cmd2.CommandType = CommandType.StoredProcedure;
                                cmd2.Parameters.AddWithValue("@userID", Classes.clsLoanDataEntry.userID);
                                cmd2.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                                cmd2.Parameters.AddWithValue("@Loan_Type", dataGridView1.Rows[x].Cells[7].Value.ToString());
                                cmd2.Parameters.AddWithValue("@Loan_Type_Loan_No", dataGridView1.Rows[x].Cells[0].Value.ToString());
                                cmd2.Parameters.AddWithValue("@Other_Deduction", dataGridView1.Rows[x].Cells[5].Value.ToString()); //Bill CURRENT First
                                cmd2.Parameters.AddWithValue("@Applied_Amount", Convert.ToDecimal(Convert.ToDecimal(dataGridView1.Rows[x].Cells[4].Value.ToString()) - Convert.ToDecimal(dataGridView1.Rows[0].Cells[2].Value.ToString())).ToString().Replace(",", "."));
                                cmd2.Parameters.AddWithValue("@Deduction_Type", "LOAN");
                                cmd2.ExecuteNonQuery();
                            }


                            //COUNTER
                            x = x + 1;
                        }

                    }

                }

                if(chckShareCapital.Checked == true && txtShareCapital.Text != "")
                {
                    SqlCommand cmdShareCapital = new SqlCommand();
                    cmdShareCapital.Connection = con;
                    cmdShareCapital.CommandText = "sp_InsertLoanDeductions";
                    cmdShareCapital.CommandType = CommandType.StoredProcedure;
                    cmdShareCapital.Parameters.AddWithValue("@userID", Classes.clsLoanDataEntry.userID);
                    cmdShareCapital.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                    cmdShareCapital.Parameters.AddWithValue("@Loan_Type", "");
                    cmdShareCapital.Parameters.AddWithValue("@Loan_Type_Loan_No", "");
                    cmdShareCapital.Parameters.AddWithValue("@Other_Deduction", "363");
                    cmdShareCapital.Parameters.AddWithValue("@Applied_Amount", Convert.ToDecimal(txtShareCapital.Text));
                    cmdShareCapital.Parameters.AddWithValue("@Deduction_Type", "OTHER DEDUCTION");
                    cmdShareCapital.ExecuteNonQuery();
                }

                if (chckSavings.Checked == true && txtSavings.Text != "")
                {
                    SqlCommand cmdSavings = new SqlCommand();
                    cmdSavings.Connection = con;
                    cmdSavings.CommandText = "sp_InsertLoanDeductions";
                    cmdSavings.CommandType = CommandType.StoredProcedure;
                    cmdSavings.Parameters.AddWithValue("@userID", Classes.clsLoanDataEntry.userID);
                    cmdSavings.Parameters.AddWithValue("@Loan_No", txtLoanNo.Text);
                    cmdSavings.Parameters.AddWithValue("@Loan_Type", "");
                    cmdSavings.Parameters.AddWithValue("@Loan_Type_Loan_No", "");
                    cmdSavings.Parameters.AddWithValue("@Other_Deduction", "300.1");
                    cmdSavings.Parameters.AddWithValue("@Applied_Amount", Convert.ToDecimal(txtSavings.Text));
                    cmdSavings.Parameters.AddWithValue("@Deduction_Type", "OTHER DEDUCTION");
                    cmdSavings.ExecuteNonQuery();
                }

                Alert.show("Loan " + txtLoanNo.Text + " Successfully Approved!", Alert.AlertType.success);

                //Convert.ToString(decimal.Round(Convert.ToDecimal(z), 2)));

                //reflect
                adapter = new SqlDataAdapter("SELECT Approved_By,Approved_Date FROM Loan WHERE Loan_No = '" + txtLoanNo.Text + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                txtApprovedBy.Text = dt.Rows[0].ItemArray[0].ToString();
                txtAppDissDate.Text = Convert.ToDateTime(dt.Rows[0].ItemArray[1].ToString()).ToShortDateString();

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
            else
            {
                return;
            }
        }

        private void chckShareCapital_CheckedChanged(object sender, EventArgs e)
        {
            if(chckShareCapital.Checked == true)
            {
                txtShareCapital.Enabled = true;
                txtShareCapital.Text = "";
            }
            else
            {
                txtShareCapital.Enabled = false;
                txtShareCapital.Text = "0.00";
            }
        }

        private void chckSavings_CheckedChanged(object sender, EventArgs e)
        {
            if (chckSavings.Checked == true)
            {
                txtSavings.Enabled = true;
                txtSavings.Text = "";
            }
            else
            {
                txtSavings.Enabled = false;
                txtSavings.Text = "0.00";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "2")
            {
                //APPROVED
                Alert.show("This Loan already Approved!", Alert.AlertType.error);
                return;
            }

            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "3")
            {
                //DISAPPROVED
                Alert.show("This Loan already Dispproved!", Alert.AlertType.error);
                return;
            }

            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "5")
            {
                //DISAPPROVED
                Alert.show("This Loan already Released!", Alert.AlertType.error);
                return;
            }

            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "7")
            {
                //DISAPPROVED
                Alert.show("This Loan already Cancelled!", Alert.AlertType.error);
                return;
            }

            string msg = Environment.NewLine + "Are you sure you want to continue?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                con = new SqlConnection();
                global.connection(con);

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE LOAN Set Status = '6' WHERE Loan_No ='"+ txtLoanNo.Text +"'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                Alert.show("Loan " + txtLoanNo.Text + " Successfully tag as FBA!", Alert.AlertType.success);

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

        private void button3_Click(object sender, EventArgs e)
        {
            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "2")
            {
                //APPROVED
                Alert.show("This Loan already Approved!", Alert.AlertType.error);
                return;
            }

            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "3")
            {
                //DISAPPROVED
                Alert.show("This Loan already Dispproved!", Alert.AlertType.error);
                return;
            }

            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "5")
            {
                //DISAPPROVED
                Alert.show("This Loan already Released!", Alert.AlertType.error);
                return;
            }

            if (clsApproval.returnStatusNo(txtLoanNo.Text) == "7")
            {
                //DISAPPROVED
                Alert.show("This Loan already Cancelled!", Alert.AlertType.error);
                return;
            }

            string msg = Environment.NewLine + "Are you sure you want to Disapprove this loan?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {

                if (txtReason.Text == "")
                {
                    Alert.show("Reason for Disapproving of loan is required!", Alert.AlertType.error);
                    return;
                }

                con = new SqlConnection();
                global.connection(con);

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE LOAN Set Status = '3',Disapproved_Date = GETDATE(), Disapproved_By = '" + Classes.clsUser.Username + "' ,Note = '" + txtReason.Text + "' WHERE Loan_No ='" + txtLoanNo.Text + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                Alert.show("Loan " + txtLoanNo.Text + " Successfully Disapproved!", Alert.AlertType.success);


                //reflect in the form

                adapter = new SqlDataAdapter("SELECT Disapproved_Date,Disapproved_By FROM Loan WHERE Loan_No = '" + txtLoanNo.Text + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                txtDisapprovedBy.Text = dt.Rows[0].ItemArray[1].ToString();
                txtAppDissDate.Text = Convert.ToDateTime(dt.Rows[0].ItemArray[0].ToString()).ToShortDateString();


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

        private void button4_Click(object sender, EventArgs e)
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
                Alert.show("This Loan already Cancelled!", Alert.AlertType.error);
                return;
            }

            string msg = Environment.NewLine + "Are you sure you want to Cancel this loan?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if(txtReason.Text == "")
                {
                    Alert.show("Reason for cancellation of loan is required!", Alert.AlertType.error);
                    return;
                }

                con = new SqlConnection();
                global.connection(con);

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE LOAN Set Status = '7',Cancelled_Date = GETDATE(), Cancelled_By = '"+ Classes.clsUser.Username +"' ,Note = '"+ txtReason.Text +"' WHERE Loan_No ='" + txtLoanNo.Text + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                Alert.show("Loan " + txtLoanNo.Text + " Successfully Cancelled!", Alert.AlertType.success);

                //reflect in the form

                adapter = new SqlDataAdapter("SELECT Cancelled_Date,Cancelled_By FROM Loan WHERE Loan_No = '"+ txtLoanNo.Text +"'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                txtCancelledBy.Text = dt.Rows[0].ItemArray[1].ToString();
                txtDateCancelled.Text = Convert.ToDateTime(dt.Rows[0].ItemArray[0].ToString()).ToShortDateString();

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

        private void txtShareCapital_Leave(object sender, EventArgs e)
        {
            if (txtShareCapital.Text != "")
            {
                txtShareCapital.Text = Convert.ToDecimal(txtShareCapital.Text).ToString("#,0.00");
            }
        }

        private void txtSavings_Leave(object sender, EventArgs e)
        {
            if (txtSavings.Text != "")
            {
                txtSavings.Text = Convert.ToDecimal(txtSavings.Text).ToString("#,0.00");
            }
        }

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Applied")
            {
                //your code goes here
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString("#,0.00");
            }
        }

        private void label55_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Applied")
            {
                //your code goes here
                compute();
            }
        }

        public void compute()
        {
            decimal sumApplied = 0;
            //Check if theres a beneficiary
            if (dataGridView1.Rows.Count > 0)
            {

                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    sumApplied += Convert.ToDecimal(dataGridView1.Rows[i].Cells[4].Value);
                }
            }

           lblTotalAmount.Text = sumApplied.ToString("#,0.00");
        }
    }
    
}
