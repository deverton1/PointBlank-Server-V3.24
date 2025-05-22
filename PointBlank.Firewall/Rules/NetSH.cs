using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointBlank.Firewall.configs;
using NetFwTypeLib;
namespace PointBlank.Firewall.Rules
{
    public class NetSH
    {
        public static string RandName(int time, string ip, uint date) // Gera nome único pra regra
        {
            Random random = new Random();
            int rand = random.Next(1, 500);

            string ruleName = "AutoBlock - " + ip;
            if (time > 0)
                ruleName += " id=" + (date / rand);
            else
                ruleName += " Permanent";
            return ruleName;
        }
        public static void RemoveRuleByName(string RuleName = "API_127.0.0.1")
        {
            try
            {
                Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
                var currentProfiles = fwPolicy2.CurrentProfileTypes;
                List<INetFwRule> RuleList = new List<INetFwRule>();

                foreach (INetFwRule rule in fwPolicy2.Rules)
                {
                    if (rule.Name.IndexOf(RuleName) != -1)
                    {
                        INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                        firewallPolicy.Rules.Remove(rule.Name);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine("Error delete rule from firewall");
            }
        }
        public void NewRule(string ip, string name, string ports, int type, int protocol)
        {
            try
            {
                Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
                var currentProfiles = fwPolicy2.CurrentProfileTypes;
                INetFwRule2 inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                inboundRule.Enabled = true;
                if (type == 1)
                {
                    inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                }
                else
                {
                    inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                }
                inboundRule.Protocol = protocol;
                inboundRule.LocalPorts = ports;
                inboundRule.RemoteAddresses = ip;
                inboundRule.Name =name;
                inboundRule.Profiles = 4;
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                firewallPolicy.Rules.Add(inboundRule);

                Console.WriteLine("[FCONTROLL] IP BLOQUEADO : {0} ", ip);


            }
            catch (Exception ex)
            {

            }
        }
        public static bool ExistRule(string name, string ip = "127.0.0.1")
        {
            try
            {
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                List<INetFwRule> firewallRules = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name.Contains(ip)).ToList();

                foreach (INetFwRule rule in firewallRules)
                {

                    if (rule.Name.IndexOf(name) != -1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                //
            }

            return false;
        }
        
        public static void Block(string addr, string name, string description)
        {
            try
            {
                string arg = "";
                arg += "/C netsh advfirewall firewall add rule name=\"" + name + "\" description=\"" + description + "\" protocol=tcp localport=39190 dir=in action=block remoteip=" + addr;
                Process pr = new Process();
                ProcessStartInfo prs = new ProcessStartInfo();
                prs.FileName = @"cmd.exe";
                prs.Verb = "runas"; // Run to admin
                prs.Arguments = arg;
                prs.WindowStyle = ProcessWindowStyle.Hidden;
                pr.StartInfo = prs;

                pr.Start();
            }
            catch (Exception ex)
            {
                Logger.error("[NETSH.ERROR.BLOCK] FATAL! " + ex.ToString());
            }

        }
        public static void PermitTCP(string addr)
        {
            try
            {
                string arg = "";

                arg += "/C netsh advfirewall firewall add rule name=\"PB_API_TCP_FIREWALL_" + addr + "\" description=\"TCP - API PROTECT\" protocol=tcp localport=" + Config.gamePort + " dir=in action=allow remoteip=" + addr;

                Process pr = new Process();
                ProcessStartInfo prs = new ProcessStartInfo();
                prs.FileName = @"cmd.exe";
                prs.Verb = "runas"; // Run to admin
                prs.Arguments = arg;
                prs.WindowStyle = ProcessWindowStyle.Hidden;
                pr.StartInfo = prs;

                pr.Start();
            }
            catch (Exception ex)
            {
                Logger.debug("[NETSH.ALLOW.ERROR] FATAL! " + ex.ToString());
            }

        }

        public static void PermitUDP(string addr)
        {
            try
            {
                string arg = "";

                arg += "/C netsh advfirewall firewall add rule name=\"PB_API_UDP_FIREWALL_" + addr + "\" description=\"UDP  -API PROTECT\" protocol=udp  localport=" + Config.battlePort + " dir=in action=allow remoteip=" + addr;


                Process pr = new Process();
                ProcessStartInfo prs = new ProcessStartInfo();
                prs.FileName = @"cmd.exe";
                prs.Verb = "runas"; // Run to admin
                prs.Arguments = arg;
                prs.WindowStyle = ProcessWindowStyle.Hidden;
                pr.StartInfo = prs;

                pr.Start();
            }
            catch (Exception ex)
            {
                Logger.debug("[NETSH.ALLOW.ERROR] FATAL! " + ex.ToString());
            }

        }



        public static void Reset()
        {
            // netsh advfirewall firewall delete rule name=all localport=" + Config.gamePort + "
            // netsh advfirewall firewall delete rule name=all localport=" + Config.battlePort + "
        }

        public static void Remove(string name)
        {
            Logger.info("Removido => " + name);
            try
            {

                Process pr = new Process();
                ProcessStartInfo prs = new ProcessStartInfo();
                prs.FileName = @"cmd.exe";
                prs.Arguments = "/c netsh advfirewall firewall delete rule name=\"" + name + "\"";
                prs.WindowStyle = ProcessWindowStyle.Hidden;
                pr.StartInfo = prs;
                pr.Start();

            }
            catch (Exception ex)
            {
                Logger.debug("[NETSH.REMOVE.ERROR] FATAL! " + ex.ToString());
            }
        }
    }
}
