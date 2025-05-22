// Decompiled with JetBrains decompiler
// Type: PointBlank.Auth.Network.ServerPacket.PROTOCOL_BASE_GET_CHANNELLIST_ACK
// Assembly: PointBlank.Auth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2F2D71E3-3E87-4155-AA64-E654DA3CFF2D
// Assembly location: C:\Users\LucasRoot\Desktop\Servidor BG\PointBlank.Auth.exe

using PointBlank.Auth.Data.Configs;
using PointBlank.Auth.Data.Model;
using PointBlank.Core.Network;
using System.Collections.Generic;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_GET_CHANNELLIST_ACK : SendPacket
  {
    private List<Channel> Channels;

    public PROTOCOL_BASE_GET_CHANNELLIST_ACK(List<Channel> Channels) => this.Channels = Channels;

    public override void write()
    {
      this.writeH((short) 541);
      this.writeH((short) 0);
      this.writeC((byte) 0);
      this.writeC((byte) this.Channels.Count);
      for (int index = 0; index < this.Channels.Count; ++index)
        this.writeH((ushort) this.Channels[index]._players);
      this.writeH((ushort) AuthConfig.maxChannelPlayers);
      this.writeC((byte) this.Channels.Count);
    }
  }
}
