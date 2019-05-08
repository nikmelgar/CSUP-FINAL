using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2
{
    public partial class Loans : Form
    {
        public Loans()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        bool x;
        Classes.clsLoan clsLoan = new Classes.clsLoan();
        Classes.clsLoanLookUp clsLookUp = new Classes.clsLoanLookUp();
        Classes.clsLoanDataEntry clsLoanDataEntry = new Classes.clsLoanDataEntry();
        Global global = new Global();

        //==================
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;

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

        private void label1_Click(object sender, EventArgs e)
        {
            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(LoansDataEntry))
                {
                    form.Activate();
                    return;
                }
            }

            LoansDataEntry frm = new LoansDataEntry();
            frm.btnClose.Text = "CANCEL";
            frm.Show();
        }

        private void Loans_Load(object sender, EventArgs e)
        {
            clsLoan.loadLoanDefault(dataGridView1);

            //DASHBOARD
            lblApproved.Text = clsLoan.loadLoansApproved();
            lblDisapproved.Text = clsLoan.loadLoansDisapproved();
            lblForApproval.Text = clsLoan.loadLoansForApproval();
            lblReleased.Text = clsLoan.loadLoansReleased();

        }

        public void refreshData()
        {
            clsLoan.loadLoanDefault(dataGridView1);
        }
        private void panel1_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Loans))
                {
                    form.Activate();
                    return;
                }
            }

            Loans frm = new Loans();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                //No Data to be edit
                Alert.show("Please select you want to edit.", Alert.AlertType.warning);
                return;
            }

            //show form
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(LoansDataEntry))
                {
                    form.Activate();
                    x = true;
                }
                else
                {
                    x = false;
                }
            }

            LoansDataEntry loansDataEntry = new LoansDataEntry();

            if (x != true)
            {
                loansDataEntry.Show();
            }

            loansDataEntry = (LoansDataEntry)Application.OpenForms["LoansDataEntry"];

            //=============================================================================
            //                      MEMBERS INFORMATION
            //=============================================================================
            Classes.clsLoanDataEntry.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanMembersInformation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userID", Classes.clsLoanDataEntry.userID);

                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    loansDataEntry.txtEmployeeID.Text = dt.Rows[0].ItemArray[1].ToString();

                    if (dt.Rows[0].ItemArray[2].ToString() == "False")
                    {
                        loansDataEntry.txtPrincipal.Text = clsLookUp.returnPrincipal(dt.Rows[0].ItemArray[1].ToString());
                    }

                    loansDataEntry.txtName.Text = dt.Rows[0].ItemArray[3].ToString();

                    if (dt.Rows[0].ItemArray[4].ToString() != "")
                    {
                        loansDataEntry.txtSalary.Text = Convert.ToDecimal(dt.Rows[0].ItemArray[4].ToString()).ToString("#,0.00");
                    }

                    loansDataEntry.txtDateHired.Text = Convert.ToString(Convert.ToDateTime(dt.Rows[0].ItemArray[5].ToString()).ToShortDateString());
                    loansDataEntry.txtMemberShipDate.Text = Convert.ToString(Convert.ToDateTime(dt.Rows[0].ItemArray[6].ToString()).ToShortDateString());

                    try
                    {
                        loansDataEntry.txtPMSDate.Text = Convert.ToString(Convert.ToDateTime(dt.Rows[0].ItemArray[7].ToString()).ToShortDateString());
                    }

                    catch
                    {

                    }


                    //For Getting the years in service
                    var yrsDate = new DateTime();
                    int yrsFrmDateHire, yrsToday, total;
                    yrsDate = Convert.ToDateTime(loansDataEntry.txtDateHired.Text);
                    yrsFrmDateHire = Convert.ToInt32(yrsDate.Date.Year.ToString());
                    yrsToday = Convert.ToInt32(DateTime.Today.Year.ToString());
                    total = yrsToday - yrsFrmDateHire;
                    loansDataEntry.txtYrsInService.Text = total.ToString();

                    loansDataEntry.txtCompany.Text = clsLoanDataEntry.returnCompanyDescription(Classes.clsLoanDataEntry.userID);

                    //Share Capital Balance
                    loansDataEntry.txtShareCapitalBalance.Text = clsLoanDataEntry.returnShareCapital(Classes.clsLoanDataEntry.userID).ToString("#,0.00");
                }

                //==================================================================================================================
                //                              END MEMBERS INFORMATION DETAIL
                //==================================================================================================================

                //============================================================
                //              DETAILS OF LOAN
                //============================================================

                loansDataEntry.txtLoanNo.Text = dataGridView1.SelectedRows[0].Cells["Loan_No"].Value.ToString();

                loansDataEntry.txtTermsInMonth.Text = dataGridView1.SelectedRows[0].Cells["Terms"].Value.ToString();
                loansDataEntry.txtMonthlyAmort.Text = Convert.ToString(Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Monthly_Amort"].Value.ToString()).ToString("#,0.00"));
                loansDataEntry.txtSemiMonthlyAmort.Text = Convert.ToString(Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Semi_Monthly_Amort"].Value.ToString()).ToString("#,0.00"));
                loansDataEntry.comboBox1.Text = dataGridView1.SelectedRows[0].Cells["Payment_Option"].Value.ToString();
                loansDataEntry.comboBox1.Enabled = false;
                loansDataEntry.cmbReleaseOption.Text = dataGridView1.SelectedRows[0].Cells["ReleaseOption"].Value.ToString();
                loansDataEntry.cmbReleaseOption.Enabled = false;

                //============================================================
                //              END DETAILS OF LOAN
                //============================================================

                //============================================================
                //              CO MAKERS DETAILS
                //============================================================
                loansDataEntry.dataGridView1.Rows.Clear();

                adapter = new SqlDataAdapter("select * FROM vw_CoMakers WHERE Loan_No = '" + dataGridView1.SelectedRows[0].Cells["Loan_No"].Value.ToString() + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count >= 1)
                {
                    loansDataEntry.dataGridView1.Rows.Add(dt.Rows.Count);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        loansDataEntry.dataGridView1.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[1].ToString(); //UserID of CO Makers
                        loansDataEntry.dataGridView1.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[2].ToString();
                        loansDataEntry.dataGridView1.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[3].ToString();
                        loansDataEntry.dataGridView1.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[4].ToString();
                        loansDataEntry.dataGridView1.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[5].ToString();
                    }

                    loansDataEntry.lblTotalCntMakers.Text = dt.Rows.Count.ToString();
                }

                //============================================================
                //              END CO MAKERS DETAILS
                //============================================================

                //===================================
                //               FOOTER
                //===================================
                loansDataEntry.txtDateEncoded.Text = Convert.ToString(Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Encoded_Date"].Value).ToString("MM/dd/yyyy"));
                loansDataEntry.txtEncodedBy.Text = dataGridView1.SelectedRows[0].Cells["Encoded_By"].Value.ToString();

                //===================================
                //           STATUS
                //===================================
                loansDataEntry.status.Visible = true;
                loansDataEntry.status.Text = dataGridView1.SelectedRows[0].Cells["status_description"].Value.ToString();

                /*
                * IF STATUS IS CANCELLED THEN DISPLAY THE CANCELLED NOTE
                */

                if(dataGridView1.SelectedRows[0].Cells["Status"].Value.ToString() == "7")
                {
                    loansDataEntry.txtCancel.Visible = true;
                    loansDataEntry.lblReason.Visible = true;
                    loansDataEntry.txtCancel.Text = dataGridView1.SelectedRows[0].Cells["Note"].Value.ToString();
                }
                else
                {
                    loansDataEntry.txtCancel.Visible = false;
                    loansDataEntry.lblReason.Visible = false;
                }


                //If Approve BtnForward visible = false and Cancel will be visible
                if (dataGridView1.SelectedRows[0].Cells["status_description"].Value.ToString() == "APPROVED")
                {
                    loansDataEntry.btnForward.Visible = false;
                    loansDataEntry.btnCancel.Visible = true;
                    loansDataEntry.lblReason.Visible = true; //Label Reason
                    loansDataEntry.txtCancel.Visible = true; //Text reason
                }
                else if (dataGridView1.SelectedRows[0].Cells["status_description"].Value.ToString() == "FOR APPROVAL" || dataGridView1.SelectedRows[0].Cells["status_description"].Value.ToString() == "FBA")
                {
                    loansDataEntry.btnForward.Visible = true;
                    loansDataEntry.btnCancel.Visible = false;
                    loansDataEntry.lblReason.Visible = false; //Label Reason
                    loansDataEntry.txtCancel.Visible = false;
                }


                if (dataGridView1.SelectedRows[0].Cells["Status"].Value.ToString() == "1" || dataGridView1.SelectedRows[0].Cells["Status"].Value.ToString() == "6")
                {
                    //1 = FOR APPROVAL
                    loansDataEntry.btnSave.Text = "EDIT";
                    loansDataEntry.btnClose.Text = "CLOSE";
                    loansDataEntry.cmbLoanType.Enabled = false;
                    loansDataEntry.btnSearch.Enabled = false;
                    loansDataEntry.txtLoanAmount.Enabled = false;
                    loansDataEntry.txtTermsInMonth.Enabled = false;

                    //Put the Loan Amount to Class First
                    Classes.clsLoanDataEntry.loan_amount = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Loan_Amount"].Value.ToString());

                }
                else if (dataGridView1.SelectedRows[0].Cells["Status"].Value.ToString() == "2" || dataGridView1.SelectedRows[0].Cells["Status"].Value.ToString() == "3" || dataGridView1.SelectedRows[0].Cells["Status"].Value.ToString() == "5" || dataGridView1.SelectedRows[0].Cells["Status"].Value.ToString() == "7" || dataGridView1.SelectedRows[0].Cells["Status"].Value.ToString() == "8")
                {
                    //2 = APPROVED 3 = DISAPPROVED 5 = RELEASED 7 = CANCELLED
                    loansDataEntry.btnSave.Text = "NEW";
                    loansDataEntry.btnClose.Text = "CLOSE";
                    loansDataEntry.cmbLoanType.Enabled = false;
                    loansDataEntry.btnSearch.Enabled = false;
                    loansDataEntry.txtLoanAmount.Enabled = false;
                    loansDataEntry.txtTermsInMonth.Enabled = false;
                }


                //Loan Details
                loansDataEntry.cmbLoanType.SelectedValue = dataGridView1.SelectedRows[0].Cells["Type"].Value.ToString();
                loansDataEntry.txtLoanAmount.Text = Convert.ToString(Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Loan_Amount"].Value.ToString()).ToString("#,0.00"));

                //Disable If Company is Non payroll
                if (clsLookUp.returnCompanyCode(loansDataEntry.txtCompany.Text) == "COMP010")
                {
                    //NON-PAYROLL
                    loansDataEntry.button1.Enabled = false;
                }
                else
                {
                    loansDataEntry.button1.Enabled = true;
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button2_Click(sender, e);
        }

        private void lblForApprovalDetails_Click(object sender, EventArgs e)
        {
            clsLoan.loadForApprovalDetails(dataGridView1);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            clsLoan.loadLoanDefault(dataGridView1);

            //DASHBOARD
            lblApproved.Text = clsLoan.loadLoansApproved();
            lblDisapproved.Text = clsLoan.loadLoansDisapproved();
            lblForApproval.Text = clsLoan.loadLoansForApproval();
            lblReleased.Text = clsLoan.loadLoansReleased();

            txtEmployeeID.Text = "";
            txtLoanNo.Text = "";
            txtName.Text = "";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            clsLoan.SearchLoan(dataGridView1, txtLoanNo, txtName, txtEmployeeID);
        }

        private void lblApprovedDetails_Click(object sender, EventArgs e)
        {
            clsLoan.loadLoanApprovedDetails(dataGridView1);
        }

        private void lblDisapprovedDetails_Click(object sender, EventArgs e)
        {
            clsLoan.loadLoanDisapprovedDetails(dataGridView1);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            clsLoan.loadLoanReleasedDetails(dataGridView1);
        }
    }
}
