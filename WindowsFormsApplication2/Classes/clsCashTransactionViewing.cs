using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;



namespace WindowsFormsApplication2.Classes
{
    class clsCashTransactionViewing
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();

        string str,strAccountCode,strCancelled;


      
        public void loadSearchCollection(DataGridView dgv,DataGridView dgvAccnt,DataGridView dgvCancelled,Label totalNo,Label totalAccountNo,Label lblTotalCancel,TextBox orNumber,CheckBox chckMember, CheckBox chckNonMember, CheckBox chckPerea, CheckBox chckTeltech, DateTimePicker dtFrom, DateTimePicker dtTo,TextBox keyword, ComboBox searchBy)
        {
            dgv.Rows.Clear();

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                str = ""; //Default value

                /*
                By Name
                Prepared By
                Posted By
                */

                if (searchBy.Text != "")
                {
                    if(searchBy.Text == "By Name")
                    {
                        str = "SELECT * FROM vw_Cash_Receipts_Report WHERE Name like  '%" + keyword.Text + "%' and Status <> 'CANCELLED' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "'";
                        strCancelled = "SELECT * FROM vw_Cash_Receipts_Report WHERE Name like '%" + keyword.Text + "%' and Status = 'CANCELLED' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "'";
                        strAccountCode = "select account_code,account_Description,sum(COALESCE(Amount,0) + COALESCE(Cash,0)) as TotalAmount from vw_Cash_Receipts_Report WHERE Status <> 'CANCELLED' and Name like '%" + keyword.Text + "%' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "' group by Account_Code,account_Description order by account_description";
                    }
                    else if (searchBy.Text == "Prepared By")
                    {
                        str = "SELECT * FROM vw_Cash_Receipts_Report WHERE Prepared_By like  '%" + keyword.Text + "%' and Status <> 'CANCELLED' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "'";
                        strCancelled = "SELECT * FROM vw_Cash_Receipts_Report WHERE Prepared_By like '%" + keyword.Text + "%' and Status = 'CANCELLED' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "'";
                        strAccountCode = "select account_code,account_Description,sum(COALESCE(Amount,0) + COALESCE(Cash,0)) as TotalAmount from vw_Cash_Receipts_Report WHERE Status <> 'CANCELLED' and Prepared_By like '%" + keyword.Text + "%' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "' group by Account_Code,account_Description order by account_description";
                    }
                    else
                    {
                        str = "SELECT * FROM vw_Cash_Receipts_Report WHERE Posted_By like  '%" + keyword.Text + "%' and Status <> 'CANCELLED' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "'";
                        strCancelled = "SELECT * FROM vw_Cash_Receipts_Report WHERE Posted_By like '%" + keyword.Text + "%' and Status = 'CANCELLED' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "'";
                        strAccountCode = "select account_code,account_Description,sum(COALESCE(Amount,0) + COALESCE(Cash,0)) as TotalAmount from vw_Cash_Receipts_Report WHERE Status <> 'CANCELLED' and Posted_By like '%" + keyword.Text + "%' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "' group by Account_Code,account_Description order by account_description";
                    }
                }
                else
                {
                    if (orNumber.Text != "")
                    {
                        str = "SELECT * FROM vw_Cash_Receipts_Report WHERE Or_No ='" + orNumber.Text + "' and Status <> 'CANCELLED'";
                        strCancelled = "SELECT * FROM vw_Cash_Receipts_Report WHERE Or_No ='" + orNumber.Text + "' and Status = 'CANCELLED'";
                        strAccountCode = "select account_code,account_Description,sum(COALESCE(Amount,0) + COALESCE(Cash,0)) as TotalAmount from vw_Cash_Receipts_Report WHERE or_no = '" + orNumber.Text + "' group by Account_Code,account_Description order by account_description";
                    }
                    else
                    {
                        //FOR OTHER CRITERIA [MEMBER - NON MEMBER]
                        if (chckMember.Checked == true && chckNonMember.Checked == false)
                        {
                            //FOR MEMBERS ONLY
                            str = "SELECT * FROM vw_Cash_Receipts_Report WHERE Status <> 'CANCELLED' and Payor_Type = 'Member' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "'";
                            strCancelled = "SELECT * FROM vw_Cash_Receipts_Report WHERE Status = 'CANCELLED' and Payor_Type = 'Member' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "'";
                            strAccountCode = "select account_code,account_Description,sum(COALESCE(Amount, 0) + COALESCE(Cash, 0)) as TotalAmount from vw_Cash_Receipts_Report WHERE Or_Date between '" + dtFrom.Text + "' and '" + dtTo.Text + "' and Payor_Type ='Member' ";
                        }
                        else if (chckMember.Checked == false && chckNonMember.Checked == true)
                        {
                            //FOR NON MEMBERS ONLY
                            str = "SELECT * FROM vw_Cash_Receipts_Report WHERE Status <> 'CANCELLED' and Payor_Type = 'Client' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "'";
                            strCancelled = "SELECT * FROM vw_Cash_Receipts_Report WHERE Status = 'CANCELLED' and Payor_Type = 'Client' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "'";
                            strAccountCode = "select account_code,account_Description,sum(COALESCE(Amount, 0) + COALESCE(Cash, 0)) as TotalAmount from vw_Cash_Receipts_Report WHERE Or_Date between '" + dtFrom.Text + "' and '" + dtTo.Text + "' and Payor_Type ='Client' ";
                        }
                        else
                        {
                            //FOR BOTH CHECKBOX IF TRUE OR FALSE SAME 
                            str = "SELECT * FROM vw_Cash_Receipts_Report WHERE Status <> 'CANCELLED' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "'";
                            strCancelled = "SELECT * FROM vw_Cash_Receipts_Report WHERE Status = 'CANCELLED' and Or_Date BETWEEN '" + dtFrom.Text + "' and '" + dtTo.Text + "'";
                            strAccountCode = strAccountCode = "select account_code,account_Description,sum(COALESCE(Amount, 0) + COALESCE(Cash, 0)) as TotalAmount from vw_Cash_Receipts_Report WHERE Or_Date between '" + dtFrom.Text + "' and '" + dtTo.Text + "' ";
                        }


                        //FOR OTHER CRITERIA [PEREA - TELTECH] LOCATION
                        if (chckPerea.Checked == true && chckTeltech.Checked == false)
                        {
                            //FOR PEREA LOCATION
                            str = str + " and Location = 'PEREA'";
                            strCancelled = strCancelled + " and Location = 'PEREA'";
                            strAccountCode = strAccountCode + " and Location = 'PEREA' group by Account_Code,account_Description order by account_description";
                        }
                        else if (chckPerea.Checked == false && chckTeltech.Checked == true)
                        {
                            //FOR TELTECH LOCATION
                            str = str + " and Location = 'TELTECH'";
                            strCancelled = strCancelled + " and Location = 'TELTECH'";
                            strAccountCode = strAccountCode + " and Location = 'TELTECH' group by Account_Code,account_Description order by account_description";
                        }
                        else
                        {
                            //FOR BOTH CHECKBOX IF TRUE OR FALSE
                            strAccountCode = strAccountCode + " group by Account_Code,account_Description order by account_description";
                        }


                    }
                }
                
                adapter = new SqlDataAdapter(str, con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);



                //===================================
                //      For Cancel Records
                //===================================

                //Display Cancelled Transaction
                SqlCommand cmdCancelled = new SqlCommand();
                cmdCancelled.Connection = con;
                cmdCancelled.CommandText = strCancelled;
                cmdCancelled.CommandType = CommandType.Text;

                SqlDataAdapter adapterCancelled = new SqlDataAdapter(cmdCancelled);
                DataTable dtCancelled = new DataTable();
                adapterCancelled.Fill(dtCancelled);

                if (dt.Rows.Count == 0)
                {
                    if (dtCancelled.Rows.Count == 0)
                    {
                        Alert.show("No record(s) found.", Alert.AlertType.error);
                        dgvAccnt.DataSource = "";
                        dgvCancelled.DataSource = "";
                        lblTotalCancel.Text = "0";
                        totalNo.Text = "0";
                        totalAccountNo.Text = "0";
                        return;
                    }
                }
                else
                {
                    dgv.Rows.Add(dt.Rows.Count);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dgv.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString(); //OR NO
                        dgv.Rows[i].Cells[1].Value = Convert.ToDateTime(dt.Rows[i].ItemArray[1].ToString()).ToShortDateString(); //OR DATE
                        dgv.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString(); //Payor
                        dgv.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString(); //Name
                        dgv.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString(); //Cheque Details
                        dgv.Rows[i].Cells[5].Value = Convert.ToDecimal(dt.Rows[i].ItemArray[6].ToString()).ToString("#,0.00"); //Amount
                        dgv.Rows[i].Cells[6].Value = Convert.ToDecimal(dt.Rows[i].ItemArray[7].ToString()).ToString("#,0.00"); //Cash
                        dgv.Rows[i].Cells[7].Value = Convert.ToDecimal(dt.Rows[i].ItemArray[8].ToString()).ToString("#,0.00"); //Total AMount
                        dgv.Rows[i].Cells[8].Value = Convert.ToDecimal(dt.Rows[i].ItemArray[9].ToString()).ToString("#,0.00"); //FD
                        dgv.Rows[i].Cells[9].Value = Convert.ToDecimal(dt.Rows[i].ItemArray[10].ToString()).ToString("#,0.00"); //SD
                        dgv.Rows[i].Cells[10].Value = Convert.ToDecimal(dt.Rows[i].ItemArray[11].ToString()).ToString("#,0.00"); //Others
                        dgv.Rows[i].Cells[11].Value = dt.Rows[i].ItemArray[12].ToString(); //Description
                        dgv.Rows[i].Cells[12].Value = dt.Rows[i].ItemArray[19].ToString(); //PreparedBY
                        dgv.Rows[i].Cells[13].Value = dt.Rows[i].ItemArray[20].ToString(); //PostedBy
                    }


                    //Label for total count
                    totalNo.Text = dt.Rows.Count.ToString();

                    //ADD TOTAL IN FOOTER ROW
                    loadTotals(dgv);

                    //Display Account Description
                    SqlCommand cmdAccount = new SqlCommand();
                    cmdAccount.Connection = con;
                    cmdAccount.CommandText = strAccountCode;
                    cmdAccount.CommandType = CommandType.Text;

                    SqlDataAdapter adapterAccount = new SqlDataAdapter(cmdAccount);
                    DataTable dtAccount = new DataTable();
                    adapterAccount.Fill(dtAccount);
                    
                        dgvAccnt.DataSource = dtAccount;

                        dgvAccnt.Columns["account_code"].Visible = true;
                        dgvAccnt.Columns["account_code"].HeaderText = "Account Code";
                        dgvAccnt.Columns["account_code"].FillWeight = 50;

                        dgvAccnt.Columns["Account_Description"].Visible = true;
                        dgvAccnt.Columns["Account_Description"].HeaderText = "Account Description";
                        dgvAccnt.Columns["Account_Description"].FillWeight = 150;

                        dgvAccnt.Columns["TotalAmount"].Visible = true;
                        dgvAccnt.Columns["TotalAmount"].HeaderText = "Total Amount";
                        dgvAccnt.Columns["TotalAmount"].FillWeight = 150;
                        dgvAccnt.Columns["TotalAmount"].DefaultCellStyle.Format = "#,0.00";

                        //TOTAL AMOUNT
                        decimal sumTotal = 0;

                        if (dgvAccnt.Rows.Count > 0)
                        {

                            for (int i = 0; i < dgvAccnt.Rows.Count; ++i)
                            {
                                sumTotal += Convert.ToDecimal(dgvAccnt.Rows[i].Cells["TotalAmount"].Value);
                            }
                        }

                        totalAccountNo.Text = sumTotal.ToString("#,0.00");
                    

                   
                }

