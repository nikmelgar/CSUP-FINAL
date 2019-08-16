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
    public partial class teamsWorklist : UserControl
    {
        public teamsWorklist()
        {
            InitializeComponent();
            clsDashboard.gettingTeamsUsername(comboBox1);
            comboBox1.SelectedIndex = -1;
        }
        Global global = new Global();
        DashboardClasses.clsDashboardTH clsDashboard = new DashboardClasses.clsDashboardTH();
        DashboardClasses.clsTeamWorkList clsTeamWork = new DashboardClasses.clsTeamWorkList();
        private void teamsWorklist_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            lblCnt.Text = "0";
            dgvList.Rows.Clear();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text != "")
            {
                clsTeamWork.loadAllVoucherPerUsername(lblCnt, dgvList, comboBox1.SelectedValue.ToString());
            }
        }
    }
}
