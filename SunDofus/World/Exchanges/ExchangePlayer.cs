using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Exchanges
{
    class ExchangePlayer
    {
        public bool IsNpc { get; set; }

        public Characters.Character Character { get; set; }
        public Characters.NPC.NPCMap Npc { get; set; }

        public List<ExchangeItem> Items { get; set; }

        public int ID { get; set; }
        public long Kamas { get; set; }

        public ExchangePlayer(Characters.Character _character)
        {
            IsNpc = false;
            Character = _character;

            Items = new List<ExchangeItem>();
        }

        public ExchangePlayer(Characters.NPC.NPCMap _npc)
        {
            IsNpc = true;
            Npc = _npc;

            Items = new List<ExchangeItem>();
        }

        public void Send(string message)
        {
            if (!IsNpc)
                Character.NClient.Send(message);
        }
    }
}
