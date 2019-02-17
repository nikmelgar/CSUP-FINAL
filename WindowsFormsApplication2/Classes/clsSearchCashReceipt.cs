using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsSearchCashReceipt
    {
        SqlCommand cmd;
        SqlConnection con;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();
        

        public string returnMembersName(String userid)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT LastName+', '+ FirstName + SPACE(1) + MiddleName + SPACE(1) + Suffix as Name From Membership WHERE userID = '" + userid + "'", con);
            dt = new DataTable();

            adapter.Fill(dt);

            return dt.Rows[0].ItemArray[0].ToString();
        }

        public string returnClientName(String ClientID)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Name FROM Client WHERE Client_Code ='"+ ClientID +"'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            return dt.Rows[0].ItemArray[0].ToString();
        }

        public string GetCompanyPerMember(String userid)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Company_Code FROM Membership WHERE userID = '"+ userid +"'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            return dt.Rows[0].ItemArray[0].ToString();
        }
        public void loadDefaultCashReceipts(DataGridView dgv)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT top 20 * FROM Cash_Receipts_Header Order by Or_Date DESC", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                dgv.DataSource = dt;

                int colCnt = dt.Columns.Count;
                int x = 0;


                while (x != colCnt)
                {
                    dgv.Columns[x].Visible = false;
                    x = x + 1;
                }

                dgv.Columns["Or_No"].Visible = true;
                dgv.Columns["Or_No"].HeaderText = "OR No";

                dgv.Columns["Or_Date"].Visible = true;
                dgv.Columns["Or_Date"].HeaderText = "Date";

                dgv.Columns["Particulars"].Visible = true;
                dgv.Columns["Particulars"].HeaderText = "Particulars";
            }
        }

        public void searchOR(DataGridView dgv, TextBox or_no, TextBox particulars,Label lblError)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT * FROM Cash_Receipts_Header WHERE Or_No like '%" + or_no.Text + "%' or Particulars like '%" + particulars.Text + "%'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                dgv.DataSource = dt;

                int colCnt = dt.Columns.Count;
                int x = 0;


                while (x != colCnt)
                {
                    dgv.Columns[x].Visible = false;
                    x = x + 1;
                }

                dgv.Columns["Or_No"].Visible = true;
                dgv.Columns["Or_No"].HeaderText = "OR No";

                dgv.Columns["Or_Date"].Visible = true;
                dgv.Columns["Or_Date"].HeaderText = "Date";

                dgv.Columns["Particulars"].Visible = true;
                dgv.Columns["Particulars"].HeaderText = "Particulars";

                lblError.Visible = false;
            }
            else
            {
                lblError.Visible = true;
                or_no.Focus();
            }
        }

        public void loadTransaction(DataGridView dgv, string or_no)
        {
            dgv.Rows.Clear();
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * from Cash_Receipts_Trans WHERE or_no = '" + or_no + "'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dgv.Rows.Add(dt.Rows.Count);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgv.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[1].ToString();
                if (dt.Rows[i].ItemArray[2].ToString() == "" || DBNull.Value.Equals(dt.Rows[i].ItemArray[2].ToString()))
                {
                    dgv.Rows[i].Cells[1].Value = DBNull.Value;
                }
                else
                {
                    dgv.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[2].ToString();
                }
                dgv.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[3].ToString();
            }


        }

        public void loadBanksCheck(DataGridView dgv, string or_no)
        {
            dgv.Rows.Clear();
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT * FROM Cash_Receipts_Checks WHERE Or_No ='"+ or_no +"'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            dgv.Rows.Add(dt.Rows.Count);

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                dgv.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[1].ToString();                                              //Bank
                dgv.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[2].ToString();                                              //Amount
                dgv.Rows[i].Cells[2].Value = Convert.ToDateTime(dt.Rows[i].ItemArray[3].ToString()).ToShortDateString();      //Check Date
                dgv.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[4].ToString();                                              //Check No.
            }
        }

        public void loadCashReceiptsDetails(DataGridView dgv,string or_no)
        {
            dgv.Rows.Clear();
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("select Account_Code,Subsidiary_Code,Loan_No,debit,credit,userID from Cash_Receipts_Detail where Or_No = '" + or_no + "'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dgv.Rows.Add(dt.Rows.Count);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dgv.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString();
                dgv.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString();
                dgv.Rows[i].Cells[2].Value = dt.Rows[i].ItemArray[2].ToString();
                dgv.Rows[i].Cells[3].Value = dt.Rows[i].ItemArray[3].ToString();
                dgv.Rows[i].Cells[4].Value = dt.Rows[i].ItemArray[4].ToString();
                if (dt.Rows[i].ItemArray[5].ToString() == "" || DBNull.Value.Equals(dt.Rows[i].ItemArray[5].ToString()))
                {
                    dgv.Rows[i].Cells[5].Value = "0";
                }
                else
                {
                    dgv.Rows[i].Cells[5].Value = dt.Rows[i].ItemArray[5].ToString();
                }

            }
        }

        public bool checkIfCancelled(string or_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Cancelled FROM Cash_Receipts_Header WHERE Or_No = '" + or_no + "'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            if (DBNull.Value.Equals(dt.Rows[0].ItemArray[0].ToString()) || dt.Rows[0].ItemArray[0].ToString() == "")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public bool checkIfPosted(string or_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Posted FROM Cash_Receipts_Header WHERE Or_No = '" + or_no + "'", con);
            dt = new DataTable();
            adapter.Fill(dt);

            if (DBNull.Value.Equals(dt.Rows[0].ItemArray[0].ToString()) || dt.Rows[0].ItemArray[0].ToString() == "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string getPostedBy(string or_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Posted_By FROM Cash_Receipts_Header WHERE Or_No = '" + or_no + "'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt.Rows[0].ItemArray[0].ToString();
        }

        public string getCancelled(string or_no)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT Cancelled_By FROM Cash_Receipts_Header WHERE Or_No = '" + or_no + "'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt.Rows[0].ItemArray[0].ToString();
        }

    }
}
