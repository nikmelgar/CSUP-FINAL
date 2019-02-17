using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsSearchDisbursement
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();


        public void loadDefaultDisbursement(DataGridView dgv)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT top 20 * FROM Disbursement_Header Order by CV_Date DESC", con);
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

                dgv.Columns["CV_No"].Visible = true;
                dgv.Columns["CV_No"].HeaderText = "CV No";

                dgv.Columns["CV_Date"].Visible = true;
                dgv.Columns["CV_Date"].HeaderText = "Date";

                dgv.Columns["Particulars"].Visible = true;
                dgv.Columns["Particulars"].HeaderText = "Particulars";
            }
        }

        public void searchCV(DataGridView dgv, TextBox cv_no, TextBox particulars,Label lblError)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT * FROM Disbursement_Header WHERE CV_No like '%" + cv_no.Text + "%' or Particulars like '%" + particulars.Text + "%'", con);
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

                dgv.Columns["CV_No"].Visible = true;
                dgv.Columns["CV_No"].HeaderText = "CV No";

                dgv.Columns["CV_Date"].Visible = true;
                dgv.Columns["CV_Date"].HeaderText = "Date";

                dgv.Columns["Particulars"].Visible = true;
                dgv.Columns["Particulars"].HeaderText = "Particulars";

                lblError.Visible = false;
            }
            else
            {
                lblError.Visible = true;
                cv_no.Focus();
            }
        }

        public string fullName(int userID)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("select (CASE WHEN Suffix is not null THEN(LastName+', '+FirstName+ ' ' + MiddleName +' '+ Suffix) ELSE (LastName+', '+FirstName+ ' ' + MiddleName) END) as FullName From Membership WHERE userID = '"+ userID +"'", con);
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

        public string ClientName(string clientCode)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Name FROM Client WHERE Client_Code ='"+ clientCode +"'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray[0].ToString();
            }
            else
            {
                return "";
            }
        }

        public void loadDetails(DataGridView dgv, string cv_no)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("select Account_Code,Subsidiary_Code,Loan_No,debit,credit,userID from Disbursement_Detail where cv_no = '" + cv_no + "'", con);
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
                if(dt.Rows[i].ItemArray[5].ToString() == "" || DBNull.Value.Equals(dt.Rows[i].ItemArray[5].ToString()))
                {
                    dgv.Rows[i].Cells[5].Value = "0";
                }
                else
                {
                    dgv.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString();
                }
  
            }


        }

        public void loadTotalDebitCredit(TextBox debit, TextBox credit, string cv_no)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("select sum(debit) as debit ,sum(credit) as credit from Disbursement_Detail where CV_No = '" + cv_no + "'", con);
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
