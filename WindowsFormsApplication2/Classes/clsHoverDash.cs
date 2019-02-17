using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApplication2
{
    class clsHoverDash
    {
        public void hoverPanelColor(Panel panelBg,Label lblViewDetails,Color panelColor)
        {
            panelBg.BackColor = panelColor;
            lblViewDetails.ForeColor = Color.White;
        }

        public void leaveHoverPanel(Panel panelBg,Label lblViewDetails,Color panelColor)
        {
            panelBg.BackColor = Color.White;
            lblViewDetails.ForeColor = panelColor;
        }
    }
}
