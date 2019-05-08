using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2.loanFrms
{
    public partial class lookUpMember : Form
    {
        public lookUpMember()
        {
            InitializeComponent();
        }
        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        Global global = new Global();
        Classes.clsLoanLookUp clsLookUp = new Classes.clsLoanLookUp();
        Classes.clsLoanDataEntry clsLoanDataEntry = new Classes.clsLoanDataEntry();
        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lookUpMember_Load(object sender, EventArgs e)
        {
            clsLookUp.loaddefaultitems(dataGridView1);
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

        private void button1_Click(object sender, EventArgs e)
        {
            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            clsLookUp.loaddefaultitems(dataGridView1);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtEmployeeID.Text == "" && txtLastName.Text == "" && txtFirstName.Text == "")
            {
                Alert.show("Please enter valid keyword in search box.", Alert.AlertType.warning);
                return;
            }
            clsLookUp.search(txtEmployeeID.Text, txtFirstName.Text, txtLastName.Text, dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoansDataEntry loanDataEntry = new LoansDataEntry ();

            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(LoansDataEntry))
                {
                    //===============================================================================
                    //                      If form is already open
                    //===============================================================================
                    form.Activate();
                    loanDataEntry = (LoansDataEntry)Application.OpenForms["LoansDataEntry"];
                    Classes.clsLoanDataEntry.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());

                    //===============================================================================
                    //                  IF SELECTED MEMBER IS A DEPENDENT
                    //===============================================================================
                    if(dataGridView1.SelectedRows[0].Cells["Principal"].Value.ToString() == "False")
                    {
                        loanDataEntry.txtPrincipal.Text = clsLookUp.returnPrincipal(dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString());
                    }
                    
                    loanDataEntry.txtEmployeeID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                    loanDataEntry.txtName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();
                   
                    if (dataGridView1.SelectedRows[0].Cells["Salary"].Value.ToString() != "")
                    {

                        loanDataEntry.txtSalary.Text = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Salary"].Value).ToString("#,0.00");
                    }

                    loanDataEntry.txtDateHired.Text = Convert.ToString(Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Hired"].Value.ToString()).ToShortDateString());
                    loanDataEntry.txtMemberShipDate.Text = Convert.ToString(Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Of_Membership"].Value.ToString()).ToShortDateString());
                    
                    try
                    {
                        loanDataEntry.txtPMSDate.Text = Convert.ToString(Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Of_PMS"].Value.ToString()).ToShortDateString());
                    }
                    catch
                    {

                    }

                    //For Getting the years in service
                    var yrsDate = new DateTime();
                    int yrsFrmDateHire,yrsToday,total;
                    yrsDate = Convert.ToDateTime(loanDataEntry.txtDateHired.Text);
                    yrsFrmDateHire = Convert.ToInt32(yrsDate.Date.Year.ToString());
                    yrsToday = Convert.ToInt32(DateTime.Today.Year.ToString());
                    total = yrsToday - yrsFrmDateHire;
                    loanDataEntry.txtYrsInService.Text = total.ToString(); 

                    loanDataEntry.txtCompany.Text = clsLoanDataEntry.returnCompanyDescription(Classes.clsLoanDataEntry.userID);


                    //Share Capital Balance
                    loanDataEntry.txtShareCapitalBalance.Text = clsLoanDataEntry.returnShareCapital(Classes.clsLoanDataEntry.userID).ToString("#,0.00");

                    //Disable If Company is Non payroll
                    if(clsLookUp.returnCompanyCode(loanDataEntry.txtCompany.Text) == "COMP010")
                    {
                        //NON-PAYROLL
                        loanDataEntry.button1.Enabled = false;
                    }
                    else
                    {
                        loanDataEntry.button1.Enabled = true;
                    }

                    //Clear details value first

                    foreach (var c in loanDataEntry.panel11.Controls)
                    {
                        if (c is TextBox) ((TextBox)c).Text = String.Empty;
                        if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
                    }

                    if (loanDataEntry.txtCompany.Text == "NON PAYROLL")
                    {
                        loanDataEntry.comboBox1.SelectedIndex = 1;
                        loanDataEntry.comboBox1.Enabled = false;
                    } 
                    this.Close();
                    return;
                }
            }
            Classes.clsLoanDataEntry.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());

            //===============================================================================
            //                  IF SELECTED MEMBER IS A DEPENDENT
            //===============================================================================
            if (dataGridView1.SelectedRows[0].Cells["Principal"].Value.ToString() == "False")
            {
                loanDataEntry.txtPrincipal.Text = clsLookUp.returnPrincipal(dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString());
            }

            loanDataEntry.txtEmployeeID.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
            loanDataEntry.txtName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();
            if (dataGridView1.SelectedRows[0].Cells["Salary"].Value.ToString() != "")
            {

                loanDataEntry.txtSalary.Text = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Salary"].Value).ToString("#,0.00");
            }
            loanDataEntry.txtDateHired.Text = Convert.ToString(Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Hired"].Value.ToString()).ToShortDateString());
            loanDataEntry.txtMemberShipDate.Text = Convert.ToString(Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Of_Membership"].Value.ToString()).ToShortDateString());
            loanDataEntry.txtPMSDate.Text = Convert.ToString(Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Date_Of_PMS"].Value.ToString()).ToShortDateString());
            loanDataEntry.txtCompany.Text = clsLoanDataEntry.returnCompanyDescription(Classes.clsLoanDataEntry.userID);
            this.Close();
        }

        private void txtLastName_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Space);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
