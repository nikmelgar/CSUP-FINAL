using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace WindowsFormsApplication2.Classes
{
    class clsLoanType
    {
        SqlCommand cmd;
        SqlConnection con;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();
        public void displayLoans(DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM loan_Type WHERE Loan_Type <> ''", con);
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

                    dgv.Columns["Loan_Type"].Visible = true;
                    dgv.Columns["Loan_Type"].HeaderText = "Loan Type";
                    dgv.Columns["Loan_Type"].FillWeight = 50;

                    dgv.Columns["Loan_Description"].Visible = true;
                    dgv.Columns["Loan_Description"].HeaderText = "Description";
                    dgv.Columns["Loan_Description"].FillWeight = 160;

                    dgv.Columns["Min_Loan_Amount"].Visible = true;
                    dgv.Columns["Min_Loan_Amount"].HeaderText = "Min Loan Amount";

                    dgv.Columns["Max_Loan_Amount"].Visible = true;
                    dgv.Columns["Max_Loan_Amount"].HeaderText = "Max Loan Amount";

                    dgv.Columns["Min_Term"].Visible = true;
                    dgv.Columns["Min_Term"].HeaderText = "Min Terms";
                    dgv.Columns["Min_Term"].FillWeight = 50;

                    dgv.Columns["Max_Term"].Visible = true;
                    dgv.Columns["Max_Term"].HeaderText = "Max Terms";
                    dgv.Columns["Max_Term"].FillWeight = 50;


                }
            }
        }

        public void loadComboBox(ComboBox cmb)
        {
            cmb.DataSource = null;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("select (Account_Code + ' - ' + Account_Description) as Accnt, Account_Code from chart_of_accounts Where Parent_Account <> '0' and LevelNo <> '1'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmb.DisplayMember = "Accnt";
                cmb.ValueMember = "Account_Code";
                cmb.DataSource = dt;
            }
        }

        public void loadServiceFactore(ComboBox cmb)
        {
            cmb.DataSource = null;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("select (Service_Code + ' - ' + format(cast(Service_Percent as decimal(18,2)),'##,#.####', 'en-US')) as [Service], Service_Code from Service_Factor", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmb.DisplayMember = "Service";
                cmb.ValueMember = "Service_Code";
                cmb.DataSource = dt;
            }
        }

        public Boolean checkIfAlreadyUsed(int priority)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM Loan_Type WHERE Priority ='" + priority + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }            
        }
    }
}
