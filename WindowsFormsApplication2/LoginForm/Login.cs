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
                    //Username does not exist!
                    Alert.show("Username does not exist!", Alert.AlertType.error);
                    txtUsername.Focus();
                    return;
                }

                if (IsExist)
                {
                    if (Classes.clsUser.Decrypt(Password).Equals(txtPassword.Text))
                    {
                        //Go To Main Form

                        //Move Name and Other Information for Users Login
                        Classes.clsUser.Username = txtUsername.Text;
                        Classes.clsUser.firstName = dt.Rows[0].ItemArray[2].ToString();
                        Classes.clsUser.middleName = dt.Rows[0].ItemArray[3].ToString();
                        Classes.clsUser.lastName = dt.Rows[0].ItemArray[4].ToString();


                        //Go to Form
                        MainForm frm = new MainForm();
                        this.Hide();
                        frm.ShowDialog();
                    }
                    else
                    {
                        Alert.show("Password is incorrect!", Alert.AlertType.error);
                        txtPassword.Focus();
                        return;
                    }
                }
                else
                {
                    Alert.show("Please enter the valid credentials", Alert.AlertType.error);
                    txtUsername.Focus();
                    return;
                }
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
    }
}
