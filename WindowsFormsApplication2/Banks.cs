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
    public partial class Banks : Form
    {
        public Banks()
        {
            InitializeComponent();
        }

        Global global = new Global();
        clsBanks clsbank = new clsBanks();
        SqlCommand cmd;
        SqlConnection con;
        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        Classes.clsAccessControl clsAccess = new Classes.clsAccessControl();

        private void btnClose_Click(object sender, EventArgs e)
        {
            if(btnClose.Text == "CANCEL")
            {
                //Restore textbox and buttons
                cleartxtField();
                disabletxtField();
                restoreBtn();
            }
            else
            {
                this.Close();
            }
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


                if (form.GetType() == typeof(Banks))
                {
                    form.Activate();
                    return;
                }
            }

            Banks frm = new Banks();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }

        private void Banks_Load(object sender, EventArgs e)
        {
            clsbank.loadBanks(dataGridView1, "Bank");
            clsbank.loadComboBox(cmbBankAccount);

            cleartxtField();
            restoreBtn();
            disabletxtField();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForInsertRestriction(lblTitle.Text, Classes.clsUser.Username) != true)
            {
                return;
            }

            if (btnNew.Text == "SAVE")
            {
                //Required Fields
                if (RequiredFields(txtCode, txtDescription, txtBranch, txtAccountNo) == true)
                {
                    Alert.show("All fields with (*) are required.", Alert.AlertType.warning);
                    return;
                }

                //Check if theres a duplicate entry for this newly insert CODE
                if (global.CheckDuplicateEntryParam("Bank_Code", txtCode.Text, "Bank") == true)
                {
                    Alert.show("Bank Code already exist.", Alert.AlertType.error);
                    return;
                }

                //Check description for duplicate
                if (global.CheckDuplicateEntryParam("Bank_Name", txtDescription.Text, "Bank") == true)
                {
                    Alert.show("Bank Name already exist.", Alert.AlertType.error);
                    return;
                }

                //If all criteria is successfully meet we now proceed to saving code
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_InsertBank";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Bank_Code", txtCode.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Bank_Name", txtDescription.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Branch", txtBranch.Text);
                    cmd.Parameters.AddWithValue("@Account_No", txtAccountNo.Text);
                    cmd.Parameters.AddWithValue("@Bank_Account_Code", cmbBankAccount.SelectedValue);
                    cmd.Parameters.AddWithValue("@Contact_Person", txtContactPerson.Text);
                    cmd.Parameters.AddWithValue("@Position", txtPosition.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmd.ExecuteNonQuery();
                }
                  
                //Success Message
                Alert.show("Successfully added.", Alert.AlertType.success);

                //load Banks
                clsbank.loadBanks(dataGridView1, "Bank");

                cleartxtField();
                restoreBtn();
                disabletxtField();
            }
            else
            {
                btnNew.Text = "SAVE";
                enabletxtField();
                cleartxtField();

                btnClose.Text = "CANCEL";
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }

        }
         
        #region Enable Disable fields
        public void cleartxtField()
        {
            Control control = new Control();
            foreach (var c in tableLayoutPanel1.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
                if (c is ComboBox) ((ComboBox)c).Text = String.Empty;
            }
        }

        public void enabletxtField()
        {
            Control control = new Control();
            foreach (var c in tableLayoutPanel1.Controls)
            {
                if (c is TextBox) ((TextBox)c).Enabled = true;
                if (c is ComboBox) ((ComboBox)c).Enabled = true;
            }
        }

        public void disabletxtField()
        {
            Control control = new Control();
            foreach (var c in tableLayoutPanel1.Controls)
            {
                if (c is TextBox) ((TextBox)c).Enabled = false;
                if (c is ComboBox) ((ComboBox)c).Enabled = false;
            }
        }

        public void restoreBtn()
        {
            btnNew.Text = "NEW";
            btnEdit.Text = "EDIT";
            btnDelete.Text = "DELETE";
            btnClose.Text = "CLOSE";

            btnNew.Enabled = true;
            btnEdit.Enabled = true;
            btnDelete.Enabled = true;
            btnClose.Enabled = true;
        }

        public bool RequiredFields(TextBox code,TextBox Description, TextBox branch, TextBox accountNo)
        {
            if(code.Text == "" || Description.Text == "" || branch.Text == "" || accountNo.Text == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && btnClose.Text == "CLOSE")
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                //populate the textbox from specific value of the coordinates of column and row.
                txtCode.Text = row.Cells["Bank_Code"].Value.ToString();
                txtDescription.Text = row.Cells["Bank_Name"].Value.ToString();
                txtBranch.Text = row.Cells["Branch"].Value.ToString();
                txtAccountNo.Text = row.Cells["Account_No"].Value.ToString();
                cmbBankAccount.Text = row.Cells["Bank_Account_Code"].Value.ToString();
                txtContactPerson.Text = row.Cells["Contact_Person"].Value.ToString();
                txtPosition.Text = row.Cells["Position"].Value.ToString();
                txtAddress.Text = row.Cells["Address"].Value.ToString();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForEditRestriction(lblTitle.Text, Classes.clsUser.Username) != true)
            {
                return;
            }

            if (txtCode.Text == "")
            {
                //No Data to be edit
                Alert.show("Please select bank you want to edit.", Alert.AlertType.warning);
                return;
            }
            
            if(btnEdit.Text == "EDIT")
            {
                btnEdit.Text = "UPDATE";
                btnClose.Text = "CANCEL";
                enabletxtField();

                txtCode.Enabled = false;

                btnNew.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                //UPDATE CODE HERE

                //Required Fields
                if (RequiredFields(txtCode, txtDescription, txtBranch, txtAccountNo) == true)
                {
                    Alert.show("All fields with (*) are required.", Alert.AlertType.warning);
                    return;
                }

                if(clsbank.CheckDuplicateEntry(txtDescription.Text,txtCode.Text,"Bank") == true)
                {
                    Alert.show("Bank Name already exist.", Alert.AlertType.error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_UpdateBank";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Bank_Code", txtCode.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Bank_Name", txtDescription.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Branch", txtBranch.Text);
                    cmd.Parameters.AddWithValue("@Account_No", txtAccountNo.Text);
                    cmd.Parameters.AddWithValue("@Bank_Account_Code", cmbBankAccount.SelectedValue);
                    cmd.Parameters.AddWithValue("@Contact_Person", txtContactPerson.Text);
                    cmd.Parameters.AddWithValue("@Position", txtPosition.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmd.ExecuteNonQuery();
                }

                //Success Message
                Alert.show("Successfully updated.", Alert.AlertType.success);

                //load Banks
                clsbank.loadBanks(dataGridView1, "Bank");

                cleartxtField();
                restoreBtn();
                disabletxtField();

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (clsAccess.checkForDeleteRestriction(lblTitle.Text, Classes.clsUser.Username) != true)
            {
                return;
            }

            if (txtCode.Text == "")
            {
                //No Data to be edit
                Alert.show("Please select bank you want to delete.", Alert.AlertType.warning);
                return;
            }


            //ASK FIRST IF YOU WANT TO TAGGED AS INACTIVE[WIll not reflect on table and in syste]
            string msg = Environment.NewLine + "Are you sure you want to delete this Bank?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //Delete or Tagged as Inactive goes here
                //CONNECTION TO SQL SERVER AND STORED PROCEDURE
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_InactiveBank";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Bank_Code", txtCode.Text);
                    cmd.ExecuteNonQuery();
                }

                //Message
                Alert.show("Company successfully deleted.", Alert.AlertType.success);

                //load Banks
                clsbank.loadBanks(dataGridView1, "Bank");

                cleartxtField();
                restoreBtn();
                disabletxtField();

            }
            else
            {
                return;
            }

        }
    }
}
