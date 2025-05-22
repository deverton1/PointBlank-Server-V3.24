// Decompiled with JetBrains decompiler
// Type: PointBlank.Auth.Network.ClientPacket.PROTOCOL_BASE_LOGIN_REQ
// Assembly: PointBlank.Auth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2F2D71E3-3E87-4155-AA64-E654DA3CFF2D
// Assembly location: C:\Users\LucasRoot\Desktop\Servidor BG\PointBlank.Auth.exe

using PointBlank.Auth.Data.Configs;
using PointBlank.Auth.Data.Managers;
using PointBlank.Auth.Data.Model;
using PointBlank.Auth.Data.Sync;
using PointBlank.Auth.Data.Sync.Server;
using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core;
using PointBlank.Core.Filters;
using PointBlank.Core.Managers;
using PointBlank.Core.Managers.Server;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using System;
using System.Net.NetworkInformation;
using System.Text;

namespace PointBlank.Auth.Network.ClientPacket
{
    public class PROTOCOL_BASE_LOGIN_REQ : ReceivePacket
    {
        private string Token;
        private string GameVersion;
        private string PublicIP;
        private int TokenSize;
        private ClientLocale GameLocale;
        private PhysicalAddress MacAddress;

        public PROTOCOL_BASE_LOGIN_REQ(AuthClient client, byte[] data) => makeme(client, data);

        //public static string Reverse(string s)
        //{
        //    char[] charArray = s.ToCharArray();
        //    Array.Reverse(charArray);
        //    return new string(charArray);
        //}
        public override void read()
        {
            readB(readC());
            readB(16);
            readS(32);
            readB(26);
            int num1 = readC();
            GameVersion = readC().ToString() + "." + readH().ToString();
            TokenSize = readH();
            Token = (readS(TokenSize));
            int num2 = readC();
            int num3 = readH();
            PublicIP = _client.GetIPAddress();
            GameLocale = ClientLocale.Thai;
            MacAddress = new PhysicalAddress(new byte[6]);
        }

