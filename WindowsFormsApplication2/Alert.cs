using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    public partial class Alert : Form
    {
        public Alert(string Message, AlertType type)
        {
            InitializeComponent();

            label3.Text = Message;
            switch (type)
            {
                case AlertType.success:
                    this.BackColor = Color.FromArgb(51, 102,51);
                    pictureBox1.Image = imageList1.Images[1];
                    break;
                case AlertType.warning:
                    this.BackColor = Color.FromArgb(210, 141, 66);
                    pictureBox1.Image = imageList1.Images[2];
                    break;
                case AlertType.error:
                    this.BackColor = Color.FromArgb(207, 54, 54);
                    pictureBox1.Image = imageList1.Images[0];
                    break;
            }
        }

        public static void show(string Message, AlertType type)
        {
            new WindowsFormsApplication2.Alert(Message, type).Show();

        }

        public enum AlertType
        {
            success , warning, error
        }

        private void Alert_Load(object sender, EventArgs e)
        {
            //Bottom of Screen
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);
        }


        int interval = 0;


        private bool m_firstClick = false;
        private Point m_firstClickLoc;


        private void label1_Click(object sender, EventArgs e)
        {
            timer3.Start();
        }

        private void Alert_MouseMove(object sender, MouseEventArgs e)
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

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (this.Opacity > 0)
            {
                this.Opacity -= 0.1;
            }
            else
            {
                this.Close();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (this.Top < 60)
            {
                this.Top += interval;
                interval += 1;
            }
            else
            {
                timer2.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
