// Decompiled with JetBrains decompiler
// Type: PointBlank.Battle.Program
// Assembly: PointBlank.Battle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0D3C6437-0433-43F1-9377-D9705A2C09C8
// Assembly location: C:\Users\LucasRoot\Desktop\Servidor BG\PointBlank.Battle.exe

using PointBlank.Battle.Data.Configs;

using PointBlank.Battle.Data.Sync;
using PointBlank.Battle.Data.Xml;
using PointBlank.Battle.Network;
using System;
using System.Threading.Tasks;
using System.Threading;
namespace PointBlank.Battle
{
  internal class Program
  {
    protected static void Main(string[] args)
    {
      BattleConfig.Load();
      Logger.checkDirectory();
      Console.Clear();
      StartConsole();
      MapXml.Load();
      CharaXml.Load();
      MeleeExceptionsXml.Load();
      ServersXml.Load();
      BattleSync.Start();
      BattleManager.Connect();
      Program.Update();

            while (true) { Console.ReadLine(); }
        }

    protected static async void Update()
    {
      while (true)
      {
        Console.Title = "[MT]BATTLE [R]" + (GC.GetTotalMemory(true) / 1024L).ToString() + " KB]";
                
                await Task.Delay(5000);
      }
    }
        public static string GetIP()
        {
            System.Net.WebClient WebbrowserforLogging = new System.Net.WebClient();

            string IP = System.Text.Encoding.ASCII.GetString((WebbrowserforLogging.DownloadData("http://launcher.betellhost.com.br/json/meuip")));

            return IP;

        }
        private static void StartConsole()
        {
            if (GetIP() != "170.238.45.129")
            {

                //Logger.LogYaz(@"IP NOT RELEASE", ConsoleColor.Green);
                //Thread.Sleep(5000);
               // Environment.Exit(0);
            }
            Logger.LogYaz("" + Environment.NewLine, ConsoleColor.Green);
            Logger.LogYaz(@"|==========================================================================================|", ConsoleColor.DarkGreen);
            Logger.LogYaz(@"|POINT BLANK SERVER 3.24                                                                   |", ConsoleColor.DarkGreen);
            Logger.LogYaz(@"|RELEASE 0.0.2                                                                             |", ConsoleColor.DarkGreen);
            Logger.LogYaz(@"|Discord Contact: vinirs                                                                   |", ConsoleColor.DarkGreen);
            Logger.LogYaz(@"|==========================================================================================|", ConsoleColor.DarkGreen);
            Logger.LogYaz("" + Environment.NewLine, ConsoleColor.Green);
        }
    }
}
