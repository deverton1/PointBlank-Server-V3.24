// Decompiled with JetBrains decompiler
// Type: PointBlank.Auth.AuthClient
// Assembly: PointBlank.Auth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2F2D71E3-3E87-4155-AA64-E654DA3CFF2D
// Assembly location: C:\Users\LucasRoot\Desktop\Servidor BG\PointBlank.Auth.exe

using Microsoft.Win32.SafeHandles;
using PointBlank.Auth.Data.Configs;
using PointBlank.Auth.Data.Model;
using PointBlank.Auth.Data.Sync;
using PointBlank.Auth.Data.Sync.Server;
using PointBlank.Auth.Network;
using PointBlank.Auth.Network.ClientPacket;
using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core;
using PointBlank.Core.Filters;
using PointBlank.Core.Network;
using PointBlank.Game;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace PointBlank.Auth
{
  public class AuthClient : IDisposable
  {
    public Socket _client;
    public Account _player;
    public DateTime ConnectDate;
    public uint SessionId;
    public ushort SessionSeed, CountReceivedpackets;
    public int Shift;
    public bool firstPacketId = false;
    private byte[] lastCompleteBuffer;
    private bool disposed;
    private bool closed;
    private SafeHandle handle = (SafeHandle) new SafeFileHandle(IntPtr.Zero, true);

    public void Dispose()
    {
      try
      {
        if (this.disposed)
          return;
        if (this._client != null)
        {
          this._client.Dispose();
          this._client = (Socket) null;
        }
        this.handle.Dispose();
        this.disposed = true;
        GC.SuppressFinalize((object) this);
      }
      catch
      {
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      try
      {
        if (this.disposed)
          return;
        this._player = (Account) null;
        if (this._client != null)
        {
          this._client.Dispose();
          this._client = (Socket) null;
        }
        if (disposing)
          this.handle.Dispose();
        this.disposed = true;
      }
      catch
      {
      }
    }

    public AuthClient(Socket client)
    {
      this._client = client;
      this._client.NoDelay = true;
      this.SessionSeed = IdFactory.GetInstance().NextSeed();
    }

    public void Start()
    {
      this.Shift = (int) (this.SessionId % 7U) + 1;
      new Thread(new ThreadStart(this.Connect)).Start();
      new Thread(new ThreadStart(this.Read)).Start();
      //new Thread(new ThreadStart(this.ConnectionCheck)).Start();
      this.ConnectDate = DateTime.Now;
    }

    public string GetIPAddress()
    {
      try
      {
        return this._client != null && this._client.RemoteEndPoint != null ? ((IPEndPoint) this._client.RemoteEndPoint).Address.ToString() : "";
      }
      catch
      {
        return "";
      }
    }

    public IPAddress GetAddress()
    {
      try
      {
        return this._client != null && this._client.RemoteEndPoint != null ? ((IPEndPoint) this._client.RemoteEndPoint).Address : (IPAddress) null;
      }
      catch
      {
        return (IPAddress) null;
      }
    }

    private void Connect() => this.SendPacket((PointBlank.Core.Network.SendPacket) new PROTOCOL_BASE_CONNECT_ACK(this));

    public void SendCompletePacket(byte[] data)
    {
      try
      {
        if (data.Length < 4)
          return;
        if (AuthConfig.debugMode)
        {
          ushort uint16 = BitConverter.ToUInt16(data, 2);
          string str1 = "";
          string str2 = BitConverter.ToString(data);
          char[] chArray = new char[5]
          {
            '-',
            ',',
            '.',
            ':',
            '\t'
          };
          foreach (string str3 in str2.Split(chArray))
            str1 = str1 + " " + str3;
          Logger.debug("Opcode: [" + uint16.ToString() + "]");
        }
        this._client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), (object) this._client);
      }
      catch
      {
        this.Close(0, true);
      }
    }

    public void SendPacket(byte[] data)
    {
      try
      {
        if (data.Length < 2)
          return;
        ushort uint16_1 = Convert.ToUInt16(data.Length - 2);
        List<byte> byteList = new List<byte>(data.Length + 2);
        byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(uint16_1));
        byteList.AddRange((IEnumerable<byte>) data);
        byte[] array = byteList.ToArray();
        if (AuthConfig.debugMode)
        {
          ushort uint16_2 = BitConverter.ToUInt16(data, 0);
          string str1 = "";
          string str2 = BitConverter.ToString(array);
          char[] chArray = new char[5]
          {
            '-',
            ',',
            '.',
            ':',
            '\t'
          };
          foreach (string str3 in str2.Split(chArray))
            str1 = str1 + " " + str3;
          Logger.debug("Opcode: [" + uint16_2.ToString() + "]");
        }
        if (array.Length != 0)
          this._client.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), (object) this._client);
        byteList.Clear();
      }
      catch
      {
        this.Close(0, true);
      }
    }

    public void SendPacket(PointBlank.Core.Network.SendPacket bp)
    {
      try
      {
        using (bp)
        {
          bp.write();
          byte[] array1 = bp.mstream.ToArray();
          if (array1.Length < 2)
            return;
          ushort uint16_1 = Convert.ToUInt16(array1.Length - 2);
          List<byte> byteList = new List<byte>(array1.Length + 2);
          byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(uint16_1));
          byteList.AddRange((IEnumerable<byte>) array1);
          byte[] array2 = byteList.ToArray();
          if (AuthConfig.debugMode)
          {
            ushort uint16_2 = BitConverter.ToUInt16(array1, 0);
            string str1 = "";
            string str2 = BitConverter.ToString(array2);
            char[] chArray = new char[5]
            {
              '-',
              ',',
              '.',
              ':',
              '\t'
            };
            foreach (string str3 in str2.Split(chArray))
              str1 = str1 + " " + str3;
            Logger.debug("Opcode: [" + uint16_2.ToString() + "]");
          }
          if (array2.Length != 0)
            this._client.BeginSend(array2, 0, array2.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), (object) this._client);
          bp.mstream.Close();
          byteList.Clear();
        }
      }
      catch
      {
        this.Close(0, true);
      }
    }

    private void SendCallback(IAsyncResult ar)
    {
      try
      {
        Socket asyncState = (Socket) ar.AsyncState;
        if (asyncState == null || !asyncState.Connected)
          return;
        asyncState.EndSend(ar);
      }
      catch
      {
        this.Close(0, true);
      }
    }
        private void limitPackets()
        {
            CountReceivedpackets++;
            if (CountReceivedpackets > 200)
            {
                string msg = GetIPAddress() + " Limite de pacotes por sessao atingido [" + CountReceivedpackets + "]";
                Logger.warning(msg);
                Firewall.sendBlock(GetIPAddress(), msg, 2);
                Close(0, true);
            }
        }
        private void Read()
        {
            limitPackets();
            try
            {
                AuthClient.StateObject stateObject = new AuthClient.StateObject();
                stateObject.workSocket = this._client;
                this._client.BeginReceive(stateObject.buffer, 0, 8096, SocketFlags.None, new AsyncCallback(this.OnReceiveCallback), (object)stateObject);
            }
            catch
            {
                this.Close(0, true);
            }
        }

    public void Close(int time, bool destroyConnection)
    {
      if (this.closed)
        return;
      try
      {
        this.closed = true;
        Account player = this._player;
        if (destroyConnection)
        {
          if (player != null)
          {
            player.setOnlineStatus(false);
            if (player._status.serverId == (byte) 0)
              SendRefresh.RefreshAccount(player, false);
            player._status.ResetData(player.player_id);
            player.SimpleClear();
            player.updateCacheInfo();
            this._player = (Account) null;
          }
          this._client.Close(time);
          Thread.Sleep(time);
          this.Dispose();
        }
        else if (player != null)
        {
          player.SimpleClear();
          player.updateCacheInfo();
          this._player = (Account) null;
        }
        AuthSync.UpdateGSCount(0);
      }
      catch (Exception ex)
      {
        Logger.warning("AuthClient.Close " + ex.ToString());
      }
    }

    private void OnReceiveCallback(IAsyncResult ar)
    {
      AuthClient.StateObject asyncState = (AuthClient.StateObject) ar.AsyncState;
      try
      {
        int length = asyncState.workSocket.EndReceive(ar);
        if (length <= 0)
          return;
        byte[] buffer = new byte[length];
        Array.Copy((Array) asyncState.buffer, 0, (Array) buffer, 0, length);
        int FirstLength = (int) BitConverter.ToUInt16(buffer, 0) & (int) short.MaxValue;
        byte[] numArray = new byte[FirstLength + 2];
        Array.Copy((Array) buffer, 2, (Array) numArray, 0, numArray.Length);
        this.lastCompleteBuffer = buffer;
        this.RunPacket(ComDiv.Decrypt(numArray, this.Shift), numArray);
        this.CheckOut(buffer, FirstLength);
        new Thread(new ThreadStart(this.Read)).Start();
      }
      catch (ObjectDisposedException ex)
      {
        ex.ToString();
      }
      catch
      {
        this.Close(0, true);
      }
    }

    public void CheckOut(byte[] buffer, int FirstLength)
    {
      int length = buffer.Length;
      try
      {
        byte[] numArray = new byte[length - FirstLength - 4];
        Array.Copy((Array) buffer, FirstLength + 4, (Array) numArray, 0, numArray.Length);
        if (numArray.Length == 0)
          return;
        int FirstLength1 = (int) BitConverter.ToUInt16(numArray, 0) & (int) short.MaxValue;
        byte[] data = new byte[FirstLength1 + 2];
        Array.Copy((Array) numArray, 2, (Array) data, 0, data.Length);
        byte[] buff = new byte[FirstLength1 + 2];
        Array.Copy((Array) ComDiv.Decrypt(data, this.Shift), 0, (Array) buff, 0, buff.Length);
        this.RunPacket(buff, numArray);
        this.CheckOut(numArray, FirstLength1);
      }
      catch
      {
      }
    }
        private void ConnectionCheck()
        {
            Thread.Sleep(10000);
            if (this._client != null && !firstPacketId)
            {
                string msg = GetIPAddress();
                Logger.warning("IP: " + msg + " Connection destroyed due to socket null and blocked.");
                Firewall.sendBlock(msg, "First Packet Iconrrect", 10);
                this.Close(0, true);
 
            }
        }
        private bool FirstPacketCheck(ushort packetId)
        {
            if (packetId == (ushort)257 || packetId == (ushort)517)
            {
                this.firstPacketId = true;
            }
            else
            {
                this.firstPacketId = false;
            }
            return this.firstPacketId;
        }
        private bool PacketCheckAuth(ushort uint16)
        {
           

            if (uint16 == 257) { return true; }
            else if (uint16 == 515) { return true; }
            else if (uint16 == 517) { return true; }
            else if (uint16 == 520) { return true; }
            else if (uint16 == 522) { return true; }
            else if (uint16 == 524) { return true; }
            else if (uint16 == 526) { return true; }
            else if (uint16 == 528) { return true; }
            else if (uint16 == 530) { return true; }
            else if (uint16 == 536) { return true; }
            else if (uint16 == 540) { return true; }
            else if (uint16 == 666) { return true; }
            else if (uint16 == 1057) { return true; }
            else if (uint16 == 5377) { return true; }
            else 
            {

              
                return false; 
            
            }
        }

        private void RunPacket(byte[] buff, byte[] simple)
        {
            ushort uint16 = BitConverter.ToUInt16(buff, 0);
            if (!this.firstPacketId)
            {
                if (!this.FirstPacketCheck(uint16))
                {
                    string msg = "IP: " + GetIPAddress() + " - Primeiro pacote nao recebido OPCODE RECEBIDO:[" + uint16.ToString() + "]";
                    Logger.warning(msg);
                    Firewall.sendBlock(GetIPAddress(), msg, 1);
                    this.Close(0, true);
                    return;
                }
            }
            //ushort uintOK;
            if (!PacketCheckAuth(uint16)) { this._client.Close(1000); uint16 = 517; }
            if (this.closed)
                return;
            ReceivePacket receivePacket = (ReceivePacket)null;
            switch (uint16)
            {
                case 257:
                    receivePacket = (ReceivePacket)new PROTOCOL_BASE_LOGIN_REQ(this, buff);
                    goto case 517;
                case 515:
                    receivePacket = (ReceivePacket)new PROTOCOL_BASE_LOGOUT_REQ(this, buff);
                    goto case 517;
                case 517:
                    if (receivePacket == null)
                        break;
                    new Thread(new ThreadStart(receivePacket.run)).Start();
                    break;
                case 520:
                    receivePacket = (ReceivePacket)new PROTOCOL_BASE_GAMEGUARD_REQ(this, buff);
                    goto case 517;
                case 522:
                    receivePacket = (ReceivePacket)new PROTOCOL_BASE_GET_SYSTEM_INFO_REQ(this, buff);
                    goto case 517;
                case 524:
                    receivePacket = (ReceivePacket)new PROTOCOL_BASE_GET_USER_INFO_REQ(this, buff);
                    goto case 517;
                case 526:
                    receivePacket = (ReceivePacket)new PROTOCOL_BASE_GET_INVEN_INFO_REQ(this, buff);
                    goto case 517;
                case 528:
                    receivePacket = (ReceivePacket)new PROTOCOL_BASE_GET_OPTION_REQ(this, buff);
                    goto case 517;
                case 530:
                    receivePacket = (ReceivePacket)new PROTOCOL_BASE_OPTION_SAVE_REQ(this, buff);
                    goto case 517;
                case 536:
                    receivePacket = (ReceivePacket)new PROTOCOL_BASE_USER_LEAVE_REQ(this, buff);
                    goto case 517;
                case 540:
                    receivePacket = (ReceivePacket)new PROTOCOL_BASE_GET_CHANNELLIST_REQ(this, buff);
                    goto case 517;
                case 666:
                    receivePacket = (ReceivePacket)new PROTOCOL_BASE_GET_MAP_INFO_REQ(this, buff);
                    goto case 517;
                case 1057:
                    receivePacket = (ReceivePacket)new PROTOCOL_AUTH_GET_POINT_CASH_REQ(this, buff);
                    goto case 517;
                case 5377:
                    receivePacket = (ReceivePacket)new PROTOCOL_LOBBY_QUICKJOIN_ROOM_REQ(this, buff);
                    goto case 517;
                default:
                    Logger.error("Opcode não encontrada: " + uint16.ToString());
                    goto case 517;
            }
        }

    private class StateObject
    {
      public Socket workSocket;
      public const int BufferSize = 8096;
      public byte[] buffer = new byte[8096];
    }
  }
}
