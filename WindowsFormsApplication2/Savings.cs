using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Savings : Form
    {
        public Savings()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        bool x;
        //Class
        clsHoverDash cls = new clsHoverDash();
        Classes.clsSavings clsSavings = new Classes.clsSavings();
        Classes.clsSavingsDataEntry clsSavingsDataEntry = new Classes.clsSavingsDataEntry();
        Classes.clsOpenTransaction clsOpen = new Classes.clsOpenTransaction();

        SqlConnection con = new SqlConnection();
        Global global = new Global();
        string mode;
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

        #region HOVER MENUS DASHBOARD
        //Cash Withdrawal View Details
        private void label7_MouseHover(object sender, EventArgs e)
        {
            cls.hoverPanelColor(panel6, label7, Color.FromArgb(60, 141, 188));
            pictureBox8.Image = imageList1.Images[1];
        }

        private void label7_MouseLeave(object sender, EventArgs e)
        {
            cls.leaveHoverPanel(panel6, label7, Color.FromArgb(60, 141, 188));
            pictureBox8.Image = imageList1.Images[4];
        }

        //ATM Withdrawal View Details
        private void label8_MouseHover(object sender, EventArgs e)
        {
            cls.hoverPanelColor(panel12, label8, Color.SeaGreen);
            pictureBox5.Image = imageList1.Images[1];
        }
        private void label8_MouseLeave(object sender, EventArgs e)
        {
            cls.leaveHoverPanel(panel12, label8, Color.SeaGreen);
            pictureBox5.Image = imageList1.Images[0];
        }

        //Cheque Withdrawal View Details
        private void label9_MouseHover(object sender, EventArgs e)
        {
            cls.hoverPanelColor(panel18, label9, Color.FromArgb(240, 173, 78));
            pictureBox7.Image = imageList1.Images[1];
        }
        private void label9_MouseLeave(object sender, EventArgs e)
        {
            cls.leaveHoverPanel(panel18, label9, Color.FromArgb(240, 173, 78));
            pictureBox7.Image = imageList1.Images[3];
        }

        //DATA ENTRY
        private void lblAddNewMember_MouseHover(object sender, EventArgs e)
        {
            cls.hoverPanelColor(panel29, lblAddWithdrawal, Color.FromArgb(217, 83, 79));
            pictureBox6.Image = imageList1.Images[1];
        }

        private void lblAddNewMember_MouseLeave(object sender, EventArgs e)
        {
            cls.leaveHoverPanel(panel29, lblAddWithdrawal, Color.FromArgb(217, 83, 79));
            pictureBox6.Image = imageList1.Images[2];
        }


        #endregion

        private void lblAddWithdrawal_Click(object sender, EventArgs e)
        {
            SavingsDataEntry savingsdataentry = new SavingsDataEntry();
            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(SavingsDataEntry))
                {
                    form.Activate();
                    savingsdataentry.btnSave.Text = "SAVE";
                    savingsdataentry.btnSearch.Enabled = true;
                    savingsdataentry.txtNote.ReadOnly = true;
                    return;
                }
            }
            savingsdataentry.btnSave.Text = "SAVE";
            savingsdataentry.btnSearch.Enabled = true;
            savingsdataentry.txtNote.ReadOnly = true;
            savingsdataentry.Show();
        }

        private void Savings_Load(object sender, EventArgs e)
        {
            refreshData();
        }
        public void refreshData()
        {
            //Load DataGridView
            clsSavings.loadSavings(dataGridView1);
            //Load Dashboard 
            clsSavings.loadDash(lblATM, lblCheque, lblCash);
            clsSavings.loadDashReleaseCancel(lblCashRelease, lblChequeRelease, lblCancelTotal);
            
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {           
            cmbMode.SelectedIndex = -1;
            txtWithdrawalSlipNo.Text = "";
            dtWithdrawalDate.Value = DateTime.Today;

            //Load Dashboard 
            refreshData();
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(txtWithdrawalSlipNo.Text != "" || cmbMode.Text != "")
            {
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    if (cmbMode.Text == "ATM - AT")
                    {
                        mode = "AT";
                    }
                    else if (cmbMode.Text == "CASH - CA")
                    {
                        mode = "CA";
                    }
                    else if (cmbMode.Text == "CHEQUE - CH")
                    {
                        mode = "CH";
                    }


                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_SavingsWithdrawalDaily WHERE Withdrawal_Slip_No like '%" + txtWithdrawalSlipNo.Text + "%' and wdDate = '" + dtWithdrawalDate.Value.ToString("yyyy-MM-dd") + "' and Withdrawal_Mode like '%" + mode + "%'", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        //No Records Found!
                        Alert.show("No record/s found.", Alert.AlertType.warning);
                    }

                    int colCnt = dt.Columns.Count;
                    int x = 0;


                    while (x != colCnt)
                    {
                        dataGridView1.Columns[x].Visible = false;
                        x = x + 1;
                    }

                    dataGridView1.Columns["wdDate"].Visible = true;
                    dataGridView1.Columns["wdDate"].HeaderText = "Date";
                    dataGridView1.Columns["wdDate"].FillWeight = 50;

                    dataGridView1.Columns["Name"].Visible = true;
                    dataGridView1.Columns["Name"].HeaderText = "Name";
                    dataGridView1.Columns["Name"].FillWeight = 140;

                    dataGridView1.Columns["Withdrawal_Slip_No"].Visible = true;
                    dataGridView1.Columns["Withdrawal_Slip_No"].HeaderText = "Withdrawal Slip No";

                    dataGridView1.Columns["Withdrawal_Mode"].Visible = true;
                    dataGridView1.Columns["Withdrawal_Mode"].HeaderText = "Mode";
                    dataGridView1.Columns["Withdrawal_Mode"].FillWeight = 50;

                    dataGridView1.Columns["Posted"].Visible = true;
                    dataGridView1.Columns["Posted"].HeaderText = "Posted";
                    dataGridView1.Columns["Posted"].FillWeight = 60;

                    dataGridView1.Columns["Cancelled"].Visible = true;
                    dataGridView1.Columns["Cancelled"].HeaderText = "Cancelled";
                    dataGridView1.Columns["Cancelled"].FillWeight = 60;

                    dataGridView1.Columns["ReleaseDate"].Visible = true;
                    dataGridView1.Columns["ReleaseDate"].HeaderText = "Release Date";
                    dataGridView1.Columns["ReleaseDate"].FillWeight = 80;

                    dataGridView1.Columns["Prepared_By"].Visible = true;
                    dataGridView1.Columns["Prepared_By"].HeaderText = "Prepared By";
                    dataGridView1.Columns["Prepared_By"].FillWeight = 80;
                }
            }
            else
            {
                Alert.show("No keywords to be search!", Alert.AlertType.warning);
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                //No Data to be edit
                Alert.show("Please select savings you want to edit!", Alert.AlertType.warning);
                return;
            }
            //===================================================================================
            //                      CHECK FIRST IF THIS IS OPEN TO OTHERS
            //===================================================================================

            //this is for Status : FOR RELEASE ONLY
            if(dataGridView1.SelectedRows[0].Cells["Posted"].Value.ToString() == "False" && dataGridView1.SelectedRows[0].Cells["Cancelled"].Value.ToString() == "False" && dataGridView1.SelectedRows[0].Cells["ReleaseDate"].Value.ToString() == string.Empty)
            {
                //if method of withdrawal is ATM this regard
                if (dataGridView1.SelectedRows[0].Cells["Withdrawal_Mode"].Value.ToString() != "AT")
                {
                    //Check if open
                    if (clsOpen.checkOpenFormsAndTransaction("Savings", dataGridView1.SelectedRows[0].Cells["Withdrawal_Slip_No"].Value.ToString()) == true)
                    {
                        //Messagebox here for open form with user whos using the form and reference
                        Alert.show(clsOpen.returnUserOnlineAndReference("Savings", dataGridView1.SelectedRows[0].Cells["Withdrawal_Slip_No"].Value.ToString(), "Withdrawal"), Alert.AlertType.error);
                        return;
                    }
                    else
                    {
                        //Insert here for register the open form and reference
                        clsOpen.insertTransaction("Savings", dataGridView1.SelectedRows[0].Cells["Withdrawal_Slip_No"].Value.ToString());
                    }
                }
            }
            

            //show form
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(SavingsDataEntry))
                {
                    form.Activate();
                    x = true;
                }
                else
                {
                    x = false;
                }
            }

            SavingsDataEntry savingsEntry = new SavingsDataEntry();
            
            if (x != true)
            {
                savingsEntry.Show();
            }

            

            savingsEntry = (SavingsDataEntry)Application.OpenForms["SavingsDataEntry"];
            savingsEntry.btnSave.Text = "UPDATE";
            savingsEntry.btnSearch.Enabled = false;
            Classes.clsSavingsDataEntry.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
            savingsEntry.txtWithdrawalSlipNo.Text = dataGridView1.SelectedRows[0].Cells["Withdrawal_Slip_No"].Value.ToString();
            savingsEntry.txtEmployeeID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
            savingsEntry.txtName.Text = dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString();
            savingsEntry.txtCurrentBalanceBeforeWithdrawal.Text = clsSavingsDataEntry.returnMembersSaving(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
            savingsEntry.txtCompany.Text = clsSavings.returnCompanyDescription(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
            
            //Put Members Bank and Account No Using ID
            clsSavings.returnBankAtm(savingsEntry.txtBankCode, savingsEntry.txtAccntNo, Classes.clsSavingsDataEntry.userID);


            //Disable Withdrawal From and Method
            savingsEntry.FieldControls(false, savingsEntry.radioPerea, savingsEntry.radioDansalan, savingsEntry.radioCash, savingsEntry.radioATM, savingsEntry.radioCheque, savingsEntry.txtAmountWithdrawn, savingsEntry.cmbBankName, savingsEntry.txtCheque);

            //Amount 
            if (dataGridView1.SelectedRows[0].Cells["AmtWithdrawn"].Value.ToString() != "")
            {
                savingsEntry.txtAmountWithdrawn.Text = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["AmtWithdrawn"].Value).ToString("#,0.00");
            }
            else
            {
                savingsEntry.txtAmountWithdrawn.Text = dataGridView1.SelectedRows[0].Cells["AmtWithdrawn"].Value.ToString();
            }



          




            savingsEntry.dtDateWithdrawal.Text = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["wdDate"].Value).ToShortDateString();

            //Cancel Note and By
            savingsEntry.txtCancelledBy.Text = dataGridView1.SelectedRows[0].Cells["Cancelled_By"].Value.ToString();
            savingsEntry.txtNote.Text = dataGridView1.SelectedRows[0].Cells["Cancelled_Note"].Value.ToString();
            
            //Posted by
            savingsEntry.txtPostedBy.Text = dataGridView1.SelectedRows[0].Cells["Posted_By"].Value.ToString();
            //Prepared 
            savingsEntry.txtPreparedBy.Text = dataGridView1.SelectedRows[0].Cells["Prepared_By"].Value.ToString();


            //Select Method

            if (dataGridView1.SelectedRows[0].Cells["Withdrawal_Mode"].Value.ToString() == "CA")
            {
                savingsEntry.radioCash.Checked = true;
            }
            else if (dataGridView1.SelectedRows[0].Cells["Withdrawal_Mode"].Value.ToString() == "CH")
            {
                savingsEntry.radioCheque.Checked = true;
                //For CHEQUE DETAILS
                if (dataGridView1.SelectedRows[0].Cells["Bank_Code"].Value.ToString() != "")
                {
                    savingsEntry.cmbBankName.Text = clsSavings.returnBankDescription(dataGridView1.SelectedRows[0].Cells["Bank_Code"].Value.ToString());
                    savingsEntry.txtCheque.Text = dataGridView1.SelectedRows[0].Cells["Check_No"].Value.ToString();
                }
            }
            else if (dataGridView1.SelectedRows[0].Cells["Withdrawal_Mode"].Value.ToString() == "AT")
            {
                savingsEntry.radioATM.Checked = true;
                savingsEntry.btnRelease.Enabled = false;
            }

            //STATUS INFORMATION
            if (dataGridView1.SelectedRows[0].Cells["ReleaseDate"].Value.ToString() != null && dataGridView1.SelectedRows[0].Cells["ReleaseDate"].Value.ToString() != string.Empty && dataGridView1.SelectedRows[0].Cells["Cancelled"].Value.ToString() != "True" && dataGridView1.SelectedRows[0].Cells["Posted"].Value.ToString() != "True")
            {
                savingsEntry.dtReleaseDate.Text = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["ReleaseDate"].Value).ToShortDateString();

                savingsEntry.status.Text = "RELEASED";
                savingsEntry.status.Visible = true;

                //Enable Cancell Note
                savingsEntry.txtNote.ReadOnly = false;

                //Button to NEW
                savingsEntry.btnSave.Text = "NEW";
            }
            else if(dataGridView1.SelectedRows[0].Cells["Cancelled"].Value.ToString() == "True")
            {
                if(dataGridView1.SelectedRows[0].Cells["ReleaseDate"].Value == DBNull.Value)
                {
                    savingsEntry.dtReleaseDate.Text = "";
                }
                savingsEntry.status.Text = "CANCELLED";
                savingsEntry.status.Visible = true;

                //Enable Cancell Note
                savingsEntry.txtNote.ReadOnly = true;

                //Button to NEW
                savingsEntry.btnSave.Text = "NEW";
            }
            else if(dataGridView1.SelectedRows[0].Cells["Posted"].Value.ToString() == "True")
            {
                if (dataGridView1.SelectedRows[0].Cells["ReleaseDate"].Value == DBNull.Value)
                {
                    savingsEntry.dtReleaseDate.Text = "";
                }
                else
                {
                    savingsEntry.dtReleaseDate.Text = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["ReleaseDate"].Value).ToShortDateString();
                }

                //SET THE CV NUMBER IF CHEQUE
                if(savingsEntry.radioCheque.Checked == true)
                {
                    //IF TRUE THEN
                    //CHECK IF THERES ALREADY A VOUCHER
                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();
                        SqlDataAdapter adapterchck = new SqlDataAdapter("SELECT CV_No FROM Withdrawal_Slip WHERE Withdrawal_Slip_No = '" + dataGridView1.SelectedRows[0].Cells["Withdrawal_Slip_No"].Value.ToString() + "'", con);
                        DataTable dtchck = new DataTable();
                        adapterchck.Fill(dtchck);

                        savingsEntry.status.Text = "POSTED - CV#" + dtchck.Rows[0].ItemArray[0].ToString();
                        savingsEntry.btnRelease.Enabled = false;
                    }
                }
                else
                {
                    savingsEntry.status.Text = "POSTED";
                }

                savingsEntry.status.Visible = true;

                //Enable Cancell Note
                savingsEntry.txtNote.ReadOnly = true;

                //Button to NEW
                savingsEntry.btnSave.Text = "NEW";
            }
            else
            {
                savingsEntry.dtReleaseDate.Text = "";
                savingsEntry.status.Visible = true;

                //CHECK IF CHEQUE WITHDRAWAL MODE
                if(dataGridView1.SelectedRows[0].Cells["Withdrawal_Mode"].Value.ToString() == "CH")
                {
                    //IF TRUE THEN
                    //CHECK IF THERES ALREADY A VOUCHER
                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();

                        SqlDataAdapter adapterchck = new SqlDataAdapter("SELECT CV_No FROM Withdrawal_Slip WHERE Withdrawal_Slip_No = '" + dataGridView1.SelectedRows[0].Cells["Withdrawal_Slip_No"].Value.ToString() + "'", con);
                        DataTable dtchck = new DataTable();
                        adapterchck.Fill(dtchck);

                        if (dtchck.Rows[0].ItemArray[0].ToString() == "")
                        {
                            savingsEntry.status.Text = "FOR CHEQUE PREPARATION";
                            savingsEntry.btnRelease.Enabled = true;
                        }
                        else
                        {
                            savingsEntry.status.Text = "FOR RELEASE - CV#" + dtchck.Rows[0].ItemArray[0].ToString();
                            savingsEntry.btnSave.Text = "NEW";
                            savingsEntry.btnRelease.Enabled = false;
                        }
                    }                        
                }
                else
                {
                    savingsEntry.status.Text = "FOR RELEASE";

                    if(dataGridView1.SelectedRows[0].Cells["Withdrawal_Mode"].Value.ToString() == "AT")
                    {
                        savingsEntry.btnRelease.Enabled = false;
                    }
                    else
                    {
                        savingsEntry.btnRelease.Enabled = true;
                    }
                   
                }
                
                //Enable Cancell Note
                savingsEntry.txtNote.ReadOnly = false;
                savingsEntry.txtAmountWithdrawn.Enabled = true;
            }

            savingsEntry.txtSDDeduction.Text = clsSavings.returnSDMonthly(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));

            if (dataGridView1.SelectedRows[0].Cells["WDFrom"].Value.ToString() == "1" || dataGridView1.SelectedRows[0].Cells["WDFrom"].Value.ToString() == "True")
            {
                //PEREA
                savingsEntry.radioPerea.Checked = true;
            }
            else
            {
                savingsEntry.radioDansalan.Checked = true;
            }

           

            //Check for last withdrawal Date
            savingsEntry.txtLastDateWithdrawal.Text = clsSavings.returnLastWithdrawalDate(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
            //if theres a deposited check
            savingsEntry.txtdepositedCheque.Text = clsSavings.returnDepositedChequeAmount(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
            savingsEntry.txtDepositedDate.Text = clsSavings.returnDepositedChequeDate(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));


            //MINUS THE CHECK AMOUNT
            if (savingsEntry.txtdepositedCheque.Text != "")
            {
                Decimal widAmnt;
                widAmnt = Convert.ToDecimal(savingsEntry.txtCurrentBalanceBeforeWithdrawal.Text) - Convert.ToDecimal(savingsEntry.txtdepositedCheque.Text);
                savingsEntry.txtWithdrawalBalance.Text = widAmnt.ToString("#,0.00");
            }
            else
            {
                savingsEntry.txtWithdrawalBalance.Text = clsSavingsDataEntry.returnMembersSaving(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
            }

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button2_Click(sender, e);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            //CASH REPORT
            CrystalDecisions.Shared.TableLogOnInfo li;

            //Print Purposes
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_SavingsReportDailyCA", con);



                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                ReportsForms.rptCashDaily cr = new ReportsForms.rptCashDaily();
                ReportsForms.rptSavingsWithdrawalDaily rpt = new ReportsForms.rptSavingsWithdrawalDaily();

                li = new TableLogOnInfo();

                li.ConnectionInfo.IntegratedSecurity = false;

                adapter.Fill(ds, "vw_SavingsReportDailyCA");
                dt = ds.Tables["vw_SavingsReportDailyCA"];
                cr.SetDataSource(ds.Tables["vw_SavingsReportDailyCA"]);

                //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

                rpt.crystalReportViewer1.ReportSource = cr;
                rpt.crystalReportViewer1.RefreshReport();
                rpt.ShowDialog();
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            //ATM REPORT
            CrystalDecisions.Shared.TableLogOnInfo li;

            //Print Purposes
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_ATMReportDaily", con);


                //Set Param

                //==================================================================================
                string str = "select Distinct(Bank_Code) as Bank_Code, Sum(AmtWithdrawn), JV_No FROM vw_ATMReportDaily WHERE Cancelled <> 1 GROUP BY Bank_Code, JV_No ORDER BY Bank_Code ASC";
                SqlDataAdapter adapterParam = new SqlDataAdapter(str, con);
                DataTable dtParam = new DataTable();
                adapterParam.Fill(dtParam);




                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                ReportsForms.rptATMDaily cr = new ReportsForms.rptATMDaily();
                ReportsForms.rptSavingsWithdrawalDaily rpt = new ReportsForms.rptSavingsWithdrawalDaily();


                li = new TableLogOnInfo();

                li.ConnectionInfo.IntegratedSecurity = false;

                adapter.Fill(ds, "vw_ATMReportDaily");
                dt = ds.Tables["vw_ATMReportDaily"];
                cr.SetDataSource(ds.Tables["vw_ATMReportDaily"]);

                //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);


                //==================================================================================================
                //                      HARDCODE PARAM FOR ATM LISTING DAILY
                //==================================================================================================
                if (dtParam.Rows.Count == 0)
                {
                    cr.SetParameterValue("param1", "");
                    cr.SetParameterValue("param2", "");
                    cr.SetParameterValue("param3", "");
                    cr.SetParameterValue("param4", "");
                    cr.SetParameterValue("param5", "");
                    cr.SetParameterValue("param6", "");
                    cr.SetParameterValue("param7", "");
                    cr.SetParameterValue("param8", "");
                    cr.SetParameterValue("param9", "");
                }

                if (dtParam.Rows.Count == 1)
                {
                    cr.SetParameterValue("param1", dtParam.Rows[0].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[0].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[0].ItemArray[2].ToString());
                    cr.SetParameterValue("param2", "");
                    cr.SetParameterValue("param3", "");
                    cr.SetParameterValue("param4", "");
                    cr.SetParameterValue("param5", "");
                    cr.SetParameterValue("param6", "");
                    cr.SetParameterValue("param7", "");
                    cr.SetParameterValue("param8", "");
                    cr.SetParameterValue("param9", "");
                }
                else if (dtParam.Rows.Count == 2)
                {
                    cr.SetParameterValue("param1", dtParam.Rows[0].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[0].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[0].ItemArray[2].ToString());
                    cr.SetParameterValue("param2", dtParam.Rows[1].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[1].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[1].ItemArray[2].ToString());
                    cr.SetParameterValue("param3", "");
                    cr.SetParameterValue("param4", "");
                    cr.SetParameterValue("param5", "");
                    cr.SetParameterValue("param6", "");
                    cr.SetParameterValue("param7", "");
                    cr.SetParameterValue("param8", "");
                    cr.SetParameterValue("param9", "");
                }
                else if (dtParam.Rows.Count == 3)
                {
                    cr.SetParameterValue("param1", dtParam.Rows[0].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[0].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[0].ItemArray[2].ToString());
                    cr.SetParameterValue("param2", dtParam.Rows[1].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[1].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[1].ItemArray[2].ToString());
                    cr.SetParameterValue("param3", dtParam.Rows[2].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[2].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[2].ItemArray[2].ToString());
                    cr.SetParameterValue("param4", "");
                    cr.SetParameterValue("param5", "");
                    cr.SetParameterValue("param6", "");
                    cr.SetParameterValue("param7", "");
                    cr.SetParameterValue("param8", "");
                    cr.SetParameterValue("param9", "");
                }
                else if (dtParam.Rows.Count == 4)
                {
                    cr.SetParameterValue("param1", dtParam.Rows[0].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[0].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[0].ItemArray[2].ToString());
                    cr.SetParameterValue("param2", dtParam.Rows[1].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[1].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[1].ItemArray[2].ToString());
                    cr.SetParameterValue("param3", dtParam.Rows[2].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[2].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[2].ItemArray[2].ToString());
                    cr.SetParameterValue("param4", dtParam.Rows[3].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[3].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[3].ItemArray[2].ToString());
                    cr.SetParameterValue("param5", "");
                    cr.SetParameterValue("param6", "");
                    cr.SetParameterValue("param7", "");
                    cr.SetParameterValue("param8", "");
                    cr.SetParameterValue("param9", "");
                }
                else if (dtParam.Rows.Count == 5)
                {
                    cr.SetParameterValue("param1", dtParam.Rows[0].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[0].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[0].ItemArray[2].ToString());
                    cr.SetParameterValue("param2", dtParam.Rows[1].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[1].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[1].ItemArray[2].ToString());
                    cr.SetParameterValue("param3", dtParam.Rows[2].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[2].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[2].ItemArray[2].ToString());
                    cr.SetParameterValue("param4", dtParam.Rows[3].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[3].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[3].ItemArray[2].ToString());
                    cr.SetParameterValue("param5", dtParam.Rows[4].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[4].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[4].ItemArray[2].ToString());
                    cr.SetParameterValue("param6", "");
                    cr.SetParameterValue("param7", "");
                    cr.SetParameterValue("param8", "");
                    cr.SetParameterValue("param9", "");
                }
                else if (dtParam.Rows.Count == 6)
                {
                    cr.SetParameterValue("param1", dtParam.Rows[0].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[0].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[0].ItemArray[2].ToString());
                    cr.SetParameterValue("param2", dtParam.Rows[1].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[1].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[1].ItemArray[2].ToString());
                    cr.SetParameterValue("param3", dtParam.Rows[2].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[2].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[2].ItemArray[2].ToString());
                    cr.SetParameterValue("param4", dtParam.Rows[3].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[3].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[3].ItemArray[2].ToString());
                    cr.SetParameterValue("param5", dtParam.Rows[4].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[4].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[4].ItemArray[2].ToString());
                    cr.SetParameterValue("param6", dtParam.Rows[5].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[5].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[5].ItemArray[2].ToString());
                    cr.SetParameterValue("param7", "");
                    cr.SetParameterValue("param8", "");
                    cr.SetParameterValue("param9", "");
                }
                else if (dtParam.Rows.Count == 7)
                {
                    cr.SetParameterValue("param1", dtParam.Rows[0].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[0].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[0].ItemArray[2].ToString());
                    cr.SetParameterValue("param2", dtParam.Rows[1].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[1].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[1].ItemArray[2].ToString());
                    cr.SetParameterValue("param3", dtParam.Rows[2].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[2].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[2].ItemArray[2].ToString());
                    cr.SetParameterValue("param4", dtParam.Rows[3].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[3].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[3].ItemArray[2].ToString());
                    cr.SetParameterValue("param5", dtParam.Rows[4].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[4].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[4].ItemArray[2].ToString());
                    cr.SetParameterValue("param6", dtParam.Rows[5].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[5].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[5].ItemArray[2].ToString());
                    cr.SetParameterValue("param7", dtParam.Rows[6].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[6].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[6].ItemArray[2].ToString());
                    cr.SetParameterValue("param8", "");
                    cr.SetParameterValue("param9", "");
                }
                else if (dtParam.Rows.Count == 8)
                {
                    cr.SetParameterValue("param1", dtParam.Rows[0].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[0].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[0].ItemArray[2].ToString());
                    cr.SetParameterValue("param2", dtParam.Rows[1].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[1].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[1].ItemArray[2].ToString());
                    cr.SetParameterValue("param3", dtParam.Rows[2].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[2].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[2].ItemArray[2].ToString());
                    cr.SetParameterValue("param4", dtParam.Rows[3].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[3].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[3].ItemArray[2].ToString());
                    cr.SetParameterValue("param5", dtParam.Rows[4].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[4].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[4].ItemArray[2].ToString());
                    cr.SetParameterValue("param6", dtParam.Rows[5].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[5].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[5].ItemArray[2].ToString());
                    cr.SetParameterValue("param7", dtParam.Rows[6].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[6].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[6].ItemArray[2].ToString());
                    cr.SetParameterValue("param8", dtParam.Rows[7].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[7].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[7].ItemArray[2].ToString());
                    cr.SetParameterValue("param9", "");
                }
                else if (dtParam.Rows.Count == 9)
                {
                    cr.SetParameterValue("param1", dtParam.Rows[0].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[0].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[0].ItemArray[2].ToString());
                    cr.SetParameterValue("param2", dtParam.Rows[1].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[1].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[1].ItemArray[2].ToString());
                    cr.SetParameterValue("param3", dtParam.Rows[2].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[2].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[2].ItemArray[2].ToString());
                    cr.SetParameterValue("param4", dtParam.Rows[3].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[3].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[3].ItemArray[2].ToString());
                    cr.SetParameterValue("param5", dtParam.Rows[4].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[4].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[4].ItemArray[2].ToString());
                    cr.SetParameterValue("param6", dtParam.Rows[5].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[5].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[5].ItemArray[2].ToString());
                    cr.SetParameterValue("param7", dtParam.Rows[6].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[6].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[6].ItemArray[2].ToString());
                    cr.SetParameterValue("param8", dtParam.Rows[7].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[7].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[7].ItemArray[2].ToString());
                    cr.SetParameterValue("param9", dtParam.Rows[8].ItemArray[0].ToString() + " - " + Convert.ToDecimal(dtParam.Rows[8].ItemArray[1].ToString()).ToString("#,0.00") + " : JV#" + dtParam.Rows[8].ItemArray[2].ToString());
                }
                //==================================================================================================
                //                      END HARDCODE PARAM
                //==================================================================================================

                rpt.crystalReportViewer1.ReportSource = cr;
                rpt.ShowDialog();
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {
            //ATM REPORT
            CrystalDecisions.Shared.TableLogOnInfo li;

            //Print Purposes
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_SavingsReportDailyCheck", con);



                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                ReportsForms.rptCheckDaily cr = new ReportsForms.rptCheckDaily();
                ReportsForms.rptSavingsWithdrawalDaily rpt = new ReportsForms.rptSavingsWithdrawalDaily();

                li = new TableLogOnInfo();

                li.ConnectionInfo.IntegratedSecurity = false;

                adapter.Fill(ds, "vw_SavingsReportDailyCheck");
                dt = ds.Tables["vw_SavingsReportDailyCheck"];
                cr.SetDataSource(ds.Tables["vw_SavingsReportDailyCheck"]);

                //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

                rpt.crystalReportViewer1.ReportSource = cr;
                rpt.crystalReportViewer1.RefreshReport();
                rpt.ShowDialog();
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Savings))
                {
                    form.Activate();
                    return;
                }
            }

            Savings frm = new Savings();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
