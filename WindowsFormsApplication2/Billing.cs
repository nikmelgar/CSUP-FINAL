using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class Billing : Form
    {
        public Billing()
        {
            InitializeComponent();
        }

        private bool m_firstClick = false;
        private Point m_firstClickLoc;

        //==================================================
        //            GLOBAL CLASSES AND FORMS
        //==================================================

        Global global = new Global();
        Classes.clsBilling clsBilling = new Classes.clsBilling();
        clsMembershipEntry clsMembershipEntry = new clsMembershipEntry();
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void Billing_Load(object sender, EventArgs e)
        {
            clsMembershipEntry.loadComboBox(cmbCompany, "Company", "Description", "Company_Code");
            clsMembershipEntry.loadComboBox(cmbRank, "Payroll_Group", "Description", "Payroll_Code");
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cmbCompany.SelectedIndex = -1;
            cmbRank.SelectedIndex = -1;
            dtBillDate.Value = DateTime.Today;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clsBilling.generateBillingAccndgCompRank(dgvTempBalances, cmbCompany.SelectedValue.ToString(), cmbRank.SelectedValue.ToString(), dtBillDate);

            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT EmployeeID,Company_Code,Payroll_Code,Deduction_Code,TotalDueAmount FROM Billing WHERE Company_Code = '"+ cmbCompany.SelectedValue.ToString() +"' and Payroll_Code = '"+ cmbRank.SelectedValue.ToString() +"' and BillDate = '"+ dtBillDate.Text +"' ORDER BY userid", con))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
            }
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT EmployeeID,Billdate,sum(TotalDueAmount) as Amount FROM Billing GROUP BY userid, EmployeeID, BillDate", con))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    saveFileDialog1.FileName = "test.xls";

                    // set filters - this can be done in properties as well
                    saveFileDialog1.Filter = "Text files (*.xls)|*.xls|All files (*.*)|*.*";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        FileStream fs1 = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter writer = new StreamWriter(fs1);

                        int cnt = 0;                        

                        while (cnt != dt.Rows.Count)
                        {
                            writer.Write(dt.Rows[cnt].ItemArray[0].ToString());
                            writer.Write("\t");
                            writer.Write(Convert.ToDateTime(dt.Rows[cnt].ItemArray[1].ToString()).ToShortDateString());
                            writer.Write("\t");
                            writer.Write(dt.Rows[cnt].ItemArray[2].ToString());
                            writer.Write("\n");
                            cnt = cnt + 1;
                        }

                        writer.Close();

                        Alert.show("Successfully save to excel.", Alert.AlertType.success);
                    }
                }
            }
        }
    }
}
