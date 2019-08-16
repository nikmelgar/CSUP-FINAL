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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        private int counter = 3600;
        private int checkCounter = 5;
        Classes.clsReminder clsReminder = new Classes.clsReminder();
        Classes.clsUser clsUser = new Classes.clsUser();
        Classes.clsOpenTransaction clsOpen = new Classes.clsOpenTransaction();
        Classes.clsAccessControl clsAccess = new Classes.clsAccessControl();
        Classes.clsReminderSavings clsReminderSavings = new Classes.clsReminderSavings();
        Global global = new Global();
        Users.clsSettings clsSettings = new Users.clsSettings();

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.WindowState = FormWindowState.Maximized;

            //LoadName
            lblFullName.Text = Classes.clsUser.firstName + " " + Classes.clsUser.middleName + " " + Classes.clsUser.lastName;
            lblDeparment.Text = clsUser.returnDepartmentDescription();

            //Loan Image
            clsSettings.loadPicture(pictureBox1);
            clsSettings.loadPicture(pictureBox2);
            //For PDC REMINDER
            if (clsAccess.checkForViewingRestriction("PDC Management",Classes.clsUser.Username) == true)
            {
                //FOR PDC
                if (clsReminder.pdcDue() == true)
                {
                    ReminderPDC.reminder frm = new ReminderPDC.reminder();
                    //SHOW NOTIF BUTTON
                    clsReminder.getCntPDCdueToday(btnNotifPDC, frm.lblSpiel, frm.btnRemid);

                    //FOR PDC MANAGEMENT
                    frm.ShowDialog();
                }
                else
                {
                    btnNotifPDC.Visible = false;
                }
            }


            //Alarm 4 days before due date for savings deduction
            String sDate = DateTime.Now.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            int dy = Convert.ToInt32(datevalue.Day.ToString());

            if(dy >= 12 && dy <= 15)
            {
                //Check All loans with monthly payment in day of 1-15 every month
                if(clsReminderSavings.checkIfHasLoanSavingsDue(1, 15) == true)
                {
                    ReminderSavings.ReminderSavingsDeduction frm = new ReminderSavings.ReminderSavingsDeduction();
                    frm.ShowDialog();
                }
            }
           
            if(dy >= 27 && dy <= 31)
            {
                //Check All loans with monthly payment in day of 16-31 every month
                if (clsReminderSavings.checkIfHasLoanSavingsDue(16, 31) == true)
                {
                    ReminderSavings.ReminderSavingsDeduction frm = new ReminderSavings.ReminderSavingsDeduction();
                    frm.ShowDialog();
                }
            }


            //Open the Dashboard of teamheads
            //1 = Accounting
            //3 = Team Head

            //Open the Dashboard of teamheads
            //3 = Audit
            //3 = Team Head
            if (Classes.clsUser.department.ToString() == "1" || Classes.clsUser.department.ToString() == "3")
            {
                button1_Click(sender, e);
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnFileMaintenance.Text, Classes.clsUser.Username) == true)
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
                panelPDCManagement.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForViewingRestriction(btnDashBoard.Text, Classes.clsUser.Username) == true)
            {
                sideVisible(btnDashBoard);

                //Hide Panels
                panelProcessSub.Visible = false;
                panelSavings.Visible = false;
                panelMembership.Visible = false;
                panelReportSub.Visible = false;
                panelMemberSettings.Visible = false;
                panelLoan.Visible = false;
                panelPDCManagement.Visible = false;
                panelFileMaintenanceSub.Visible = false;
                //controls

                //Accounting Department TH
                if (Classes.clsUser.department.ToString() == "1" && Classes.clsUser.role.ToString() == "3")
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form.GetType() == typeof(Dashboard.DashboardTH))
                        {
                            form.Activate();
                            return;
                        }
                    }

                    Dashboard.DashboardTH frm = new Dashboard.DashboardTH();
                    frm.timerRefreshAccntg.Enabled = true;
                    frm.Show();
                    frm.MdiParent = this;
                }

                //Audit Department TH
                if (Classes.clsUser.department.ToString() == "3" && Classes.clsUser.role.ToString() == "3")
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form.GetType() == typeof(Dashboard.DashboardTH))
                        {
                            form.Activate();
                            return;
                        }
                    }

                    Dashboard.DashboardTH frm = new Dashboard.DashboardTH();
                    frm.timerAudit.Enabled = true;
                    frm.Show();
                    frm.MdiParent = this;
                }


                //================================================================================
                //                          ACCOUNTING STAFF DASHBOARD
                //================================================================================
                if (Classes.clsUser.department.ToString() == "1" && Classes.clsUser.role.ToString() != "3")
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form.GetType() == typeof(Dashboard.DashboardStaff))
                        {
                            form.Activate();
                            return;
                        }
                    }

                    Dashboard.DashboardStaff frm = new Dashboard.DashboardStaff();
                    frm.timerAccntg.Enabled = true;
                    frm.Show();
                    frm.MdiParent = this;
                }

                //================================================================================
                //                          AUDIT STAFF DASHBOARD
                //================================================================================
                if (Classes.clsUser.department.ToString() == "3" && Classes.clsUser.role.ToString() != "3")
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form.GetType() == typeof(Dashboard.DashboardStaff))
                        {
                            form.Activate();
                            return;
                        }
                    }

                    Dashboard.DashboardStaff frm = new Dashboard.DashboardStaff();
                    frm.timerAudit.Enabled = true;
                    frm.Show();
                    frm.MdiParent = this;
                }
            }
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
            if(clsAccess.checkForViewingRestriction(btnProcess.Text, Classes.clsUser.Username) == true)
            {
                sideVisible(btnProcess);

                panelProcessSub.Visible = true;
                panelProcessSub.Top = btnLoans.Top;
                panelProcessSub.Left = panelMenu.Right;
                panelProcessSub.Height = panelMenu.Bottom;

                //Hide Panels
                panelFileMaintenanceSub.Visible = false;
                panelSavings.Visible = false;
                panelMembership.Visible = false;
                panelReportSub.Visible = false;
                panelMemberSettings.Visible = false;
                panelLoan.Visible = false;
                panelPDCManagement.Visible = false;
            }
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnReports.Text, Classes.clsUser.Username) == true)
            {
                sideVisible(btnReports);
                panelReportSub.Visible = true;
                panelReportSub.Top = btnReports.Bottom;
                panelReportSub.Left = panelMenu.Right;
                panelReportSub.Height = panelMenu.Bottom;

                //Hide Panels
                panelProcessSub.Visible = false;
                panelSavings.Visible = false;
                panelMembership.Visible = false;
                panelMemberSettings.Visible = false;
                panelFileMaintenanceSub.Visible = false;
                panelPDCManagement.Visible = false;
                panelLoan.Visible = false;
            }
            
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if(panelMenu.Width == 341)
            {
                panelMenu.Width = 90;
                
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

                if(panelLoan.Visible ==  true)
                {
                    panelLoan.Left = panelMenu.Right;
                }

                if(panelPDCManagement.Visible == true)
                {
                    panelPDCManagement.Left = panelMenu.Right;
                }

                if(panelReportSub.Visible == true)
                {
                    panelReportSub.Left = panelMenu.Right;
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

                if (panelLoan.Visible == true)
                {
                    panelLoan.Left = panelMenu.Right;
                }

                if (panelPDCManagement.Visible == true)
                {
                    panelPDCManagement.Left = panelMenu.Right;
                }

                if (panelReportSub.Visible == true)
                {
                    panelReportSub.Left = panelMenu.Right;
                }
            }
        }

        private void btnMembership_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnMembership.Text, Classes.clsUser.Username) == true)
            {
                sideVisible(btnMembership);
                panelMembership.Visible = true;
                panelMembership.Top = btnMembership.Bottom;
                panelMembership.Left = panelMenu.Right;
                panelMembership.Height = panelMenu.Bottom;

                //Hide Panels
                panelProcessSub.Visible = false;
                panelSavings.Visible = false;
                panelFileMaintenanceSub.Visible = false;
                panelReportSub.Visible = false;
                panelMemberSettings.Visible = false;
                panelLoan.Visible = false;
                panelPDCManagement.Visible = false;
            }
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
            panelPDCManagement.Visible = false;
        }

        private void btnCompany_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnCompany.Text, Classes.clsUser.Username) == true)
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
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnCostCenter.Text, Classes.clsUser.Username) == true)
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
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnPayrollGroup.Text, Classes.clsUser.Username) == true)
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
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnTransactionType.Text, Classes.clsUser.Username) == true)
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
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnClient.Text, Classes.clsUser.Username) == true)
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
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnChartOfAccount.Text, Classes.clsUser.Username) == true)
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
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnBanks.Text, Classes.clsUser.Username) == true)
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
        }

        private void btnSavings_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnSavings.Text, Classes.clsUser.Username) == true)
            {
                sideVisible(btnSavings);

                panelSavings.Visible = true;
                panelSavings.Top = btnSavings.Bottom;
                panelSavings.Left = panelMenu.Right;
                panelSavings.Height = panelMenu.Bottom;


                //Hide Panels
                panelProcessSub.Visible = false;
                panelFileMaintenanceSub.Visible = false;
                panelMembership.Visible = false;
                panelReportSub.Visible = false;
                panelMemberSettings.Visible = false;
                panelLoan.Visible = false;
                panelPDCManagement.Visible = false;

            }
        }

        private void btnLoans_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnLoans.Text, Classes.clsUser.Username) == true)
            {
                sideVisible(btnLoans);

                panelLoan.Visible = true;
                panelLoan.Top = btnLoans.Bottom;
                panelLoan.Left = panelMenu.Right;
                panelLoan.Height = panelMenu.Bottom;


                //Hide Panels
                panelProcessSub.Visible = false;
                panelSavings.Visible = false;
                panelMembership.Visible = false;
                panelReportSub.Visible = false;
                panelMemberSettings.Visible = false;
                panelFileMaintenanceSub.Visible = false;
                panelPDCManagement.Visible = false;
            }
        }

        private void btnReplenishment_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnReplenishment.Text, Classes.clsUser.Username) == true)
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
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnSavingsWithdrawal.Text, Classes.clsUser.Username) == true)
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
        }

        private void panelSavings_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnParameter_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnParameter.Text, Classes.clsUser.Username) == true)
            {
                sideVisible(btnParameter);


                //Hide Panels
                panelProcessSub.Visible = false;
                panelSavings.Visible = false;
                panelMembership.Visible = false;
                panelReportSub.Visible = false;
                panelMemberSettings.Visible = false;
                panelLoan.Visible = false;
                panelPDCManagement.Visible = false;
                panelFileMaintenanceSub.Visible = false;
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
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction("Membership Data Entry", Classes.clsUser.Username) == true)
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

                MembershipMain frm = new MembershipMain();
                frm.Show();
                frm.MdiParent = this;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnBatchApprove.Text, Classes.clsUser.Username) == true)
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
                panelPDCManagement.Visible = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnJournalVoucher.Text, Classes.clsUser.Username) == true)
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
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnDisbursementVoucher.Text, Classes.clsUser.Username) == true)
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
                //Delete all open transaction before closing the program
                clsOpen.deleteOpenTransaction();

                //Remove Login Information
                clsUser.removeUserLogin();

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
            if(clsAccess.checkForViewingRestriction(btnAtmPrep.Text, Classes.clsUser.Username) == true)
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
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnReceiptVoucher.Text, Classes.clsUser.Username) == true)
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
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnAtmDisk.Text, Classes.clsUser.Username) == true)
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
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnAtmReject.Text, Classes.clsUser.Username) == true)
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
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnSavingsReport.Text, Classes.clsUser.Username) == true)
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
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnHoldAccounts.Text, Classes.clsUser.Username) == true)
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
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnLoanDataEntry.Text, Classes.clsUser.Username) == true)
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
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnLoanType.Text, Classes.clsUser.Username) == true)
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
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnLoanAmort.Text, Classes.clsUser.Username) == true)
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
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnLoanComputation.Text, Classes.clsUser.Username) == true)
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
        }

        private void button25_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnLoanAtm.Text, Classes.clsUser.Username) == true)
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
        }

        private void button26_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnPLBI.Text, Classes.clsUser.Username) == true)
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
        }

        private void button27_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnLoanReport.Text, Classes.clsUser.Username) == true)
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
        }

        private void button28_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnBilling.Text, Classes.clsUser.Username) == true)
            {
                sideVisible(btnBilling);


                //Hide Panels
                panelProcessSub.Visible = false;
                panelSavings.Visible = false;
                panelMembership.Visible = false;
                panelReportSub.Visible = false;
                panelMemberSettings.Visible = false;
                panelLoan.Visible = false;
                panelPDCManagement.Visible = false;

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
        }

        private void button29_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnTransactionReceipt.Text, Classes.clsUser.Username) == true)
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
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnQuery.Text, Classes.clsUser.Username) == true)
            {
                sideVisible(btnQuery);


                //Hide Panels
                panelProcessSub.Visible = false;
                panelSavings.Visible = false;
                panelMembership.Visible = false;
                panelReportSub.Visible = false;
                panelMemberSettings.Visible = false;
                panelLoan.Visible = false;
                panelPDCManagement.Visible = false;

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

        private void button30_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnPdcMngt.Text, Classes.clsUser.Username) == true)
            {
                sideVisible(btnPdcMngt);

                panelPDCManagement.Visible = true;
                panelPDCManagement.Top = btnPdcMngt.Bottom;
                panelPDCManagement.Left = panelMenu.Right;
                panelPDCManagement.Height = panelMenu.Bottom;

                //Hide Panels
                panelFileMaintenanceSub.Visible = false;
                panelSavings.Visible = false;
                panelMembership.Visible = false;
                panelMemberSettings.Visible = false;
                panelReportSub.Visible = false;
                panelLoan.Visible = false;
                panelProcessSub.Visible = false;
            }
        }


        #region TimerNotificationForPDC
    
        private void timer1_Tick(object sender, EventArgs e)
        {
            counter--;
            if (counter == 0)
            {
                if (clsReminder.pdcDue() == true)
                {
                    //REMIND USER AGAIN AFTER THE GIVEN PERIOD OF TIME
                    counter = 3600;
                    timer1.Stop();
                    ReminderPDC.reminder frm = new ReminderPDC.reminder();
                    clsReminder.getCntPDCdueToday(btnNotifPDC, frm.lblSpiel, frm.btnRemid);
                    this.Activate();
                    this.Show();
                    frm.ShowDialog();
                }
                else
                {
                    counter = 0;
                    timer1.Stop();
                }
            }
            else
            {
                if(checkCounter == 0)
                {
                    //STOP 10 seconds and check in database
                    //IF still there then reset again to 10 seconds
                    if (clsReminder.pdcDue() == true)
                    {
                        checkCounter = 5;
                        ReminderPDC.reminder frm = new ReminderPDC.reminder();
                        clsReminder.getCntPDCdueToday(btnNotifPDC, frm.lblSpiel, frm.btnRemid);
                    }
                    else
                    {
                        counter = 0;
                        //NO RECORDS 
                        btnNotifPDC.Visible = false;
                        timer1.Stop();
                    }
                }
                else
                {
                    checkCounter--;
                }
            }
        }

        private void tmerTickIfClickOK_Tick(object sender, EventArgs e)
        {
            if (clsReminder.pdcDue() == true)
            {
                checkCounter = 5;
                ReminderPDC.reminder frm = new ReminderPDC.reminder();
                clsReminder.getCntPDCdueToday(btnNotifPDC, frm.lblSpiel, frm.btnRemid);
            }
            else
            {
                //NO RECORDS 
                btnNotifPDC.Visible = false;
                tmerTickIfClickOK.Stop();
            }
        }

        private void btnNotifPDC_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                var mos = DateTime.Today.Month;
                var yr = DateTime.Today.Year;
                var day = DateTime.Today.Day;

                DateTime dtNowFrom;
                DateTime dtNowTo;


                if (day <= 15)
                {
                    dtNowFrom = new DateTime(yr, mos, 09);
                    dtNowTo = new DateTime(yr, mos, 15);
                }
                else
                {
                    dtNowFrom = new DateTime(yr, mos, 25);
                    dtNowTo = new DateTime(yr, mos, DateTime.DaysInMonth(yr, mos));
                }


                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetChequeDateDueToday";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ChequeDateFrom", dtNowFrom);
                cmd.Parameters.AddWithValue("@ChequeDateTo", dtNowTo);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                PDCManagementV2 frm = new PDCManagementV2();

                foreach (Form form in Application.OpenForms)
                {


                    if (form.GetType() == typeof(PDCManagementV2))
                    {
                        form.Activate();
                        frm = (PDCManagementV2)Application.OpenForms["PDCManagementV2"];
                        frm.dataGridView1.DataSource = dt;
                        frm.str = "";
                        frm.str = "SELECT * FROM vw_PDCManagement ChequeDate between '" + dtNowFrom.ToShortDateString() + "' and '" + dtNowTo.ToShortDateString() + "'";
                        return;
                    }
                }
                frm.str = "";
                frm.str = "SELECT * FROM vw_PDCManagement WHERE ChequeDate between '" + dtNowFrom.ToShortDateString() + "' and '" + dtNowTo.ToShortDateString() + "'";
                frm.Show();
                frm.MdiParent = this;
                frm.dataGridView1.DataSource = dt;
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            panelPDCManagement.Visible = false;
            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(PDCManagementV2))
                {
                    form.Activate();
                    return;
                }
            }

            PDCManagementV2 frm = new PDCManagementV2();
            frm.Show();
            frm.MdiParent = this;
        }

        private void button31_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnLoanListing.Text, Classes.clsUser.Username) == true)
            {
                panelPDCManagement.Visible = false;
                //controls
                foreach (Form form in Application.OpenForms)
                {


                    if (form.GetType() == typeof(PDCLoanListing))
                    {
                        form.Activate();
                        return;
                    }
                }

                PDCLoanListing frm = new PDCLoanListing();
                frm.Show();
                frm.MdiParent = this;
            }
        }

        private void panelMenu_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button32_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            panelMemberSettings.Visible = false;
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Users.UserSettings))
                {
                    form.Activate();
                    return;
                }
            }

            Users.UserSettings frm = new Users.UserSettings();
            frm.Show();
            frm.MdiParent = this;

        }

        private void button34_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnProvisionalReceipt.Text, Classes.clsUser.Username) == true)
            {
                //Hide sub menu filemaintenance
                panelProcessSub.Visible = false;

                //controls
                foreach (Form form in Application.OpenForms)
                {


                    if (form.GetType() == typeof(ProvisionalReceipt))
                    {
                        form.Activate();
                        return;
                    }
                }

                ProvisionalReceipt frm = new ProvisionalReceipt();
                frm.Show();
                frm.MdiParent = this;
            }
        }

        private void btnAccessRight_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForViewingRestriction(btnAccessRight.Text, Classes.clsUser.Username) == true)
            {
                //Hide sub menu filemaintenance
                panelFileMaintenanceSub.Visible = false;

                //controls
                foreach (Form form in Application.OpenForms)
                {


                    if (form.GetType() == typeof(Users.AccessControl))
                    {
                        form.Activate();
                        return;
                    }
                }

                Users.AccessControl frm = new Users.AccessControl();
                frm.Show();
                frm.MdiParent = this;
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {

            if(clsAccess.checkForViewingRestriction(btnUsers.Text, Classes.clsUser.Username) == true)
            {
                panelFileMaintenanceSub.Visible = false;

                //controls
                foreach (Form form in Application.OpenForms)
                {


                    if (form.GetType() == typeof(UsersFileMaintenance))
                    {
                        form.Activate();
                        return;
                    }
                }

                UsersFileMaintenance frm = new UsersFileMaintenance();
                frm.Show();
                frm.MdiParent = this;
            }
           
        }

        private void btnSavingDeduction_Click(object sender, EventArgs e)
        {
            //Hide sub menu filemaintenance
            panelProcessSub.Visible = false;

            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(SavingsDeductionBilling))
                {
                    form.Activate();
                    return;
                }
            }

            SavingsDeductionBilling frm = new SavingsDeductionBilling();
            frm.Show();
            frm.MdiParent = this;
        }
    }
    #endregion
}
