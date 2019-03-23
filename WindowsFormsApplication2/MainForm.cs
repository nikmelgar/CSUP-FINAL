using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.WindowState = FormWindowState.Maximized;

            //LoadName
            lblFullName.Text = Classes.clsUser.firstName + " " + Classes.clsUser.middleName + " " + Classes.clsUser.lastName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sideVisible(btnFileMaintenance);

            panelFileMaintenanceSub.Visible = true;
            panelFileMaintenanceSub.Top = btnFileMaintenance.Bottom;
            panelFileMaintenanceSub.Left = panelMenu.Right;
            panelFileMaintenanceSub.Height = panelMenu.Bottom;

            //Hide Panels
            panelProcessSub.Visible = false;
            panelSavings.Visible = false;
            panelMembership.Visible = false;
            panelReportSub.Visible = false;
            panelMemberSettings.Visible = false;
            panelLoan.Visible = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            sideVisible(btnDashBoard);

            //Hide Panels who is ON
            panelFileMaintenanceSub.Visible = false;
            panelProcessSub.Visible = false;
            panelSavings.Visible = false;
            panelMembership.Visible = false;
            panelReportSub.Visible = false;
            panelMemberSettings.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        //For Side Panel 
        public void sideVisible(Button btnName)
        {
            SidePanel.Height = btnName.Height;
            SidePanel.Top = btnName.Top;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            sideVisible(btnProcess);
                     
            panelProcessSub.Visible = true;
            panelProcessSub.Top = btnProcess.Bottom;
            panelProcessSub.Left = panelMenu.Right;
            panelProcessSub.Height = panelMenu.Bottom;

            //Hide Panels
            panelFileMaintenanceSub.Visible = false;
            panelSavings.Visible = false;
            panelMembership.Visible = false;
            panelMemberSettings.Visible = false;
            panelReportSub.Visible = false;
            panelLoan.Visible = false;
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            sideVisible(btnReports);
            panelReportSub.Visible = true;
            panelReportSub.Top = btnReports.Bottom;
            panelReportSub.Left = panelMenu.Right;
            panelReportSub.Height = panelMenu.Bottom;

            //Hide Panels
            panelFileMaintenanceSub.Visible = false;
            panelSavings.Visible = false;
            panelMembership.Visible = false;
            panelMemberSettings.Visible = false;
            panelProcessSub.Visible = false;
            panelLoan.Visible = false;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if(panelMenu.Width == 341)
            {
                panelMenu.Width = 107;
                
                //check if sub panel is on
                if(panelFileMaintenanceSub.Visible == true) //FIle Maintenance
                {
                    panelFileMaintenanceSub.Left = panelMenu.Right;
                }                               

                if(panelSavings.Visible == true)
                {
                    panelSavings.Left = panelMenu.Right;
                }

                if(panelProcessSub.Visible == true)
                {
                    panelProcessSub.Left = panelMenu.Right;
                }

                if (panelMembership.Visible == true)
                {
                    panelMembership.Left = panelMenu.Right;
                }
            }
            else
            {
                panelMenu.Width = 341;

                //check if sub panel is on
                if (panelFileMaintenanceSub.Visible == true)
                {
                    panelFileMaintenanceSub.Left = panelMenu.Right;
                }

                if (panelSavings.Visible == true)
                {
                    panelSavings.Left = panelMenu.Right;
                }

                if (panelProcessSub.Visible == true)
                {
                    panelProcessSub.Left = panelMenu.Right;
                }

                if (panelMembership.Visible == true)
                {
                    panelMembership.Left = panelMenu.Right;
                }
            }
        }

        private void btnMembership_Click(object sender, EventArgs e)
        {
            sideVisible(btnMembership);
            panelMembership.Visible = true;
            panelMembership.Top = btnMembership.Bottom;
            panelMembership.Left = panelMenu.Right;
            panelMembership.Height = panelMenu.Bottom;

            //Hide Panels
            panelFileMaintenanceSub.Visible = false;
            panelProcessSub.Visible = false;
            panelSavings.Visible = false;
            panelMemberSettings.Visible = false;
            panelReportSub.Visible = false;
            panelLoan.Visible = false;

        }

        private void panelMenu_Click(object sender, EventArgs e)
        {
            //Hide all the panels visible
            panelFileMaintenanceSub.Visible = false;
            panelProcessSub.Visible = false;
            panelSavings.Visible = false;
            panelMembership.Visible = false;
            panelReportSub.Visible = false;
            panelLoan.Visible = false;
            panelMemberSettings.Visible = false;
        }

        private void btnCompany_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelFileMaintenanceSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Company))
                {
                    form.Activate();
                    return;
                }
            }

            Company frm = new Company();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelFileMaintenanceSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(CostCenter))
                {
                    form.Activate();
                    return;
                }
            }

            CostCenter frm = new CostCenter();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelFileMaintenanceSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(PayrollGroup))
                {
                    form.Activate();
                    return;
                }
            }

            PayrollGroup frm = new PayrollGroup();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelFileMaintenanceSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(TransactionType))
                {
                    form.Activate();
                    return;
                }
            }

            TransactionType frm = new TransactionType();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelFileMaintenanceSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Client))
                {
                    form.Activate();
                    return;
                }
            }

            Client frm = new Client();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelFileMaintenanceSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(ChartOfAccounts))
                {
                    form.Activate();
                    return;
                }
            }

            ChartOfAccounts frm = new ChartOfAccounts();
            frm.Show();
            frm.MdiParent = this;
            frm.treeView1.ExpandAll();
            frm.clearFields();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelFileMaintenanceSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Banks))
                {
                    form.Activate();
                    return;
                }
            }

            Banks frm = new Banks();
            frm.Show();
            frm.MdiParent = this;
            frm.cleartxtField();
        }

        private void btnSavings_Click(object sender, EventArgs e)
        {
            sideVisible(btnSavings);
            
            panelSavings.Visible = true;
            panelSavings.Top = btnSavings.Bottom;
            panelSavings.Left = panelMenu.Right;
            panelSavings.Height = panelMenu.Bottom;


            //Hide Panels
            panelFileMaintenanceSub.Visible = false;
            panelProcessSub.Visible = false;
            panelMembership.Visible = false;
            panelMemberSettings.Visible = false;
            panelReportSub.Visible = false;
            panelLoan.Visible = false;


        }

        private void btnLoans_Click(object sender, EventArgs e)
        {
            sideVisible(btnLoans);

            panelLoan.Visible = true;
            panelLoan.Top = btnLoans.Bottom;
            panelLoan.Left = panelMenu.Right;
            panelLoan.Height = panelMenu.Bottom;


            //Hide Panels
            panelFileMaintenanceSub.Visible = false;
            panelProcessSub.Visible = false;
            panelMembership.Visible = false;
            panelMemberSettings.Visible = false;
            panelReportSub.Visible = false;
            panelSavings.Visible = false;
        }

        private void btnReplenishment_Click(object sender, EventArgs e)
        {
            //Hide sub menu Process
            panelSavings.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(CashReplenishment))
                {
                    form.Activate();
                    return;
                }
            }

            CashReplenishment frm = new CashReplenishment();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //Hide Panels
            panelSavings.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Savings))
                {
                    form.Activate();
                    return;
                }
            }

            Savings frm = new Savings();
            frm.Show();
            frm.MdiParent = this;
        }

        private void panelSavings_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnParameter_Click(object sender, EventArgs e)
        {
            sideVisible(btnParameter);


            //Hide Panels
            panelFileMaintenanceSub.Visible = false;
            panelProcessSub.Visible = false;
            panelSavings.Visible = false;
            panelMembership.Visible = false;
            panelMemberSettings.Visible = false;
            panelReportSub.Visible = false;
            panelLoan.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Parameter))
                {
                    form.Activate();
                    return;
                }
            }

            Parameter frm = new Parameter();
            frm.Show();
            frm.MdiParent = this;


            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //Hide Panel
            panelMembership.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(MembershipMain))
                {
                    form.Activate();
                    return;
                }
            }

            MembershipMain membership = new MembershipMain();
            membership.Show();
            membership.MdiParent = this;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Hide Panel
            panelMembership.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(MembershipBatchApprove))
                {
                    form.Activate();
                    return;
                }
            }

            MembershipBatchApprove membership = new MembershipBatchApprove();
            membership.Show();
            membership.MdiParent = this;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                panelSavings.Visible = false;
                panelFileMaintenanceSub.Visible = false;
                panelMembership.Visible = false;
                panelProcessSub.Visible = false;
                panelMemberSettings.Visible = false;
                panelReportSub.Visible = false;
                panelLoan.Visible = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelProcessSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(JournalVoucher))
                {
                    form.Activate();
                    return;
                }
            }

            JournalVoucher frm = new JournalVoucher();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelProcessSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(DisbursementVoucher))
                {
                    form.Activate();
                    return;
                }
            }

            DisbursementVoucher frm = new DisbursementVoucher();
            frm.Show();
            frm.MdiParent = this;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
           //Environment.Exit(0);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if(panelMemberSettings.Visible == true)
            {
                panelMemberSettings.Visible = false;
            }
            else
            {
                panelMemberSettings.Visible = true;
                panelMemberSettings.Top = panelHeader.Bottom;
                panelMemberSettings.Location = new Point(label3.Location.X - panelMemberSettings.Width + label3.Width, panelHeader.Bottom);

                panelFileMaintenanceSub.Visible = false;
                panelProcessSub.Visible = false;
                panelSavings.Visible = false;
                panelMembership.Visible = false;
                panelReportSub.Visible = false;
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            panelMemberSettings.Visible = false;

            string msg = Environment.NewLine + "Are you sure you want to close this program?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
            else
            {
                return;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            panelMemberSettings.Visible = false;
            this.WindowState = FormWindowState.Minimized;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            //Hide sub menu Process
            panelSavings.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(SavingsWithdrawalATM))
                {
                    form.Activate();
                    return;
                }
            }

            SavingsWithdrawalATM frm = new SavingsWithdrawalATM();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelProcessSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(CashReceiptVoucher))
                {
                    form.Activate();
                    return;
                }
            }

            CashReceiptVoucher frm = new CashReceiptVoucher();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelProcessSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(ATMDiskBankAdvice))
                {
                    form.Activate();
                    return;
                }
            }

            ATMDiskBankAdvice frm = new ATMDiskBankAdvice();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelProcessSub.Visible = false;

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

        private void button20_Click(object sender, EventArgs e)
        {
            //Hide Panel
            panelReportSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(rptSavingsMain))
                {
                    form.Activate();
                    return;
                }
            }

            rptSavingsMain frm = new rptSavingsMain();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button21_Click(object sender, EventArgs e)
        {
            //Hide Panel
            panelMembership.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(HoldAccounts))
                {
                    form.Activate();
                    return;
                }
            }

            HoldAccounts frm = new HoldAccounts();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            //Hide Panel
            panelLoan.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Loans))
                {
                    form.Activate();
                    return;
                }
            }

            Loans frm = new Loans();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelFileMaintenanceSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(LoanType))
                {
                    form.Activate();
                    return;
                }
            }

            LoanType frm = new LoanType();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            //Hide Panel
            panelLoan.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(LoanAmmortizationComputationcs))
                {
                    form.Activate();
                    return;
                }
            }

            LoanAmmortizationComputationcs frm = new LoanAmmortizationComputationcs();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button24_Click(object sender, EventArgs e)
        {
            //Hide Panel
            panelLoan.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(LoanComputation))
                {
                    form.Activate();
                    return;
                }
            }

            LoanComputation frm = new LoanComputation();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button25_Click(object sender, EventArgs e)
        {
            //Hide Panel
            panelLoan.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(LoanATM))
                {
                    form.Activate();
                    return;
                }
            }

            LoanATM frm = new LoanATM();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button26_Click(object sender, EventArgs e)
        {
            //Hide Panel
            panelReportSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(ReportsForms.rptPLBI))
                {
                    form.Activate();
                    return;
                }
            }

            ReportsForms.rptPLBI frm = new ReportsForms.rptPLBI();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button27_Click(object sender, EventArgs e)
        {
            //Hide Panel
            panelReportSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(loanReport))
                {
                    form.Activate();
                    return;
                }
            }

            loanReport frm = new loanReport();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button28_Click(object sender, EventArgs e)
        {
            sideVisible(button28);


            //Hide Panels
            panelFileMaintenanceSub.Visible = false;
            panelProcessSub.Visible = false;
            panelSavings.Visible = false;
            panelMembership.Visible = false;
            panelMemberSettings.Visible = false;
            panelReportSub.Visible = false;
            panelLoan.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(BillingAndCollectionMain))
                {
                    form.Activate();
                    return;
                }
            }

            BillingAndCollectionMain frm = new BillingAndCollectionMain();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button29_Click(object sender, EventArgs e)
        {

            //Hide sub menu filemaintenance
            panelProcessSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(CashTransactionViewing))
                {
                    form.Activate();
                    return;
                }
            }

            CashTransactionViewing frm = new CashTransactionViewing();
            frm.Show();
            frm.MdiParent = this;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            sideVisible(btnQuery);


            //Hide Panels
            panelFileMaintenanceSub.Visible = false;
            panelProcessSub.Visible = false;
            panelSavings.Visible = false;
            panelMembership.Visible = false;
            panelMemberSettings.Visible = false;
            panelReportSub.Visible = false;
            panelLoan.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Query))
                {
                    form.Activate();
                    return;
                }
            }

            Query frm = new Query();
            frm.Show();
            frm.MdiParent = this;
        }
    }
}
