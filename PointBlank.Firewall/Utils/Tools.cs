using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointBlank.Firewall.configs;
namespace PointBlank.Firewall.Utils
{
    public class Tools
    {
        public static int getGravit(int x) // Pega gravidade da regra
        {
            switch (x)
            {
                case 0:
                    x = Config.timeBlock[0];
                    break;
                case 1:
                    x = Config.timeBlock[1];
                    break;
                case 2:
                    x = Config.timeBlock[2];
                    break;
                default:
                    x = Config.timeBlock[0];
                    Logger.debug("[Warning] Nivel de gravidade invalido, foi setado para _GRAVE_");
                    break;
            }
            return x;
        }
    }
}
