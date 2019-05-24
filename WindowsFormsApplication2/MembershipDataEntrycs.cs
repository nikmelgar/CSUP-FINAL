using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class MembershipDataEntrycs : Form
    {
        public MembershipDataEntrycs()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        SqlConnection con;
        
        Global global = new Global();
        clsMembershipEntry clsMembershipEntry = new clsMembershipEntry();
        clsMembership clsmembership = new clsMembership();
        MembershipMain Membership = new MembershipMain();
        private void btnClose_Click(object sender, EventArgs e)
        {
            if(btnNew.Enabled == true)
            {
                if (clsMembershipEntry.CheckValuesEntry(txtLastName, txtFirstName, txtAddress, cmbCivilStatus, txtTINno, dtDateOfBirth, txtPlacePMS, dtDatePMS, txtEmployeeIDNo, cmbCompany, cmbPayrollGroup, cmbPayrollGroup, dtDateHired, txtContactName, txtContactNo1) == true)
                {
                    string msg = Environment.NewLine + "Are you sure you want to Cancel this Entry?";
                    DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        //Clear All fields inside Membership Data Entry
                        clearAllFields();
                        this.Close();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                //For update purposes
                if (clsMembershipEntry.CheckValuesForUpdating(txtLastName, txtFirstName, txtAddress, cmbCivilStatus, txtTINno, dtDateOfBirth, txtPlacePMS, dtDatePMS, txtEmployeeIDNo, cmbCompany,cmbPayrollGroup,cmbCostCenter, dtDateHired, txtContactName, txtContactNo1) == false)
                {
                    string msg = Environment.NewLine + "Are you sure you want to exit without saving changes?";
                    DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        //Clear All fields inside Membership Data Entry
                        clearAllFields();
                        this.Close();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    this.Close();
                }
            }
        }
    

        private void panelHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_firstClick == false)
                {
                    m_firstClick = true;
                    m_firstClickLoc = new Point(e.X, e.Y);
                }

                this.Location = new Point(
                    this.Location.X + e.X - m_firstClickLoc.X,
                    this.Location.Y + e.Y - m_firstClickLoc.Y
                    );
            }
            else
            {
                m_firstClick = false;
            }
        }
                
        private void dateTimePicker_OnTextChange(object sender, EventArgs e)
        {
            // Saving the 'Selected Date on Calendar' into DataGridView current cell  
            if(dataGridView1.CurrentCell.ColumnIndex == 2)
            {
                dataGridView1.CurrentCell.Value = oDateTimePicker.Text.ToString();
            }
            
        }

        void oDateTimePicker_CloseUp(object sender, EventArgs e)
        {
            // Hiding the control after use   
            oDateTimePicker.Visible = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 2 && dataGridView1.SelectedRows[0].Cells[0].Value.ToString() != "" && dataGridView1.SelectedRows[0].Cells[1].Value.ToString() != "")
                    {
                        oDateTimePicker.Visible = false;
                        //Initialized a new DateTimePicker Control  
                        oDateTimePicker = new DateTimePicker();

                        //Adding DateTimePicker control into DataGridView   
                        dataGridView1.Controls.Add(oDateTimePicker);

                        // Setting the format (i.e. 2014-10-10)  
                        oDateTimePicker.Format = DateTimePickerFormat.Short;

                        // It returns the retangular area that represents the Display area for a cell  
                        Rectangle oRectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                        //Setting area for DateTimePicker Control  
                        oDateTimePicker.Size = new Size(oRectangle.Width, oRectangle.Height);

                        // Setting Location  
                        oDateTimePicker.Location = new Point(oRectangle.X, oRectangle.Y);

                        // An event attached to dateTimePicker Control which is fired when DateTimeControl is closed  
                        oDateTimePicker.CloseUp += new EventHandler(oDateTimePicker_CloseUp);

                        // An event attached to dateTimePicker Control which is fired when any date is selected  
                        oDateTimePicker.TextChanged += new EventHandler(dateTimePicker_OnTextChange);

                        // Now make it visible  
                        oDateTimePicker.Visible = true;

                    }
                    else
                    {
                        oDateTimePicker.Visible = false;
                    }
                }
            }
            catch
            {

            }
           
        }

        public void oDateTimePicker_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true; 
        }

        private void MembershipDataEntrycs_Load(object sender, EventArgs e)
        {
            //Load All Data we need for Data Entry of New Members or Updating Info

            //=====================================================================
            //                      LOAD COMBOBOX DATA
            //=====================================================================
            clsMembershipEntry.loadComboBox(cmbCompany, "Company", "Description", "Company_Code");
            clsMembershipEntry.loadComboBox(cmbPayrollGroup, "Payroll_Group", "Description", "Payroll_Code");
            clsMembershipEntry.loadComboBox(cmboPrevComp, "Company", "Description", "Company_Code");
            clsMembershipEntry.loadComboBox(cmbBankName, "Bank", "Bank_Name", "Bank_Code");
            clsMembershipEntry.loadComboBox(cmbAreaCode, "Area_Code", "Area_Code", "Area_Code");
            clsMembershipEntry.loadComboBox(cmbOfficeArea, "Area_Code", "Area_Code", "Area_Code");
            clsMembershipEntry.loadComboBox(cmbContactAreaCode, "Area_Code", "Area_Code", "Area_Code");
            
            clearAllFields();

        }

        public void clearAllFields()
        {
            Control control = new Control();

            //=====================================================================
            //                      Company Information
            //=====================================================================
            foreach (var c in panel6.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
                if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
                if (c is MaskedTextBox) ((MaskedTextBox)c).Text = String.Empty;
            }

            //=====================================================================
            //                      Personal Information
            //=====================================================================
            foreach (var c in panelPersonalInformation.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
                if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
                if (c is MaskedTextBox) ((MaskedTextBox)c).Text = String.Empty;
            }

            //=====================================================================
            //                      Other Information
            //=====================================================================
            foreach (var c in panel10.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
            }

            //=====================================================================
            //                      Beneficiary and Contact Information
            //=====================================================================
            foreach (var c in tabControl1.TabPages[1].Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
                if (c is MaskedTextBox) ((MaskedTextBox)c).Text = String.Empty;
                if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
            }

            resetDatesToday();

        }


        public void clearAllFieldsFromDependentUsed()
        {
            Control control = new Control();
            //=====================================================================
            //                      Personal Information
            //=====================================================================
            foreach (var c in panelPersonalInformation.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
                if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
                if (c is MaskedTextBox) ((MaskedTextBox)c).Text = String.Empty;
            }

            //=====================================================================
            //                      Other Information
            //=====================================================================
            foreach (var c in panel10.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
            }

            //=====================================================================
            //                      Beneficiary and Contact Information
            //=====================================================================
            foreach (var c in tabControl1.TabPages[1].Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
            }


            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Value = "";
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {

            if(btnNew.Text == "NEW")
            {
                //controls
                foreach (Form form in Application.OpenForms)
                {
                    if (form.GetType() == typeof(MembershipSubForms.subMembershipOption))
                    {
                        form.Activate();
                        return;
                    }
                }

                MembershipSubForms.subMembershipOption membership = new MembershipSubForms.subMembershipOption();
                membership.Show();
            }
            else
            {
                //=====================================================================
                //                      Fields Validation for Entry (Membership)
                //=====================================================================
                //if(ValidateChildren(ValidationConstraints.Enabled))
                //{

                //}

                if (clsMembershipEntry.RequiredFields(txtLastName, txtFirstName,txtMiddleName, txtAddress, cmbGender, cmbCivilStatus, txtTINno, dtDateOfBirth, txtPlacePMS, dtDatePMS, txtEmployeeIDNo, cmbCompany, cmbPayrollGroup,cmbCostCenter, dtDateHired, txtContactName, txtOfficeTelNo,cmbBankName,txtAccountNo) == true)
                {
                    Alert.show("All fields with (*) are required.", Alert.AlertType.warning);
                    return;
                }

                //Check if theres a landline or Cellphone
                txtContactNo1.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                txtContactNo2.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

                if (txtContactNo1.Text == "" && txtContactNo2.Text == "")
                {
                    string str = "Please provide at least 1 contact no. in";
                    str += Environment.NewLine;
                    str += "Contact Person Information";
                    Alert.show(str, Alert.AlertType.warning);
                    return;
                }

                //=======================================================================
                //                  For Cellphone and Home Tel No.
                //=======================================================================
                //Check if no valiue for CP or Home Tel    
                txtHomeTel.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                txtCellNo.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

                if (txtHomeTel.Text == "" && txtCellNo.Text == "")
                {
                    string str = "Please provide at least 1 contact no. in";
                    str += Environment.NewLine;
                    str += "Personal Information Section";
                    Alert.show(str, Alert.AlertType.warning);
                    return;
                }
                //=======================================================================


                if (Convert.ToInt32(txtAccountNo.TextLength.ToString()) <= 9)
                {
                    Alert.show("Account No. Should not be less than 10 characters!", Alert.AlertType.warning);
                    return;
                }

                if (clsMembershipEntry.principal.ToString() != "False" && clsMembershipEntry.principal.ToString() != "0")
                {
                    //TIN get count and not save if less than 12
                    txtTINno.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                    if (txtTINno.Text.Length < 12)
                    {
                        Alert.show("TIN No. Should not be less than 12 characters!", Alert.AlertType.warning);
                        return;
                    }

                    //EMAIL VALIDATION
                    if (txtEmail.Text.Contains("@") != true)
                    {
                        Alert.show("Email address is invalid.", Alert.AlertType.warning);
                        return;
                    }
                }

                //=====================================================================
                //                      Saving Code Here
                //                      For Duplicate EmployeeID in PRINCIPAL
                //=====================================================================
                if (clsMembershipEntry.principal == "1")
                {
                    if (clsMembershipEntry.CheckDuplicateEmployeeID("Membership", txtEmployeeIDNo.Text) == true)
                    {
                        Alert.show("Employee ID No Already Exist", Alert.AlertType.error);
                        return;
                    }
                }

                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_InsertMembership";
                    cmd.Parameters.AddWithValue("@EmployeeID", txtEmployeeIDNo.Text);
                    cmd.Parameters.AddWithValue("@Principal", clsMembershipEntry.principal);
                    cmd.Parameters.AddWithValue("@PrincipalID", txtPrincipalNo.Text);
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@MiddleName", txtMiddleName.Text);
                    cmd.Parameters.AddWithValue("@Suffix", txtSuffix.Text);
                    cmd.Parameters.AddWithValue("@Gender", cmbGender.Text);



                    //DropDownList            
                    if (cmbBankName.Text == "")
                    {
                        cmd.Parameters.AddWithValue("@Bank_Code", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Bank_Code", cmbBankName.SelectedValue);
                    }

                    if (cmboPrevComp.Text == "")
                    {
                        cmd.Parameters.AddWithValue("@Prev_Company_Code", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Prev_Company_Code", cmboPrevComp.SelectedValue);
                    }


                    //For Image Saving
                    if (txtPath.Text != "")
                    {
                        byte[] bImageData = GetImageData(openFileDialog1.FileName);
                        cmd.Parameters.AddWithValue("@Member_Picture", bImageData);
                    }
                    else
                    {
                        byte[] bImageData = GetImageData("icons8-person-40.png");
                        cmd.Parameters.AddWithValue("@Member_Picture", bImageData);
                    }

                    cmd.Parameters.AddWithValue("@Residential_Address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@Date_Of_Birth", dtDateOfBirth.Text);
                    cmd.Parameters.AddWithValue("@Place_Of_Birth", txtPlaceOfBirth.Text);
                    cmd.Parameters.AddWithValue("@Civil_Status", cmbCivilStatus.Text);
                    cmd.Parameters.AddWithValue("@Name_Of_Spouse", txtSpouseName.Text);
                    if (cmbAreaCode.Text != "")
                    {
                        cmd.Parameters.AddWithValue("@Area_Code", cmbAreaCode.SelectedValue);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Area_Code", DBNull.Value);
                    }
                    cmd.Parameters.AddWithValue("@Home_Tel_No", txtHomeTel.Text);
                    cmd.Parameters.AddWithValue("@Cellphone_No", txtCellNo.Text);
                    cmd.Parameters.AddWithValue("@TinNo", txtTINno.Text);
                    cmd.Parameters.AddWithValue("@SSSNo", txtSSSNo.Text);
                    cmd.Parameters.AddWithValue("@Email_Address", txtEmail.Text);

                    cmd.Parameters.AddWithValue("@Atm_Account_No", txtAccountNo.Text);
                    cmd.Parameters.AddWithValue("@Place_PMS", txtPlacePMS.Text);
                    cmd.Parameters.AddWithValue("@Date_Of_PMS", dtDatePMS.Text);
                    cmd.Parameters.AddWithValue("@Company_Code", cmbCompany.SelectedValue);
                    cmd.Parameters.AddWithValue("@Payroll_Code", cmbPayrollGroup.SelectedValue);
                    cmd.Parameters.AddWithValue("@Cost_Center_Code", cmbCostCenter.SelectedValue);
                    cmd.Parameters.AddWithValue("@Date_Hired", dtDateHired.Text);

                    //Check if Date Resigned is open
                    if (dtDateResigned.Checked == true)
                    {
                        cmd.Parameters.AddWithValue("@Date_Resigned_From_Company", dtDateResigned.Text);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Date_Resigned_From_Company", DBNull.Value);
                    }

                    //Check if Date Resigned From PECCI is open
                    if (dtResignedFromPecci.Checked == true)
                    {
                        cmd.Parameters.AddWithValue("@Date_Resigned_From_Pecci", dtResignedFromPecci.Text);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Date_Resigned_From_Pecci", DBNull.Value);
                    }

                    if (cmbOfficeArea.Text != "")
                    {
                        cmd.Parameters.AddWithValue("@Office_Area_Code", cmbOfficeArea.SelectedValue);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Office_Area_Code", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@Office_Tel_No", txtOfficeTelNo.Text);
                    cmd.Parameters.AddWithValue("@Contact_Person", txtContactName.Text);

                    if (cmbContactAreaCode.Text != "")
                    {
                        cmd.Parameters.AddWithValue("@Contact_Area_Code", cmbContactAreaCode.SelectedValue);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Contact_Area_Code", DBNull.Value);
                    }


                    cmd.Parameters.AddWithValue("@Contact_No1", txtContactNo1.Text);
                    cmd.Parameters.AddWithValue("@Contact_No2", txtContactNo2.Text);
                    cmd.Parameters.AddWithValue("@Date_Of_Membership", dtDateMembership.Text);

                    cmd.Parameters.AddWithValue("@First_Deduction", dtFirstDeduction.Text);//First Deduction

                    //For Decimals
                    if (txtSalary.Text == "")
                    {
                        cmd.Parameters.AddWithValue("@Salary", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Salary", Convert.ToDecimal(txtSalary.Text));
                    }

                    if (txtMembershipFee.Text == "")
                    {
                        cmd.Parameters.AddWithValue("@Membership_Fee", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Membership_Fee", Convert.ToDecimal(txtMembershipFee.Text));
                    }

                    if (txtShareCapital.Text == "")
                    {
                        cmd.Parameters.AddWithValue("@Share_Capital", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Share_Capital", Convert.ToDecimal(txtShareCapital.Text));
                    }

                    if (txtSavingsDeposit.Text == "")
                    {
                        cmd.Parameters.AddWithValue("@Savings_Deposit", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Savings_Deposit", Convert.ToDecimal(txtSavingsDeposit.Text));
                    }


                    //Added Feb 17 2019 as per maam vangie
                    //Relationship and Remarks

                    cmd.Parameters.AddWithValue("@Relationship", txtRelationship.Text);
                    cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text);

                    //Check if theres a beneficiary
                    if (dataGridView1.Rows.Count > 0)
                    {
                        foreach (DataGridViewRow row1 in dataGridView1.Rows)
                        {
                            if (!row1.IsNewRow)
                            {
                                if (row1.Cells[2].Value == null || row1.Cells[2].Value.ToString() == "")
                                {
                                    Alert.show("Birthday of beneficiary is required.", Alert.AlertType.error);
                                    return;
                                }
                            }
                        }
                    }

                    if (dataGridView1.Rows.Count >= 2)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells[0].Value != null)
                            {
                                SqlCommand cmdBeneficiary = new SqlCommand();
                                cmdBeneficiary.Connection = con;
                                cmdBeneficiary.CommandType = CommandType.StoredProcedure;
                                cmdBeneficiary.CommandText = "sp_InsertBeneficiaries";
                                cmdBeneficiary.Parameters.AddWithValue("@EmployeeID", txtEmployeeIDNo.Text);
                                cmdBeneficiary.Parameters.AddWithValue("@Full_Name", row.Cells[0].Value);
                                cmdBeneficiary.Parameters.AddWithValue("@Relationship", row.Cells[1].Value);
                                cmdBeneficiary.Parameters.AddWithValue("@Date_Of_Birth", row.Cells[2].Value);
                                cmdBeneficiary.ExecuteNonQuery(); //SAVE BENEFICIARY
                            }
                        }
                    }
                    else
                    {
                        if (clsMembershipEntry.principal == "1")
                        {
                            Alert.show("Please put at least 1 Beneficiary!", Alert.AlertType.error);
                            return;
                        }
                    }



                    cmd.ExecuteNonQuery(); //SAVE MEMBER
                }

                //Clear all fields
                clearAllFields();
                picPicture.Image = imageList1.Images[0];
                dataGridView1.Rows.Clear();
                btnNew.Text = "NEW";

                Alert.show("Member successfully added.", Alert.AlertType.success);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {   
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = Path.GetFileName(openFileDialog1.FileName);
                picPicture.Image = new Bitmap(openFileDialog1.FileName);
            }
        }

        public byte[] GetImageData(String FileName)
        {
            FileStream fsImageStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            byte[] bImageData = new byte[fsImageStream.Length];
            fsImageStream.Read(bImageData, 0, System.Convert.ToInt32(fsImageStream.Length));
            fsImageStream.Close();
            return bImageData;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //=====================================================================
            //                      Fields Validation for Entry (Membership)
            //=====================================================================
            //if (ValidateChildren(ValidationConstraints.Enabled))
            //{

            //}

            if (clsMembershipEntry.RequiredFields(txtLastName, txtFirstName, txtMiddleName , txtAddress, cmbGender, cmbCivilStatus, txtTINno, dtDateOfBirth, txtPlacePMS, dtDatePMS, txtEmployeeIDNo, cmbCompany, cmbPayrollGroup, cmbCostCenter, dtDateHired, txtContactName, txtOfficeTelNo,cmbBankName,txtAccountNo) == true)
            {
                Alert.show("All fields with (*) are required.", Alert.AlertType.warning);
                return;
            }

            //Check if theres a landline or Cellphone
            txtContactNo1.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            txtContactNo2.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            if (txtContactNo1.Text == "" && txtContactNo2.Text == "")
            {
                string str = "Please provide at least 1 contact no. in";
                str += Environment.NewLine;
                str += "Contact Person Information";
                Alert.show(str, Alert.AlertType.warning);
                return;
            }

            //=======================================================================
            //                  For Cellphone and Home Tel No.
            //=======================================================================
            //Check if no valiue for CP or Home Tel    
            txtHomeTel.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            txtCellNo.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            if(txtHomeTel.Text == "" && txtCellNo.Text == "")
            {
                string str = "Please provide at least 1 contact no. in";
                str += Environment.NewLine;
                str += "Personal Information Section";
                Alert.show(str, Alert.AlertType.warning);
                return;
            }
            //=======================================================================
            if (Convert.ToInt32(txtAccountNo.TextLength.ToString()) <= 9)
            {
                Alert.show("Account No. Should not be less than 10 characters!", Alert.AlertType.warning);
                return;
            }

            if (clsMembershipEntry.principal.ToString() != "False" && clsMembershipEntry.principal.ToString() != "0")
            {
                //TIN get count and not save if less than 12
                txtTINno.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                if (txtTINno.Text.Length < 12)
                {
                    Alert.show("TIN No. Should not be less than 12 characters!", Alert.AlertType.warning);
                    return;
                }

                if (txtEmail.Text.Contains("@") != true)
                {
                    Alert.show("Email address is invalid.", Alert.AlertType.warning);
                    return;
                }
            }
            
            //=====================================================================
            //                      Saving Code Here
            //                      For Duplicate EmployeeID in PRINCIPAL
            //=====================================================================
            if (clsMembershipEntry.principal == "1" || clsMembershipEntry.principal == "True")
            {
                if (clsMembershipEntry.CheckDuplicateEmployeeIDUpdate("Membership", txtEmployeeIDNo.Text,clsMembershipEntry.userID,txtEmployeeIDNo.Text) == true)
                {
                    Alert.show("Employee ID No Already Exist", Alert.AlertType.error);
                    return;
                }
            }

            //=====================================================================
            //                     Before Resigning the EMployee
            //                      For ASking before updating
            //=====================================================================

            if (dtResignedFromPecci.Checked == true || dtDateResigned.Checked == true)
            {
                string msg = Environment.NewLine + "Are you sure you want to resign this member?";
                DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
                else
                {
                    //===============================================
                    //              Check if this is principal
                    //===============================================
                    if (clsMembershipEntry.principal == "1" || clsMembershipEntry.principal == "True")
                    {
                        //==========================================
                        //      Check if theres a Dependet
                        //==========================================

                        if (clsMembershipEntry.checkDependent(txtEmployeeIDNo.Text) == true)
                        {
                            using (SqlConnection con = new SqlConnection(global.connectString()))
                            {
                                con.Open();

                                SqlCommand cmdDependent = new SqlCommand();
                                cmdDependent.Connection = con;
                                cmdDependent.CommandText = "sp_ResignDependent";
                                cmdDependent.CommandType = CommandType.StoredProcedure;

                                if (dtResignedFromPecci.Checked == true)
                                {
                                    cmdDependent.Parameters.AddWithValue("@dtPecciResign", dtResignedFromPecci.Text);
                                }
                                else
                                {
                                    cmdDependent.Parameters.AddWithValue("@dtPecciResign", DBNull.Value);
                                }

                                if (dtDateResigned.Checked == true)
                                {
                                    cmdDependent.Parameters.AddWithValue("@dtCompResign", dtDateResigned.Text);
                                }
                                else
                                {
                                    cmdDependent.Parameters.AddWithValue("@dtCompResign", DBNull.Value);
                                }

                                cmdDependent.Parameters.AddWithValue("@EmployeeID", txtEmployeeIDNo.Text);

                                cmdDependent.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            //Setup Connection
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                //If Principal changes his/her info dependent will also reflect the company info he/she change
                if (clsMembershipEntry.principal == "1" || clsMembershipEntry.principal == "True")
                {
                    //CHeck if theres a dependent
                    SqlDataAdapter adapterCheck = new SqlDataAdapter("SELECT * FROM Membership WHERE EmployeeID ='" + txtEmployeeIDNo.Text + "' and Principal <> 1", con);
                    DataTable dtCheck = new DataTable();
                    adapterCheck.Fill(dtCheck);

                    if (dtCheck.Rows.Count >= 1)
                    {
                        SqlCommand cmdUpdateDependent = new SqlCommand();
                        cmdUpdateDependent.Connection = con;
                        if (cmbOfficeArea.Text != "" && txtSalary.Text != "")
                        {
                            cmdUpdateDependent.CommandText = "UPDATE Membership SET Payroll_Code ='" + cmbPayrollGroup.SelectedValue + "', Office_Area_Code ='" + cmbOfficeArea.Text + "', Salary ='" + Convert.ToDecimal(txtSalary.Text) + "' WHERE EmployeeID ='" + txtEmployeeIDNo.Text + "'";
                        }
                        else if (cmbOfficeArea.Text == "" && txtSalary.Text != "")
                        {
                            cmdUpdateDependent.CommandText = "UPDATE Membership SET Payroll_Code ='" + cmbPayrollGroup.SelectedValue + "', Office_Area_Code ='" + DBNull.Value + "', Salary ='" + Convert.ToDecimal(txtSalary.Text) + "' WHERE EmployeeID ='" + txtEmployeeIDNo.Text + "'";
                        }
                        else if (cmbOfficeArea.Text != "" && txtSalary.Text == "")
                        {
                            cmdUpdateDependent.CommandText = "UPDATE Membership SET Payroll_Code ='" + cmbPayrollGroup.SelectedValue + "', Office_Area_Code ='" + cmbOfficeArea.Text + "', Salary ='" + DBNull.Value + "' WHERE EmployeeID ='" + txtEmployeeIDNo.Text + "'";
                        }
                        else
                        {
                            cmdUpdateDependent.CommandText = "UPDATE Membership SET Payroll_Code ='" + cmbPayrollGroup.SelectedValue + "', Office_Area_Code ='" + DBNull.Value + "', Salary ='" + DBNull.Value + "' WHERE EmployeeID ='" + txtEmployeeIDNo.Text + "'";
                        }
                        cmdUpdateDependent.CommandType = CommandType.Text;
                        cmdUpdateDependent.ExecuteNonQuery();
                    }
                }


                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_UpdateMembership";
                cmd.Parameters.AddWithValue("@userID", clsMembershipEntry.userID);
                cmd.Parameters.AddWithValue("@EmployeeID", txtEmployeeIDNo.Text);
                cmd.Parameters.AddWithValue("@PrincipalID", txtPrincipalNo.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@MiddleName", txtMiddleName.Text);
                cmd.Parameters.AddWithValue("@Suffix", txtSuffix.Text);
                cmd.Parameters.AddWithValue("@Gender", cmbGender.Text);



                //DropDownList            
                if (cmbBankName.Text == "")
                {
                    cmd.Parameters.AddWithValue("@Bank_Code", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Bank_Code", cmbBankName.SelectedValue);
                }

                if (cmboPrevComp.Text == "")
                {
                    cmd.Parameters.AddWithValue("@Prev_Company_Code", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Prev_Company_Code", cmboPrevComp.SelectedValue);
                }


                //For Image Saving
                if (picPicture.Image == null)
                {
                    byte[] bImageData = GetImageData("icons8-person-40.png");
                    cmd.Parameters.AddWithValue("@Member_Picture", bImageData);
                }
                if (txtPath.Text != "")
                {
                    byte[] bImageData = GetImageData(openFileDialog1.FileName);
                    cmd.Parameters.AddWithValue("@Member_Picture", bImageData);
                }
                else
                {
                    Image myImage = picPicture.Image;
                    byte[] data;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        myImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        data = ms.ToArray();
                    }

                    cmd.Parameters.AddWithValue("@Member_Picture", data);
                }

                cmd.Parameters.AddWithValue("@Residential_Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@Date_Of_Birth", dtDateOfBirth.Text);
                cmd.Parameters.AddWithValue("@Place_Of_Birth", txtPlaceOfBirth.Text);
                cmd.Parameters.AddWithValue("@Civil_Status", cmbCivilStatus.Text);
                cmd.Parameters.AddWithValue("@Name_Of_Spouse", txtSpouseName.Text);
                if (cmbAreaCode.Text != "")
                {
                    cmd.Parameters.AddWithValue("@Area_Code", cmbAreaCode.SelectedValue);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Area_Code", DBNull.Value);
                }
                cmd.Parameters.AddWithValue("@Home_Tel_No", txtHomeTel.Text);
                cmd.Parameters.AddWithValue("@Cellphone_No", txtCellNo.Text);
                cmd.Parameters.AddWithValue("@TinNo", txtTINno.Text);
                cmd.Parameters.AddWithValue("@SSSNo", txtSSSNo.Text);
                cmd.Parameters.AddWithValue("@Email_Address", txtEmail.Text);

                cmd.Parameters.AddWithValue("@Atm_Account_No", txtAccountNo.Text);
                cmd.Parameters.AddWithValue("@Place_PMS", txtPlacePMS.Text);
                cmd.Parameters.AddWithValue("@Date_Of_PMS", dtDatePMS.Text);
                cmd.Parameters.AddWithValue("@Company_Code", cmbCompany.SelectedValue);
                cmd.Parameters.AddWithValue("@Payroll_Code", cmbPayrollGroup.SelectedValue);
                cmd.Parameters.AddWithValue("@Cost_Center_Code", cmbCostCenter.SelectedValue);
                cmd.Parameters.AddWithValue("@Date_Hired", dtDateHired.Text);

                //Check if Date Resigned is open
                if (dtDateResigned.Checked == true)
                {
                    cmd.Parameters.AddWithValue("@Date_Resigned_From_Company", dtDateResigned.Text);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Date_Resigned_From_Company", DBNull.Value);
                }

                //Check if Date Resigned From PECCI is open
                if (dtResignedFromPecci.Checked == true)
                {
                    cmd.Parameters.AddWithValue("@Date_Resigned_From_Pecci", dtResignedFromPecci.Text);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Date_Resigned_From_Pecci", DBNull.Value);
                }

                cmd.Parameters.AddWithValue("@Office_Area_Code", cmbOfficeArea.SelectedValue);
                cmd.Parameters.AddWithValue("@Office_Tel_No", txtOfficeTelNo.Text);
                cmd.Parameters.AddWithValue("@Contact_Person", txtContactName.Text);
                cmd.Parameters.AddWithValue("@Contact_Area_Code", cmbContactAreaCode.SelectedValue);
                cmd.Parameters.AddWithValue("@Contact_No1", txtContactNo1.Text);
                cmd.Parameters.AddWithValue("@Contact_No2", txtContactNo2.Text);
                cmd.Parameters.AddWithValue("@Date_Of_Membership", dtDateMembership.Text);

                cmd.Parameters.AddWithValue("@First_Deduction", dtFirstDeduction.Text);//First Deduction

                //For Decimals
                if (txtSalary.Text == "")
                {
                    cmd.Parameters.AddWithValue("@Salary", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Salary", Convert.ToDecimal(txtSalary.Text));
                }

                if (txtMembershipFee.Text == "")
                {
                    cmd.Parameters.AddWithValue("@Membership_Fee", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Membership_Fee", Convert.ToDecimal(txtMembershipFee.Text));
                }

                if (txtShareCapital.Text == "")
                {
                    cmd.Parameters.AddWithValue("@Share_Capital", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Share_Capital", Convert.ToDecimal(txtShareCapital.Text));
                }

                if (txtSavingsDeposit.Text == "")
                {
                    cmd.Parameters.AddWithValue("@Savings_Deposit", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Savings_Deposit", Convert.ToDecimal(txtSavingsDeposit.Text));
                }


                //Added Feb 17 2019 as per maam vangie
                //Relationship and Remarks
                cmd.Parameters.AddWithValue("@Relationship", txtRelationship.Text);
                cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text);

                cmd.ExecuteNonQuery();

                //Check if theres a beneficiary
                if (dataGridView1.Rows.Count > 0)
                {

                    //Delete First All Beneficiaries then Re-Insert
                    SqlCommand cmd1 = new SqlCommand();
                    cmd1.Connection = con;
                    cmd1.CommandText = "sp_DeleteBeneficiaries";
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@EmployeeID", txtEmployeeIDNo.Text);
                    cmd1.ExecuteNonQuery();

                    //Insert
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        foreach (DataGridViewRow row1 in dataGridView1.Rows)
                        {
                            if (!row1.IsNewRow)
                            {
                                if (row1.Cells[2].Value == null || row1.Cells[2].Value.ToString() == "")
                                {
                                    Alert.show("Birthday of beneficiary is required.", Alert.AlertType.error);
                                    return;
                                }
                            }
                        }
                    }

                }

                if (clsMembershipEntry.principal.ToString() == "True")
                {
                    if (dataGridView1.Rows.Count >= 2)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells[0].Value != null)
                            {
                                SqlCommand cmdBeneficiary = new SqlCommand();
                                cmdBeneficiary.Connection = con;
                                cmdBeneficiary.CommandType = CommandType.StoredProcedure;
                                cmdBeneficiary.CommandText = "sp_InsertBeneficiaries";
                                cmdBeneficiary.Parameters.AddWithValue("@EmployeeID", txtEmployeeIDNo.Text);
                                cmdBeneficiary.Parameters.AddWithValue("@Full_Name", row.Cells[0].Value);
                                cmdBeneficiary.Parameters.AddWithValue("@Relationship", row.Cells[1].Value);
                                cmdBeneficiary.Parameters.AddWithValue("@Date_Of_Birth", row.Cells[2].Value);
                                cmdBeneficiary.ExecuteNonQuery(); //SAVE BENEFICIARY
                            }
                        }
                    }
                    else
                    {
                        if (clsMembershipEntry.principal == "1")
                        {
                            Alert.show("Please put at least 1 Beneficiary!", Alert.AlertType.error);
                            return;
                        }
                    }

                    if (clsMembership.empIDStored != "")
                    {
                        //if empid is not null then he or she is a principal
                        //UPDATE ALL DEPENDENT ID IF PRINCIPAL CHANGE ID
                        SqlCommand cmdUpdateID = new SqlCommand();
                        cmdUpdateID.Connection = con;
                        cmdUpdateID.CommandText = "UPDATE Membership SET PrincipalID = '"+ txtEmployeeIDNo.Text +"', EmployeeID = '" + txtEmployeeIDNo.Text + "' WHERE EmployeeID = '" + clsMembership.empIDStored + "' and Principal <> '1'";
                        cmdUpdateID.CommandType = CommandType.Text;
                        cmdUpdateID.ExecuteNonQuery();
                    }
                }
               
            }
            
            //Clear all fields
            clearAllFields();
            //Clear datagridview
            dataGridView1.Rows.Clear();
            picPicture.Image = imageList1.Images[0];
            btnNew.Text = "NEW";
            btnNew.Enabled = true;
            btnEdit.Enabled = false;

            //refresh
            Membership = (MembershipMain)Application.OpenForms["MembershipMain"];
            Membership.loadDatas();

            Alert.show("Successfully updated.", Alert.AlertType.success);

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow.IsNewRow)
                {
                    return;
                }

                if (dataGridView1.SelectedRows.Count >= 1)
                {
                    string msg = Environment.NewLine + "Are you sure you want to delete this beneficiary?";
                    DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            if (row.Cells[0].Value != null)
                            {
                                dataGridView1.Rows.Remove(row);
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch
            {

            }
            
        }

        private void txtHomeTel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&(e.KeyChar != '-') && (e.KeyChar != '(') && (e.KeyChar != ')') && (e.KeyChar != (char)Keys.Space))
            {
                e.Handled = true;
            }
        }

        private void txtLastName_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Space);
        }

        private void txtFirstName_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back || e.KeyChar ==(char)Keys.Space);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private void txtSalary_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dataGridView1.CurrentCell.ColumnIndex == 2) //Desired Column
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtSalary_Leave(object sender, EventArgs e)
        {
            if(txtSalary.Text != "")
            {
                txtSalary.Text = Convert.ToDecimal(txtSalary.Text).ToString("#,0.00");
            }
        }

        private void txtMembershipFee_Leave(object sender, EventArgs e)
        {
            if(txtMembershipFee.Text != "")
            {
                txtMembershipFee.Text = Convert.ToDecimal(txtMembershipFee.Text).ToString("#,0.00");
            }
        }

        private void txtShareCapital_Leave(object sender, EventArgs e)
        {
            if (txtShareCapital.Text != "")
            {
                txtShareCapital.Text = Convert.ToDecimal(txtShareCapital.Text).ToString("#,0.00");
            }
        }

        private void txtSavingsDeposit_Leave(object sender, EventArgs e)
        {
            if (txtSavingsDeposit.Text != "")
            {
                txtSavingsDeposit.Text = Convert.ToDecimal(txtSavingsDeposit.Text).ToString("#,0.00");
            }
        }

        public void resetDatesToday()
        {
            dtDateOfBirth.Value = DateTime.Today;
            dtDatePMS.Value = DateTime.Today;
            dtDateResigned.Value = DateTime.Today;
            dtDateHired.Value = DateTime.Today;
            dtResignedFromPecci.Value = DateTime.Today;
            dtFirstDeduction.Value = DateTime.Today;
            dtDateMembership.Value = DateTime.Today;
        }

        private void cmbAreaCode_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbAreaCode.Text != "")
            {
                txtHomeTel.Enabled = true;
            }
            else
            {
                txtHomeTel.Enabled = false;
                txtHomeTel.Text = "";
            }
        }

        private void cmbOfficeArea_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbOfficeArea.Text != "")
            {
                txtOfficeTelNo.Enabled = true;
            }
            else
            {
                txtOfficeTelNo.Enabled = false;
                txtOfficeTelNo.Text = "";
            }
        }

        private void cmbContactAreaCode_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbContactAreaCode.Text != "")
            {
                txtContactNo1.Enabled = true;
            }
            else
            {
                txtContactNo1.Enabled = false;
                txtContactNo1.Text = "";
            }
        }

        private void cmbCompany_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbCompany.SelectedValue != DBNull.Value && cmbCompany.SelectedIndex != -1 && cmbCompany.Text != "")
            {
                if (cmbCompany.Text != "PLDT" && cmbCompany.Text != "NON PAYROLL")
                {
                    //Updating of old company
                    if(clsmembership.returnCompanyNonPayroll(Convert.ToInt32(clsMembershipEntry.userID)) != cmbCompany.SelectedValue.ToString())
                    {
                        cmboPrevComp.SelectedValue = clsmembership.returnCompanyNonPayroll(Convert.ToInt32(clsMembershipEntry.userID));
                    }
                    else
                    {
                        cmboPrevComp.SelectedIndex = -1;
                    }

                    clsMembershipEntry.loadComboBox(cmbCostCenter, "Company", "Description", "Company_Code");
                    cmbCostCenter.SelectedValue = cmbCompany.SelectedValue;
                    cmbCostCenter.Enabled = false;
                }
                else if (cmbCompany.Text == "NON PAYROLL")
                {
                    //Move the Company to Prev Company Automatic
                    cmboPrevComp.SelectedValue = clsmembership.returnCompanyNonPayroll(Convert.ToInt32(clsMembershipEntry.userID));

                    //move to cost center
                    clsMembershipEntry.loadComboBox(cmbCostCenter, "Company", "Description", "Company_Code");
                    cmbCostCenter.SelectedValue = cmbCompany.SelectedValue;
                    cmbCostCenter.Enabled = false;
                }
                else
                {
                    //Updating of old company
                    if (clsmembership.returnCompanyNonPayroll(Convert.ToInt32(clsMembershipEntry.userID)) != cmbCompany.SelectedValue.ToString())
                    {
                        cmboPrevComp.SelectedValue = clsmembership.returnCompanyNonPayroll(Convert.ToInt32(clsMembershipEntry.userID));
                    }
                    else
                    {
                        cmboPrevComp.SelectedIndex = -1;
                    }

                    clsMembershipEntry.loadComboBox(cmbCostCenter, "Cost_Center", "Description", "Cost_Center_Code");
                    cmbCostCenter.Enabled = true;
                   
                }
            }
        }

