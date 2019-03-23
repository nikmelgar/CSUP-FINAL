using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;


namespace WindowsFormsApplication2.Classes
{
    class clsCashReceipt
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        Global global = new Global();

        public static int userID { get; set; }

        public string returnCompanyDescription(String CompanyCode)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Description FROM Company WHERE Company_Code = '" + CompanyCode + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }
        }

        public string returnCompanyCodeFromDescription(String CompanyDescription)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT Company_Code FROM Company WHERE Description = '" + CompanyDescription + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                return dt.Rows[0].ItemArray[0].ToString();
            }  
        }

        public void loadBank(ComboBox cmb)
        {
            cmb.DataSource = null;

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT (Bank_Code) as Bank_Name,Bank_Code FROM Bank WHERE isActive = 1", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmb.DisplayMember = "Bank_Name";
                cmb.ValueMember = "Bank_Code";
                cmb.DataSource = dt;
            }
        }


        #region =========================================================VALIDATION=================================================================================

        public bool orNumberDuplicate(TextBox or)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                adapter = new SqlDataAdapter("SELECT * FROM Cash_Receipts_Header WHERE Or_No ='" + or.Text + "'", con);
                dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool CashValidation(TextBox or, TextBox payorid, TextBox totalTransaction, TextBox totalDebit, TextBox totalCredit, DataGridView dgvTrans, DataGridView dgvDetails, DataGridView dgvCheck, RadioButton radioCash, RadioButton radioPecciCheck, RadioButton radioNonPecci)
        {

            if (radioCash.Checked == true)
            {
                //Validation For Cash Transaction

                if(orNumberDuplicate(or) == true)
                {
                    Alert.show("OR Number already used. Please try again.", Alert.AlertType.error);
                    return true;
                }
                else if (or.Text == "" || payorid.Text == "")
                {
                    Alert.show("All fields with ( * ) are required.", Alert.AlertType.warning);
                    return true;
                }
                else if (totalTransaction.Text == "" || totalTransaction.Text == "0.00")
                {
                    Alert.show("Transaction is required.", Alert.AlertType.warning);
                    return true;
                }
                else if (totalDebit.Text == "" || totalCredit.Text == "")
                {
                    Alert.show("Details information is required.", Alert.AlertType.warning);
                    return true;
                }
                else if (dgvDetails.Rows.Count <= 1)
                {
                    Alert.show("Details information is required.", Alert.AlertType.warning);
                    return true;
                }
                else if (Convert.ToDecimal(totalTransaction.Text) != Convert.ToDecimal(totalDebit.Text) || Convert.ToDecimal(totalTransaction.Text) != Convert.ToDecimal(totalCredit.Text))
                {
                    Alert.show("Amount in transaction not equal to amount in details.", Alert.AlertType.error);
                    return true;
                }
                else if (Convert.ToDecimal(totalDebit.Text) != Convert.ToDecimal(totalCredit.Text))
                {
                    Alert.show("Debit / Credit not equal.", Alert.AlertType.error);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //Validation For Cash Transaction
                if (orNumberDuplicate(or) == true)
                {
                    Alert.show("OR Number already used. Please try again.", Alert.AlertType.error);
                    return true;
                }
                else if (or.Text == "" || payorid.Text == "")
                {
                    Alert.show("All fields with ( * ) are required.", Alert.AlertType.warning);
                    return true;
                }
                else if (totalTransaction.Text == "" || totalTransaction.Text == "0.00")
                {
                    Alert.show("Transaction is required.", Alert.AlertType.warning);
                    return true;
                }
                else if(dgvCheck.Rows.Count == 0)
                {
                    Alert.show("Please add cheque information.", Alert.AlertType.warning);
                    return true;
                }
                else if (totalDebit.Text == "" || totalCredit.Text == "")
                {
                    Alert.show("Details information is required.", Alert.AlertType.warning);
                    return true;
                }
                else if (dgvDetails.Rows.Count <= 1)
                {
                    Alert.show("Details information is required.", Alert.AlertType.warning);
                    return true;
                }
                else if (Convert.ToDecimal(totalTransaction.Text) != Convert.ToDecimal(totalDebit.Text) || Convert.ToDecimal(totalTransaction.Text) != Convert.ToDecimal(totalCredit.Text))
                {
                    Alert.show("Amount in transaction not equal to amount in details.", Alert.AlertType.error);
                    return true;
                }
                else if (Convert.ToDecimal(totalDebit.Text) != Convert.ToDecimal(totalCredit.Text))
                {
                    Alert.show("Debit / Credit not equal.", Alert.AlertType.error);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        //FOR UPDATE
        public bool CashValidationUpdate(TextBox or, TextBox payorid, TextBox totalTransaction, TextBox totalDebit, TextBox totalCredit, DataGridView dgvTrans, DataGridView dgvDetails, DataGridView dgvCheck, RadioButton radioCash, RadioButton radioPecciCheck, RadioButton radioNonPecci)
        {

            if (radioCash.Checked == true)
            {
                //Validation For Cash Transaction
                if (or.Text == "" || payorid.Text == "")
                {
                    Alert.show("All fields with ( * ) are required.", Alert.AlertType.warning);
                    return true;
                }
                else if (totalTransaction.Text == "" || totalTransaction.Text == "0.00")
                {
                    Alert.show("Transaction is required.", Alert.AlertType.warning);
                    return true;
                }
                else if (totalDebit.Text == "" || totalCredit.Text == "")
                {
                    Alert.show("Details information is required.", Alert.AlertType.warning);
                    return true;
                }
                else if (dgvDetails.Rows.Count <= 1)
                {
                    Alert.show("Details information is required.", Alert.AlertType.warning);
                    return true;
                }
                else if (Convert.ToDecimal(totalTransaction.Text) != Convert.ToDecimal(totalDebit.Text) || Convert.ToDecimal(totalTransaction.Text) != Convert.ToDecimal(totalCredit.Text))
                {
                    Alert.show("Amount in transaction not equal to amount in details.", Alert.AlertType.error);
                    return true;
                }
                else if (Convert.ToDecimal(totalDebit.Text) != Convert.ToDecimal(totalCredit.Text))
                {
                    Alert.show("Debit / Credit not equal.", Alert.AlertType.error);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //Validation For Cash Transaction
                if (or.Text == "" || payorid.Text == "")
                {
                    Alert.show("All fields with ( * ) are required.", Alert.AlertType.warning);
                    return true;
                }
                else if (totalTransaction.Text == "" || totalTransaction.Text == "0.00")
                {
                    Alert.show("Transaction is required.", Alert.AlertType.warning);
                    return true;
                }
                else if (dgvCheck.Rows.Count == 0)
                {
                    Alert.show("Please add Cheque information.", Alert.AlertType.warning);
                    return true;
                }
                else if (totalDebit.Text == "" || totalCredit.Text == "")
                {
                    Alert.show("Details information is required.", Alert.AlertType.warning);
                    return true;
                }
                else if (dgvDetails.Rows.Count <= 1)
                {
                    Alert.show("Details information is required.", Alert.AlertType.warning);
                    return true;
                }
                else if (Convert.ToDecimal(totalTransaction.Text) != Convert.ToDecimal(totalDebit.Text) || Convert.ToDecimal(totalTransaction.Text) != Convert.ToDecimal(totalCredit.Text))
                {
                    Alert.show("Amount in transaction not equal to amount in details.", Alert.AlertType.error);
                    return true;
                }
                else if (Convert.ToDecimal(totalDebit.Text) != Convert.ToDecimal(totalCredit.Text))
                {
                    Alert.show("Debit / Credit not equal.", Alert.AlertType.error);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

    }
}
