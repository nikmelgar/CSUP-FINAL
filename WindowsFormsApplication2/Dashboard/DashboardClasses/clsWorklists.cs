using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace WindowsFormsApplication2.Dashboard.DashboardClasses
{
    class clsWorklists
    {
        Global global = new Global();
        int totalCnt;
        string sql;
        string jv, cv, or;
        #region JournalVoucher
        //======================================================================
        //                      JOURNAL VOUCHER
        //======================================================================
        public void loadJournalVoucher(Label lblTotalCount,DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                dgv.Rows.Clear();
                lblTotalCount.Text = "";
                SqlDataAdapter adapter;

                if (Classes.clsUser.department.ToString() == "1" && Classes.clsUser.role.ToString() == "3")
                {
                    //Accounting
                    adapter = new SqlDataAdapter("SELECT * FROM vw_Journal_List_TH", con);
                }
                else
                {
                    adapter = new SqlDataAdapter("SELECT * FROM vw_Journal_List_TH_Audit", con);
                    dgv.ReadOnly = true;
                    
                }

                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                           dgv.Rows.Add(ds.Tables[0].Rows[x]["JV_No"].ToString(),
                               "JV",
                               false,//FOR CHECKBOX
                               "JV#"+ ds.Tables[0].Rows[x]["JV_No"].ToString(),
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

        #region DisbursementVoucher
        //======================================================================
        //                      DISBURSEMENT VOUCHER
        //======================================================================
        public void loadDisbursementVoucher(Label lblTotalCount, DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                dgv.Rows.Clear();
                lblTotalCount.Text = "";

                if (Classes.clsUser.department.ToString() == "1" && Classes.clsUser.role.ToString() == "3")
                {
                    //Accounting
                    sql = "SELECT * FROM vw_Disbursement_List_TH";
                }
                else
                {
                    sql = "SELECT * FROM vw_Disbursement_List_TH_Audit";
                    dgv.ReadOnly = true;
                }

                SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(ds.Tables[0].Rows[x]["CV_No"].ToString(),
                            "CV",
                            false, //FOR CHECKBOX
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

                if (Classes.clsUser.department.ToString() == "1" && Classes.clsUser.role.ToString() == "3")
                {
                    //Accounting
                    sql = "SELECT * FROM vw_CashReceipt_List_TH";
                }
                else
                {
                    sql = "SELECT * FROM vw_CashReceipt_List_TH_Audit";
                    dgv.ReadOnly = true;
                }

                SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(ds.Tables[0].Rows[x]["Or_No"].ToString(),
                            "OR",
                            false, //FOR CHECKBOX
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

                if (Classes.clsUser.department.ToString() == "1" && Classes.clsUser.role.ToString() == "3")
                {
                    //Accounting
                }
                else
                {
                    dgv.ReadOnly = true;
                }

                if (Classes.clsUser.department.ToString() == "1" && Classes.clsUser.role.ToString() == "3")
                {
                    //Accounting
                    or = "SELECT * FROM vw_CashReceipt_List_TH";
                }
                else
                {
                    or = "SELECT * FROM vw_CashReceipt_List_TH_Audit";
                    dgv.ReadOnly = true;
                }

                //============================================================================================
                //                      RECEIPT VOUCHER
                //============================================================================================
                SqlDataAdapter adapterOR = new SqlDataAdapter(or, con);
                DataSet dsOr = new DataSet();
                adapterOR.Fill(dsOr);

                if (dsOr.Tables[0].Rows.Count > 0)
                {
                    for (int x = 0; x < dsOr.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(dsOr.Tables[0].Rows[x]["Or_No"].ToString(),
                            "OR",
                            false, //FOR CHECKBOX
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


                if (Classes.clsUser.department.ToString() == "1" && Classes.clsUser.role.ToString() == "3")
                {
                    //Accounting
                    jv = "SELECT * FROM vw_Journal_List_TH";
                }
                else
                {
                    jv = "SELECT * FROM vw_Journal_List_TH_Audit";
                }
                //============================================================================================
                //                      JOURNAL VOUCHER
                //============================================================================================
                SqlDataAdapter adapterJV = new SqlDataAdapter(jv, con);
                DataSet dsJV = new DataSet();
                adapterJV.Fill(dsJV);

                if (dsJV.Tables[0].Rows.Count > 0)
                {
                    if(dgv.Rows.Count > 0)
                    {
                        //ADD NEW ROW AND REPAINT
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewCellStyle style = new DataGridViewCellStyle();
                        style.BackColor = System.Drawing.Color.SeaGreen; // the color change
                        row.DefaultCellStyle = style;
                        dgv.Rows.Add(row);

                        dgv.Rows[dgv.Rows.Count - 1].Cells[2] = new DataGridViewTextBoxCell();
                        dgv.Rows[dgv.Rows.Count - 1].Cells[2].Value = "";

                    }

                    for (int x = 0; x < dsJV.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows.Add(dsJV.Tables[0].Rows[x]["JV_No"].ToString(),
                            "JV",
                            false, //FOR CHECKBOX
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

                if (Classes.clsUser.department.ToString() == "1" && Classes.clsUser.role.ToString() == "3")
                {
                    //Accounting
                    cv = "SELECT * FROM vw_Disbursement_List_TH";
                }
                else
                {
                    cv = "SELECT * FROM vw_Disbursement_List_TH_Audit";
                }
                //============================================================================================
                //                      DISBURSEMENT VOUCHER
                //============================================================================================
                SqlDataAdapter adapterCV = new SqlDataAdapter(cv, con);
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
                            false, //FOR CHECKBOX
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