//=========================================================================================================================================================
//                                                      START REGION FOR HIGHLIGHT TEXTBOX 

        private void txtLastName_TextChanged(object sender, EventArgs e)
        {
            clsMembershipEntry.RequiredFieldColor(txtLastName, txtFirstName,txtMiddleName, txtAddress, txtTINno, txtAccountNo, txtEmail, txtPlacePMS, txtEmployeeIDNo);
        }

        private void txtLastName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                txtLastName.Focus();
                errorProvider1.SetError(txtLastName, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(txtLastName, "");
            }
        }

        private void txtFirstName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                txtFirstName.Focus();
                errorProvider1.SetError(txtFirstName, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(txtFirstName, "");
            }
        }

        private void cmbGender_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbGender.Text))
            {
                cmbGender.Focus();
                errorProvider1.SetError(cmbGender, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(cmbGender, "");
            }
        }

        private void txtAddress_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                txtAddress.Focus();
                errorProvider1.SetError(txtAddress, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(txtAddress, "");
            }
        }

        private void cmbCivilStatus_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbCivilStatus.Text))
            {
                cmbCivilStatus.Focus();
                errorProvider1.SetError(cmbCivilStatus, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(cmbCivilStatus, "");
            }
        }

        private void txtTINno_Validating(object sender, CancelEventArgs e)
        {
            if(clsMembershipEntry.principal.ToString() != "False" && clsMembershipEntry.principal.ToString() != "0") //If not dependent
            {
                txtTINno.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

                if (string.IsNullOrWhiteSpace(txtTINno.Text))
                {
                    txtTINno.Focus();
                    errorProvider1.SetError(txtTINno, "This should not be left blank!");
                }
                else
                {
                    errorProvider1.SetError(txtTINno, "");
                }
                
            }
            
        }

        private void cmbBankName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbBankName.Text))
            {
                cmbBankName.Focus();
                errorProvider1.SetError(cmbBankName, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(cmbBankName, "");
            }
        }

        private void txtAccountNo_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAccountNo.Text))
            {
                txtAccountNo.Focus();
                errorProvider1.SetError(txtAccountNo, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(txtAccountNo, "");
            }
        }

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            if (clsMembershipEntry.principal.ToString() != "False" && clsMembershipEntry.principal.ToString() != "0")
            {
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    txtEmail.Focus();
                    errorProvider1.SetError(txtEmail, "This should not be left blank!");
                }
                else
                {
                    errorProvider1.SetError(txtEmail, "");
                }
            }
        }

        private void txtPlacePMS_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlacePMS.Text))
            {
                txtPlacePMS.Focus();
                errorProvider1.SetError(txtPlacePMS, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(txtPlacePMS, "");
            }
        }

        private void txtEmployeeIDNo_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeIDNo.Text))
            {
                txtEmployeeIDNo.Focus();
                errorProvider1.SetError(txtEmployeeIDNo, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(txtEmployeeIDNo, "");
            }
        }

        private void txtContactName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContactName.Text))
            {
                txtContactName.Focus();
                errorProvider1.SetError(txtContactName, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(txtContactName, "");
            }
        }

        private void cmbCompany_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbCompany.Text))
            {
                cmbCompany.Focus();
                errorProvider1.SetError(cmbCompany, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(cmbCompany, "");
            }
        }

        private void cmbPayrollGroup_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbPayrollGroup.Text))
            {
                cmbPayrollGroup.Focus();
                errorProvider1.SetError(cmbPayrollGroup, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(cmbPayrollGroup, "");
            }
        }

        private void cmbCostCenter_Validating(object sender, CancelEventArgs e)
        {
            if(cmbCostCenter.DataSource != null)
            {
                if (string.IsNullOrWhiteSpace(cmbCostCenter.Text))
                {
                    cmbCostCenter.Focus();
                    errorProvider1.SetError(cmbCostCenter, "This should not be left blank!");
                }
                else
                {
                    errorProvider1.SetError(cmbCostCenter, "");
                }
            }
        }

        private void txtContactName_TextChanged(object sender, EventArgs e)
        {
            if (txtContactName.Text == "")
            {
                txtContactName.BackColor = Color.FromArgb(128, 255, 128);
                tabControl1.SelectedTab = tabPage2;
            }
            else
            {
                txtContactName.BackColor = SystemColors.Window;
            }
        }

        private void txtMiddleName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMiddleName.Text))
            {
                txtMiddleName.Focus();
                errorProvider1.SetError(txtMiddleName, "This should not be left blank!");
            }
            else
            {
                errorProvider1.SetError(txtMiddleName, "");
            }
        }

        private void txtMiddleName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Tab)
            {
                oDateTimePicker.Visible = false;
            }
        }

        private void label66_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            picPicture.Image = imageList1.Images[0];
        }
    }
}
