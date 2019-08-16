using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2.PDCFolder
{
    public partial class pdcQuestion : Form
    {
        public pdcQuestion()
        {
            InitializeComponent();
        }

        Global global = new Global();


        private void btnClose_Click(object sender, EventArgs e)
        {
            PDCPastDue pdc = new PDCPastDue();
            pdc.tabControl1.SelectedTab = pdc.tabPage1;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(chckRunDate.Checked == false)
            {
                Alert.show("Please check run date first.", Alert.AlertType.error);
                return;
            }
            
            if(txtPassword.Text == "")
            {
                Alert.show("Please enter your Password.", Alert.AlertType.error);
                return;
            }

            string Password = "";

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetUsernameAndPassword";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", Classes.clsUser.Username);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                Password = dt.Rows[0].ItemArray[1].ToString();

                string output = Classes.clsUser.Decrypt(Password).ToString();
                if (txtPassword.Text == output)
                {
                    //Password Correct       
                    //Create JV and others
                    //CALL FUNCTION IN PDC PAST DUE
                    PDCPastDue pdc = new PDCPastDue();
                    pdc.runPastDue();
                    this.Close();                 
                }
                else
                {
                    MessageBox.Show("Password is incorrect");
                    txtPassword.Focus();
                    return;
                }
            }
        }
    }
}
