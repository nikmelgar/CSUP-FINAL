using System;
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
        public static int userID { get; set; }
        public static Boolean plarRenew { get; set; }
        public static Double plarExistingBalance { get; set; }
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

        private void LoanAmmortizationComputationcs_Load(object sender, EventArgs e)
        {
            clsLoan.loadComboBox(cmbLoanType);
            cmbLoanType.SelectedIndex = -1;
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
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if(cmbLoanType.Text == "")
            {
                Alert.show("Please select loan first!", Alert.AlertType.error);
                return;
            }

            if(txtLoanAmount.Text == "")
            {
                Alert.show("Please put loan amount!", Alert.AlertType.error);
                return;
            }

            if(interestRate.Text == "")
            {
                Alert.show("Please put interest rate!", Alert.AlertType.error);
                return;
            }

            if(txtTerminMos.Text == "")
            {
                Alert.show("Please fill up Terms in Months", Alert.AlertType.error);
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
            con = new SqlConnection();
            global.connection(con);

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
                cmd.Parameters.AddWithValue("@payment", txtMonthlyPayment.Text.Replace(",",""));
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
                    cmd2.Parameters.AddWithValue("@Loan_Type", cmbLoanType.SelectedValue);
                    cmd2.Parameters.AddWithValue("@Description", dataGridView1.Rows[b].Cells[0].Value.ToString());
                    cmd2.Parameters.AddWithValue("@Amount", Convert.ToDecimal(dataGridView1.Rows[b].Cells[1].Value.ToString()));
                    cmd2.Parameters.AddWithValue("@encoded_by", Classes.clsUser.Username);
                    cmd2.ExecuteNonQuery();
                }


            }






        }


        public void loadReport()
        {
            CrystalDecisions.Shared.TableLogOnInfo li;
                       
            string str = "SELECT * FROM Loan_Amortization WHERE encoded_by = '"+ Classes.clsUser.Username.ToString() +"' order by balance desc";
            string strSubreport = "SELECT * FROM loan_amort_less WHERE Loan_Type = '"+ cmbLoanType.SelectedValue +"' and encoded_by ='"+ Classes.clsUser.Username +"'";
            //=======================================================
            //                  for sub report
            //=======================================================

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
            if(dtLess.Rows[0].ItemArray[0].ToString() != DBNull.Value.ToString())
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
            double lessTotal,netAmount;


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
            netAmount = netAmount - less;

            cr.SetParameterValue("LessServiceFee", Convert.ToString(Convert.ToDecimal(lessTotal).ToString("#,0.00")));
            cr.SetParameterValue("TotalNet", Convert.ToString(Convert.ToDecimal(netAmount).ToString("#,0.00")));

            crystalReportViewer1.ReportSource = cr;
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
            if(txtAmountLESS.Text == "")
            {
                Alert.show("Please put amount first!", Alert.AlertType.error);
                return;
            }
            txtAmountLESS.Text = Convert.ToDecimal(txtAmountLESS.Text).ToString("#,0.00");
            string[] row = { txtDescriptionLESS.Text, txtAmountLESS.Text};
            dataGridView1.Rows.Add(row);

            txtAmountLESS.Text = "";
            txtDescriptionLESS.Text = "";
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
            interestRate.Text = "";
            txtTerminMos.Text = "";
            txtMonthlyPayment.Text = "";
            txtNoPayment.Text = "";
            txtTotalPayment.Text = "";
            txtInterest.Text = "";
            dataGridView1.Rows.Clear();
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

                con = new SqlConnection();
                global.connection(con);

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
                if(dt.Rows.Count > 0)
                {
                    //SET plarBoolean = TRUE if PLAR
                    if (cmbLoanType.SelectedValue.ToString() == "PLAR")
                    {
                        plarRenew = true;
                        plarExistingBalance = Convert.ToDouble(dt.Rows[0].ItemArray[37].ToString());
                    }
                    else
                    {
                        plarRenew = false;
                        plarExistingBalance = 0;
                    }

                    //Put in LESS 
                    string[] row = { dt.Rows[0].ItemArray[1].ToString() + " - " + dt.Rows[0].ItemArray[0].ToString(), Convert.ToDecimal(dt.Rows[0].ItemArray[37].ToString()).ToString("#,0.00") };
                    dataGridView1.Rows.Add(row);

                }
                else
                {
                    plarRenew = false;
                }
            }
        }
    }
}
