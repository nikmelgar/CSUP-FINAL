using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace WindowsFormsApplication2
{
    class clsMembership
    {
        SqlConnection con;
        SqlDataAdapter adapter;
        Global global = new Global();

        public void loadMembership(DataGridView dgv)
        {
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE isActive = '1' and isApprove = '1'", con);
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

            dgv.Columns["LastName"].Visible = true;
            dgv.Columns["LastName"].HeaderText = "Last Name";

            dgv.Columns["FirstName"].Visible = true;
            dgv.Columns["FirstName"].HeaderText = "First Name";

            dgv.Columns["MiddleName"].Visible = true;
            dgv.Columns["MiddleName"].HeaderText = "Middle Name";

            dgv.Columns["Date_Of_Birth"].Visible = true;
            dgv.Columns["Date_Of_Birth"].HeaderText = "Birthday";
        }

        public string returnBankName(String BankCode)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT Bank_Name FROM Bank WHERE Bank_Code = '"+ BankCode +"'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray[0].ToString();
            }
            else
            {
                return "";
            }
        }

        public string returnCompanyDescription(String CompanyCode)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT Description FROM Company WHERE Company_Code = '"+ CompanyCode +"'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray[0].ToString();
            }
            else
            {
                return "";
            }
        }

        public string returnPayrollDescription(String PayrollCode)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT Description FROM Payroll_Group WHERE Payroll_Code = '"+ PayrollCode +"'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray[0].ToString();
            }
            else
            {
                return "";
            }
        }

        public string returnCostCenter(String CostCenterCode)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT Description FROM Cost_Center WHERE Cost_Center_Code ='"+ CostCenterCode + "'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray[0].ToString();
            }
            else
            {
                //Check from company 
                //NOt equal to PLDT
                SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT Description FROM Company WHERE Company_Code = '" + CostCenterCode + "'", con);
                DataTable dt1 = new DataTable();
                adapter1.Fill(dt1);

                if(dt1.Rows.Count > 0)
                {
                    return dt1.Rows[0].ItemArray[0].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public string returnCompanyNonPayroll(int userid)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT Company_Code FROM Membership WHERE userID = '" + userid +"'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt.Rows[0].ItemArray[0].ToString();

        }

        public void loadBeneficiaries(DataGridView dgv,string EmployeeID)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT Full_Name,Relationship,Date_Of_Birth FROM Beneficiaries WHERE EmployeeID = '"+ EmployeeID +"'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count >= 1)
            {
                dgv.Rows.Add(dt.Rows.Count);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dgv.Rows[i].Cells[0].Value = dt.Rows[i].ItemArray[0].ToString();
                    dgv.Rows[i].Cells[1].Value = dt.Rows[i].ItemArray[1].ToString();
                    dgv.Rows[i].Cells[2].Value = Convert.ToDateTime(dt.Rows[i].ItemArray[2].ToString()).ToShortDateString();
                }
            }
        }

        public void loadPicture(string UserID,PictureBox pic)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(new SqlCommand("SELECT Member_Picture FROM Membership WHERE userID = '" + UserID  + "'", con));
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            if (dataSet.Tables[0].Rows.Count == 1)
            {
                Byte[] data = new Byte[0];
                data = (Byte[])(dataSet.Tables[0].Rows[0]["Member_Picture"]);
                MemoryStream mem = new MemoryStream(data);
                pic.Image = Image.FromStream(mem);
            }
        }

        public void SearchMembers(DataGridView dgv, string EmployeeID,string lastName,string firstName,CheckBox chckPrincipal,CheckBox chckDependent)
        {
            con = new SqlConnection();
            global.connection(con);

            if(chckPrincipal.Checked == true && chckDependent.Checked == false)
            {
                //Search Principal Only
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE isActive = '1' and isApprove = '1' and EmployeeID like '%"+ EmployeeID + "%' and LastName like '%"+ lastName +"%' and FirstName like '%"+ firstName +"%' and Principal ='1'" , con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    dgv.DataSource = dt;
                }
                else
                {
                    Alert.show("No Records found on Principal", Alert.AlertType.warning);
                    return;
                }

            }
            else if(chckDependent.Checked == true && chckPrincipal.Checked == false)
            {
                //Search Principal Only
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE isActive = '1' and isApprove = '1' and EmployeeID like '%" + EmployeeID + "%' and LastName like '%" + lastName + "%' and FirstName like '%" + firstName + "%' and Principal ='0'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgv.DataSource = dt;
                }
                else
                {
                    Alert.show("No Records found on Dependent", Alert.AlertType.warning);
                    return;
                }
            }
            else if (chckDependent.Checked == true && chckPrincipal.Checked == true)
            {
                //Search
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE isActive = '1' and isApprove = '1' and EmployeeID like '%" + EmployeeID + "%' and LastName like '%" + lastName + "%' and FirstName like '%" + firstName + "%'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgv.DataSource = dt;
                }
                else
                {
                    Alert.show("No Records found on Dependent", Alert.AlertType.warning);
                    return;
                }
            }
            else if(chckDependent.Checked == false && chckPrincipal.Checked == false)
            {
                //Search
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE isActive = '1' and isApprove = '1' and EmployeeID like '%" + EmployeeID + "%' and LastName like '%" + lastName + "%' and FirstName like '%" + firstName + "%'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgv.DataSource = dt;
                }
                else
                {
                    Alert.show("No Records found on Dependent", Alert.AlertType.warning);
                    return;
                }
            }


        }

        public string countPendingMembers(Label lblPending)
        {
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT count(*) FROM Membership WHERE IsApprove is null",con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows[0].ItemArray[0].ToString() == "0")
            {
                return lblPending.Text = "0";
            }
            else
            {
                return lblPending.Text = dt.Rows[0].ItemArray[0].ToString();
            }
        }

        public string countActiveMembers(Label lblActive)
        {
            //Change From Active Members to Non-Payroll Members
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT count(*) FROM Membership WHERE IsApprove ='1' and IsActive = '1' and Company_Code = 'COMP010'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows[0].ItemArray[0].ToString() == "0")
            {
                return lblActive.Text = "0";
            }
            else
            {
                return lblActive.Text = dt.Rows[0].ItemArray[0].ToString();
            }
        }

        public string countTotalMembers(Label lblTotalMember)
        {
            //Change From All Members w/ Pending to Active Members Only (Payroll)
            con = new SqlConnection();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT count(*) FROM Membership WHERE IsApprove ='1' and IsActive = '1' and Company_Code <> 'COMP010'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows[0].ItemArray[0].ToString() == "0")
            {
                return lblTotalMember.Text = "0";
            }
            else
            {
                return lblTotalMember.Text = dt.Rows[0].ItemArray[0].ToString();
            }
        }
    }
}