        public override void run()
        {
            try
            {
                //Logger.console($"token  : {Token}");
                //if (this.PublicIP == null || this.Token.Trim().Length == 0 || !Token.Contains("~C0mb4t"))
                if (PublicIP == null || Token.Trim().Length == 0)
                { 
                _client.Close(0, true);
                }
                //Token = Token.Replace("~C0mb4t", "");
                ServerConfig config = AuthManager.Config;
                if (config == null || !AuthConfig.isTestMode && !AuthConfig.GameLocales.Contains(GameLocale) || 
                    Token.Length < AuthConfig.minTokenSize || GameVersion != config.ClientVersion)
                {
                    string text = "";
                    if (config == null)
                        text = "Invalid server setting [" + Token + "]";
                    else if (!AuthConfig.isTestMode && !AuthConfig.GameLocales.Contains(GameLocale))
                        text = GameLocale.ToString() + " Bloqueado [" + Token + "]";
                    else if (Token.Length < AuthConfig.minTokenSize)
                        text = "Token < Size [" + Token + "]";
                    else if (GameVersion != config.ClientVersion)
                        text = "Version: " + GameVersion + " not compatible [" + Token + "]";
                    _client.SendPacket(new PROTOCOL_SERVER_MESSAGE_DISCONNECTIONSUCCESS_ACK(2147483904U, false));
                    Logger.LogLogin(text);
                    _client.Close(1000, true);
                }
                else
                {
                    _client._player = AccountManager.getInstance().getAccountDB(Token, null, 0, 0);
                    if (_client._player == null && AuthConfig.AUTO_ACCOUNTS && !AccountManager.getInstance().CreateAccount(out _client._player, Token))
                    {
                        _client.SendPacket(new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_DELETE_ACCOUNT, "", 0L));
                        Logger.LogLogin("Failed to create account automatically");
                        _client.Close(1000, false);
                    }
                    else
                    {
                        Account player = _client._player;
                        if (player == null || !player.CompareToken(Token))
                        {
                            string str = "";
                            if (player == null)
                                str = "Invaild Account";
                            else if (!player.CompareToken(Token))
                                str = "Invalid Token";
                            _client.SendPacket(new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_INVALID_ACCOUNT, "", 0L));
                            Logger.LogLogin(str + " [" + Token + "]");
                            _client.Close(1000, false);
                        }
                        else if (player.access >= 0)
                        {
                           // if (player.MacAddress != MacAddress)
                           //     ComDiv.updateDB("players", "last_mac", MacAddress, "player_id", player.player_id);
                            bool validMac;
                            bool validIp;
                            BanManager.GetBanStatus(MacAddress.ToString(), PublicIP, out validMac, out validIp);
                            if (validMac | validIp)
                            {
                                if (validMac)
                                    Logger.LogLogin("Mac banned [" + player.login + "]");
                                else
                                    Logger.LogLogin("Ip banned [" + player.login + "]");
                                _client.SendPacket(new PROTOCOL_BASE_LOGIN_ACK(validIp ? EventErrorEnum.LOGIN_BLOCK_IP : EventErrorEnum.LOGIN_BLOCK_ACCOUNT, "", 0L));
                                _client.Close(1000, false);
                            }
                            else if (player.IsGM() && config.onlyGM || player.access >= 0 && !config.onlyGM)
                            {
                                Account account = AccountManager.getInstance().getAccount(player.player_id, true);
                                if (!player._isOnline)
                                {
                                    if (BanManager.GetAccountBan(player.ban_obj_id).endDate > DateTime.Now)
                                    {
                                        _client.SendPacket(new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_BLOCK_ACCOUNT, "", 0L));
                                        Logger.LogLogin("Account with ban active [" + player.login + "]");
                                        _client.Close(1000, false);
                                    }
                                    else if (CheckHwId(account.hwid))
                                    {
                                        _client.SendPacket(new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_BLOCK_ACCOUNT, "", 0L));
                                        Logger.LogLogin("Ban HwId [" + player.login + "]");
                                        _client.Close(1000, false);
                                    }
                                    else
                                    {
                                        if (account != null)
                                            account._connection = _client;

                                                string jogador = player.player_name.Length == 0 ? "Nova conta" : "Nickaname: "+player.player_name;
                                  
                                          
                                                long LastDate = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                                                //string TokenNew = Guid.NewGuid().ToString("N").Substring(10, 10);
                                                player.SetPlayerId(player.player_id, 31);
                                                player._clanPlayers = ClanManager.getClanPlayers(player.clan_id, player.player_id);
                                                _client.SendPacket(new PROTOCOL_BASE_LOGIN_ACK(0, player.login, player.player_id));

                                                //Jogo pede dnv no pacote apropriado
                                               // _client.SendPacket(new PROTOCOL_AUTH_GET_POINT_CASH_ACK(0, player._gp, player._money, player._tag));
                                                
                                                if (player.clan_id > 0)
                                                    _client.SendPacket(new PROTOCOL_CS_MEMBER_INFO_ACK(player._clanPlayers));
                                                player._status.SetData(uint.MaxValue, player.player_id);
                                                player._status.updateServer(0);
                                                player.setOnlineStatus(true);
                                                SendRefresh.RefreshAccount(player, true);
                                                Firewall.sendAllow(PublicIP);
                                                if(ComDiv.updateDB("players", "last_login", LastDate, "player_id", player.player_id))
                                                Logger.LogYaz("PlayerID: " + player.player_id + " " + jogador + " ", ConsoleColor.Yellow);

                                                 if (!AuthConfig.ClearToken)
                                                    return;
                                        if (ComDiv.updateDB("players", "token", (object)DBNull.Value, "player_id", player.player_id))
                                        {
                                            Logger.LogYaz("PlayerID: " + player.player_id + " " + jogador + "  ", ConsoleColor.Red);
                                        }
                                    }
                                }
                                else
                                {
                                    _client.SendPacket(new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_ALREADY_LOGIN_WEB, "", 0L));
                                    Logger.LogLogin("Online account [" + player.login + "]");
                                    if (account != null && account._connection != null)
                                    {
                                        account.SendPacket(new PROTOCOL_AUTH_ACCOUNT_KICK_ACK(1));
                                        account.SendPacket(new PROTOCOL_SERVER_MESSAGE_ERROR_ACK(2147487744U));
                                        account.Close(1000);
                                    }
                                    else
                                        AuthSync.SendLoginKickInfo(player);
                                    _client.Close(1000, false);
                                }
                            }
                            else
                            {
                                _client.SendPacket(new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_TIME_OUT_2, "", 0L));
                                Logger.LogLogin("Invalid Access [" + player.login + "]");
                                _client.Close(1000, false);
                            }
                        }
                        else
                        {
                            _client.SendPacket(new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.LOGIN_BLOCK_ACCOUNT, "", 0L));
                            Logger.LogLogin("Permanent ban [" + player.login + "]");
                            _client.Close(1000, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.warning("PROTOCOL_BASE_LOGIN_REQ: " + ex.ToString());
            }
        }

        public bool CheckHwId(string PlayerHwId)
        {
            foreach (string hwId in BanManager.GetHwIdList())
            {
                if ((PlayerHwId.Length != 0 || hwId.Length != 0 || hwId != null || hwId == "") && PlayerHwId == hwId)
                    return true;
            }
            return false;
        }
    }
}
