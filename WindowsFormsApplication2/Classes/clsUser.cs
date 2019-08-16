using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace WindowsFormsApplication2.Classes
{
    class clsUser
    {

        //Information 

        public static string Username { get; set; }
        public static string firstName { get; set; }
        public static string middleName { get; set; }
        public static string lastName{ get; set; }

        public static string department { get; set; }
        public static string role { get; set; }


        Global global = new Global();

        public static string Encrypt(string encryptString)
        {
            string EncryptionKey = "15nik@1234xxxxxxxxxxtttttuuuuuiiiiio";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "15nik@1234xxxxxxxxxxtttttuuuuuiiiiio";  
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public string returnDepartmentDescription()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Department FROM Department WHERE DepartmentID = '"+ department +"'", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                SqlDataAdapter adapterRole = new SqlDataAdapter("SELECT RoleDescription FROM Role WHERE RoleID = '"+ role +"'", con);
                DataTable dt2 = new DataTable();
                adapterRole.Fill(dt2);

                return dt.Rows[0].ItemArray[0].ToString() + Environment.NewLine + dt2.Rows[0].ItemArray[0].ToString();
            }
        }

        public void updateUserLogin()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                string MachineName4 = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
                IPAddress ip = Dns.GetHostAddresses(Dns.GetHostName()).Where(address => address.AddressFamily == AddressFamily.InterNetwork).First();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_GetLoginInformation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@LoggedAtName", MachineName4);
                cmd.Parameters.AddWithValue("@LoggedAtIPAddress", ip.ToString());
                cmd.ExecuteNonQuery();
            }
        }

        public void removeUserLogin()
        {
            using (SqlConnection con = new SqlConnection(global.connectString()))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "sp_RemoveLoginInformation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
