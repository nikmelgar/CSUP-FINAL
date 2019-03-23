using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;



namespace WindowsFormsApplication2.Classes
{
    class clsMembersTotalBalances
    {
        SqlConnection con;
        SqlDataAdapter adapter;

        Global global = new Global();


        public void UpdateMembersTotalBalance(int userID,string EmployeeID,Decimal Savings,Decimal Share_Capital,Decimal Loans,Decimal Deferred,string FromForm)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_AdjustMentMembershipBalanceInfo";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
                cmd.Parameters.AddWithValue("@Savings", Savings);
                cmd.Parameters.AddWithValue("@Share_Capital", Share_Capital);
                cmd.Parameters.AddWithValue("@Loans", Loans);
                cmd.Parameters.AddWithValue("@Deferred", Deferred);
                cmd.Parameters.AddWithValue("@FromForm", FromForm);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
