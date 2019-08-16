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
    public partial class LookUp : Form
    {
        public LookUp()
        {
            InitializeComponent();
        }

        SqlConnection con;
        Global global = new Global();
        Classes.clsLookUp clsLookUp = new Classes.clsLookUp();
        Classes.clsSavingsDataEntry clsSavingsDataEntry = new Classes.clsSavingsDataEntry();
        Classes.clsDisbursement clsDisbursement = new Classes.clsDisbursement();
        Classes.clsJournalVoucher clsJournal = new Classes.clsJournalVoucher();
        Classes.clsCashReceipt clsCash = new Classes.clsCashReceipt();
        Classes.clsSavings clsSavings = new Classes.clsSavings();
        Classes.clsHoldAccounts clsHoldAccount = new Classes.clsHoldAccounts();
        clsMembership clsMembership = new clsMembership();

        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        private void label1_Click(object sender, EventArgs e)
        {
           
        }

        private void LookUp_Load(object sender, EventArgs e)
        {
            //Load According to Whos Form Trigger
            switch (Classes.clsLookUp.whosLookUp)
            {
                case "0": //Savings
                    clsLookUp.loadLookUpQuery("Membership Where IsActive = '1' and IsApprove ='1'", dataGridView1);
                    break;
                case "1": //Disbursement
                    clsLookUp.loadLookUpQuery("Membership Where IsActive = '1' and IsApprove ='1'", dataGridView1);
                    break;
                case "2": //OR
                    clsLookUp.loadLookUpQuery("Membership Where IsActive = '1' and IsApprove ='1'", dataGridView1);
                    break;
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            //Load According to Whos Form Trigger
            switch (Classes.clsLookUp.whosLookUp)
            {
                case "0": //Savings
                    clsLookUp.loadLookUpQuery("Membership Where IsActive = '1' and IsApprove ='1'", dataGridView1);
                    clearTextField();
                    break;
                case "1": //Disbursement Search
                    clsLookUp.loadLookUpQuery("Membership Where IsActive = '1' and IsApprove ='1'", dataGridView1);
                    clearTextField();
                    break;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //Load According to Whos Form Trigger
            switch (Classes.clsLookUp.whosLookUp)
            {
                case "0": //Savings
                    if(requiredField() == true)
                    {
                        Alert.show("Please enter valid Keyword.", Alert.AlertType.warning);
                        return;
                    }
                    clsLookUp.SearchSavings(txtEmployeeID.Text,txtFirstName.Text,txtLastName.Text,dataGridView1);
                    break;
                case "1": //Disbursement 
                    if (requiredField() == true)
                    {
                        Alert.show("Please enter valid Keyword.", Alert.AlertType.warning);
                        return;
                    }
                    clsLookUp.SearchMemberDisbursement(txtEmployeeID.Text, txtFirstName.Text, txtLastName.Text, dataGridView1);
                    break;
                case "2": //Cash Receipt
                    if (requiredField() == true)
                    {
                        Alert.show("Please enter valid Keyword.", Alert.AlertType.warning);
                        return;
                    }
                    clsLookUp.SearchMemberCashReceipt(txtEmployeeID.Text, txtFirstName.Text, txtLastName.Text, dataGridView1);
                    break;
            }
        }

        public bool requiredField()
        {
            if (txtEmployeeID.Text == "" && txtFirstName.Text == "" && txtLastName.Text == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void clearTextField()
        {
            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (Classes.clsLookUp.whosLookUp)
            {
                case "0": //Savings
                    SavingsDataEntry savingsDataentry = new SavingsDataEntry();

                    //Check for hold accounts
                    if (clsHoldAccount.checkIfHoldAccount(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString())) == true)
                    {
                        Alert.show("Member's account is on hold.", Alert.AlertType.error);
                        return;
                    }

                    if (clsHoldAccount.checkIfTHeresADependent(dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString()) == true)
                    {
                        Alert.show("Member's account is on hold.", Alert.AlertType.error);
                        return;
                    }
                    //for dependent purposes


                    //For Resigned Member
                    if(clsMembership.isResigned(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString())) == true)
                    {
                        //Already Resigned
                        DialogResult result = MessageBox.Show(this, "This member already resigned, are you sure you want to continue?", "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }

                    foreach (Form form in Application.OpenForms)
                    {
                        //Start Validation first if selected account is hold                   

                        if (form.GetType() == typeof(SavingsDataEntry))
                        {
                            //===============================================================================
                            //                      If form is already open
                            //===============================================================================
                            form.Activate();
                            savingsDataentry = (SavingsDataEntry)Application.OpenForms["SavingsDataEntry"];
                            savingsDataentry.txtEmployeeID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                            Classes.clsSavingsDataEntry.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                            savingsDataentry.txtName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();

                            if (dataGridView1.SelectedRows[0].Cells["Savings_Deposit"].Value == DBNull.Value)
                            {
                                savingsDataentry.txtSDDeduction.Text = "0.00";
                            }
                            else
                            {
                                savingsDataentry.txtSDDeduction.Text = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Savings_Deposit"].Value).ToString("#,##0.00");
                            }
                            
                            savingsDataentry.txtCurrentBalanceBeforeWithdrawal.Text = clsSavingsDataEntry.returnMembersSaving(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());

                            //If theres a deposited check
                            savingsDataentry.txtdepositedCheque.Text = clsSavings.returnDepositedChequeAmount(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
                            savingsDataentry.txtDepositedDate.Text = clsSavings.returnDepositedChequeDate(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));

                            //MINUS THE CHECK AMOUNT
                            if(savingsDataentry.txtdepositedCheque.Text != "")
                            {
                                Decimal widAmnt;
                                widAmnt = Convert.ToDecimal(savingsDataentry.txtCurrentBalanceBeforeWithdrawal.Text) - Convert.ToDecimal(savingsDataentry.txtdepositedCheque.Text);
                                savingsDataentry.txtWithdrawalBalance.Text = widAmnt.ToString("#,0.00");
                            }
                            else
                            {
                                savingsDataentry.txtWithdrawalBalance.Text = clsSavingsDataEntry.returnMembersSaving(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                            }
                            

                            savingsDataentry.txtCompany.Text = clsSavingsDataEntry.returnCompanyDescription(dataGridView1.SelectedRows[0].Cells["Company_Code"].Value.ToString());
                            savingsDataentry.txtLastDateWithdrawal.Text = clsSavings.returnLastWithdrawalDate(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));

                            //For Account number and Bank Code
                            savingsDataentry.txtAccntNo.Text = dataGridView1.SelectedRows[0].Cells["ATM_Account_No"].Value.ToString();
                            savingsDataentry.txtBankCode.Text = dataGridView1.SelectedRows[0].Cells["Bank_Code"].Value.ToString();

                            savingsDataentry.txtAmountWithdrawn.Focus();
                            this.Close();
                            return;
                        }
                    }

                    savingsDataentry.txtEmployeeID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                    savingsDataentry.txtLastDateWithdrawal.Text = clsSavings.returnLastWithdrawalDate(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
                    Classes.clsSavingsDataEntry.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                    
                    savingsDataentry.txtName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();

                    if (dataGridView1.SelectedRows[0].Cells["Savings_Deposit"].Value == DBNull.Value)
                    {
                        savingsDataentry.txtSDDeduction.Text = "0.00";
                    }
                    else
                    {
                        savingsDataentry.txtSDDeduction.Text = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Savings_Deposit"].Value).ToString("#,##0.00");
                    }

                    savingsDataentry.txtCurrentBalanceBeforeWithdrawal.Text = clsSavingsDataEntry.returnMembersSaving(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());

                    //If theres a deposited check
                    savingsDataentry.txtdepositedCheque.Text = clsSavings.returnDepositedChequeAmount(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
                    savingsDataentry.txtDepositedDate.Text = clsSavings.returnDepositedChequeDate(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));

                    //MINUS THE CHECK AMOUNT
                    if (savingsDataentry.txtdepositedCheque.Text != "")
                    {
                        Decimal widAmnt;
                        widAmnt = Convert.ToDecimal(savingsDataentry.txtCurrentBalanceBeforeWithdrawal.Text) - Convert.ToDecimal(savingsDataentry.txtdepositedCheque.Text);
                        savingsDataentry.txtWithdrawalBalance.Text = widAmnt.ToString("#,0.00");
                    }
                    else
                    {
                        savingsDataentry.txtWithdrawalBalance.Text = clsSavingsDataEntry.returnMembersSaving(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                    }


                    savingsDataentry.txtCompany.Text = clsSavingsDataEntry.returnCompanyDescription(dataGridView1.SelectedRows[0].Cells["Company_Code"].Value.ToString());

                    //For Account number and Bank Code
                    savingsDataentry.txtAccntNo.Text = dataGridView1.SelectedRows[0].Cells["ATM_Account_No"].Value.ToString();
                    savingsDataentry.txtBankCode.Text = dataGridView1.SelectedRows[0].Cells["Bank_Code"].Value.ToString();


                    savingsDataentry.Show();

                    this.Close();
                    savingsDataentry.txtAmountWithdrawn.Focus();
                    savingsDataentry.Show();
                    break;


                case "1":
                    //==========================================================================================
                    //                      DISBURSEMENT CODE
                    //==========================================================================================
                    DisbursementVoucher disbursement = new DisbursementVoucher();
                    
                    foreach (Form form in Application.OpenForms)
                    {

                        if (form.GetType() == typeof(DisbursementVoucher))
                        {
                            //===============================================================================
                            //                      If form is already open
                            //===============================================================================
                            form.Activate();
                            disbursement = (DisbursementVoucher)Application.OpenForms["DisbursementVoucher"];
                            Classes.clsDisbursement.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                            LoanLookUpProcess.clsLoanLookUpMember.userid = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());

                            disbursement.txtLoanNo.Text = "";
                            disbursement.txtLoanType.Text = "";

                            disbursement.txtPayee.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                            disbursement.txtPayeeName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();
                            this.Close();
                            return;
                        }
                    }

                    Classes.clsDisbursement.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                    LoanLookUpProcess.clsLoanLookUpMember.userid = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());

                    disbursement.txtLoanNo.Text = "";
                    disbursement.txtLoanType.Text = "";

                    disbursement.txtPayee.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                    disbursement.txtPayeeName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();
                    disbursement.Show();
                    this.Close();
                    break;
                case "2":
                    //==========================================================================================
                    //                      CASH RECEIPT CODE
                    //==========================================================================================
                    CashReceiptVoucher cashOR= new CashReceiptVoucher();

                    foreach (Form form in Application.OpenForms)
                    {

                        if (form.GetType() == typeof(CashReceiptVoucher))
                        {
                            //===============================================================================
                            //                      If form is already open
                            //===============================================================================
                            form.Activate();
                            cashOR = (CashReceiptVoucher)Application.OpenForms["CashReceiptVoucher"];
                            Classes.clsCashReceipt.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                            cashOR.txtPayorID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                            cashOR.txtPayorName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();
                            cashOR.txtPayorCompany.Text = clsCash.returnCompanyDescription(dataGridView1.SelectedRows[0].Cells["Company_Code"].Value.ToString());
                            this.Close();
                            return;
                        }
                    }

                    Classes.clsCashReceipt.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                    cashOR.txtPayorID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                    cashOR.txtPayorName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();
                    cashOR.txtPayorCompany.Text = clsCash.returnCompanyDescription(dataGridView1.SelectedRows[0].Cells["Company_Code"].Value.ToString());
                    cashOR.Show();
                    this.Close();
                    break;
            }
        }

        private void txtEmployeeID_TextChanged(object sender, EventArgs e)
        {
            //Load According to Whos Form Trigger
            switch (Classes.clsLookUp.whosLookUp)
            {
                case "0": //Savings
                    clsLookUp.SearchSavings(txtEmployeeID.Text, txtFirstName.Text, txtLastName.Text, dataGridView1);
                    break;
                case "1":
                    break;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
