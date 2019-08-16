using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsUserFileMaintenance
    {

        Global global = new Global();

        public void loadUsers(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_UsersFileMaintenance", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    dgv.Rows.Clear();
                    dgv.Rows.Add(ds.Tables[0].Rows.Count);

                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows[x].Cells[0].Value = ds.Tables[0].Rows[x][0].ToString();
                        dgv.Rows[x].Cells[1].Value = ds.Tables[0].Rows[x][1].ToString();
                        dgv.Rows[x].Cells[2].Value = ds.Tables[0].Rows[x][2].ToString();
                        dgv.Rows[x].Cells[3].Value = ds.Tables[0].Rows[x][3].ToString();
                        dgv.Rows[x].Cells[4].Value = ds.Tables[0].Rows[x][4].ToString();
                        dgv.Rows[x].Cells[5].Value = ds.Tables[0].Rows[x][5].ToString();
                    }

                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }
            }
        }

        public void loadComboBox(ComboBox cmb, string tableName, string Display, string val)
        {
            cmb.DataSource = null;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT " + Display + ", " + val + " FROM  " + tableName , con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmb.DisplayMember = Display;
                cmb.ValueMember = val;
                cmb.DataSource = dt;
            }
        }

        public string returnPassword(string username)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT UserPassword FROM Users WHERE Username = '"+ username +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return Classes.clsUser.Decrypt(dt.Rows[0].ItemArray[0].ToString());
            }
        }

        public string returnDepartment(string username)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT DepartmentID FROM Users WHERE Username = '" + username + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }

        public string returnRole(string username)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT RoleID FROM Users WHERE Username = '" + username +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }

        public bool alreadyUsedUsername(string username)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Username FROM Users WHERE Username = '"+ username +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    Alert.show("Username already exist.", Alert.AlertType.error);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int returnLogAttemp(string username)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT loginAttemps FROM Users WHERE Username = '" + username + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString());
            }
        }
    }
}
