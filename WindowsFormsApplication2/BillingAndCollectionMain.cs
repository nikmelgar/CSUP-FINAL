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
    public partial class BillingAndCollectionMain : Form
    {
        public BillingAndCollectionMain()
        {
            InitializeComponent();
        }

        Global global = new Global();
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        private bool m_firstClick = false;
        private Point m_firstClickLoc;
        private void BillingAndCollectionMain_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_returnBillingDashboard";
                cmd.CommandType = CommandType.StoredProcedure;
                
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                //chart1.DataSource = dt;

                SqlCommand cmdCollection = new SqlCommand();
                cmdCollection.Connection = con;
                cmdCollection.CommandText = "sp_returnCollectionDashboard";
                cmdCollection.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adapterCollect = new SqlDataAdapter(cmdCollection);
                DataSet ds = new DataSet();
                adapterCollect.Fill(ds);

                ////Billing
                //chart1.Series["Billing"].XValueMember = dt.Rows[0].ItemArray[0].ToString();
                //chart1.Series["Billing"].YValueMembers = dt.Rows[0].ItemArray[1].ToString();

                //chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;
                //

                ////Collection
                //chart1.Series["Collection"].XValueMember = ds.Tables[0].Columns[0].ToString();
                //chart1.Series["Collection"].YValueMembers = ds.Tables[0].Columns[1].ToString();

                //chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;
                //chart1.Series["Collection"].ToolTip = "#VALY";


                try
                {
                    for (int xb = 0; xb < dt.Rows.Count; xb++)
                    {
                        chart1.Series["Billing"].Points.AddXY(dt.Rows[xb].ItemArray[0].ToString(), Convert.ToDecimal(dt.Rows[xb].ItemArray[1].ToString()));
                    }
                    chart1.Series["Billing"].ToolTip = "#VALY";

                    for (int xc = 0; xc < ds.Tables[0].Rows.Count; xc++)
                    {
                        chart1.Series["Collection"].Points.AddXY(ds.Tables[0].Rows[xc]["Deduction_Code"].ToString(), Convert.ToDecimal(ds.Tables[0].Rows[xc]["TotalDueAmount"].ToString()));
                    }
                    chart1.Series["Collection"].ToolTip = "#VALY";

                    chart1.AlignDataPointsByAxisLabel();
                }
                catch
                {

                }
               
            }
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

        private void panel1_Click(object sender, EventArgs e)
        {
            //Bring To Front (Child Form)
            //Nikko Melgar
            foreach (Form form in Application.OpenForms)
            {


                if (form.GetType() == typeof(BillingAndCollectionMain))
                {
                    form.Activate();
                    return;
                }
            }

            BillingAndCollectionMain frm = new BillingAndCollectionMain();
            MainForm main = new MainForm();
            frm.Show();
            frm.MdiParent = main;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Billing billing = new Billing();
            billing.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panelAction_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            CollectionForm collection = new CollectionForm();
            collection.ShowDialog();
        }
    }
}
