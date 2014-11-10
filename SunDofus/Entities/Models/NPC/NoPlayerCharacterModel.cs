using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Entities.Models.NPC
{
    class NoPlayerCharacterModel
    {
        public int ID { get; set; }
        public int GfxID { get; set; }
        public int Size { get; set; }
        public int Sex { get; set; }

        public int Color { get; set; }
        public int Color2 { get; set; }
        public int Color3 { get; set; }

        //public int ArtWork;
        //public int Bonus;

        public NPCsQuestion Question { get; set; }

        public string Name { get; set; }
        public string Items { get; set; }

        public List<int> SellingList { get; set; }

        public NoPlayerCharacterModel()
        {
            SellingList = new List<int>();
            Question = null;
        }
    }
}
