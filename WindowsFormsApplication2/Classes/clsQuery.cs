using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApplication2.Classes
{
    class clsQuery
    {

        public static int searchUserID { get; set; }

        Global global = new Global();

        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        public void loadSearchMember(DataGridView dgv,TextBox LastName,TextBox FirstName,TextBox EmployeeID)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();


                if (LastName.Text == "" && FirstName.Text == "" && EmployeeID.Text == "")
                {
                    //No Search Criteria 
                    Alert.show("No Keywords to be search.", Alert.AlertType.error);
                    return;
                }
                else
                {
                    string str;
                    str = "SELECT userid,EmployeeID,Principal,LastName,FirstName,MiddleName,Suffix,Date_Of_Birth FROM Membership WHERE EmployeeID like '%" + EmployeeID.Text + "%' and LastName like '%" + LastName.Text + "%' and FirstName like '%" + FirstName.Text + "%'";

                    adapter = new SqlDataAdapter(str, con);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        //Theres a record found
                        dgv.DataSource = dt;

                        dgv.Columns["userID"].Visible = false;
                    }
                    else
                    {
                        //No Records
                        Alert.show("No Records Found.", Alert.AlertType.error);
                        return;
                    }
                }
            }

        }

        //LOAD SAVINGS 
        public void loadSavingsByUserID(int userid,DataGridView dgv)
        {
            if(userid.ToString() == "" || userid.ToString() == "0")
            {
                return;
            }

            using(SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SavingsFixedComp_sp";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RefID", userid);
                cmd.Parameters.AddWithValue("@Switch", "0");

                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                dgv.Rows.Clear();

                string mos, yr;

                if (dt.Rows.Count > 0)
                {
                    //SET FIRST
                    mos = dt.Rows[0].ItemArray[6].ToString();
                    yr = dt.Rows[0].ItemArray[7].ToString();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        if (dt.Rows[i].ItemArray[1].ToString() != "")
                        {
                            if (dt.Rows[i].ItemArray[6].ToString() == mos && dt.Rows[i].ItemArray[7].ToString() == yr)
                            {
                                //Mons and Year = SAME 
                                dgv.Rows.Add(Convert.ToDateTime(dt.Rows[i].ItemArray[0].ToString()).ToShortDateString(), dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[9].ToString(), Convert.ToDecimal(dt.Rows[i].ItemArray[2].ToString()).ToString("#,0.00"), Convert.ToDecimal(dt.Rows[i].ItemArray[3].ToString()).ToString("#,0.00"), Convert.ToDecimal(dt.Rows[i].ItemArray[4].ToString()).ToString("#,0.00"), Convert.ToDecimal(dt.Rows[i].ItemArray[5].ToString()).ToString("#,0.00"));
                            }
                            else
                            {
                                //ADD NEW ROW AND REPAINT
                                DataGridViewRow row = new DataGridViewRow();
                                DataGridViewCellStyle style = new DataGridViewCellStyle();
                                style.BackColor = Color.SeaGreen; // the color change
                                row.DefaultCellStyle = style;

                                dgv.Rows.Add(row);
                                dgv.Rows.Add(Convert.ToDateTime(dt.Rows[i].ItemArray[0].ToString()).ToShortDateString(), dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[9].ToString(), Convert.ToDecimal(dt.Rows[i].ItemArray[2].ToString()).ToString("#,0.00"), Convert.ToDecimal(dt.Rows[i].ItemArray[3].ToString()).ToString("#,0.00"), Convert.ToDecimal(dt.Rows[i].ItemArray[4].ToString()).ToString("#,0.00"), Convert.ToDecimal(dt.Rows[i].ItemArray[5].ToString()).ToString("#,0.00"));
                            }
                            //Put the Month and Year in String
                            mos = dt.Rows[i].ItemArray[6].ToString();
                            yr = dt.Rows[i].ItemArray[7].ToString();
                        }
                    }
                }
            }
        }


        //LOAD SHARE CAPITAL 
        public void loadShareCapitalByUserID(int userid, DataGridView dgv)
        {
            if (userid.ToString() == "" || userid.ToString() == "0")
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SavingsFixedComp_sp";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RefID", userid);
                cmd.Parameters.AddWithValue("@Switch", "1");

                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                dgv.Rows.Clear();

                string mos, yr;

                if (dt.Rows.Count > 0)
                {
                    //SET FIRST
                    mos = dt.Rows[0].ItemArray[6].ToString();
                    yr = dt.Rows[0].ItemArray[7].ToString();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        if (dt.Rows[i].ItemArray[1].ToString() != "")
                        {
                            if (dt.Rows[i].ItemArray[6].ToString() == mos && dt.Rows[i].ItemArray[7].ToString() == yr)
                            {
                                //Mons and Year = SAME 
                                dgv.Rows.Add(Convert.ToDateTime(dt.Rows[i].ItemArray[0].ToString()).ToShortDateString(), dt.Rows[i].ItemArray[1].ToString(), Convert.ToDecimal(dt.Rows[i].ItemArray[2].ToString()).ToString("#,0.00"), Convert.ToDecimal(dt.Rows[i].ItemArray[3].ToString()).ToString("#,0.00"), Convert.ToDecimal(dt.Rows[i].ItemArray[4].ToString()).ToString("#,0.00"));
                            }
                            else
                            {
                                //ADD NEW ROW AND REPAINT
                                DataGridViewRow row = new DataGridViewRow();
                                DataGridViewCellStyle style = new DataGridViewCellStyle();
                                style.BackColor = Color.SeaGreen; // the color change
                                row.DefaultCellStyle = style;

                                dgv.Rows.Add(row);
                                dgv.Rows.Add(Convert.ToDateTime(dt.Rows[i].ItemArray[0].ToString()).ToShortDateString(), dt.Rows[i].ItemArray[1].ToString(), Convert.ToDecimal(dt.Rows[i].ItemArray[2].ToString()).ToString("#,0.00"), Convert.ToDecimal(dt.Rows[i].ItemArray[3].ToString()).ToString("#,0.00"), Convert.ToDecimal(dt.Rows[i].ItemArray[4].ToString()).ToString("#,0.00"));
                            }
                            //Put the Month and Year in String
                            mos = dt.Rows[i].ItemArray[6].ToString();
                            yr = dt.Rows[i].ItemArray[7].ToString();
                        }
                    }
                }
            }
        }
        /*
        * ---------------------------------------------------------------------------------------------------------
        *           LOAN BALANCES DISPLAY  
        * ---------------------------------------------------------------------------------------------------------
        */

        public void loadLoanBalances(int userid,DataGridView dgv)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                if(userid.ToString() == "" || userid.ToString() == "0")
                {
                    return;
                }
                else
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_ReturnLoanBalancesQuery";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", userid);

                    adapter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    dgv.Rows.Clear();

                    if(ds.Tables[0].Rows.Count > 0)
                    {
                        //Fill in the datagridview
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            dgv.Rows.Add(ds.Tables[0].Rows[i]["Loan_Description"].ToString(), ds.Tables[0].Rows[i]["Loan_No"].ToString(), Convert.ToDecimal(ds.Tables[0].Rows[i]["Loan_Amount"].ToString()).ToString("#,0.00"),Convert.ToDateTime(ds.Tables[0].Rows[i]["ReleaseDate"].ToString()).ToShortDateString(), ds.Tables[0].Rows[i]["Terms"].ToString(), Convert.ToDecimal(ds.Tables[0].Rows[i]["Monthly_Amort"].ToString()).ToString("#,0.00"), Convert.ToDecimal(ds.Tables[0].Rows[i]["Balance"].ToString()).ToString("#,0.00"), Convert.ToDecimal(ds.Tables[0].Rows[i]["Deferred"].ToString()).ToString("#,0.00"));
                        }
                    }

                    //Load Footer
                    loadTotals(dgv);
                }
            }
        }


        public void loadTotals(DataGridView dgv)
        {
            dgv.Rows.Add(1);

            decimal sumMonthlyAmort = 0;
            decimal sumBalance = 0;
            decimal sumDeferred = 0;
            //Check if theres a beneficiary
            if (dgv.Rows.Count > 0)
            {
                for (int i = 0; i < dgv.Rows.Count; ++i)
                {
                    sumMonthlyAmort += Convert.ToDecimal(dgv.Rows[i].Cells["Monthly_Amort"].Value);
                    sumBalance += Convert.ToDecimal(dgv.Rows[i].Cells["bal"].Value);
                    sumDeferred += Convert.ToDecimal(dgv.Rows[i].Cells["Deferred"].Value);
                }
            }

            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[4].Value = "Totals";
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Monthly_Amort"].Value = sumMonthlyAmort.ToString("#,0.00");
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["bal"].Value = sumBalance.ToString("#,0.00");
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Deferred"].Value = sumDeferred.ToString("#,0.00");

            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells[4].Style.Font = new System.Drawing.Font("Tahoma", 12);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Monthly_Amort"].Style.Font = new System.Drawing.Font("Tahoma", 12);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["bal"].Style.Font = new System.Drawing.Font("Tahoma", 12);
            dgv.Rows[Convert.ToInt32(dgv.Rows.Count - 1)].Cells["Deferred"].Style.Font = new System.Drawing.Font("Tahoma", 12);

        }

        //RETURN Members Name and ID
        public string returMembersNameAndID(int userid)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                if (userid.ToString() == "" || userid.ToString() == "0")
                {
                    return "";
                }
                else
                {
                    string str;
                    str = "select EmployeeID +' - ' + CASE WHEN Suffix is not null THEN LastName +', '+ FirstName + SPACE(1) + MiddleName + SPACE(1) + Suffix ELSE LastName +', '+ FirstName + SPACE(1) + MiddleName END FROM Membership WHERE UserID = '" + userid + "'";
                    adapter = new SqlDataAdapter(str, con);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    return "Member: " + dt.Rows[0].ItemArray[0].ToString();
                }
            }
        }

        public string returnCompanyDescription(string CompanyCode)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Description FROM Company WHERE Company_Code = '"+ CompanyCode +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }

        public string returnPayrollDescription(string PayrollCode)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Description FROM Payroll_Group WHERE Payroll_Code = '" + PayrollCode + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }
    }
}
