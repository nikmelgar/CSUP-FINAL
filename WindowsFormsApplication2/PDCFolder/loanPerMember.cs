using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2.PDCFolder
{
    public partial class loanPerMember : Form
    {
        public loanPerMember()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        Classes.clsPDCManagement clsPDC = new Classes.clsPDCManagement();

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void loanPerMember_Load(object sender, EventArgs e)
        {
            clsPDC.loadLoanPerMemberPDC(dataGridView1);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
        {
            PDCManagement pdc = new PDCManagement();

            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(PDCManagement))
                {
                    //===============================================================================
                    //                      If form is already open
                    //===============================================================================
                    form.Activate();
                    pdc = (PDCManagement)Application.OpenForms["PDCManagement"];

                    pdc.txtLoanNumber.Text = dataGridView1.SelectedRows[0].Cells["Loan_No"].Value.ToString();
                    pdc.txtLoanType.Text = dataGridView1.SelectedRows[0].Cells["Loan_Type"].Value.ToString();
                    pdc.txtGross.Text = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Loan_Amount"].Value.ToString()).ToString("#,0.00");
                }
            }

            pdc.txtLoanNumber.Text = dataGridView1.SelectedRows[0].Cells["Loan_No"].Value.ToString();
            pdc.txtLoanType.Text = dataGridView1.SelectedRows[0].Cells["Loan_Type"].Value.ToString();
            pdc.txtGross.Text = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Loan_Amount"].Value.ToString()).ToString("#,0.00");
            this.Close();
        }
    }
}
