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
    public partial class Billing : Form
    {
        public Billing()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        clsMembershipEntry clsMembershipEntry = new clsMembershipEntry();
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void Billing_Load(object sender, EventArgs e)
        {
            clsMembershipEntry.loadComboBox(cmbCompany, "Company", "Description", "Company_Code");
            clsMembershipEntry.loadComboBox(cmbRank, "Payroll_Group", "Description", "Payroll_Code");
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cmbCompany.SelectedIndex = -1;
            cmbRank.SelectedIndex = -1;
            dtBillDate.Value = DateTime.Today;
        }
    }
}
