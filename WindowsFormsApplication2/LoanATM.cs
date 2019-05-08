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
    public partial class LoanATM : Form
    {
        public LoanATM()
        {
            InitializeComponent();
        }
        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        Classes.clsLoanATM clsLoanATM = new Classes.clsLoanATM();
        Global global = new Global();

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        private void btnClose_Click(object sender, EventArgs e)
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

        private void LoanATM_Load(object sender, EventArgs e)
        {
            clsLoanATM.loadATMWithdrawal(dataGridView2);
            clsLoanATM.loadBank(cmbBank);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            clsLoanATM.search(txtEmployeeID, txtLastName, txtFirstName, txtLoanNo, cmbBank, dataGridView2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clsLoanATM.loadATMWithdrawal(dataGridView2);
            clsLoanATM.loadBank(cmbBank);
            txtReason.Text = "";
            txtEmployeeID.Text = "";
            txtLastName.Text = "";
            txtFirstName.Text = "";
            txtLoanNo.Text = "";
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            if(dataGridView2.Rows.Count > 0)
            {
                string msg = Environment.NewLine + "Are you sure you want to continue?";
                DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();

                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            cmd = new SqlCommand();
                            cmd.Connection = con;
                            cmd.CommandText = "sp_InsertLoanToATM";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@userID", row.Cells["userID"].Value);
                            cmd.Parameters.AddWithValue("@EmployeeID", row.Cells["EmployeeID"].Value);
                            cmd.Parameters.AddWithValue("@Account_No", row.Cells["Atm_Account_No"].Value);
                            cmd.Parameters.AddWithValue("@Bank_Code", row.Cells["Bank_Code"].Value);
                            cmd.Parameters.AddWithValue("@Amount", row.Cells["NetProceeds"].Value);
                            cmd.Parameters.AddWithValue("@Purpose", row.Cells["Loan_Type"].Value);
                            cmd.Parameters.AddWithValue("@Name", row.Cells["LastName"].Value + ", " + row.Cells["FirstName"].Value + " " + row.Cells["MiddleName"].Value + " " + row.Cells["Suffix"].Value);
                            cmd.Parameters.AddWithValue("@Loan_No", row.Cells["Loan_No"].Value);
                            cmd.Parameters.AddWithValue("@jv_no", row.Cells["jv_no"].Value);
                            cmd.ExecuteNonQuery();

                            SqlCommand cmd2 = new SqlCommand();
                            cmd2.Connection = con;
                            cmd2.CommandText = "UPDATE Loan SET Status = '5' WHERE Loan_No = '" + row.Cells["Loan_No"].Value + "'";
                            cmd2.CommandType = CommandType.Text;
                            cmd2.ExecuteNonQuery();

                            //UPDATE JOURNAL = POSTED
                            SqlCommand cmd3 = new SqlCommand();
                            cmd3.Connection = con;
                            cmd3.CommandType = CommandType.Text;
                            cmd3.CommandText = "UPDATE Journal_Header SET Posted = '1', Posted_By = '" + Classes.clsUser.Username + "' WHERE JV_No = '" + row.Cells["jv_no"].Value + "'";
                            cmd3.ExecuteNonQuery();
                        }
                    }
                        Alert.show("Loan Successfully Posted", Alert.AlertType.success);

                    //refresh
                    clsLoanATM.loadATMWithdrawal(dataGridView2);
                    clsLoanATM.loadBank(cmbBank);
                }
            }
            else
            {
                Alert.show("No record/s for posting.", Alert.AlertType.error);
                return;
            }
        }

        private void txtLastName_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Space);
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(dataGridView2.Rows.Count > 0)
            {
                if (dataGridView2.SelectedRows.Count == 0)
                {
                    //No Data to be edit
                    Alert.show("Please select record first.", Alert.AlertType.warning);
                    return;
                }

                if(txtReason.Text == "")
                {
                    Alert.show("Reason is required for cancellation of loan.", Alert.AlertType.error);
                    return;
                }

                string msg = Environment.NewLine + "Are you sure you want to cancel this loan?";
                DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    /*
                    *   cancel loan
                    *   cancel journal 
                    *   task to cancel
                    */

                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();

                        //Cancelled Loan
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE Loan SET Status = '7', Note ='"+ txtReason.Text +"' WHERE Loan_No = '"+ dataGridView2.SelectedRows[0].Cells["Loan_No"].Value.ToString() +"'";
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();

                        //Cancelled JV
                        SqlCommand cmdJV = new SqlCommand();
                        cmdJV.Connection = con;
                        cmdJV.CommandText = "UPDATE Journal_Header SET Posted = '0', Cancelled = '1', Cancelled_By = '"+ Classes.clsUser.Username +"', Cancel_Note = '"+ txtReason.Text +"' WHERE jv_no = '"+ dataGridView2.SelectedRows[0].Cells["jv_no"].Value.ToString() + "'";
                        cmdJV.CommandType = CommandType.Text;
                        cmdJV.ExecuteNonQuery();
                    }
                    Alert.show("Loan successfully cancelled.", Alert.AlertType.success);
                    //refresh
                    clsLoanATM.loadATMWithdrawal(dataGridView2);
                    clsLoanATM.loadBank(cmbBank);
                }

            }
            else
            {
                Alert.show("No records found.", Alert.AlertType.error);
                return;
            }
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count > 0)
            {
                if (dataGridView2.SelectedRows.Count == 0)
                {
                    //No Data to be edit
                    Alert.show("Please select record first.", Alert.AlertType.warning);
                    return;
                }

                if (txtReason.Text == "")
                {
                    Alert.show("Reason is required.", Alert.AlertType.error);
                    return;
                }

                string msg = Environment.NewLine + "Are you sure you want to return this loan for approval?";
                DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();

                        //Cancelled JV
                        SqlCommand cmdJV = new SqlCommand();
                        cmdJV.Connection = con;
                        cmdJV.CommandText = "UPDATE Journal_Header SET Posted = '0', Cancelled = '1', Cancelled_By = '" + Classes.clsUser.Username + "', Cancel_Note = '" + txtReason.Text + "' WHERE jv_no = '" + dataGridView2.SelectedRows[0].Cells["jv_no"].Value.ToString() + "'";
                        cmdJV.CommandType = CommandType.Text;
                        cmdJV.ExecuteNonQuery();
                        

                        //RETURN TO FOR APPROVAL
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE Loan SET Status = '1', jv_no = NULL WHERE loan_no = '"+ dataGridView2.SelectedRows[0].Cells["loan_no"].Value.ToString() +"'";
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }

                    Alert.show("Loan No "+ dataGridView2.SelectedRows[0].Cells["loan_no"].Value.ToString() + " successfully returned for approval.", Alert.AlertType.success);
                    //refresh
                    clsLoanATM.loadATMWithdrawal(dataGridView2);
                    clsLoanATM.loadBank(cmbBank);
                }
            }
            else
            {
                Alert.show("No records found.", Alert.AlertType.error);
                return;
            }

        }
    }
}
