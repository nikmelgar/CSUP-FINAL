using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace WindowsFormsApplication2.Dashboard.DashboardClasses
{
    class clsDashboardTH
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

                //============================================================================================
                //                      JOURNAL VOUCHER
                //============================================================================================
                SqlDataAdapter adapterJV = new SqlDataAdapter("SELECT * FROM vw_Journal_List_TH", con);
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
                SqlDataAdapter adapterCV = new SqlDataAdapter("SELECT * FROM vw_Disbursement_List_TH", con);
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


        #region Teams Work List
        public void teamsWorkListCount(Label lblTotalCount)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                
                lblTotalCount.Text = "";
                totalCnt = 0;

                //============================================================================================
                //                      RECEIPT VOUCHER
                //============================================================================================
                SqlDataAdapter adapterOR = new SqlDataAdapter("SELECT * FROM vw_CashReceiptsDaily WHERE DepartmentID = '1' and RoleID <> '"+ Classes.clsUser.role +"'", con);
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
                SqlDataAdapter adapterJV = new SqlDataAdapter("SELECT * FROM vw_JournalDaily WHERE DepartmentID = '1' and RoleID <> '" + Classes.clsUser.role + "'", con);
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
                SqlDataAdapter adapterCV = new SqlDataAdapter("SELECT * FROM vw_DisbursementDaily WHERE DepartmentID = '1' and RoleID <> '" + Classes.clsUser.role + "'", con);
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


        #region Incoming Work List
        public void IncomingWork(Label lblTotalCount)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                lblTotalCount.Text = "";
                totalCnt = 0;

                //============================================================================================
                //                      RECEIPT VOUCHER
                //============================================================================================
                SqlDataAdapter adapterOR = new SqlDataAdapter("SELECT * FROM vw_CashReceiptIncoming", con);
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
                SqlDataAdapter adapterJV = new SqlDataAdapter("SELECT * FROM vw_JournalIncoming", con);
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
                SqlDataAdapter adapterCV = new SqlDataAdapter("SELECT * FROM vw_DisbursementIncoming", con);
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
                SqlDataAdapter adapterJV = new SqlDataAdapter("SELECT * FROM vw_JournalDailyProductivity WHERE Posted_By = '" + Classes.clsUser.Username + "'", con);
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
                SqlDataAdapter adapterCV = new SqlDataAdapter("SELECT * FROM vw_DisbursementDailyProductivity WHERE Posted_By = '" + Classes.clsUser.Username + "'", con);
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

        public void gettingTeamsUsername(ComboBox cmb)
        {
            cmb.DataSource = null;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();


                SqlDataAdapter adapter = new SqlDataAdapter("select Username from Users where DepartmentID = '"+ Classes.clsUser.department +"' and RoleID <> '"+ Classes.clsUser.role +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmb.DisplayMember = dt.Columns[0].ToString();
                cmb.ValueMember = dt.Columns[0].ToString();
                cmb.DataSource = dt;
            }
        }



        #region Worklist for Audit
        public void workListAudit(Label lblTotalCount)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                lblTotalCount.Text = "";
                totalCnt = 0;

                //============================================================================================
                //                      RECEIPT VOUCHER
                //============================================================================================
                SqlDataAdapter adapterOR = new SqlDataAdapter("SELECT * FROM vw_CashReceipt_List_TH_Audit", con);
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
                SqlDataAdapter adapterJV = new SqlDataAdapter("SELECT * FROM vw_Journal_List_TH_Audit", con);
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
                SqlDataAdapter adapterCV = new SqlDataAdapter("SELECT * FROM vw_Disbursement_List_TH_Audit", con);
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


        #region Teams Work List Audit
        public void teamsWorkListCountAudit(Label lblTotalCount)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                lblTotalCount.Text = "";
                totalCnt = 0;

                //============================================================================================
                //                      RECEIPT VOUCHER
                //============================================================================================
                SqlDataAdapter adapterOR = new SqlDataAdapter("SELECT * FROM vw_CashReceiptsDaily WHERE Audited_By <> '" + Classes.clsUser.Username + "'", con);
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
                SqlDataAdapter adapterJV = new SqlDataAdapter("SELECT * FROM vw_JournalDaily WHERE Audited_By <> '" + Classes.clsUser.Username + "'", con);
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
                SqlDataAdapter adapterCV = new SqlDataAdapter("SELECT * FROM vw_DisbursementDaily WHERE Audited_By <> '" + Classes.clsUser.Username + "'", con);
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


        #region Daily Productivity Audit
        public void DailyProductivityAudit(Label lblTotalCount)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                lblTotalCount.Text = "";
                totalCnt = 0;

                //============================================================================================
                //                      RECEIPT VOUCHER
                //============================================================================================
                SqlDataAdapter adapterOR = new SqlDataAdapter("SELECT * FROM vw_CashReceiptsDaily WHERE Audited_By = '" + Classes.clsUser.Username + "'", con);
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
                SqlDataAdapter adapterJV = new SqlDataAdapter("SELECT * FROM vw_JournalDaily WHERE Audited_By = '" + Classes.clsUser.Username + "'", con);
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
                SqlDataAdapter adapterCV = new SqlDataAdapter("SELECT * FROM vw_DisbursementDaily WHERE Audited_By = '" + Classes.clsUser.Username + "'", con);
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
