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

namespace WindowsFormsApplication2.ReportsForms
{
    public partial class rptPLBI : Form
    {
        public rptPLBI()
        {
            InitializeComponent();
        }

        Global global = new Global();
        Classes.clsPLBI clsPLBI = new Classes.clsPLBI();
        SqlConnection con;
        
        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        double semiOrmonth = 0;
        double billing6Mons = 0;
        double billing2Mons = 0;
        double collection6Mons = 0;
        double collection2Mons = 0;
        double fstBckt = 0;
        double collectionPercent = 0;
        CrystalDecisions.Shared.TableLogOnInfo li;
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
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

        private void rptPLBI_Load(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(txtLoanNo.Text == "")
            {
                Alert.show("Loan No. is required.", Alert.AlertType.error);
                return;
            }

            //Check if the loan no is in database
            if(clsPLBI.checkLoanNo(txtLoanNo.Text) != true)
            {
                Alert.show("Loan No. is required.", Alert.AlertType.error);
                return;
            }

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                CrystalDecisions.Shared.TableLogOnInfo li;

                string str = "SELECT * FROM vw_CoMakers WHERE Loan_No = '" + txtLoanNo.Text + "'";
                //=======================================================
                //                  for sub report
                //=======================================================

                SqlDataAdapter subreportAdapter = new SqlDataAdapter(str, con);
                DataTable dtsubreport = new DataTable();
                DataSet dsSubreport = new DataSet();


                DataSet ds = new DataSet();
                //=============================================================
                ReportsForms.PLBI cr = new ReportsForms.PLBI();

                li = new TableLogOnInfo();

                li.ConnectionInfo.IntegratedSecurity = false;


                //=========================================
                //          sub report
                //=========================================
                subreportAdapter.Fill(dsSubreport, "vw_CoMakers");
                dtsubreport = dsSubreport.Tables["vw_CoMakers"];
                cr.Subreports[0].SetDataSource(dsSubreport.Tables["vw_CoMakers"]);

                //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);


                //============================================================
                //Generate BIlling and Collection for 6 and 2 mons
                //============================================================

                SqlCommand cmdGetLoans = new SqlCommand();
                cmdGetLoans.Connection = con;
                cmdGetLoans.CommandText = "sp_ReturnLoanBalancesREPORT";
                cmdGetLoans.CommandType = CommandType.StoredProcedure;
                cmdGetLoans.Parameters.AddWithValue("@userid", clsPLBI.userid(txtLoanNo.Text));

                SqlDataAdapter adapter = new SqlDataAdapter(cmdGetLoans);
                DataSet dsList = new DataSet();
                adapter.Fill(dsList);

                //Variables
                billing6Mons = 0;
                billing2Mons = 0;
                collection2Mons = 0;
                collection6Mons = 0;

                //====================================
                //      FOR 6 MONTHS FIRST
                //====================================
                for (int x = 0; x < dsList.Tables[0].Rows.Count; x++)
                {
                    semiOrmonth = 0;
                    fstBckt = 0;

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_ReturnListDatesForBillingPLBI";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", clsPLBI.userid(txtLoanNo.Text));
                    cmd.Parameters.AddWithValue("@loan_No", dsList.Tables[0].Rows[x]["Loan_No"].ToString());
                    cmd.Parameters.AddWithValue("@ReleasedDate", dsList.Tables[0].Rows[x]["ReleaseDate"].ToString());

                    SqlDataAdapter adapterList = new SqlDataAdapter(cmd);
                    DataTable dtCnt = new DataTable();
                    adapterList.Fill(dtCnt);

                    if (dtCnt.Rows.Count > 0)
                    {
                        if (checkIfNonPayroll() == true)
                        {
                            //FOR NON PAYROLL COMPUTATION
                            semiOrmonth = Convert.ToDouble(dsList.Tables[0].Rows[x]["Monthly_Amort"].ToString());
                        }
                        else
                        {
                            //PAYROLL COMPUTATION
                            semiOrmonth = Convert.ToDouble(dsList.Tables[0].Rows[x]["Semi_Monthly_Amort"].ToString());
                        }

                        //Compute
                        fstBckt = Convert.ToDouble(dtCnt.Rows.Count) * semiOrmonth;

                        billing6Mons += fstBckt;
                    }
                }

