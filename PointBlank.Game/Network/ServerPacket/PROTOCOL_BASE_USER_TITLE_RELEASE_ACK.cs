﻿
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BASE_USER_TITLE_RELEASE_ACK : SendPacket
  {
    private uint _erro;

    public PROTOCOL_BASE_USER_TITLE_RELEASE_ACK(uint erro) => this._erro = erro;

    public override void write()
    {
      this.writeH((short) 589);
      this.writeD(this._erro);
    }
  }
}
