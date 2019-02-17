using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2.LoanReports
{
    public partial class searchMemberAmort : Form
    {
        public searchMemberAmort()
        {
            InitializeComponent();
        }
        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        //=================================================
        //              Classes
        //=================================================
        Global global = new Global();
        loanFrms.clsLookUpAmort clsLookUp = new loanFrms.clsLookUpAmort();
        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void searchMemberAmort_Load(object sender, EventArgs e)
        {
            clsLookUp.loaddefaultitems(dataGridView1);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtEmployeeID.Text == "" && txtLastName.Text == "" && txtFirstName.Text == "")
            {
                Alert.show("No keywords to be search!", Alert.AlertType.warning);
                return;
            }
            clsLookUp.search(txtEmployeeID.Text, txtFirstName.Text, txtLastName.Text, dataGridView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            clsLookUp.loaddefaultitems(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // dataGridView1.Rows[selectedRow].Cells[selected].Value = dataGridView3.SelectedRows[0].Cells["EmployeeID"].Value.ToString() + " - " + dataGridView3.SelectedRows[0].Cells["Name"].Value.ToString();

            LoanAmmortizationComputationcs loanAmort = new LoanAmmortizationComputationcs();

            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(LoanAmmortizationComputationcs))
                {
                    //===============================================================================
                    //                      If form is already open
                    //===============================================================================
                    form.Activate();
                    loanAmort = (LoanAmmortizationComputationcs)Application.OpenForms["LoanAmmortizationComputationcs"];
                    LoanAmmortizationComputationcs.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                    loanAmort.txtEmployeeID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                    loanAmort.txtName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();

                    
                    this.Close();
                    return;
                }
            }

            this.Close();
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
    }
}
