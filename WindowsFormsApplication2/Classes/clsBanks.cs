using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    class clsBanks
    {
        Global global = new Global();
        SqlDataAdapter adapter;
        SqlConnection con;
        public void loadBanks(DataGridView dgv, string tableName)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT * FROM  " + tableName + " Where isActive = '1'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dgv.DataSource = dt;
            
            dgv.Columns["Bank_Code"].HeaderText = "Code";
            dgv.Columns["Bank_Code"].FillWeight = 40;
            dgv.Columns["Bank_Name"].HeaderText = "Name";
            dgv.Columns["Bank_Name"].FillWeight = 120;
            int colCnt = dt.Columns.Count;
            int x = 1;

            while (x != colCnt)
            {
                if (x != 1)
                {
                    dgv.Columns[x].Visible = false;
                }
                x = x + 1;
            }

        }

        public bool CheckDuplicateEntry(String Bank_Name, string Bank_Code,string tableName)
        {

            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM " + tableName + " WHERE Bank_Name ='" + Bank_Name + "' and Bank_Code <> '"+ Bank_Code + "'", con);
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

        public void loadComboBox(ComboBox cmb)
        {
            cmb.DataSource = null;

            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("select (Account_Code +' - '+ Account_Description) as Display , Account_Code from chart_of_accounts where Parent_Account = '102'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            cmb.DisplayMember = dt.Columns[0].ToString();
            cmb.ValueMember = dt.Columns[1].ToString();
            cmb.DataSource = dt;

        }
    }
}
