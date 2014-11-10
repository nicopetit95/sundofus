using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using SunDofus.World.Characters;
using SunDofus.World.Characters.Stats;
using SunDofus.World.Characters.Spells;
using SunDofus.World.Effects;
using SunDofus.Utilities;
using SunDofus.World.Maps.Fights.Effects;

namespace SunDofus.World.Maps.Fights
{
    #region Enums

    public enum FightType
    {
        CHALLENGE = 0,
        AGRESSION = 1,
        PVMA = 2,
        MXVM = 3,
        PVM = 4,
        PVT = 5,
        PVMU = 6,
        PRISME = 7,
        COLLECTOR = 8
    }

    public enum FightState
    {
        STARTING,
        PLAYING,
        WAITING,
        FINISHED
    }

    public enum FighterType
    {
        CHARACTER,
        MONSTER,
        TAX_COLLECTOR
    }

    #endregion

    abstract class Fight
    {
        private int myID;

        private FightType myType;
        private FightState myState;

        private FightTeam myTeam1;
        private FightTeam myTeam2;

        private List<Fighter> myTurnFighters = new List<Fighter>();
        private Fighter myCurrentFighter;

        private Timer myTimer;
        private long myStartTimeOut;
        private long myTurnTimeOut;
        private long myWaitingTimeOut;

        public long EndTime
        {
            get
            {
                return (Environment.TickCount - myStartTimeOut);
            }
        }

        private Map myMap;
        private List<Character> mySpectators = new List<Character>();

        public Fight(FightType type, Map map)
        {
            myID = map.NextFightID();
            myType = type;
            myState = FightState.STARTING;

            myMap = map;
            myTimer = new Timer(new TimerCallback(TimerLoop), null, 1000, 1000);

            Dictionary<int, int[]> places = GeneratePlaces();

            myTeam1 = new FightTeam(0, places[0]);
            myTeam2 = new FightTeam(1, places[1]);
        }

        public abstract int StartTime();
        public abstract int TurnTime();

        public abstract void PlayerLeave(Fighter fighter);
        public abstract void FightEnd(FightTeam winners, FightTeam loosers);

        public int ID
        {
            get { return myID; }
        }

        public FightType Type
        {
            get { return myType; }
        }

        public FightState State
        {
            get { return myState; }
            set { myState = value; }
        }

        public FightTeam Team1
        {
            get { return myTeam1; }
        }

        public FightTeam Team2
        {
            get { return myTeam2; }
        }

        public Fighter CurrentFighter
        {
            get { return myCurrentFighter; }
        }

        public Map Map
        {
            get { return myMap; }
        }

        public FightTeam GetTeam(int leaderID)
        {
            return (myTeam1.Leader.ID == leaderID ? myTeam1 : myTeam2);
        }

        public FightTeam GetEnnemyTeam(FightTeam team)
        {
            return (team == myTeam1 ? myTeam2 : myTeam1);
        }

        public FightTeam GetWinners()
        {
            if (!myTeam1.HasAliveFighters())
            {
                return this.myTeam2;
            }
            else if (!myTeam2.HasAliveFighters())
            {
                return this.myTeam1;
            }

            return null;
        }

        public Fighter GetFighter(int ID)
        {
            return myTeam1.GetFighters().Concat(myTeam2.GetFighters()).First(x => x.ID == ID);
        }

        public Fighter GetAliveFighter(int cell)
        {
            return GetAliveFighters().FirstOrDefault(x => x.Cell == cell);
        }

        public Fighter[] GetFighters()
        {
            return myTeam1.GetFighters().Concat(myTeam2.GetFighters()).ToArray();
        }

        public Fighter[] GetAliveFighters()
        {
            return myTeam1.GetAliveFighters().Concat(myTeam2.GetAliveFighters()).ToArray();
        }

        public bool IsFreeCell(int cell)
        {
            return !GetFighters().Any(x => x.Cell == cell);
        }

        #region Format

        public string FormatFighterShow(Fighter[] fighters)
        {
            StringBuilder builder = new StringBuilder("GM");

            foreach (Fighter fighter in fighters)
                builder.Append("|+").Append(fighter.GetPattern());

            return builder.ToString();

        }

