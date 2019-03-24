using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsQueryMember
    {

        Global global = new Global();
        clsQuery clsQuery = new clsQuery();

        public static string EmployeeID { get; set; }
        public static string LastName { get; set; }
        public static string FirstName { get; set; }
        public static string MiddleName { get; set; }
        public static string suffix { get; set; }
        public static string Address { get; set; }
        public static string Telephone { get; set; }
        public static string sex { get; set; }
        public static string CivilStatus { get; set; }
        public static string Birthday { get; set; }
        public static string NameOfSpouse { get; set; }
        public static string BirthPlace { get; set; }
        public static string DateOfMembership { get; set; }
        public static string Company { get; set; }
        public static string DateOfApproval { get; set; }
        public static string DateOfResigned { get; set; }
        public static string PayrollGroup { get; set; }
        public static string bank { get; set; }
        public static string AccountNo { get; set; }
        public static string Savings { get; set; }
        public static string ShareCapital { get; set; }

        public void loadMembersProfile(int userid)
        {
            if (userid.ToString() == "" || userid.ToString() == "0")
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Membership WHERE userID = '"+ userid +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                EmployeeID = dt.Rows[0].ItemArray[1].ToString();
                LastName = dt.Rows[0].ItemArray[4].ToString();
                FirstName = dt.Rows[0].ItemArray[5].ToString();
                MiddleName = dt.Rows[0].ItemArray[6].ToString();
                suffix = dt.Rows[0].ItemArray[7].ToString();
                Address = dt.Rows[0].ItemArray[9].ToString();
                Telephone = dt.Rows[0].ItemArray[16].ToString();
                sex = dt.Rows[0].ItemArray[12].ToString();
                CivilStatus = dt.Rows[0].ItemArray[13].ToString();
                Birthday = Convert.ToDateTime(dt.Rows[0].ItemArray[10].ToString()).ToShortDateString();
                BirthPlace = dt.Rows[0].ItemArray[11].ToString();
                NameOfSpouse = dt.Rows[0].ItemArray[14].ToString();
                DateOfMembership = Convert.ToDateTime(dt.Rows[0].ItemArray[39].ToString()).ToShortDateString();
                if(dt.Rows[0].ItemArray[30].ToString() != "")
                {
                    DateOfResigned = Convert.ToDateTime(dt.Rows[0].ItemArray[30].ToString()).ToShortDateString();
                }
                Company = clsQuery.returnCompanyDescription(dt.Rows[0].ItemArray[25].ToString());
                DateOfApproval = Convert.ToDateTime(dt.Rows[0].ItemArray[48].ToString()).ToShortDateString();
                PayrollGroup = clsQuery.returnPayrollDescription(dt.Rows[0].ItemArray[26].ToString());
                bank = dt.Rows[0].ItemArray[21].ToString();
                AccountNo = dt.Rows[0].ItemArray[22].ToString();
                Savings = Convert.ToDecimal(dt.Rows[0].ItemArray[43].ToString()).ToString("#,0.00");
                ShareCapital = Convert.ToDecimal(dt.Rows[0].ItemArray[42].ToString()).ToString("#,0.00");
            }

        }

    }
}
