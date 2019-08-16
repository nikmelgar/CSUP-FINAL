using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsVoucherValidation
    {
        Global global = new Global();


        //3 = Team Head
        //1 = Accounting

        public Boolean checkIfTeamHeadAccounting()
        {
            if(clsUser.department == "1" && clsUser.role == "3")
            {
                return true;
            }
            else
            {
                Alert.show("Error : Access denied.", Alert.AlertType.error);
                return false;
            }
        }

        public Boolean checkIfAccntgDepartment()
        {
            if(clsUser.department == "1")
            {
                return true;
            }
            else
            {
                Alert.show("Error : Access denied.", Alert.AlertType.error);
                return false;
            }
        }

        public Boolean isPrepBy(string preparedBy)
        {
            if(preparedBy == clsUser.Username)
            {
                //Abort Posting for user who created the voucher
                Alert.show("You cannot post your own voucher.", Alert.AlertType.error);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
