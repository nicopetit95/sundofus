using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Zaaps
{
    class ZaapsManager
    {
        public static void SendZaaps(Characters.Character character)
        {
            if (Entities.Requests.ZaapsRequests.ZaapsList.Any(x => x.MapID == character.MapID))
            {
                var zaap = Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == character.MapID);

                if (!character.Zaaps.Contains(zaap.MapID))
                {
                    character.Zaaps.Add(zaap.MapID);
                    character.NClient.Send("Im024");
                }

                var savepos = (Entities.Requests.ZaapsRequests.ZaapsList.Any(x => x.MapID == character.SaveMap) ?
                    Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == character.SaveMap).MapID.ToString() : "");
                var packet = string.Format("WC{0}|", savepos);

                Entities.Requests.ZaapsRequests.ZaapsList.Where(x => character.Zaaps.Contains(x.MapID)).ToList().
                    ForEach(x => packet = string.Format("{0}{1};{2}|", packet, x.MapID, CalcPrice(character.GetMap(), x.Map)));

                character.NClient.Send(packet.Substring(0, packet.Length - 1));
            }
            else
                character.NClient.Send("BN");
        }

        public static void SaveZaap(Characters.Character character)
        {
            if (Entities.Requests.ZaapsRequests.ZaapsList.Any(x => x.MapID == character.MapID))
            {
                var zaap = Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == character.MapID);

                character.SaveMap = zaap.MapID;
                character.SaveCell = zaap.CellID;

                character.NClient.Send("Im06");
            }
            else
                character.NClient.Send("BN");
        }

        public static void OnMove(Characters.Character character, int nextZaap)
        {
            if (Entities.Requests.ZaapsRequests.ZaapsList.Any(x => x.MapID == nextZaap))
            {
                var zaap = Entities.Requests.ZaapsRequests.ZaapsList.First(x => x.MapID == nextZaap);

                var price = CalcPrice(character.GetMap(), zaap.Map);

                character.Kamas -= price;
                character.NClient.Send(string.Concat("Im046;", price));
                character.TeleportNewMap(zaap.MapID, zaap.CellID);

                character.NClient.Send("WV");

                character.SendChararacterStats();
            }
            else
                character.NClient.Send("BN");
        }

        private static int CalcPrice(Map startMap, Map nextMap)
        {
            return (int)(10 * (Math.Abs(nextMap.Model.PosX - startMap.Model.PosX) + Math.Abs(nextMap.Model.PosY - startMap.Model.PosY)));
        }
    }
}
