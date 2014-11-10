using SunDofus.World.Characters;
using SunDofus.World.Characters.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Fights
{
    class CharacterFighter : Fighter
    {
        private Character myCharacter;

        public CharacterFighter(Character player, Fight fight)
            : base(FighterType.CHARACTER, fight)
        {
            player.Fight = fight;
            player.Fighter = this;

            myCharacter = player;

            AP = Stats.GetStat(StatEnum.MaxPA).Total;
            MP = Stats.GetStat(StatEnum.MaxPM).Total;
        }

        public override int ID
        {
            get { return myCharacter.ID; }
        }

        public override string Name
        {
            get { return myCharacter.Name; }
        }

        public override int Level
        {
            get { return myCharacter.Level; }
        }

        public override int Life
        {
            get { return myCharacter.Life; }
            set { myCharacter.Life = value; }
        }

        public override int Skin
        {
            get { return myCharacter.Skin; }
            set { myCharacter.Skin = value; }
        }

        public override GenericStats Stats
        {
            get { return myCharacter.Stats; }
        }

        public override Character Character
        {
            get { return myCharacter; }
        }

        public override string GetPattern()
        {
            return myCharacter.PatternFightDisplayChar();
        }
    }
}
