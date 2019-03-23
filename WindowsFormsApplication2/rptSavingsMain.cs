using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class rptSavingsMain : Form
    {
        public rptSavingsMain()
        {
            InitializeComponent();
        }


        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;

        Global global = new Global();

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        public static string wdMode { get; set; }
        public static string searchBy { get; set; }
        public static string status { get; set; }
        public static string criteria { get; set; }

       
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(cmbMode.Text == "")
            {
                Alert.show("Please Select Withdrawal Mode!", Alert.AlertType.error);
                return;
            }

            if(cmbMode.Text == "ALL Mode" && cmbStatus.Text == "ALL Status")
            {
                if(cmbSearchBy.Text == "All Withdrawal")
                {
                    searchBy = "*";
                }
            }
            else
            {
                if (cmbSearchBy.Text == "")
                {
                    Alert.show("Please Select Type of Search you want!", Alert.AlertType.error);
                    return;
                }
            }
            
            if(cmbStatus.Text == "")
            {
                Alert.show("Please Select Status of Withdrawal", Alert.AlertType.error);
                return;
            }

            if(cmbSearchBy.Text != "All Withdrawal" && cmbSearchBy.Text != "")
            {
                if(txtKeyWord.Text == "")
                {
                    Alert.show("Please Put Valid Keyword To Search!", Alert.AlertType.error);
                    return;
                }
            }

            if(Convert.ToDateTime(dtFrom.Text) > Convert.ToDateTime(dtTo.Text))
            {
                Alert.show("Please Put Valid Dates To Search", Alert.AlertType.error);
                return;
            }





            //====================================================================================
            //              GENERATE REPORT
            //====================================================================================

            //ATM REPORT
            CrystalDecisions.Shared.TableLogOnInfo li;

            //Print Purposes
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();


                //=====================================================================================
                //                  STRING QUERY BUILDER
                //=====================================================================================
                string str = "";
                if (searchBy == "*")
                {
                    str = "SELECT * FROM vw_SavingsWithdrawal WHERE Withdrawal_Mode in " + wdMode + " and Status in " + status + " and wdDate Between '" + dtFrom.Text + "' and '" + dtTo.Text + "' ORDER BY Withdrawal_Slip_No ASC";
                }
                else
                {
                    str = "SELECT * FROM vw_SavingsWithdrawal WHERE Withdrawal_Mode in " + wdMode + " and " + searchBy + " like '%" + txtKeyWord.Text + "%' and Status in " + status + " and wdDate Between '" + dtFrom.Text + "' and '" + dtTo.Text + "' ORDER BY Withdrawal_Slip_No ASC";
                }


                SqlDataAdapter adapter = new SqlDataAdapter(str, con);
                DataTable checkDT = new DataTable();
                adapter.Fill(checkDT);

                if (checkDT.Rows.Count == 0)
                {
                    Alert.show("No Record(s) Found!", Alert.AlertType.error);
                    return;
                }

                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                //Generate Report according to MODE
                if (cmbMode.Text == "ALL Mode") //ALL MODE
                {
                    ReportsForms.rptSavingsMainAllMode cr = new ReportsForms.rptSavingsMainAllMode();

                    li = new TableLogOnInfo();

                    li.ConnectionInfo.IntegratedSecurity = false;

                    adapter.Fill(ds, "vw_SavingsWithdrawal");
                    dt = ds.Tables["vw_SavingsWithdrawal"];
                    cr.SetDataSource(ds.Tables["vw_SavingsWithdrawal"]);

                    //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                    cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);


                    cr.SetParameterValue("paramDateFrom", dtFrom.Text);
                    cr.SetParameterValue("paramDateTo", dtTo.Text);


                    cr.SetParameterValue("paramGenerateBy", Classes.clsUser.Username);


                    crystalReportViewer1.ReportSource = cr;
                }
                else if (cmbMode.Text == "ATM - AT")//GENERATE ATM ONLY
                {
                    ReportsForms.rptSavingsMain cr = new ReportsForms.rptSavingsMain();

                    li = new TableLogOnInfo();

                    li.ConnectionInfo.IntegratedSecurity = false;

                    adapter.Fill(ds, "vw_SavingsWithdrawal");
                    dt = ds.Tables["vw_SavingsWithdrawal"];
                    cr.SetDataSource(ds.Tables["vw_SavingsWithdrawal"]);

                    //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                    cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);


                    cr.SetParameterValue("paramDateFrom", dtFrom.Text);
                    cr.SetParameterValue("paramDateTo", dtTo.Text);

                    cr.SetParameterValue("paramGenerateBy", Classes.clsUser.Username);


                    crystalReportViewer1.ReportSource = cr;
                }
                else if (cmbMode.Text == "CASH - CA")//GENERATE CASH ONLY
                {
                    ReportsForms.rptSavingsMainCash cr = new ReportsForms.rptSavingsMainCash();

                    li = new TableLogOnInfo();

                    li.ConnectionInfo.IntegratedSecurity = false;

                    adapter.Fill(ds, "vw_SavingsWithdrawal");
                    dt = ds.Tables["vw_SavingsWithdrawal"];
                    cr.SetDataSource(ds.Tables["vw_SavingsWithdrawal"]);

                    //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                    cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);


                    cr.SetParameterValue("paramDateFrom", dtFrom.Text);
                    cr.SetParameterValue("paramDateTo", dtTo.Text);

                    cr.SetParameterValue("paramGenerateBy", Classes.clsUser.Username);


                    crystalReportViewer1.ReportSource = cr;
                }
                else if (cmbMode.Text == "CHEQUE - CH")
                {
                    ReportsForms.rptSavingsMain_Cheque cr = new ReportsForms.rptSavingsMain_Cheque();

                    li = new TableLogOnInfo();

                    li.ConnectionInfo.IntegratedSecurity = false;

                    adapter.Fill(ds, "vw_SavingsWithdrawal");
                    dt = ds.Tables["vw_SavingsWithdrawal"];
                    cr.SetDataSource(ds.Tables["vw_SavingsWithdrawal"]);

                    //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                    cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);


                    cr.SetParameterValue("paramDateFrom", dtFrom.Text);
                    cr.SetParameterValue("paramDateTo", dtTo.Text);


                    cr.SetParameterValue("paramGenerateBy", Classes.clsUser.Username);


                    crystalReportViewer1.ReportSource = cr;
                }

            }
        }

        private void cmbSearchBy_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbSearchBy.Text == "All Withdrawal")
            {
                searchBy = "*";
                txtKeyWord.Enabled = false;
                txtKeyWord.Text = "";
            }
            else if(cmbSearchBy.Text == "Name of Member")
            {
                searchBy = "Name";
                txtKeyWord.Enabled = true;
                txtKeyWord.Text = "";
            }
            else if (cmbSearchBy.Text == "Prepared By")
            {
                searchBy = "Prepared_By";
                txtKeyWord.Enabled = true;
                txtKeyWord.Text = "";
            }
            else if (cmbSearchBy.Text == "JV Number")
            {
                searchBy = "JV_No";
                txtKeyWord.Enabled = true;
                txtKeyWord.Text = "";
            }
            else if (cmbSearchBy.Text == "CV Number")
            {
                searchBy = "CV_No";
                txtKeyWord.Enabled = true;
                txtKeyWord.Text = "";
            }
            else if (cmbSearchBy.Text == "Posted By")
            {
                searchBy = "Posted_By";
                txtKeyWord.Enabled = true;
                txtKeyWord.Text = "";
            }
            else if (cmbSearchBy.Text == "Cancelled By")
            {
                searchBy = "Cancelled_By";
                txtKeyWord.Enabled = true;
                txtKeyWord.Text = "";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbMode_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbMode.Text == "ALL Mode")
            {
                wdMode = "('AT','CA','CH')";
            }
            else if(cmbMode.Text == "ATM - AT")
            {
                wdMode = "('AT')";
            }
            else if(cmbMode.Text == "CASH - CA")
            {
                wdMode = "('CA')";
            }
            else if (cmbMode.Text == "CHEQUE - CH")
            {
                wdMode = "('CH')";
            }
        }

        private void cmbStatus_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbStatus.Text == "ALL Status")
            {
                status = "('FOR RELEASE','POSTED','CANCELLED')";
            }
            else if(cmbStatus.Text == "For Release")
            {
                status = "('FOR RELEASE')";
            }
            else if(cmbStatus.Text == "Posted")
            {
                status = "('POSTED')";
            }
            else if(cmbStatus.Text == "Cancelled")
            {
                status = "('CANCELLED')";
            }
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rptSavingsMain_Load(object sender, EventArgs e)
        {

        }
    }
}
