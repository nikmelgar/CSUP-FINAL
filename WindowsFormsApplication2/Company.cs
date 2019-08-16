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
    public partial class Company : Form
    {
        public Company()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        //Declaration
        Global global = new Global();
        bool saveTrigger, updateTrigger; //For Insert and Update

        Classes.clsAccessControl clsAccess = new Classes.clsAccessControl();

        private void btnClose_Click(object sender, EventArgs e)
        {
            //Button : Close
            //Check if Cancel or Close Command
            //Nikko Melgar

            if (btnClose.Text == "CANCEL")
            {
                //Clear text first
                txtCode.Text = "";
                txtDescription.Text = "";
                txtRemarks.Text = "";

                //Change Buttons [Disabled - Enabled]
                btnNew.Text = "NEW";
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
                btnNew.Enabled = true;

                //Revert BtnClose
                btnClose.Text = "CLOSE";

                //Revert BtnEdit
                btnEdit.Text = "EDIT";

                //Disable Textbox
                txtDescription.Enabled = false;
                txtRemarks.Enabled = false;
            }
            else
            {
                this.Close();
            }
        }

        private void Company_Load(object sender, EventArgs e)
        {
            //Load Records
            global.loadDataForFileMaintenance(dataGridView1, "Company");
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if(clsAccess.checkForInsertRestriction(lblTitle.Text, Classes.clsUser.Username) != true)
            {
                return;
            }
            
            //CONNECTION TO SQL SERVER AND STORED PROCEDURE
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                saveTrigger = false;

                if (btnNew.Text == "SAVE")
                {
                    //CHECK FIRST BEFORE SAVING
                    if (txtDescription.Text == "")
                    {
                        Alert.show("All fields with (*) are required.", Alert.AlertType.warning);
                        return;
                    }
                    else
                    {

                        //Check if theres a duplicate entry
                        if (global.CheckDuplicateEntry(txtDescription.Text, "Company") == true)
                        {
                            Alert.show("Company Description already exist.", Alert.AlertType.error);
                            return;
                        }

                        //Saving here
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sp_InsertCompany";
                        txtDescription.Text = txtDescription.Text.Trim(); //trim space first
                        cmd.Parameters.AddWithValue("@Description", txtDescription.Text.ToUpper());
                        cmd.Parameters.AddWithValue("@Remark", txtRemarks.Text);
                        cmd.ExecuteNonQuery();

                        //CLEAR TEXTFIELDS
                        txtCode.Text = "";
                        txtDescription.Text = "";
                        txtRemarks.Text = "";

                        //Enable Buttons
                        btnEdit.Enabled = true;
                        btnDelete.Enabled = true;
                        btnClose.Text = "CLOSE";

                        //Save here
                        saveTrigger = true;
                        btnNew.Text = "NEW";

                        //load data
                        global.loadDataForFileMaintenance(dataGridView1, "Company");
                        //customize alert
                        Alert.show("Successfully added.", Alert.AlertType.success);

                        txtDescription.Enabled = false;
                        txtRemarks.Enabled = false;
                    }
                }
                else
                {

                    SqlCommand cmd = new SqlCommand("sp_AutoGenerateCompanyCode", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    //REMOVE DESCRIPTION IF HAS A VALUE
                    txtDescription.Text = "";
                    txtRemarks.Text = "";

                    txtCode.Text = dt.Rows[0].ItemArray[0].ToString();    //Fill txtCode //Autogenerate
                                                                          //ENABLED ALL FIELDS 
                    txtDescription.Enabled = true;
                    txtRemarks.Enabled = true;

                    btnEdit.Enabled = false;
                    btnDelete.Enabled = false;

                    //Change Close to Cancel
                    btnClose.Text = "CANCEL";

                    //SET FOCUS FIRST TO DESCRIPTION
                    txtDescription.Focus();

                }

                //Change New to Save
                if (saveTrigger == true)
                {
                    btnNew.Text = "NEW";
                }
                else
                {
                    btnNew.Text = "SAVE";
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForEditRestriction(lblTitle.Text, Classes.clsUser.Username) != true)
            {
                return;
            }

            if (txtCode.Text == "") //Check if code is empty
            {
                if (dataGridView1.SelectedRows.Count > 0) //make sure user select at least 1 row 
                {
                    string code = dataGridView1.SelectedRows[0].Cells[0].Value + string.Empty;
                    string description = dataGridView1.SelectedRows[0].Cells[1].Value + string.Empty;
                    string remarks = dataGridView1.SelectedRows[0].Cells[2].Value + string.Empty;

                    txtCode.Text = code;
                    txtDescription.Text = description;
                    txtRemarks.Text = remarks;
                }

            }

            //Disable button first 
            btnNew.Enabled = false;
            btnDelete.Enabled = false;

            //change button close to cancel
            btnClose.Text = "CANCEL";

            updateTrigger = false;

            //Enable first the textbox
            txtDescription.Enabled = true;
            txtRemarks.Enabled = true;

            if (btnEdit.Text == "UPDATE")
            {
                //Update code goes here
                //CHECK FIRST BEFORE SAVING
                if (txtDescription.Text == "")
                {
                    Alert.show("All fields with (*) are required.", Alert.AlertType.warning);
                    return;
                }


                //CONNECTION TO SQL SERVER AND STORED PROCEDURE
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();
                    //UPDATE CODE HERE
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_UpdateCompany";
                    cmd.Parameters.AddWithValue("@Company_Code", txtCode.Text);
                    txtDescription.Text = txtDescription.Text.Trim(); //trim space first
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text);
                    cmd.ExecuteNonQuery();

                    //load data realtime
                    global.loadDataForFileMaintenance(dataGridView1, "Company");
                    //update trigger call
                    updateTrigger = true;


                    //if successfully updated
                    //Disable textbox
                    txtDescription.Enabled = false;
                    txtRemarks.Enabled = false;

                    //clear 
                    txtCode.Text = "";
                    txtDescription.Text = "";
                    txtRemarks.Text = "";

                    //enable buttons
                    btnNew.Enabled = true;
                    btnDelete.Enabled = true;

                    btnClose.Text = "CLOSE";

                    //success
                    Alert.show("Successfully updated.", Alert.AlertType.success);

                    btnEdit.Text = "EDIT";
                }

            }
            else
            {

            }

            if (updateTrigger == false)
            {
                btnEdit.Text = "UPDATE";
            }
            else
            {
                btnEdit.Text = "EDIT";
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForDeleteRestriction(lblTitle.Text, Classes.clsUser.Username) != true)
            {
                return;
            }

            if (txtCode.Text == "") //Check if code is empty
            {
                if (dataGridView1.SelectedRows.Count > 0) // make sure user select at least 1 row 
                {
                    string code = dataGridView1.SelectedRows[0].Cells[0].Value + string.Empty;
                    string description = dataGridView1.SelectedRows[0].Cells[1].Value + string.Empty;
                    string remarks = dataGridView1.SelectedRows[0].Cells[2].Value + string.Empty;

                    txtCode.Text = code;
                    txtDescription.Text = description;
                    txtRemarks.Text = remarks;
                }

            }

            //ASK FIRST IF YOU WANT TO TAGGED AS INACTIVE[WIll not reflect on table and in syste]
            string msg = Environment.NewLine + "Are you sure you want to delete this Company?";
            DialogResult result = MessageBox.Show(this,msg,"PLDT Credit Cooperative",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //Delete or Tagged as Inactive goes here
                //CONNECTION TO SQL SERVER AND STORED PROCEDURE
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_InactiveCompany";
                    cmd.Parameters.AddWithValue("@Company_Code", txtCode.Text);
                    cmd.ExecuteNonQuery();

                    txtCode.Text = "";
                    txtDescription.Text = "";
                    txtRemarks.Text = "";


                    //Load Real Time
                    global.loadDataForFileMaintenance(dataGridView1, "Company");

                    //Message
                    Alert.show("Company successfully deleted.", Alert.AlertType.success);
                }   
            }
            else
            {
                return;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && btnClose.Text == "CLOSE")
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                //populate the textbox from specific value of the coordinates of column and row.
                txtCode.Text = row.Cells["Company_Code"].Value.ToString();
                txtDescription.Text = row.Cells["Description"].Value.ToString();
                txtRemarks.Text = row.Cells["Remarks"].Value.ToString();
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Company))
                {
                    form.Activate();
                    return;
                }
            }

            Company frm = new Company();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
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
    }
}
