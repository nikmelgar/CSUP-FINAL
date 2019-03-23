using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
namespace WindowsFormsApplication2.Classes
{
    class clsLoanReport
    {
        SqlCommand cmd;
        SqlConnection con;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();
        public void loadComboBox(ComboBox cmb,string procName,int cnt)
        {
            cmb.DataSource = null;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;


                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                string str = dt.Columns[cnt].ToString();
                cmb.DisplayMember = str;
                cmb.ValueMember = dt.Columns[0].ToString();
                cmb.DataSource = dt;
            }
        }
    }
}
