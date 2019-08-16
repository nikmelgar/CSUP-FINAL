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
    public partial class InvalidAccounts : Form
    {
        public InvalidAccounts()
        {
            InitializeComponent();
        }

        //=======================================================
        //              MOVEABLE PANEL
        //=======================================================

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        //=======================================================
        //              CLASSES
        //=======================================================
        Global global = new Global();
        Classes.clsATMRejects clsATMReject = new Classes.clsATMRejects();
        Classes.clsAccessControl clsAccess = new Classes.clsAccessControl();

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(txtSearchText.Text == "")
            {
                Alert.show("Please enter a valid keyword.", Alert.AlertType.error);
                return;
            }
            clsATMReject.searchInvalidAccount(txtSearchText, txtSearchText.Text.Trim(), dataGridView1);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForDeleteRestriction("ATM Rejects", Classes.clsUser.Username) != true)
            {
                return;
            }

            if (dataGridView1.SelectedRows.Count == 0)
            {
                if(dataGridView1.Rows.Count > 0)
                {
                    Alert.show("Please select account to be remove.", Alert.AlertType.error);
                    return;
                }
                else
                {
                    Alert.show("No record(s) to be removed.", Alert.AlertType.error);
                    return;
                }
            }

            string msg = Environment.NewLine + "Are you sure you want to continue?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //For Removing of Transaction = Savings or Loans what ever is selected
                if(dataGridView1.SelectedRows[0].Cells["Purpose"].Value.ToString() == "SD")
                {
                    //Withdrawal ATM
                    //=======================================================================
                    //      Tag as CANCEL the WITHDRAWAL SLIP FIRST
                    //=======================================================================
                    clsATMReject.tagSDCancel(dataGridView1.SelectedRows[0].Cells["wd_loan_slip"].Value.ToString());
                    clsATMReject.updateJVandRecompute(dataGridView1.SelectedRows[0].Cells["jv_no"].Value.ToString(), dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());

                    //=======================================================================
                    //      REMOVE IN ATM TABLE AFTER
                    //=======================================================================
                    clsATMReject.removeATM(dataGridView1.SelectedRows[0].Cells["wd_loan_slip"].Value.ToString());
                }
                else
                {
                    //=======================================================================
                    //      FOR LOANS REMOVE
                    //=======================================================================
                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();

                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE Journal_Header SET Cancelled = '1' , Cancelled_By = '" + Classes.clsUser.Username + "' , Posted = '0', Cancel_Note ='Invalid Account Number', Particulars = 'Invalid Account Number' WHERE JV_No = '" + dataGridView1.SelectedRows[0].Cells["jv_no"].Value.ToString() + "'";
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();

                        SqlCommand cmd2 = new SqlCommand();
                        cmd2.Connection = con;
                        cmd2.CommandText = "UPDATE Loan SET Status = '2' WHERE Loan_No = '" + dataGridView1.SelectedRows[0].Cells["wd_loan_slip"].Value.ToString() + "'";
                        cmd2.CommandType = CommandType.Text;
                        cmd2.ExecuteNonQuery();

                        //=======================================================================
                        //      REMOVE IN ATM TABLE AFTER
                        //=======================================================================
                        clsATMReject.removeATM(dataGridView1.SelectedRows[0].Cells["wd_loan_slip"].Value.ToString());
                    }

                }

                //End of Process
                Alert.show("Successfully removed.", Alert.AlertType.success);

                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    dataGridView1.Rows.Remove(row);
                }
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(InvalidAccounts))
                {
                    form.Activate();
                    return;
                }
            }

            InvalidAccounts frm = new InvalidAccounts();
            frm.Show();
            frm.MdiParent = this;
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
