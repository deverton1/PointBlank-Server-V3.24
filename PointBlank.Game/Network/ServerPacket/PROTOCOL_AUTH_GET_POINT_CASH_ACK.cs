
using PointBlank.Core.Network;
using System.Security.Policy;

namespace PointBlank.Game.Network.ServerPacket
{
    public class PROTOCOL_AUTH_GET_POINT_CASH_ACK : SendPacket
    {
        private readonly int _erro, _gold, _cash, _tag;

        public PROTOCOL_AUTH_GET_POINT_CASH_ACK(int erro, int gold, int cash, int tag)
        {
            _erro = erro;
            _gold = gold;
            _cash = cash;
            _tag = tag;
        }

        public override void write()
        {
            writeH(1058);
            writeD(_erro);
            if (_erro >= 0)
            {
                writeD(_gold);
                writeD(_cash);
                writeD(_tag);//tag
            }
        }

    }
}
