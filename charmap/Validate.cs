using VectorAuth;
using System.Reflection;

namespace charmap
{
    [Obfuscation(Feature = "virtualization", Exclude = false)]
    internal class auth_instance
    {
        internal static API api = new API(
                "18",
                "EU8oCF1smw5Y7jK7SwPYmNSwaeghrle0zg+RTjxgQCM=",
                "hBd+SKHdBsuGgsxDTWJAvg=="
            );
    }

    public class Validate
    {
        public static bool Login(string username, string password)
        {
            if (auth_instance.api.Login(username, password))
            {
                Values.angles = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewAngles>(auth_instance.api.GrabVariable("viewangles"));

                (new Main()).Show();

                return true;
            } else
            {

                return false;
            }
        }
    }
}
