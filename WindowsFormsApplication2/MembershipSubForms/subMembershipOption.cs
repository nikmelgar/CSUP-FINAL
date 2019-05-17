using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2.MembershipSubForms
{
    public partial class subMembershipOption : Form
    {
        public subMembershipOption()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        clsMembershipEntry clsmementry = new clsMembershipEntry();
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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (radioPrincipal.Checked == true)
            {
                this.Close();

                //Set For Principal

                clsMembershipEntry.principal = "1";
                MembershipDataEntrycs mementry = new MembershipDataEntrycs();

                foreach (Form form in Application.OpenForms)
                {

                    if (form.GetType() == typeof(MembershipDataEntrycs))
                    {
                        form.Activate();
                        mementry = (MembershipDataEntrycs)Application.OpenForms["MembershipDataEntrycs"];
                        mementry.clearAllFields();
                        mementry.btnEdit.Enabled = false;
                        mementry.btnNew.Enabled = true;
                        mementry.btnNew.Text = "SAVE";
                        mementry.picPicture.Image = mementry.imageList1.Images[0];
                        mementry.tabControl1.SelectedTab = mementry.tabPage2;
                        mementry.panel6.Enabled = true;
                        mementry.dataGridView1.Enabled = true;

                        //Required fields validation for dependent
                        //tin sss bank atmno
                        mementry.txtTINno.BackColor = Color.FromArgb(128, 255, 128);
                        mementry.txtAccountNo.BackColor = Color.FromArgb(128, 255, 128);
                        mementry.txtEmail.BackColor = Color.FromArgb(128, 255, 128);
                        mementry.lblAsteriskEmail.Visible = true;
                        mementry.lblAsteriskTIN.Visible = true;
                        mementry.lblAsteriskBank.Visible = true;
                        mementry.lblAsteriskATMnO.Visible = true;

                        return;
                    }
                }

                mementry.clearAllFields();
                mementry.btnEdit.Enabled = false;
                mementry.btnNew.Text = "SAVE";
                mementry.btnNew.Enabled = true;
                mementry.picPicture.Image = mementry.imageList1.Images[0];
                mementry.tabControl1.SelectedTab = mementry.tabPage2;
                mementry.panel6.Enabled = true;
                mementry.dataGridView1.Enabled = true;

                //Required fields validation for dependent
                //tin sss bank atmno
                mementry.txtTINno.BackColor = Color.FromArgb(128, 255, 128);
                mementry.txtAccountNo.BackColor = Color.FromArgb(128, 255, 128);
                mementry.txtEmail.BackColor = Color.FromArgb(128, 255, 128);
                mementry.lblAsteriskEmail.Visible = true;
                mementry.lblAsteriskTIN.Visible = true;
                mementry.lblAsteriskBank.Visible = true;
                mementry.lblAsteriskATMnO.Visible = true;

                mementry.Show();

            }
            else if (radioDependent.Checked == true) 
            {
                this.Close();
                //New Window for searching the principal
                //controls

                //Set For Principal

                clsMembershipEntry.principal = "0";

                foreach (Form form in Application.OpenForms)
                {


                    if (form.GetType() == typeof(MembershipSubForms.subMembershipSearchPrincipal))
                    {
                        form.Activate();
                        return;
                    }
                }
                subMembershipSearchPrincipal subSearchPrincipal = new subMembershipSearchPrincipal();
                subSearchPrincipal.Show();
                
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
