using SunDofus.World.Characters.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Entities.Models.Clients
{
    class GiftModel
    {
        public int ID { get; set; }
        public int Target { get; set; }
        public int ItemID { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }

        public CharacterItem Item { get; set; }

        public override string ToString()
        {
            return string.Format("{0}~{1}~{2}~{3}~{4}", ID, Title, Message, ItemID, Image);
        }
    }
}
