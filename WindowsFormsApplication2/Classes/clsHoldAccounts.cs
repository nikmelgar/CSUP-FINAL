using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsHoldAccounts
    {

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();

        public static int userid { get; set; }

        public void displayHoldAccounts(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("select * From vw_HoldAccounts", con);
                dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;

                dgv.Columns["userid"].Visible = false;
                dgv.Columns["EmployeeID"].HeaderText = "ID";
                dgv.Columns["EmployeeID"].FillWeight = 30;

                dgv.Columns["reason"].HeaderText = "Reason";
                dgv.Columns["user_inserted"].HeaderText = "Posted by";
                dgv.Columns["dateinserted"].HeaderText = "Date Posted";

                dgv.Columns["Name"].FillWeight = 120;
                dgv.Columns["user_inserted"].FillWeight = 70;
                dgv.Columns["dateinserted"].FillWeight = 70;
            }            
        }

        public Boolean checkIfHoldAccount(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM vw_HoldAccounts WHERE userid ='" + userid + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //HOLD ACCOUNT
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Boolean checkIfTHeresADependent(string employeeid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM vw_HoldAccounts WHERE EmployeeID = '" + employeeid + "'", con);
                dt = new DataTable();
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
    }
}
