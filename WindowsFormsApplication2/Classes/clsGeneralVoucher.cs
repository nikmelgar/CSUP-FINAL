using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsGeneralVoucher
    {

        SqlCommand cmd;
        SqlConnection con;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();


        public string returnSavings(string loanNumber)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Applied_Amount From Loan_Deductions WHERE loan_No = '" + loanNumber + "' and Other_Deduction = '300.1'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                return Convert.ToString(Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString()).ToString("#,0.00"));
            }
            else
            {
                return "0.00";
            }
        }

        public string returnShareCapital(string loanNumber)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Applied_Amount From Loan_Deductions WHERE loan_No = '" + loanNumber + "' and Other_Deduction = '363'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return Convert.ToString(Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString()).ToString("#,0.00"));
            }
            else
            {
                return "0.00";
            }
        }

        public string returnTotalDeduction(string loanNumber)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Loan_Amount,NetProceeds FROM Loan WHERE Loan_No = '"+ loanNumber +"'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            double ans;
            ans = Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString()) - Convert.ToDouble(dt.Rows[0].ItemArray[1].ToString());

            return Convert.ToDecimal(ans).ToString("#,0.00");
        }
    }
}
