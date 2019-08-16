using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApplication2.Classes
{
    class clsProvisionalReceipt
    {
        Global global = new Global();

        public void DisplayCheckProvisionalReceipt(DataGridView dgv, DateTimePicker dtFrom, DateTimePicker dtTo, Label lblCnt)
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetProvisionalReceipt";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DateFrom", dtFrom.Text);
                cmd.Parameters.AddWithValue("@DateTo", dtTo.Text);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                if(ds.Tables[0].Rows.Count > 0)
                {
                    dgv.Rows.Clear();
                    dgv.Rows.Add(ds.Tables[0].Rows.Count);

                    for(int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        dgv.Rows[x].Cells[0].Value = ds.Tables[0].Rows[x]["Or_No"];
                        dgv.Rows[x].Cells[1].Value = ds.Tables[0].Rows[x]["Check_No"];
                        dgv.Rows[x].Cells[2].Value = ds.Tables[0].Rows[x]["Name"];
                        dgv.Rows[x].Cells[3].Value = ds.Tables[0].Rows[x]["Bank"];
                    }

                    lblCnt.Text = ds.Tables[0].Rows.Count.ToString();
                }
                else
                {
                    Alert.show("No record(s) found.", Alert.AlertType.error);
                    return;
                }
            }
        }

        public void moveToBounceGrid(DataGridView dgvProvisional,DataGridView dgvBounce,Label lblCnt,Label lblBounceCnt)
        {
            if(dgvProvisional.SelectedRows.Count > 0)
            {
                //Add to bounce grid
                dgvBounce.Rows.Add(
                    dgvProvisional.SelectedRows[0].Cells[0].Value.ToString(),
                    dgvProvisional.SelectedRows[0].Cells[1].Value.ToString(),
                    dgvProvisional.SelectedRows[0].Cells[2].Value.ToString(),
                    dgvProvisional.SelectedRows[0].Cells[3].Value.ToString()
                    );

                //Remove from provisional
                foreach (DataGridViewRow cell in dgvProvisional.SelectedRows)
                {
                    int row = dgvProvisional.CurrentRow.Index;
                    dgvProvisional.Rows.RemoveAt(row);
                }

                lblCnt.Text = Convert.ToString(Convert.ToInt32(lblCnt.Text) - 1);
                lblBounceCnt.Text = Convert.ToString(Convert.ToInt32(lblBounceCnt.Text) + 1);
            }
            else
            {
                Alert.show("Please select provisional receipt first.", Alert.AlertType.error);
                return;
            }
        }

        public void moveToProvisionalGrid(DataGridView dgvProvisional, DataGridView dgvBounce, Label lblCnt, Label lblBounceCnt)
        {
            if (dgvBounce.SelectedRows.Count > 0)
            {
                //Add to bounce grid
                dgvProvisional.Rows.Add(
                    dgvBounce.SelectedRows[0].Cells[0].Value.ToString(),
                    dgvBounce.SelectedRows[0].Cells[1].Value.ToString(),
                    dgvBounce.SelectedRows[0].Cells[2].Value.ToString(),
                    dgvBounce.SelectedRows[0].Cells[3].Value.ToString()
                    );

                //Remove from provisional
                foreach (DataGridViewRow cell in dgvBounce.SelectedRows)
                {
                    int row = dgvBounce.CurrentRow.Index;
                    dgvBounce.Rows.RemoveAt(row);
                }

                lblCnt.Text = Convert.ToString(Convert.ToInt32(lblCnt.Text) + 1);
                lblBounceCnt.Text = Convert.ToString(Convert.ToInt32(lblBounceCnt.Text) - 1);
            }
            else
            {
                Alert.show("Please select provisional receipt first.", Alert.AlertType.error);
                return;
            }
        }
        
    }
}
