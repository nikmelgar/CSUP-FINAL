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
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        Global global = new Global();

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

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (btnClose.Text == "CLOSE")
            {
                this.Close();
            }
            else
            {
                //For Cancel Function
                clearTextField();
                Disable();
                btnNew.Text = "NEW";
                btnEdit.Text = "EDIT";
                btnDelete.Text = "DELETE";

                btnNew.Enabled = true;
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;

                btnClose.Text = "CLOSE";
            }
        }

        public void loadClient()
        {
            SqlConnection con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Client where isActive = 1", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;


            int colCnt = dt.Columns.Count;
            int x = 1;

            while (x != colCnt)
            {
                if (x != 1)
                {
                    dataGridView1.Columns[x].Visible = false;
                }
                x = x + 1;
            }
        }


        private void Client_Load(object sender, EventArgs e)
        {
            loadClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text != "")
            {
                SqlConnection con = new SqlConnection();
                global.connection(con);

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Client where isActive = 1 and Client_Code like '%" + txtSearch.Text + "%' or Name like '%" + txtSearch.Text + "%'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;

                //Check if we got nothing
                if(dt.Rows.Count == 0)
                {
                    Alert.show("No Records Found! Please search again.", Alert.AlertType.error);
                    loadClient(); //Load default data
                }

                int colCnt = dt.Columns.Count;
                int x = 1;

                while (x != colCnt)
                {
                    if (x != 1)
                    {
                        dataGridView1.Columns[x].Visible = false;
                    }
                    x = x + 1;
                }

                //Search field remove
                txtSearch.Text = "";
            }
            else
            {
                Alert.show("Please Input Data you want to search!", Alert.AlertType.error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            loadClient();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (btnNew.Text == "SAVE")
            {
                //SAVE FUNCTION
                SqlConnection con = new SqlConnection();
                global.connection(con);

                //Check if all fields has a value
                if (CheckAllTextFields() == true)
                {
                    Alert.show("All fields with (*) are required", Alert.AlertType.warning);
                    return;
                }

                //Check if theres a duplicate of code or name
                //Check if theres a duplicate entry
                if (global.CheckDuplicateEntryParam("Client_Code", txtCode.Text, "Client") == true)
                {
                    Alert.show("Client Code Already Exist", Alert.AlertType.error);
                    return;
                }

                if (global.CheckDuplicateEntryParam("Name", txtName.Text, "Client") == true)
                {
                    Alert.show("Client Name Already Exist", Alert.AlertType.error);
                    return;
                }

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_InsertClient";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Client_Code", txtCode.Text.ToUpper());
                cmd.Parameters.AddWithValue("@Name", txtName.Text.ToUpper());
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@Telephone", txtTelephone.Text);
                cmd.Parameters.AddWithValue("@Fax", txtFaxNumber.Text);
                cmd.ExecuteNonQuery();

                //Success Message
                Alert.show("Successfully Added", Alert.AlertType.success);

                //Load Data
                loadClient();

                //After Saving Return to NEW and disable all textfields
                Disable(); //Disable All TextField
                btnNew.Text = "NEW";
                btnClose.Text = "CLOSE"; //Return to original text
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                //clear textfields
                clearTextField();
                //Enable all textfields
                Enable();
                btnNew.Text = "SAVE";
                btnClose.Text = "CANCEL";

                //Disable two button
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        public bool CheckAllTextFields()
        {
            if (txtCode.Text == "" || txtName.Text == "" || txtAddress.Text == "" || txtTelephone.Text == "")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void clearTextField()
        {
            foreach (Control ctrl in this.tableLayoutPanel1.Controls)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    ctrl.Text = "";
                }
            }
        }

        public void Enable()
        {
            foreach (Control ctrl in this.tableLayoutPanel1.Controls)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    ctrl.Enabled = true;
                }
            }
        }

        public void Disable()
        {
            foreach (Control ctrl in this.tableLayoutPanel1.Controls)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    ctrl.Enabled = false;
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                //No Data to be edit
                Alert.show("Please select client you want to edit!", Alert.AlertType.warning);
                return;
            }

            if (btnEdit.Text == "UPDATE")
            {
                //UPDATE FUNCTION

                //Check if all fields has a value
                if (CheckAllTextFields() == true)
                {
                    Alert.show("All fields with (*) are required", Alert.AlertType.warning);
                    return;
                }

                SqlConnection con = new SqlConnection();
                global.connection(con);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_UpdateClient";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Client_Code", txtCode.Text);
                cmd.Parameters.AddWithValue("@Name", txtName.Text.ToUpper());
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@Telephone", txtTelephone.Text);
                cmd.Parameters.AddWithValue("@Fax", txtFaxNumber.Text);
                cmd.ExecuteNonQuery();

                //Success
                Alert.show("Successfully Updated", Alert.AlertType.success);

                //loadData
                loadClient();

                Disable();
                btnEdit.Text = "EDIT";
                btnNew.Enabled = true;
                btnDelete.Enabled = true;
                btnClose.Text = "CLOSE";


            }
            else
            {
                //Enable all textfields
                Enable();
                btnEdit.Text = "UPDATE";
                btnClose.Text = "CANCEL";

                //Disable two button
                btnNew.Enabled = false;
                btnDelete.Enabled = false;
                //Disable txtCode
                txtCode.Enabled = false;


                if (dataGridView1.SelectedRows.Count > 0)
                {
                    txtCode.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                    txtName.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                    txtAddress.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                    txtTelephone.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
                    txtFaxNumber.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();

                }

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtCode.Text == "")
            {
                //No Data to be deleted
                Alert.show("Please select client you want to delete!", Alert.AlertType.error);
                return;
            }

            //ASK FIRST IF YOU WANT TO TAGGED AS INACTIVE[WIll not reflect on table and in syste]
            string msg = Environment.NewLine + "Are you sure you want to delete this Client?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //Delete or Tagged as Inactive goes here
                //CONNECTION TO SQL SERVER AND STORED PROCEDURE
                SqlConnection con = new SqlConnection();
                global.connection(con);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_InactiveClient";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Client_Code", txtCode.Text);
                cmd.ExecuteNonQuery();

                //Message
                Alert.show("Company Successfully Deleted", Alert.AlertType.success);

                loadClient();

                clearTextField();
            }
            else
            {
                return;
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Client))
                {
                    form.Activate();
                    return;
                }
            }

            Client frm = new Client();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }
    }
}
