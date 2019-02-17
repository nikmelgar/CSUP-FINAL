using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsLoanComputationDetails
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();

        public void loadLoansDetails(string loan_No,string loan_type,DataGridView dgv)
        {
            con = new SqlConnection();
            global.connection(con);

            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "sp_ReturnLoanComputationDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@loan_no", loan_No);
            cmd.Parameters.AddWithValue("@Loan_Type", loan_type);

            adapter = new SqlDataAdapter(cmd);
            dt = new DataTable();
            adapter.Fill(dt);

            dgv.DataSource = dt;

            dgv.Columns["Loan_Type_Loan_No"].HeaderText = "Loan No";
        }

        public void loadLoansDetailsOthers(string loan_No, DataGridView dgv)
        {
            con = new SqlConnection();
            global.connection(con);

            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "sp_ReturnLoanComputationDetailOthers";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@loan_no", loan_No);

            adapter = new SqlDataAdapter(cmd);
            dt = new DataTable();
            adapter.Fill(dt);

            dgv.DataSource = dt;

            dgv.Columns["Shared_Capital"].HeaderText = "Share Capital";
        }
    }
}
