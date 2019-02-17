using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2.ReportsForms
{
    public partial class rptBPI : Form
    {
        public rptBPI()
        {
            InitializeComponent();
        }

        private void rptBPI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
