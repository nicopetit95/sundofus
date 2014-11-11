using SunDofus.World.Characters;
using SunDofus.World.Maps.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SunDofus.World.Maps.Fights
{
    class MonsterFight : Fight
    {
        public MonsterFight(Character character, MonstersGroup group, Map map)
            : base(FightType.PVM, map)
        {
            Fighter attacker = new CharacterFighter(character, this);
            MonsterFighter monster = new MonsterFighter(group.Monsters[0], this);

            monster.InitCell = group.MapCell;
            group.Monsters.Remove(group.Monsters[0]);

            FightInit(attacker, monster);

            foreach (var mob in group.Monsters)
            {
                var otherMonster = new MonsterFighter(mob, this);
                FighterJoin(otherMonster, 1);
            }            
        }

        public override int StartTime()
        {
            return 0;
        }

        public override int TurnTime()
        {
            return 30000;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void PlayerLeave(Fighter fighter)
        {
            switch (State)
            {
                case FightState.STARTING:

                    if (fighter.IsLeader)
                    {
                        fighter.Left = true;

                        FightEnd(GetEnnemyTeam(fighter.Team), fighter.Team);
                    }
                    else
                    {
                        fighter.Team.FighterLeave(fighter);

                        fighter.Character.State.InFight = false;
                        fighter.Character.Fight = null;
                        fighter.Character.Fighter = null;

                        Send(FormatFighterDestroy(fighter));
                        Map.Send(FormatFlagFighterDestroy(fighter));
                        fighter.Character.NClient.Send(FormatLeave());
                    }

                    break;

                case FightState.PLAYING:

                    if (CurrentFighter == fighter)
                        TurnEnd();

                    fighter.Left = true;

                    if (GetWinners() != null)
                    {
                        FightEnd(GetEnnemyTeam(fighter.Team), fighter.Team);
                    }
                    else
                    {
                        fighter.Buffs.Debuff();

                        fighter.Character.State.InFight = false;
                        fighter.Character.Fight = null;
                        fighter.Character.Fighter = null;

                        Send(FormatFighterDestroy(fighter));
                        fighter.Character.NClient.Send(FormatLeave());
                    }

                    break;
            }
        }

        public override void FightEnd(FightTeam winners, FightTeam loosers)
        {
            if (State == FightState.STARTING)
                Map.Send(FormatFlagDestroy());

            StringBuilder builder = new StringBuilder("GE");

            builder.Append(EndTime).Append('|');
            builder.Append(ID).Append('|');
            builder.Append(Type == FightType.AGRESSION ? 1 : 0);

            foreach (Fighter fighter in GetFighters())
            {
                builder.Append('|').Append(fighter.Team == winners ? 2 : 0).Append(';');
                builder.Append(fighter.ID).Append(';');
                builder.Append(fighter.Name).Append(';');
                builder.Append(fighter.Level).Append(';');
                builder.Append(fighter.Dead ? 1 : 0).Append(';');
                builder.Append(";;;;;;;");
            }

            Send(builder.ToString());

            State = FightState.FINISHED;

            foreach (Fighter fighter in GetFighters().OfType<CharacterFighter>())
            {
                if (fighter.Fight == this)
                {
                    fighter.Buffs.Debuff();

                    fighter.Character.State.InFight = false;
                    fighter.Character.Fight = null;
                    fighter.Character.Fighter = null;
                }
            }

            KickSpectator(true);

            Map.RemoveFight(this);
            Map.Send(Map.FormatFightCount());
        }
    }
}
