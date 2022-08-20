using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Windows;

namespace VectorAuth
{
    public class API
    {
        public User user;

        public string url = "https://vector.rip/api/auth/";

        public string id;
        public string key;
        public string salt;

        public API(string _id, string _key, string _salt)
        {
            user = new User();

            id = _id;
            key = _key;
            salt = _salt;
        }

        public bool Login(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                ShowMessage("Please enter a username and password!", MessageBoxImage.Error);

                return false;
            }

            NameValueCollection values = new NameValueCollection();

            SHA256 sha = SHA256Managed.Create();

            byte[] _key = Convert.FromBase64String(key);
            byte[] _iv = Convert.FromBase64String(salt);

            byte[] sessioniv = new byte[16];
            byte[] sessionkey = new byte[32];

            var rng = new RNGCryptoServiceProvider();

            rng.GetBytes(sessioniv);
            rng.GetBytes(sessionkey);

            string hwid = Security.GetHWID();

            values["username"] = Security.Encrypt(username, _key, _iv);
            values["password"] = Security.Encrypt(password, _key, _iv);
            values["iv"] = Security.Encrypt(Convert.ToBase64String(sessioniv), _key, _iv);
            values["key"] = Security.Encrypt(Convert.ToBase64String(sessionkey), _key, _iv);
            values["hwid"] = Security.Encrypt(hwid, _key, _iv);
            values["ownerid"] = id;
            values["action"] = Security.Encrypt("login", _key, _iv);

            string response = null;

            Grab(out response, values);

            if (response == null)
            {
                ShowMessage("Invalid server response!", MessageBoxImage.Error);

                return false;
            }
            else
            {
                string decrypted = Security.Decrypt(response, sessionkey, sessioniv);

                try
                {
                    dynamic json = JsonConvert.DeserializeObject(decrypted);

                    switch ((string)json.result)
                    {
                        case "success":
                            {
                                user = new User();

                                user.username = username;
                                user.password = password;
                                user.hwid = hwid;
                                user.expiry = (string)json.expiry;

                                return true;
                            }
                        case "missing credentials":
                            {
                                ShowMessage("Missing credentials.", MessageBoxImage.Error);

                                return false;
                            }
                        case "banned":
                            {
                                ShowMessage("Your account is banned!", MessageBoxImage.Error);

                                return false;
                            }
                        case "expired":
                            {
                                ShowMessage("Subscription expired on " + (string)json.expiry + "\n\nContact Dufresne#8634 to extend your subscription.", MessageBoxImage.Error);

                                return false;
                            }
                        case "invalid hwid":
                            {
                                ShowMessage("Invalid hardware ID.", MessageBoxImage.Error);

                                return false;
                            }
                        case "invalid credentials":
                            {
                                ShowMessage("Invalid login credentials.", MessageBoxImage.Error);

                                return false;
                            }
                    }
                }
                catch
                {
                    ShowMessage("Invalid server response!", MessageBoxImage.Error);

                    return false;
                }
            }

            return true;
        }

        public byte[] LoadAssembly(string name)
        {
            byte[] result = null;

            if (!user.IsLoggedIn()) return result;

            NameValueCollection values = new NameValueCollection();

            SHA256 sha = SHA256Managed.Create();

            byte[] _key = Convert.FromBase64String(key);
            byte[] _iv = Convert.FromBase64String(salt);

            byte[] sessioniv = new byte[16];
            byte[] sessionkey = new byte[32];

            var rng = new RNGCryptoServiceProvider();

            rng.GetBytes(sessioniv);
            rng.GetBytes(sessionkey);

            values["username"] = Security.Encrypt(user.username, _key, _iv);
            values["password"] = Security.Encrypt(user.password, _key, _iv);
            values["iv"] = Security.Encrypt(Convert.ToBase64String(sessioniv), _key, _iv);
            values["key"] = Security.Encrypt(Convert.ToBase64String(sessionkey), _key, _iv);
            values["hwid"] = Security.Encrypt(user.hwid, _key, _iv);
            values["ownerid"] = id;
            values["action"] = Security.Encrypt("loadAssembly", _key, _iv);
            values["assembly"] = Security.Encrypt(name, _key, _iv);

            string response;

            Grab(out response, values);

            return Security.DecryptAssembly(response, sessionkey, sessioniv);
        }

