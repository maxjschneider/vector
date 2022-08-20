using System.Reflection;
using VectorAuth;

namespace Loader
{
    public class Validate
    {
        public Validate()
        {
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        internal class auth_instance
        {
            internal static API api = new API(
                "18",
                "EU8oCF1smw5Y7jK7SwPYmNSwaeghrle0zg+RTjxgQCM=",
                "hBd+SKHdBsuGgsxDTWJAvg=="
            );
        }

        // virtualized for security 
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public bool login(string username, string password)
        {
            // self explanatory what's happening here 

            bool result = auth_instance.api.Login(username, password);

            if (result)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
