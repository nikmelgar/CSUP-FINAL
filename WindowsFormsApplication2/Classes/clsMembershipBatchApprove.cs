using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsMembershipBatchApprove
    {
        Global global = new Global();
        SqlConnection con;
        public void loadAllMembersForApproval(DataGridView dgv)
        {

            dgv.DataSource = null;
            dgv.Rows.Clear();
            

            con = new SqlConnection();
            global.connection(con);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "sp_SelectAllMembersForApproval";
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            dt.Clear();
            adapter.Fill(dt);

            dgv.DataSource = dt;

            int colCnt = dt.Columns.Count;
            int x = 0;


            while (x != colCnt)
            {
                dgv.Columns[x].Visible = false;
                dgv.Columns[x].ReadOnly = true;
                x = x + 1;
            }

            dgv.Columns["EmployeeID"].Visible = true;
            dgv.Columns["EmployeeID"].HeaderText = "Employee ID";

            dgv.Columns["LastName"].Visible = true;
            dgv.Columns["LastName"].HeaderText = "Last Name";

            dgv.Columns["FirstName"].Visible = true;
            dgv.Columns["FirstName"].HeaderText = "First Name";

            dgv.Columns["MiddleName"].Visible = true;
            dgv.Columns["MiddleName"].HeaderText = "Middle Name";

            dgv.Columns["Date_Of_Birth"].Visible = true;
            dgv.Columns["Date_Of_Birth"].HeaderText = "Birthday";

            dgv.Columns["IsApprove"].Visible = true;
            dgv.Columns["IsApprove"].FillWeight = 60;
            dgv.Columns["IsApprove"].HeaderText = "Approve";
            dgv.Columns["IsApprove"].ReadOnly = false;


        }

        public void loadMembersByDate(DataGridView dgv,DateTimePicker dt1,DateTimePicker dt2 )
        {
            dgv.DataSource = null;

            con = new SqlConnection();
            global.connection(con);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "sp_SelectMembersByDate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@value1", dt1.Value);
            cmd.Parameters.AddWithValue("@value2", dt2.Value);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dgv.DataSource = dt;

            int colCnt = dt.Columns.Count;
            int x = 0;


            while (x != colCnt)
            {
                dgv.Columns[x].Visible = false;
                dgv.Columns[x].ReadOnly = true;
                x = x + 1;
            }

            dgv.Columns["IsApprove"].Visible = true;
            dgv.Columns["IsApprove"].FillWeight = 60;
            dgv.Columns["IsApprove"].HeaderText = "Approve";
            dgv.Columns["IsApprove"].ReadOnly = false;

            dgv.Columns["EmployeeID"].Visible = true;
            dgv.Columns["EmployeeID"].HeaderText = "Employee ID";

            dgv.Columns["LastName"].Visible = true;
            dgv.Columns["LastName"].HeaderText = "Last Name";

            dgv.Columns["FirstName"].Visible = true;
            dgv.Columns["FirstName"].HeaderText = "First Name";

            dgv.Columns["MiddleName"].Visible = true;
            dgv.Columns["MiddleName"].HeaderText = "Middle Name";

            dgv.Columns["Date_Of_Birth"].Visible = true;
            dgv.Columns["Date_Of_Birth"].HeaderText = "Birthday";
        }


    }
}
