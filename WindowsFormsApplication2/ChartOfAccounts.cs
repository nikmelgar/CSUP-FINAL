using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication2;

namespace WindowsFormsApplication2
{
    public partial class ChartOfAccounts : Form
    {

        TreeNode _selectedNode = null;
        DataTable _acountsTb = null;
        SqlConnection _connection;
        SqlCommand _command;
        SqlDataAdapter _adapter;
        public ChartOfAccounts()
        {           

            InitializeComponent();
            _acountsTb = new DataTable();
            // _connection = new SqlConnection("Data Source=192.168.255.176;Initial Catalog=PECCI-NEW;UserID=sa;Password=SYSADMIN;");
            Global global = new Global();
            _connection = new SqlConnection();
            global.connection(_connection);
            _command = new SqlCommand();
            _command.Connection = _connection;

        
            global.loadComboBoxDistinct(cmbAccountGroup, "chart_of_accounts", "account_group");
            

        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        //Getting the Parent Account
        Global global = new Global();
        SqlConnection con = new SqlConnection();

        clsChartOfAccount clsChart = new clsChartOfAccount();

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
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

        private void ChartOfAccounts_Load(object sender, EventArgs e)
        {

            String sql = "SELECT *  FROM [chart_of_accounts] where isActive = '1'";
            try
            {
               // _connection.Open();
                _adapter = new SqlDataAdapter(sql, _connection);

                _adapter.Fill(_acountsTb);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
               // _connection.Close();
            }

            loadParentNo(cmbParentAccount, "chart_of_accounts", "Account");
            PopulateTreeView("0", null);
        }
        private void PopulateTreeView(string parentId, TreeNode parentNode)
        {
            TreeNode childNode;

            foreach (DataRow dr in _acountsTb.Select("[Parent_Account]='" + parentId + "'"))
            {
                TreeNode t = new TreeNode();
                t.Text = dr["Account_Code"].ToString() + " - " + dr["Account_Description"].ToString();
                t.Name = dr["Account_Code"].ToString();
                t.Tag = _acountsTb.Rows.IndexOf(dr);
                if (parentNode == null)
                {
                    treeView1.Nodes.Add(t);
                    childNode = t;
                }
                else
                {
                    parentNode.Nodes.Add(t);
                    childNode = t;
                }
                PopulateTreeView(dr["Account_Code"].ToString(), childNode);
            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _selectedNode = treeView1.SelectedNode;
            ShowNodeData(_selectedNode);

        }

        private void ShowNodeData(TreeNode nod)
        {
            DataRow r = _acountsTb.Rows[int.Parse(nod.Tag.ToString())];
            txtCode.Text = r["Account_Code"].ToString();


            ////Load Parent ComboBox FIrst
            loadParentNoEDIT(cmbParentAccount, "chart_of_accounts", "Account", txtCode.Text);

            global.connection(con);
            SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT Account_Description,Account_Group,Account_Type FROM chart_of_accounts WHERE Account_Code = '"+ txtCode.Text +"'", con);
            DataTable dt2 = new DataTable();
            adapter2.Fill(dt2);

            if(dt2.Rows.Count > 0)
            {
                txtDescription.Text = dt2.Rows[0].ItemArray[0].ToString();
                cmbAccountGroup.Text = dt2.Rows[0].ItemArray[1].ToString();
                cmbAccountType.Text = dt2.Rows[0].ItemArray[2].ToString();
            }

            try
            {
                if (r["Parent_Account"].ToString() == "0")
                {
                    cmbParentAccount.Text = "";
                }
                else
                {                    
                    global.connection(con);

                    //Get the Account code then get the parent code
                    SqlDataAdapter adapterGetParentCode = new SqlDataAdapter("SELECT Parent_Account FROM chart_of_accounts WHERE Account_Code ='" + r["Account_Code"] +"'", con);
                    DataTable dtParent = new DataTable();
                    adapterGetParentCode.Fill(dtParent);

                    //Check if theres a ROW
                    if(dtParent.Rows.Count > 0)
                    {
                        //Getting The Account Code + Account Description
                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT top 1 (account_code + ' - ' + Account_description)  FROM chart_of_accounts where Account_Code ='" + dtParent.Rows[0].ItemArray[0].ToString() + "'", con);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);


                        cmbParentAccount.Text = dt.Rows[0].ItemArray[0].ToString();
                    }
                    
                }

                global.connection(con);

                //Get the Account code then get the parent code
                SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT Loan_Related,isActive FROM chart_of_accounts WHERE Account_Code ='" + r["Account_Code"] + "'", con);
                DataTable dt1 = new DataTable();
                adapter1.Fill(dt1);

                if(dt1.Rows.Count > 0)
                {
                    if(dt1.Rows[0].ItemArray[0].ToString() == "True")
                    {
                        chckLoanRelated.Checked = true;
                    }
                    else
                    {
                        chckLoanRelated.Checked = false;
                    }

                    if(dt1.Rows[0].ItemArray[1].ToString() == "True")
                    {
                        chckActive.Checked = true;
                    }
                    else
                    {
                        chckActive.Checked = false;
                    }
                }
            }
            catch
            {

            }
            
        }

        public void loadParentNo(ComboBox cmb, string tableName, string Display)
        {
            cmb.DataSource = null;

            SqlConnection con = new SqlConnection();
            Global global = new Global();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT (account_code + ' - ' + Account_description) as Account,Account_Code FROM  " + tableName + " WHERE isActive ='1'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            cmb.DisplayMember = Display;
            cmb.ValueMember = "Account_Code";
            cmb.DataSource = dt;

        }

        public void loadParentNoEDIT(ComboBox cmb, string tableName, string Display,string accountCode)
        {
            cmb.DataSource = null;

            SqlConnection con = new SqlConnection();
            Global global = new Global();
            global.connection(con);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT (account_code + ' - ' + Account_description) as Account,Account_Code FROM  " + tableName + " WHERE isActive ='1' and Account_Code <> '"+ accountCode +"'", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            cmb.DisplayMember = Display;
            cmb.ValueMember = "Account_Code";
            cmb.DataSource = dt;

        }

        public void clearFields()
        {
            txtCode.Text = "";
            txtDescription.Text = "";
            cmbAccountGroup.Text = "";
            cmbAccountType.Text = "";
            cmbParentAccount.Text = "";
            chckActive.Checked = false;
            chckLoanRelated.Checked = false;
        }

        public void enabledFields()
        {
            txtCode.Enabled = true;
            txtDescription.Enabled = true;
            cmbAccountType.Enabled = true;
            cmbParentAccount.Enabled = true;
            cmbAccountGroup.Enabled = true;
            chckActive.Enabled = true;
            chckLoanRelated.Enabled = true;
        }

        public void disabledFields()
        {
            txtCode.Enabled = false;
            txtDescription.Enabled = false;
            cmbAccountType.Enabled = false;
            cmbParentAccount.Enabled = false;
            cmbAccountGroup.Enabled = false;
            chckActive.Enabled = false;
            chckLoanRelated.Enabled = false;
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            //Nikko Melgar
            //DATE : 7/31/2018

            if(btnClose.Text == "CANCEL")
            {
                //Bring back the original text in buttons
                btnNew.Text = "NEW";
                btnEdit.Text = "EDIT";
                btnDelete.Text = "DELETE";
                btnClose.Text = "CLOSE";

                //Enable all buttons again
                btnNew.Enabled = true;
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
                
                //Clear Fields and disable it
                disabledFields();
                clearFields();
            }
            else
            {
                this.Close();
            }
        }

        //Checking of fields
        public Boolean checkFields()
        {
            if(txtCode.Text == "" || txtDescription.Text == "" || cmbAccountGroup.Text == "" || cmbAccountType.Text == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            enabledFields();

            if(btnNew.Text == "SAVE")
            {
                //Save goes here

                //Check if all fields have value
                if(checkFields() == true)
                {
                    Alert.show("All fields with (*) are required", Alert.AlertType.warning);
                    return;
                }

                //check for duplicate entry
                Global global = new Global();
                if(global.CheckDuplicateEntryParam("Account_Code", txtCode.Text, "chart_of_accounts") == true)
                {
                    Alert.show("Account Code Already Exist", Alert.AlertType.error);
                    return;
                }

                SqlConnection con = new SqlConnection();
                global.connection(con);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_insertChartofAccounts";
                cmd.Parameters.AddWithValue("@Account_Code", txtCode.Text);
                cmd.Parameters.AddWithValue("@Account_Description", txtDescription.Text);
                cmd.Parameters.AddWithValue("@Account_Group", cmbAccountGroup.Text);
                cmd.Parameters.AddWithValue("@Account_Type", cmbAccountType.Text);
                
                //Check if Parent account is null
                if(cmbParentAccount.Text == "")
                {
                    cmd.Parameters.AddWithValue("@Parent_Account", "0");
                    cmd.Parameters.AddWithValue("@LevelNo", "0");
                }
                else
                {
                    //Get Parent And Level of Parent + 1
                    cmd.Parameters.AddWithValue("@Parent_Account", cmbParentAccount.SelectedValue.ToString());
                    cmd.Parameters.AddWithValue("@LevelNo", clsChart.GetParentLevel(cmbParentAccount.SelectedValue.ToString()).ToString());
                }
                cmd.Parameters.AddWithValue("@Loan_Related", chckLoanRelated.Checked);
                cmd.Parameters.AddWithValue("@active", chckActive.Checked);
                cmd.ExecuteNonQuery();            

                //Load Treeview
                _acountsTb.Rows.Add(txtCode.Text, txtDescription.Text,cmbAccountGroup.Text,cmbAccountType.Text,cmbParentAccount.Text, (chckLoanRelated.Checked ? 1 : 0), (chckActive.Checked ? 1 : 0));
                TreeNode tn = new TreeNode();
                tn.Text = txtCode.Text + " - " + txtDescription.Text;
                tn.Name = txtCode.Text;
                tn.Tag = _acountsTb.Rows.Count - 1;
                
                

                if (cmbParentAccount.Text == "")
                {
                    //If parent is null add to last 
                    treeView1.Nodes.Add(tn);
                }
                else
                {
                    //if have parent then insert into parent child

                    //focus first on node
                    //then get selected node
                    foreach (TreeNode node in GetAllNodes(treeView1))
                    {
                        if (node.Text == cmbParentAccount.Text)
                        {
                            treeView1.SelectedNode = node;
                            treeView1.Focus();
                            _selectedNode.Nodes.Add(tn);
                            treeView1.ExpandAll();
                        }
                    }
                }

                //MessageBox
                Alert.show("Successfully Added", Alert.AlertType.success);

                //load Parent for real time adding
                loadParentNo(cmbParentAccount, "chart_of_accounts", "Account"); //Parent
                global.loadComboBoxDistinct(cmbAccountGroup, "chart_of_accounts", "account_group"); //Account Group

                //if No error or all validation meet
                btnNew.Text = "NEW";

                //enable button
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;

                //Cancel to Close
                btnClose.Text = "CLOSE";

                //clear
                clearFields();

                //Disable all fields again
                //After Saving
                disabledFields()
;            }
            else
            {
                //From NEW to SAVE 
                //Clear Fields and Rename Button to SAVE
                btnNew.Text = "SAVE";
                clearFields();

                //Change button Close to Cancel
                btnClose.Text = "CANCEL";

                //Disable EDIT and DELETE Button
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;

                loadParentNo(cmbParentAccount, "chart_of_accounts", "Account");
            }
        }


        private void panel1_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(ChartOfAccounts))
                {
                    form.Activate();
                    return;
                }
            }

            ChartOfAccounts frm = new ChartOfAccounts();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
            frm.treeView1.ExpandAll();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            
            //check if theres a data to be updated
            if (txtCode.Text == "")
            {
                Alert.show("Please select data you want to edit!", Alert.AlertType.warning);
                return;
            }

            //change BtnEdit to UPDATE
            if(btnEdit.Text == "EDIT")
            {
                //Change Button Text
                btnEdit.Text = "UPDATE";
                //enable fields to update except account code
                enabledFields(); //Enable all
                txtCode.Enabled = false; //This will disable the txtcode

                //button close to cancel
                btnClose.Text = "CANCEL";

                //disable delete and new button
                btnNew.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                //Edit Code goes here
                //Nikko Melgar

                //Check if all fields have value
                if (checkFields() == true)
                {
                    Alert.show("All fields with (*) are required", Alert.AlertType.warning);
                    return;
                }

                //check for duplicate entry
                //Call Stored Procedure
                SqlConnection con = new SqlConnection();
                Global global = new Global();
                global.connection(con);


                SqlDataAdapter adapterFIlter = new SqlDataAdapter("SELECT * from chart_of_accounts where account_Description = '" + txtDescription.Text + "' and account_code <> +'"+ txtCode.Text +"'", con);
                DataTable dtFilter = new DataTable();
                adapterFIlter.Fill(dtFilter);

                if (dtFilter.Rows.Count > 0)
                {
                    Alert.show("Account Description Already Exist", Alert.AlertType.error);
                    return;
                }

                
                

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_UpdateChartofAccounts";
                cmd.Parameters.AddWithValue("@Account_Code", txtCode.Text);
                cmd.Parameters.AddWithValue("@Account_Description", txtDescription.Text);
                cmd.Parameters.AddWithValue("@Account_Group", cmbAccountGroup.Text);
                cmd.Parameters.AddWithValue("@Account_Type", cmbAccountType.Text);

                //Check if Parent account is null
                if (cmbParentAccount.Text == "")
                {
                    cmd.Parameters.AddWithValue("@Parent_Account", "0");
                    cmd.Parameters.AddWithValue("@LevelNo", "0");
                }
                else
                {
                    //Get Parent And Level of Parent + 1
                    cmd.Parameters.AddWithValue("@Parent_Account", cmbParentAccount.SelectedValue.ToString());
                    cmd.Parameters.AddWithValue("@LevelNo", clsChart.GetParentLevel(cmbParentAccount.SelectedValue.ToString()).ToString());
                }
                cmd.Parameters.AddWithValue("@Loan_Related", chckLoanRelated.Checked);
                cmd.Parameters.AddWithValue("@active", chckActive.Checked);
                cmd.ExecuteNonQuery();


                //Load Treeview
                _acountsTb.Rows.Add(txtCode.Text, txtDescription.Text, cmbAccountGroup.Text, cmbAccountType.Text, cmbParentAccount.Text, (chckLoanRelated.Checked ? 1 : 0), (chckActive.Checked ? 1 : 0));
                TreeNode tn = new TreeNode();
                tn.Text = txtCode.Text + " - " + txtDescription.Text;
                tn.Name = txtCode.Text;
                tn.Tag = _acountsTb.Rows.Count - 1;

                
                if (cmbParentAccount.Text == "")
                {
                    //If parent is null add to last 
                    treeView1.Nodes.Add(tn);
                }
                else
                {
                    //_selectedNode.Text = tn.Text;
                    //treeView1.Focus();
                    //treeView1.ExpandAll();
                    string container = cmbParentAccount.Text;
                    _selectedNode.Remove();


                    //Re-Arrange in Treeview
                    foreach (TreeNode node in GetAllNodes(treeView1))
                    {
                        if (node.Text == container)
                        {
                            treeView1.SelectedNode = node;
                            treeView1.Focus();
                            _selectedNode.Nodes.Add(tn);
                            treeView1.ExpandAll();
                        }
                    }
                    
                    
                }

                //MessageBox
                Alert.show("Successfully Updated", Alert.AlertType.success);

                //load Parent for real time adding
                loadParentNo(cmbParentAccount, "chart_of_accounts", "Account"); //Parent
                global.loadComboBoxDistinct(cmbAccountGroup, "chart_of_accounts", "account_group"); //Account Group

                //Disable all fields again and clear revert all buttons
                clearFields();
                disabledFields();

                btnEdit.Text = "EDIT";

                btnNew.Enabled = true;
                btnDelete.Enabled = true;
                btnClose.Text = "CLOSE";

            } //End Button Update function

            


        }


