using SunDofus.World.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Fights
{
    public enum ToggleType
    {
        LOCK = 'N',
        HELP = 'H',
        PARTY = 'P',
        SPECTATOR = 'S'
    }

    class FightTeam
    {
        private int myID;
        private int myCell;
        private int[] myPlaces;

        private Fighter myLeader;
        private List<Fighter> myFighters = new List<Fighter>(8);

        private Dictionary<ToggleType, bool> myToggles = new Dictionary<ToggleType, bool>()
        {
            { ToggleType.LOCK, false },
            { ToggleType.HELP, false },
            { ToggleType.PARTY, false },
            { ToggleType.SPECTATOR, false },
        };

        public FightTeam(int ID, int[] places)
        {
            myID = ID;
            myPlaces = places;
        }

        public int ID
        {
            get { return myID; }
        }

        public int Cell
        {
            get { return myCell; }
        }

        public int[] Places
        {
            get { return myPlaces; }
        }

        public Fighter Leader
        {
            get { return myLeader; }
        }

        public Fighter[] GetFighters()
        {
            return myFighters.ToArray();
        }

        public Fighter[] GetAliveFighters()
        {
            return myFighters.Where(x => !x.Dead).ToArray();
        }

        public bool HasAliveFighters()
        {
            return myFighters.Any(x => !x.Dead);
        }

        public int GetAvailablePlace()
        {
            return myPlaces.First(x => !myFighters.Any(f => f.Cell == x));
        }

        public bool IsAvailablePlace(int cell)
        {
            return !myFighters.Any(x => x.Cell == cell);
        }

        public void Send(string packet)
        {
            foreach (Fighter fighter in GetFighters())
                if (fighter.Type == FighterType.CHARACTER) fighter.Character.NClient.Send(packet);
        }

        public void Toggle(ToggleType toggle, bool value)
        {
            lock (myToggles)
                myToggles[toggle] = value;
        }

        public bool IsToggle(ToggleType toggle)
        {
            lock (myToggles)
                return myToggles[toggle];
        }

        public bool CanJoin(Character Character)
        {
            if (myFighters.Count >= 8)
                return false;

            if (IsToggle(ToggleType.LOCK))
                return false;

            if (IsToggle(ToggleType.PARTY))
                if ((Character.State.Party != null) && Character.State.Party.Members.Keys.Any(x => x.ID == myLeader.ID))
                    return true;
                else
                    return false;

            return true;
        }

        public void FighterJoin(Fighter Fighter, bool isLeader = false)
        {
            Fighter.Team = this;
            Fighter.Cell = GetAvailablePlace();

            if (isLeader)
            {
                myLeader = Fighter;

                switch(Fighter.Type)
                {
                    case FighterType.CHARACTER:
                        myCell = Fighter.Character.MapCell;
                        break;

                    case FighterType.MONSTER:
                        myCell = ((MonsterFighter)Fighter).InitCell;
                        break;
                }
            }

            myFighters.Add(Fighter);
        }

        public void FighterLeave(Fighter Fighter)
        {
            myFighters.Remove(Fighter);
        }
    }
}
