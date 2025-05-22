// Decompiled with JetBrains decompiler
// Type: PointBlank.Auth.Network.ServerPacket.PROTOCOL_BASE_GET_SYSTEM_INFO_ACK
// Assembly: PointBlank.Auth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2F2D71E3-3E87-4155-AA64-E654DA3CFF2D
// Assembly location: C:\Users\LucasRoot\Desktop\Servidor BG\PointBlank.Auth.exe

using PointBlank.Auth.Data.Xml;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Servers;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using System.Collections.Generic;

namespace PointBlank.Auth.Network.ServerPacket
{
    public class PROTOCOL_BASE_GET_SYSTEM_INFO_ACK : SendPacket
    {
        public override void write()
        {
            writeH(523);
            writeH(0);
            writeC(0);
            writeC(5);
            writeC(10);
            writeC(32);
            writeC(4);
            writeC(0);
            writeC(1);
            writeC(2);
            writeC(5);
            writeC(3);
            writeC(6);
            writeB(new byte[25]);
            writeC(7);
            writeB(new byte[229]);
            short val = 0;
            if (AuthManager.Config.ClanEnable)
                val += 4096;
            writeH(val);
            writeH(1024); //tag
            writeC(3);
            writeD(600);
            writeD(2400);
            writeD(6000);
            writeC(1);
            writeH((ushort)MissionsXml._missionPage1);
            writeH((ushort)MissionsXml._missionPage2);
            writeH(29890);
            writeC((byte)ServersXml._servers.Count);
            for (int index1 = 0; index1 < ServersXml._servers.Count; ++index1)
            {
                GameServerModel server = ServersXml._servers[index1];
                writeD(server._state);
                writeIP(server.Connection.Address);
                writeH(server._port);
                writeC((byte)server._type);
                writeH((ushort)server._maxPlayers);
                writeD(server._LastCount);
                if (index1 == 0)
                {
                    for (int index2 = 0; index2 < 10; ++index2)
                        writeC(1);
                }
                else
                {
                    for (int index3 = 0; index3 < ChannelsXml.getChannels(index1).Count; ++index3)
                        writeC((byte)ChannelsXml._channels[index3]._type);
                }
            }
            writeH((ushort)AuthManager.Config.ExitURL.Length);
            writeS(AuthManager.Config.ExitURL, AuthManager.Config.ExitURL.Length);
            writeC(51);
            for (int rank = 0; rank < 52; ++rank)
            {
                List<ItemsModel> awards = RankXml.getAwards(rank);
                writeC((byte)rank);
                for (int index = 0; index < awards.Count; ++index)
                {
                    ItemsModel itemsModel = awards[index];
                    if (ShopManager.getItemId(itemsModel._id) == null)
                        writeD(0);
                    else
                        writeD(ShopManager.getItemId(itemsModel._id).id);
                }
                for (int count = awards.Count; 4 - count > 0; ++count)
                    writeD(0);
            }
            writeC(1);
        }
    }
}
