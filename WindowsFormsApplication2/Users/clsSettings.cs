using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace WindowsFormsApplication2.Users
{
    class clsSettings
    {
        Global global = new Global();

        public static string defaultPasswordUponLogin { get; set; }

        public string returnDepartment(string DepartmentID)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Department FROM Department WHERE DepartmentID = '" + DepartmentID + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }

        public void loadPicture(PictureBox pic)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(new SqlCommand("SELECT userImage FROM Users WHERE Username = '" + Classes.clsUser.Username + "'", con));
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                if (dataSet.Tables[0].Rows.Count == 1 && Convert.ToString(dataSet.Tables[0].Rows[0]["userImage"].ToString()) != "")
                {
                    Byte[] data = new Byte[0];
                    data = (Byte[])(dataSet.Tables[0].Rows[0]["userImage"]);
                    MemoryStream mem = new MemoryStream(data);
                    pic.Image = Image.FromStream(mem);
                }
            }
        }
    }
}
