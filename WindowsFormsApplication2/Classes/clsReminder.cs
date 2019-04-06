using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using Extensions.DateTime;

namespace WindowsFormsApplication2.Classes
{
    class clsReminder
    {
        Global global = new Global();
        public Boolean pdcDue()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                var mos = DateTime.Today.Month;
                var yr = DateTime.Today.Year;
                var day = DateTime.Today.Day;

                DateTime dtNowFrom;
                DateTime dtNowTo;

                if (day <= 15 && day > 8)
                {
                    dtNowFrom = new DateTime(yr, mos, 09);
                    dtNowTo = new DateTime(yr, mos, 15);
                }
                else if (day <= 31 && day > 24)
                {
                    
                    dtNowFrom = new DateTime(yr, mos, 25);
                    dtNowTo = new DateTime(yr, mos, DateTime.DaysInMonth(yr, mos));
                }
                else
                {
                    dtNowFrom = new DateTime(1991, 11, 15);
                    dtNowTo = new DateTime(1991, 11, 15);
                }


                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetChequeDateDue";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ChequeDateFrom", dtNowFrom);
                cmd.Parameters.AddWithValue("@ChequeDateTo", dtNowTo);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if(dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void getCntPDCdueToday(Button btnNotif,Label lblSpiel,Button btnReminder)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                var mos = DateTime.Today.Month;
                var yr = DateTime.Today.Year;
                var day = DateTime.Today.Day;

                DateTime dtNowFrom;
                DateTime dtNowTo;

                
                if (day <= 15)
                {
                    dtNowFrom = new DateTime(yr, mos, 09);
                    dtNowTo = new DateTime(yr, mos, 15);
                }
                else
                {
                    dtNowFrom = new DateTime(yr, mos, 25);
                    dtNowTo = new DateTime(yr, mos, DateTime.DaysInMonth(yr, mos));
                }


                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetChequeDateDueToday";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ChequeDateFrom", dtNowFrom);
                cmd.Parameters.AddWithValue("@ChequeDateTo", dtNowTo);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);


                if(dt.Rows.Count > 0)
                {
                    btnNotif.Visible = true;
                    btnNotif.Text = dt.Rows.Count.ToString();


                    //SPIEL
                    if (day < 15)
                    {
                        if(dt.Rows.Count == 1)
                        {
                            lblSpiel.Text = "1 PDC will be due on 15th of this month.";
                        }
                        else
                        {
                            lblSpiel.Text = dt.Rows.Count.ToString() + " PDCs will be due on the 15th of this month.";
                        }
                        btnReminder.Visible = false;
                    }
                    else if(day == 15)
                    {
                        if (dt.Rows.Count == 1)
                        {
                            lblSpiel.Text = "1 PDC due today.";
                        }
                        else
                        {
                            lblSpiel.Text = dt.Rows.Count.ToString() + " PDCs due today.";
                        }
                        btnReminder.Visible = true;
                    }
                    else if(day < DateTime.DaysInMonth(yr, mos) && day > 24)
                    {
                        if (dt.Rows.Count == 1)
                        {
                            lblSpiel.Text = "1 PDC will due on the " + DateTime.DaysInMonth(yr, mos) + "th of this month.";
                        }
                        else
                        {
                            lblSpiel.Text = dt.Rows.Count.ToString() + " PDCs will due on the " + DateTime.DaysInMonth(yr, mos) + "th of this month.";
                        }
                        btnReminder.Visible = false;
                    }
                    else if(day == DateTime.DaysInMonth(yr, mos))
                    {
                        if (dt.Rows.Count == 1)
                        {
                            lblSpiel.Text = "1 PDC due today.";
                        }
                        else
                        {
                            lblSpiel.Text = dt.Rows.Count.ToString() + " PDCs due today.";
                        }
                        btnReminder.Visible = true;
                    }
                }
                else
                {
                    btnNotif.Visible = false;
                    btnReminder.Visible = false;
                }
                
               
            }
        }



        
    }
}
