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
    public partial class DashboardStaff : Form
    {
        public DashboardStaff()
        {
            InitializeComponent();
            if(Classes.clsUser.department.ToString() == "1")
            {
                //Accounting
                clsDashboardStaff.WorkListCount(lblWorkCnt);
                clsDashboardStaff.DailyProductivity(lblDailyCnt);
            }
            else
            {
                clsDashboardTH.workListAudit(lblWorkCnt);
                clsDashboardTH.DailyProductivityAudit(lblDailyCnt);
            }
            
        }

        DashboardClasses.clsWorklistsStaff clsWorklistStaff = new DashboardClasses.clsWorklistsStaff();
        DashboardClasses.clsDashboardStaff clsDashboardStaff = new DashboardClasses.clsDashboardStaff();
        DashboardClasses.clsDashboardTH clsDashboardTH = new DashboardClasses.clsDashboardTH();
        private void DashboardStaff_Load(object sender, EventArgs e)
        {
            
        }

        private void myWorkList_Click(object sender, EventArgs e)
        {
            worklistStaff1.BringToFront();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            dailyProductivityStaff1.loadDailyActivity();
            dailyProductivityStaff1.BringToFront();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            timerAccntg.Enabled = false;
            this.Close();
        }

        private void timerAccntg_Tick(object sender, EventArgs e)
        {
            clsDashboardStaff.WorkListCount(lblWorkCnt);
            clsDashboardStaff.DailyProductivity(lblDailyCnt);
        }

        private void timerAudit_Tick(object sender, EventArgs e)
        {
            clsDashboardTH.workListAudit(lblWorkCnt);
            clsDashboardTH.DailyProductivityAudit(lblDailyCnt);
        }

        private void worklistStaff1_Load(object sender, EventArgs e)
        {

        }
    }
}
