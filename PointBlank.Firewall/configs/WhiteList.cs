using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PointBlank.Core.Network;
using PointBlank.Core.Filters;
using Newtonsoft.Json;
namespace PointBlank.Firewall.configs
{
    public class WhiteList
    {
        private class whiteListJsonMODEL
        {
            public string _address { get; set; }
            public byte _cidr { get; set; }
        }
        public class whiteListModel
        {
            public byte _cidr;
            public IPAddress _address;
        }
        public static List<whiteListModel> _whiteList = new List<whiteListModel>();


        public static void Load()
        {
            try
            {
                string path = "config/api-protection/ignore.json";
                if (!File.Exists(path))
                {
                    Logger.error("[WhiteList] Não existe o arquivo: " + path);
                    return;
                }

                var result = JsonConvert.DeserializeObject<List<whiteListJsonMODEL>>(File.ReadAllText(path));


                for (byte i = 0; i < result.Count; i++)
                {
                    _whiteList.Add(new whiteListModel()
                    {
                        _cidr = result[i]._cidr,

                        _address = IPAddress.Parse(result[i]._address),

                    });

                }
            }
            catch (Exception)
            {
                Logger.error("[WhiteList.Load] Erro fatal!");
            }
        }

        public static bool check(IPAddress ipForCheck)
        {
            for (short i = 0; i < _whiteList.Count; i++)
            {
                whiteListModel whiteCheck = _whiteList[i];

                if (ipForCheck.checkInSubNet(whiteCheck._address, whiteCheck._cidr))
                {
                    Logger.debug("[WhiteList] Nao foi banido " + ipForCheck);
                    return true;
                }
            }
            return false;
        }
    }
}
