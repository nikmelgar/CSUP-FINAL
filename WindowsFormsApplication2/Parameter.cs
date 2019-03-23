using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication2
{
    public partial class Parameter : Form
    {
        public Parameter()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        public static string lastVal { get; set; }

        //==============================================
        //          FORM INCLUDE
        //==============================================
        Global global = new Global();
        Classes.clsParameter clsParam = new Classes.clsParameter();

        //==============================================
        //          DECLARATION
        //==============================================
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;


        private void Parameter_Load(object sender, EventArgs e)
        {
            clsParam.displayParameter(dataGridView1);
            clsParam.loadComboBox(cmbFrm);

            cmbFrm.SelectedIndex = -1;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //Moveable Forms / Screens
            //Nikko Melgar
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if(btnNew.Text == "NEW")
            {
                btnNew.Text = "SAVE";
                btnCancel.Visible = true;

                //remove
                txtDescription.Text = "";
                txtValues.Text = "";
                cmbFrm.SelectedIndex = -1;
            }
            else if(btnNew.Text == "SAVE")
            {
                //For Newly Inserted
                if (cmbFrm.Text == "")
                {
                    Alert.show("Please Select Form First!", Alert.AlertType.error);
                    return;
                }
                
                if(txtDescription.Text == "")
                {
                    Alert.show("Please Fill Description Box!", Alert.AlertType.error);
                    return;
                }

                if(txtValues.Text == "")
                {
                    Alert.show("Please Fill Values Box!", Alert.AlertType.error);
                    return;
                }

                //====================================================================
                //                      END VALIDATION
                //====================================================================


                //====================================================================
                //                      START INSERTING
                //====================================================================
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();

                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_InsertParameter";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@frm", cmbFrm.Text);
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@val", txtValues.Text);
                    cmd.Parameters.AddWithValue("@inserted_by", Classes.clsUser.Username);
                    cmd.ExecuteNonQuery();
                }
                //====================================================================
                //                      RESET FIELDS AND BUTTONS
                //====================================================================
                btnCancel.Visible = false;
                btnNew.Text = "NEW";
                txtValues.Text = "";
                txtDescription.Text = "";
                cmbFrm.SelectedIndex = -1;

                //====================================================================
                //                      Re-Load
                //====================================================================
                clsParam.displayParameter(dataGridView1);

                //====================================================================
                //                      Alert
                //====================================================================

                Alert.show("Successfully Added.", Alert.AlertType.success);
            }
            else
            {
                //For Update

                if (txtValues.Text == "")
                {
                    Alert.show("Please Fill Values Box!", Alert.AlertType.error);
                    return;
                }

                //====================================================================
                //                      START UPDATE
                //====================================================================
                using (SqlConnection con = new SqlConnection(global.connectString()))
                {
                    con.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "sp_UpdateParameter";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@frm", cmbFrm.Text);
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@val", txtValues.Text);
                    cmd.Parameters.AddWithValue("@lastval", lastVal);
                    cmd.Parameters.AddWithValue("@usermodified", Classes.clsUser.Username);

                    cmd.ExecuteNonQuery();
                }
                //====================================================================
                //                      RESET FIELDS AND BUTTONS
                //====================================================================
                btnCancel.Visible = false;
                btnNew.Text = "NEW";
                txtValues.Text = "";
                txtDescription.Text = "";
                cmbFrm.SelectedIndex = -1;

                //====================================================================
                //                      Re-Load
                //====================================================================
                clsParam.displayParameter(dataGridView1);

                //====================================================================
                //                      Alert
                //====================================================================

                Alert.show("Successfully updated.", Alert.AlertType.success);
                lastVal = "";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //reset
            cmbFrm.SelectedIndex = -1;
            txtValues.Text = "";
            txtDescription.Text = "";

            txtDescription.Enabled = true;

            btnCancel.Visible = false;
            btnNew.Text = "NEW";
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                //populate the textbox from specific value of the coordinates of column and row.
                cmbFrm.Text = row.Cells["frm"].Value.ToString();
                txtDescription.Text = row.Cells["Description"].Value.ToString();
                txtValues.Text = row.Cells["val"].Value.ToString();

                lastVal = txtValues.Text;

                btnNew.Text = "UPDATE";
                btnCancel.Visible = true;
                txtDescription.Enabled = false;
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(Parameter))
                {
                    form.Activate();
                    return;
                }
            }

            Parameter frm = new Parameter();
            frm.Show();
            frm.MdiParent = this;
        }
    }
}
