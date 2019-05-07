using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.LoanLookUpProcess
{
    class clsLoanLookUpMember
    {
        Global global = new Global();

        public static int userid { get; set; }
        public static string frmPass { get; set; }


        public void loadLoanMember(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanBalances";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;

                for(int x = 0; x<dt.Columns.Count; x++)
                {
                    dgv.Columns[x].Visible = false;
                }

                dgv.Columns["Loan_No"].Visible = true;
                dgv.Columns["Loan_Type"].Visible = true;
                dgv.Columns["Loan_Amount"].Visible = true;
                dgv.Columns["Monthly_Amort"].Visible = true;
                dgv.Columns["ReleaseDate"].Visible = true;

                dgv.Columns["Loan_No"].HeaderText = "Loan No";
                dgv.Columns["Loan_Type"].HeaderText = "Type";
                dgv.Columns["Loan_Amount"].HeaderText = "Gross Amount";
                dgv.Columns["Monthly_Amort"].HeaderText = "Monthly";
                dgv.Columns["ReleaseDate"].HeaderText = "Released Date";

                dgv.Columns["Loan_Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns["Monthly_Amort"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns["ReleaseDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns["Loan_Type"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgv.Columns["Loan_Type"].FillWeight = 50;
            }
        }
    }
}
