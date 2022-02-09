using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text.RegularExpressions; // for Regular expression
using System.Drawing; // for change of color

using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace SitConnect
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btn_checkPassword_Click(object sender, EventArgs e)
        {
            int scores = checkPassword(tb_Password.Text);
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }
            lbl_pwdchecker2.Text = "  Password Complexity : " + status;
            if (scores < 4)
            {
                lbl_pwdchecker2.ForeColor = Color.Red;
                return;
            }
            lbl_pwdchecker2.ForeColor = Color.Green;

        }
        private int checkPassword(string password)
        {
            int score = 0;
            if (password.Length < 12)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[1-9]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                score++;
            }
            return score;
        }
        protected void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @FirstName, @LastName, @CreditCard, @PasswordHash, @PasswordSalt, @EmailVerified, @DateTimeRegistered, @IV, @Key, @Img, @FailLogin, @LoginDate, @DOB)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", tb_Email.Text.Trim());
                            cmd.Parameters.AddWithValue("@FirstName", tb_FirstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", tb_LastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@CreditCard", encryptData(tb_CreditCard.Text.Trim()));
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@DOB", DOB.SelectedDate);
                            cmd.Parameters.AddWithValue("@DateTimeRegistered", DateTime.Now);
                            cmd.Parameters.AddWithValue("@EmailVerified", DBNull.Value);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(IV));
                            using (Stream fs = PhotoUpload.PostedFile.InputStream)
                            {
                                using (BinaryReader br = new BinaryReader(fs))
                                {
                                    byte[] bytes = br.ReadBytes((Int32)fs.Length);
                                    cmd.Parameters.AddWithValue("@Img", bytes);
                                }
                            }
                            cmd.Parameters.AddWithValue("@FailLogin", 0);
                            cmd.Parameters.AddWithValue("@LoginDate", DateTime.Now);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
                // lb_error1.Text = ex.ToString();
            }
            finally
            { }
        }
        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                // throw new Exception(ex.ToString());
                lb_error1.Text = ex.ToString();

            }
            finally { }
            return cipherText;
        }
        protected bool CheckEmailExist(string UserEmail)
        {
            string s = "";
            bool check = false;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Email FROM [Account]";
            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if ((string)reader["Email"] != null)
                        {
                            if (reader["Email"] != DBNull.Value)
                            {
                                s = (string)reader["Email"];
                                if (s == UserEmail)
                                {
                                    check = false;
                                }
                                else
                                {
                                    check = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return check;
        }
        
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Register Page: Validating empty fields");
            int scores = checkPassword(tb_Password.Text);
            System.Diagnostics.Debug.WriteLine("Register Page: Password Score " + scores);
            // System.Diagnostics.Debug.WriteLine("Register Page: PhotoUpload " + PhotoUpload.FileName.ToString().ToLower());

            if (tb_FirstName.Text.ToString() == "")
            {
                validation.Text = " First Name Field cannot be empty ";
                validation.ForeColor = Color.Red;
                return;
                
            }else if (DOB.SelectedDate.ToString() == "1/1/0001 12:00:00 am")
            {
                validation.Text = " Please select a Birth Date ";
                validation.ForeColor = Color.Red;
                return;
            }
            else if (tb_LastName.Text.ToString() == "")
            {
                validation.Text = " Last Name Field cannot be empty ";
                validation.ForeColor = Color.Red;
                return;
            }
            else if (tb_Email.Text.ToString() == "" || CheckEmailExist(tb_Email.Text.ToString().Trim()) == false)
            {
                validation.Text = " Email Field Invalid or Already Exist ";
                validation.ForeColor = Color.Red;
                return;
            }
            else if ((tb_Password.Text.ToString() == "") || (scores < 4))
            {
                validation.Text = " Password Field Invalid ";
                validation.ForeColor = Color.Red;
                return;
            }
            else if ((tb_CreditCard.Text.ToString() == "") || (tb_CreditCard.Text.ToString().Length != 16) || (Regex.IsMatch(tb_CreditCard.Text, "[^1-9]")))
            {
                validation.Text = " Credit Card Field invalid ";
                validation.ForeColor = Color.Red;
                return;
            }
            else if (PhotoUpload.HasFile == false || (PhotoUpload.FileName.ToString().ToLower().EndsWith(".png") == false && PhotoUpload.FileName.ToString().ToLower().EndsWith(".jpg") == false))
            {
                validation.Text = " Upload a photo (Only .png and .jpg are accepted)";
                validation.ForeColor = Color.Red;
                return;
            }
            else {
                System.Diagnostics.Debug.WriteLine("Register Page: Validation PASSED");
                //string pwd = get value from your Textbox
                string pwd = tb_Password.Text.ToString().Trim(); ;
                //Generate random "salt"
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];
                //Fills array of bytes with a cryptographically strong sequence of random values.
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);
                SHA512Managed hashing = new SHA512Managed();
                string pwdWithSalt = pwd + salt;
                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                finalHash = Convert.ToBase64String(hashWithSalt);
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;
                createAccount();
                Response.Redirect("Login.aspx", false);

            }
            



        }
    }
}