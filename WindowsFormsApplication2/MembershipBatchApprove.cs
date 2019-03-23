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
    public partial class MembershipBatchApprove : Form
    {
        public MembershipBatchApprove()
        {
            InitializeComponent();
        }

        SqlConnection con;
        Global global = new Global();
        Classes.clsMembershipBatchApprove clsBatch = new Classes.clsMembershipBatchApprove();

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        private void cmbView_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbView.Text == "Date / Month")
            {
                dtMonthFrom.Visible = true;
                dtMonthTo.Visible = true;
                lblFrom.Visible = true;
                lblTo.Visible = true;

                var vwpoint = new Point(12, 126);
                this.btnView.Location = vwpoint;
                

            }
            else
            {
                dtMonthFrom.Visible = false;
                dtMonthTo.Visible = false;
                lblFrom.Visible = false;
                lblTo.Visible = false;

                var vwpoint = new Point(12, 54);
                this.btnView.Location = vwpoint;
                
            }
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

        private void panel1_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(MembershipBatchApprove))
                {
                    form.Activate();
                    return;
                }
            }

            MembershipBatchApprove frm = new MembershipBatchApprove();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if(cmbView.Text == "Members for Approval")
            {
                clsBatch.loadAllMembersForApproval(dataGridView1);
            }
            else if(cmbView.Text == "Date / Month")
            {
                clsBatch.loadMembersByDate(dataGridView1, dtMonthFrom, dtMonthTo);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_SearchMemberForApproval";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Keyword", txtSearch.Text);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt;

                    int colCnt = dt.Columns.Count;
                    int x = 0;


                    while (x != colCnt)
                    {
                        dataGridView1.Columns[x].Visible = false;
                        x = x + 1;
                    }

                    dataGridView1.Columns["EmployeeID"].Visible = true;
                    dataGridView1.Columns["EmployeeID"].HeaderText = "Employee ID";

                    dataGridView1.Columns["LastName"].Visible = true;
                    dataGridView1.Columns["LastName"].HeaderText = "Last Name";

                    dataGridView1.Columns["FirstName"].Visible = true;
                    dataGridView1.Columns["FirstName"].HeaderText = "First Name";

                    dataGridView1.Columns["MiddleName"].Visible = true;
                    dataGridView1.Columns["MiddleName"].HeaderText = "Middle Name";

                    dataGridView1.Columns["Date_Of_Birth"].Visible = true;
                    dataGridView1.Columns["Date_Of_Birth"].HeaderText = "Birthday";

                    dataGridView1.Columns["IsApprove"].Visible = true;
                    dataGridView1.Columns["IsApprove"].FillWeight = 60;
                    dataGridView1.Columns["IsApprove"].HeaderText = "Approve";
                    dataGridView1.Columns["IsApprove"].ReadOnly = false;

                }
                else
                {
                    Alert.show("No record found.", Alert.AlertType.error);
                }
            }
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(dataGridView1.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4);
            }
        }

        private void btnBatch_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count > 0)
            {
                int x = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DataGridViewCheckBoxCell cell = row.Cells[0] as DataGridViewCheckBoxCell;

                    //We don't want a null exception!
                    if (cell.Value != null)
                    {
                        if (cell.Value.ToString() == "" || cell.Value.ToString() == "False")
                        {
                            //Get Total Count of False
                            x = x + 1;
                        }
                       
                    }
                }
                //===========================================================================
                //              Check if theres no checked in datagrid
                //===========================================================================

                if(x == dataGridView1.Rows.Count)
                {
                    //No data check
                    Alert.show("Please select member you want to approve!", Alert.AlertType.error);
                    return;
                }
                else
                {
                    //Ask first if you really want to approve those members
                    //ASK FIRST IF YOU WANT TO TAGGED AS INACTIVE[WIll not reflect on table and in syste]
                    string msg = Environment.NewLine + "Are you sure you want to continue?";
                    DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        using (SqlConnection con = new SqlConnection(global.connectString()))
                        {
                            con.Open();

                            //========================================================================
                            //                  PROGRESSBAR
                            //========================================================================

                            progressBar1.Visible = true;

                            //set progressbar
                            progressBar1.Minimum = 0;
                            progressBar1.Maximum = dataGridView1.Rows.Count;
                            progressBar1.Value = 0;

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                DataGridViewCheckBoxCell cell = row.Cells[0] as DataGridViewCheckBoxCell;

                                //We don't want a null exception!
                                if (cell.Value != null)
                                {
                                    if (cell.Value.ToString() == "True")
                                    {
                                        //It's checked!
                                        //Code for saving all the check members approve
                                        SqlCommand cmd = new SqlCommand();
                                        cmd.Connection = con;
                                        cmd.CommandText = "sp_ApproveMembers";
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@userID", row.Cells["userID"].Value.ToString());
                                        cmd.Parameters.AddWithValue("@Approve_By", Classes.clsUser.Username);
                                        cmd.ExecuteNonQuery();

                                        //Insert into members fee table
                                        SqlCommand cmdFee = new SqlCommand();
                                        cmdFee.Connection = con;
                                        cmdFee.CommandText = "sp_InsertMembersFee";
                                        cmdFee.CommandType = CommandType.StoredProcedure;
                                        cmdFee.Parameters.AddWithValue("@userid", row.Cells["userID"].Value.ToString());
                                        cmdFee.ExecuteNonQuery();
                                    }
                                }

                                progressBar1.Value = progressBar1.Value + 1;
                            }

                            //ReLoad The Gridview for Real Time
                            if (cmbView.Text == "Members for Approval")
                            {
                                clsBatch.loadAllMembersForApproval(dataGridView1);
                            }
                            else if (cmbView.Text == "Date / Month")
                            {
                                clsBatch.loadMembersByDate(dataGridView1, dtMonthFrom, dtMonthTo);
                            }
                        }
                        //Success Message
                        Alert.show("Successfully approved.", Alert.AlertType.success);
                    } //End Question for continue
                    else
                    {
                        return;
                    }
                }     
            }
            else
            {
                Alert.show("No record found.", Alert.AlertType.error);
                return;
            }

        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count > 0)
            {
                //if theres a data
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // This will check the cell.
                    row.Cells["IsApprove"].Value = "true";
                }
            }
            else
            {
                Alert.show("No record found.", Alert.AlertType.error);
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                //if theres a data
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // This will check the cell.
                    row.Cells["IsApprove"].Value = "false";
                }
            }
            else
            {
                Alert.show("No record found.", Alert.AlertType.error);
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //CHECK IF THERES A SELECTED DATA
            if (dataGridView1.SelectedRows.Count == 0)
            {
                //No Data to be edit
                Alert.show("Please select members you want to delete.", Alert.AlertType.warning);
                return;
            }

            //Code for delete of Members Pending
            string msg = Environment.NewLine + "Are you sure you want to delete this member?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_DeleteMembershipInApproval";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userID", dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString());
                    cmd.ExecuteNonQuery();

                    //Load Datagridview per COmbobox description
                    if (cmbView.Text == "Members for Approval")
                    {
                        clsBatch.loadAllMembersForApproval(dataGridView1);
                    }
                    else if (cmbView.Text == "Date / Month")
                    {
                        clsBatch.loadMembersByDate(dataGridView1, dtMonthFrom, dtMonthTo);
                    }
                }
                //Message
                Alert.show("Member successfully deleted", Alert.AlertType.success);
            }

        }
    }
}
