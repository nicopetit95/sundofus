using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Characters.Items
{
    class CharacterSet
    {
        public int ID { get; set; }

        public List<int> ItemsList { get; set; }
        public Dictionary<int, List<Effects.EffectItem>> BonusList { get; set; }

        public CharacterSet(int id)
        {
            ID = id;

            ItemsList = new List<int>();
            BonusList = Entities.Requests.ItemsRequests.SetsList.First(x => x.ID == ID).BonusList;
            BonusList[1] = new List<Effects.EffectItem>();
        }
    }
}
