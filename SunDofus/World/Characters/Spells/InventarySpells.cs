using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Characters.Spells
{
    class InventarySpells
    {
        public List<CharacterSpell> Spells { get; set; }
        public Character Client { get; set; }

        public InventarySpells(Character client)
        {
            Spells = new List<CharacterSpell>();
            Client = client;
        }

        public void ParseSpells(string datas)
        {
            var spells = datas.Split('|');

            foreach (var spell in spells)
            {
                var infos = spell.Split(',');
                AddSpells(int.Parse(infos[0]), int.Parse(infos[1]), int.Parse(infos[2]));
            }
        }

        public void LearnSpells()
        {
            foreach (var spell in Entities.Requests.SpellsRequests.SpellsToLearnList.Where(x => x.Race == Client.Class && x.Level <= Client.Level))
            {
                if (Spells.Any(x => x.ID == spell.SpellID))
                    continue;

                AddSpells(spell.SpellID, 1, spell.Pos);
            }
        }

        public void AddSpells(int id, int level, int pos)
        {
            if (Spells.Any(x => x.ID == id)) 
                return;

            if (level < 1) level = 1;
            if (level > 6) level = 6;

            if (pos > 25) pos = 25;
            if (pos < 1) pos = 25;

            lock(Spells)
                Spells.Add(new CharacterSpell(id, level, pos));
        }

        public void SendAllSpells()
        {
            var packet = "";

            foreach (var spell in Spells)
                packet += string.Format("{0}~{1}~{2};", spell.ID, spell.Level, Maps.Pathfinding.GetDirChar(spell.Position));

            Client.NClient.Send(string.Concat("SL", packet));
        }

        public string SaveSpells()
        {
            return string.Join("|", Spells);
        }
    }
}
