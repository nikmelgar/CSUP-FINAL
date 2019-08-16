using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace WindowsFormsApplication2
{
    public partial class searchCashReceipt : Form
    {
        public searchCashReceipt()
        {
            InitializeComponent();
        }

        //=================================================
        //                  SQL CONNECTION
        //=================================================
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        //=================================================
        //             Calling Class and FOrms
        //=================================================
        Global global = new Global();
        Classes.clsSearchCashReceipt clsSearchCash = new Classes.clsSearchCashReceipt();
        Classes.clsCashReceipt clsCash = new Classes.clsCashReceipt();
        Classes.clsOpenTransaction clsOpen = new Classes.clsOpenTransaction();
        //=================================================
        //             Panel Mouse Move
        //=================================================
        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void searchCashReceipt_Load(object sender, EventArgs e)
        {
            clsSearchCash.loadDefaultCashReceipts(dataGridView1);
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

        private void txtOrNo_TextChanged(object sender, EventArgs e)
        {
            clsSearchCash.searchOR(dataGridView1, txtOrNo, txtOrNo, lblError);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            clsSearchCash.searchOR(dataGridView1, txtOrNo, txtOrNo, lblError);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtOrNo.Text = "";
            clsSearchCash.loadDefaultCashReceipts(dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                CashReceiptVoucher or = new CashReceiptVoucher();

                or = (CashReceiptVoucher)Application.OpenForms["CashReceiptVoucher"];

                if (or.txtORNo.Text != "")
                {
                    clsOpen.deleteTransaction("Receipt Voucher", or.txtORNo.Text);
                }

                if (clsOpen.checkOpenFormsAndTransaction("Receipt Voucher", dataGridView1.SelectedRows[0].Cells["Or_No"].Value.ToString()) == true)
                {
                    //Messagebox here for open form with user whos using the form and reference
                    Alert.show(clsOpen.returnUserOnlineAndReference("Receipt Voucher", dataGridView1.SelectedRows[0].Cells["Or_No"].Value.ToString(), "Receipt Voucher"), Alert.AlertType.error);
                    return;
                }
                else
                {
                    //Insert here for register the open form and reference
                    clsOpen.insertTransaction("Receipt Voucher", dataGridView1.SelectedRows[0].Cells["Or_No"].Value.ToString());
                }

                //=========================================================================================
                //                              Header Information
                //=========================================================================================

                Classes.clsCashReceipt.userID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                or.txtORNo.Text = dataGridView1.SelectedRows[0].Cells["Or_No"].Value.ToString();
                or.dtOrDate.Text = dataGridView1.SelectedRows[0].Cells["Or_Date"].Value.ToString();

                if (dataGridView1.SelectedRows[0].Cells["Payor_Type"].Value.ToString() == "True")
                {
                    or.radioClient.Checked = true;
                    //Return Company Name According to ID
                    or.txtPayorName.Text = clsSearchCash.returnClientName(dataGridView1.SelectedRows[0].Cells["Payor"].Value.ToString());
                }
                else
                {
                    or.radioMember.Checked = true;
                    //Return Member Name According to ID
                    or.txtPayorName.Text = clsSearchCash.returnMembersName(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                    or.txtPayorCompany.Text = clsCash.returnCompanyDescription(clsSearchCash.GetCompanyPerMember(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
                }

                or.txtPayorID.Text = dataGridView1.SelectedRows[0].Cells["Payor"].Value.ToString();
                or.txtParticulars.Text = dataGridView1.SelectedRows[0].Cells["Particulars"].Value.ToString();

                //=========================================================================================
                //                      COLLECTION TYPE
                //=========================================================================================
                //0 = cash
                //1 = pecci check
                //2 = non-pecci check
                if (dataGridView1.SelectedRows[0].Cells["Collection_Type"].Value.ToString() == "0")
                {
                    or.radioCash.Checked = true;
                }
                else if (dataGridView1.SelectedRows[0].Cells["Collection_Type"].Value.ToString() == "1")
                {
                    or.radioPecciCheck.Checked = true;
                }
                else
                {
                    or.radioNonPecciCheck.Checked = true;
                }

                //=========================================================================================
                //                      Transaction Header
                //=========================================================================================
                clsSearchCash.loadTransaction(or.datagridviewTransaction, dataGridView1.SelectedRows[0].Cells["Or_No"].Value.ToString());


                //=========================================================================================
                //                      Bank Details
                //=========================================================================================
                if (dataGridView1.SelectedRows[0].Cells["Collection_Type"].Value.ToString() == "1" || dataGridView1.SelectedRows[0].Cells["Collection_Type"].Value.ToString() == "2")
                {
                    clsSearchCash.loadBanksCheck(or.dgvChecks, dataGridView1.SelectedRows[0].Cells["Or_No"].Value.ToString());

                    //Disable Bank Grid and buttons
                    or.dgvChecks.Enabled = false;
                    or.btnAddCheck.Enabled = false;
                    or.btnRemoveCheck.Enabled = false;

                }
                else
                {
                    or.dgvChecks.Rows.Clear();
                }


                //=========================================================================================
                //                      Cash Receipt Details
                //=========================================================================================

                clsSearchCash.loadCashReceiptsDetails(or.dataGridView3, dataGridView1.SelectedRows[0].Cells["Or_No"].Value.ToString());


                //=========================================================================================
                //                      Cash Receipt Footer
                //=========================================================================================
                or.txtPostedBy.Text = dataGridView1.SelectedRows[0].Cells["Posted_By"].Value.ToString();
                or.txtCancelledBy.Text = dataGridView1.SelectedRows[0].Cells["Cancelled_By"].Value.ToString();

                if(dataGridView1.SelectedRows[0].Cells["Posted"].Value.ToString() == "True" || dataGridView1.SelectedRows[0].Cells["Posted"].Value.ToString() == "1")
                {
                    or.status.Visible = true;
                    or.status.Text = "POSTED";
                }
                else if(dataGridView1.SelectedRows[0].Cells["Cancelled"].Value.ToString() == "True" || dataGridView1.SelectedRows[0].Cells["Cancelled"].Value.ToString() == "1")
                {
                    or.status.Visible = true;
                    or.status.Text = "CANCELLED";
                }
                else
                {
                    or.status.Visible = false;
                }


                //Location of OR as Per Maam Diane Request
                if(dataGridView1.SelectedRows[0].Cells["Location"].Value.ToString() == "PEREA")
                {
                    or.radioLocPerea.Checked = true;
                }
                else
                {
                    or.radioLocTeltech.Checked = true;
                }

                //Put Prepared By
                or.txtPreparedBy.Text = dataGridView1.SelectedRows[0].Cells["Prepared_By"].Value.ToString();
                or.txtAuditedBy.Text = dataGridView1.SelectedRows[0].Cells["Audited_By"].Value.ToString();

                //Enable Commands
                or.btnEdit.Enabled = true;
                or.btnPost.Enabled = true;
                or.btnCancel.Enabled = true;
                or.btnAuditted.Enabled = true;
                //Sorting
                or.dataGridView3.Sort(or.dataGridView3.Columns["Debit"], ListSortDirection.Descending);
                this.Close();
            }
        }

        private void searchCashReceipt_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
