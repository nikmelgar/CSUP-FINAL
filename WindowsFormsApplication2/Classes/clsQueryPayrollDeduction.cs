using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;

namespace WindowsFormsApplication2.Classes
{
    class clsQueryPayrollDeduction
    {

        Global global = new Global();
        

        public void displayPayrollDeduction(DataGridView dgv,int userid,string dtPayroll)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetPayrollDeductionPerDate";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@payrollDate", dtPayroll.ToString());

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if(ds.Tables[0].Rows.Count > 0)
                {
                    dgv.Rows.Clear();

                    for(int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(ds.Tables[0].Rows[x]["Account_Description"].ToString(),
                                ds.Tables[0].Rows[x]["Loan_No"].ToString(),
                                Convert.ToDecimal(ds.Tables[0].Rows[x]["DueAmount"].ToString()).ToString("#,0.00"),
                                Convert.ToDecimal(ds.Tables[0].Rows[x]["AppliedAmount"].ToString()).ToString("#,0.00"),
                                Convert.ToDecimal(ds.Tables[0].Rows[x]["DeferredAmount"].ToString()).ToString("#,0.00"));
                    }

                    loadTotals(dgv);
                }
                else
                {
                    dgv.Rows.Clear();
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }
            }
        }

        public void loadTotals(DataGridView dgv)
        {
            dgv.Rows.Add(1);

            decimal DueAmount = 0;
            decimal AppliedAmount = 0;
            decimal DeferredAmount = 0;
            //Check if theres a beneficiary
            if (dgv.Rows.Count > 0)
            {
                for (int i = 0; i < dgv.Rows.Count; ++i)
                {
                    DueAmount += Convert.ToDecimal(dgv.Rows[i].Cells["DueAmount"].Value);
                    AppliedAmount += Convert.ToDecimal(dgv.Rows[i].Cells["AppliedAmount"].Value);
                    DeferredAmount += Convert.ToDecimal(dgv.Rows[i].Cells["DeferredAmount"].Value);
                }
            }

            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Loan_No"].Value = "Totals";
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["DueAmount"].Value = DueAmount.ToString("#,0.00");
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["AppliedAmount"].Value = AppliedAmount.ToString("#,0.00");
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["DeferredAmount"].Value = DeferredAmount.ToString("#,0.00");

            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Loan_No"].Style.Font = new System.Drawing.Font("Calibri", 10, FontStyle.Bold);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Loan_No"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["DueAmount"].Style.Font = new System.Drawing.Font("Calibri", 10, FontStyle.Bold);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["AppliedAmount"].Style.Font = new System.Drawing.Font("Calibri", 10, FontStyle.Bold);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["DeferredAmount"].Style.Font = new System.Drawing.Font("Calibri", 10, FontStyle.Bold);

        }

    }
}