                dgvCancelled.DataSource = dtCancelled;
                lblTotalCancel.Text = dtCancelled.Rows.Count.ToString();

                int colCnt1 = dtCancelled.Columns.Count;
                int x1 = 0;


                while (x1 != colCnt1)
                {
                    dgvCancelled.Columns[x1].Visible = false;
                    x1 = x1 + 1;
                }

                dgvCancelled.Columns["Or_No"].Visible = true;
                dgvCancelled.Columns["Or_No"].HeaderText = "Or_No";
                dgvCancelled.Columns["Or_No"].FillWeight = 50;

                dgvCancelled.Columns["Or_Date"].Visible = true;
                dgvCancelled.Columns["Or_Date"].HeaderText = "Or_Date";
                dgvCancelled.Columns["Or_Date"].FillWeight = 150;

                dgvCancelled.Columns["Payor"].Visible = true;
                dgvCancelled.Columns["Payor"].HeaderText = "Payor";
                dgvCancelled.Columns["Payor"].FillWeight = 50;

                dgvCancelled.Columns["Name"].Visible = true;
                dgvCancelled.Columns["Name"].HeaderText = "Name";
                dgvCancelled.Columns["Name"].FillWeight = 140;

                dgvCancelled.Columns["Check"].Visible = true;
                dgvCancelled.Columns["Check"].HeaderText = "Check";
                dgvCancelled.Columns["Check"].FillWeight = 70;

