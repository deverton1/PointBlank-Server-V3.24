using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Managers;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Managers;
using PointBlank.Game.Data.Sync.Server;
using PointBlank.Game.Network.ServerPacket;
using PointBlank.Core.Network;
using PointBlank.Core.Models.Account;
using PointBlank.Core.Progress;

namespace PointBlank.Game.Data.API
{
    
    internal class API_SendCash
    {
        public static void SendByNick(string nick, int valor)
        {
            Account pR = AccountManager.getAccount(nick, 1, 0);
            BaseGiveCash(pR, valor);
        }
        public static void SendById(long pId, int valor)
        {
            Account pR = AccountManager.getAccount(pId, 0);
            BaseGiveCash(pR, valor);
        }

        private static void BaseGiveCash(Account player, int cash)
        {
            if (player == null)
            {
                //Printf.danger("[API-SendCash] Falha ao adicionar cash! Player Nulo");
                return;
            }
            if (player._money + cash > 999999999)
            {
                //Printf.danger("[API_SendCash] A soma ultrapassou o limite!");
                return;
            }
            if (PlayerManager.updateAccountCash(player.player_id, player._money + cash))
            {
                player._money += cash;
                player.SendPacket((SendPacket)new PROTOCOL_AUTH_GET_POINT_CASH_ACK(0, player._gp, player._money, player._tag), false);
                SendItemInfo.LoadGoldCash(player);
                if (player._isOnline)
                {
                    if (InGame(player)){
                        player.SendPacket(new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK("Você recebeu " + cash + " de cash!"));
                    }
                    else {
                        Message msg = new Message(3)
                        {
                            sender_name = "[CASH] POINT FIRE",
                            sender_id = 0,
                            text = $"Parabéns! Você recebeu {cash} cash.",
                            state = 1
                        };

                        player.SendPacket(new PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK(msg), false);
                        MessageManager.CreateMessage(player.player_id, msg);
                    }
                }
            }
            else
            {
                //Printf.danger("[API.SendCash] Falha ao adicionar cash ao player: " + player.player_name);
            }

        }

        private static bool InGame(Account player)
        {
            Room room = player._room;
            Slot slot;
            if (room != null && room.getSlot(player._slotId, out slot) && (int)slot.state >= 9)
                return true;
            return false;
        }
    }
}