        public void BanUser()
        {
            if (!user.IsLoggedIn()) System.Diagnostics.Process.GetCurrentProcess().Kill();

            NameValueCollection values = new NameValueCollection();

            byte[] _key = Convert.FromBase64String(key);
            byte[] _iv = Convert.FromBase64String(salt);

            values["hwid"] = Security.Encrypt(user.hwid, _key, _iv);
            values["action"] = Security.Encrypt("ban", _key, _iv);

            string response;

            Grab(out response, values);

            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        public string GrabVariable(string name)
        {
            if (!user.IsLoggedIn()) 
                return null;

            NameValueCollection values = new NameValueCollection();

            SHA256 sha = SHA256Managed.Create();

            byte[] _key = Convert.FromBase64String(key);
            byte[] _iv = Convert.FromBase64String(salt);

            byte[] sessioniv = new byte[16];
            byte[] sessionkey = new byte[32];

            var rng = new RNGCryptoServiceProvider();

            rng.GetBytes(sessioniv);
            rng.GetBytes(sessionkey);

            values["username"] = Security.Encrypt(user.username, _key, _iv);
            values["password"] = Security.Encrypt(user.password, _key, _iv);
            values["iv"] = Security.Encrypt(Convert.ToBase64String(sessioniv), _key, _iv);
            values["key"] = Security.Encrypt(Convert.ToBase64String(sessionkey), _key, _iv);
            values["hwid"] = Security.Encrypt(Security.GetHWID(), _key, _iv);
            values["ownerid"] = id;
            values["action"] = Security.Encrypt("grabVar", _key, _iv);
            values["variable"] = Security.Encrypt(name, _key, _iv);

            string response = null;

            Grab(out response, values);

            if (response == null)
            {
                ShowMessage("Invalid server response!", MessageBoxImage.Error);

                return null;
            }
            else
            {
                try
                {
                    return Security.Decrypt(response, sessionkey, sessioniv);
                }
                catch
                {
                    return null;
                }
            }
        }

        private void ShowMessage(string message, MessageBoxImage icon)
        {
            MessageBox.Show(message, "Vector", MessageBoxButton.OK, icon);
        }

        private void Grab(out string response, NameValueCollection values)
        {
            try
            {
                response = Encoding.Default.GetString(new WebClient().UploadValues(url, values));
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);

                response = null;
            }

        }


        private void Grab(out byte[] response, NameValueCollection values)
        {
            try
            {
                response = new WebClient().UploadValues(url, values);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);

                response = null;
            }
        }

    }

    public class User
    {
        public string username = null;
        public string password = null;
        public string hwid = null;
        public string expiry = null;

        public bool IsLoggedIn()
        {
            if (username == null || password == null || hwid == null || expiry == null)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();

                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class Security
    {

        public static string Encrypt(string str, byte[] key, byte[] iv)
        {
            Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;

            aes.Key = key;
            aes.IV = iv;

            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform encryptor = aes.CreateEncryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            byte[] plain = Encoding.ASCII.GetBytes(str).ToArray();

            cryptoStream.Write(plain, 0, plain.Length);
            cryptoStream.FlushFinalBlock();

            byte[] cipher = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(cipher, 0, cipher.Length);
        }

        public static string Decrypt(string str, byte[] key, byte[] iv)
        {
            Aes aes = Aes.Create();

            aes.Mode = CipherMode.CBC;
            aes.Key = key;
            aes.IV = iv;

            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform decryptor = aes.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);

            string text = null;

            try
            {
                byte[] cipher = Convert.FromBase64String(str);

                cryptoStream.Write(cipher, 0, cipher.Length);
                cryptoStream.FlushFinalBlock();

                byte[] plain = memoryStream.ToArray();

                text = Encoding.ASCII.GetString(plain, 0, plain.Length);
            }
            finally
            {
                memoryStream.Close();
                cryptoStream.Close();
            }

            return text;
        }

        public static byte[] DecryptAssembly(string str, byte[] key, byte[] iv)
        {
            Aes aes = Aes.Create();

            aes.Mode = CipherMode.CBC;
            aes.Key = key;
            aes.IV = iv;

            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform decryptor = aes.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);

            byte[] bytes = null;

            try
            {
                byte[] cipher = Convert.FromBase64String(str);

                cryptoStream.Write(cipher, 0, cipher.Length);
                cryptoStream.FlushFinalBlock();

                bytes = memoryStream.ToArray();
            }
            finally
            {
                memoryStream.Close();
                cryptoStream.Close();
            }

            return bytes;
        }

        public static string GetHWID()
        {
            string hwid = RemoveSpecialCharacters(Convert.ToBase64String(Encoding.UTF8.GetBytes(WindowsIdentity.GetCurrent().User.ToString())));

            return hwid;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
