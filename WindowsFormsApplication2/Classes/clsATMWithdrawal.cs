using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsATMWithdrawal
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

            adapter = new SqlDataAdapter("SELECT * FROM vw_SavingsWithdrawalATM ORDER BY Bank_Code", con);
            dt = new DataTable();
            adapter.Fill(dt);

            dgv.DataSource = dt;

            dgv.Columns["userID"].Visible = false;
            dgv.Columns["Posted"].Visible = false;
            dgv.Columns["Cancelled"].Visible = false;
            dgv.Columns["Withdrawal_Mode"].Visible = false;
            dgv.Columns["forAtmPrep"].Visible = false;

            dgv.Columns["AmtWithdrawn"].HeaderText = "Amount";
            dgv.Columns["Slip_No"].HeaderText = "Slip No";
            dgv.Columns["EmployeeID"].HeaderText = "ID";
            dgv.Columns["Atm_Account_No"].HeaderText = "Account No";
            dgv.Columns["Bank_Code"].HeaderText = "Bank";
        }

        public void loadBank(ComboBox cmb)
        {
            cmb.DataSource = null;

            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT (Bank_Code + ' - '+ Bank_Name) as Bank_Name,Bank_Code FROM Bank WHERE isActive = 1", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            cmb.DisplayMember = "Bank_Name";
            cmb.ValueMember = "Bank_Code"; ;
            cmb.DataSource = dt;
        }
        
    }
}
