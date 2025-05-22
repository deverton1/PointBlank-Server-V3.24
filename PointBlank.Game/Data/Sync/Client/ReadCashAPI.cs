using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Core;
using PointBlank.Game.Data.API;
namespace PointBlank.Game.Data.Sync.Client
{
    internal class ReadCashAPI
    {
        public static void Load(ReceiveGPacket buffer)
        {
            long pId = buffer.readQ();
            int cash = buffer.readD();
            API_SendCash.SendById(pId, cash);

        }
    }
}
