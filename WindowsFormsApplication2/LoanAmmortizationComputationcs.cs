﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2
{
    public partial class LoanAmmortizationComputationcs : Form
    {
        public LoanAmmortizationComputationcs()
        {
            InitializeComponent();
        }
        CrystalDecisions.Shared.TableLogOnInfo li;

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        //=========================================
        Classes.clsLoan clsLoan = new Classes.clsLoan();
        Classes.clsParameter clsParameter = new Classes.clsParameter();
        Global global = new Global();

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        double less;
        double net20;
        public static int userID { get; set; }
        public static Boolean plarRenew { get; set; }
        public static Double plarExistingBalance { get; set; }

        double lessTotal, netAmount;
        private void button1_Click(object sender, EventArgs e)
        {
          
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //Moveable Forms / Screens
            //Nikko Melgar
            if (e.Button == MouseButtons.Left)
            {
                if (m_firstClick == false)
                {
                    m_firstClick = true;
                    m_firstClickLoc = new Point(e.X, e.Y);
                }

                this.Location = new Point(
                    this.Location.X + e.X - m_firstClickLoc.X,
                    this.Location.Y + e.Y - m_firstClickLoc.Y
                    );
            }
            else
            {
                m_firstClick = false;
            }
        }

        //GET THE INTEREST
        public int returnInterest()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT val FROM Parameter WHERE frm = 'Loan' and Description = 'Loan Interest Amort'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                try
                {
                    return Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString());
                }
                catch
                {
                    return 9;
                }
            }
        }

        private void LoanAmmortizationComputationcs_Load(object sender, EventArgs e)
        {
            clsLoan.loadComboBox(cmbLoanType);
            cmbLoanType.SelectedIndex = -1;
            clsLoan.loadComboBox(cmbLessType);
            cmbLessType.SelectedIndex = -1;
        }

        private void txtLoanAmount_Leave(object sender, EventArgs e)
        {
            if (txtLoanAmount.Text != "")
            {
                txtLoanAmount.Text = Convert.ToDecimal(txtLoanAmount.Text).ToString("#,0.00");
                txtGrossAmount.Text = Convert.ToDecimal(txtLoanAmount.Text).ToString("#,0.00");
            }
        }

        private void label21_Click(object sender, EventArgs e)
        {
           
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if(cmbLoanType.Text == "")
            {
                Alert.show("Please select Loan type.", Alert.AlertType.error);
                return;
            }

            if(txtLoanAmount.Text == "")
            {
                Alert.show("Please enter Loan amount.", Alert.AlertType.error);
                return;
            }

            if(interestRate.Text == "")
            {
                Alert.show("Please enter interest rate.", Alert.AlertType.error);
                return;
            }

            if(txtTerminMos.Text == "")
            {
                Alert.show("Please enter Loan term in months.", Alert.AlertType.error);
                return;
            }

            computeMonthly();
            //call the Report then display to the crystal report viewer
            loadReport();
        }

        //=======================================================================
        //                  computation of monthly payment
        //=======================================================================

        public void computeMonthly()
        {
            //( Pv  *  R ) / (1 - ( 1 + R )^(-n)

            double PV = Convert.ToDouble(txtLoanAmount.Text.Replace(",", ""));
            double rate = Convert.ToDouble(interestRate.Text) / 100;
            rate = rate / 12;
            double term = Convert.ToDouble(txtTerminMos.Text);
            double val1 = 1 + rate;
            double val2 = -term;
            double powResult = Math.Pow(val1, val2);
            double rightSide = 1 - powResult;
            double leftSide = PV * rate;
            double finalResult = leftSide / rightSide;

            double finalResult2 = (PV * rate) / (1 - (Math.Pow((1 + rate), -term)));
            decimal dec = Convert.ToDecimal(finalResult2);

            //FOR SUMMARY SECTION
            txtMonthlyPayment.Text = dec.ToString("#,0.00");
            double ttalPayment, ttalInterest;
            ttalPayment = finalResult2 * Convert.ToDouble(txtTerminMos.Text);
            ttalInterest = ttalPayment - PV;
            txtTotalPayment.Text = Convert.ToString(Convert.ToDecimal(ttalPayment).ToString("#,0.00"));
            txtInterest.Text = Convert.ToString(Convert.ToDecimal(ttalInterest).ToString("#,0.00"));


            //Insert into table for REPORT but first remove any data which already encoded 
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.CommandText = "DELETE Loan_Amortization WHERE Encoded_By = '" + Classes.clsUser.Username + "'";
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                int x = 0;
                double y, z, i, ob;

                //Now Insert the DATA to the TABLE
                for (int a = 0; a < Convert.ToInt32(txtTerminMos.Text); a++)
                {
                    z = PV * rate;
                    i = finalResult2 - z;
                    ob = PV - i;

                    //save code here
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_InsertLoanAmortization";
                    cmd.Parameters.AddWithValue("@loan_type", cmbLoanType.SelectedValue);
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@EmployeeID", txtEmployeeID.Text);
                    cmd.Parameters.AddWithValue("@Name", txtName.Text);
                    cmd.Parameters.AddWithValue("@payment", txtMonthlyPayment.Text.Replace(",", ""));
                    cmd.Parameters.AddWithValue("@interest", Convert.ToString(decimal.Round(Convert.ToDecimal(z), 2)));
                    cmd.Parameters.AddWithValue("@principal", Convert.ToString(decimal.Round(Convert.ToDecimal(i), 2)));
                    cmd.Parameters.AddWithValue("@balance", Convert.ToString(decimal.Round(Convert.ToDecimal(ob), 2)));
                    cmd.Parameters.AddWithValue("@encoded_by", Classes.clsUser.Username.ToString());
                    cmd.ExecuteNonQuery();

                    z = 0;
                    PV = ob;
                }

                //================================================================================
                //              IF THERES A  LESS FOR NET AMOUNT
                //================================================================================
                SqlCommand cmdLess = new SqlCommand();
                cmdLess.Connection = con;
                cmdLess.CommandText = "DELETE loan_amort_less WHERE encoded_by = '" + Classes.clsUser.Username + "'";
                cmdLess.CommandType = CommandType.Text;
                cmdLess.ExecuteNonQuery();

                if (dataGridView1.Rows.Count > 0)
                {
                    //=================================================================================
                    //              Insert to the Database if theres a value
                    //=================================================================================
                    //Now Insert the DATA to the TABLE
                    for (int b = 0; b < dataGridView1.Rows.Count; b++)
                    {
                        SqlCommand cmd2 = new SqlCommand();
                        cmd2.Connection = con;
                        cmd2.CommandText = "sp_insertLoanAmortLess";
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@Loan_Type", cmbLoanType.SelectedValue.ToString());
                        cmd2.Parameters.AddWithValue("@Description", dataGridView1.Rows[b].Cells[0].Value.ToString());
                        cmd2.Parameters.AddWithValue("@Amount", Convert.ToDecimal(dataGridView1.Rows[b].Cells[1].Value.ToString()));
                        cmd2.Parameters.AddWithValue("@encoded_by", Classes.clsUser.Username);
                        cmd2.ExecuteNonQuery();
                    }


                }

                //IF SC AND SD NOT NULL
                if(txtSC.Text != "")
                {
                    SqlCommand cmdSC = new SqlCommand();
                    cmdSC.Connection = con;
                    cmdSC.CommandText = "sp_insertLoanAmortLess";
                    cmdSC.CommandType = CommandType.StoredProcedure;
                    cmdSC.Parameters.AddWithValue("@Loan_Type", cmbLoanType.SelectedValue.ToString());
                    cmdSC.Parameters.AddWithValue("@Description", "Share Capital");
                    cmdSC.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtSC.Text));
                    cmdSC.Parameters.AddWithValue("@encoded_by", Classes.clsUser.Username);
                    cmdSC.ExecuteNonQuery();
                }

                if (txtSD.Text != "")
                {
                    SqlCommand cmdSD = new SqlCommand();
                    cmdSD.Connection = con;
                    cmdSD.CommandText = "sp_insertLoanAmortLess";
                    cmdSD.CommandType = CommandType.StoredProcedure;
                    cmdSD.Parameters.AddWithValue("@Loan_Type", cmbLoanType.SelectedValue.ToString());
                    cmdSD.Parameters.AddWithValue("@Description", "Savings");
                    cmdSD.Parameters.AddWithValue("@Amount", Convert.ToDecimal(txtSD.Text));
                    cmdSD.Parameters.AddWithValue("@encoded_by", Classes.clsUser.Username);
                    cmdSD.ExecuteNonQuery();
                }
            }

        }


        public void loadReport()
        {
            updateTableLess();

            CrystalDecisions.Shared.TableLogOnInfo li;
                       
            string str = "SELECT * FROM Loan_Amortization WHERE encoded_by = '"+ Classes.clsUser.Username.ToString() +"' order by balance desc";
            string strSubreport = "SELECT * FROM loan_amort_less WHERE Loan_Type = '"+ cmbLoanType.SelectedValue +"' and encoded_by ='"+ Classes.clsUser.Username +"'";
            //=======================================================
            //                  for sub report
            //=======================================================
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter subreportAdapter = new SqlDataAdapter(strSubreport, con);
                DataTable dtsubreport = new DataTable();
                DataSet dsSubreport = new DataSet();


                //=======================================================
                //              getting the less total
                //=======================================================

                SqlDataAdapter lessadapter = new SqlDataAdapter("SELECT sum(Amount) FROM loan_amort_less WHERE Loan_Type = '" + cmbLoanType.SelectedValue + "' and encoded_by ='" + Classes.clsUser.Username + "'", con);
                DataTable dtLess = new DataTable();
                lessadapter.Fill(dtLess);

                less = 0;
                if (dtLess.Rows[0].ItemArray[0].ToString() != DBNull.Value.ToString())
                {
                    less = Convert.ToDouble(dtLess.Rows[0].ItemArray[0].ToString());
                }


                SqlDataAdapter adapter1 = new SqlDataAdapter(str, con);
                DataTable dt1 = new DataTable();
                DataSet ds = new DataSet();

                LoanReports.loanAmortization cr = new LoanReports.loanAmortization();

                li = new TableLogOnInfo();

                li.ConnectionInfo.IntegratedSecurity = false;


                //=========================================
                //          sub report
                //=========================================
                subreportAdapter.Fill(ds, "loan_amort_less");
                dtsubreport = dsSubreport.Tables["loan_amort_less"];
                cr.Subreports[0].SetDataSource(ds.Tables["loan_amort_less"]);


                adapter1.Fill(ds, "Loan_Amortization");
                dt = ds.Tables["Loan_Amortization"];
                cr.SetDataSource(ds.Tables["Loan_Amortization"]);


                //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);


                //Signature
                cr.SetParameterValue("LoanAmount", txtLoanAmount.Text);
                cr.SetParameterValue("AnnualInterest", interestRate.Text);
                cr.SetParameterValue("TermInMons", txtTerminMos.Text);
                cr.SetParameterValue("MonthlyPayment", txtMonthlyPayment.Text);
                cr.SetParameterValue("NoOfPayments", txtTerminMos.Text);
                cr.SetParameterValue("TotalPayments", txtTotalPayment.Text);
                cr.SetParameterValue("TotalInterest", txtInterest.Text);
               
                //If Loan PLAR is RENEW then change the service fee amount

                if (plarRenew == true)
                {
                    lessTotal = Convert.ToDouble(txtLoanAmount.Text) - plarExistingBalance;
                    lessTotal = lessTotal * Convert.ToDouble(clsParameter.serviceFee());
                    //testing for plarbalance
                    MessageBox.Show(plarExistingBalance.ToString("#,0.00"));
                }
                else
                {
                    //For LOANS NOT REQUIRED SERVICE FEE
                    SqlDataAdapter adapterNoFee = new SqlDataAdapter("SELECT VAL FROM Parameter WHERE val = '" + cmbLoanType.SelectedValue.ToString() + "' and Description = 'No Service Fee'", con);
                    DataTable dtNoFee = new DataTable();
                    adapterNoFee.Fill(dtNoFee);

                    if(dtNoFee.Rows.Count > 0)
                    {
                        lessTotal = Convert.ToDouble(txtLoanAmount.Text) * 0.00;
                    }
                    else
                    {
                        lessTotal = Convert.ToDouble(txtLoanAmount.Text) * Convert.ToDouble(clsParameter.serviceFee());
                    }

                }


                netAmount = Convert.ToDouble(txtLoanAmount.Text) - lessTotal;
                netAmount = netAmount - less; //Less all the loans and other deductions

                cr.SetParameterValue("LessServiceFee", Convert.ToString(Convert.ToDecimal(lessTotal).ToString("#,0.00")));
                cr.SetParameterValue("TotalNet", Convert.ToString(Convert.ToDecimal(netAmount).ToString("#,0.00")));

                crystalReportViewer1.ReportSource = cr;
            }
        }

        public void updateTableLess()
        {
            //If Loan PLAR is RENEW then change the service fee amount

            if (plarRenew == true)
            {
                lessTotal = Convert.ToDouble(txtLoanAmount.Text) - plarExistingBalance;
                lessTotal = lessTotal * Convert.ToDouble(clsParameter.serviceFee());
            }
            else
            {
                lessTotal = Convert.ToDouble(txtLoanAmount.Text) * Convert.ToDouble(clsParameter.serviceFee());
            }


            netAmount = Convert.ToDouble(txtLoanAmount.Text) - lessTotal;
            if(txtSD.Text != "")
            {
                netAmount = netAmount - Convert.ToDouble(txtSD.Text);
            }

            if(txtSC.Text != "")
            {
                netAmount = netAmount - Convert.ToDouble(txtSC.Text);
            }

            netAmount = netAmount - totalLoanLessNotDeferred();
            //Get 20% of net first
            net20 = netAmount * clsLoan.returnNetForAmmortization();
            //net = This will be the Net 20%

            if (checkIfHasDeferredLoan() == true)
            {
                if (net20 < Convert.ToDouble(totalDeferredLoanAmount()))
                {
                    updateDeferredPercentageAndAmount(net20);//First Update Percentage before Setting up the applied amount
                }
            }
        }

        public double totalLoanLessNotDeferred()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Amount FROM loan_amort_less WHERE Loan_Type ='" + cmbLoanType.SelectedValue.ToString() + "' and encoded_by = '" + Classes.clsUser.Username + "' and Description not like '%Deferred%' and Description not in('Share Capital','Savings')", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT sum(Amount) FROM loan_amort_less WHERE Loan_Type ='" + cmbLoanType.SelectedValue.ToString() + "' and encoded_by = '" + Classes.clsUser.Username + "' and Description not like '%Deferred%' and Description not in('Share Capital','Savings')", con);
                    DataTable dt2 = new DataTable();
                    adapter2.Fill(dt2);

                    return Convert.ToDouble(dt2.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal totalDeferredLoanAmount()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT SUM(Amount) FROM loan_amort_less WHERE Loan_Type ='" + cmbLoanType.SelectedValue.ToString() + "' and encoded_by = '" + Classes.clsUser.Username +"' and Description like '%Deferred%'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return Convert.ToDecimal(decimal.Round(Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString()), 2));
            }
        }
        
        //Check if theres a deferred loan first
        public Boolean checkIfHasDeferredLoan()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM loan_amort_less WHERE Loan_Type = '" + cmbLoanType.SelectedValue.ToString() + "' and encoded_by = '" + Classes.clsUser.Username + "' and Description like '%Deferred%'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        //Update the percentage of the deferred loans
        public void updateDeferredPercentageAndAmount(double net)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                string amnt;
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Amount,Description FROM loan_amort_less WHERE Loan_Type = '" + cmbLoanType.SelectedValue.ToString() + "' and encoded_by = '" + Classes.clsUser.Username + "' and Description like '%Deferred%'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                for(int x = 0; x < dt.Rows.Count; x++)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "UPDATE loan_amort_less SET deferred_Percent ="+ Convert.ToDouble(dt.Rows[x].ItemArray[0].ToString()) / Convert.ToDouble(totalDeferredLoanAmount()) + " WHERE loan_Type = '"+ cmbLoanType.SelectedValue.ToString() +"' and encoded_by = '"+ Classes.clsUser.Username +"' and Description = '"+ dt.Rows[x].ItemArray[1].ToString() +"'";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT Amount,Description,deferred_Percent FROM loan_amort_less WHERE Loan_Type = '" + cmbLoanType.SelectedValue.ToString() + "' and encoded_by = '" + Classes.clsUser.Username + "' and Description like '%Deferred%'", con);
                DataTable dt2 = new DataTable();
                adapter2.Fill(dt2);

                for(int y = 0; y < dt2.Rows.Count; y++)
                {
                    SqlCommand cmd2 = new SqlCommand();
                    cmd2.Connection = con;
                    amnt = Convert.ToString(Convert.ToDecimal(net * Convert.ToDouble(dt2.Rows[y].ItemArray[2].ToString())));
                    cmd2.CommandText = "UPDATE loan_amort_less set Amount = " + Convert.ToDecimal(decimal.Round(Convert.ToDecimal(amnt), 2)) + " WHERE loan_type = '" + cmbLoanType.SelectedValue.ToString() + "' and encoded_by = '" + Classes.clsUser.Username + "' and Description = '" + dt2.Rows[y].ItemArray[1].ToString() + "'";
                    cmd2.CommandType = CommandType.Text;
                    cmd2.ExecuteNonQuery();
                }
               
            }
        }
        
        private void txtTerminMos_Leave(object sender, EventArgs e)
        {
            if (txtTerminMos.Text != "")
            {
                txtNoPayment.Text = txtTerminMos.Text;
            }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoanReports.searchMemberAmort frm = new LoanReports.searchMemberAmort();
            frm.ShowDialog();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(LoanAmmortizationComputationcs))
                {
                    form.Activate();
                    return;
                }
            }

            LoanAmmortizationComputationcs frm = new LoanAmmortizationComputationcs();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(cmbLessType.Text == "")
            {
                Alert.show("Please enter Description.", Alert.AlertType.error);
                return;
            }

            if(txtAmountLESS.Text == "")
            {
                Alert.show("Please enter Amount to be deducted.", Alert.AlertType.error);
                return;
            }
            txtAmountLESS.Text = Convert.ToDecimal(txtAmountLESS.Text).ToString("#,0.00");
            string[] row = { cmbLessType.SelectedValue.ToString(), txtAmountLESS.Text};
            dataGridView1.Rows.Add(row);

            txtAmountLESS.Text = "";
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow.IsNewRow)
                {
                    return;
                }

                if (dataGridView1.SelectedRows.Count >= 1)
                {
                    string msg = Environment.NewLine + "Are you sure you want to remove this?";
                    DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            if (row.Cells[0].Value != null)
                            {
                                dataGridView1.Rows.Remove(row);
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch
            {

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            txtEmployeeID.Text = "";
            txtName.Text = "";
            cmbLoanType.SelectedIndex = -1;
            txtLoanAmount.Text = "";
            txtGrossAmount.Text = "";
            txtTerminMos.Text = "";
            txtMonthlyPayment.Text = "";
            txtNoPayment.Text = "";
            txtTotalPayment.Text = "";
            txtInterest.Text = "";
            dataGridView1.Rows.Clear();
            txtSC.Text = "";
            txtSD.Text = "";
            cmbLessType.SelectedIndex = -1;
            txtDateHired.Text = "";
            txtNoOfServiceInYears.Text = "";
        }

        private void txtLoanAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void cmbLoanType_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbLoanType.Text != "")
            {
                dataGridView1.Rows.Clear();
                

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_ReturnLoanBalancesForPrevLoan";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", userID);
                    cmd.Parameters.AddWithValue("@Loan_Type", cmbLoanType.SelectedValue);
                    cmd.Parameters.AddWithValue("@Loan_No", ' ');

                    adapter = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adapter.Fill(dt);


                    //If Theres a data for prev loan
                    if (dt.Rows.Count > 0)
                    {
                        //SET plarBoolean = TRUE if PLAR
                        if (cmbLoanType.SelectedValue.ToString() == "PLAR")
                        {
                            plarRenew = true;
                            plarExistingBalance = Convert.ToDouble(dt.Rows[0].ItemArray[38].ToString());
                        }
                        else
                        {
                            plarRenew = false;
                            plarExistingBalance = 0;
                        }

                        //Put in LESS 
                        string[] row = { dt.Rows[0].ItemArray[1].ToString() + " - " + dt.Rows[0].ItemArray[0].ToString(), Convert.ToDecimal(dt.Rows[0].ItemArray[38].ToString()).ToString("#,0.00") };
                        dataGridView1.Rows.Add(row);

                    }
                    else
                    {
                        plarRenew = false;
                    }


                    //Load Loans Deferred 
                    SqlCommand cmdDeferred = new SqlCommand();
                    cmdDeferred.Connection = con;
                    cmdDeferred.CommandText = "sp_ReturnLoanBalancesDeferred";
                    cmdDeferred.CommandType = CommandType.StoredProcedure;
                    cmdDeferred.Parameters.AddWithValue("@userid", userID);

                    SqlDataAdapter adapterDef = new SqlDataAdapter(cmdDeferred);
                    DataTable dtDef = new DataTable();
                    adapterDef.Fill(dtDef);

                    if(dtDef.Rows.Count > 0)
                    {
                        //Has a deferred loan(s)
                        //Put the deferred loans in datagridview
                        for(int x = 0; x<dtDef.Rows.Count; x++)
                        {
                            if(dt.Rows.Count > 0)
                            {
                                if (dt.Rows[0].ItemArray[1].ToString() != dtDef.Rows[x].ItemArray[1].ToString())
                                {
                                    //Will not add if the loan is selected already for renewal
                                    string[] row = { dtDef.Rows[x].ItemArray[1].ToString() + " - Deferred", Convert.ToDecimal(dtDef.Rows[x].ItemArray[39].ToString()).ToString("#,0.00") };
                                    dataGridView1.Rows.Add(row);
                                }
                            }
                            else
                            {
                                string[] row = { dtDef.Rows[x].ItemArray[1].ToString() + " - Deferred", Convert.ToDecimal(dtDef.Rows[x].ItemArray[39].ToString()).ToString("#,0.00") };
                                dataGridView1.Rows.Add(row);
                            }
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if(txtSC.Text != "")
            {
                txtSC.Text = Convert.ToDecimal(txtSC.Text).ToString("#,0.00");
            }
        }

        private void txtSD_Leave(object sender, EventArgs e)
        {
            if (txtSD.Text != "")
            {
                txtSD.Text = Convert.ToDecimal(txtSD.Text).ToString("#,0.00");
            }
        }

        private void cmbLoanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbLoanType.Text != "")
            {
                interestRate.Text = returnInterest().ToString();
            }
            else
            {
                interestRate.Text = "";
            }
        }

        private void txtAmountLESS_Leave(object sender, EventArgs e)
        {
            if (txtAmountLESS.Text != "")
            {
                txtAmountLESS.Text = Convert.ToDecimal(txtAmountLESS.Text).ToString("#,0.00");
            }
        }
    }
}
