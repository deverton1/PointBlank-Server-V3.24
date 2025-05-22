using PointBlank.Core.Network;
using PointBlank.Core;
using PointBlank.Game.Data.API;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using System.Security.Cryptography;
using PointBlank.Game.Network.ServerPacket;
using Npgsql;
using PointBlank.Core.Managers;
using PointBlank.Game.Data.Sync.Server;
using System.Security.Principal;
using System;
namespace PointBlank.Game.Data.Sync.Client
{
    public class GiftItemAPI
    {
        public static void Load(ReceiveGPacket buffer)
        {
            long playerID = buffer.readQ();
            int itemID = buffer.readD();
            int quantity = buffer.readD();
            int itemPRICE = buffer.readD();
            API_SendItem.SendById(playerID, itemID, itemPRICE, quantity);
        }
    }
}
