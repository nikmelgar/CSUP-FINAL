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
    public partial class searchJournal : Form
    {
        public searchJournal()
        {
            InitializeComponent();
        }

        //================================================================
        //              DECLARATION
        //================================================================

        SqlConnection con;
        Global global = new Global();


        Classes.clsSearchJournal clsSearchJournal = new Classes.clsSearchJournal();

        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void searchJournal_Load(object sender, EventArgs e)
        {
            clsSearchJournal.loadDefaultJV(dataGridView1);
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


                if (form.GetType() == typeof(searchJournal))
                {
                    form.Activate();
                    return;
                }
            }

            searchJournal frm = new searchJournal();
            frm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clsSearchJournal.loadDefaultJV(dataGridView1);
            txtJVNo.Text = "";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            clsSearchJournal.searchJV(dataGridView1, txtJVNo, txtJVNo,lblError);
        }

        private void txtJVNo_TextChanged(object sender, EventArgs e)
        {
            clsSearchJournal.searchJV(dataGridView1, txtJVNo, txtJVNo, lblError);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count > 0)
            {
                JournalVoucher jv = new JournalVoucher();

                jv = (JournalVoucher)Application.OpenForms["JournalVoucher"];

                //=========================================================================================
                //                              Header Information
                //=========================================================================================
                jv.txtJVNumber.Text = dataGridView1.SelectedRows[0].Cells["JV_No"].Value.ToString();
                jv.txtMember.Text = dataGridView1.SelectedRows[0].Cells["AdjTo"].Value.ToString();
                jv.dtJVDate.Text = dataGridView1.SelectedRows[0].Cells["JV_Date"].Value.ToString();

                if (dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString() != "" || dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString() != string.Empty)
                {
                    jv.txtName.Text = clsSearchJournal.fullName(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString()));
                }
                jv.txtParticulars.Text = dataGridView1.SelectedRows[0].Cells["Particulars"].Value.ToString();
                if(dataGridView1.SelectedRows[0].Cells["summarize"].Value.ToString() == "True")
                {
                    jv.checkBox1.Checked = true;
                }
                else
                {
                    jv.checkBox1.Checked = false;
                }
                jv.cmbTransaction.SelectedValue = dataGridView1.SelectedRows[0].Cells["Transaction_Type"].Value.ToString();
                jv.txtLoanNumber.Text = dataGridView1.SelectedRows[0].Cells["Loan_No"].Value.ToString();

                //=========================================================================================
                //                              Footer Information
                //=========================================================================================

                jv.txtPreparedBy.Text = dataGridView1.SelectedRows[0].Cells["Prepared_By"].Value.ToString();
                jv.txtPostedBy.Text = dataGridView1.SelectedRows[0].Cells["Posted_By"].Value.ToString();
                jv.txtCancelled.Text = dataGridView1.SelectedRows[0].Cells["Cancelled_By"].Value.ToString();

                //=========================================================================================
                //                              Status Information
                //=========================================================================================

                if(dataGridView1.SelectedRows[0].Cells["Posted"].Value.ToString() == "True")
                {
                    jv.status.Visible = true;
                    jv.status.Text = "POSTED";
                }
                else if(dataGridView1.SelectedRows[0].Cells["Cancelled"].Value.ToString() == "True")
                {
                    jv.status.Visible = true;
                    jv.status.Text = "CANCELLED";
                }
                else
                {
                    jv.status.Visible = false;
                    jv.status.Text = "";
                }

                //=========================================================================================
                //                              Details Information
                //=========================================================================================

                jv.dataGridView1.Rows.Clear();

                //Check first if summarize or not
                if (dataGridView1.SelectedRows[0].Cells["summarize"].Value.ToString() == "True")
                {
                    clsSearchJournal.loadDetailSummary(jv.dataGridView1, dataGridView1.SelectedRows[0].Cells["JV_No"].Value.ToString());
                }
                else
                {
                    clsSearchJournal.loadDetailsNotSummarize(jv.dataGridView1, dataGridView1.SelectedRows[0].Cells["JV_No"].Value.ToString());
                }

                //=========================================================================================
                //                              Compute
                //=========================================================================================
                clsSearchJournal.loadTotalDebitCredit(jv.txtDebit, jv.txtCredit, dataGridView1.SelectedRows[0].Cells["JV_No"].Value.ToString());


                //=========================================================================================
                //                              Enable Buttons
                //=========================================================================================
                jv.btnEdit.Enabled = true;
                jv.btnPost.Enabled = true;
                jv.btnCancel.Enabled = true;
                jv.btnPrint.Enabled = true;

                jv.txtParticulars.BackColor = SystemColors.Control;

                //CLOSE AFTER SELECTION OF JOURNAL VOUCHER
                this.Close();
            }
        }
    }
}
