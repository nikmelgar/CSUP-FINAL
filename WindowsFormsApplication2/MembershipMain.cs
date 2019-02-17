using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2
{
    public partial class MembershipMain : Form
    {
        public MembershipMain()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        bool x;
        clsHoverDash cls = new clsHoverDash();
        clsMembership clsMembership = new clsMembership();
        private void lblAddNewMember_Click(object sender, EventArgs e)
        {
            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(MembershipSubForms.subMembershipOption))
                {
                    form.Activate();
                    return;
                }
            }

            MembershipSubForms.subMembershipOption membership = new MembershipSubForms.subMembershipOption();
            membership.Show();

        }

        private void btnClose_Click(object sender, EventArgs e)
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

        #region Design Hover Dashboard
        
        //Total No. Of Members
        private void label7_MouseHover(object sender, EventArgs e)
        {
            cls.hoverPanelColor(panel6, label7, Color.FromArgb(60, 141, 188));
            pictureBox8.Image = imageList1.Images[1];
        }

        private void label7_MouseLeave(object sender, EventArgs e)
        {
            cls.leaveHoverPanel(panel6, label7, Color.FromArgb(60, 141, 188));
            pictureBox8.Image = imageList1.Images[4];
        }

        private void label8_MouseHover(object sender, EventArgs e)
        {
            cls.hoverPanelColor(panel12, label8, Color.SeaGreen);
            pictureBox5.Image = imageList1.Images[1];
        }

        private void label8_MouseLeave(object sender, EventArgs e)
        {
            cls.leaveHoverPanel(panel12, label8, Color.SeaGreen);
            pictureBox5.Image = imageList1.Images[0];
        }

        private void label9_MouseHover(object sender, EventArgs e)
        {
            cls.hoverPanelColor(panel18, label9, Color.FromArgb(240, 173, 78));
            pictureBox7.Image = imageList1.Images[1];
        }

        private void label9_MouseLeave(object sender, EventArgs e)
        {
            cls.leaveHoverPanel(panel18, label9, Color.FromArgb(240, 173, 78));
            pictureBox7.Image = imageList1.Images[3];
        }

        private void lblAddNewMember_MouseHover(object sender, EventArgs e)
        {
            cls.hoverPanelColor(panel29, lblAddNewMember, Color.FromArgb(217, 83, 79));
            pictureBox6.Image = imageList1.Images[1];
        }

        private void lblAddNewMember_MouseLeave(object sender, EventArgs e)
        {
            cls.leaveHoverPanel(panel29, lblAddNewMember, Color.FromArgb(217, 83, 79));
            pictureBox6.Image = imageList1.Images[2];
        }

        #endregion

        private void MembershipMain_Load(object sender, EventArgs e)
        {
            loadDatas();
        }

        public void loadDatas()
        {
            clsMembership.loadMembership(dataGridView1);

            //Dashboard labels
            clsMembership.countPendingMembers(lblTotalPendingMembers); //Change to Members for Approval
            clsMembership.countActiveMembers(lblTotalActiveMembers); //Change to Non-Payrol Members
            clsMembership.countTotalMembers(lblTotalNumberCount);   //Change to Total Active Members (Payroll)
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                //No Data to be edit
                Alert.show("Please select members you want to edit!", Alert.AlertType.warning);
                return;
            }

            //show form
            foreach (Form form in Application.OpenForms)
            {

                if (form.GetType() == typeof(MembershipDataEntrycs))
                {
                    form.Activate();
                    x = true;
                }
                else
                {
                    x = false;
                }
                
            }

            MembershipDataEntrycs mementry = new MembershipDataEntrycs();
            mementry.btnDelete.Enabled = true;
            mementry.btnEdit.Enabled = true;
            mementry.btnNew.Enabled = false;
            if(x != true)
            {
                mementry.Show();
            }

            MembershipDataEntrycs dataentry = new MembershipDataEntrycs();

            dataentry = (MembershipDataEntrycs)Application.OpenForms["MembershipDataEntrycs"];

            //get If Principal or not
            if(dataGridView1.SelectedRows[0].Cells["Principal"].Value.ToString() == "True")
            {
                dataentry.panel6.Enabled = true;
                dataentry.dataGridView1.Enabled = true;

                //Required fields validation for dependent
                //tin sss bank atmno
                dataentry.txtTINno.BackColor = Color.FromArgb(128, 255, 128);
                dataentry.txtAccountNo.BackColor = Color.FromArgb(128, 255, 128);
                dataentry.txtEmail.BackColor = Color.FromArgb(128, 255, 128);
                dataentry.lblAsteriskEmail.Visible = true;
                dataentry.lblAsteriskTIN.Visible = true;
                dataentry.lblAsteriskBank.Visible = true;
                dataentry.lblAsteriskATMnO.Visible = true;

                
            }
            else
            {
                dataentry.panel6.Enabled = false;
                dataentry.dataGridView1.Enabled = false;

                //Required fields validation for dependent
                //tin sss bank atmno
                dataentry.txtTINno.BackColor = SystemColors.Window;
                dataentry.txtEmail.BackColor = SystemColors.Window;
                dataentry.lblAsteriskEmail.Visible = false;
                dataentry.lblAsteriskTIN.Visible = false;
                
            }


            dataentry.btnDelete.Enabled = true;
            dataentry.btnEdit.Enabled = true;
            dataentry.btnNew.Enabled = false;


            //First Panel (Personal Information)
            clsMembershipEntry.userID = dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString();
            clsMembershipEntry.principal = dataGridView1.SelectedRows[0].Cells["Principal"].Value.ToString();
            dataentry.txtLastName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString();
            dataentry.txtFirstName.Text = dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString();
            dataentry.txtMiddleName.Text = dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString();
            dataentry.txtSuffix.Text = dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();
            dataentry.txtAddress.Text = dataGridView1.SelectedRows[0].Cells["Residential_Address"].Value.ToString();
            dataentry.dtDateOfBirth.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Of_Birth"].Value);
            dataentry.cmbGender.Text = dataGridView1.SelectedRows[0].Cells["Gender"].Value.ToString();
            dataentry.txtPlaceOfBirth.Text = dataGridView1.SelectedRows[0].Cells["Place_Of_Birth"].Value.ToString();
            dataentry.cmbCivilStatus.SelectedItem = dataGridView1.SelectedRows[0].Cells["Civil_Status"].Value.ToString();
            dataentry.txtSpouseName.Text = dataGridView1.SelectedRows[0].Cells["Name_Of_Spouse"].Value.ToString();
            dataentry.cmbAreaCode.SelectedValue = dataGridView1.SelectedRows[0].Cells["Area_Code"].Value.ToString();
            dataentry.txtHomeTel.Text = dataGridView1.SelectedRows[0].Cells["Home_Tel_No"].Value.ToString();
            dataentry.txtCellNo.Text = dataGridView1.SelectedRows[0].Cells["Cellphone_No"].Value.ToString();
            dataentry.txtTINno.Text = dataGridView1.SelectedRows[0].Cells["TinNo"].Value.ToString();
            dataentry.txtSSSNo.Text = dataGridView1.SelectedRows[0].Cells["SSSNo"].Value.ToString();
            dataentry.txtEmail.Text = dataGridView1.SelectedRows[0].Cells["Email_Address"].Value.ToString();
            dataentry.cmbBankName.Text = clsMembership.returnBankName(dataGridView1.SelectedRows[0].Cells["Bank_Code"].Value.ToString());
            dataentry.txtAccountNo.Text = dataGridView1.SelectedRows[0].Cells["Atm_Account_No"].Value.ToString();
            dataentry.txtPlacePMS.Text = dataGridView1.SelectedRows[0].Cells["Place_PMS"].Value.ToString();

            //For Uploading of members
            try
            {
                dataentry.dtDatePMS.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Of_PMS"].Value);
            }
            catch
            {

            }

            //Second Panel (Company Information)
            dataentry.txtEmployeeIDNo.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
            dataentry.cmbCompany.Text = clsMembership.returnCompanyDescription(dataGridView1.SelectedRows[0].Cells["Company_Code"].Value.ToString());
            dataentry.cmbPayrollGroup.Text = clsMembership.returnPayrollDescription(dataGridView1.SelectedRows[0].Cells["Payroll_Code"].Value.ToString());
            dataentry.cmbCostCenter.Text = clsMembership.returnCostCenter(dataGridView1.SelectedRows[0].Cells["Cost_Center_Code"].Value.ToString());
            dataentry.dtDateHired.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Hired"].Value);
            dataentry.cmbOfficeArea.SelectedValue = dataGridView1.SelectedRows[0].Cells["Office_Area_Code"].Value.ToString();
            dataentry.txtOfficeTelNo.Text = dataGridView1.SelectedRows[0].Cells["Office_Tel_No"].Value.ToString();
            dataentry.cmboPrevComp.Text = clsMembership.returnCompanyDescription(dataGridView1.SelectedRows[0].Cells["Prev_Company_Code"].Value.ToString());

            if(dataGridView1.SelectedRows[0].Cells["Salary"].Value.ToString() != "")
            {
                dataentry.txtSalary.Text = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Salary"].Value).ToString("#,0.00");
            }
            else
            {
                dataentry.txtSalary.Text = dataGridView1.SelectedRows[0].Cells["Salary"].Value.ToString();
            }
         
            dataentry.txtPrincipalNo.Text = dataGridView1.SelectedRows[0].Cells["PrincipalID"].Value.ToString();



            //==================================================================================================
            //                          RESIGN FROM COMPANY
            //==================================================================================================
            if (dataGridView1.SelectedRows[0].Cells["Date_Resigned_From_Company"].Value.ToString() != string.Empty && dataGridView1.SelectedRows[0].Cells["Date_Resigned_From_Company"].Value != DBNull.Value)
            {
                dataentry.dtDateResigned.Text = dataGridView1.SelectedRows[0].Cells["Date_Resigned_From_Company"].Value.ToString();
            }
            else if (dataGridView1.SelectedRows[0].Cells["Date_Resigned_From_Company"].Value != DBNull.Value)
            {
                dataentry.dtDateResigned.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Resigned_From_Company"].Value);
            }
            else
            {
                dataentry.dtDateResigned.Value = DateTime.Today.Date;
                dataentry.dtDateResigned.Checked = false;
            }

            //==================================================================================================
            //                          RESIGN FROM PECCI
            //==================================================================================================
            if (dataGridView1.SelectedRows[0].Cells["Date_Resigned_From_Pecci"].Value.ToString() != string.Empty && dataGridView1.SelectedRows[0].Cells["Date_Resigned_From_Pecci"].Value != DBNull.Value)
            {
                dataentry.dtResignedFromPecci.Text = dataGridView1.SelectedRows[0].Cells["Date_Resigned_From_Pecci"].Value.ToString();
            }
            else if (dataGridView1.SelectedRows[0].Cells["Date_Resigned_From_Pecci"].Value != DBNull.Value)
            {
                dataentry.dtResignedFromPecci.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Resigned_From_Pecci"].Value);
            }
            else
            {
                dataentry.dtResignedFromPecci.Value = DateTime.Today.Date;
                dataentry.dtResignedFromPecci.Checked = false;
            }
            
            //Third Panel (Contact Person)
            dataentry.txtContactName.Text = dataGridView1.SelectedRows[0].Cells["Contact_Person"].Value.ToString();
            dataentry.txtContactNo1.Text = dataGridView1.SelectedRows[0].Cells["Contact_No1"].Value.ToString();
            dataentry.txtContactNo2.Text = dataGridView1.SelectedRows[0].Cells["Contact_No2"].Value.ToString();
            dataentry.cmbContactAreaCode.SelectedValue = dataGridView1.SelectedRows[0].Cells["Contact_Area_Code"].Value.ToString();

            //Added Feb 17 2019 as per maam vangie
            //Relationship and Remarks
            dataentry.txtRelationship.Text = dataGridView1.SelectedRows[0].Cells["Relationship"].Value.ToString();
            dataentry.txtRemarks.Text = dataGridView1.SelectedRows[0].Cells["Remarks"].Value.ToString();

            //Forth Panel (Other Information)
            dataentry.dtDateMembership.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Of_Membership"].Value);
            
            try
            {
                dataentry.dtFirstDeduction.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["First_Deduction"].Value);
            }
            catch
            {

            }


            //DECIMALS FORMAT
            if (dataGridView1.SelectedRows[0].Cells["Membership_Fee"].Value.ToString() != "")
            {
                dataentry.txtMembershipFee.Text = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Membership_Fee"].Value).ToString("#,##0.00");
            }
            else
            {
                dataentry.txtMembershipFee.Text = dataGridView1.SelectedRows[0].Cells["Membership_Fee"].Value.ToString();
            }

            if(dataGridView1.SelectedRows[0].Cells["Share_Capital"].Value.ToString() != "")
            {
                dataentry.txtShareCapital.Text = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Share_Capital"].Value).ToString("#,##0.00");
            }
            else
            {
                dataentry.txtShareCapital.Text = dataGridView1.SelectedRows[0].Cells["Share_Capital"].Value.ToString();
            }
            
            if(dataGridView1.SelectedRows[0].Cells["Savings_Deposit"].Value.ToString() != "")
            {
                dataentry.txtSavingsDeposit.Text = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Savings_Deposit"].Value).ToString("#,##0.00");
            }
            else
            {
                dataentry.txtSavingsDeposit.Text = dataGridView1.SelectedRows[0].Cells["Savings_Deposit"].Value.ToString();
            }
           
            

            //For Beneficiaries Panel
            clsMembership.loadBeneficiaries(dataentry.dataGridView1, dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString());
            clsMembership.loadPicture(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString(), dataentry.picPicture);
            dataentry = null;
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            clsMembership.loadMembership(dataGridView1);
            //Dashboard labels
            clsMembership.countPendingMembers(lblTotalPendingMembers);
            clsMembership.countActiveMembers(lblTotalActiveMembers);
            clsMembership.countTotalMembers(lblTotalNumberCount);

            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            chckDependent.Checked = false;
            chckPrincipal.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(txtEmployeeID.Text == "" && txtFirstName.Text == "" && txtLastName.Text == "")
            {
                Alert.show("No keywords to be search!", Alert.AlertType.warning);
                return;
            }
            else
            {
                clsMembership.SearchMembers(dataGridView1, txtEmployeeID.Text, txtLastName.Text, txtFirstName.Text, chckPrincipal, chckDependent);
            }
        }

        private void txtEmployeeID_TextChanged(object sender, EventArgs e)
        {
            clsMembership.SearchMembers(dataGridView1, txtEmployeeID.Text, txtLastName.Text, txtFirstName.Text, chckPrincipal, chckDependent);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button2_Click(sender, e);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            //All Members
            CrystalDecisions.Shared.TableLogOnInfo li;

            //Print Purposes
            SqlConnection con = new SqlConnection();
            Global global = new Global();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_MembershipReport WHERE IsApprove ='1' and IsActive = '1' and Company_Code <> 'COMP010' ORDER BY Company ASC", con);



            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            ReportsForms.rptMembershipMainPage cr = new ReportsForms.rptMembershipMainPage();
            ReportsForms.rptMembershipMain rpt = new ReportsForms.rptMembershipMain();

            li = new TableLogOnInfo();

            li.ConnectionInfo.IntegratedSecurity = false;

            adapter.Fill(ds, "vw_MembershipReport");
            dt = ds.Tables["vw_MembershipReport"];
            cr.SetDataSource(ds.Tables["vw_MembershipReport"]);
            cr.SetParameterValue("status", "Active Members");

            //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
            cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

            rpt.crystalReportViewer1.ReportSource = cr;
            rpt.ShowDialog();
        }

        private void label9_Click(object sender, EventArgs e)
        {
            //All ACTIVE
            CrystalDecisions.Shared.TableLogOnInfo li;

            //Print Purposes
            SqlConnection con = new SqlConnection();
            Global global = new Global();
            global.connection(con);

            //Change from Active Members to Non-Payroll Members
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_MembershipReport where isActive = 1 and isApprove = 1 and Company_Code = 'COMP010'", con);



            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            ReportsForms.rptMembershipMainPageNonPayroll cr = new ReportsForms.rptMembershipMainPageNonPayroll();
            ReportsForms.rptMembershipMain rpt = new ReportsForms.rptMembershipMain();

            li = new TableLogOnInfo();

            li.ConnectionInfo.IntegratedSecurity = false;

            adapter.Fill(ds, "vw_MembershipReport");
            dt = ds.Tables["vw_MembershipReport"];
            cr.SetDataSource(ds.Tables["vw_MembershipReport"]);
            cr.SetParameterValue("status", "Non-Payroll Members");

            //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
            cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

            rpt.crystalReportViewer1.ReportSource = cr;
            rpt.ShowDialog();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            //All ACTIVE
            CrystalDecisions.Shared.TableLogOnInfo li;

            //Print Purposes
            SqlConnection con = new SqlConnection();
            Global global = new Global();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_MembershipReport where isActive = 1 and isApprove is null ORDER BY Company ASC", con);



            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            ReportsForms.rptMembershipMainPage cr = new ReportsForms.rptMembershipMainPage();
            ReportsForms.rptMembershipMain rpt = new ReportsForms.rptMembershipMain();

            li = new TableLogOnInfo();

            li.ConnectionInfo.IntegratedSecurity = false;

            adapter.Fill(ds, "vw_MembershipReport");
            dt = ds.Tables["vw_MembershipReport"];
            cr.SetDataSource(ds.Tables["vw_MembershipReport"]);
            cr.SetParameterValue("status", "Members for Approval");

            //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
            cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

            rpt.crystalReportViewer1.ReportSource = cr;
            rpt.ShowDialog();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(MembershipMain))
                {
                    form.Activate();
                    return;
                }
            }

            MembershipMain frm = new MembershipMain();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }
    }
}
