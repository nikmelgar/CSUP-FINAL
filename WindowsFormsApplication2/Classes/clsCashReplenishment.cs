using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsCashReplenishment
    {
        Global global = new Global();

        SqlConnection con;
        SqlDataAdapter adapter;

        public void loadReplenishment(DataGridView dgv, int rangeFrom,int rangeTo,string wdFrom,string wdTo,TextBox lblTotalSlips,TextBox totalAmnt)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "sp_GetCashReplenishment";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@rangeFrom", rangeFrom);
            cmd.Parameters.AddWithValue("@rangeTo", rangeTo);
            cmd.Parameters.AddWithValue("@wdFrom", wdFrom);
            cmd.Parameters.AddWithValue("@wdTo", wdTo);

            adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                dgv.DataSource = dt;
                lblTotalSlips.Text = dt.Rows.Count.ToString();

                //visible false
                dgv.Columns["userID"].Visible = false;

                dgv.Columns["Withdrawal_Slip_No"].HeaderText = "Slip No";
                dgv.Columns["wdDate"].HeaderText = "Date";


                //Get Total Amount
                SqlCommand cmd1 = new SqlCommand();
                cmd1.Connection = con;
                cmd1.CommandText = "sp_GetAmountReplenishment";
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@rangeFrom", rangeFrom);
                cmd1.Parameters.AddWithValue("@rangeTo", rangeTo);
                cmd1.Parameters.AddWithValue("@wdFrom", wdFrom);
                cmd1.Parameters.AddWithValue("@wdTo", wdTo);

                adapter = new SqlDataAdapter(cmd1);
                DataTable dt1 = new DataTable();
                adapter.Fill(dt1);

                totalAmnt.Text = Convert.ToDecimal(dt1.Rows[0].ItemArray[0].ToString()).ToString();
            }
            else
            {
                Alert.show("No Cash to be replenish", Alert.AlertType.warning);
                lblTotalSlips.Text = "0";
                totalAmnt.Text = "0.00";
                dgv.DataSource = null; 
                return;
            }
        }

        public string getCashier()
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT [NAME] FROM Client WHERE Client_Code = 'CASHIER'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt.Rows[0].ItemArray[0].ToString();
        }
    }
}
