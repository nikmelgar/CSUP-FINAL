using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApplication2.Classes
{
    class clsQueryCoMaker
    {

        Global global = new Global();
        Classes.clsQuery clsQuery = new Classes.clsQuery();
        //Classes.clsQuery.searchUserID
        public void getAllLoanMakers(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_CoMakers where userID = '"+ Classes.clsQuery.searchUserID  + "' and Status = 5", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                dgv.Rows.Clear();

                if(ds.Tables[0].Rows.Count > 0)
                {
                    dgv.Rows.Add(ds.Tables[0].Rows.Count);

                    for(int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows[x].Cells[0].Value = ds.Tables[0].Rows[x]["Loan_No"].ToString();
                        dgv.Rows[x].Cells[1].Value = returnLoanType(ds.Tables[0].Rows[x]["Loan_No"].ToString());
                        dgv.Rows[x].Cells[2].Value = clsQuery.returMembersNameAndID(returnUserIDLoanBorrower(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                        dgv.Rows[x].Cells[3].Value = Convert.ToDecimal(returnAmount(ds.Tables[0].Rows[x]["Loan_No"].ToString())).ToString("#,0.00");
                    }

                    loadTotals(dgv);
                }
            }
        }

        public string returnLoanType(string loan_No)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Loan_type FROM Loan WHERE Loan_No ='"+ loan_No +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }

        public int returnUserIDLoanBorrower(string loan_No)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT userID FROM Loan WHERE Loan_No ='" + loan_No + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString());
            }
        }

        public double returnAmount(string loan_No)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetLoanAmountForQuery";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@loanNo", loan_No);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                double getLoanCoMakerNumber,outstandingBalance,amnt;

                SqlDataAdapter adapterGetNumber = new SqlDataAdapter("SELECT count(*) FROM vw_CoMakers WHERE Loan_No = '"+ loan_No +"'", con);
                DataTable dt = new DataTable();
                adapterGetNumber.Fill(dt);

                getLoanCoMakerNumber = Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString());
                outstandingBalance = Convert.ToDouble(ds.Tables[0].Rows[0]["Balance"].ToString());
                amnt = outstandingBalance / getLoanCoMakerNumber;
                return amnt;
            }
        }

        public void loadTotals(DataGridView dgv)
        {
            dgv.Rows.Add(1);
            
            decimal sumBalance = 0;
            //Check if theres a beneficiary
            if (dgv.Rows.Count > 0)
            {
                for (int i = 0; i < dgv.Rows.Count; ++i)
                {
                    sumBalance += Convert.ToDecimal(dgv.Rows[i].Cells[3].Value);
                }
            }

            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[2].Value = "Totals";
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[3].Value = sumBalance.ToString("#,0.00");


            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[2].Style.Font = new System.Drawing.Font("Calibri", 10, FontStyle.Bold);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[3].Style.Font = new System.Drawing.Font("Calibri", 10, FontStyle.Bold);
        }

    }
}
