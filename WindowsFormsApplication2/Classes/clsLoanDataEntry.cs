using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2.Classes
{
    class clsLoanDataEntry
    {
        public static int userID { get; set; }
        public static decimal loan_amount { get; set; }

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();
        public string returnCompanyDescription(int userID)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Company_Code FROM Membership WHERE userID='" + userID + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                //Return Company Description

                SqlDataAdapter CompanyAdapter = new SqlDataAdapter("SELECT Description FROM Company WHERE Company_Code = '" + dt.Rows[0].ItemArray[0].ToString() + "'", con);
                DataTable dtCompany = new DataTable();
                CompanyAdapter.Fill(dtCompany);

                if (dtCompany.Rows.Count > 0)
                {
                    return dtCompany.Rows[0].ItemArray[0].ToString();
                }
                else
                {
                    return "";
                }

            }

        }

        public string returnStatusDescription(int id)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM status_loan WHERE id = '" + id + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0].ItemArray[1].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public Boolean isShortTerm(string loan_Type)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT isShortTerm FROM loan_co_maker_no WHERE loan_type = '" + loan_Type + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows[0].ItemArray[0].ToString() == "True")
                {
                    return true; //Short Term Loan = 6 Co Makers
                }
                else
                {
                    return false; //Long Term Depends on AMOUNT
                }
            }
             
        }

        public int GetDot(string text, string stopAt = ".")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    string str = text.Substring(0, charLocation);
                    
                    int ans = Convert.ToInt32(str);
                    return ans + 1;
                }
            }

            return 0;
        }

        public decimal returnShareCapital(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetShareCapitalBalancePerMember";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userID", userid);

                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0].ItemArray[0] != DBNull.Value)
                    {
                        return Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        //With existing Loan for approval
        public Boolean checkIfSameLoanType(string loan_type,int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM vw_LoanDisplay WHERE userID = '" + userID + "' and type = '" + loan_type + "' and Status not in ('3','5','7')", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //Already has a exisiting loan with same type
                    if (dt.Rows[0].ItemArray[29].ToString() == "8")
                    {
                        Alert.show("Account with the same Loan type waiting for release", Alert.AlertType.error);
                    }
                    else if (dt.Rows[0].ItemArray[29].ToString() == "1")
                    {
                        Alert.show("Account with the same Loan type pending for approval", Alert.AlertType.error);
                    }
                    else if (dt.Rows[0].ItemArray[29].ToString() == "6")
                    {
                        Alert.show("Account with the same Loan type for board approval", Alert.AlertType.error);
                    }
                    else
                    {
                        Alert.show("Account with the same Loan type already approved", Alert.AlertType.error);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Boolean checkIfGreaterThanLoanableAmount(TextBox amount,string loan_Type)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Max_loan_Amount FROM Loan_Type WHERE Loan_Type = '" + loan_Type + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                decimal maxAmnt;

                maxAmnt = Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());

                if (Convert.ToDecimal(amount.Text) > maxAmnt)
                {
                    Alert.show("Please input valid amount!", Alert.AlertType.error);
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
