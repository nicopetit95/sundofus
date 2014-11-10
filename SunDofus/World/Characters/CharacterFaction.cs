using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Characters
{
    class CharacterFaction
    {
        private Character character;

        public CharacterFaction(Character character)
        {
            this.character = character;
        }

        public bool IsEnabled { get; set; }

        public int ID { get; set; }
        public int Honor { get; set; }
        public int Deshonor { get; set; }
        public int Level { get; set; }

        public string AlignementInfos
        {
            get
            {
                return string.Format("{0},{1},{2},{3}", ID, ID, (IsEnabled ? Level.ToString() : "0"), (character.Level + character.ID));
            }
        }

        public override string ToString()
        {
            return string.Format("{0}~2,{1},{2},{3},{4},{5}", ID, Level, Level, Honor, Deshonor, (IsEnabled ? "1" : "0"));
        }
    }
}