        IEnumerable<TreeNode> GetAllNodes(TreeView treeview)
        {
            LinkedList<TreeNodeCollection> collections =
                new LinkedList<TreeNodeCollection>();

            collections.AddLast(treeview.Nodes);
            while (collections.Count > 0)
            {
                foreach (TreeNode cur in collections.First.Value)
                {
                    collections.AddLast(cur.Nodes);
                    yield return cur;
                }
                collections.RemoveFirst();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(txtCode.Text == "")
            {
                //No Data to be deleted
                Alert.show("Please select Account you want to delete!", Alert.AlertType.error);
                return;
            }

            //ASK FIRST IF YOU WANT TO TAGGED AS INACTIVE[WIll not reflect on table and in syste]
            string msg = Environment.NewLine + "Are you sure you want to delete this Account?";
            DialogResult result = MessageBox.Show(this, msg, "PLDT Credit Cooperative", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //Delete or Tagged as Inactive goes here
                //CONNECTION TO SQL SERVER AND STORED PROCEDURE
                SqlConnection con = new SqlConnection();
                global.connection(con);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_InactiveChartOfAccounts";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Account_Code", txtCode.Text);
                cmd.ExecuteNonQuery();

                //Message
                Alert.show("Company Successfully Deleted", Alert.AlertType.success);

                //Remove from treeview
                _selectedNode.Remove();

            }
            else
            {
                clearFields();
                return;
            }


        }
    }
}
