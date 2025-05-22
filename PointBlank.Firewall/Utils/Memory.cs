using PointBlank.Firewall.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointBlank.Firewall.Utils
{
    public class Memory
    {
        public static ushort blockPerm = 0;
        public static async void updateRAM()
        {
            try
            {
                PerformanceCounter myAppCpu =
                new PerformanceCounter(
                    "Process", "% Processor Time", "PointBlank.Firewall", true);


                while (true)
                {
                    double pct = myAppCpu.NextValue();
                    Console.Title = "FIREWALL API [ALLOW: " + FirewallAllowUser.allowed.Count + "; TEMP BLOCK:" + Monitoring.unlockQueue.Count + " PERM BLOCK: " + blockPerm + " || CPU: " + pct.ToString("n") + "% RAM: " + (GC.GetTotalMemory(true) / 1024) + " KB]";
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
