using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsAccessControl
    {
        Global global = new Global();

        public void UserAccount(CheckBox chckActive, CheckBox chckInActive,Label lblInformation,string UserName)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Username,isLoggedIn, LoggedAtName, LoggedAtIPAddress FROM Users WHERE Username = '"+ UserName + "'", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if(ds.Tables[0].Rows[0]["isLoggedIn"].ToString() == "True")
                {
                    chckActive.Checked = true;
                    chckInActive.Checked = false;
                    lblInformation.Text = ds.Tables[0].Rows[0]["LoggedAtName"].ToString() + " / " + ds.Tables[0].Rows[0]["LoggedAtIPAddress"].ToString();
                }
                else
                {
                    chckActive.Checked = false;
                    chckInActive.Checked = true;
                    lblInformation.Text = "Not login to any other devices.";
                }
            }
        }

        //Load Default Windows
        public void loadDefaultWindows(DataGridView dgv,string username)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapterCheckFromTable = new SqlDataAdapter("SELECT * FROM AccessRights WHERE username ='"+ username +"'", con);
                DataSet ds = new DataSet();
                adapterCheckFromTable.Fill(ds);

                if(ds.Tables[0].Rows.Count > 0)
                {
                    //Has already access
                    //display access
                    dgv.Rows.Clear();
                    dgv.Rows.Add(ds.Tables[0].Rows.Count);

                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows[x].Cells[0].Value = ds.Tables[0].Rows[x]["windowsForm"].ToString();
                        dgv.Rows[x].Cells[1].Value = ds.Tables[0].Rows[x]["vView"].ToString();
                        dgv.Rows[x].Cells[2].Value = ds.Tables[0].Rows[x]["iInsert"].ToString();
                        dgv.Rows[x].Cells[3].Value = ds.Tables[0].Rows[x]["eEdit"].ToString();
                        dgv.Rows[x].Cells[4].Value = ds.Tables[0].Rows[x]["dDelete"].ToString();
                                               
                    }
                }
                else
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM windowsAccess", con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgv.Rows.Clear();
                    dgv.Rows.Add(dt.Rows.Count);

                    for (int x = 0; x < dt.Rows.Count; x++)
                    {
                        dgv.Rows[x].Cells[0].Value = dt.Rows[x].ItemArray[0].ToString();
                        dgv.Rows[x].Cells[1].Value = false;
                        dgv.Rows[x].Cells[2].Value = false;
                        dgv.Rows[x].Cells[3].Value = false;
                        dgv.Rows[x].Cells[4].Value = false;
                    }
                }
            }
        }


        //========================================================================
        //                      REGION RESTRICTION
        //========================================================================

        public bool checkForViewingRestriction(string windowName,string username)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM AccessRights WHERE username ='" + username + "' and windowsForm = '"+ windowName +"'", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if(ds.Tables[0].Rows.Count > 0)
                {
                    if(ds.Tables[0].Rows[0]["vView"].ToString() != "True")
                    {
                        Alert.show("Error : Access denied.", Alert.AlertType.error);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    Alert.show("Error : Access denied.", Alert.AlertType.error);
                    return false;
                }
            }
        }

        public bool checkForInsertRestriction(string windowName, string username)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM AccessRights WHERE username ='" + username + "' and windowsForm = '" + windowName + "'", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["iInsert"].ToString() != "True")
                    {
                        Alert.show("Error : Access denied.", Alert.AlertType.error);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    Alert.show("Error : Access denied.", Alert.AlertType.error);
                    return false;
                }
            }
        }

        public bool checkForEditRestriction(string windowName, string username)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM AccessRights WHERE username ='" + username + "' and windowsForm = '" + windowName + "'", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["eEdit"].ToString() != "True")
                    {
                        Alert.show("Error : Access denied.", Alert.AlertType.error);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    Alert.show("Error : Access denied.", Alert.AlertType.error);
                    return false;
                }
            }
        }

        public bool checkForDeleteRestriction(string windowName, string username)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM AccessRights WHERE username ='" + username + "' and windowsForm = '" + windowName + "'", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["dDelete"].ToString() != "True")
                    {
                        Alert.show("Error : Access denied.", Alert.AlertType.error);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    Alert.show("Error : Access denied.", Alert.AlertType.error);
                    return false;
                }
            }
        }

    }
}
