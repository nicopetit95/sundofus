using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Effects
{
    class EffectAction
    {
        public static void ParseEffect(Characters.Character client, int type, string args)
        {
            var datas = args.Split(',');

            switch (type)
            {
                case 0: //Telep
                    client.TeleportNewMap(int.Parse(datas[0]), int.Parse(datas[1]));
                    break;

                case 1: //Life
                    client.AddLife(int.Parse(datas[0]));
                    break;
            }
        }
    }
}
