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
    public partial class searchCoMaker : Form
    {
        public searchCoMaker()
        {
            InitializeComponent();
        }

        bool alreadyUsed = false;
        private bool m_firstClick = false;
        private Point m_firstClickLoc;
    
        public static string getLoanType { get; set; }

        Classes.clsSearchCoMaker clsSearchComaker = new Classes.clsSearchCoMaker();
        Classes.clsLoanDataEntry clsLoanDataEntry = new Classes.clsLoanDataEntry();
        Classes.clsLoanLookUp clsLookUp = new Classes.clsLoanLookUp();
        Classes.clsParameter clsParameter = new Classes.clsParameter();

        private void searchCoMaker_Load(object sender, EventArgs e)
        {
            clsSearchComaker.loaddefaultitems(dataGridView1);
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

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtEmployeeID.Text == "" && txtLastName.Text == "" && txtFirstName.Text == "")
            {
                Alert.show("Please enter valid Keyword.", Alert.AlertType.warning);
                return;
            }
            clsSearchComaker.search(txtEmployeeID.Text, txtFirstName.Text, txtLastName.Text, dataGridView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            clsSearchComaker.loaddefaultitems(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // dataGridView1.Rows[selectedRow].Cells[selected].Value = dataGridView3.SelectedRows[0].Cells["EmployeeID"].Value.ToString() + " - " + dataGridView3.SelectedRows[0].Cells["Name"].Value.ToString();

            LoansDataEntry loanDataEntry = new LoansDataEntry();

            loanDataEntry = (LoansDataEntry)Application.OpenForms["LoansDataEntry"];
            //Disable If Company is Non payroll
            if (clsLookUp.returnCompanyCode(loanDataEntry.txtCompany.Text) != "COMP010")
            {
                //NOT NON-PAYROLL
                //Validation For Co Makers
                if (clsLoanDataEntry.isShortTerm(loanDataEntry.cmbLoanType.SelectedValue.ToString()) == true)
                {
                    if (loanDataEntry.dataGridView1.Rows.Count >= 6)
                    {
                        Alert.show("Already exceeds the No. of required Co-Makers.", Alert.AlertType.error);
                        return;
                    }

                }
                else
                {
                    //Long Term
                    if(loanDataEntry.txtLoanAmount.Text != "" && loanDataEntry.txtTermsInMonth.Text != "")
                    {
                        string str = loanDataEntry.txtLoanAmount.Text;
                        str = str.Replace(",", "");
                        decimal answer;
                        double ttalCO = Convert.ToDouble(loanDataEntry.txtLoanAmount.Text) / 500000.00;
                        answer = Convert.ToDecimal(ttalCO);

                        if ((answer % 1) > 0)
                        {
                            //is decimal
                            answer = clsLoanDataEntry.GetDot(Convert.ToString(ttalCO)) * 6;
                        }
                        else
                        {
                            answer = answer * 6;
                        }

                        if (loanDataEntry.dataGridView1.Rows.Count >= answer)
                        {
                            Alert.show("Already exceeds the No. of required Co-Makers.", Alert.AlertType.error);
                            return;
                        }
                    }
                    else
                    {
                        Alert.show("Please fill up Loan amount and Loan term in months.", Alert.AlertType.error);
                        return;
                    }



                    //For RPL Loans must be 10 years above 

                    if(clsParameter.checkForRPLloans(getLoanType) == true)
                    {
                        //Selected loan type is RPL 
                        if(clsParameter.check10yearsAbove(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString())) == false)
                        {
                            //Must be 10 years above
                            Alert.show("Co-maker should be 10 years above in the company.", Alert.AlertType.error);
                            return;
                        }
                    }
                }
            }
            
            //==========================================================================================

            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(LoansDataEntry))
                {
                    //===============================================================================
                    //                      If form is already open
                    //===============================================================================
                    form.Activate();
                    loanDataEntry = (LoansDataEntry)Application.OpenForms["LoansDataEntry"];

                    //check first if this member is already been a co maker in his/her list
                    
                    for(int i = 0; i < loanDataEntry.dataGridView1.Rows.Count; i++)
                    {
                        if(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString() == loanDataEntry.dataGridView1.Rows[i].Cells[0].Value.ToString())
                        {
                            alreadyUsed = true;
                        }
                    }

                    if (Classes.clsLoanDataEntry.userID.ToString() == dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString())
                    {
                        Alert.show("You cannot be a Co-Maker to your own loan.", Alert.AlertType.error);
                        return;
                    }

                    if (alreadyUsed == true)
                    {
                        Alert.show("Member already used as co-maker.", Alert.AlertType.error);
                        alreadyUsed = false;
                        return;
                    }

                    var index = loanDataEntry.dataGridView1.Rows.Add();
                    loanDataEntry.dataGridView1.Rows[index].Cells[0].Value = dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString();
                    loanDataEntry.dataGridView1.Rows[index].Cells[1].Value = dataGridView1.SelectedRows[0].Cells["EmployeeID"].Value.ToString();
                    loanDataEntry.dataGridView1.Rows[index].Cells[2].Value = dataGridView1.SelectedRows[0].Cells["LastName"].Value.ToString() + ", " + dataGridView1.SelectedRows[0].Cells["FirstName"].Value.ToString() + " " + dataGridView1.SelectedRows[0].Cells["MiddleName"].Value.ToString() + dataGridView1.SelectedRows[0].Cells["Suffix"].Value.ToString();
                    loanDataEntry.dataGridView1.Rows[index].Cells[3].Value = dataGridView1.SelectedRows[0].Cells["Cellphone_No"].Value.ToString();
                    loanDataEntry.dataGridView1.Rows[index].Cells[4].Value = dataGridView1.SelectedRows[0].Cells["Residential_Address"].Value.ToString();

                    loanDataEntry.lblTotalCntMakers.Text = loanDataEntry.dataGridView1.Rows.Count.ToString();
                    //return to false
                    alreadyUsed = false;
                    return;
                }
            }
        
            this.Close();
        }

        private void txtLastName_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Space);
        }
    }
}
