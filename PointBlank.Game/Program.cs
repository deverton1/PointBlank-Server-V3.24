
using Game.data.managers;
using PointBlank.Core;
using PointBlank.Core.Filters;
using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Managers.Server;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Sync;
using PointBlank.Game.Data.Xml;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace PointBlank.Game
{
    public class Programm
    {
        public static void Main(string[] args)
        {
            ComDiv.GetLinkerTime(Assembly.GetExecutingAssembly()).ToString("dd/MM/yyyy HH:mm");
            Logger.StartedFor = "Game";
            Logger.checkDirectorys();
            Console.Clear();
            StartConsole();
            GameConfig.Load();
            LoadXML();
            if (Logger.erro)
            {
                Logger.error("Check your configuration.");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }
            if (GameManager.Start())
                PointBlank.Game.Game.Update();


            while (true) {
                string Read = Console.ReadLine();
                if (Read.StartsWith("reload_shop"))
                {
                    string Result = "";
                    try
                    {
                        ShopManager.Reset();
                        ShopManager.Load(1);
                        foreach (GameClient client in GameManager._socketList.Values)
                        {
                            Account Player = client._player;
                            if (Player != null && Player._isOnline)
                            {
                                for (int i = 0; i < ShopManager.ShopDataItems.Count; i++)
                                {
                                    ShopData data = ShopManager.ShopDataItems[i];
                                    Player.SendPacket(new PROTOCOL_AUTH_SHOP_ITEMLIST_ACK(data, ShopManager.TotalItems));
                                }
                                for (int i = 0; i < ShopManager.ShopDataGoods.Count; i++)
                                {
                                    ShopData data = ShopManager.ShopDataGoods[i];
                                    Player.SendPacket(new PROTOCOL_AUTH_SHOP_GOODSLIST_ACK(data, ShopManager.TotalGoods));
                                }
                                for (int i = 0; i < ShopManager.ShopDataItemRepairs.Count; i++)
                                {
                                    ShopData data = ShopManager.ShopDataItemRepairs[i];
                                    Player.SendPacket(new PROTOCOL_AUTH_SHOP_REPAIRLIST_ACK(data, ShopManager.TotalRepairs));
                                }
                                //int cafe = Player.pc_cafe;
                                if (Player.pc_cafe == 0)
                                {
                                    for (int i = 0; i < ShopManager.ShopDataMt1.Count; i++)
                                    {
                                        ShopData data = ShopManager.ShopDataMt1[i];
                                        Player.SendPacket(new PROTOCOL_AUTH_SHOP_MATCHINGLIST_ACK(data, ShopManager.TotalMatching1));
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < ShopManager.ShopDataMt2.Count; i++)
                                    {
                                        ShopData data = ShopManager.ShopDataMt2[i];
                                        Player.SendPacket(new PROTOCOL_AUTH_SHOP_MATCHINGLIST_ACK(data, ShopManager.TotalMatching2));
                                    }
                                }
                                Player.SendPacket(new PROTOCOL_SHOP_GET_SAILLIST_ACK(true));
                            }
                        }
                        Result = "Reload Shop Success.";
                    }
                    catch
                    {
                        Result = "Command Error.";
                    }
                    Logger.console(Result);
                    Logger.LogConsole(Read, Result);
                }
                else if (Read.StartsWith("reload_xml"))
                {
                    LoadXML();
                }
            }
        }
        public static void LoadXML() {
            BasicInventoryXml.Load();
            CafeInventoryXml.Load();
            ServerConfigSyncer.GenerateConfig(GameConfig.configId);
            ServersXml.Load();
            ChannelsXml.Load(GameConfig.serverId);
            EventLoader.LoadAll();
            TitlesXml.Load();
            TitleAwardsXml.Load();
            ClanManager.Load();
            NickFilter.Load();
            MissionCardXml.LoadBasicCards(1);
            RankedXml.Load();
            RankedXml.LoadAwards();
            RankXml.Load();
            WeaponExpXml.Load();
            BattleServerXml.Load();
            RankXml.LoadAwards();
            ClanRankXml.Load();
            MissionAwardsXml.Load();
            MissionsXml.Load();
            Translation.Load();
            ShopManager.Load(1);
            BattleBoxManager.Load();
            ClassicModeManager.LoadList();
            MapsXml.Load();
            RandomBoxXml.LoadBoxes();
            ICafeManager.GetList();
            CouponEffectManager.LoadCouponFlags();
            GameSync.Start();
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

               // Logger.LogYaz(@"IP NOT RELEASE", ConsoleColor.Green);
               // Thread.Sleep(5000);
                //Environment.Exit(0);
            }
            Logger.LogYaz("" + Environment.NewLine, ConsoleColor.Green);
            Logger.LogYaz(@"|==========================================================================================|", ConsoleColor.DarkGreen);
            Logger.LogYaz(@"|POINT BLANK SERVER 3.24                                                                   |", ConsoleColor.DarkGreen);
            Logger.LogYaz(@"|RELEASE 0.0.2                                                                             |", ConsoleColor.DarkGreen);
            Logger.LogYaz(@"|Discord Contact: vetoxxy1                                                                 |", ConsoleColor.DarkGreen);
            Logger.LogYaz(@"|==========================================================================================|", ConsoleColor.DarkGreen);
            Logger.LogYaz("" + Environment.NewLine, ConsoleColor.Green);
        }
    }
}
