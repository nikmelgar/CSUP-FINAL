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
    public partial class searchDisbursement : Form
    {
        public searchDisbursement()
        {
            InitializeComponent();
        }

        //=======================================================
        //              DECLARATION
        //=======================================================

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();
        Classes.clsSearchDisbursement clsDisbursement = new Classes.clsSearchDisbursement();
        Classes.clsOpenTransaction clsOpen = new Classes.clsOpenTransaction();
        //=======================================================
        //              MOVEABLE PANEL
        //=======================================================

        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

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

        private void panel1_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(searchDisbursement))
                {
                    form.Activate();
                    return;
                }
            }

            searchDisbursement frm = new searchDisbursement();
            frm.Show();
        }

        private void searchDisbursement_Load(object sender, EventArgs e)
        {
            clsDisbursement.loadDefaultDisbursement(dataGridView1);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            clsDisbursement.searchCV(dataGridView1, txtCVNo, txtCVNo,lblError);
        }

        private void txtCVNo_TextChanged(object sender, EventArgs e)
        {
            clsDisbursement.searchCV(dataGridView1, txtCVNo, txtCVNo, lblError);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtCVNo.Text = "";
            clsDisbursement.loadDefaultDisbursement(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                
                DisbursementVoucher cv = new DisbursementVoucher();

                cv = (DisbursementVoucher)Application.OpenForms["DisbursementVoucher"];


                if (cv.txtCVNo.Text != "")
                {
                    clsOpen.deleteTransaction("Disbursement Voucher", cv.txtCVNo.Text);
                }

                if (clsOpen.checkOpenFormsAndTransaction("Disbursement Voucher", dataGridView1.SelectedRows[0].Cells["CV_No"].Value.ToString()) == true)
                {
                    //Messagebox here for open form with user whos using the form and reference
                    Alert.show(clsOpen.returnUserOnlineAndReference("Disbursement Voucher", dataGridView1.SelectedRows[0].Cells["CV_No"].Value.ToString(), "Disbursement Voucher"), Alert.AlertType.error);
                    return;
                }
                else
                {
                    //Insert here for register the open form and reference
                    clsOpen.insertTransaction("Disbursement Voucher", dataGridView1.SelectedRows[0].Cells["CV_No"].Value.ToString());
                }


                //=========================================================================================
                //                              Header Information
                //=========================================================================================

                cv.txtCVNo.Text = dataGridView1.SelectedRows[0].Cells["CV_No"].Value.ToString();
                cv.dtCVDate.Text = dataGridView1.SelectedRows[0].Cells["CV_Date"].Value.ToString();


                if(dataGridView1.SelectedRows[0].Cells["Payee_Type"].Value.ToString() == "True")
                {
                    cv.radioClient.Checked = true;
                }
                else
                {
                    cv.radioMember.Checked = true;
                }
                cv.txtPayee.Text = dataGridView1.SelectedRows[0].Cells["Payee"].Value.ToString();

                if (dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString() != "" || dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString() != string.Empty)
                {
                    Classes.clsDisbursement.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                    cv.txtPayeeName.Text = clsDisbursement.fullName(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
                }
                else
                {
                    cv.txtPayeeName.Text = clsDisbursement.ClientName(dataGridView1.SelectedRows[0].Cells["Payee"].Value.ToString());
                }
                    
                

                cv.txtLoanNo.Text = dataGridView1.SelectedRows[0].Cells["Loan_No"].Value.ToString();
                cv.cmbTransaction.SelectedValue = dataGridView1.SelectedRows[0].Cells["Transaction_Type"].Value.ToString();
                cv.cmbBank.SelectedValue = dataGridView1.SelectedRows[0].Cells["Bank_Code"].Value.ToString();
                cv.txtChequeNo.Text = dataGridView1.SelectedRows[0].Cells["Check_No"].Value.ToString();
                cv.dtChequeDate.Text = dataGridView1.SelectedRows[0].Cells["Check_Date"].Value.ToString();
                cv.txtAmount.Text = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Amount"].Value).ToString("#,0.00");
                cv.txtParticular.Text = dataGridView1.SelectedRows[0].Cells["Particulars"].Value.ToString();

                //=========================================================================================
                //                              Footer Information
                //=========================================================================================

                cv.txtPreparedBy.Text = dataGridView1.SelectedRows[0].Cells["Prepared_By"].Value.ToString();
                cv.txtPostedBy.Text = dataGridView1.SelectedRows[0].Cells["Posted_By"].Value.ToString();
                cv.txtCancelledBy.Text = dataGridView1.SelectedRows[0].Cells["Cancelled_By"].Value.ToString();
                cv.txtAuditedBy.Text = dataGridView1.SelectedRows[0].Cells["Audited_By"].Value.ToString();

                //=========================================================================================
                //                              Status Information
                //=========================================================================================

                if (dataGridView1.SelectedRows[0].Cells["Posted"].Value.ToString() == "True")
                {
                    if(dataGridView1.SelectedRows[0].Cells["Released_By"].Value.ToString() != "")
                    {
                        cv.status.Visible = true;
                        cv.status.Text = "POSTED and RELEASED";
                    }
                    else
                    {
                        cv.status.Visible = true;
                        cv.status.Text = "POSTED";
                    }
                   
                }
                else if (dataGridView1.SelectedRows[0].Cells["Cancelled"].Value.ToString() == "True")
                {
                    cv.status.Visible = true;
                    cv.status.Text = "CANCELLED";
                }
                else
                {
                    cv.status.Visible = false;
                    cv.status.Text = "";
                }

                //=========================================================================================
                //                              Details Information
                //=========================================================================================

                cv.dataGridView1.Rows.Clear();
                clsDisbursement.loadDetails(cv.dataGridView1, cv.txtCVNo.Text);

                //=========================================================================================
                //                              Compute
                //=========================================================================================
                clsDisbursement.loadTotalDebitCredit(cv.txtDebit, cv.txtCredit, cv.txtCVNo.Text);


                //=========================================================================================
                //                              Enable Buttons
                //=========================================================================================
                cv.btnEdit.Enabled = true;
                cv.btnPost.Enabled = true;
                cv.btnCancel.Enabled = true;
                cv.btnPrint.Enabled = true;
                cv.btnPrintCheque.Enabled = true;
                cv.btnRelease.Enabled = true;
                cv.btnAuditted.Enabled = true;

                //CLOSE AFTER SELECTION OF JOURNAL VOUCHER
                this.Close();
            }
        }
    }
}
