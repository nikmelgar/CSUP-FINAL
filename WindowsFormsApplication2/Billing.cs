using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Globalization;

namespace WindowsFormsApplication2
{
    public partial class Billing : Form
    {
        public Billing()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        string ret;
        //==================================================
        //            GLOBAL CLASSES AND FORMS
        //==================================================

        Global global = new Global();
        Classes.clsBilling clsBilling = new Classes.clsBilling();
        clsMembershipEntry clsMembershipEntry = new clsMembershipEntry();
        Classes.clsParameter clsParameter = new Classes.clsParameter();

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panelHeader_MouseMove(object sender, MouseEventArgs e)
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

        private void Billing_Load(object sender, EventArgs e)
        {
            clsMembershipEntry.loadComboBox(cmbCompany, "Company", "Description", "Company_Code");
            clsMembershipEntry.loadComboBox(cmbRank, "Payroll_Group", "Description", "Payroll_Code");

            dtBillDate.Format = DateTimePickerFormat.Custom;
            dtBillDate.CustomFormat = "MM/dd/yyyy";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            cmbCompany.SelectedIndex = -1;
            cmbRank.SelectedIndex = -1;
            dtBillDate.Value = DateTime.Today;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Delete First if generated is not save 
            //to avoid double entry purposes
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "DELETE Billing WHERE BillDate = '" + dtBillDate.Value.ToShortDateString() + "' and isDone = 0";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Billing WHERE Deduction_Code like '%LOAN%' and BillDate ='" + dtBillDate.Value.ToShortDateString() + "' and Company_Code = '"+ cmbCompany.SelectedValue.ToString() +"' and Payroll_Code ='"+ cmbRank.SelectedValue.ToString() +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    clsBilling.generateBillingAccndgCompRank(dgvTempBalances, cmbCompany.SelectedValue.ToString(), cmbRank.SelectedValue.ToString(), dtBillDate);
                }
                
            }

            if(cmbCompany.SelectedValue.ToString() != "COMP021")
            {
                clsBilling.generateMembershipFee(dtBillDate, cmbCompany.SelectedValue.ToString(), cmbRank.SelectedValue.ToString());
                clsBilling.generateShareCapital(dtBillDate, cmbCompany.SelectedValue.ToString(), cmbRank.SelectedValue.ToString());
                clsBilling.generateSavings(dtBillDate, cmbCompany.SelectedValue.ToString(), cmbRank.SelectedValue.ToString());
            }
            else
            {
                //SMART BILLING
                clsBilling.generateMembershipFeeSMART(dtBillDate, cmbCompany.SelectedValue.ToString(), cmbRank.SelectedValue.ToString());
                clsBilling.generateShareCapitalSMART(dtBillDate, cmbCompany.SelectedValue.ToString(), cmbRank.SelectedValue.ToString());
                clsBilling.generateSavingsSMART(dtBillDate, cmbCompany.SelectedValue.ToString(), cmbRank.SelectedValue.ToString());

            }

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT EmployeeID,Company_Code,Payroll_Code,Deduction_Code,TotalDueAmount FROM Billing WHERE Company_Code = '"+ cmbCompany.SelectedValue.ToString() +"' and Payroll_Code = '"+ cmbRank.SelectedValue.ToString() +"' and BillDate = '"+ dtBillDate.Text +"' ORDER BY userid", con))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                    
                    dataGridView1.Columns["EmployeeID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["Company_Code"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["Payroll_Code"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["Deduction_Code"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["TotalDueAmount"].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count == 0)
            {
                Alert.show("No record(s) to save.", Alert.AlertType.error);
                return;
            }

            DialogResult result = MessageBox.Show(this, "Are you sure you want to continue?", "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //Update amount first
                updateTotalAmount();
                //#2
                displayinTemp();
                //#3
                updateMembershipFee();
                //#4
                updateLoanDetails();
                //#5
                moveToCollection();
                //#6
                updateBilling();
                //#7
                btnExport.Enabled = true;

                //Next STEP
                //2. display  the data in temp datagridview
                //3. update status in billing table = 1
                //4. update status in membersfee table [=1 , date selected]
                //5. update status in loan details [SELECT * FROM BILLING WHERE DEDUCTION_CODE = LOAN PRINCIPAL THEN GET TOP 1 FROM LOAN DETAILS]
                //6. move to collection table [MOVE ALL DATA IN BILLING TO COLLECTION TABLE]
                //7. enable export button (ADD THIS BUTTON)
                //8. Create function for export (Refer to Another program created)


            }
        }

