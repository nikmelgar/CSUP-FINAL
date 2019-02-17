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
    public partial class subMembershipSearchPrincipal : Form
    {
        public subMembershipSearchPrincipal()
        {
            InitializeComponent();
        }

        Classes.clsSearchPrincipal clsSearchPrincipal = new Classes.clsSearchPrincipal();
        clsMembership clsmembership = new clsMembership();
        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        
        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void subMembershipSearchPrincipal_Load(object sender, EventArgs e)
        {
            clsSearchPrincipal.loadAllPrincipal(dataGridView1);
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

            if (dataGridView1.SelectedRows.Count == 0)
            {
                //No Data to be edit
                Alert.show("Please select principal!", Alert.AlertType.warning);
                return;
            }
           

            MembershipDataEntrycs mementry = new MembershipDataEntrycs();
            MembershipDataEntrycs dataentry = new MembershipDataEntrycs();

            foreach (Form form in Application.OpenForms)
            {

                if (form.GetType() == typeof(MembershipDataEntrycs))
                {
                    //===============================================================================
                    //                      If form is already open
                    //===============================================================================
                    form.Activate();
                    mementry = (MembershipDataEntrycs)Application.OpenForms["MembershipDataEntrycs"];
                    mementry.btnEdit.Enabled = false;
                    mementry.btnNew.Text = "SAVE";
                    mementry.btnDelete.Enabled = false;
                    mementry.picPicture.Image = mementry.imageList1.Images[0];
                    mementry.panel6.Enabled = false;
                    mementry.btnNew.Enabled = true;



                    //clear
                    mementry.clearAllFieldsFromDependentUsed();

                    //Move to Company Information        

                    dataentry = (MembershipDataEntrycs)Application.OpenForms["MembershipDataEntrycs"];
                    //First Panel (Personal Information)
                    dataentry.txtEmployeeIDNo.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                    dataentry.txtPrincipalNo.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                    dataentry.cmbCompany.Text = clsmembership.returnCompanyDescription(dataGridView1.SelectedRows[0].Cells["Company_Code"].Value.ToString());
                    dataentry.cmbPayrollGroup.Text = clsmembership.returnPayrollDescription(dataGridView1.SelectedRows[0].Cells["Payroll_Code"].Value.ToString());
                    dataentry.cmbCostCenter.Text = clsmembership.returnCostCenter(dataGridView1.SelectedRows[0].Cells["Cost_Center_Code"].Value.ToString());
                    dataentry.dtDateHired.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Hired"].Value);
                    dataentry.cmboPrevComp.Text = clsmembership.returnCompanyDescription(dataGridView1.SelectedRows[0].Cells["Prev_Company_Code"].Value.ToString());

                    //Disable Beneficiary tab
                    dataentry.dataGridView1.Enabled = false;
                    dataentry.tabControl1.SelectedTab = dataentry.tabPage2;
                    this.Close();

                    //Required fields validation for dependent
                    //tin sss bank atmno
                    dataentry.txtTINno.BackColor = SystemColors.Window;
                    dataentry.txtEmail.BackColor = SystemColors.Window;
                    dataentry.lblAsteriskEmail.Visible = false;
                    dataentry.lblAsteriskTIN.Visible = false;

                    return;
                }
            }

            mementry.Show();
            mementry.btnNew.Text = "SAVE";
            mementry.btnEdit.Enabled = false;
            mementry.btnDelete.Enabled = false;
            mementry.panel6.Enabled = false;

            //Move to Company Information        

            dataentry = (MembershipDataEntrycs)Application.OpenForms["MembershipDataEntrycs"];
            dataentry.picPicture.Image = dataentry.imageList1.Images[0];
            //First Panel (Personal Information)
            dataentry.txtEmployeeIDNo.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
            dataentry.txtPrincipalNo.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();    
            dataentry.cmbCompany.Text = clsmembership.returnCompanyDescription(dataGridView1.SelectedRows[0].Cells["Company_Code"].Value.ToString());
            dataentry.cmbPayrollGroup.Text = clsmembership.returnPayrollDescription(dataGridView1.SelectedRows[0].Cells["Payroll_Code"].Value.ToString());
            dataentry.cmbCostCenter.Text = clsmembership.returnCostCenter(dataGridView1.SelectedRows[0].Cells["Cost_Center_Code"].Value.ToString());
            dataentry.dtDateHired.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Hired"].Value);
            dataentry.cmboPrevComp.Text = clsmembership.returnCompanyDescription(dataGridView1.SelectedRows[0].Cells["Prev_Company_Code"].Value.ToString());
            
            dataentry.dtDateResigned.Text = dataGridView1.SelectedRows[0].Cells["Date_Resigned_From_Company"].Value.ToString();
            dataentry.dtResignedFromPecci.Text = dataGridView1.SelectedRows[0].Cells["Date_Resigned_From_Pecci"].Value.ToString();

            //Disable Beneficiary tab
            dataentry.dataGridView1.Enabled = false;
            dataentry.tabControl1.SelectedTab = dataentry.tabPage2;

            //Required fields validation for dependent
            //tin sss bank atmno
            dataentry.txtTINno.BackColor = SystemColors.Window;
            dataentry.txtEmail.BackColor = SystemColors.Window;
            dataentry.lblAsteriskEmail.Visible = false;
            dataentry.lblAsteriskTIN.Visible = false;

            this.Close();

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (txtEmployeeID.Text == "" && txtFirstName.Text == "" && txtLastName.Text == "")
            {
                Alert.show("No keywords to be search!", Alert.AlertType.warning);
                return;
            }
            else
            {
                clsSearchPrincipal.SearchMembers(dataGridView1, txtEmployeeID.Text, txtLastName.Text, txtFirstName.Text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clsSearchPrincipal.loadAllPrincipal(dataGridView1);
            txtEmployeeID.Text = "";
            txtLastName.Text = "";
            txtFirstName.Text = "";
        }

        private void txtEmployeeID_TextChanged(object sender, EventArgs e)
        {
            clsSearchPrincipal.SearchMembers(dataGridView1, txtEmployeeID.Text, txtLastName.Text, txtFirstName.Text);
        }
    }
}
