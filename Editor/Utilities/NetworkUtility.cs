using System.Net;

namespace RemoteFileExplorer.Editor
{
    public class NetworkUtility
    {
        public static string GetLocalHost()
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach(var address in addresses)
            {
                if(address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }
            }
            return null;
        }
    }
}