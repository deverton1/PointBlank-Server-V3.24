using PointBlank.Core.Network;
using PointBlank.Firewall.configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PointBlank.Firewall.Utils;
namespace PointBlank.Firewall.Rules
{
    public class FirewallRemoveUser
    {
        public static List<string> blocked = new List<string>();
        public FirewallRemoveUser(ReceiveGPacket dados)
        {
            try
            {
                int ipSize = dados.readC();
                int descSize = dados.readC();
                string ip = Encoding.UTF8.GetString(dados.readB(ipSize));

                if (blocked.Contains(ip))
                    return;

                IPAddress ipAddr;
                if (!IPAddress.TryParse(ip, out ipAddr))
                {
                    Logger.debug("[Error] Invalid IP");
                    return;
                }

                if (WhiteList.check(ipAddr)) return; // WhiteList

                string descricao = "[" + DateTime.Now.ToString() + "] " + Encoding.UTF8.GetString(dados.readB(descSize));
                int timeBan = Tools.getGravit(dados.readC());

                // Verifica se o ip já foi permitido
                if (FirewallAllowUser.allowed.Contains(ip))
                {
                    Logger.LogYaz("[API] IP :" + ip + " FOI REMOVIDO (CONEXAO BLOQUEADA)", System.ConsoleColor.Red);

                    if (NetSH.ExistRule("PB_API_UDP_FIREWALL_" + ip, ip))
                    {
                        NetSH.RemoveRuleByName("PB_API_UDP_FIREWALL_" + ip);
                    }
                    if (NetSH.ExistRule("PB_API_TCP_FIREWALL_" + ip, ip))
                    {
                        NetSH.RemoveRuleByName("PB_API_TCP_FIREWALL_" + ip);
                    }

                    FirewallAllowUser.allowed.Remove(ip);
                }



                uint date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));

                string name = NetSH.RandName(timeBan, ip, date);
                NetSH.Block(ip, name, descricao); // Bloqueia no firewall

  

                Logger.debug("[BLOCKED " + timeBan + " MINUTES]");
                Logger.debug("IP CONNECTION " + ip);
                Logger.debug("RULE NAME: " + name);
                Logger.debug("DESCRIPTION RULE: " + descricao);

                // Adiciona na lista de bloqueados caso o tempo seja diferente de 0
                if (timeBan > 0)
                {
                    Monitoring.RuleInfo ev = new Monitoring.RuleInfo
                    {
                        start = date,
                        end = (date + (int)timeBan),
                        name = name,
                        _ip = ip
                    };
                    Monitoring.unlockQueue.Add(ev);
                    Logger.LogYaz("IP BLOQUEADO TEMPORARIAMENTE, TEMPO: " + (date + timeBan) + " NAME:" + name, System.ConsoleColor.DarkGray);
                }
                else
                {
                    Memory.blockPerm++;
                    Logger.LogYaz("BLOQUEADO PREMANENTE: " + ip, System.ConsoleColor.Red);
                }

                blocked.Add(ip);
            }
            catch (Exception ex)
            {
                Logger.debug("[AddRule]\n" + ex);
            }
        }
    }
}
