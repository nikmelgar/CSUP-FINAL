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
    public partial class JournalMemberClientLookUp : Form
    {
        public JournalMemberClientLookUp()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        Global global = new Global();
        Classes.clsJournalMemberClient clsJournalMemberClient = new Classes.clsJournalMemberClient();
        Classes.clsJournalVoucher clsJournalVoucher = new Classes.clsJournalVoucher();
        private void JournalMemberClientLookUp_Load(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2; //Make Default Page [Members]
            clsJournalMemberClient.loadLookUpQuery("Membership Where IsActive = '1' and IsApprove ='1'", dataGridView1);

        }

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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1)
            {
                //Client Code Search
                if (txtClientID.Text != "" || txtName.Text != "")
                {
                    clsJournalMemberClient.SearchClient(dataGridView1, txtClientID.Text, txtName.Text);
                }
                else
                {
                    Alert.show("No keywords to be search!", Alert.AlertType.warning);
                    return;
                }
            }
            else
            {
                //Member Code Search 
                
                if(txtEmployeeID.Text != "" || txtFirstName.Text != "" || txtLastName.Text != "")
                {
                    clsJournalMemberClient.SearchMember(txtEmployeeID.Text, txtFirstName.Text, txtLastName.Text, dataGridView1);
                }
                else
                {
                    Alert.show("No keywords to be search!", Alert.AlertType.warning);
                    return;
                }

            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedTab == tabPage1)
            {
                clsJournalMemberClient.loadClient(dataGridView1);
            }
            else
            {
                clsJournalMemberClient.loadLookUpQuery("Membership Where IsActive = '1' and IsApprove ='1'", dataGridView1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Clear Members Field
            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";

            //Clear Client Field
            txtClientID.Text = "";
            txtName.Text = "";

            if(tabControl1.SelectedTab == tabPage1)
            {
                //Refresh
                clsJournalMemberClient.loadClient(dataGridView1);
            }
            else
            {
                clsJournalMemberClient.loadLookUpQuery("Membership Where IsActive = '1' and IsApprove ='1'", dataGridView1);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(tabControl1.SelectedTab == tabPage1)
            {
                //Client Page
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
                        LoanLookUpProcess.clsLoanLookUpMember.userid = 0; //no record
                        jv.txtMember.Text = dataGridView1.SelectedRows[0].Cells["Client_Code"].Value.ToString();
                        jv.txtName.Text = dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString();

                        //clear loan first
                        jv.txtLoanNumber.Text = "";
                        jv.txtLoanType.Text = "";

                        this.Close();
                        return;
                    }
                }
                LoanLookUpProcess.clsLoanLookUpMember.userid = 0; //no record
                jv.txtMember.Text = dataGridView1.SelectedRows[0].Cells["Client_Code"].Value.ToString();
                jv.txtName.Text = dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString();

                //clear loan first
                jv.txtLoanNumber.Text = "";
                jv.txtLoanType.Text = "";

                jv.Show();
                this.Close();
            }
            else
            {
                //Members Page
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
                        Classes.clsJournalVoucher.userId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                        //Adding to get the loan of member
                        LoanLookUpProcess.clsLoanLookUpMember.userid = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                        jv.txtMember.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                        jv.txtName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();

                        //clear loan first
                        jv.txtLoanNumber.Text = "";
                        jv.txtLoanType.Text = "";

                        this.Close();
                        return;
                    }
                }

                Classes.clsJournalVoucher.userId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                //Adding to get the loan of member
                LoanLookUpProcess.clsLoanLookUpMember.userid = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                jv.txtMember.Text = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                jv.txtName.Text = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();

                //clear loan first
                jv.txtLoanNumber.Text = "";
                jv.txtLoanType.Text = "";

                jv.Show();
                this.Close();
            }
        }
    }
}
