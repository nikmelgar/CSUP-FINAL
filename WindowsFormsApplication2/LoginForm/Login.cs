using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using MetroFramework.Forms;

namespace WindowsFormsApplication2.LoginForm
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        //=====================================================
        //              Declaration
        //=====================================================

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        int attempt = 0;
        Global global = new Global();

        Classes.clsUser clsUser = new Classes.clsUser();

        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        private void Login_Load(object sender, EventArgs e)
        {
            txtUsername.Focus();
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

        public void LoginScript()
        {
            string Password = "";
            bool IsExist = false;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetUsernameAndPassword";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text);

                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    Password = dt.Rows[0].ItemArray[1].ToString();
                    IsExist = true;
                }
                else
                {
                    //Username does not exist.
                    Alert.show("Username does not exist.", Alert.AlertType.error);
                    txtUsername.Focus();
                    return;
                }

                if (IsExist)
                {
                    //Increment Login Attempts
                    SqlDataAdapter adapterAttempt = new SqlDataAdapter("SELECT loginAttemps FROM Users WHERE Username = '" + txtUsername.Text + "'", con);
                    DataTable dtAttempt = new DataTable();
                    adapterAttempt.Fill(dtAttempt);

                    attempt = Convert.ToInt32(dtAttempt.Rows[0].ItemArray[0].ToString());

                    if (Classes.clsUser.Decrypt(Password).Equals(txtPassword.Text))
                    {
                        if(attempt >= 3)
                        {
                            string str = "You have exceeded maximum Password attempts." + Environment.NewLine + "Please contact your account administrator.";

                            Alert.show(str, Alert.AlertType.error);
                            return;
                        }

                        if(checkIfAlreadyLogged() == true)
                        {
                            return;
                        }
                        //Go To Main Form

                        //Move Name and Other Information for Users Login
                        Classes.clsUser.Username = txtUsername.Text;
                        Classes.clsUser.firstName = dt.Rows[0].ItemArray[2].ToString();
                        Classes.clsUser.middleName = dt.Rows[0].ItemArray[3].ToString();
                        Classes.clsUser.lastName = dt.Rows[0].ItemArray[4].ToString();
                        Classes.clsUser.department = dt.Rows[0].ItemArray[5].ToString();
                        Classes.clsUser.role = dt.Rows[0].ItemArray[6].ToString();

                        //Go to Form
                        //this.Close();
                        //MainForm frm = new MainForm();
                        //frm.ShowDialog();

                        //Login At and IP Address
                        clsUser.updateUserLogin();

                        //reset attemp after successfully login
                        resetAttemp();

                        //clsMembership.loadPicture(dataGridView1.SelectedRows[0].Cells["userID"].Value.ToString(), dataentry.picPicture);

                        MainForm frm = new MainForm();// I have created object of Form2
                        this.Hide();//Hide cirrent form.
                        frm.Show();
                    }
                    else
                    {
                        if (attempt >= 3)
                        {
                            string str = "You have exceeded maximum Password attempts." + Environment.NewLine + "Please contact your account administrator.";

                            Alert.show(str, Alert.AlertType.error);
                            return;
                        }

                        cntAttempt();

                        Alert.show("Password is incorrect!", Alert.AlertType.error);
                        txtPassword.Focus();
                        return;
                    }
                }
                else
                {
                    Alert.show("Please enter the valid credentials.", Alert.AlertType.error);
                    txtUsername.Focus();
                    return;
                }
            }

        }

        public bool checkIfAlreadyLogged()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT isLoggedIn FROM Users WHERE Username = '"+ txtUsername.Text +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows[0].ItemArray[0].ToString() == "True" || dt.Rows[0].ItemArray[0].ToString() == "1")
                {
                    Alert.show("Account already in used.", Alert.AlertType.error);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void resetAttemp()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE Users SET loginAttemps = 0 WHERE Username ='" + txtUsername.Text + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

        public void cntAttempt()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_IncrementAttemp";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                cmd.ExecuteNonQuery();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            LoginScript();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if(label3.Text == "Show password")
            {
                label3.Text = "Hide";
                txtPassword.isPassword = false;
            }
            else
            {
                label3.Text = "Show password";
                txtPassword.isPassword = true;
            }
        }

        private void txtUsername_Enter(object sender, EventArgs e)
        {
            if(txtUsername.Text == "Username")
            {
                txtUsername.Text = "";
            }
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            if(txtUsername.Text == "")
            {
                txtUsername.Text = "Username";
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoginScript();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if(txtPassword.Text == "Password")
            {
                txtPassword.Text = "";
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if(txtPassword.Text == "")
            {
                txtPassword.Text = "Password";
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label4_Click(object sender, EventArgs e)
        {
          txtUsername.Text = Classes.clsUser.Encrypt(txtUsername.Text);
        }
    }
}
