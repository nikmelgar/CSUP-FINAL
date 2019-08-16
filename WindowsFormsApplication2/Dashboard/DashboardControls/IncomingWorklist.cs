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
    public partial class IncomingWorklist : UserControl
    {
        public IncomingWorklist()
        {
            InitializeComponent();
        }

        DashboardClasses.clsIncomingWork clsIncoming = new DashboardClasses.clsIncomingWork();
        private void IncomingWorklist_Load(object sender, EventArgs e)
        {
            
        }

        public void loadIncomingAccntng()
        {
            clsIncoming.loadAllVoucher(lblCnt, dgvList);
        }
    }
}