                //====================================
                //      FOR 2 MONTHS FIRST
                //====================================
                for (int x = 0; x < dsList.Tables[0].Rows.Count; x++)
                {
                    semiOrmonth = 0;
                    fstBckt = 0;

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_ReturnListDatesForBillingPLBI2months";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", clsPLBI.userid(txtLoanNo.Text));
                    cmd.Parameters.AddWithValue("@loan_No", dsList.Tables[0].Rows[x]["Loan_No"].ToString());
                    cmd.Parameters.AddWithValue("@ReleasedDate", dsList.Tables[0].Rows[x]["ReleaseDate"].ToString());

                    SqlDataAdapter adapterList = new SqlDataAdapter(cmd);
                    DataTable dtCnt = new DataTable();
                    adapterList.Fill(dtCnt);

                    if (dtCnt.Rows.Count > 0)
                    {
                        if (checkIfNonPayroll() == true)
                        {
                            //FOR NON PAYROLL COMPUTATION
                            semiOrmonth = Convert.ToDouble(dsList.Tables[0].Rows[x]["Monthly_Amort"].ToString());
                        }
                        else
                        {
                            //PAYROLL COMPUTATION
                            semiOrmonth = Convert.ToDouble(dsList.Tables[0].Rows[x]["Semi_Monthly_Amort"].ToString());
                        }

                        //Compute
                        fstBckt = Convert.ToDouble(dtCnt.Rows.Count) * semiOrmonth;

                        billing2Mons += fstBckt;
                    }
                }


