using SunDofus.World.Maps.Fights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Monsters
{
    class MonsterIA
    {
        public MonsterIA(MonsterFighter player)
        {
            //Play

            player.Fight.State = FightState.WAITING;
            player.Fight.PlayerTurnReady(player);
        }
    }
}
