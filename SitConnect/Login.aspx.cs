using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Data;
using System.Drawing;

namespace SitConnect
{
    // sitekey = 6LeI1kAeAAAAAFoWmCnKHlgwKeT9rCUnigt-46Cz
    // secretkey = 6LeI1kAeAAAAAHM9ubZ3L6Rj0ucz_sixe_3brB7w
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }
        public bool ValidateCaptcha()
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(" https://www.google.com/recaptcha/api/siteverify?secret=6LeI1kAeAAAAAHM9ubZ3L6Rj0ucz_sixe_3brB7w &response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        // lbl_gScore.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        
        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                           
                        if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { 
                connection.Close(); 
            }
            return h;
        }

        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
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
            return s;
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
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }
        protected int findFailAttemptCount(string userid)
        {
            int s = 0;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select FailLogin FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if ((int)reader["FailLogin"] != 0)
                        {
                            if (reader["FailLogin"] != DBNull.Value)
                            {
                                s = (int)reader["FailLogin"];
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
            return s;
        }

        protected DateTime findFailLoginDate(string userid)
        {
            DateTime s = DateTime.Now;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select LoginDate FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if ((DateTime)reader["LoginDate"] != null)
                        {
                            if (reader["LoginDate"] != DBNull.Value)
                            {
                                s = (DateTime)reader["LoginDate"];
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
            return s;
        }
        protected void editFailCount(string userid, int failcount)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET FailLogin = @FailLogin WHERE Email = @USERID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FailLogin", failcount);
                            cmd.Parameters.AddWithValue("@USERID", userid);
                            
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
            }
            finally
            { }
        }
        protected void editFailCountAndFailDate(string userid, int failcount, DateTime loginDate)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET FailLogin = @FailLogin, LoginDate = @LoginDate WHERE Email = @USERID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FailLogin", failcount);
                            cmd.Parameters.AddWithValue("@LoginDate", loginDate);
                            cmd.Parameters.AddWithValue("@USERID", userid);

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
            }
            finally
            { }
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
                                    break;
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

        protected void LoginMe(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Login Page: RUNNING LoginMe");
            if (tb_userid.Text.ToString() == "")
            {
                validation.Text = " Email Field Invalid ";
                validation.ForeColor = Color.Red;
                return;
            }
            else if ((tb_pwd.Text.ToString() == ""))
            {
                validation.Text = " Password Field Invalid ";
                validation.ForeColor = Color.Red;
                return;
            }else if (CheckEmailExist(tb_userid.Text.ToString().Trim()) == true)
            {
                System.Diagnostics.Debug.WriteLine("Login Page: Email Doesn't Exist");
                validation.Text = "Userid or password is not valid. Please try again.";
                validation.ForeColor = Color.Red;
                return;
            }
            string pwd = tb_pwd.Text.ToString().Trim();
            string email = tb_userid.Text.ToString().Trim();

            var failCount = findFailAttemptCount(email);
            System.Diagnostics.Debug.WriteLine("Login Page: Fail Counter: " + failCount);

            // if failcount = 10 & lockout date & today date less than xxx mins --> remain lockout
            // if failcount = 10 & lockout date & today date more than xxx mins --> end lockout - failcount set to 0
            if (failCount == 10)
            {
                System.Diagnostics.Debug.WriteLine("Login Page: Account is Locked");
                var FailLoginDateTime = findFailLoginDate(email);
                System.Diagnostics.Debug.WriteLine("Login Page: Fail Login DateTime: " + FailLoginDateTime);
                DateTime NowDateTime = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("Login Page: Now DateTime : " + NowDateTime);
                // Assuming account lockout for 5mins
                DateTime AccountRecoveryDateTime = FailLoginDateTime.AddMinutes(5);
                System.Diagnostics.Debug.WriteLine("Login Page: Account Recovery DateTime : " + AccountRecoveryDateTime);
                if (AccountRecoveryDateTime > NowDateTime)
                {
                    System.Diagnostics.Debug.WriteLine("Login Page: Account Recovery False");
                    validation.Text = "Due to many Failed Login Attempts, this account has been locked out.";
                    validation.ForeColor = Color.Red;
                    return;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Login Page: Account Recovery True");
                    // set failcount to 0
                    failCount = 0;
                    // update failcount in DB
                    editFailCount(email, failCount);
                    System.Diagnostics.Debug.WriteLine("Login Page: Successfully reset FailCount for Email: " + email + ", failCount: " + failCount);
                }

            }

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);
            System.Diagnostics.Debug.WriteLine("Login Page: dbHash " + dbHash);
            System.Diagnostics.Debug.WriteLine("Login Page: dbSalt " + dbSalt);

            if (ValidateCaptcha())
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = pwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);

                    if (userHash.Equals(dbHash))
                    {
                        // sucessfully logged in
                        System.Diagnostics.Debug.WriteLine("Login Page: Login Successful");


                        // set failcount to 0
                        failCount = 0;
                        // update failcount in DB
                        editFailCount(email, failCount);
                        System.Diagnostics.Debug.WriteLine("Login Page: Successfully RESET FailCount for Email: " + email + ", failCount: " + failCount);

                        System.Diagnostics.Debug.WriteLine("Login Page: Create Session");
                        Session["LoggedIn"] = tb_userid.Text.Trim();

                        // creates a new GUID and save into the session
                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;

                        // now create a new cookie with this guid value
                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                        Response.Redirect("StaffHomePage.aspx", false);

                    }
                    else
                    {
                        // if failcount < 3 --> end
                        // if failcount = 3 --> lockout && lockout date && change failcount to 10
                        // update failcount and lockout date to DB
                        System.Diagnostics.Debug.WriteLine("Login Page: Password Wrong");
                        failCount += 1;
                        System.Diagnostics.Debug.WriteLine("Login Page: Fail Counter: " + failCount);
                        if (failCount < 3)
                        {
                            editFailCount(email, failCount);
                            System.Diagnostics.Debug.WriteLine("Login Page: Successfully UPDATED FailCount for Email: " + email + ", failCount: " + failCount);
                            var errorMsg = "Userid or password is not valid. Please try again.";
                            validation.Text = errorMsg;
                            validation.ForeColor = Color.Red;

                            return;
                        }else if(failCount == 3){
                            System.Diagnostics.Debug.WriteLine("Login Page: Account is LOCKED");
                            // set failcount to 10
                            failCount = 10;
                            DateTime newFailLoginDate = DateTime.Now;
                            // update failcount in DB
                            editFailCountAndFailDate(email, failCount, newFailLoginDate);
                            System.Diagnostics.Debug.WriteLine("Login Page: Successfully UPDATED FailCount for Email: " + email + ", failCount: " + failCount + ", newFailLoginDate: " + newFailLoginDate);
                            var errorMsg = "Userid or password is not valid. Please try again.";
                            validation.Text = errorMsg;
                            validation.ForeColor = Color.Red;
                            return;
                        }

                    }
                }
            }
        }
            
        
        }


    }
