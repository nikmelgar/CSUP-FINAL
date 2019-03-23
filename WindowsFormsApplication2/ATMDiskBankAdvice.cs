using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;


namespace WindowsFormsApplication2
{
    public partial class ATMDiskBankAdvice : Form
    {
        public ATMDiskBankAdvice()
        {
            InitializeComponent();
        }

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        CrystalDecisions.Shared.TableLogOnInfo li;

        //==========================================================
        //                  BPI CREATE DISK VARIABLE
        //==========================================================
        decimal ceil = 0;
        decimal totalAmount = 0;
        int cnt;
        decimal ttalAccntHash = 0;
        decimal ttalTrailerAmount = 0;
        double ttalTrailerHash = 0;
        string tHash;
        //==========================================================


        Global global = new Global();

        Classes.clsATMDiskAdvice clsATMDiskAdvice = new Classes.clsATMDiskAdvice();

        decimal number;
        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        string purpose;
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void ATMDiskBankAdvice_Load(object sender, EventArgs e)
        {
            clsATMDiskAdvice.loadATMDiskAdvice(dgvATM);
            clsATMDiskAdvice.loadBankCode(cmbBank);
        }

        private void cmbBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("select Bank_Code,PURPOSE,sum(Amount) as Total FROM ATM WHERE Deposited is null and Bank_Code ='" + cmbBank.Text + "' Group by Bank_Code,Purpose", con);
                dt = new DataTable();
                adapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    listView1.Items.Add(dt.Rows[i].ItemArray[1].ToString() + " - " + Convert.ToDecimal(dt.Rows[i].ItemArray[2].ToString()).ToString("#,0.00")).Checked = true;
                }

