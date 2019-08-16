using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Dashboard.DashboardClasses
{
    class clsDashboardStaff
    {
        Global global = new Global();
        int totalCnt;


        #region Worklist For Accounting

        public void WorkListCount(Label lblTotalCount)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                lblTotalCount.Text = "";
                totalCnt = 0;

                if (Classes.clsUser.department.ToString() == "3")
                {
                    //============================================================================================
                    //                      RECEIPT VOUCHER
                    //============================================================================================

                    SqlDataAdapter adapterOR = new SqlDataAdapter("SELECT * FROM vw_CashReceipt_List_TH", con);
                    DataSet dsOr = new DataSet();
                    adapterOR.Fill(dsOr);

                    if (dsOr.Tables[0].Rows.Count > 0)
                    {
                        totalCnt = dsOr.Tables[0].Rows.Count;
                    }

                    //============================================================================================
                    //                              END
                    //============================================================================================
                }

                //============================================================================================
                //                      JOURNAL VOUCHER
                //============================================================================================
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_Journal_Worklist_Staff";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@username", Classes.clsUser.Username);

                SqlDataAdapter adapterJV = new SqlDataAdapter(cmd);
                DataSet dsJV = new DataSet();
                adapterJV.Fill(dsJV);

                if (dsJV.Tables[0].Rows.Count > 0)
                {
                    totalCnt = totalCnt + dsJV.Tables[0].Rows.Count;
                }
                //============================================================================================
                //                              END
                //============================================================================================
                

                //============================================================================================
                //                      DISBURSEMENT VOUCHER
                //============================================================================================
                SqlCommand cmd2 = new SqlCommand();
                cmd2.Connection = con;
                cmd2.CommandText = "sp_Disbursement_Worklist_Staff";
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.AddWithValue("@username", Classes.clsUser.Username);

                SqlDataAdapter adapterCV = new SqlDataAdapter(cmd2);
                DataSet dsCV = new DataSet();
                adapterCV.Fill(dsCV);

                if (dsCV.Tables[0].Rows.Count > 0)
                {
                    totalCnt = totalCnt + dsCV.Tables[0].Rows.Count;
                }
                //============================================================================================
                //                              END
                //============================================================================================

                lblTotalCount.Text = totalCnt.ToString();
            }
        }

        #endregion


        #region Daily Productivity 
        public void DailyProductivity(Label lblTotalCount)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                lblTotalCount.Text = "";
                totalCnt = 0;

                //============================================================================================
                //                      RECEIPT VOUCHER
                //============================================================================================
                SqlDataAdapter adapterOR = new SqlDataAdapter("SELECT * FROM vw_CashReceiptDailyProductivity WHERE Posted_By = '" + Classes.clsUser.Username + "'", con);
                DataSet dsOr = new DataSet();
                adapterOR.Fill(dsOr);

                if (dsOr.Tables[0].Rows.Count > 0)
                {
                    totalCnt = dsOr.Tables[0].Rows.Count;
                }
                //============================================================================================
                //                              END
                //============================================================================================


                //============================================================================================
                //                      JOURNAL VOUCHER
                //============================================================================================
                SqlDataAdapter adapterJV = new SqlDataAdapter("SELECT * FROM vw_JournalDaily WHERE Prepared_By = '" + Classes.clsUser.Username + "'", con);
                DataSet dsJV = new DataSet();
                adapterJV.Fill(dsJV);

                if (dsJV.Tables[0].Rows.Count > 0)
                {
                    totalCnt = totalCnt + dsJV.Tables[0].Rows.Count;
                }
                //============================================================================================
                //                              END
                //============================================================================================


                //============================================================================================
                //                      DISBURSEMENT VOUCHER
                //============================================================================================
                SqlDataAdapter adapterCV = new SqlDataAdapter("SELECT * FROM vw_DisbursementDaily WHERE Prepared_By = '" + Classes.clsUser.Username + "'", con);
                DataSet dsCV = new DataSet();
                adapterCV.Fill(dsCV);

                if (dsCV.Tables[0].Rows.Count > 0)
                {
                    totalCnt = totalCnt + dsCV.Tables[0].Rows.Count;
                }
                //============================================================================================
                //                              END
                //============================================================================================

                lblTotalCount.Text = totalCnt.ToString();
            }
        }
        #endregion
    }
}
