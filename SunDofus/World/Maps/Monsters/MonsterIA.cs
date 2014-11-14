using SunDofus.World.Characters.Stats;
using SunDofus.World.Maps.Fights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SunDofus.World.Maps.Monsters
{
    class MonsterIA
    {
        public MonsterIA(MonsterFighter player)
        {
            Thread t = new Thread(() =>
            {
                switch (player.IA)
                {
                    case 1:
                        while(!player.Dead)
                        {
                            Fighter enemy = GetNearestEnnemy(player.Fight, player);
                            Fighter friend = GetNearestFriend(player.Fight, player);

                            //TODO play

                            break;
                        }
                        break;
                }

                System.Threading.Thread.Sleep(1000);

                player.Fight.State = FightState.WAITING;
                player.Fight.PlayerTurnReady(player);
            });

            t.Start();
        }

        private Fighter GetNearestEnnemy(Fight fight, MonsterFighter player)
        {
            int dist = 1000;
            Fighter curF = null;

            foreach (Fighter f in fight.GetFighters().Where(x => !x.Dead && x.Team != player.Team && !x.Buffs.HasBuff(EffectEnum.Invisible, BuffActiveType.ACTIVE_STATS)).ToList())
            {
                int d = Pathfinding.GetDistanceBetween(fight.Map, player.Cell, f.Cell);
                if (d < dist)
                {
                    dist = d;
                    curF = f;
                }
            }

            return curF;
        }

        private Fighter GetNearestFriend(Fight fight, MonsterFighter player)
        {
            int dist = 1000;
            Fighter curF = null;

            foreach (Fighter f in fight.GetFighters().Where(x => !x.Dead && x.Team == player.Team && !x.Buffs.HasBuff(EffectEnum.Invisible, BuffActiveType.ACTIVE_STATS)).ToList())
            {
                int d = Pathfinding.GetDistanceBetween(fight.Map, player.Cell, f.Cell);
                if (d < dist)
                {
                    dist = d;
                    curF = f;
                }
            }

            return curF;
        }
    }
}