                //Display Every Combobox Change
                clsATMDiskAdvice.displayBrandAndCompanyCode(txtBranchCode, txtCompanyCOde, txtAccountNo, cmbBank.Text);
            }
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            loadDepositAmount();
        }

        //Compute All selected
        public void loadDepositAmount()
        {
           if(listView1.Items.Count >= 1)
            {
                Decimal sum = 0;
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if(listView1.Items[i].Checked == true)
                    {
                        using (SqlConnection con = new SqlConnection(global.connectString()))
                        {
                            con.Open();

                            adapter = new SqlDataAdapter("select Bank_Code,PURPOSE,sum(Amount) as Total FROM ATM WHERE Deposited is null and Bank_Code ='" + cmbBank.Text + "' Group by Bank_Code,Purpose", con);
                            dt = new DataTable();
                            adapter.Fill(dt);

                            sum += Convert.ToDecimal(dt.Rows[i].ItemArray[2].ToString());
                        }  
                    }
                }

                txtDepositAmount.Text = sum.ToString("#,0.00");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clsATMDiskAdvice.loadATMDiskAdvice(dgvATM);
            clsATMDiskAdvice.loadBankCode(cmbBank);

            sig1name.Text = "";
            sig1pos.Text = "";
            sig2name.Text = "";
            sig2pos.Text = "";
            sig3name.Text = "";
            sig3pos.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(dgvATM.Rows.Count == 0)
            {
                Alert.show("No ATM transaction.", Alert.AlertType.error);
                return;
            }

            if(cmbBank.Text == "")
            {
                //Message Alert
                Alert.show("Bank Code is required!",Alert.AlertType.error);
                return;
            }

            if(txtTrans.Text == "")
            {
                //Message Alert
                Alert.show("Transaction Number is required!", Alert.AlertType.error);
                return;
            }

            //check if theres a selected purpose
            int icount = 0;
            foreach (ListViewItem lItem in listView1.Items)
            {
                if (lItem.Checked == true)
                {
                    icount = icount + 1;
                }
            }

            if(icount == 0)
            {
                Alert.show("Please check at least 1 purpose!", Alert.AlertType.error);
                return;
            }


            if (cmbBank.Text == "BPI")
            {
                CreateBPI();
            }
            else if(cmbBank.Text == "MBTC")
            {
                createMBTCDisk();
            }
            else if(cmbBank.Text == "BDO")
            {
                CreateBDO();
            }


        }

        public void CreateBDO()
        {
            //this code segment write data to file.
            saveFileDialog1.FileName = "BDO.txt";

            // set filters - this can be done in properties as well
            saveFileDialog1.Filter = "Text files (*.txt)|*.dat|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fs1 = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fs1);


                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    int cnt = 0;
                    int icount = 0;
                    foreach (ListViewItem lItem in listView1.Items)
                    {
                        if (lItem.Checked == true)
                        {
                            icount = icount + 1;
                        }
                    }

                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if (listView1.Items[i].Checked == true)
                        {
                            purpose += "'" + getBetween(listView1.Items[i].Text, "-") + "'";
                            if (i != icount - 1)
                            {
                                purpose += " , ";
                            }
                        }
                    }

                    string str = "select distinct account_No,sum(Amount) as total,Name FROM ATM where Bank_Code='" + cmbBank.Text + "' and Deposited is null and purpose in (" + purpose + ") GROUP BY Account_No,Name Order by Name ASC";
                    adapter = new SqlDataAdapter(str, con);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    while (cnt != dt.Rows.Count)
                    {
                        writer.Write(dt.Rows[cnt].ItemArray[0].ToString());
                        writer.Write("\t");
                        writer.Write(dt.Rows[cnt].ItemArray[1].ToString());
                        cnt = cnt + 1;
                    }

                    writer.Close();
                }

                Alert.show("BDO DISK Successfully Created!", Alert.AlertType.success);
            }
        }
        public void CreateBPI()
        {
            //this code segment write data to file.
            saveFileDialog1.FileName = "09045.dat";

            // set filters - this can be done in properties as well
            saveFileDialog1.Filter = "Text files (*.dat)|*.dat|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fs1 = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fs1);


                //GET TOTALS AND OTHER INFO
                //===================================================================================================
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    int icount = 0;
                    foreach (ListViewItem lItem in listView1.Items)
                    {
                        if (lItem.Checked == true)
                        {
                            icount = icount + 1;
                        }
                    }

                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if (listView1.Items[i].Checked == true)
                        {
                            purpose += "'" + getBetween(listView1.Items[i].Text, "-") + "'";
                            if (i != icount - 1)
                            {
                                purpose += " , ";
                            }
                        }
                    }


                    string str = "select distinct account_No,Bank_code,sum(Amount) as total , Name FROM ATM where Bank_Code='" + cmbBank.Text + "' and Deposited is null and purpose in (" + purpose + ") GROUP BY Account_No,Bank_Code,Name Order by Name ASC";
                    adapter = new SqlDataAdapter(str, con);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    string str2 = "select distinct account_No,Bank_code,sum(Amount) as total FROM ATM where Bank_Code='" + cmbBank.Text + "' and Deposited is null and purpose in (" + purpose + ") GROUP BY Account_No,Bank_Code Order by total desc";
                    SqlDataAdapter adapter2 = new SqlDataAdapter(str2, con);
                    DataTable dt2 = new DataTable();
                    adapter2.Fill(dt2);


                    purpose = string.Empty;

                    ceil = Convert.ToDecimal(dt2.Rows[0].ItemArray[2].ToString()); //Ceil for Header
                                                                                   //Get Accnt No Count 5-10
                    String sp56;
                    String sp78;
                    String sp910;


                    //GET Total AMount
                    for (int x = 0; x < dt2.Rows.Count; x++)
                    {
                        totalAmount += Convert.ToDecimal(dt2.Rows[x].ItemArray[2].ToString());
                    }

                    //====================================================================================================
                    //                              BPI HEADER
                    //====================================================================================================

                    writer.Write("H"); //Default Value
                    writer.Write(txtCompanyCOde.Text); //Company Code
                    writer.Write(dtDepDate.Value.ToString("MMddyy")); //Deposit Date
                    writer.Write(string.Format(txtTrans.Text, "00")); //Transaction or Batch Upload
                    writer.Write("1");  //Fix Value
                    writer.Write(txtAccountNo.Text); //Account Number
                    writer.Write(txtBranchCode.Text); //Branch Code                
                    writer.Write(String.Format("{0:000000000000}", ceil * 100)); //Highest Amount 
                    writer.Write(String.Format("{0:000000000000}", totalAmount * 100)); //Total Amount
                    writer.Write("1" + new string(' ', 75));
                    writer.Write(Environment.NewLine);


                    //====================================================================================================
                    //                              BPI DETAILS
                    //====================================================================================================
                    cnt = 0;
                    double dHash = 0;
                    string sHash;
                    ttalAccntHash = 0;
                    ttalTrailerAmount = 0;
                    ttalTrailerHash = 0;


                    while (cnt != dt.Rows.Count)
                    {
                        writer.Write("D"); //Fix Value
                        writer.Write(txtCompanyCOde.Text);
                        writer.Write(dtDepDate.Value.ToString("MMddyy")); //Deposit Date
                        writer.Write(string.Format(txtTrans.Text, "00")); //Transaction or Batch Upload
                        writer.Write("3"); //Fix Value
                        writer.Write(dt.Rows[cnt].ItemArray[0].ToString());
                        ttalAccntHash = ttalAccntHash + Convert.ToDecimal(dt.Rows[cnt].ItemArray[0].ToString());
                        writer.Write(String.Format("{0:000000000000}", Convert.ToDecimal(dt.Rows[cnt].ItemArray[2].ToString()) * 100));
                        ttalTrailerAmount = Convert.ToDecimal(String.Format("{0:000000000000}", Convert.ToDecimal(dt.Rows[cnt].ItemArray[2].ToString()) * 100)) + ttalTrailerAmount;
                        sp56 = Mid(dt.Rows[cnt].ItemArray[0].ToString(), 5, 2);
                        sp78 = Mid(dt.Rows[cnt].ItemArray[0].ToString(), 7, 2);
                        sp910 = Mid(dt.Rows[cnt].ItemArray[0].ToString(), 9, 2);
                        dHash = (Convert.ToInt32(sp56) + Convert.ToInt32(sp78) + Convert.ToInt32(sp910)) * Convert.ToDouble(dt.Rows[cnt].ItemArray[2].ToString());
                        sHash = string.Format("{0:N}", dHash);
                        sHash = sHash.Replace(",", "");
                        sHash = sHash.Replace(".", "");
                        writer.Write(Convert.ToInt64(sHash).ToString("D12"));
                        ttalTrailerHash = ttalTrailerHash + Convert.ToDouble((Convert.ToInt32(sp56) + Convert.ToInt32(sp78) + Convert.ToInt32(sp910)) * Convert.ToDouble(dt.Rows[cnt].ItemArray[2].ToString()));
                        tHash = string.Format("{0:N}", ttalTrailerHash);
                        tHash = tHash.Replace(",", "");
                        tHash = tHash.Replace(".", "");
                        tHash = Convert.ToInt64(tHash).ToString("D18");
                        writer.Write(new string(' ', 79));
                        cnt = cnt + 1;
                        writer.Write(Environment.NewLine);
                    }


                    //====================================================================================================
                    //                              BPI TRAILER
                    //====================================================================================================
                    writer.Write("T"); //Default Value
                    writer.Write(txtCompanyCOde.Text); //Company Code
                    writer.Write(dtDepDate.Value.ToString("MMddyy")); //Deposit Date
                    writer.Write(string.Format(txtTrans.Text, "00")); //Transaction or Batch Upload
                    writer.Write("2");  //Fix Value
                    writer.Write(txtAccountNo.Text); //Account Number
                    writer.Write(String.Format("{0:000000000000000}", ttalAccntHash)); //Total account Hash
                    writer.Write(String.Format("{0:000000000000000}", ttalTrailerAmount)); //Total 
                    writer.Write(String.Format("{0:000000000000000000}", tHash)); //Total Amount
                    int trailerTailCnt = Convert.ToInt32(dt.Rows.Count.ToString());
                    writer.Write(String.Format("{0:00000}", trailerTailCnt));
                    writer.Write(new string(' ', 50));

                    writer.Close();
                }
                Alert.show("BPI DISK Successfully Created!", Alert.AlertType.success);
            }    
        }

        public void createMBTCDisk()
        {
            //this code segment write data to file.
            saveFileDialog1.FileName = "Payroll.dat";

            // set filters - this can be done in properties as well
            saveFileDialog1.Filter = "Text files (*.dat)|*.dat|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fs1 = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fs1);


                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    string CompName = "PECCI";
                    int cnt = 0;
                    int icount = 0;
                    foreach (ListViewItem lItem in listView1.Items)
                    {
                        if (lItem.Checked == true)
                        {
                            icount = icount + 1;
                        }
                    }

                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if (listView1.Items[i].Checked == true)
                        {
                            purpose += "'" + getBetween(listView1.Items[i].Text, "-") + "'";
                            if (i != icount - 1)
                            {
                                purpose += " , ";
                            }
                        }
                    }

                    string str = "select distinct account_No,Bank_code,sum(Amount) as total , Name FROM ATM where Bank_Code='" + cmbBank.Text + "' and Deposited is null and purpose in (" + purpose + ") GROUP BY Account_No,Bank_Code,Name Order by Name ASC";
                    adapter = new SqlDataAdapter(str, con);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    while (cnt != dt.Rows.Count)
                    {
                        writer.Write("2"); //Fix Value
                        writer.Write("096"); //Fix Value
                        writer.Write("26"); //Fix Value
                        writer.Write("001"); //Fix Value //Currency 001- peso , 002 - dollar
                        if (Mid(dt.Rows[cnt].ItemArray[0].ToString(), 2, 3) == "722")
                        {
                            writer.Write("096");
                        }
                        else
                        {
                            writer.Write(Mid(dt.Rows[cnt].ItemArray[0].ToString(), 2, 3));
                        }
                        writer.Write("0000000");
                        writer.Write(CompName);
                        writer.Write(new string(' ', 35));
                        writer.Write(String.Format("{0:0000000000}", dt.Rows[cnt].ItemArray[0].ToString())); //Account No.
                        writer.Write(String.Format("{0:000000000000000}", Convert.ToDecimal(dt.Rows[cnt].ItemArray[2].ToString()) * 100));
                        writer.Write("9");
                        writer.Write(txtCompanyCOde.Text);
                        writer.Write(dtDepDate.Value.ToString("MMddyyyy")); //Deposit Date
                        cnt = cnt + 1;
                    }

                    writer.Close();
                }

                Alert.show("MBTC DISK Successfully Created!", Alert.AlertType.success);
            }
        }

        //===============================================================================================
        //                    Move to Temp Table for TempBPI : For Advice Report
        //===============================================================================================
        public void movetoTempBPI()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                purpose = "";
                int icount = 0;
                foreach (ListViewItem lItem in listView1.Items)
                {
                    if (lItem.Checked == true)
                    {
                        icount = icount + 1;
                    }
                }

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (listView1.Items[i].Checked == true)
                    {
                        purpose += "'" + getBetween(listView1.Items[i].Text, "-") + "'";
                        if (i != icount - 1)
                        {
                            purpose += " , ";
                        }
                    }
                }

                SqlDataAdapter adapter = new SqlDataAdapter("select distinct account_No,Bank_code,sum(Amount) as total , Name FROM ATM where Bank_Code='" + cmbBank.Text + "' and Deposited is null and purpose in (" + purpose + ") GROUP BY Account_No,Bank_Code,Name Order by Name ASC", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);


                //====================================================================================================
                //                              BPI DETAILS
                //====================================================================================================
                String sp56;
                String sp78;
                String sp910;

                cnt = 0;
                double dHash = 0;
                string sHash;

                while (cnt != dt.Rows.Count)
                {

                    sp56 = Mid(dt.Rows[cnt].ItemArray[0].ToString(), 5, 2);
                    sp78 = Mid(dt.Rows[cnt].ItemArray[0].ToString(), 7, 2);
                    sp910 = Mid(dt.Rows[cnt].ItemArray[0].ToString(), 9, 2);
                    dHash = (Convert.ToInt32(sp56) + Convert.ToInt32(sp78) + Convert.ToInt32(sp910)) * Convert.ToDouble(dt.Rows[cnt].ItemArray[2].ToString());
                    sHash = string.Format("{0:N}", dHash);
                    sHash = sHash.Replace(",", "");
                    sHash = sHash.Replace(".", "");

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_InsertTempBPI";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@accntNo", dt.Rows[cnt].ItemArray[0].ToString());
                    cmd.Parameters.AddWithValue("@Name", dt.Rows[cnt].ItemArray[3].ToString());
                    cmd.Parameters.AddWithValue("@Amount", dt.Rows[cnt].ItemArray[2].ToString());
                    cmd.Parameters.AddWithValue("@HorizontalHash", Convert.ToInt64(sHash).ToString("D12"));
                    cmd.ExecuteNonQuery();

                    cnt = cnt + 1;
                }

                purpose = "";
            }
        }

        public static string Mid(string s, int a, int b)

        {

            string temp = s.Substring(a - 1, b);

            return temp;

        }

        private void button3_Click(object sender, EventArgs e)
        {
             
        }

        public static string getBetween(string strSource, string strEnd)
        {
            int Start, End;
            Start = 0;
            End = strSource.IndexOf(strEnd, Start);
            return strSource.Substring(Start, End - Start);
        }

        private void txtTrans_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Space);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(dgvATM.Rows.Count >= 1)
            {
                int icount = 0;
                purpose = "";
                foreach (ListViewItem lItem in listView1.Items)
                {
                    if (lItem.Checked == true)
                    {
                        icount = icount + 1;
                    }
                }

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (listView1.Items[i].Checked == true)
                    {
                        purpose += "'" + getBetween(listView1.Items[i].Text, "-") + "'";
                        if (i != icount - 1)
                        {
                            purpose += " , ";
                        }
                    }
                }


                //If theres a value
                if (cmbBank.Text == "")
                {
                    Alert.show("Please select bank code first!", Alert.AlertType.error);
                    return;
                }
                else if(icount == 0)
                {
                    Alert.show("Please check at least one on deposit purpose!", Alert.AlertType.error);
                    return;
                }

                string msg = Environment.NewLine + "Are you sure you want to continue?";
                DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //GO Tag selected purpose
                    using (SqlConnection con = new SqlConnection(global.connectString()))
                    {
                        con.Open();

                        //===========================================================================================
                        //              GET THE PURPOSE IF = SD OR SAVINGS
                        //===========================================================================================
                        if (purpose.Contains("SD") == true)
                        {
                            //========================================================================
                            // GET JV NO FROM ATM TABLE THEN UPDATE WITHDRAWAL AND JV HEADER
                            //========================================================================
                            SqlDataAdapter getAdapter = new SqlDataAdapter("SELECT DISTINCT(jv_no),bank_Code FROM Atm WHERE Purpose = 'SD' and bank_code = '" + cmbBank.Text + "' and Deposited is null", con);
                            DataTable dtGet = new DataTable();
                            getAdapter.Fill(dtGet);

                            //========================================================================
                            //  UPDATE TABLE WITHDRAWAL AND JOURNAL HEADER
                            //========================================================================
                            int x = 0;
                            while (x != dtGet.Rows.Count)
                            {
                                SqlCommand cmdwd = new SqlCommand();
                                cmdwd.Connection = con;
                                cmdwd.CommandText = "UPDATE Withdrawal_Slip SET Posted = 1, Posted_By = '" + Classes.clsUser.Username + "' WHERE JV_No = '" + dtGet.Rows[x].ItemArray[0].ToString() + "'";
                                cmdwd.CommandType = CommandType.Text;
                                cmdwd.ExecuteNonQuery();

                                SqlCommand cmdJv = new SqlCommand();
                                cmdJv.Connection = con;
                                cmdJv.CommandText = "UPDATE Journal_Header SET Posted = 1, Posted_By = '" + Classes.clsUser.Username + "' WHERE JV_No = '" + dtGet.Rows[x].ItemArray[0].ToString() + "'";
                                cmdJv.CommandType = CommandType.Text;
                                cmdJv.ExecuteNonQuery();

                                x = x + 1;
                            }
                        }


                        //===========================================================================================
                        //              UPDATING ATM TABLE DEPOSITED = DATE TODAY
                        //===========================================================================================

                        string str = "UPDATE ATM SET Deposited ='" + dtDepDate.Text + "' where Bank_Code='" + cmbBank.Text + "' and Deposited is null and purpose in (" + purpose + ")";
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = str;
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();

                        Alert.show("ATM Successfully Tagged!", Alert.AlertType.success);

                        clsATMDiskAdvice.loadATMDiskAdvice(dgvATM);
                        clsATMDiskAdvice.loadBankCode(cmbBank);

                        if (dgvATM.Rows.Count == 0)
                        {
                            cmbBank.Text = "";
                            listView1.Items.Clear();
                        }
                    }
                }
            }
            else
            {
                //No Value
                Alert.show("No ATM transaction to be tagged.", Alert.AlertType.error);
                return;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if(txtTrans.Text == "")
            {
                Alert.show("Transaction number is required.", Alert.AlertType.error);
                return;
            }

            //check if theres a selected purpose
            int icount = 0;
            foreach (ListViewItem lItem in listView1.Items)
            {
                if (lItem.Checked == true)
                {
                    icount = icount + 1;
                }
            }

            if (icount == 0)
            {
                Alert.show("Please check at least 1 purpose!", Alert.AlertType.error);
                return;
            }


            if (cmbBank.Text == "BPI")
            {
                //delete first atm temp BPI
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "Delete tempBPI";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                //Print Report
                loadBPIReport();
            }
            else if(cmbBank.Text == "BDO")
            {
                loadBDOReport();
            }
            else if(cmbBank.Text == "MBTC")
            {
                loadMBTCRepor();
            }
        }
        public void loadMBTCRepor()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                CrystalDecisions.Shared.TableLogOnInfo li;

                //===============================================
                //          GETTING BANK INFO
                //===============================================
                adapter = new SqlDataAdapter("SELECT * FROM Bank WHERE Bank_Code ='MBTC'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                //===============================================
                //          STRING QUERY
                //===============================================
                int icount = 0;
                purpose = "";
                foreach (ListViewItem lItem in listView1.Items)
                {
                    if (lItem.Checked == true)
                    {
                        icount = icount + 1;
                    }
                }

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (listView1.Items[i].Checked == true)
                    {
                        purpose += "'" + getBetween(listView1.Items[i].Text, "-") + "'";
                        if (i != icount - 1)
                        {
                            purpose += " , ";
                        }
                    }
                }

                string str = "select distinct account_No,sum(Amount) as Amount,Name FROM ATM where Bank_Code='" + cmbBank.Text + "' and Deposited is null and purpose in (" + purpose + ") GROUP BY Account_No,Name Order by Name ASC";
                SqlDataAdapter adapter1 = new SqlDataAdapter(str, con);
                DataTable dt1 = new DataTable();
                DataSet ds = new DataSet();



                string contactPerson = dt.Rows[0].ItemArray[7].ToString();
                string bnkName = dt.Rows[0].ItemArray[1].ToString();
                string branchName = dt.Rows[0].ItemArray[2].ToString();

                ReportsForms.rptMBTCListing cr = new ReportsForms.rptMBTCListing();
                ReportsForms.rptMBTC rpt = new ReportsForms.rptMBTC();

                li = new TableLogOnInfo();

                li.ConnectionInfo.IntegratedSecurity = false;

                adapter1.Fill(ds, "ATM");
                dt = ds.Tables["ATM"];
                cr.SetDataSource(ds.Tables["ATM"]);

                //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

                cr.SetParameterValue("maincontactperson", contactPerson);
                cr.SetParameterValue("mainbankName", bnkName);
                cr.SetParameterValue("mainbranch", branchName);

                try
                {
                    string num = txtDepositAmount.Text;

                    string word;


                    if (txtDepositAmount.Text == "")
                    {
                        txtDepositAmount.Text = "";
                    }
                    else
                    {
                        number = decimal.Parse(num.ToString());

                        if (number.ToString() == "0")
                        {
                            MessageBox.Show("The number in currency fomat is \nZero Only");
                        }
                        else
                        {
                            word = Classes.clsSavingsDataEntry.ConvertToWords(number.ToString());
                        }
                    }


                    Console.ReadKey();
                }
                catch (System.Exception ex)
                {


                }


                string Msg = Classes.clsSavingsDataEntry.ConvertToWords(number.ToString()) + " (P " + txtDepositAmount.Text + ")";

                cr.SetParameterValue("mainmsg", Msg);
                cr.SetParameterValue("mainaccntNumber", txtAccountNo.Text);
                cr.SetParameterValue("mainpurpose", "loans/savings/refund");
                string dref = DateTime.Now.ToString("yyyy-MM-dd");
                cr.SetParameterValue("mainrefno", "Reference No: MBTC" + dref.Replace("-", "") + txtTrans.Text);

                //Signature
                cr.SetParameterValue("mainsig1Name", sig1name.Text);
                cr.SetParameterValue("mainsig1pos", sig1pos.Text);
                cr.SetParameterValue("mainsig2name", sig2name.Text);
                cr.SetParameterValue("mainsig2pos", sig2pos.Text);
                cr.SetParameterValue("mainsig3name", sig3name.Text);
                cr.SetParameterValue("mainsig3pos", sig3pos.Text);


                rpt.crystalReportViewer1.ReportSource = cr;
                rpt.ShowDialog();
            }
        }
        public void loadBDOReport()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                CrystalDecisions.Shared.TableLogOnInfo li;

                //===============================================
                //          GETTING BANK INFO
                //===============================================
                adapter = new SqlDataAdapter("SELECT * FROM Bank WHERE Bank_Code ='BDO'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);


                //===============================================
                //          STRING QUERY
                //===============================================
                int icount = 0;
                purpose = "";
                foreach (ListViewItem lItem in listView1.Items)
                {
                    if (lItem.Checked == true)
                    {
                        icount = icount + 1;
                    }
                }

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (listView1.Items[i].Checked == true)
                    {
                        purpose += "'" + getBetween(listView1.Items[i].Text, "-") + "'";
                        if (i != icount - 1)
                        {
                            purpose += " , ";
                        }
                    }
                }

                string str = "select distinct account_No,sum(Amount) as Amount,Name FROM ATM where Bank_Code='" + cmbBank.Text + "' and Deposited is null and purpose in (" + purpose + ") GROUP BY Account_No,Name Order by Name ASC";
                SqlDataAdapter adapter1 = new SqlDataAdapter(str, con);
                DataTable dt1 = new DataTable();
                DataSet ds = new DataSet();



                string contactPerson = dt.Rows[0].ItemArray[7].ToString();
                string bnkName = dt.Rows[0].ItemArray[1].ToString();
                string branchName = dt.Rows[0].ItemArray[2].ToString();

                ReportsForms.rptBDOListing cr = new ReportsForms.rptBDOListing();
                ReportsForms.rptBDO rpt = new ReportsForms.rptBDO();

                li = new TableLogOnInfo();

                li.ConnectionInfo.IntegratedSecurity = false;

                adapter1.Fill(ds, "ATM");
                dt = ds.Tables["ATM"];
                cr.SetDataSource(ds.Tables["ATM"]);

                //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

                cr.SetParameterValue("maincontactperson", contactPerson);
                cr.SetParameterValue("mainbankName", bnkName);
                cr.SetParameterValue("mainbranch", branchName);

                try
                {
                    string num = txtDepositAmount.Text;

                    string word;


                    if (txtDepositAmount.Text == "")
                    {
                        txtDepositAmount.Text = "";
                    }
                    else
                    {
                        number = decimal.Parse(num.ToString());

                        if (number.ToString() == "0")
                        {
                            MessageBox.Show("The number in currency fomat is \nZero Only");
                        }
                        else
                        {
                            word = Classes.clsSavingsDataEntry.ConvertToWords(number.ToString());
                        }
                    }


                    Console.ReadKey();
                }
                catch (System.Exception ex)
                {


                }


                string Msg = Classes.clsSavingsDataEntry.ConvertToWords(number.ToString()) + " (P " + txtDepositAmount.Text + ")";

                cr.SetParameterValue("mainmsg", Msg);
                cr.SetParameterValue("mainaccntNumber", txtAccountNo.Text);
                cr.SetParameterValue("mainpurpose", "loans/savings/refund");
                string dref = DateTime.Now.ToString("yyyy-MM-dd");
                cr.SetParameterValue("mainrefno", "Reference No: BDO" + dref.Replace("-", "") + txtTrans.Text);

                //Signature
                cr.SetParameterValue("mainsig1Name", sig1name.Text);
                cr.SetParameterValue("mainsig1pos", sig1pos.Text);
                cr.SetParameterValue("mainsig2name", sig2name.Text);
                cr.SetParameterValue("mainsig2pos", sig2pos.Text);
                cr.SetParameterValue("mainsig3name", sig3name.Text);
                cr.SetParameterValue("mainsig3pos", sig3pos.Text);


                rpt.crystalReportViewer1.ReportSource = cr;
                rpt.ShowDialog();
            }
        }
        public void loadBPIReport()
        {
            //=====Move to tempBPI
            movetoTempBPI();

            CrystalDecisions.Shared.TableLogOnInfo li;

            //Get BPI Bank Information
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM Bank WHERE Bank_Code ='BPI'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                //==========================================================================
                //                  Ceiling
                //==========================================================================
                SqlDataAdapter adapterCeiling = new SqlDataAdapter("select min(Amount) from tempBPI", con);
                DataTable dtCeling = new DataTable();
                adapterCeiling.Fill(dtCeling);

                //==========================================================================
                //                  Get the total sum of accnt no
                //==========================================================================
                SqlDataAdapter adapterAccntNo = new SqlDataAdapter("SELECT sum(CONVERT(BIGINT,accntNo)) from TempBPI ", con);
                DataTable dtAccnt = new DataTable();
                adapterAccntNo.Fill(dtAccnt);

                //==========================================================================
                //                  Get the total sum of amount
                //==========================================================================
                SqlDataAdapter adpAmount = new SqlDataAdapter("SELECT sum(amount) from TempBPI ", con);
                DataTable dtAmount = new DataTable();
                adpAmount.Fill(dtAmount);

                string contactPerson = dt.Rows[0].ItemArray[7].ToString();
                string bnkName = dt.Rows[0].ItemArray[1].ToString();
                string branchName = dt.Rows[0].ItemArray[2].ToString();



                ReportsForms.rptBPIListing cr = new ReportsForms.rptBPIListing();
                ReportsForms.rptBPI rpt = new ReportsForms.rptBPI();

                li = new TableLogOnInfo();

                li.ConnectionInfo.IntegratedSecurity = false;


                //cr.SetDatabaseLogon("sa", "SYSADMIN", "192.168.255.176", "PECCI-NEW");
                cr.SetDatabaseLogon(global.username, global.pass, global.datasource, global.initialCatalog);

                cr.SetParameterValue("maincontactperson", contactPerson);
                cr.SetParameterValue("mainbankName", bnkName);
                cr.SetParameterValue("mainbranch", branchName);



                //==============param for listing
                cr.SetParameterValue("paramCompanyCode", txtCompanyCOde.Text);
                cr.SetParameterValue("paramBatch", txtTrans.Text);
                cr.SetParameterValue("paramAccntNo", txtAccountNo.Text);
                cr.SetParameterValue("paramCeiling", Convert.ToDecimal(dtCeling.Rows[0].ItemArray[0].ToString()).ToString("#,0.00"));
                cr.SetParameterValue("paramTotal", Convert.ToDecimal(dtAmount.Rows[0].ItemArray[0].ToString()).ToString("#,0.00"));


                cr.SetParameterValue("paramAccntSum", Convert.ToInt64(dtAccnt.Rows[0].ItemArray[0].ToString()));

                try
                {
                    string num = txtDepositAmount.Text;

                    string word;


                    if (txtDepositAmount.Text == "")
                    {
                        txtDepositAmount.Text = "";
                    }
                    else
                    {
                        number = decimal.Parse(num.ToString());

                        if (number.ToString() == "0")
                        {
                            MessageBox.Show("The number in currency fomat is \nZero Only");
                        }
                        else
                        {
                            word = Classes.clsSavingsDataEntry.ConvertToWords(number.ToString());
                        }
                    }


                    Console.ReadKey();
                }
                catch (System.Exception ex)
                {


                }


                string Msg = Classes.clsSavingsDataEntry.ConvertToWords(number.ToString()) + " (P " + txtDepositAmount.Text + ")";

                cr.SetParameterValue("mainmsg", Msg);
                cr.SetParameterValue("mainaccntNumber", txtAccountNo.Text);
                cr.SetParameterValue("mainpurpose", "loans/savings/refund");
                string dref = DateTime.Now.ToString("yyyy-MM-dd");
                cr.SetParameterValue("mainrefno", "Reference No: BPI" + dref.Replace("-", "") + txtTrans.Text);

                //Signature
                cr.SetParameterValue("mainsig1Name", sig1name.Text);
                cr.SetParameterValue("mainsig1pos", sig1pos.Text);
                cr.SetParameterValue("mainsig2name", sig2name.Text);
                cr.SetParameterValue("mainsig2pos", sig2pos.Text);
                cr.SetParameterValue("mainsig3name", sig3name.Text);
                cr.SetParameterValue("mainsig3pos", sig3pos.Text);


                rpt.crystalReportViewer1.ReportSource = cr;
                rpt.ShowDialog();
            }
        }
    }
}
