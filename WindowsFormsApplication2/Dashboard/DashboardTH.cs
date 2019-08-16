using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Dashboard
{
    public partial class DashboardTH : Form
    {
        public DashboardTH()
        {
            InitializeComponent();

            if (Classes.clsUser.department.ToString() == "1" && Classes.clsUser.role.ToString() == "3")
            {
                //For Accounting
                clsDashboard.WorkListCount(lblWorkCnt);
                clsDashboard.teamsWorkListCount(lblTeamCnt);
                clsDashboard.IncomingWork(lblIncoming);
                clsDashboard.DailyProductivity(lblDailyCnt);
            }

            if (Classes.clsUser.department.ToString() == "3" && Classes.clsUser.role.ToString() == "3")
            {
                //For Audit
                clsDashboard.workListAudit(lblWorkCnt);
                clsDashboard.teamsWorkListCountAudit(lblTeamCnt);
                lblIncoming.Text = "0";
                clsDashboard.DailyProductivityAudit(lblDailyCnt);
            }


        }

        DashboardClasses.clsDashboardTH clsDashboard = new DashboardClasses.clsDashboardTH(); 

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void myWorkList_Click(object sender, EventArgs e)
        {
            worklist2.BringToFront();
        }

        private void myTeamWorklist_Click(object sender, EventArgs e)
        {
            teamsWorklist1.BringToFront();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            if (Classes.clsUser.department.ToString() != "3")
            {
                incomingWorklist1.loadIncomingAccntng();
                incomingWorklist1.BringToFront();
            }
            else
            {
                Alert.show("Error : Access denied.", Alert.AlertType.error);
                return;
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            dailyProductivity1.loadDaily();
            dailyProductivity1.BringToFront();
        }

        private void DashboardTH_Load(object sender, EventArgs e)
        {

        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            clsDashboard.WorkListCount(lblWorkCnt);
            clsDashboard.teamsWorkListCount(lblTeamCnt);
            clsDashboard.IncomingWork(lblIncoming);
            clsDashboard.DailyProductivity(lblDailyCnt);
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            timerRefreshAccntg.Enabled = false;
            timerAudit.Enabled = false;
            this.Close();
        }

        private void timerAudit_Tick(object sender, EventArgs e)
        {
            clsDashboard.workListAudit(lblWorkCnt);
            clsDashboard.teamsWorkListCountAudit(lblTeamCnt);
            lblIncoming.Text = "0";
            clsDashboard.DailyProductivityAudit(lblDailyCnt);
        }
    }
}
