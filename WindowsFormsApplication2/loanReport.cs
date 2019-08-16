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

namespace WindowsFormsApplication2
{
    public partial class loanReport : Form
    {
        public loanReport()
        {
            InitializeComponent();
        }

        public static string status { get; set; }

        Global global = new Global();
        Classes.clsLoanReport clsLoanReport = new Classes.clsLoanReport();

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        string str;
        private void loanReport_Load(object sender, EventArgs e)
        {
            clsLoanReport.loadComboBox(cmbLoanType, "sp_ReturnLoanTypesForReport",2);
            clsLoanReport.loadComboBox(cmbStatus, "sp_ReturnLoanStatusForReport", 0);
            clsLoanReport.loadComboBox(cmbCompany, "sp_ReturnLoanCompanyForReport", 0);
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //====================================================================================
            //              GENERATE REPORT
            //====================================================================================

            //ATM REPORT
            CrystalDecisions.Shared.TableLogOnInfo li;

            //Print Purposes
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //SET VALUES FIRST
                string type, comp, stat;

                if (cmbLoanType.SelectedValue.ToString() == "ALL")
                {
                    type = "";
                }
                else
                {
                    type = cmbLoanType.SelectedValue.ToString();
                }

                if (cmbCompany.SelectedValue.ToString() == "ALL COMPANY")
                {
                    comp = "";

                }
                else
                {
                    comp = cmbCompany.SelectedValue.ToString();
                }

                if (cmbStatus.SelectedValue.ToString() == "ALL STATUS")
                {
                    status = "('FOR APPROVAL','APPROVED','DISAPPROVED','RELEASED','FBA','CANCELLED','FOR RELEASE','FOR POSTING')";
                }
                else
                {
                    status = "('" + cmbStatus.SelectedValue.ToString() + "')";
                }

                //=====================================================================================
                //                  STRING QUERY BUILDER
                //=====================================================================================


                str = "select * From vw_LoanReport WHERE Loan_No Like '%" + txtLoanNo.Text + "%' and Loan_Type Like '%" + type + "%' and Company Like '%" + comp + "%' and Status in " + status + " and Name like '%" + txtName.Text + "%' and [Encoded By] like '%" + txtEncodedBy.Text + "%' and [Date Encoded] Between '" + dtFrom.Text + "' and '" + dtTo.Text + "'";


                SqlDataAdapter adapter = new SqlDataAdapter(str, con);
                DataTable checkDT = new DataTable();
                adapter.Fill(checkDT);

                if (checkDT.Rows.Count == 0)
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }

                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                ReportsForms.loanReport cr = new ReportsForms.loanReport();

                li = new TableLogOnInfo();

                li.ConnectionInfo.IntegratedSecurity = false;

                adapter.Fill(ds, "vw_LoanReport");
                dt = ds.Tables["vw_LoanReport"];
                cr.SetDataSource(ds.Tables["vw_LoanReport"]);

                //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

                //Setup Parameters
                cr.SetParameterValue("from", dtFrom.Text);
                cr.SetParameterValue("to", dtTo.Text);
                cr.SetParameterValue("PrintedBy", Classes.clsUser.Username);

                cr.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;

                crystalReportViewer1.ReportSource = cr;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbStatus_SelectedValueChanged(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtLoanNo.Text = "";
            cmbLoanType.SelectedIndex = 0;
            cmbCompany.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;
            txtName.Text = "";
            txtEncodedBy.Text = "";
        }
    }
}