                dgvCancelled.Columns["Amount"].Visible = true;
                dgvCancelled.Columns["Amount"].HeaderText = "Amount";
                dgvCancelled.Columns["Amount"].FillWeight = 60;

                dgvCancelled.Columns["Cash"].Visible = true;
                dgvCancelled.Columns["Cash"].HeaderText = "Cash";
                dgvCancelled.Columns["Cash"].FillWeight = 60;

                dgvCancelled.Columns["TotalAmount"].Visible = true;
                dgvCancelled.Columns["TotalAmount"].HeaderText = "Total Amount";
                dgvCancelled.Columns["TotalAmount"].FillWeight = 60;
                dgvCancelled.Columns["TotalAmount"].DefaultCellStyle.Format = "#,0.00";

                dgvCancelled.Columns["FD"].Visible = true;
                dgvCancelled.Columns["FD"].HeaderText = "FD";
                dgvCancelled.Columns["FD"].FillWeight = 60;

                dgvCancelled.Columns["SD"].Visible = true;
                dgvCancelled.Columns["SD"].HeaderText = "SD";
                dgvCancelled.Columns["SD"].FillWeight = 60;

                dgvCancelled.Columns["Others"].Visible = true;
                dgvCancelled.Columns["Others"].HeaderText = "Others";
                dgvCancelled.Columns["Others"].FillWeight = 60;
                dgvCancelled.Columns["Others"].DefaultCellStyle.Format = "#,0.00";

