using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class QueryMemberProfile : UserControl
    {
        public QueryMemberProfile()
        {
            InitializeComponent();
        }

        Global global = new Global();
        Classes.clsQueryMember clsQueryMember = new Classes.clsQueryMember();
        
        private void QueryMemberProfile_Load(object sender, EventArgs e)
        {
            txtEmpID.Text = "";
            txtLastName.Text = "";
            txtFirstName.Text = "";
            txtMiddleName.Text = "";
            txtSuffix.Text = "";
            txtAddress.Text = "";
            txtTelephone.Text = "";
            txtSex.Text = "";
            txtCivilStatus.Text = "";
            txtBirthday.Text = "";
            txtBirthPlace.Text = "";
            txtNameOfSpouse.Text = "";
            txtDateOfMembership.Text = "";
            txtCompany.Text = "";
            txtDateOfApproval.Text = "";
            txtPayrollGroup.Text = "";
            txtBank.Text = "";
            txtDateOfResigned.Text = "";
            txtAccntNo.Text = "";
            txtSavings.Text = "";
            txtShareCapital.Text = "";
            txtEmailAddress.Text = "";
            txtMobileNo.Text = "";
            txtContactName.Text = "";
            txtContactRelationShip.Text = "";
            txtMobileNo.Text = "";
            txtTelephone.Text = "";
            txtContactRemarks.Text = "";
        }

        public void loadMembersInfo()
        {
            txtEmpID.Text = Classes.clsQueryMember.EmployeeID;
            txtLastName.Text = Classes.clsQueryMember.LastName;
            txtFirstName.Text = Classes.clsQueryMember.FirstName;
            txtMiddleName.Text = Classes.clsQueryMember.MiddleName;
            txtSuffix.Text = Classes.clsQueryMember.suffix;
            txtAddress.Text = Classes.clsQueryMember.Address;
            txtTelephone.Text = Classes.clsQueryMember.Telephone;
            txtSex.Text = Classes.clsQueryMember.sex;
            txtCivilStatus.Text = Classes.clsQueryMember.CivilStatus;
            txtBirthday.Text = Classes.clsQueryMember.Birthday;
            txtBirthPlace.Text = Classes.clsQueryMember.BirthPlace;
            txtNameOfSpouse.Text = Classes.clsQueryMember.NameOfSpouse;
            txtDateOfMembership.Text = Classes.clsQueryMember.DateOfMembership;
            txtCompany.Text = Classes.clsQueryMember.Company;
            txtDateOfApproval.Text = Classes.clsQueryMember.DateOfApproval;
            txtPayrollGroup.Text = Classes.clsQueryMember.PayrollGroup;
            txtBank.Text = Classes.clsQueryMember.bank;
            txtDateOfResigned.Text = Classes.clsQueryMember.DateOfResigned;
            txtAccntNo.Text = Classes.clsQueryMember.AccountNo;
            txtSavings.Text = Classes.clsQueryMember.Savings;
            txtShareCapital.Text = Classes.clsQueryMember.ShareCapital;
            txtMobileNo.Text = Classes.clsQueryMember.mobileNo;
            txtEmailAddress.Text = Classes.clsQueryMember.emailAddress;
            txtContactName.Text = Classes.clsQueryMember.contactName;
            txtContactRelationShip.Text = Classes.clsQueryMember.contactRelationship;
            txtContactMobileNo.Text = Classes.clsQueryMember.contactMobileNo;
            txtContactTelephone.Text = Classes.clsQueryMember.contactTelephone;
            txtContactRemarks.Text = Classes.clsQueryMember.contactRemarks;

            clsQueryMember.loadBeneficiaries(dataGridView1, txtEmpID.Text, Classes.clsQueryMember.principal);
        }
    }
}
