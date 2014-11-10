using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Zaapis
{
    class ZaapisManager
    {
        public static void SendZaapis(Characters.Character character)
        {
            if (Entities.Requests.ZaapisRequests.ZaapisList.Any(x => x.MapID == character.MapID))
            {
                var zaapis = Entities.Requests.ZaapisRequests.ZaapisList.First(x => x.MapID == character.MapID);
                var packet = string.Format("Wc{0}|", character.MapID);

                if ((zaapis.Faction == 1 && character.Faction.ID == 2) || (zaapis.Faction == 2 && character.Faction.ID == 1))
                {
                    character.NClient.Send("Im1196");
                    return;
                }

                Entities.Requests.ZaapisRequests.ZaapisList.Where(x => x.Faction == zaapis.Faction).ToList().
                    ForEach(x => packet = string.Concat(packet, x.MapID, (character.Faction.ID == x.Faction ? ";10|" : ";20|")));

                character.NClient.Send(packet);
            }
            else
                character.NClient.Send("BN");
        }

        public static void OnMove(Characters.Character character, int nextZaapis)
        {
            if (Entities.Requests.ZaapisRequests.ZaapisList.Any(x => x.MapID == nextZaapis))
            {
                var zaapis = Entities.Requests.ZaapisRequests.ZaapisList.First(x => x.MapID == nextZaapis);

                if ((zaapis.Faction == 1 && character.Faction.ID == 2) || (zaapis.Faction == 2 && character.Faction.ID == 1))
                {
                    character.NClient.Send("Im1196");
                    return;
                }

                var price = (character.Faction.ID == zaapis.Faction ? 10 : 20);

                character.Kamas -= price;
                character.NClient.Send(string.Concat("Im046;", price));
                character.TeleportNewMap(zaapis.MapID, zaapis.CellID);

                character.NClient.Send("Wv");

                character.SendChararacterStats();
            }
            else
                character.NClient.Send("BN");
        }
    }
}
