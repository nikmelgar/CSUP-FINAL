using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2.PDC
{
    public partial class searchMember : Form
    {
        public searchMember()
        {
            InitializeComponent();
        }
        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        Classes.clsLoanLookUp clsLookUp = new Classes.clsLoanLookUp();
        private void label1_Click(object sender, EventArgs e)
        {
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

        private void searchMember_Load(object sender, EventArgs e)
        {
            clsLookUp.loaddefaultitems(dataGridView1);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtEmployeeID.Text == "" && txtLastName.Text == "" && txtFirstName.Text == "")
            {
                Alert.show("Please enter valid Keyword.", Alert.AlertType.warning);
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
            PDCManagementV2 pdc = new PDCManagementV2();

            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(PDCManagementV2))
                {
                    //===============================================================================
                    //                      If form is already open
                    //===============================================================================
                    form.Activate();
                    pdc = (PDCManagementV2)Application.OpenForms["PDCManagementV2"];
                    Classes.clsPDCManagement.userid = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());

                    pdc.txtEmployeeID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                    pdc.txtEmployeeName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();
                }
            }
            Classes.clsPDCManagement.userid = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());

            pdc.txtEmployeeID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
            pdc.txtEmployeeName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();
            this.Close();
        }
    }
}
