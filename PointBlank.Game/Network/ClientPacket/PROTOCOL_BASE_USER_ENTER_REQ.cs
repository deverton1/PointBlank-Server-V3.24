// Decompiled with JetBrains decompiler
// Type: PointBlank.Game.Network.ClientPacket.PROTOCOL_BASE_USER_ENTER_REQ
// Assembly: PointBlank.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9391C126-F6F2-4165-85EA-1FCDF75131C4
// Assembly location: C:\Users\LucasRoot\Desktop\Servidor BG\PointBlank.Game.exe

using PointBlank.Core;
using PointBlank.Core.Managers.Events;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_USER_ENTER_REQ : ReceivePacket
  {
    private long pId;
    private uint erro;

    public PROTOCOL_BASE_USER_ENTER_REQ(GameClient client, byte[] data) => this.makeme(client, data);

    public override void read()
    {
      int num = (int) this.readC();
      this.pId = this.readQ();
    }

    public override void run()
    {
      if (this._client == null)
        return;
      try
      {
        if (this._client._player != null)
        {
          this.erro = 2147483648U;
        }
        else
        {
          PointBlank.Game.Data.Model.Account accountDb = AccountManager.getAccountDB((object) this.pId, 2, 0);
          if (accountDb != null && accountDb._status.serverId == (byte) 0)
          {
            this._client.player_id = accountDb.player_id;
            accountDb._connection = this._client;
            accountDb.GetAccountInfos(29);
            accountDb.LoadInventory();
            accountDb.LoadMissionList();
            accountDb.LoadPlayerBonus();
            this.EnableQuestMission(accountDb);
   
            accountDb._inventory.LoadBasicItems();


                        if (accountDb.pc_cafe == 1)
                        {
                            accountDb._inventory.LoadVipBasic();
                        }
                        else if (accountDb.pc_cafe == 2)
                        {
                            accountDb._inventory.LoadVipPlus();
                        }
                        else if (accountDb.pc_cafe == 3)
                        {
                            accountDb._inventory.LoadVipMaster();
                        }
                        else if (accountDb.pc_cafe == 4)
                        {
                            accountDb._inventory.LoadVipCombat();
                        }
                        else if (accountDb.pc_cafe == 5)
                        {
                            accountDb._inventory.LoadVipExtreme();
                        }
                        else if (accountDb.pc_cafe == 6)
                        {
                            accountDb._inventory.LoadVipBooster();
                        }

                        accountDb.SetPublicIP(this._client.GetAddress());
            accountDb.Session = new PlayerSession()
            {
              _sessionId = this._client.SessionId,
              _playerId = this._client.player_id
            };
            accountDb.updateCacheInfo();
            accountDb._status.updateServer((byte) GameConfig.serverId);
            this._client._player = accountDb;
            ComDiv.updateDB("players", "lastip", (object) accountDb.PublicIP.ToString(), "player_id", (object) accountDb.player_id);
          }
          else
            this.erro = 2147483648U;
        }
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_USER_ENTER_ACK(this.erro));
        if (this.erro <= 0U)
          return;
        this._client.Close(500);
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BASE_USER_ENTER_REQ: " + ex.ToString());
        this._client.Close(0);
      }
    }

    private void EnableQuestMission(PointBlank.Game.Data.Model.Account player)
    {
      PlayerEvent playerEvent = player._event;
      if (playerEvent == null || playerEvent.LastQuestFinish != 0 || EventQuestSyncer.getRunningEvent() == null)
        return;
      player._mission.mission4 = 13;
    }
  }
}