        public string FormatFighterShow(Fighter fighter)
        {
            return new StringBuilder("GM|+").Append(fighter.GetPattern()).ToString();
        }

        public string FormatFighterDestroy(Fighter fighter)
        {
            return new StringBuilder("GM|-").Append(fighter.ID).ToString();
        }

        public string FormatFlagShow()
        {
            StringBuilder builder = new StringBuilder("Gc+");

            builder.Append(myID).Append(';').Append((int)myType).Append('|');
            builder.Append(myTeam1.Leader.ID).Append(';').Append(myTeam1.Cell).Append(';');
            builder.Append('0').Append(';').Append("-1").Append('|');
            builder.Append(myTeam2.Leader.ID).Append(';').Append(myTeam2.Cell).Append(';');
            builder.Append('0').Append(';').Append("-1");

            return builder.ToString();
        }

        public string FormatFlagDestroy()
        {
            return new StringBuilder("Gc-").Append(ID).ToString();
        }

        public string FormatFlagFighter(Fighter[] fighters)
        {
            StringBuilder builder = new StringBuilder("Gt");
            builder.Append(fighters[0].Team.Leader.ID);

            foreach (Fighter fighter in fighters)
                builder.Append("|+").Append(fighter.ID).Append(";").Append(fighter.Name).Append(";").Append(fighter.Level);

            return builder.ToString();
        }

        public string FormatFlagFighterShow(Fighter fighter)
        {
            return new StringBuilder("Gt").Append(fighter.Team.Leader.ID).Append("|+").Append(fighter.ID).Append(";").Append(fighter.Name).Append(";").Append(fighter.Level).ToString();
        }

        public string FormatFlagFighterDestroy(Fighter fighter)
        {
            return new StringBuilder("Gt").Append(fighter.Team.Leader.ID).Append("|-").Append(fighter.ID).Append(";").Append(fighter.Name).Append(";").Append(fighter.Level).ToString();
        }

        public string FormatFlagOption(ToggleType type, FightTeam team)
        {
            return new StringBuilder("Go").Append(team.IsToggle(type) ? '+' : '-').Append(type == ToggleType.LOCK ? 'A' : (char)type).Append(team.ID).ToString();
        }

        public string FormatJoinInformation()
        {
            return new StringBuilder("GJK2|").Append(myType == FightType.CHALLENGE ? 1 : 0).Append("|1|0|").Append(StartTime()).ToString();
        }

        public string FormatJoinPlaces(int team)
        {
            return new StringBuilder("GP").Append(PlacesToString(myTeam1.Places)).Append("|").Append(PlacesToString(myTeam2.Places)).Append("|").Append(team).ToString();
        }

        public string FormatPlace(Fighter fighter)
        {
            return new StringBuilder("GIC|").Append(fighter.ID).Append(";").Append(fighter.Cell).Append(";1").ToString();
        }

        public string FormatFightReady(Fighter fighter)
        {
            return new StringBuilder("GR").Append(fighter.FightReady ? "1" : "0").Append(fighter.ID).ToString();
        }

        public string FormatFightStart()
        {
            return "GS";
        }

        public string FormatTurnList()
        {
            StringBuilder builder = new StringBuilder("GTL");

            foreach (Fighter fighter in myTurnFighters)
                builder.Append("|").Append(fighter.ID);

            return builder.ToString();
        }

        public string FormatTurnStart()
        {
            return new StringBuilder("GTS").Append(myCurrentFighter.ID).Append("|").Append(TurnTime()).ToString();
        }

        public string FormatTurnEnd()
        {
            return new StringBuilder("GTF").Append(myCurrentFighter.ID).ToString();
        }

        public string FormatTurnWait()
        {
            return new StringBuilder("GTR").Append(myCurrentFighter.ID).ToString();
        }