                //====================================
                //     FOR 6 MONTHS FIRST COLELCTION
                //====================================
                for (int x = 0; x < dsList.Tables[0].Rows.Count; x++)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_ReturnListDatesForCollectionPLBI6months";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", clsPLBI.userid(txtLoanNo.Text));
                    cmd.Parameters.AddWithValue("@loan_No", dsList.Tables[0].Rows[x]["Loan_No"].ToString());
                    cmd.Parameters.AddWithValue("@ReleasedDate", dsList.Tables[0].Rows[x]["ReleaseDate"].ToString());
                    cmd.Parameters.AddWithValue("@account_Dr", dsList.Tables[0].Rows[x]["CurrentDr"].ToString());
                    cmd.Parameters.AddWithValue("@pastDueCr", dsList.Tables[0].Rows[x]["PastDueDr"].ToString());

                    SqlDataAdapter adapterList = new SqlDataAdapter(cmd);
                    DataTable dtCnt = new DataTable();
                    adapterList.Fill(dtCnt);

                    collection6Mons += Convert.ToDouble(dtCnt.Rows[0].ItemArray[0].ToString());
                }

                //====================================
                //     FOR 2 MONTHS FIRST COLELCTION
                //====================================
                for (int x = 0; x < dsList.Tables[0].Rows.Count; x++)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_ReturnListDatesForCollectionPLBI2months";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", clsPLBI.userid(txtLoanNo.Text));
                    cmd.Parameters.AddWithValue("@loan_No", dsList.Tables[0].Rows[x]["Loan_No"].ToString());
                    cmd.Parameters.AddWithValue("@ReleasedDate", dsList.Tables[0].Rows[x]["ReleaseDate"].ToString());
                    cmd.Parameters.AddWithValue("@account_Dr", dsList.Tables[0].Rows[x]["CurrentDr"].ToString());
                    cmd.Parameters.AddWithValue("@pastDueCr", dsList.Tables[0].Rows[x]["PastDueDr"].ToString());

                    SqlDataAdapter adapterList = new SqlDataAdapter(cmd);
                    DataTable dtCnt = new DataTable();
                    adapterList.Fill(dtCnt);

                    collection2Mons += Convert.ToDouble(dtCnt.Rows[0].ItemArray[0].ToString());
                }

                //====================================================
                //          END GETTING COLLECTION AND BILLING
                //====================================================

                //PARAMETERS
                cr.SetParameterValue("employeeID", clsPLBI.empid(txtLoanNo.Text));
                cr.SetParameterValue("name", clsPLBI.namewithcompany(txtLoanNo.Text));
                cr.SetParameterValue("loanNoType", clsPLBI.loanNoType(txtLoanNo.Text));
                cr.SetParameterValue("membershipDate", clsPLBI.memDate(txtLoanNo.Text));
                cr.SetParameterValue("pmsDate", clsPLBI.pmsDate(txtLoanNo.Text));
                cr.SetParameterValue("sc", clsPLBI.shareCapital(txtLoanNo.Text));
                cr.SetParameterValue("sd", clsPLBI.savings(txtLoanNo.Text));
                cr.SetParameterValue("sc_perday", clsPLBI.scPerDay(txtLoanNo.Text));
                cr.SetParameterValue("sd_perday", clsPLBI.SavingsPerday(txtLoanNo.Text));
                cr.SetParameterValue("BANK", clsPLBI.Bank(txtLoanNo.Text));
                cr.SetParameterValue("bank_accnt", clsPLBI.Atm(txtLoanNo.Text));
                cr.SetParameterValue("BillingFor6Mons", Convert.ToDecimal(Convert.ToString(decimal.Round(Convert.ToDecimal(billing6Mons), 2))).ToString("#,0.00"));
                cr.SetParameterValue("BillingFor2Mons", Convert.ToDecimal(Convert.ToString(decimal.Round(Convert.ToDecimal(billing2Mons), 2))).ToString("#,0.00"));
                cr.SetParameterValue("CollectionFor6Mons", Convert.ToDecimal(Convert.ToString(decimal.Round(Convert.ToDecimal(collection6Mons), 2))).ToString("#,0.00"));
                cr.SetParameterValue("collectionlast2", Convert.ToDecimal(Convert.ToString(decimal.Round(Convert.ToDecimal(collection2Mons), 2))).ToString("#,0.00"));

                //SETTING THE RATE AND COLLECTION PERCENTAGE
                collectionPercent = 0;

                if(collection6Mons == 0 && billing6Mons == 0)
                {
                    collection6Mons = 1;
                    billing6Mons = 1;
                }

                collectionPercent = collection6Mons / billing6Mons;
                collectionPercent = collectionPercent * 100;

                if(Convert.ToDecimal(Convert.ToString(decimal.Round(Convert.ToDecimal(collectionPercent), 2))) > 90)
                {
                    cr.SetParameterValue("rate", "A");
                    cr.SetParameterValue("collection%", Convert.ToDecimal(Convert.ToString(decimal.Round(Convert.ToDecimal(collectionPercent), 2))));
                }
                else
                {
                    cr.SetParameterValue("rate", "B");
                    cr.SetParameterValue("collection%", Convert.ToDecimal(Convert.ToString(decimal.Round(Convert.ToDecimal(collectionPercent), 2))));
                }
                
                cr.SetParameterValue("printedBy", Classes.clsUser.Username.ToString());
                cr.SetParameterValue("sc_date", "10/15/2017");
                cr.SetParameterValue("sd_date", "10/15/2017");
                cr.SetParameterValue("@userid", clsPLBI.userid(txtLoanNo.Text));
                cr.SetParameterValue("dateHired", clsPLBI.dateHired(txtLoanNo.Text));
                cr.SetParameterValue("Salary", clsPLBI.salary(txtLoanNo.Text));
                cr.SetParameterValue("yearsInService", clsPLBI.noOfyearsService(txtLoanNo.Text));

                crystalReportViewer1.ReportSource = cr;
            }
        }

        public bool checkIfNonPayroll()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Company_Code FROM Membership WHERE userID = '"+ clsPLBI.userid(txtLoanNo.Text) +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows[0].ItemArray[0].ToString() == "COMP010")
                {
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
