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
    public partial class HoldAccounts : Form
    {
        public HoldAccounts()
        {
            InitializeComponent();
        }
        //=======================================================
        //              MOVEABLE PANEL
        //=======================================================

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        Global global = new Global();
        Classes.clsHoldAccounts clsHoldAccount = new Classes.clsHoldAccounts();

        //=======================================================
        //              DECLARATION
        //=======================================================
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        private void HoldAccounts_Load(object sender, EventArgs e)
        {
            clsHoldAccount.displayHoldAccounts(dataGridView1);
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            //controls
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(HoldAccounts))
                {
                    form.Activate();
                    return;
                }
            }

            HoldAccounts frm = new HoldAccounts();
            frm.Show();
            frm.MdiParent = this;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            lookupPrincipal lookup = new lookupPrincipal();
            Classes.clsHoldAccounts.userid = 0;
            lookup.ShowDialog();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            txtID.Text = "";
            txtReason.Text = "";
            Classes.clsHoldAccounts.userid = 0;
            btnLock.Image = imageList1.Images[0];
            btnLock.Text = "LOCK";
            btnCancel.Visible = false;
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            if(txtID.Text == "")
            {
                Alert.show("Please select member first!", Alert.AlertType.error);
                return;
            }

            if(txtReason.Text == "")
            {
                Alert.show("Reason field is required!", Alert.AlertType.error);
                return;
            }


            string msg = Environment.NewLine + "Are you sure you want to continue?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (btnLock.Text == "LOCK")
                {
                    //=======================================================================
                    //                     INSERT INTO HOLD ACCOUNTS
                    //=======================================================================
                    con = new SqlConnection();
                    global.connection(con);

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_InsertHoldAccounts";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", Classes.clsHoldAccounts.userid);
                    cmd.Parameters.AddWithValue("@reason", txtReason.Text);
                    cmd.Parameters.AddWithValue("@user_inserted", Classes.clsUser.Username);
                    cmd.ExecuteNonQuery();

                    //Display in time
                    clsHoldAccount.displayHoldAccounts(dataGridView1);

                    //alert
                    Alert.show("Members Account Successfully Locked!", Alert.AlertType.success);

                    btnCancel.Visible = false;
                    txtID.Text = "";
                    txtName.Text = "";
                    txtReason.Text = "";
                    Classes.clsHoldAccounts.userid = 0;

                }
                else
                {
                    //============================================================================
                    //                          UN LOCK ACCOUNT
                    //============================================================================
                    con = new SqlConnection();
                    global.connection(con);

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "DELETE Hold_Accounts WHERE userid = "+ Classes.clsHoldAccounts.userid;
                    cmd.ExecuteNonQuery();

                    //Alert
                    Alert.show("Members Account Successfully UnLocked!", Alert.AlertType.success);

                    btnCancel.Visible = false;
                    txtID.Text = "";
                    txtName.Text = "";
                    txtReason.Text = "";
                    Classes.clsHoldAccounts.userid = 0;

                    btnLock.Image = imageList1.Images[0];
                    btnLock.Text = "LOCK";

                    //Display in time
                    clsHoldAccount.displayHoldAccounts(dataGridView1);
                }
            }
            else
            {
                return;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                //populate the textbox from specific value of the coordinates of column and row.
                txtID.Text = row.Cells["EmployeeID"].Value.ToString();
                txtName.Text = row.Cells["Name"].Value.ToString();
                txtReason.Text = row.Cells["reason"].Value.ToString();
                Classes.clsHoldAccounts.userid = Convert.ToInt32(row.Cells["userid"].Value.ToString());

                btnLock.Text = "UNLOCK";
                btnLock.Image = imageList1.Images[1];
                btnCancel.Visible = true;
            }
        }
    }
}
