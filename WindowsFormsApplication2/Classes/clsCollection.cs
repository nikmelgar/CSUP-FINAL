using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsCollection
    {
        Global global = new Global();

        public decimal GetTotalAR(string CompanyCode,string PayrollCode, DateTimePicker dtBillDate)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT SUM(AppliedAmount) FROM Collection WHERE Company_Code = '" + CompanyCode + "' and Payroll_Code = '" + PayrollCode + "' and Payroll_Date = '"+ dtBillDate.Text +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());
            }
        }

        public string GetSubsidiary(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT EmployeeID +' - '+ CASE WHEN membership.suffix IS NOT NULL THEN dbo.Membership.LastName + ', ' + dbo.Membership.FirstName + SPACE(1) + dbo.Membership.MiddleName + SPACE(1) + dbo.Membership.Suffix ELSE dbo.Membership.LastName + ', ' + dbo.Membership.FirstName + SPACE(1) + dbo.Membership.MiddleName END FROM Membership WHERE userID = '" + userid + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }
    }
}
