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
    public partial class ClientLookUp : Form
    {
        public ClientLookUp()
        {
            InitializeComponent();
        }

        SqlConnection con;
        SqlDataAdapter adapter;

        Global global = new Global();
        Classes.clsCashReceipt clsCash = new Classes.clsCashReceipt();
        //DECLARATION
        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        public static string whosLookUp{get; set;}
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

        private void ClientLookUp_Load(object sender, EventArgs e)
        {
            loadClient();
        }

        //LOAD CLIENT
        public void loadClient()
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT * FROM Client WHERE isActive = 1", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;

            int colCnt = dt.Columns.Count;
            int x = 0;


            while (x != colCnt)
            {
                dataGridView1.Columns[x].Visible = false;
                x = x + 1;
            }

            dataGridView1.Columns["Client_Code"].Visible = true;
            dataGridView1.Columns["Client_Code"].HeaderText = "Code";
            dataGridView1.Columns["Client_Code"].FillWeight = 30;

            dataGridView1.Columns["Name"].Visible = true;
            dataGridView1.Columns["Name"].HeaderText = "Client Name";
            dataGridView1.Columns["Name"].FillWeight = 100;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtClientID.Text = "";
            txtName.Text = "";
            loadClient();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(txtClientID.Text != "" || txtName.Text != "")
            {
                con = new SqlConnection();
                global.connection(con);

                adapter = new SqlDataAdapter("SELECT * FROM Client WHERE isActive = 1 and Client_Code like '%"+ txtClientID.Text +"%' and Name like '%"+ txtName.Text +"%'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;

                if(dt.Rows.Count > 0)
                {
                    int colCnt = dt.Columns.Count;
                    int x = 0;


                    while (x != colCnt)
                    {
                        dataGridView1.Columns[x].Visible = false;
                        x = x + 1;
                    }

                    dataGridView1.Columns["Client_Code"].Visible = true;
                    dataGridView1.Columns["Client_Code"].HeaderText = "Code";
                    dataGridView1.Columns["Client_Code"].FillWeight = 30;

                    dataGridView1.Columns["Name"].Visible = true;
                    dataGridView1.Columns["Name"].HeaderText = "Client Name";
                    dataGridView1.Columns["Name"].FillWeight = 100;
                }
                else
                {
                    Alert.show("No Records Found!", Alert.AlertType.warning);
                    return;
                }

                
            }
            else
            {
                Alert.show("No keywords to be search!", Alert.AlertType.warning);
                return;
            }
            
        }

        private void btnUse_Click(object sender, EventArgs e)
        {
            if(whosLookUp == "2") //From OR 
            {
                //==========================================================================================
                //                      CASH RECEIPT CODE
                //==========================================================================================
                CashReceiptVoucher cashOR = new CashReceiptVoucher();

                foreach (Form form in Application.OpenForms)
                {

                    if (form.GetType() == typeof(CashReceiptVoucher))
                    {
                        //===============================================================================
                        //                      If form is already open
                        //===============================================================================
                        form.Activate();
                        cashOR = (CashReceiptVoucher)Application.OpenForms["CashReceiptVoucher"];
                        cashOR.txtPayorID.Text = dataGridView1.SelectedRows[0].Cells["Client_Code"].Value.ToString();
                        cashOR.txtPayorName.Text = dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString();
                        this.Close();
                        return;
                    }
                }

                cashOR.txtPayorID.Text = dataGridView1.SelectedRows[0].Cells["Client_Code"].Value.ToString();
                cashOR.txtPayorName.Text = dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString();
                cashOR.Show();
                this.Close();
            }
            else
            {
                DisbursementVoucher disbursement = new DisbursementVoucher();

                foreach (Form form in Application.OpenForms)
                {

                    if (form.GetType() == typeof(DisbursementVoucher))
                    {
                        //===============================================================================
                        //                      If form is already open
                        //===============================================================================
                        form.Activate();
                        disbursement = (DisbursementVoucher)Application.OpenForms["DisbursementVoucher"];
                        disbursement.txtPayee.Text = dataGridView1.SelectedRows[0].Cells["Client_Code"].Value.ToString();
                        disbursement.txtPayeeName.Text = dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString();
                        disbursement.radioClient.Checked = true;
                        Classes.clsDisbursement.userID = 0;
                        this.Close();
                        return;
                    }
                }

                disbursement.txtPayee.Text = dataGridView1.SelectedRows[0].Cells["Client_Code"].Value.ToString();
                disbursement.txtPayeeName.Text = dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString();
                disbursement.radioClient.Checked = true;
                Classes.clsDisbursement.userID = 0;
                disbursement.Show();

                this.Close();
                disbursement.Show();
            }
        }
    }
}
