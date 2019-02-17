using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;

namespace WindowsFormsApplication2
{
    class clsChartOfAccount
    {
        Global global = new Global();
        SqlConnection con = new SqlConnection();

        //Get Parent Level
        public int GetParentLevel(string accntCode)
        {
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT LevelNo FROM chart_of_accounts where Account_Code ='" + accntCode + "'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            int x = 0;
            if(dt.Rows.Count > 0)
            {
                return x = Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString()) + 1;
            }
            else
            {
                return x = x + 1;
            }

        }
    }
}