                dgvCancelled.Columns["Description"].Visible = true;
                dgvCancelled.Columns["Description"].HeaderText = "Description";
                dgvCancelled.Columns["Description"].FillWeight = 60;
            }

        }


        public void loadTotals(DataGridView dgv)
        {
            dgv.Rows.Add(1);

            decimal sumCheck = 0;
            decimal sumCash = 0;
            decimal sumTotal = 0;
            decimal sumShareCapital = 0;
            decimal sumSavings = 0;
            decimal sumOthers = 0;
            //Check if theres a beneficiary
            if (dgv.Rows.Count > 0)
            {

                for (int i = 0; i < dgv.Rows.Count; ++i)
                {
                    sumCheck += Convert.ToDecimal(dgv.Rows[i].Cells["Amount"].Value);
                    sumCash += Convert.ToDecimal(dgv.Rows[i].Cells["Cash"].Value);
                    sumShareCapital += Convert.ToDecimal(dgv.Rows[i].Cells["FD"].Value);
                    sumSavings += Convert.ToDecimal(dgv.Rows[i].Cells["SD"].Value);
                    sumOthers += Convert.ToDecimal(dgv.Rows[i].Cells["Others"].Value);
                    sumTotal += Convert.ToDecimal(dgv.Rows[i].Cells["TotalAmount"].Value);
                }
            }

            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[3].Value = "Totals";
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Amount"].Value = sumCheck.ToString("#,0.00");
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Cash"].Value = sumCash.ToString("#,0.00");
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["TotalAmount"].Value = sumTotal.ToString("#,0.00");
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["FD"].Value = sumShareCapital.ToString("#,0.00");
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["SD"].Value = sumSavings.ToString("#,0.00");
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Others"].Value = sumOthers.ToString("#,0.00");

            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[3].Style.Font = new System.Drawing.Font("Tahoma", 9,System.Drawing.FontStyle.Bold);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Amount"].Style.Font = new System.Drawing.Font("Tahoma", 9, System.Drawing.FontStyle.Bold);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Cash"].Style.Font = new System.Drawing.Font("Tahoma", 9, System.Drawing.FontStyle.Bold);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["TotalAmount"].Style.Font = new System.Drawing.Font("Tahoma", 9, System.Drawing.FontStyle.Bold);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["FD"].Style.Font = new System.Drawing.Font("Tahoma", 9, System.Drawing.FontStyle.Bold);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["SD"].Style.Font = new System.Drawing.Font("Tahoma", 9, System.Drawing.FontStyle.Bold);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Others"].Style.Font = new System.Drawing.Font("Tahoma", 9, System.Drawing.FontStyle.Bold);

        }
    }
}
