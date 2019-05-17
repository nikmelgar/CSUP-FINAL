using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApplication2
{
    class clsMembershipEntry
    {
        SqlConnection con;
        Global global = new Global();
        clsMembership clsMembership = new clsMembership();

        public static string principal { get; set; }
        public static string userID { get; set; }

        public void loadComboBox(ComboBox cmb, string tableName, string Display, string val)
        {
            cmb.DataSource = null;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Distinct(" + Display + "), " + val + " FROM  " + tableName + " WHERE isActive ='1'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmb.DisplayMember = Display;
                cmb.ValueMember = val;
                cmb.DataSource = dt;
            }
        }

        //Checking for Data Entry upon Saving and Updating
        public bool RequiredFields(TextBox LastName, TextBox FirstName,TextBox MiddleName , TextBox Address, ComboBox gender, ComboBox CivilStatus, MaskedTextBox TinNo, DateTimePicker DateOfBirth, TextBox PlacePMS, DateTimePicker DatePMS, TextBox EmployeeID, ComboBox Company, ComboBox PayrollGroup, ComboBox CostCenter, DateTimePicker DateHired, TextBox NameContactPerson, MaskedTextBox OfficeTel,ComboBox bank,TextBox atm)
        {
            TinNo.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            OfficeTel.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            if (clsMembershipEntry.principal.ToString() != "False" && clsMembershipEntry.principal.ToString() != "0")
            {
                if (LastName.Text == "" || FirstName.Text == "" || MiddleName.Text == "" || Address.Text == "" || gender.Text == "" || CivilStatus.Text == "" || TinNo.Text == "" || DateOfBirth.Text == "" || PlacePMS.Text == "" || DatePMS.Text == "" || EmployeeID.Text == "" || Company.Text == "" || PayrollGroup.Text == "" || CostCenter.Text == "" || DateHired.Text == "" || NameContactPerson.Text == "" ||  bank.Text == "" || atm.Text == "" || OfficeTel.Text == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (LastName.Text == "" || FirstName.Text == "" || MiddleName.Text == "" || Address.Text == "" || gender.Text == "" || CivilStatus.Text == "" || DateOfBirth.Text == "" || PlacePMS.Text == "" || DatePMS.Text == "" || EmployeeID.Text == "" || Company.Text == "" || PayrollGroup.Text == "" || CostCenter.Text == "" || DateHired.Text == "" || NameContactPerson.Text == "" || bank.Text == "" || atm.Text == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }  
        }

        //Checking for Cancellation of Data Entry if theres a value already
        public bool CheckValuesEntry(TextBox LastName, TextBox FirstName, TextBox Address, ComboBox CivilStatus, MaskedTextBox TinNo, DateTimePicker DateOfBirth, TextBox PlacePMS, DateTimePicker DatePMS, TextBox EmployeeID, ComboBox Company, ComboBox PayrollGroup, ComboBox CostCenter, DateTimePicker DateHired, TextBox NameContactPerson, MaskedTextBox ContactNo1)
        {
            TinNo.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            ContactNo1.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            if (LastName.Text != "" || FirstName.Text != "" || Address.Text != "" || CivilStatus.Text != "" || TinNo.Text != "" || PlacePMS.Text != "" || EmployeeID.Text != "" || Company.Text != "" || PayrollGroup.Text != "" || CostCenter.Text != "" || NameContactPerson.Text != "" || ContactNo1.Text != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //For Leaving without saving
        public bool CheckValuesForUpdating(TextBox LastName, TextBox FirstName, TextBox Address, ComboBox CivilStatus, MaskedTextBox TinNo, DateTimePicker DateOfBirth, TextBox PlacePMS, DateTimePicker DatePMS, TextBox EmployeeID, ComboBox Company, ComboBox PayrollGroup, ComboBox CostCenter, DateTimePicker DateHired, TextBox NameContactPerson, MaskedTextBox ContactNo1)
        {
            //TRUE = Not Change
            //FALSE = Change values
            TinNo.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            ContactNo1.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("select LastName,FirstName,Residential_Address,Civil_Status,TinNo,Date_Of_Birth,Place_PMS,Date_Of_PMS,EmployeeID,Company_Code,Payroll_Code,Cost_Center_Code,Date_Hired, Contact_Person, Contact_No1 from Membership WHERE userid = '" + clsMembershipEntry.userID + "'", con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if (LastName.Text == ds.Tables[0].Rows[0]["LastName"].ToString() && FirstName.Text == ds.Tables[0].Rows[0]["FirstName"].ToString() && Address.Text == ds.Tables[0].Rows[0]["Residential_Address"].ToString() && CivilStatus.Text == ds.Tables[0].Rows[0]["Civil_Status"].ToString() && TinNo.Text == ds.Tables[0].Rows[0]["TinNo"].ToString() && DateOfBirth.Text == Convert.ToDateTime(ds.Tables[0].Rows[0]["Date_Of_Birth"].ToString()).ToShortDateString() && PlacePMS.Text == ds.Tables[0].Rows[0]["Place_PMS"].ToString() && DatePMS.Text == Convert.ToDateTime(ds.Tables[0].Rows[0]["Date_Of_PMS"].ToString()).ToShortDateString() && EmployeeID.Text == ds.Tables[0].Rows[0]["EmployeeID"].ToString() && Company.Text == clsMembership.returnCompanyDescription(ds.Tables[0].Rows[0]["Company_Code"].ToString()) && PayrollGroup.Text == clsMembership.returnPayrollDescription(ds.Tables[0].Rows[0]["Payroll_Code"].ToString()) && CostCenter.Text == clsMembership.returnCostCenter(ds.Tables[0].Rows[0]["Cost_Center_Code"].ToString()) && DateHired.Text == Convert.ToDateTime(ds.Tables[0].Rows[0]["Date_Hired"].ToString()).ToShortDateString() && NameContactPerson.Text == ds.Tables[0].Rows[0]["Contact_Person"].ToString() && ContactNo1.Text == ds.Tables[0].Rows[0]["Contact_No1"].ToString())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //Checking for Duplicate EmployeeID
        public bool CheckDuplicateEmployeeID(String tableName,String stringToCompare)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();


                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM " + tableName + " Where EmployeeID ='" + stringToCompare + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        //Checking for Duplicate EmployeeID For Update 
        public bool CheckDuplicateEmployeeIDUpdate(String tableName, String stringToCompare,string userid,string EmployeeID)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM " + tableName + " Where EmployeeID ='" + stringToCompare + "' and userID <> '" + userid + "' and PrincipalID <> '" + EmployeeID + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

                
        }

        //Get Dependet
        public bool checkDependent(string EmployeeID)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE Principal = 0 and EmployeeID = '" + EmployeeID + "'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

             
        }

        public void RequiredFieldColor(TextBox lastname,TextBox firstname ,TextBox middleName, TextBox address,MaskedTextBox tin,TextBox atm,TextBox email, TextBox pms,TextBox empid)
        {
            tin.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            if (lastname.Text == "")
            {
                lastname.BackColor = Color.FromArgb(128, 255, 128);
            }
            else
            {
                lastname.BackColor = SystemColors.Window;
            }

            if(firstname.Text == "")
            {
                firstname.BackColor = Color.FromArgb(128, 255, 128);
            }
            else
            {
                firstname.BackColor = SystemColors.Window;
            }

            if (middleName.Text == "")
            {
                middleName.BackColor = Color.FromArgb(128, 255, 128);
            }
            else
            {
                middleName.BackColor = SystemColors.Window;
            }

            if (address.Text == "")
            {
                address.BackColor = Color.FromArgb(128, 255, 128);
            }
            else
            {
                address.BackColor = SystemColors.Window;
            }

            if(principal == "True" || principal == "1")
            {
                if (tin.Text == "")
                {
                    tin.BackColor = Color.FromArgb(128, 255, 128);
                }
                else
                {

                }
                {
                    tin.BackColor = SystemColors.Window;
                }
            }
            else
            {
                tin.BackColor = SystemColors.Window;
            }
            
            if (atm.Text == "")
            {
                atm.BackColor = Color.FromArgb(128, 255, 128);
            }
            else
            {
                atm.BackColor = SystemColors.Window;
            }

            if (email.Text == "")
            {
                email.BackColor = Color.FromArgb(128, 255, 128);
            }
            else
            {
                email.BackColor = SystemColors.Window;
            }

            if (pms.Text == "")
            {
                pms.BackColor = Color.FromArgb(128, 255, 128);
            }
            else
            {
                pms.BackColor = SystemColors.Window;
            }

            if (empid.Text == "")
            {
                empid.BackColor = Color.FromArgb(128, 255, 128);
            }
            else
            {
                empid.BackColor = SystemColors.Window;
            }

        }
    }
}
