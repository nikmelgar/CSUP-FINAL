using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2.ReminderPDC
{
    public partial class reminder : Form
    {
        public reminder()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            int x = (panel5.Size.Width - lblSpiel.Size.Width) / 2;
            lblSpiel.Location = new Point(x, lblSpiel.Location.Y);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            MainForm frm = new MainForm();
            frm = (MainForm)Application.OpenForms["MainForm"];
            frm.tmerTickIfClickOK.Interval = 5000;
            frm.timer1.Stop();
            frm.tmerTickIfClickOK.Start();
            this.Close();
        }

        private void btnRemid_Click(object sender, EventArgs e)
        {
            MainForm frm = new MainForm();
            frm = (MainForm)Application.OpenForms["MainForm"];
            frm.timer1.Interval = 1000;
            frm.timer1.Start();
            this.Close();

        }
    }
}
