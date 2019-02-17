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
    public partial class lookupPrincipal : Form
    {
        public lookupPrincipal()
        {
            InitializeComponent();
        }
        //=======================================================
        //              MOVEABLE PANEL
        //=======================================================

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        Global global = new Global();
        Classes.clsLookPrincipal clsLookUp = new Classes.clsLookPrincipal();
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //Moveable Forms / Screens
            //Nikko Melgar
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

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lookupPrincipal_Load(object sender, EventArgs e)
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
            clsLookUp.searchPrincipal(txtEmployeeID.Text, txtFirstName.Text, txtLastName.Text, dataGridView1);
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
            HoldAccounts holdAccnt = new HoldAccounts();

            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(HoldAccounts))
                {
                    //===============================================================================
                    //                      If form is already open
                    //===============================================================================
                    form.Activate();
                    holdAccnt = (HoldAccounts)Application.OpenForms["HoldAccounts"];
                    Classes.clsHoldAccounts.userid = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());

                    
                    holdAccnt.txtID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                    holdAccnt.txtName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();
                    holdAccnt.btnLock.Text = "LOCK";
                    holdAccnt.btnCancel.Visible = true;
                    this.Close();
                    return;
                }
            }

            Classes.clsHoldAccounts.userid = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
            holdAccnt.txtID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
            holdAccnt.txtName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();
            holdAccnt.btnLock.Text = "LOCK";
            holdAccnt.btnCancel.Visible = true;
            this.Close();
        }
    }
}
