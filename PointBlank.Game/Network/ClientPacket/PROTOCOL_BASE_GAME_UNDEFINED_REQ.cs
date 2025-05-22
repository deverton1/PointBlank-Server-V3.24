using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Linq;

namespace PointBlank.Game.Network.ClientPacket
{
    internal class PROTOCOLA_BASE_GAME_UNDEFINED_REQ : ReceivePacket
    {
        string nickname;

        public PROTOCOLA_BASE_GAME_UNDEFINED_REQ(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            nickname = readS(readC()); 
        }
        public override void run()
        {
            try
            {
                Logger.error($"[OP: 6657 | RESULT:]: {nickname}");
            }
            catch (Exception ex)
            {
                Logger.error($"[{GetType().Name}]: {ex.Message}");
            }
        }
    }
}
