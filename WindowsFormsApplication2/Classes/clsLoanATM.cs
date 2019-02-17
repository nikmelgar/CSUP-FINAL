using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace WindowsFormsApplication2.Classes
{
    class clsLoanATM
    {

        //Call GLobal COnnection
        Global global = new Global();

        //Declaration
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        public void loadATMWithdrawal(DataGridView dgv)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT * FROM vw_LoanToATM", con);
            dt = new DataTable();
            adapter.Fill(dt);

            dgv.DataSource = dt;

            dgv.Columns["userID"].Visible = false;
            dgv.Columns["Status"].Visible = false;

            dgv.Columns["EmployeeID"].HeaderText = "ID";
            dgv.Columns["Bank_Code"].HeaderText = "Bank";
        }

        public void search(TextBox empID,TextBox lastName, TextBox firstName, TextBox LoanNo, ComboBox Bank, DataGridView dgv)
        {
            

            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT * FROM vw_LoanToATM WHERE EmployeeID like '%"+ empID.Text +"%' and LastName like '%"+ lastName.Text +"%' and FirstName like '%"+ LoanNo.Text +"%' and Bank_Code Like '%"+ Bank.Text +"%'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            dgv.DataSource = dt;

            dgv.Columns["userID"].Visible = false;
            dgv.Columns["Status"].Visible = false;

            dgv.Columns["EmployeeID"].HeaderText = "ID";
            dgv.Columns["Bank_Code"].HeaderText = "Bank";
        }

        public void loadBank(ComboBox cmb)
        {
            cmb.DataSource = null;

            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT DISTINCT(Bank_Code) FROM vw_LoanToATM", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            cmb.DisplayMember = "Bank_Code";
            cmb.ValueMember = "Bank_Code"; ;
            cmb.DataSource = dt;
        }
    }
}
