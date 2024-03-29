﻿using System;
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
    public partial class LoanType : Form
    {
        public LoanType()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;


        Classes.clsLoanType clsLoanType = new Classes.clsLoanType();
        Classes.clsAccessControl clsAccess = new Classes.clsAccessControl();
        Global global = new Global();

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            if(btnClose.Text == "CLOSE")
            {
                this.Close();
            }
            else
            {
                resetAllControls();
                //CLEAR ONLY
            }
        }

        private void LoanType_Load(object sender, EventArgs e)
        {
            clsLoanType.displayLoans(dataGridView1);
            clsLoanType.loadComboBox(cmbCashAccnt);
            clsLoanType.loadComboBox(cmbDebitCreditAccnt);
            clsLoanType.loadComboBox(cmbServiceFeeAccnt);
            clsLoanType.loadComboBox(cmbCurrentInterestAccnt);
            clsLoanType.loadComboBox(cmbDeferredAccnt);
            clsLoanType.loadComboBox(cmbRefundAccnt);
            clsLoanType.loadComboBox(cmbPastDueAccnt);
            clsLoanType.loadServiceFactore(cmbServicefactorCode);

            foreach (var c in tableLayoutPanel1.Controls)
            {
                if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
            }
        }


        public void resetAllControls()
        {
            foreach (var c in tableLayoutPanel1.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
                if (c is TextBox) ((TextBox)c).Enabled = true;
            }

            foreach (var c in tableLayoutPanel1.Controls)
            {
                if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
            }

            btnSave.Text = "NEW";
            btnClose.Text = "CLOSE";

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (btnSave.Text == "NEW")
            {
                if (clsAccess.checkForInsertRestriction(lblTitle.Text, Classes.clsUser.Username) != true)
                {
                    return;
                }

                resetAllControls();
                btnSave.Text = "SAVE";
                btnClose.Text = "CANCEL";

            }
            else if(btnSave.Text == "SAVE")
            {
                //SAVE
                if(txtLoanType.Text == "")
                {
                    txtLoanType.Focus();
                    Alert.show("Loan Type is required.", Alert.AlertType.error);
                    return;
                }
                
                if(txtDescription.Text == "")
                {
                    txtDescription.Focus();
                    Alert.show("Loan Description is required.", Alert.AlertType.error);
                    return;
                }

                if (txtMinAmount.Text == "")
                {
                    txtMinAmount.Focus();
                    Alert.show("Minimum  Loanable Amount is required.", Alert.AlertType.error);
                    return;
                }

                if (txtMaxAmount.Text == "")
                {
                    txtMaxAmount.Focus();
                    Alert.show("Maximum  Loanable Amount is required.", Alert.AlertType.error);
                    return;
                }

                if (txtMinTerm.Text == "")
                {
                    txtMinTerm.Focus();
                    Alert.show("Minimum Loan Term is required.", Alert.AlertType.error);
                    return;
                }

                if (txtMaxTerm.Text == "")
                {
                    txtMaxTerm.Focus();
                    Alert.show("Maximum Loan Term is required.", Alert.AlertType.error);
                    return;
                }

                if (txtPercent.Text == "")
                {
                    txtPercent.Focus();
                    Alert.show("Interest is Required!", Alert.AlertType.error);
                    return;
                }

                if(cmbCashAccnt.Text == "")
                {
                    Alert.show("Cash Account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbDebitCreditAccnt.Text == "")
                {
                    Alert.show("Cash Account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbServiceFeeAccnt.Text == "")
                {
                    Alert.show("Service Fee Account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbCurrentInterestAccnt.Text == "")
                {
                    Alert.show("Current Interest Account is required.", Alert.AlertType.error);
                    return;
                }
                
                if (cmbDeferredAccnt.Text == "")
                {
                    Alert.show("Deferred Account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbRefundAccnt.Text == "")
                {
                    Alert.show("Refund account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbPastDueAccnt.Text == "")
                {
                    Alert.show("Past-Due Account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbServicefactorCode.Text == "")
                {
                    Alert.show("Service Factor is required.", Alert.AlertType.error);
                    return;
                }

                if (txtPriority.Text == "")
                {
                    txtPriority.Focus();
                    Alert.show("Priority is required.", Alert.AlertType.error);
                    return;
                }

                //=====================================================================
                //          END NO VALUE VALIDATION
                //=====================================================================

                //=====================================================================
                //          Validation for Priority if already used
                //=====================================================================
                if(clsLoanType.checkIfAlreadyUsed(Convert.ToInt32(txtPriority.Text)) == true)
                {
                    Alert.show("Priority number already used.", Alert.AlertType.error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_InsertLoanType";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Loan_Type", txtLoanType.Text);
                    cmd.Parameters.AddWithValue("@Loan_Description", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@MinAmount", txtMinAmount.Text);
                    cmd.Parameters.AddWithValue("@MaxAmount", txtMaxAmount.Text);
                    cmd.Parameters.AddWithValue("@MinTerm", txtMinTerm.Text);
                    cmd.Parameters.AddWithValue("@MaxTerm", txtMaxTerm.Text);
                    cmd.Parameters.AddWithValue("@Interest", Convert.ToDecimal(txtPercent.Text));
                    cmd.Parameters.AddWithValue("@Service_Factor_Code", cmbServicefactorCode.SelectedValue);
                    cmd.Parameters.AddWithValue("@Account_Dr", cmbDebitCreditAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Account_Cr", cmbDebitCreditAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Cash_Account", cmbCashAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Service_Fee_Account", cmbServiceFeeAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Current_Interest_Account", cmbCurrentInterestAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Deferred_Interest_Account", cmbDeferredAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Rebate_Account", cmbRefundAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@PastDue_Account", cmbPastDueAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Priority", txtPriority.Text);
                    cmd.ExecuteNonQuery();
                }

                Alert.show("Successfully inserted.", Alert.AlertType.success);

                clsLoanType.displayLoans(dataGridView1); //Dispaly Real Time

                resetAllControls();
                
            }
            else
            {
                if (clsAccess.checkForEditRestriction(lblTitle.Text, Classes.clsUser.Username) != true)
                {
                    return;
                }

                //UPDATE
                if (txtLoanType.Text == "")
                {
                    txtLoanType.Focus();
                    Alert.show("Loan Type is required.", Alert.AlertType.error);  
                    return;
                }

                if (txtDescription.Text == "")
                {
                    txtDescription.Focus();
                    Alert.show("Loan Description is required.", Alert.AlertType.error);
                    return;
                }

                if (txtMinAmount.Text == "")
                {
                    txtMinAmount.Focus();
                    Alert.show("Minimum  Loanable Amount is required.", Alert.AlertType.error);
                    return;
                }

                if (txtMaxAmount.Text == "")
                {
                    txtMaxAmount.Focus();
                    Alert.show("Maximum  Loanable Amount is required.", Alert.AlertType.error);
                    return;
                }

                if (txtMinTerm.Text == "")
                {
                    txtMinTerm.Focus();
                    Alert.show("Minimum Loan Term is required.", Alert.AlertType.error);
                    return;
                }

                if (txtMaxTerm.Text == "")
                {
                    txtMaxTerm.Focus();
                    Alert.show("Maximum Loan Term is required.", Alert.AlertType.error);
                    return;
                }

                if (txtPercent.Text == "")
                {
                    txtPercent.Focus();
                    Alert.show("Interest is Required!", Alert.AlertType.error);
                    return;
                }

                if (cmbCashAccnt.Text == "")
                {
                    Alert.show("Cash Account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbDebitCreditAccnt.Text == "")
                {
                    Alert.show("Cash Account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbServiceFeeAccnt.Text == "")
                {
                    Alert.show("Service Fee Account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbCurrentInterestAccnt.Text == "")
                {
                    Alert.show("Current Interest Account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbDeferredAccnt.Text == "")
                {
                    Alert.show("Deferred Account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbRefundAccnt.Text == "")
                {
                    Alert.show("Refund account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbPastDueAccnt.Text == "")
                {
                    Alert.show("Past-Due Account is required.", Alert.AlertType.error);
                    return;
                }

                if (cmbServicefactorCode.Text == "")
                {
                    Alert.show("Service Factor is required.", Alert.AlertType.error);
                    return;
                }

                if (txtPriority.Text == "")
                {
                    txtPriority.Focus();
                    Alert.show("Priority is required.", Alert.AlertType.error);
                    return;
                }

                //=====================================================================
                //          END NO VALUE VALIDATION
                //=====================================================================


                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_UpdateLoanType";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Loan_Type", txtLoanType.Text);
                    cmd.Parameters.AddWithValue("@Loan_Description", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@MinAmount", txtMinAmount.Text);
                    cmd.Parameters.AddWithValue("@MaxAmount", txtMaxAmount.Text);
                    cmd.Parameters.AddWithValue("@MinTerm", txtMinTerm.Text);
                    cmd.Parameters.AddWithValue("@MaxTerm", txtMaxTerm.Text);
                    cmd.Parameters.AddWithValue("@Interest", Convert.ToDecimal(txtPercent.Text));
                    cmd.Parameters.AddWithValue("@Service_Factor_Code", cmbServicefactorCode.SelectedValue);
                    cmd.Parameters.AddWithValue("@Account_Dr", cmbDebitCreditAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Account_Cr", cmbDebitCreditAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Cash_Account", cmbCashAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Service_Fee_Account", cmbServiceFeeAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Current_Interest_Account", cmbCurrentInterestAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Deferred_Interest_Account", cmbDeferredAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Rebate_Account", cmbRefundAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@PastDue_Account", cmbPastDueAccnt.SelectedValue);
                    cmd.Parameters.AddWithValue("@Priority", txtPriority.Text);
                    cmd.ExecuteNonQuery();
                }

                Alert.show("Successfully updated.", Alert.AlertType.success);

                clsLoanType.displayLoans(dataGridView1); //Dispaly Real Time

                resetAllControls();
            }
            
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

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                //populate the textbox from specific value of the coordinates of column and row.
                //Disable First txt loan type
                txtLoanType.Enabled = false;
                btnSave.Text = "UPDATE";
                btnClose.Text = "CANCEL";

                txtDescription.Enabled = true;
                txtMinAmount.Enabled = true;
                txtMaxAmount.Enabled = true;
                txtMinTerm.Enabled = true;
                txtMaxTerm.Enabled = true;
                txtPercent.Enabled = true;

                txtLoanType.Text = row.Cells["Loan_Type"].Value.ToString();
                txtDescription.Text = row.Cells["Loan_Description"].Value.ToString();
                txtMinAmount.Text = row.Cells["Min_Loan_Amount"].Value.ToString();
                txtMaxAmount.Text = row.Cells["Max_Loan_Amount"].Value.ToString();
                txtMinTerm.Text = row.Cells["Min_Term"].Value.ToString();
                txtMaxTerm.Text = row.Cells["Max_Term"].Value.ToString();
                txtPercent.Text = row.Cells["Interest"].Value.ToString();
                cmbCashAccnt.SelectedValue = row.Cells["Cash_Account"].Value.ToString();
                cmbDebitCreditAccnt.SelectedValue = row.Cells["Account_Dr"].Value.ToString();
                cmbServiceFeeAccnt.SelectedValue = row.Cells["Service_Fee_Account"].Value.ToString();
                cmbCurrentInterestAccnt.SelectedValue = row.Cells["Current_Interest_Account"].Value.ToString();
                cmbDeferredAccnt.SelectedValue = row.Cells["Deferred_Interest_Account"].Value.ToString();
                cmbRefundAccnt.SelectedValue = row.Cells["Rebate_Account"].Value.ToString();
                cmbPastDueAccnt.SelectedValue = row.Cells["PastDue_Account"].Value.ToString();
                cmbServicefactorCode.SelectedValue = row.Cells["Service_Factor_Code"].Value.ToString();
                txtPriority.Text = row.Cells["Priority"].Value.ToString();
            }
        }
    }
}
