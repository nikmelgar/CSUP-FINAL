using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Query : Form
    {
        public Query()
        {
            InitializeComponent();
            querySearchMember1.BringToFront();
        }

        Global global = new Global();
        Classes.clsQuery clsQuery = new Classes.clsQuery();

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMembership_Click(object sender, EventArgs e)
        {
            querySearchMember1.BringToFront();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            querySavingsDeposit1.minmax();
            querySavingsDeposit1.BringToFront();
            
        }
        private void button7_Click(object sender, EventArgs e)
        {
            queryMemberProfile1.BringToFront();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            queryShareCapital1.minmax();
            queryShareCapital1.BringToFront();
        }
    }
}
