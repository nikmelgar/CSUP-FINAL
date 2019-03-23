using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsSearchJournal
    {

        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        Global global = new Global();

        
        public void loadDefaultJV(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT top 20 * FROM Journal_Header Order by JV_Date DESC", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgv.DataSource = dt;

                    int colCnt = dt.Columns.Count;
                    int x = 0;


                    while (x != colCnt)
                    {
                        dgv.Columns[x].Visible = false;
                        x = x + 1;
                    }

                    dgv.Columns["JV_No"].Visible = true;
                    dgv.Columns["JV_No"].HeaderText = "JV No";

                    dgv.Columns["JV_Date"].Visible = true;
                    dgv.Columns["JV_Date"].HeaderText = "Date";

                    dgv.Columns["Particulars"].Visible = true;
                    dgv.Columns["Particulars"].HeaderText = "Particulars";
                }
            }
        }

        public void searchJV(DataGridView dgv,TextBox jv_no,TextBox particulars,Label lblError)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();


                adapter = new SqlDataAdapter("SELECT * FROM Journal_Header WHERE JV_No like '%" + jv_no.Text + "%' or Particulars like '%" + particulars.Text + "%'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgv.DataSource = dt;

                    int colCnt = dt.Columns.Count;
                    int x = 0;


                    while (x != colCnt)
                    {
                        dgv.Columns[x].Visible = false;
                        x = x + 1;
                    }

                    dgv.Columns["JV_No"].Visible = true;
                    dgv.Columns["JV_No"].HeaderText = "JV No";

                    dgv.Columns["JV_Date"].Visible = true;
                    dgv.Columns["JV_Date"].HeaderText = "Date";

                    dgv.Columns["Particulars"].Visible = true;
                    dgv.Columns["Particulars"].HeaderText = "Particulars";

                    lblError.Visible = false;
                }
                else
                {
                    lblError.Visible = true;
                    jv_no.Focus();
                }
            }

        }

        public string fullName(int userID)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("select(CASE WHEN Suffix is not null THEN(LastName + ', ' + FirstName + ' ' + MiddleName + ' ' + Suffix) ELSE(LastName + ', ' + FirstName + ' ' + MiddleName) END) as FullName From Membership WHERE userID = '" + userID + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0].ItemArray[0].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public void loadDetailsNotSummarize(DataGridView dgv, string jv_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("select Account_Code,Subsidiary_Code,Loan_No,debit,credit,userID from Journal_Detail where jv_no = '" + jv_no + "' order by debit desc,credit desc", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.Rows.Add(dt.Rows.Count);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dgv.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString();
                    dgv.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString();
                    dgv.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString();
                    dgv.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString();
                    dgv.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString();
                    if (dt.Rows[i].ItemArray[5].ToString() == "" || DBNull.Value.Equals(dt.Rows[i].ItemArray[5].ToString()))
                    {
                        dgv.Rows[i].Cells[5].Value = "0";
                    }
                    else
                    {
                        dgv.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString();
                    }
                }
            }
        }

        public void loadDetailSummary(DataGridView dgv, string jv_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetJournalDetailSummary";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@JV_No", jv_no);

                adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.Rows.Add(dt.Rows.Count);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dgv.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString();
                    dgv.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[1].ToString();
                    dgv.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[2].ToString();
                }
            }

               
        }

        public void loadTotalDebitCredit(TextBox debit,TextBox credit,string jv_no)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("select sum(debit) as debit ,sum(credit) as credit from Journal_Detail where JV_No = '" + jv_no + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    debit.Text = dt.Rows[0].ItemArray[0].ToString();
                    credit.Text = dt.Rows[0].ItemArray[1].ToString();
                }
                else
                {
                    debit.Text = "0.00";
                    credit.Text = "0.00;";
                }
            }
        }
    }
}
