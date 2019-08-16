using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Dashboard.DashboardClasses
{
    class clsDailyProductivityStaffAccnt
    {
        Global global = new Global();
        int totalCnt;

        #region ALL VOUCHER
        //======================================================================
        //                      ALL VOUCHER
        //======================================================================
        public void loadAllVoucher(Label lblTotalCount, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                dgv.Rows.Clear();
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
                    for (int x = 0; x < dsOr.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(dsOr.Tables[0].Rows[x]["Or_No"].ToString(),
                            "OR",
                            "OR#" + dsOr.Tables[0].Rows[x]["Or_No"].ToString(),
                            Convert.ToDateTime(dsOr.Tables[0].Rows[x]["Or_Date"].ToString()).ToShortDateString(),
                            dsOr.Tables[0].Rows[x]["Prepared_By"].ToString(),
                            dsOr.Tables[0].Rows[x]["Audited_By"].ToString());
                    }

                    totalCnt = dsOr.Tables[0].Rows.Count;
                }
                //============================================================================================
                //                              END
                //============================================================================================

                //============================================================================================
                //                      JOURNAL VOUCHER
                //============================================================================================
                SqlDataAdapter adapterJV = new SqlDataAdapter("SELECT * FROM vw_JournalDaily WHERE Prepared_By = '" + Classes.clsUser.Username + "'",con);
                DataSet dsJV = new DataSet();
                adapterJV.Fill(dsJV);

                if (dsJV.Tables[0].Rows.Count > 0)
                {
                    if (dgv.Rows.Count > 0)
                    {
                        //ADD NEW ROW AND REPAINT
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewCellStyle style = new DataGridViewCellStyle();
                        style.BackColor = System.Drawing.Color.SeaGreen; // the color change
                        row.DefaultCellStyle = style;

                        dgv.Rows.Add(row);
                    }

                    for (int x = 0; x < dsJV.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(dsJV.Tables[0].Rows[x]["JV_No"].ToString(),
                            "JV",
                            "JV#" + dsJV.Tables[0].Rows[x]["JV_No"].ToString(),
                            Convert.ToDateTime(dsJV.Tables[0].Rows[x]["JV_Date"].ToString()).ToShortDateString(),
                            dsJV.Tables[0].Rows[x]["Prepared_By"].ToString(),
                            dsJV.Tables[0].Rows[x]["Audited_By"].ToString());
                    }

                    totalCnt = totalCnt + dsJV.Tables[0].Rows.Count;
                }
                //============================================================================================
                //                              END
                //============================================================================================

                //============================================================================================
                //                      DISBURSEMENT VOUCHER
                //=============================================================================================
                SqlDataAdapter adapterCV = new SqlDataAdapter("SELECT * FROM vw_DisbursementDaily WHERE Prepared_By = '" + Classes.clsUser.Username + "'",con);
                DataSet dsCV = new DataSet();
                adapterCV.Fill(dsCV);

                if (dsCV.Tables[0].Rows.Count > 0)
                {
                    if (dgv.Rows.Count > 0)
                    {
                        //ADD NEW ROW AND REPAINT
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewCellStyle style = new DataGridViewCellStyle();
                        style.BackColor = System.Drawing.Color.SeaGreen; // the color change
                        row.DefaultCellStyle = style;

                        dgv.Rows.Add(row);
                    }

                    for (int x = 0; x < dsCV.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(dsCV.Tables[0].Rows[x]["CV_No"].ToString(),
                            "CV",
                            "CV#" + dsCV.Tables[0].Rows[x]["CV_No"].ToString(),
                            Convert.ToDateTime(dsCV.Tables[0].Rows[x]["CV_Date"].ToString()).ToShortDateString(),
                            dsCV.Tables[0].Rows[x]["Prepared_By"].ToString(),
                            dsCV.Tables[0].Rows[x]["Audited_By"].ToString());
                    }

                    totalCnt = totalCnt + dsCV.Tables[0].Rows.Count;
                }
                //============================================================================================
                //                              END
                //============================================================================================

                lblTotalCount.Text = totalCnt.ToString();
            }
        }
        #endregion

        #region ALL VOUCHER AUDIT
        //======================================================================
        //                      ALL VOUCHER
        //======================================================================
        public void loadAllVoucherAudit(Label lblTotalCount, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                dgv.Rows.Clear();
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
                    for (int x = 0; x < dsOr.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(dsOr.Tables[0].Rows[x]["Or_No"].ToString(),
                            "OR",
                            "OR#" + dsOr.Tables[0].Rows[x]["Or_No"].ToString(),
                            Convert.ToDateTime(dsOr.Tables[0].Rows[x]["Or_Date"].ToString()).ToShortDateString(),
                            dsOr.Tables[0].Rows[x]["Prepared_By"].ToString(),
                            dsOr.Tables[0].Rows[x]["Audited_By"].ToString());
                    }

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
                    if (dgv.Rows.Count > 0)
                    {
                        //ADD NEW ROW AND REPAINT
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewCellStyle style = new DataGridViewCellStyle();
                        style.BackColor = System.Drawing.Color.SeaGreen; // the color change
                        row.DefaultCellStyle = style;

                        dgv.Rows.Add(row);
                    }

                    for (int x = 0; x < dsJV.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(dsJV.Tables[0].Rows[x]["JV_No"].ToString(),
                            "JV",
                            "JV#" + dsJV.Tables[0].Rows[x]["JV_No"].ToString(),
                            Convert.ToDateTime(dsJV.Tables[0].Rows[x]["JV_Date"].ToString()).ToShortDateString(),
                            dsJV.Tables[0].Rows[x]["Prepared_By"].ToString(),
                            dsJV.Tables[0].Rows[x]["Audited_By"].ToString());
                    }

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
                    if (dgv.Rows.Count > 0)
                    {
                        //ADD NEW ROW AND REPAINT
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewCellStyle style = new DataGridViewCellStyle();
                        style.BackColor = System.Drawing.Color.SeaGreen; // the color change
                        row.DefaultCellStyle = style;

                        dgv.Rows.Add(row);
                    }

                    for (int x = 0; x < dsCV.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(dsCV.Tables[0].Rows[x]["CV_No"].ToString(),
                            "CV",
                            "CV#" + dsCV.Tables[0].Rows[x]["CV_No"].ToString(),
                            Convert.ToDateTime(dsCV.Tables[0].Rows[x]["CV_Date"].ToString()).ToShortDateString(),
                            dsCV.Tables[0].Rows[x]["Prepared_By"].ToString(),
                            dsCV.Tables[0].Rows[x]["Audited_By"].ToString());
                    }

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