        #region UPDATE OF BILLING
        //======================================================================================================================
        //                                  ALL TASK UPDATING COMES HERE
        //======================================================================================================================

        //#1
        //UPDATE PRINCIPAL TOTAL AMOUNT 
        //PRINCIPAL TOTAL AMOUNT + DEPENDENT TOTAL AMOUNT
        public void updateTotalAmount()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //Delete first
                SqlCommand cmdDelete = new SqlCommand();
                cmdDelete.Connection = con;
                cmdDelete.CommandText = "DELETE ExportBilling";
                cmdDelete.CommandType = CommandType.Text;
                cmdDelete.ExecuteNonQuery();


                //Move principal to temp table
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_TempPrincipalBilling";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BillDate", dtBillDate.Value.ToShortDateString());

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                string str = Convert.ToString(dtBillDate.Value.ToString("yyyy/MM/dd"));
                str = str.Replace("/", "");


                for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                {

                    try
                    {
                        //Insert to Export table
                        SqlCommand cmdInsert = new SqlCommand();
                        cmdInsert.Connection = con;
                        cmdInsert.CommandText = "sp_InsertExportBilling";
                        cmdInsert.CommandType = CommandType.StoredProcedure;
                        cmdInsert.Parameters.AddWithValue("@employeeID", ds.Tables[0].Rows[x]["employeeID"].ToString());
                        cmdInsert.Parameters.AddWithValue("@BillDate", str);
                        cmdInsert.Parameters.AddWithValue("@TotalDueAmount", Convert.ToDecimal(ds.Tables[0].Rows[x]["TotalDueAmount"].ToString()));
                        cmdInsert.ExecuteNonQuery();
                    }
                    catch
                    {
                    }

                }


                //Move dependent to temp table
                SqlCommand cmdDependent = new SqlCommand();
                cmdDependent.Connection = con;
                cmdDependent.CommandText = "sp_TempDependentBilling";
                cmdDependent.CommandType = CommandType.StoredProcedure;
                cmdDependent.Parameters.AddWithValue("@BillDate", dtBillDate.Value.ToShortDateString());

                SqlDataAdapter adapterDep = new SqlDataAdapter(cmdDependent);
                DataSet dsDep = new DataSet();
                adapterDep.Fill(dsDep);


                for (int y = 0; y < dsDep.Tables[0].Rows.Count; y++)
                {
                    try
                    {
                        //UPDATE THE TOTAL AMOUNT DUE OF PRINCIPAL ACCOUNT
                        SqlCommand cmdUpDue = new SqlCommand();
                        cmdUpDue.Connection = con;
                        cmdUpDue.CommandText = "sp_UpdateExportBilling";
                        cmdUpDue.CommandType = CommandType.StoredProcedure;
                        cmdUpDue.Parameters.AddWithValue("@BillDate", str);
                        cmdUpDue.Parameters.AddWithValue("@PrincipalID", dsDep.Tables[0].Rows[y]["PrincipalID"].ToString());
                        cmdUpDue.Parameters.AddWithValue("@TotalDueAmount", Convert.ToDecimal(dsDep.Tables[0].Rows[y]["TotalDueAmount"].ToString()));
                        cmdUpDue.ExecuteNonQuery();
                    }
                    catch
                    {

                    }
                    
                }

            }
        }


        //#2 
        public void displayinTemp()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_returnExportBilling";
                cmd.CommandType = CommandType.StoredProcedure;
                
                SqlDataAdapter adapterTemp = new SqlDataAdapter(cmd);
                DataSet dsTemp = new DataSet();
                adapterTemp.Fill(dsTemp);

                if(dsTemp.Tables[0].Rows.Count > 0)
                {
                    dgvPrincipal.Rows.Clear();

                    decimal total;

                    for(int x = 0; x < dsTemp.Tables[0].Rows.Count; x++)
                    {
                        total = Convert.ToDecimal(dsTemp.Tables[0].Rows[x]["TotalDueAmount"].ToString());

                        if(total > clsParameter.DeferredAmount())
                        {
                            total = 50000;
                        }

                        dgvPrincipal.Rows.Add(
                            dsTemp.Tables[0].Rows[x]["employeeID"].ToString(),
                            dsTemp.Tables[0].Rows[x]["LastName"].ToString(),
                            dsTemp.Tables[0].Rows[x]["FirstName"].ToString(),
                            dsTemp.Tables[0].Rows[x]["MiddleName"].ToString(),
                            dsTemp.Tables[0].Rows[x]["BillDate"].ToString(),
                            total
                            );
                    }
                }
            }
        }

        //#3 update status in billing table

        public void updateBilling()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE Billing SET isDone = 1 WHERE BillDate = '"+ dtBillDate.Value.ToShortDateString() +"'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }


        //#4
        public void updateMembershipFee()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //GET LIST FIRST IN MEMBERSHIP FEE
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Billing WHERE Deduction_Code = 'MEMBERSHIP FEE' and BillDate = '"+ dtBillDate.Value.ToShortDateString() +"'", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if(ds.Tables[0].Rows.Count > 0)
                {
                    //theres a record for membership fee
                    //update now in table membersfee
                    for(int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE membersFee set isDone = 1, DateDone = '"+ dtBillDate.Value.ToShortDateString() +"' WHERE userid ='"+ ds.Tables[0].Rows[x]["userid"].ToString() +"'";
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        //#5
        public void updateLoanDetails()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();


                //GET LIST FIRST IN BILLING
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Billing WHERE Deduction_Code = 'LOAN PRINCIPAL' and BillDate = '" + dtBillDate.Value.ToShortDateString() + "'", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        //Select top 1  first per loan no
                        SqlDataAdapter adapterLoan = new SqlDataAdapter("SELECT TOP 1 * FROM Loan_Details WHERE loan_no = '" + ds.Tables[0].Rows[x]["Loan_No"].ToString() + "' and isdone = 0 and Schedule_Payment = '" + dtBillDate.Value.ToString() +"' order by PaymentNoSemi ASC", con);
                        DataSet dsLoan = new DataSet();
                        adapterLoan.Fill(dsLoan);

                        if(dsLoan.Tables[0].Rows.Count > 0)
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = con;
                            cmd.CommandText = "UPDATE Loan_Details SET isDone = 1 WHERE Loan_No ='" + ds.Tables[0].Rows[x]["Loan_No"].ToString() + "' and PaymentNoSemi = '" + dsLoan.Tables[0].Rows[0]["PaymentNoSemi"].ToString() + "'";
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        #region Move to Collection
        //#6 Move to Collection table
        public void moveToCollection()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Billing WHERE BillDate = '" + dtBillDate.Text + "' and isDone = 0", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if(ds.Tables[0].Rows.Count > 0)
                {
                    for(int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "sp_InsertCollection";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@userid",ds.Tables[0].Rows[x]["userid"].ToString());
                        cmd.Parameters.AddWithValue("@EmployeeID", ds.Tables[0].Rows[x]["EmployeeID"].ToString());
                        cmd.Parameters.AddWithValue("@PrincipalID", ds.Tables[0].Rows[x]["PrincipalID"].ToString());
                        cmd.Parameters.AddWithValue("@Company_Code",ds.Tables[0].Rows[x]["Company_Code"].ToString());
                        cmd.Parameters.AddWithValue("@Payroll_Code",ds.Tables[0].Rows[x]["Payroll_Code"].ToString());
                        cmd.Parameters.AddWithValue("@Deduction_Code",ds.Tables[0].Rows[x]["Deduction_Code"].ToString());
                        cmd.Parameters.AddWithValue("@Account_Dr", returnAccountCode(ds.Tables[0].Rows[x]["Deduction_Code"].ToString(), ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                        cmd.Parameters.AddWithValue("@Account_Cr", returnAccountCodeCR(ds.Tables[0].Rows[x]["Deduction_Code"].ToString(), ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                        cmd.Parameters.AddWithValue("@Loan_No", ds.Tables[0].Rows[x]["Loan_No"].ToString());
                        cmd.Parameters.AddWithValue("@Loan_Type", returnLoanType(ds.Tables[0].Rows[x]["Loan_No"].ToString()));
                        cmd.Parameters.AddWithValue("@Priority", ds.Tables[0].Rows[x]["Priority"].ToString());
                        cmd.Parameters.AddWithValue("@Interest", ds.Tables[0].Rows[x]["Interest"].ToString());
                        cmd.Parameters.AddWithValue("@Principal", ds.Tables[0].Rows[x]["Principal"].ToString());
                        cmd.Parameters.AddWithValue("@DueAmount", ds.Tables[0].Rows[x]["TotalDueAmount"].ToString());
                        cmd.Parameters.AddWithValue("@Payroll_Date", ds.Tables[0].Rows[x]["BillDate"].ToString());
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public string returnLoanType(string loanNo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Loan_Type FROM Loan WHERE Loan_No = '"+ loanNo +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                try
                {
                    return dt.Rows[0].ItemArray[0].ToString();
                }
                catch
                {
                    return "";
                }
               
            }
        }

        public string returnAccountCode(string Deduction_Code,string loanNo)
        {
            ret = "";
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                switch (Deduction_Code)
                {
                    case "SHARE CAPITAL":
                        ret = "363";
                        break;
                    case "SAVINGS DEPOSIT":
                        ret = "300.1";
                        break;
                    case "DEFERRED INTEREST":
                        ret = "314";
                        break;
                    case "DEFERRED PRINCIPAL":
                        //Return Past Due Cr from loan type of loanNo
                        SqlDataAdapter adaper = new SqlDataAdapter("SELECT PastDue_Account FROM Loan_Type WHERE Loan_Type = '"+ returnLoanType(loanNo) +"'", con);
                        DataTable dt = new DataTable();
                        adaper.Fill(dt);

                        ret = dt.Rows[0].ItemArray[0].ToString();
                        break;
                    case "LOAN PRINCIPAL":
                        //Return Past Due Cr from loan type of loanNo
                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT Account_Dr FROM Loan_Type WHERE Loan_Type = '" + returnLoanType(loanNo) + "'", con);
                        DataTable dt2 = new DataTable();
                        adapter.Fill(dt2);

                        ret = dt2.Rows[0].ItemArray[0].ToString();
                        break;
                    case "LOAN INTEREST":
                        ret = "401.1";
                        break;
                    case "MEMBERSHIP FEE":
                        ret = "150.19";
                        break;
                }

                return ret;
            }
        }

        public string returnAccountCodeCR(string Deduction_Code, string loanNo)
        {
            ret = "";
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                switch (Deduction_Code)
                {
                    case "SHARE CAPITAL":
                        ret = "363";
                        break;
                    case "SAVINGS DEPOSIT":
                        ret = "300.1";
                        break;
                    case "DEFERRED INTEREST":
                        //Return Past Due Cr from loan type of loanNo
                        SqlDataAdapter adaperDefInt = new SqlDataAdapter("SELECT PastDue_Account FROM Loan_Type WHERE Loan_Type = '" + returnLoanType(loanNo) + "'", con);
                        DataTable d1t = new DataTable();
                        adaperDefInt.Fill(d1t);

                        ret = d1t.Rows[0].ItemArray[0].ToString();
                        break;
                    case "DEFERRED PRINCIPAL":
                        //Return Past Due Cr from loan type of loanNo
                        SqlDataAdapter adaper = new SqlDataAdapter("SELECT PastDue_Account FROM Loan_Type WHERE Loan_Type = '" + returnLoanType(loanNo) + "'", con);
                        DataTable dt = new DataTable();
                        adaper.Fill(dt);

                        ret = dt.Rows[0].ItemArray[0].ToString();
                        break;
                    case "LOAN PRINCIPAL":
                        //Return Past Due Cr from loan type of loanNo
                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT PastDue_Account FROM Loan_Type WHERE Loan_Type = '" + returnLoanType(loanNo) + "'", con);
                        DataTable dt2 = new DataTable();
                        adapter.Fill(dt2);

                        ret = dt2.Rows[0].ItemArray[0].ToString();
                        break;
                    case "LOAN INTEREST":
                        //Return Past Due Cr from loan type of loanNo
                        SqlDataAdapter adapterInt = new SqlDataAdapter("SELECT PastDue_Account FROM Loan_Type WHERE Loan_Type = '" + returnLoanType(loanNo) + "'", con);
                        DataTable dtInt = new DataTable();
                        adapterInt.Fill(dtInt);

                        ret = dtInt.Rows[0].ItemArray[0].ToString();
                        break;
                    case "MEMBERSHIP FEE":
                        ret = "150.19";
                        break;
                }

                return ret;
            }
        }
        #endregion


        #endregion

        private void btnExport_Click(object sender, EventArgs e)
        {
            if(cmbCompany.SelectedValue.ToString() == "COMP021")
            {
                //GENERATE TEXT FILE FOR SMART
                createSMARTtxtFile();
            }
            else
            {
                //NOT SMART
                copyAlltoClipboard();
                Microsoft.Office.Interop.Excel.Application xlexcel;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;
                xlexcel = new Microsoft.Office.Interop.Excel.Application();
                xlexcel.Visible = true;
                xlWorkBook = xlexcel.Workbooks.Add(misValue);
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, 1];
                CR.Select();
                xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            }
            btnExport.Enabled = false;
        }

        public void createSMARTtxtFile()
        {
            saveFileDialog1.FileName = "Payroll.txt";

            // set filters - this can be done in properties as well
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fs1 = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fs1);

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    string dtReplace = Convert.ToString(dtBillDate.Value.ToString("MM/dd/yy"));
                    dtReplace = dtReplace.Replace("/", "");

                    writer.Write("0000000"); //instcode
                    writer.Write("01"); //batchcode
                    writer.Write("               "); //white space
                    writer.Write(dtReplace);//BillDate
                    writer.Write("H");//Header
                    writer.Write(dtReplace);//Upload Date
                    writer.Write("                                                 ");//white space
                    writer.Write("001");//Default
                    writer.Write("0967096504353");//pecciAccountNo
                    writer.Write("2020");//Default
                    writer.Write(Environment.NewLine); //For New Line

                    //Write Details
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_SmartBillingTextTotxtFile";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BillDateGenerated", dtBillDate.Text);
                    cmd.Parameters.AddWithValue("@billdate", dtReplace);
                    cmd.Parameters.AddWithValue("@uploaddate", dtReplace);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    for(int x = 0; x < dt.Rows.Count; x++)
                    {
                        writer.Write(dt.Rows[x].ItemArray[0].ToString());
                        writer.Write(Environment.NewLine); //For New Line
                    }

                    //Write Trailer
                    SqlCommand cmdTrailer = new SqlCommand();
                    cmdTrailer.Connection = con;
                    cmdTrailer.CommandText = "sp_SmartBillingTextTotxtFileTrailer";
                    cmdTrailer.CommandType = CommandType.StoredProcedure;
                    cmdTrailer.Parameters.AddWithValue("@BillDateGenerated", dtBillDate.Text);
                    cmdTrailer.Parameters.AddWithValue("@billdate", dtReplace);
                    cmdTrailer.Parameters.AddWithValue("@uploaddate", dtReplace);

                    SqlDataAdapter adapterTrailer = new SqlDataAdapter(cmdTrailer);
                    DataTable dtTrailer = new DataTable();
                    adapterTrailer.Fill(dtTrailer);

                    writer.Write(dtTrailer.Rows[0].ItemArray[0].ToString());
                    
                    writer.Close();
                }

                Alert.show("Successfully exported.", Alert.AlertType.success);
            }
        }

        private void copyAlltoClipboard()
        {
            dgvPrincipal.SelectAll();
            DataObject dataObj = dgvPrincipal.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }
    }
}
