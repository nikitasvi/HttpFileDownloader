using System.Net;
using System.Net.Sockets;

namespace HttpFileDownloader.Core
{
    public static class NetUtil
    {
        public static IPAddress ResolveIpAddress(string dnsNameOrUrl)
        {
            IPAddress resolvedIp;
            if (!IPAddress.TryParse(dnsNameOrUrl, out resolvedIp))
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(dnsNameOrUrl);

                if (hostEntry != null && hostEntry.AddressList != null && hostEntry.AddressList.Length > 0)
                {
                    if (hostEntry.AddressList.Length == 1)
                    {
                        return hostEntry.AddressList[0];
                    }
                    else
                    {
                        foreach (var ipAddress in hostEntry.AddressList)
                        {
                            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return ipAddress;
                            }
                        }
                    }
                }
            }
            else
            {
                return resolvedIp;
            }

            throw new ApplicationException("Can not resolve IP for address: " + dnsNameOrUrl);
        }
    }
}
