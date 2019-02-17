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

namespace WindowsFormsApplication2.ReportsForms
{
    public partial class rptPLBI : Form
    {
        public rptPLBI()
        {
            InitializeComponent();
        }

        Global global = new Global();
        Classes.clsPLBI clsPLBI = new Classes.clsPLBI();
        SqlConnection con;
        
        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        CrystalDecisions.Shared.TableLogOnInfo li;
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

        private void rptPLBI_Load(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(txtLoanNo.Text == "")
            {
                Alert.show("Loan No is required!", Alert.AlertType.error);
                return;
            }

            //Check if the loan no is in database
            if(clsPLBI.checkLoanNo(txtLoanNo.Text) != true)
            {
                Alert.show("Loan Number not found!", Alert.AlertType.error);
                return;
            }

            con = new SqlConnection();
            global.connection(con);

            CrystalDecisions.Shared.TableLogOnInfo li;

            string str = "SELECT * FROM vw_CoMakers WHERE Loan_No = '" + txtLoanNo.Text + "'";
            //=======================================================
            //                  for sub report
            //=======================================================

            SqlDataAdapter subreportAdapter = new SqlDataAdapter(str, con);
            DataTable dtsubreport = new DataTable();
            DataSet dsSubreport = new DataSet();


            DataSet ds = new DataSet();
            //=============================================================
            ReportsForms.PLBI cr = new ReportsForms.PLBI();

            li = new TableLogOnInfo();

            li.ConnectionInfo.IntegratedSecurity = false;


            //=========================================
            //          sub report
            //=========================================
            subreportAdapter.Fill(dsSubreport, "vw_CoMakers");
            dtsubreport = dsSubreport.Tables["vw_CoMakers"];
            cr.Subreports[0].SetDataSource(dsSubreport.Tables["vw_CoMakers"]);

            //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
            cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);


            //PARAMETERS
            cr.SetParameterValue("employeeID", clsPLBI.empid(txtLoanNo.Text));
            cr.SetParameterValue("name", clsPLBI.namewithcompany(txtLoanNo.Text));
            cr.SetParameterValue("loanNoType", clsPLBI.loanNoType(txtLoanNo.Text));
            cr.SetParameterValue("membershipDate", clsPLBI.memDate(txtLoanNo.Text));
            cr.SetParameterValue("pmsDate", clsPLBI.pmsDate(txtLoanNo.Text));
            cr.SetParameterValue("sc", clsPLBI.shareCapital(txtLoanNo.Text));
            cr.SetParameterValue("sd", clsPLBI.savings(txtLoanNo.Text));
            cr.SetParameterValue("sc_perday", clsPLBI.scPerDay(txtLoanNo.Text));
            cr.SetParameterValue("sd_perday", clsPLBI.SavingsPerday(txtLoanNo.Text));
            cr.SetParameterValue("BANK", clsPLBI.Bank(txtLoanNo.Text));
            cr.SetParameterValue("bank_accnt", clsPLBI.Atm(txtLoanNo.Text));
            cr.SetParameterValue("rate", "A");
            cr.SetParameterValue("collection%", "100");
            cr.SetParameterValue("BillingFor6Mons", "");
            cr.SetParameterValue("BillingFor2Mons", "");
            cr.SetParameterValue("CollectionFor6Mons", "");
            cr.SetParameterValue("collectionlast2", "");
            cr.SetParameterValue("printedBy", Classes.clsUser.Username.ToString());
            cr.SetParameterValue("sc_date", "10/15/2017");
            cr.SetParameterValue("sd_date", "10/15/2017");
            cr.SetParameterValue("@userid", clsPLBI.userid(txtLoanNo.Text));


            crystalReportViewer1.ReportSource = cr;
        }
    }
}
