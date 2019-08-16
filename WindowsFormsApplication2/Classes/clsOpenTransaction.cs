using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsOpenTransaction
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();

        public void deleteOpenTransaction()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "DELETE Open_Transaction WHERE userid = '" + Classes.clsUser.Username + "'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }
        
        public void insertTransaction(string form, string reference)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_OpenTransactionInsert";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", Classes.clsUser.Username);
                cmd.Parameters.AddWithValue("@form", form);
                cmd.Parameters.AddWithValue("@reference_no", reference);
                cmd.ExecuteNonQuery();
            }
        }

        public void deleteTransaction(string form, string reference)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_OpenTransactionDelete";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", Classes.clsUser.Username);
                cmd.Parameters.AddWithValue("@form", form);
                cmd.Parameters.AddWithValue("@reference_no", reference);
                cmd.ExecuteNonQuery();
            }
        }

        public bool checkOpenFormsAndTransaction(string form,string reference)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM Open_Transaction WHERE FORM = '" + form + "' and reference_no = '" + reference + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //ALREADY OPEN
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        public string returnUserOnlineAndReference(string form,string reference,string tag) //tag is for withdrawal no or loan no or anything
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM Open_Transaction WHERE FORM = '" + form + "' and reference_no ='" + reference + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //GET THE USER WHOS USING THE FORM AND REFERENCE
                    return "This " + tag + " : " + reference + " is already \r\n used by Username : " + dt.Rows[0].ItemArray[0].ToString().ToUpper();
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
