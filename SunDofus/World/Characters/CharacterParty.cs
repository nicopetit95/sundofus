using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Characters
{
    class CharacterParty
    {
        public Dictionary<Character, int> Members { get; set; }

        private string ownerName;
        private int ownerID;

        public CharacterParty(Character leader)
        {
            Members = new Dictionary<Character, int>();

            lock (Members)
                Members.Add(leader, 1);

            ownerID = leader.ID;
            ownerName = leader.Name;
        }

        public void AddMember(Character member)
        {
            lock(Members)
                Members.Add(member, 0);

            member.State.Party = this;

            if (Members.Count == 2)
            {
                Send(string.Concat("PCK", ownerName));
                Send(string.Concat("PL", ownerID));
                Send(string.Concat("PM", PartyPattern()));
            }
            else
            {
                member.NClient.Send(string.Concat("PCK", ownerName));
                member.NClient.Send(string.Concat("PL", ownerID));
                member.NClient.Send(string.Concat("PM", PartyPattern()));

                foreach (var character in Members.Keys.ToList().Where(x => x != member))
                    character.NClient.Send(string.Concat("PM", character.PatternOnParty()));
            }

            UpdateMembers();
        }

        public void UpdateMembers()
        {
            Send(string.Concat("PM~", string.Join("|", from x in Members.Keys.ToList().OrderByDescending(x => x.Stats.GetStat(Stats.StatEnum.Initiative).Total) select x.PatternOnParty())));
        }

        public void LeaveParty(string name, string kicker = "")
        {
            if (!Members.Keys.ToList().Any(x => x.Name == name) || (kicker != "" && ownerID != int.Parse(kicker)))
                return;

            var character = Members.Keys.ToList().First(x => x.Name == name);
            character.State.Party = null;

            lock (Members)
                Members.Remove(character);

            Send(string.Concat("PM-", character.ID));

            if (character.State.IsFollow)
            {
                character.State.Followers.Clear();
                character.State.IsFollow = false;
            }

            if (character.IsConnected)
                character.NClient.Send(string.Concat("PV", kicker));

            if (Members.Count == 1)
            {
                var last = Members.Keys.ToList()[0];
                last.State.Party = null;

                Members.Remove(last);

                if (last.IsConnected)
                    last.NClient.Send(string.Concat("PV", kicker));
            }
            else if (ownerID == character.ID)
                GetNewLeader();

            if(Members.Count >= 2)
                UpdateMembers();
        }

        private void Send(string text)
        {
            foreach (var character in Members.Keys)
                character.NClient.Send(text);
        }

        private void GetNewLeader()
        {
            var character = Members.Keys.ToList()[0];
            Members[character] = 1;

            ownerID = character.ID;
            ownerName = character.Name;

            Send(string.Concat("PL", ownerID));
        }

        private string PartyPattern()
        {
            return string.Concat("+", string.Join("|", from x in Members.Keys.ToList().OrderByDescending(x => x.Stats.GetStat(Stats.StatEnum.Initiative).Total) select x.PatternOnParty()));
        }
    }
}
