using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsPLBI
    {
        SqlCommand cmd;
        SqlConnection con;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();


        public Boolean checkLoanNo(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT * FROM Loan WHERE Loan_No = '"+ loan_no +"'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int userid(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT userid FROM Loan WHERE Loan_No = '" + loan_no + "'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            return Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString());
        }
        public string empid(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT EmployeeID FROM Loan WHERE Loan_No = '"+ loan_no +"'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            return dt.Rows[0].ItemArray[0].ToString();
        }

        public string namewithcompany(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT userID FROM Loan WHERE Loan_No = '" + loan_no + "'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT  LastName+', '+FirstName+SPACE(1)+MiddleName+SPACE(1)+Suffix , Company_Code FROM Membership WHERE userID = '" + dt.Rows[0].ItemArray[0].ToString() +"'", con);
            DataTable dt2 = new DataTable();
            adapter2.Fill(dt2);

            SqlDataAdapter adapter3 = new SqlDataAdapter("SELECT Description FROM Company WHERE Company_Code = '"+ dt2.Rows[0].ItemArray[1].ToString()+"'", con);
            DataTable dt3 = new DataTable();
            adapter3.Fill(dt3);

            return dt2.Rows[0].ItemArray[0].ToString() + " -- " + dt3.Rows[0].ItemArray[0].ToString();
        }

        public string loanNoType(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Loan_No,Loan_Type FROM Loan WHERE Loan_No = '"+ loan_no +"'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            return dt.Rows[0].ItemArray[0].ToString() + " -- " + dt.Rows[0].ItemArray[1].ToString();
        }

        public string memDate(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT userID FROM Loan WHERE Loan_No = '" + loan_no + "'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT Date_Of_Membership FROM Membership WHERE userID = '" + dt.Rows[0].ItemArray[0].ToString() + "'", con);
            DataTable dt2 = new DataTable();
            adapter2.Fill(dt2);

            return Convert.ToDateTime(dt2.Rows[0].ItemArray[0].ToString()).ToString("MM/dd/yyyy");
        }

        public string pmsDate(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT userID FROM Loan WHERE Loan_No = '" + loan_no + "'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT Date_Of_PMS FROM Membership WHERE userID = '" + dt.Rows[0].ItemArray[0].ToString() + "'", con);
            DataTable dt2 = new DataTable();
            adapter2.Fill(dt2);

            return Convert.ToDateTime(dt2.Rows[0].ItemArray[0].ToString()).ToString("MM/dd/yyyy");
        }

        public string shareCapital(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT userID FROM Loan WHERE Loan_No = '" + loan_no + "'", con);
            DataTable dt2 = new DataTable();
            adapter2.Fill(dt2);

            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "sp_GetShareCapitalBalancePerMember";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@userID", dt2.Rows[0].ItemArray[0].ToString());

            adapter = new SqlDataAdapter(cmd);
            dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0].ItemArray[0] != DBNull.Value)
                {
                    return Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString()).ToString("#,0.00");
                }
                else
                {
                    return "0.00";
                }
            }
            else
            {
                return "0.00";
            }
        }

        public string savings(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT userID FROM Loan WHERE Loan_No = '" + loan_no + "'", con);
            DataTable dt2 = new DataTable();
            adapter2.Fill(dt2);

            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "sp_GetSavingsPerUser";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@userID", dt2.Rows[0].ItemArray[0].ToString());

            adapter = new SqlDataAdapter(cmd);
            dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0].ItemArray[0] != DBNull.Value)
                {
                    return Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString()).ToString("#,0.00");
                }
                else
                {
                    return "0.00";
                }
            }
            else
            {
                return "0.00";
            }
        }

        public string scPerDay(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT userID FROM Loan WHERE Loan_No = '" + loan_no + "'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT Share_Capital FROM Membership WHERE userID = '" + dt.Rows[0].ItemArray[0].ToString() + "'", con);
            DataTable dt2 = new DataTable();
            adapter2.Fill(dt2);

            return Convert.ToDecimal(dt2.Rows[0].ItemArray[0].ToString()).ToString("#,0.00");
        }

        public string SavingsPerday(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT userID FROM Loan WHERE Loan_No = '" + loan_no + "'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT Savings_Deposit FROM Membership WHERE userID = '" + dt.Rows[0].ItemArray[0].ToString() + "'", con);
            DataTable dt2 = new DataTable();
            adapter2.Fill(dt2);

            return Convert.ToDecimal(dt2.Rows[0].ItemArray[0].ToString()).ToString("#,0.00");
        }

        public string Bank(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT userID FROM Loan WHERE Loan_No = '" + loan_no + "'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT Bank_Code FROM Membership WHERE userID = '" + dt.Rows[0].ItemArray[0].ToString() + "'", con);
            DataTable dt2 = new DataTable();
            adapter2.Fill(dt2);

            return dt2.Rows[0].ItemArray[0].ToString();
        }

        public string Atm(string loan_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT userID FROM Loan WHERE Loan_No = '" + loan_no + "'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT Atm_Account_No FROM Membership WHERE userID = '" + dt.Rows[0].ItemArray[0].ToString() + "'", con);
            DataTable dt2 = new DataTable();
            adapter2.Fill(dt2);

            return dt2.Rows[0].ItemArray[0].ToString();
        }


    }
}
