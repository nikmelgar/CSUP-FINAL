using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System.Collections;

namespace WindowsFormsApplication2.Classes
{
    class clsATMDiskAdvice
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();

        public void loadATMDiskAdvice(DataGridView dgv)
        {
            dgv.Rows.Clear();

            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("select distinct(bank_code) from ATM WHERE Deposited is null Group by Bank_Code,Purpose", con);
            dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count >= 1)
            {
                dgv.Rows.Add(dt.Rows.Count);
                decimal ttlPerBankRight = 0;
                decimal ttalLoans = 0;
                decimal ttalSDPrBank = 0;
                for (int i = 0; i < dt.Rows.Count; i++) //Loop for Banks
                {
                    dgv.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString();

                    //Put the SD or Loan Values
                    SqlDataAdapter adapter2 = new SqlDataAdapter("select distinct purpose,sum(amount) from atm where Bank_Code ='" + dgv.Rows[i].Cells[0].Value.ToString() + "' and Deposited is null group by Purpose", con);
                    DataTable dt2 = new DataTable();
                    adapter2.Fill(dt2);

                    for (int x = 0; x < dt2.Rows.Count; x++) //For Loans and SD
                    {
                        if (dt2.Rows[x].ItemArray[0].ToString() == "SD")
                        {
                            //SAVINGS
                            dgv.Rows[i].Cells[1].Value = Convert.ToDecimal(dt2.Rows[x].ItemArray[1].ToString()).ToString("#,0.00");
                            ttalSDPrBank = Convert.ToDecimal(dgv.Rows[i].Cells[1].Value);
                        }
                        else
                        {
                            //LOANS
                            ttalLoans += Convert.ToDecimal(dt2.Rows[x].ItemArray[1].ToString());
                        }

                        //Total Amount Per Banks[Savings/Loans]
                        ttlPerBankRight = ttalSDPrBank + ttalLoans;

                        dgv.Rows[i].Cells[3].Value = ttlPerBankRight.ToString("#,0.00");
                        dgv.Rows[i].Cells[2].Value = ttalLoans.ToString("#,0.00");
                    }
                }

                loadTotals(dgv);
            }
        }
        //Additional Row
        public void loadTotals(DataGridView dgv)
        {
            dgv.Rows.Add(1);

            decimal sumSD = 0;
            decimal sumLI = 0;
            decimal sumTT = 0;
            //Check if theres a beneficiary
            if (dgv.Rows.Count > 0)
            {

                for (int i = 0; i < dgv.Rows.Count; ++i)
                {
                    sumSD += Convert.ToDecimal(dgv.Rows[i].Cells[1].Value);
                    sumLI += Convert.ToDecimal(dgv.Rows[i].Cells[2].Value);
                    sumTT += Convert.ToDecimal(dgv.Rows[i].Cells[3].Value);
                }
            }

            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[0].Value = "Total";
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[1].Value = sumSD.ToString("#,0.00");
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[2].Value = sumLI.ToString("#,0.00");
            sumTT = sumSD + sumLI;
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[3].Value = sumTT.ToString("#,0.00");


        }

        public void loadBankCode(ComboBox cmb)
        {
            SqlConnection con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("select distinct(bank_code) from ATM WHERE Deposited is null Group by Bank_Code,Purpose", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            cmb.DisplayMember = "Bank_Code";
            cmb.ValueMember = "Bank_Code";
            cmb.DataSource = dt;
        }

        public void displayBrandAndCompanyCode(TextBox brnch,TextBox company,TextBox accountNo,string bankcode)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT * FROM Bank WHERE Bank_Code ='"+ bankcode +"'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            brnch.Text = dt.Rows[0].ItemArray[6].ToString(); //Branch Code
            company.Text = dt.Rows[0].ItemArray[5].ToString(); //Company Code
            accountNo.Text = dt.Rows[0].ItemArray[3].ToString(); //Account number
        }



    }
}
