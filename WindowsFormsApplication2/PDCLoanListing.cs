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
    public partial class PDCLoanListing : Form
    {
        public PDCLoanListing()
        {
            InitializeComponent();
        }

        Global global = new Global();

        CrystalDecisions.Shared.TableLogOnInfo li;

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panelHeader_Click(object sender, EventArgs e)
        {
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

        private void panelHeader_MouseMove(object sender, MouseEventArgs e)
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
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();      
                li = new TableLogOnInfo();
                PDCFolder.PDCLoanListing cr = new PDCFolder.PDCLoanListing();

                li.ConnectionInfo.IntegratedSecurity = false;
                

                //PARAMETERS
                cr.SetParameterValue("pdcDate", "PDC FROM : " + dtLoanFrom.Text + " To " + dtLoanTo.Text);
                cr.SetParameterValue("printedBy", Classes.clsUser.Username.ToString());
                cr.SetParameterValue("@loanDateFrom", dtLoanFrom.Text);
                cr.SetParameterValue("@loanDateTo", dtLoanTo.Text);


                //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);
                crystalReportViewer1.ReportSource = cr;
            }
        }
    }
}
