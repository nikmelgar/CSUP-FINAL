using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;

namespace WindowsFormsApplication2
{
    class Global
    {
        public string datasource { get; set; }
        public string initialCatalog { get; set; }
        public string username { get; set; }
        public string pass { get; set; }

        public void connection(SqlConnection con)
        {
            SqlConnectionStringBuilder conbd = new SqlConnectionStringBuilder();

            //Office Connection

            //conbd.DataSource = "192.168.255.176";
            //conbd.InitialCatalog = "PECCI-NEW";
            ////conbd.IntegratedSecurity = true;
            //conbd.UserID = "sa";
            //conbd.Password = "SYSADMIN";
            //conbd.MaxPoolSize = 20000;

            //Local Connection House
            conbd.DataSource = ".";
            conbd.InitialCatalog = "PECCI-NEW";
            conbd.IntegratedSecurity = true;
            conbd.UserID = "sa";
            conbd.Password = "SYSADMIN";
            conbd.MaxPoolSize = 20000;



            //Maam Vangie IP

            //conbd.DataSource = "192.168.255.178";
            //conbd.InitialCatalog = "PECCI-NEW";
            ////conbd.IntegratedSecurity = true;
            //conbd.UserID = "sa";
            //conbd.Password = "SYSADMIN";

            datasource = conbd.DataSource;
            initialCatalog = conbd.InitialCatalog;
            username = conbd.UserID;
            pass = conbd.Password;
            con.ConnectionString = conbd.ToString();

            if (con.State != ConnectionState.Open)
            {
                con.Close();
                con.Open();
            }
        }

        public string connectString()
        {
            SqlConnectionStringBuilder conbd = new SqlConnectionStringBuilder();

            //Office Connection

            //conbd.DataSource = "192.168.255.176";
            //conbd.InitialCatalog = "PECCI-NEW";
            ////conbd.IntegratedSecurity = true;
            //conbd.UserID = "sa";
            //conbd.Password = "SYSADMIN";
            //conbd.MaxPoolSize = 20000;

            //Local Connection House
            conbd.DataSource = ".";
            conbd.InitialCatalog = "PECCI-NEW";
            conbd.IntegratedSecurity = true;
            conbd.UserID = "sa";
            conbd.Password = "SYSADMIN";
            conbd.MaxPoolSize = 20000;



            //Maam Vangie IP

            //conbd.DataSource = "192.168.255.178";
            //conbd.InitialCatalog = "PECCI-NEW";
            ////conbd.IntegratedSecurity = true;
            //conbd.UserID = "sa";
            //conbd.Password = "SYSADMIN";
            datasource = conbd.DataSource;
            initialCatalog = conbd.InitialCatalog;
            username = conbd.UserID;
            pass = conbd.Password;

            return conbd.ToString();
        }

        //For Resigned in PECCI cannot Transcat
        public Boolean checkMemberIfResignedTRUE(int id)
        {
            using (SqlConnection con = new SqlConnection(connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE userID = '" + id + "' and Date_Resigned_From_Pecci is not null", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count >= 1)
                {
                    //Meaning Member is already resigned
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #region File Maintenance 
        //File Maintenance 
        //Created By : Nikko Melgar


        public void loadDataForFileMaintenance(DataGridView dgv, string tblName)
        {
            using (SqlConnection con = new SqlConnection(connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM " + tblName + " where isActive = 1", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;

                int colCnt = dt.Columns.Count;
                int x = 0;


                while (x != colCnt)
                {
                    if (x != 0 && x != 1)
                    {
                        dgv.Columns[x].Visible = false;
                    }
                    x = x + 1;
                }
            }
        }

        public bool CheckDuplicateEntry(String Description, string tableName)
        {

            using (SqlConnection con = new SqlConnection(connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM " + tableName + " WHERE Description ='" + Description + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CheckDuplicateEntryParam(String criteria, string stringtoCompare, string tableName)
        {

            using (SqlConnection con = new SqlConnection(connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM " + tableName + " WHERE " + criteria + " ='" + stringtoCompare + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            

        }
        public void loadComboBox(ComboBox cmb, string tableName, string Display, string val)
        {
            cmb.DataSource = null;

            using (SqlConnection con = new SqlConnection(connectString()))
            {
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT " + Display + ", " + val + " FROM  " + tableName + " WHERE isActive ='1'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmb.DisplayMember = Display;
                cmb.ValueMember = val;
                cmb.DataSource = dt;
            }
        }

        public void loadComboBoxDistinct(ComboBox cmb, string tableName, string Display)
        {
            cmb.DataSource = null;

            using (SqlConnection con = new SqlConnection(connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT distinct(" + Display + ") FROM  " + tableName + " WHERE isActive ='1'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmb.DisplayMember = Display;
                cmb.DataSource = dt;
            }
        }

        #endregion region
    }
}
