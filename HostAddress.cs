using System.Net;
using System.Net.Sockets;

namespace BV5Tl0N.HostAddress
{
    public static class HostAddress
    {
        public static bool IsHostValid(string host)
        {
            if (!string.IsNullOrEmpty(host))
                return IsValidIP(host) || IsValidFQDN(host);

            return false;
        }
        public static bool IsPrivate(string host)
        {
            if (!string.IsNullOrEmpty(host))
            {
                if (IsValidIP(host))
                {
                    if (IPAddress.IsLoopback(IPAddress.Parse(host)))
                        return true;

                    if (IsInPrivateRange(host))
                        return true;
                }

                if (IsValidFQDN(host))
                {
                    IPAddress[] ipAddresses = Dns.GetHostAddresses(host);

                    foreach (IPAddress ipAddress in ipAddresses)
                    {
                        if (IPAddress.IsLoopback(ipAddress))
                            return true;

                        if (IsInPrivateRange(ipAddress.ToString()))
                            return true;
                    }
                }
            }

            return false;
        }
        public static bool IsValidFQDN(string host)
        {
            if (!string.IsNullOrEmpty(host))
            {
                try
                {
                    Dns.GetHostEntry(host);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }
        public static bool IsValidIP(string host)
        {
            if (!string.IsNullOrEmpty(host))
                return IPAddress.TryParse(host, out _);

            return false;
        }
        public static bool IsValidIPv4(string host)
        {
            if (!string.IsNullOrEmpty(host))
                return IPAddress.TryParse(host, out _) && IPAddress.Parse(host).AddressFamily == AddressFamily.InterNetwork;

            return false;
        }
        public static bool IsValidIPv6(string host)
        {
            if (!string.IsNullOrEmpty(host))
                return IPAddress.TryParse(host, out _) && IPAddress.Parse(host).AddressFamily == AddressFamily.InterNetworkV6;

            return false;
        }
        private static bool IsInPrivateRange(string host)
        {
            if (!string.IsNullOrEmpty(host))
            {
                IPAddress ip = IPAddress.Parse(host);
                byte[] ipBytes = ip.GetAddressBytes();

                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (ipBytes[0] == 10 ||
                        ipBytes[0] == 172 && ipBytes[1] >= 16 && ipBytes[1] <= 31 ||
                        ipBytes[0] == 192 && ipBytes[1] == 168)
                    {
                        return true;
                    }
                }

                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    if (ipBytes[0] == 0xFD && (ipBytes[1] & 0xC0) == 0x80 ||
                        ipBytes[0] == 0xFE && (ipBytes[1] & 0xC0) == 0xC0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
