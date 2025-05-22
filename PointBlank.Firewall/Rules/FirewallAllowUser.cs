using PointBlank.Core.Network;
using System.Collections.Generic;
using System.Net;
using PointBlank.Firewall.configs;
using System.Text;
using System.IO;
namespace PointBlank.Firewall.Rules
{
    public class FirewallAllowUser
    {
        public static List<string> allowed = new List<string>();

        public FirewallAllowUser(ReceiveGPacket dados)
        {
            // codigo de operação - 1 byte
            // ipSize - 1 byte
            // ip
            int ipSize = dados.readC();
            byte[] bytes = dados.readB(ipSize);
            string ip = Encoding.UTF8.GetString(bytes);
            IPAddress ipAddr;
            if (!IPAddress.TryParse(ip, out ipAddr))
            {
                Logger.error("[ERROR]IP Invalido.");
                return;
            }

            if (allowed.Contains(ip))
            {
                Logger.LogYaz("[API] IP: " + ip + " JA ESTA LIBERADO EM NOSSO SISTEMA.", System.ConsoleColor.Yellow);
                return;
            }
            if(!NetSH.ExistRule("PB_API_TCP_FIREWALL_" + ip, ip))
            {
                NetSH.PermitTCP(ip);
            }
            if (!NetSH.ExistRule("PB_API_UDP_FIREWALL_" + ip, ip))
            {
                NetSH.PermitUDP(ip);
            }
            allowed.Add(ip);

            Logger.LogYaz("[API] IP: " + ip + " PORT: " + Config.gamePort + " UDP " + Config.battlePort, System.ConsoleColor.Green);


        }
    }
}
