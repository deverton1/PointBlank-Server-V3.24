// Decompiled with JetBrains decompiler
// Type: PointBlank.Auth.Network.ServerPacket.PROTOCOL_BASE_LOGIN_ACK
// Assembly: PointBlank.Auth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2F2D71E3-3E87-4155-AA64-E654DA3CFF2D
// Assembly location: C:\Users\LucasRoot\Desktop\Servidor BG\PointBlank.Auth.exe

using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ServerPacket
{
  public class PROTOCOL_BASE_LOGIN_ACK : SendPacket
  {
    private uint _result;
    private string _login;
    private long _pId;

    public PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum result, string login, long pId)
    {
      this._result = (uint) result;
      this._login = login;
      this._pId = pId;
    }

    public PROTOCOL_BASE_LOGIN_ACK(uint result, string login, long pId)
    {
      this._result = result;
      this._login = login;
      this._pId = pId;
    }

    public PROTOCOL_BASE_LOGIN_ACK(int result, string login, long pId)
    {
      this._result = (uint) result;
      this._login = login;
      this._pId = pId;
    }

    public override void write()
    {
      this.writeH((short) 259);
      this.writeB(new byte[15]);
      if (this._result == 0U)
      {
        this.writeC((byte) this._login.Length);
        this.writeS(this._login, this._login.Length);
        this.writeC((byte) 0);
        this.writeC((byte) this._login.Length);
        this.writeS(this._login, this._login.Length);
        this.writeQ(this._pId);
      }
      else
      {
        this.writeC((byte) 0);
        this.writeS("", 0);
        this.writeC((byte) 0);
        this.writeC((byte) 0);
        this.writeS("", 0);
        this.writeQ(0L);
      }
      this.writeD(this._result);
    }
  }
}
