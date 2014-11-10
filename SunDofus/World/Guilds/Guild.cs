using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Guilds
{
    class Guild
    {
        public bool IsNewGuild { get; set; }
        public bool MustDelete { get; set; }

        public string Emblem
        {
            get
            {
                return string.Concat(Utilities.Basic.ToBase36(BgID), ",", Utilities.Basic.ToBase36(BgColor), ",",
                    Utilities.Basic.ToBase36(EmbID), ",", Utilities.Basic.ToBase36(EmbColor));
            }
        }

        public string Name { get; set; }

        public int ID { get; set; }
        public int BgID { get; set; }
        public int BgColor { get; set; }
        public int EmbID { get; set; }
        public int EmbColor { get; set; }

        public int Level { get; set; }
        public long Exp { get; set; }
        public int CollectorMax { get; set; }
        public int CollectorWisdom { get; set; }
        public int CollectorProspection { get; set; }
        public int CollectorPods { get; set; }
        public int BoostPoints { get; set; }

        public List<GuildMember> Members;
        public List<GuildCollector> Collectors;
        public Dictionary<int, int> Spells;

        public Guild()
        {
            Members = new List<GuildMember>();
            Collectors = new List<GuildCollector>();
            Spells = new Dictionary<int, int>();
        }

        public void AddMember(GuildMember member)
        {
            Members.Add(member);
            member.Character.Guild = this;

            if (Members.Count < 2)
            {
                member.Rights = 1;
                member.Rank = 1;
            }
            else
            {
                member.Rights = 0;
                member.Rank = 0;
            }
        }

        public void Send(string message)
        {
            Members.Where(x => x.Character.IsConnected).ToList().ForEach(x => x.Character.NClient.Send(message));
        }

        public void SendMessage(string message)
        {
            Members.Where(x => x.Character.IsConnected).ToList().ForEach(x => x.Character.NClient.Send(string.Concat("cs<font color=\"663399\">", message, "</font>")));
        }

        public string GetSpells()
        {
            var packet = string.Empty;

            foreach (var spell in Spells.Keys)
                packet = string.Format("{0}{1};{2}|", packet, spell, Spells[spell]);

            return (packet != "" ? packet.Substring(0, packet.Length - 1) : packet);
        }

        public string GetSqlStats()
        {
            return string.Format("{0}~{1}~{2}~{3}~{4}~{5}", CollectorMax, CollectorPods, CollectorProspection, CollectorWisdom, BoostPoints, GetSpells());
        }

        public string GetSqlMembers()
        {
            return string.Join("|", from c in Members select c.SqlToString());
        }

        public string GetSqlEmblem()
        {
            return string.Format("{0};{1};{2};{3}", BgID, BgColor, EmbID, EmbColor);
        }

        public void ParseSqlStats(string datas)
        {
            var infos = datas.Split('~');

            CollectorMax = int.Parse(infos[0]);
            CollectorPods = int.Parse(infos[1]);
            CollectorProspection = int.Parse(infos[2]);
            CollectorWisdom = int.Parse(infos[3]);
            BoostPoints = int.Parse(infos[4]);

            foreach (var spell in infos[5].Split('|'))
            {
                var spellinfos = spell.Split(';');
                Spells.Add(int.Parse(spellinfos[0]), int.Parse(spellinfos[1]));
            }
        }

        public void ParseSqlMembers(string datas)
        {
            foreach (var c in datas.Split('|'))
            {
                var memberInfos = c.Split(';');

                if (!SunDofus.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == int.Parse(memberInfos[0])))
                    continue;

                var character = SunDofus.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == int.Parse(memberInfos[0]));
                character.Guild = this;

                var member = new GuildMember(character);

                member.Rank = int.Parse(memberInfos[1]);
                member.ExpGaved = int.Parse(memberInfos[2]);
                member.ExpGived = int.Parse(memberInfos[3]);
                member.Rights = int.Parse(memberInfos[4]);

                Members.Add(member);
            }
        }

        public void ParseEmblem(string datas)
        {
            var infos = datas.Split(';');

            BgID = int.Parse(infos[0]);
            BgColor = int.Parse(infos[1]);
            EmbID = int.Parse(infos[2]);
            EmbColor = int.Parse(infos[3]);
        }
    }
}
