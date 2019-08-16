using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Dashboard.DashboardControls
{
    public partial class DailyProductivity : UserControl
    {
        public DailyProductivity()
        {
            InitializeComponent();
        }

        Global global = new Global();
        DashboardClasses.clsDailyProductivity clsDaily = new DashboardClasses.clsDailyProductivity();
        private void DailyProductivity_Load(object sender, EventArgs e)
        {

        }
        public void loadDaily()
        {
            clsDaily.loadAllVoucher(lblCnt, dgvList);
        }
    }
}