        public string FormatTurnStats()
        {
            StringBuilder builder = new StringBuilder("GTM");

            foreach (Fighter fighter in myTurnFighters)
            {
                builder.Append('|');
                builder.Append(fighter.ID).Append(';');
                builder.Append(fighter.Dead ? '1' : '0').Append(';');
                builder.Append(fighter.Life).Append(';');
                builder.Append(fighter.Stats.GetStat(StatEnum.MaxPA).Total).Append(';');
                builder.Append(fighter.Stats.GetStat(StatEnum.MaxPM).Total).Append(';');
                builder.Append(fighter.Cell).Append(";;");
                builder.Append(fighter.Stats.GetStat(StatEnum.MaxLife).Total);
            }

            return builder.ToString();
        }

        public string FormatLeave()
        {
            return "GV";
        }

        #endregion

        public void Send(string packet, Fighter except = null)
        {
            foreach (Fighter fighter in GetFighters())
                if (fighter != except & fighter.Type == FighterType.CHARACTER && fighter.Character.State.InFight) fighter.Character.NClient.Send(packet);

            foreach (Character spectator in mySpectators)
                spectator.NClient.Send(packet);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void TimerLoop(Object obj = null)
        {
            switch (myState)
            {
                case FightState.STARTING:
                    if (StartTime() > 0 & myStartTimeOut < Environment.TickCount) FightStart(); break;
                case FightState.PLAYING:
                    if (myTurnTimeOut < Environment.TickCount) TurnEnd(); break;
                case FightState.WAITING:
                    if (myWaitingTimeOut < Environment.TickCount) WaitFighters(); break;
            }
        }

        #region Starting

        #region Join

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PlayerJoin(Character player, int team)
        {
            FighterJoin(new CharacterFighter(player, this), team);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool CanJoin(Character player, FightTeam team)
        {
            return myState == FightState.STARTING && team.CanJoin(player);
        }

        public void FighterJoin(Fighter fighter, int team, bool isLeader = false)
        {
            switch (fighter.Type)
            {
                case FighterType.CHARACTER:

                    fighter.Character.GetMap().DelPlayer(fighter.Character);
                    fighter.Character.State.InFight = true;

                    switch (team)
                    {
                        case 0: myTeam1.FighterJoin(fighter, isLeader); break;
                        case 1: myTeam2.FighterJoin(fighter, isLeader); break;
                    }

                    Send(FormatFighterShow(fighter), fighter);

                    if (!isLeader)
                        myMap.Send(FormatFlagFighterShow(fighter));

                    fighter.Character.NClient.Send(FormatJoinInformation());
                    fighter.Character.NClient.Send(FormatJoinPlaces(team));
                    fighter.Character.NClient.Send(FormatFighterShow(GetFighters()));

                    foreach (Fighter forFighter in GetFighters())
                        if (forFighter.FightReady) fighter.Character.NClient.Send(FormatFightReady(forFighter));

                    break;
            }
        }

        #endregion

        #region Toggle

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Toggle(Fighter fighter, ToggleType toggle)
        {
            if (!fighter.IsLeader)
                return;

            bool isToggle = !fighter.Team.IsToggle(toggle);

            fighter.Team.Toggle(toggle, isToggle);

            if (myState == FightState.STARTING)
                myMap.Send(FormatFlagOption(toggle, fighter.Team));

            switch (toggle)
            {
                case ToggleType.LOCK:
                    if (isToggle)
                        fighter.Team.Send("Im095");
                    else
                        fighter.Team.Send("Im096");
                    break;

                case ToggleType.HELP:
                    if (isToggle)
                        fighter.Team.Send("Im0103");
                    else
                        fighter.Team.Send("Im0104");
                    break;

                case ToggleType.PARTY:
                    if (isToggle)
                    {
                        KickNoParty(fighter.Team);
                        fighter.Team.Send("Im093");
                    }
                    else
                    {
                        fighter.Team.Send("Im094");
                    }
                    break;

                case ToggleType.SPECTATOR:
                    if (isToggle)
                    {
                        KickSpectator();
                        fighter.Team.Send("Im040");
                    }
                    else
                    {
                        fighter.Team.Send("Im039");
                    }
                    break;
            }
        }

        private void KickNoParty(FightTeam team)
        {
            if (myState != FightState.STARTING)
                return;

            foreach (Fighter fighter in team.GetFighters())
                if (!team.CanJoin(fighter.Character)) PlayerLeave(fighter);
        }

        public void KickSpectator(bool force = false)
        {
            if (CanJoinSpectator() & !force)
                return;

            foreach (Character spectator in mySpectators)
                SpectatorLeave(spectator);
        }

        #endregion

        #region Places

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PlayerPlace(Fighter fighter, int cell)
        {
            if (myState != FightState.STARTING)
                return;

            if (fighter.FightReady)
                return;

            if (!fighter.Team.IsAvailablePlace(cell))
                return;

            fighter.Cell = cell;

            Send(FormatPlace(fighter));
        }

        public Dictionary<int, int[]> GeneratePlaces()
        {
            Random rand = new Random();
            List<int> cells = myMap.RushablesCells.OrderBy(x => rand.Next()).ToList();

            return new Dictionary<int, int[]> { { 0, cells.GetRange(0, 8).ToArray() }, { 1, cells.GetRange(7, 8).ToArray() } };
        }

        public string PlacesToString(int[] places)
        {
            StringBuilder builder = new StringBuilder();

            foreach (int cell in places)
                builder.Append(Pathfinding.GetCellChars(cell));

            return builder.ToString();
        }

        #endregion

        #region Ready

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PlayerFightReady(Fighter fighter, bool ready)
        {
            if (myState != FightState.STARTING)
                return;

            fighter.FightReady = ready;

            Send(FormatFightReady(fighter));

            if (IsAllFightReady())
                FightStart();
        }

        private bool IsAllFightReady()
        {
            return GetFighters().All(fighter => fighter.FightReady);
        }

        #endregion

        public void FightInit(Fighter attacker, Fighter defender)
        {
            FighterJoin(attacker, 0, true);
            FighterJoin(defender, 1, true);

            myMap.Send(FormatFlagShow());
            myMap.Send(myMap.FormatFightCount());
            myMap.Send(FormatFlagFighterShow(myTeam1.Leader));
            myMap.Send(FormatFlagFighterShow(myTeam2.Leader));

            myStartTimeOut = Environment.TickCount + StartTime();
        }

        private void FightStart()
        {
            List<Fighter> team1 = myTeam1.GetFighters().OrderByDescending(x => x.Stats.GetStat(StatEnum.Initiative).Total).ToList();
            List<Fighter> team2 = myTeam2.GetFighters().OrderByDescending(x => x.Stats.GetStat(StatEnum.Initiative).Total).ToList();
            int maxCount = (team1.Count >= team2.Count ? team1.Count : team2.Count);

            for (int i = 0; i <= maxCount - 1; i++)
            {
                int fighterInitiative = -1;
                int oppositeFighterInitiative = -1;

                if (team1.Count - 1 >= i)
                    fighterInitiative = team1[i].Stats.GetStat(StatEnum.Initiative).Total;

                if (team2.Count - 1 >= i)
                    oppositeFighterInitiative = team2[i].Stats.GetStat(StatEnum.Initiative).Total;

                if (fighterInitiative == -1 & oppositeFighterInitiative == -1)
                    break;

                if (fighterInitiative > oppositeFighterInitiative)
                {
                    myTurnFighters.Add(team1[i]);

                    if (oppositeFighterInitiative != -1)
                        myTurnFighters.Add(team2[i]);
                }
                else
                {
                    myTurnFighters.Add(team2[i]);

                    if (fighterInitiative != -1)
                        myTurnFighters.Add(team1[i]);
                }
            }

            myMap.Send(FormatFlagDestroy());
            myMap.Send(myMap.FormatFightCount());

            Send(FormatFightStart());
            Send(FormatTurnList());

            TurnStart();
        }

        #endregion

        #region Playing

        #region Spectator

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PlayerJoinSpectator(Character player)
        {
            if (myState != FightState.PLAYING)
                return;

            player.GetMap().DelPlayer(player);

            player.State.IsSpectator = true;
            player.Fight = this;

            mySpectators.Add(player);

            player.NClient.Send(FormatJoinInformation());
            player.NClient.Send(FormatFighterShow(GetFighters()));
            player.NClient.Send(FormatFightStart());
            player.NClient.Send(FormatTurnList());
            player.NClient.Send(FormatTurnStart());

            Send("Im036;" + player.Name);
        }

        public bool CanJoinSpectator()
        {
            return myState == Fights.FightState.PLAYING && !myTeam1.IsToggle(ToggleType.SPECTATOR) && !myTeam2.IsToggle(ToggleType.SPECTATOR);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SpectatorLeave(Character player)
        {
            mySpectators.Remove(player);

            player.State.IsSpectator = false;
            player.Fight = null;

            player.NClient.Send(FormatLeave());
        }

        #endregion

        #region Turn

        private void TurnStart()
        {
            NextFighter();

            myState = FightState.PLAYING;
            myTurnTimeOut = Environment.TickCount + TurnTime();

            myCurrentFighter.Buffs.OnTurnBegin();

            Send(FormatTurnStart());
        }

        private void NextFighter()
        {
            do
            {
                if (myCurrentFighter == null || myCurrentFighter == myTurnFighters.LastOrDefault())
                    myCurrentFighter = myTurnFighters[0];
                else
                    myCurrentFighter = myTurnFighters[myTurnFighters.IndexOf(myCurrentFighter) + 1];

            }
            while (myCurrentFighter.Dead);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PlayerTurnPass(Fighter fighter)
        {
            if (myState != FightState.PLAYING)
                return;

            if (myCurrentFighter != fighter)
                return;

            TurnEnd();
        }

        public void TurnEnd()
        {
            SetAllTurnUnready();

            myState = FightState.WAITING;
            myWaitingTimeOut = Environment.TickCount + 5000;

            myCurrentFighter.SpellController.OnTurnEnd();
            myCurrentFighter.Buffs.OnTurnEnd();

            myCurrentFighter.AP = myCurrentFighter.Stats.GetStat(StatEnum.MaxPA).Total;
            myCurrentFighter.MP = myCurrentFighter.Stats.GetStat(StatEnum.MaxPM).Total;

            Send(FormatTurnStats());
            Send(FormatTurnEnd());
            Send(FormatTurnWait());
        }

        private void SetAllTurnUnready()
        {
            foreach (Fighter fighter in myTurnFighters)
                fighter.TurnReady = false;
        }

        #endregion

        #region Waiting

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PlayerTurnReady(Fighter fighter)
        {
            if (myState != FightState.WAITING)
                return;

            fighter.TurnReady = true;

            fighter.Character.NClient.Send("BN");

            if (IsAllTurnReady())
                TurnStart();
        }

        private bool IsAllTurnReady()
        {
            return myTurnFighters.Where(x => !x.Left).All(x => x.TurnReady);
        }

        public void WaitFighters()
        {
            Fighter[] NoReadyFighters = GetFighters().Where(x => !x.TurnReady).ToArray();

            if (NoReadyFighters.Length == 1)
                Send("Im128;" + NoReadyFighters[0].Name);
            else if (NoReadyFighters.Length > 1)
            {
                string names = String.Empty;

                foreach (Fighter fighter in NoReadyFighters)
                    names += fighter.Name + ", ";

                Send("Im129;" + names.Substring(0, names.Length - 2));
            }

            TurnStart();
        }

        #endregion

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool TryMove(Fighter fighter, Pathfinding path)
        {
            if (myState != FightState.PLAYING)
                return false;

            if (myCurrentFighter != fighter)
                return false;

            int length = path.GetLength();

            if (length > fighter.MP)
                return false;

            fighter.MP -= length;
            Send("GA;129;" + fighter.ID + ';' + fighter.ID + ",-" + length);

            fighter.Buffs.OnMoveEnd();

            return true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LaunchSpell(Fighter fighter, CharacterSpell spell, int cellID)
        {
            if (myState != FightState.PLAYING)
                return;

            Fighter firstTarget = GetAliveFighters().FirstOrDefault(x => x.Cell == cellID);
            int targetID = firstTarget == null ? -1 : firstTarget.ID;

            if (!CanLaunchSpell(fighter, spell, cellID, targetID))
                return;

            fighter.AP -= spell.LevelModel.Cost;

            bool isEchec = false;

            if (spell.LevelModel.EC != 0)
            {
                int echecRate = spell.LevelModel.EC - fighter.Stats.GetStat(StatEnum.EchecCritic).Total;

                if (echecRate < 2)
                    echecRate = 2;

                if (Basic.Rand(1, echecRate) == 1)
                    isEchec = true;
            }

            if (isEchec)
            {
                Send("GA;302;" + fighter.ID + ';' + spell.ID);
            }
            else
            {
                fighter.SpellController.OnLaunchSpell(spell, targetID);

                Send("GA;300;" + fighter.ID + ';' + spell.ID + ',' + cellID + ',' + spell.Model.Sprite + ',' + spell.Level + ',' + spell.Model.SpriteInfos);

                bool isCritic = false;

                if (spell.LevelModel.CC != 0 & spell.LevelModel.CriticalEffects.Count > 0)
                {
                    int criticRate = spell.LevelModel.CC - fighter.Stats.GetStat(StatEnum.DamageCritic).Total;

                    if (criticRate < 2)
                        criticRate = 2;

                    if (Basic.Rand(1, criticRate) == 1)
                        isCritic = true;
                }

                if (isCritic)
                    Send("GA;301;" + fighter.ID + ';' + spell.ID);

                List<EffectSpell> effects = isCritic ? spell.LevelModel.CriticalEffects : spell.LevelModel.Effects;
                List<Fighter> targets = new List<Fighter>();

                foreach (int cell in CellZone.GetCells(myMap, cellID, fighter.Cell, spell.LevelModel.Type))
                {
                    Fighter target = GetAliveFighters().FirstOrDefault(x => x.Cell == cell);

                    if (target != null)
                        targets.Add(target);
                }

                int actualChance = 0;

                foreach (EffectSpell effect in effects)
                {
                    if (effect.Chance > 0)
                    {
                        if (Basic.Rand(1, 100) > effect.Chance + actualChance)
                        {
                            actualChance += effect.Chance;
                            continue;
                        }

                        actualChance -= 100;
                    }

                    targets.RemoveAll(x => x.Dead);

                    EffectProcessor.ApplyEffect(new EffectCast
                        ((EffectEnum)effect.ID, spell.ID, cellID, effect.Value, effect.Value2, effect.Value3, effect.Chance, effect.Round, (spell.LevelModel.MinRP == 1 & spell.LevelModel.MaxRP == 1), fighter, effect.Target.RemixTargets(fighter, targets)));
                }
            }

            Send("GA;102;" + fighter.ID + ';' + fighter.ID + ",-" + spell.LevelModel.Cost);

            if (GetWinners() != null)
                FightEnd(GetWinners(), GetEnnemyTeam(GetWinners()));
            else if (isEchec & spell.LevelModel.isECEndTurn)
                TurnEnd();
        }

        public bool CanLaunchSpell(Fighter fighter, CharacterSpell spell, int spellCell, long target)
        {
            if (fighter != myCurrentFighter)
                return false;

            //if (!myMap.RushablesCells.Contains(spellCell))
            //    return false;

            if (fighter.AP < spell.LevelModel.Cost)
                return false;

            int distance = Pathfinding.GetDistanceBetween(myMap, fighter.Cell, spellCell);
            int maxPO = spell.LevelModel.MaxRP + (spell.LevelModel.isAlterablePO ? fighter.Stats.GetStat(StatEnum.PO).Total : 0);

            if (maxPO < spell.LevelModel.MinRP)
                maxPO = spell.LevelModel.MinRP;

            if (distance > maxPO || distance < spell.LevelModel.MinRP)
                return false;

            return fighter.SpellController.CanLaunchSpell(spell, target);
        }

        #endregion
    }
}
