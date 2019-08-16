using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsReminderSavings
    {
        Global global = new Global();
        clsSavings clsSavings = new clsSavings();
        clsSavingsDataEntry clsSavingsDataEntry = new clsSavingsDataEntry();
        public void loadSavingsBillPerDate(DataGridView dgv, DateTimePicker dtbill)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM SavingsDeductionBillingCollection WHERE deductiondate = '"+ dtbill.Text +"' and isDone = 0", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgv.DataSource = dt;


                int colCnt = dt.Columns.Count;
                int x = 0;


                while (x != colCnt)
                {
                    dgv.Columns[x].Visible = false;
                    x = x + 1;
                }

                dgv.Columns["EmployeeID"].Visible = true;
                dgv.Columns["EmployeeID"].HeaderText = "Employee ID";

                dgv.Columns["loan_no"].Visible = true;
                dgv.Columns["loan_no"].HeaderText = "Loan No";

                dgv.Columns["loan_type"].Visible = true;
                dgv.Columns["loan_type"].HeaderText = "Loan Type";

                dgv.Columns["deduction_code"].Visible = true;
                dgv.Columns["deduction_code"].HeaderText = "Deduction Code";

                dgv.Columns["dueamount"].Visible = true;
                dgv.Columns["dueamount"].HeaderText = "Due Amount";

                dgv.Columns["appliedamount"].Visible = true;
                dgv.Columns["appliedamount"].HeaderText = "Applied Amount";

                dgv.Columns["deferredamount"].Visible = true;
                dgv.Columns["deferredamount"].HeaderText = "Deferred Amount";

            }
        }
        public bool checkIfHasLoanSavingsDue(int DayFrom,int DayTo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanBalancesSavingsDeduction";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DateEnter", DateTime.Today.ToShortDateString());
                cmd.Parameters.AddWithValue("@DayFrom", DayFrom);
                cmd.Parameters.AddWithValue("@DayTo", DayTo);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
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

        public void generateLoanSavingsPayment(DataGridView dgv, DateTimePicker dt, int dayFrom, int dayTo)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_ReturnLoanBalancesSavingsDeduction";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DateEnter", dt.Text);
                cmd.Parameters.AddWithValue("@DayFrom", dayFrom);
                cmd.Parameters.AddWithValue("@DayTo", dayTo);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if(ds.Tables[0].Rows.Count > 0)
                {
                    dgv.Rows.Clear();
                    dgv.Rows.Add(ds.Tables[0].Rows.Count);

                    for(int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows[x].Cells[0].Value = ds.Tables[0].Rows[x]["userID"].ToString();
                        dgv.Rows[x].Cells[1].Value = ds.Tables[0].Rows[x]["EmployeeID"].ToString();
                        dgv.Rows[x].Cells[2].Value = ds.Tables[0].Rows[x]["Loan_No"].ToString();
                        dgv.Rows[x].Cells[3].Value = ds.Tables[0].Rows[x]["Loan_Type"].ToString();
                        dgv.Rows[x].Cells[4].Value = Convert.ToDateTime(ds.Tables[0].Rows[x]["ReleaseDate"].ToString()).ToShortDateString();
                        dgv.Rows[x].Cells[5].Value = ds.Tables[0].Rows[x]["Loan_Amount"].ToString();
                        dgv.Rows[x].Cells[6].Value = ds.Tables[0].Rows[x]["Monthly_Amort"].ToString();
                        dgv.Rows[x].Cells[7].Value = ds.Tables[0].Rows[x]["Deferred"].ToString();
                        dgv.Rows[x].Cells[8].Value = ds.Tables[0].Rows[x]["Balance"].ToString();
                    }
                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    dgv.Rows.Clear();
                    return;
                }
            }
        }

        public bool alreadyRun(string loan_no, DateTimePicker dt)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM SavingsDeductionBillingCollection WHERE Loan_No = '" + loan_no + "' and deductiondate ='" + dt.Text + "' and isDone = 0", con);
                DataTable dtable = new DataTable();
                adapter.Fill(dtable);

                if(dtable.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }


        public string returnSavingsLessTheChequeClearing(string userid)
        {
            //MINUS THE CHECK AMOUNT
            if (clsSavings.returnDepositedChequeAmount(Convert.ToInt32(userid)) != "")
            {
                Decimal widAmnt;
                widAmnt = Convert.ToDecimal(clsSavingsDataEntry.returnMembersSaving(userid)) - Convert.ToDecimal(clsSavings.returnDepositedChequeAmount(Convert.ToInt32(userid)));
                return widAmnt.ToString("#,0.00");
            }
            else
            {
                return Convert.ToDecimal(clsSavingsDataEntry.returnMembersSaving(userid)).ToString("#,0.00");
            }
        }

        public decimal GetTotalDeduction(string userid, DateTimePicker dtBillDate)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT SUM(AppliedAmount) FROM SavingsDeductionBillingCollection WHERE userid = '"+ userid +"' and deductiondate = '"+ dtBillDate.Text +"' and (isDone = 0 or isDone is NULL)",con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());
            }
        }

    }
}
