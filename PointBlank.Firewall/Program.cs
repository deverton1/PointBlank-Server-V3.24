using System;
using System.Diagnostics;
using PointBlank.Firewall.Rules;
using PointBlank.Firewall.configs;
using PointBlank.Firewall.Socket;
using PointBlank.Firewall.Utils;
namespace PointBlank.Firewall
{
    public class Program
    {
        static void Main(string[] args)
        {
            Logger.checkDirectory();
            WhiteList.Load();

            new Config();
            Console.Title = "FIREWALL PROTECT SERVER";
            DateTime started = DateTime.Now;
            Logger.LogYaz("" + Environment.NewLine, ConsoleColor.Green);
            Logger.LogYaz(@"|==========================================================================================|", ConsoleColor.DarkGreen);
            Logger.LogYaz(@"|POINT BLANK FIREWALL API                                                                  |", ConsoleColor.DarkGreen);
            Logger.LogYaz(@"|TIME: " + started + "                                                                     |", ConsoleColor.DarkGreen);
            Logger.LogYaz(@"|==========================================================================================|", ConsoleColor.DarkGreen);
            Logger.LogYaz("" + Environment.NewLine, ConsoleColor.Green);

            NetSH.Reset();
            SocketSync.Start();


            Monitoring.Load();
            Memory.updateRAM();

            Process.GetCurrentProcess().WaitForExit();
            
        }
    }
}
