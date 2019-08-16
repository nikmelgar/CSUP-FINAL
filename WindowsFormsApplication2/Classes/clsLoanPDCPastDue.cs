using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;


namespace WindowsFormsApplication2.Classes
{
    class clsLoanPDCPastDue
    {
        Global global = new Global();

        public void checkPDC(DataGridView dgv,Label cnt,DateTimePicker dtRunDate)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmdGetAllList = new SqlCommand();
                cmdGetAllList.Connection = con;
                cmdGetAllList.CommandText = "sp_ReturnListOfPDCLoan";
                cmdGetAllList.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adapterList = new SqlDataAdapter(cmdGetAllList);
                DataSet dsList = new DataSet();
                adapterList.Fill(dsList);

                dgv.Rows.Clear();

                
                for(int x = 0; x < dsList.Tables[0].Rows.Count; x++)
                {
                    SqlCommand cmdCheckPaymentOr = new SqlCommand();
                    cmdCheckPaymentOr.Connection = con;
                    cmdCheckPaymentOr.CommandText = "sp_CheckLoanNoPDCPayment";
                    cmdCheckPaymentOr.CommandType = CommandType.StoredProcedure;
                    cmdCheckPaymentOr.Parameters.AddWithValue("@Loan_No", dsList.Tables[0].Rows[x]["Loan_No"].ToString());
                    cmdCheckPaymentOr.Parameters.AddWithValue("@RunDate", dtRunDate.Text);

                    SqlDataAdapter adapterCheck = new SqlDataAdapter(cmdCheckPaymentOr);
                    DataSet dsCheck = new DataSet();
                    adapterCheck.Fill(dsCheck);
                    
                    if(dsCheck.Tables[0].Rows.Count == 0)
                    {
                        //ADD TO DATAGRIDVIEW
                        dgv.Rows.Add(
                            dsList.Tables[0].Rows[x]["userID"].ToString(),
                            dsList.Tables[0].Rows[x]["EmployeeID"].ToString(),
                            dsList.Tables[0].Rows[x]["Loan_No"].ToString(),
                            dsList.Tables[0].Rows[x]["Loan_Type"].ToString(),
                            Convert.ToDecimal(dsList.Tables[0].Rows[x]["Balance"].ToString()).ToString("#,0.00"),
                            Convert.ToDecimal(dsList.Tables[0].Rows[x]["Deferred"].ToString()).ToString("#,0.00")
                            );
                    }
                }
                cnt.Text = dgv.Rows.Count.ToString();
                
                if(dgv.Rows.Count == 0)
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }
            }
        }
    }
}
