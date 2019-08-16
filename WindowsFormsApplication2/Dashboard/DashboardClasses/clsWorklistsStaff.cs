using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Dashboard.DashboardClasses
{
    class clsWorklistsStaff
    {
        Global global = new Global();
        int totalCnt;

        #region Load journal
        public void loadJournalVoucher(Label lblTotalCount, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                dgv.Rows.Clear();
                lblTotalCount.Text = "";

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_Journal_Worklist_Staff";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@username", Classes.clsUser.Username);


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(ds.Tables[0].Rows[x]["JV_No"].ToString(),
                            "JV",
                            "JV#" + ds.Tables[0].Rows[x]["JV_No"].ToString(),
                            Convert.ToDateTime(ds.Tables[0].Rows[x]["JV_Date"].ToString()).ToShortDateString(),
                            ds.Tables[0].Rows[x]["Prepared_By"].ToString(),
                            ds.Tables[0].Rows[x]["Audited_By"].ToString());
                    }

                    lblTotalCount.Text = ds.Tables[0].Rows.Count.ToString();
                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }
            }
        }
        #endregion

        #region Load Disbursement
        public void loadDisbursementVoucher(Label lblTotalCount, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                dgv.Rows.Clear();
                lblTotalCount.Text = "";

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_Disbursement_Worklist_Staff";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@username", Classes.clsUser.Username);


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(ds.Tables[0].Rows[x]["CV_No"].ToString(),
                            "CV",
                            "CV#" + ds.Tables[0].Rows[x]["CV_No"].ToString(),
                            Convert.ToDateTime(ds.Tables[0].Rows[x]["JV_Date"].ToString()).ToShortDateString(),
                            ds.Tables[0].Rows[x]["Prepared_By"].ToString(),
                            ds.Tables[0].Rows[x]["Audited_By"].ToString());
                    }

                    lblTotalCount.Text = ds.Tables[0].Rows.Count.ToString();
                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }
            }
        }
        #endregion

        #region RECEIPTVoucher
        //======================================================================
        //                      RECEIPT VOUCHER
        //======================================================================
        public void loadReceiptVoucher(Label lblTotalCount, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                dgv.Rows.Clear();
                lblTotalCount.Text = "";
                
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_CashReceipt_List_TH", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(ds.Tables[0].Rows[x]["Or_No"].ToString(),
                            "OR",
                            "OR#" + ds.Tables[0].Rows[x]["Or_No"].ToString(),
                            Convert.ToDateTime(ds.Tables[0].Rows[x]["Or_Date"].ToString()).ToShortDateString(),
                            ds.Tables[0].Rows[x]["Prepared_By"].ToString(),
                            ds.Tables[0].Rows[x]["Audited_By"].ToString());
                    }

                    lblTotalCount.Text = ds.Tables[0].Rows.Count.ToString();
                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }
            }
        }
        #endregion

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

                if(Classes.clsUser.department == "3")
                {
                    //============================================================================================
                    //                      RECEIPT VOUCHER
                    //============================================================================================
                    SqlDataAdapter adapterOR = new SqlDataAdapter("SELECT * FROM vw_CashReceipt_List_TH", con);
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


        #region JournalVoucherForAudit
        //======================================================================
        //                      JOURNAL VOUCHER
        //======================================================================
        public void loadJournalVoucherAudit(Label lblTotalCount, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                dgv.Rows.Clear();
                lblTotalCount.Text = "";
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_Journal_List_TH_Audit", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(ds.Tables[0].Rows[x]["JV_No"].ToString(),
                            "JV",
                            "JV#" + ds.Tables[0].Rows[x]["JV_No"].ToString(),
                            Convert.ToDateTime(ds.Tables[0].Rows[x]["JV_Date"].ToString()).ToShortDateString(),
                            ds.Tables[0].Rows[x]["Prepared_By"].ToString(),
                            ds.Tables[0].Rows[x]["Audited_By"].ToString());
                    }

                    lblTotalCount.Text = ds.Tables[0].Rows.Count.ToString();
                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }
            }
        }
        #endregion

        #region DisbursementVoucherAudit
        //======================================================================
        //                      DISBURSEMENT VOUCHER
        //======================================================================
        public void loadDisbursementVoucherAudit(Label lblTotalCount, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                dgv.Rows.Clear();
                lblTotalCount.Text = "";
                
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_Disbursement_List_TH_Audit", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(ds.Tables[0].Rows[x]["CV_No"].ToString(),
                            "CV",
                            "CV#" + ds.Tables[0].Rows[x]["CV_No"].ToString(),
                            Convert.ToDateTime(ds.Tables[0].Rows[x]["CV_Date"].ToString()).ToShortDateString(),
                            ds.Tables[0].Rows[x]["Prepared_By"].ToString(),
                            ds.Tables[0].Rows[x]["Audited_By"].ToString());
                    }

                    lblTotalCount.Text = ds.Tables[0].Rows.Count.ToString();
                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }
            }
        }
        #endregion

        #region RECEIPTVoucherAudit
        //======================================================================
        //                      RECEIPT VOUCHER
        //======================================================================
        public void loadReceiptVoucherAudit(Label lblTotalCount, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                dgv.Rows.Clear();
                lblTotalCount.Text = "";
                
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM vw_CashReceipt_List_TH_Audit", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(ds.Tables[0].Rows[x]["Or_No"].ToString(),
                            "OR",
                            "OR#" + ds.Tables[0].Rows[x]["Or_No"].ToString(),
                            Convert.ToDateTime(ds.Tables[0].Rows[x]["Or_Date"].ToString()).ToShortDateString(),
                            ds.Tables[0].Rows[x]["Prepared_By"].ToString(),
                            ds.Tables[0].Rows[x]["Audited_By"].ToString());
                    }

                    lblTotalCount.Text = ds.Tables[0].Rows.Count.ToString();
                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }
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
                SqlDataAdapter adapterOR = new SqlDataAdapter("SELECT * FROM vw_CashReceipt_List_TH_Audit", con);
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
                SqlDataAdapter adapterJV = new SqlDataAdapter("SELECT * FROM vw_Journal_List_TH_Audit", con);
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
                SqlDataAdapter adapterCV = new SqlDataAdapter("SELECT * FROM vw_Disbursement_List_TH_Audit", con);
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
