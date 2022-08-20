using Microsoft.Win32;
using System.Collections.Specialized;

namespace charmap
{
    class Reg
    {
        public static string keyName = "mv94TPLUyphNlvlthltw";
        public static string userKey = "V7Z1gKGBdG3MspitbgDi";
        public static string passKey = "HeGDVGW2nEi48AWnU8Dx";

        public static NameValueCollection getUserData()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName);

            if (key == null) return null;

            string username = (string)key.GetValue(userKey);
            string password = (string)key.GetValue(passKey);

            if (username == null || password == null) return null;

            NameValueCollection values = new NameValueCollection();

            values.Add("username", username);
            values.Add("password", password);

            return values;
        }

        public static void createUserData(string username, string password)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("mv94TPLUyphNlvlthltw");

            key.SetValue(userKey, username);
            key.SetValue(passKey, password);

            key.Close();
        }
    }
}
