using PointBlank.Core.Models.Account;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Managers;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Managers;
using PointBlank.Game.Data.Sync.Server;
using PointBlank.Game.Network.ServerPacket;
using PointBlank.Core.Network;
using PointBlank.Core.Models.Account.Players;
using System.Security.Policy;


namespace PointBlank.Game.Data.API
{

    internal class API_SendItem
    {
        public static void SendByNick(string nick, int itemid, int cash, int qtd)
        {
            Account pR = AccountManager.getAccount(nick, 1, 0);
            BaseGiveCash(pR, itemid, cash, qtd);
        }
        public static void SendById(long pId, int itemid, int cash, int qtd)
        {
            Account pR = AccountManager.getAccount(pId, 0);
            BaseGiveCash(pR, itemid, cash, qtd);
        }

        private static void BaseGiveCash(Account player, int itemid, int cash, int qtd)
        {
            if (player == null)
            {
                return;
            }
            if (player._money < cash)
            {
                return;
            }
            var itemGood = ShopManager.getItemId(itemid);

            if (itemGood == null)
            {
                return;
            }
            ItemsModel item = new ItemsModel(itemid)
            {
                _name = itemGood._item._name,
                _count = qtd,
                _equip = 1
            };
            int total = (player._money - cash);
            if (PlayerManager.updateAccountCash(player.player_id, total))
            {
                player._money = total;
                player.SendPacket((SendPacket)new PROTOCOL_AUTH_GET_POINT_CASH_ACK(0, player._gp, player._money, player._tag), false);
                SendItemInfo.LoadGoldCash(player);
                if (player._isOnline)
                {
                    if (InGame(player))
                    {
                        player.SendPacket(new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK($"Parabéns! Você comprou recebeu {item._name} verifique seu inventario."));
                    }
                    else
                    {
                        Message msg = new Message(3)
                        {
                            sender_name = "[WEBSHOP] POINT FIRE",
                            sender_id = 0,
                            text = $"Parabéns! Você comprou recebeu {item._name} verifique seu inventario.",
                            state = 1
                        };

                        player.SendPacket(new PROTOCOL_MESSENGER_NOTE_RECEIVE_ACK(msg), false);
                        MessageManager.CreateMessage(player.player_id, msg);
                    }

                    player.SendPacket((SendPacket)new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, item));

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
