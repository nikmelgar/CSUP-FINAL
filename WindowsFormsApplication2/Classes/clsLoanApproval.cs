using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsLoanApproval
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();

      
        public string returnStatusNo(string loan_No)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Status FROM Loan WHERE Loan_No ='" + loan_No + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }

        }

        public void loadLoanBalances(int userid,DataGridView dgv,string loanType)
        {
            dgv.Rows.Clear();

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                //SqlDataAdapter adapter = new SqlDataAdapter("SELECT Loan_No,Loan_Type,balance FROM vw_LoanBalances WHERE userID = '"+ userid +"'", con);
                //DataTable dt = new DataTable();
                //adapter.Fill(dt);

                //if(dt.Rows.Count > 0)
                //{
                //    dgv.Rows.Add(dt.Rows.Count);

                //    for (int i = 0; i < dt.Rows.Count; i++)
                //    {
                //        dgv.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString();
                //        dgv.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString();
                //        dgv.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString();
                //    }
                //}

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanBalances";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);

                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgv.Rows.Add(dt.Rows.Count);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgv.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString(); //Loan No
                        dgv.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString() + " - " + dt.Rows[i].ItemArray[34].ToString(); //Type
                        dgv.Rows[i].Cells[2].Value = Convert.ToDecimal(dt.Rows[i].ItemArray[37].ToString()).ToString("#,0.00");  //Balance
                        dgv.Rows[i].Cells[3].Value = Convert.ToDecimal(dt.Rows[i].ItemArray[38].ToString()).ToString("#,0.00");  //Def Balance

                        dgv.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[35].ToString(); //Accnt Cr
                        dgv.Rows[i].Cells[6].Value = dt.Rows[i].ItemArray[36].ToString(); //Past Due / Deferred

                        dgv.Rows[i].Cells[7].Value = dt.Rows[i].ItemArray[1].ToString(); //typeofloan

                        //Put Applied Amount on Prev Loan
                        if (dt.Rows[i].ItemArray[1].ToString() == loanType)
                        {
                            dgv.Rows[i].Cells[4].Value = Convert.ToDecimal(dt.Rows[i].ItemArray[37].ToString()).ToString("#,0.00");
                        }
                    }
                }
            }
        }
    }
}
