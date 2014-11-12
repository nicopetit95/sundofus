using SunDofus.World.Characters.Stats;
using SunDofus.World.Maps.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunDofus.World.Maps.Fights
{
    class MonsterFighter : Fighter
    {
        private Monster myMonster;
        private int id;
        private GenericStats stats;
        private int life;

        public int InitCell { get; set; }

        public MonsterFighter(Monster monster, Fight fight, int nId) 
            : base(FighterType.MONSTER, fight)
        {
            myMonster = monster;
            id = nId;

            stats = new GenericStats();
            life = monster.Life;
            stats.GetStat(StatEnum.Vitalite).Base = monster.Life;
            stats.GetStat(StatEnum.MaxPA).Base = monster.MonsterLevel.AP;
            stats.GetStat(StatEnum.MaxPM).Base = monster.MonsterLevel.MP;
        }

        public override int ID
        {
            get { return id; }
        }

        public override string Name
        {
            get { return myMonster.Model.Name; }
        }

        public override int Level
        {
            get { return myMonster.Level; }
        }

        public override int Life
        {
            get { return life; }
            set { life = value; }
        }

        public override int Skin
        {
            get { return myMonster.Model.GfxID; }
            set { myMonster.Model.GfxID = value; }
        }

        public override Characters.Stats.GenericStats Stats
        {
            get { return stats; }
        }

        public override Characters.Character Character
        {
            get { return null; }
        }

        public override string GetPattern()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(this.Cell).Append(";1;0;");
                builder.Append(ID).Append(";");
                builder.Append(myMonster.Model.ID).Append(";-2;");

                builder.Append(myMonster.Model.GfxID).Append("^100;");
                builder.Append(myMonster.Level).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(myMonster.Model.Color)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(myMonster.Model.Color2)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(myMonster.Model.Color3)).Append(";");
                builder.Append("0,0,0,0;");
                builder.Append(myMonster.Life).Append(";");
                builder.Append(stats.GetStat(StatEnum.MaxPA).Total).Append(";");
                builder.Append(stats.GetStat(StatEnum.MaxPM).Total).Append(";1");
            }

            return builder.ToString();
        }
    }
}
