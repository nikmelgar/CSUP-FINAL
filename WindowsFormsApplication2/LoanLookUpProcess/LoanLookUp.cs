using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2.LoanLookUpProcess
{
    public partial class LoanLookUp : Form
    {
        public LoanLookUp()
        {
            InitializeComponent();
        }

        Global global = new Global();
        clsLoanLookUpMember clsLookUp = new clsLoanLookUpMember();

        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void LoanLookUp_Load(object sender, EventArgs e)
        {
            clsLookUp.loadLoanMember(dataGridView1);
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
            if(dataGridView1.SelectedRows.Count > 0)
            {
                if (LoanLookUpProcess.clsLoanLookUpMember.frmPass == "Journal")
                {
                    JournalVoucher jv = new JournalVoucher();

                    foreach (Form form in Application.OpenForms)
                    {
                        if (form.GetType() == typeof(JournalVoucher))
                        {
                            //===============================================================================
                            //                      If form is already open
                            //===============================================================================
                            form.Activate();
                            jv = (JournalVoucher)Application.OpenForms["JournalVoucher"];

                            jv.txtLoanNumber.Text = dataGridView1.SelectedRows[0].Cells["Loan_No"].Value.ToString();
                            jv.txtLoanType.Text = dataGridView1.SelectedRows[0].Cells["Loan_Type"].Value.ToString();
                        }
                    }

                    jv.txtLoanNumber.Text = dataGridView1.SelectedRows[0].Cells["Loan_No"].Value.ToString();
                    jv.txtLoanType.Text = dataGridView1.SelectedRows[0].Cells["Loan_Type"].Value.ToString();
                    this.Close();
                }
                else if(LoanLookUpProcess.clsLoanLookUpMember.frmPass == "Disbursement")
                {
                    DisbursementVoucher cv = new DisbursementVoucher();

                    foreach (Form form in Application.OpenForms)
                    {
                        if (form.GetType() == typeof(DisbursementVoucher))
                        {
                            //===============================================================================
                            //                      If form is already open
                            //===============================================================================
                            form.Activate();
                            cv = (DisbursementVoucher)Application.OpenForms["DisbursementVoucher"];

                            cv.txtLoanNo.Text = dataGridView1.SelectedRows[0].Cells["Loan_No"].Value.ToString();
                            cv.txtLoanType.Text = dataGridView1.SelectedRows[0].Cells["Loan_Type"].Value.ToString();
                        }
                    }

                    cv.txtLoanNo.Text = dataGridView1.SelectedRows[0].Cells["Loan_No"].Value.ToString();
                    cv.txtLoanType.Text = dataGridView1.SelectedRows[0].Cells["Loan_Type"].Value.ToString();
                    this.Close();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
