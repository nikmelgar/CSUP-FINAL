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
            con = new SqlConnection();
            global.connection(con);

            adapter = new SqlDataAdapter("SELECT * FROM BILLING", con);
            dt = new DataTable();
            adapter.Fill(dt);

            chart1.DataSource = dt;

            //Billing
            chart1.Series["Billing"].XValueMember = "Deduction_Code";
            chart1.Series["Billing"].YValueMembers = "TotalDueAmount";

            chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;
            chart1.Series["Billing"].ToolTip = "#VALY";

            //Collection
            chart1.Series["Collection"].XValueMember = "Deduction_Code";
            chart1.Series["Collection"].YValueMembers = "TotalDueAmount";

            chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;
            chart1.Series["Collection"].ToolTip = "#VALY";
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
    }
}
