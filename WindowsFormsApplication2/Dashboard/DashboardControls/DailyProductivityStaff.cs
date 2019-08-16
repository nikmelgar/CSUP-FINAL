using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2.Dashboard.DashboardControls
{
    public partial class DailyProductivityStaff : UserControl
    {
        public DailyProductivityStaff()
        {
            InitializeComponent();
        }

        Global global = new Global();
        DashboardClasses.clsDailyProductivityStaffAccnt clsDaily = new DashboardClasses.clsDailyProductivityStaffAccnt();
        private void DailyProductivityStaff_Load(object sender, EventArgs e)
        {

        }
        
        public void loadDailyActivity()
        {
            if(Classes.clsUser.department.ToString() == "1")
            {
                clsDaily.loadAllVoucher(lblCnt, dgvList);
            }
            else
            {
                clsDaily.loadAllVoucherAudit(lblCnt, dgvList);
            }
            
        }
    }
}
