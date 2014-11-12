using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus;
using SunDofus.World.Maps.Fights;
using SunDofus.World.Characters.Stats;
using SunDofus.World.Characters.Items;
using SunDofus.World.Characters.Spells;
using SunDofus.Network.Realm;
using SunDofus.Network.Game;

namespace SunDofus.World.Characters
{
    class Character
    {
        public string Name { get; set; }

        public int ID { get; set; }
        public int Color { get; set; }
        public int Color2 { get; set; }
        public int Color3 { get; set; }
        public int Class { get; set; }
        public int Sex { get; set; }
        public int Skin { get; set; }
        public int Size { get; set; }
        public int Level { get; set; }
        public int MapID { get; set; }
        public int MapCell { get; set; }
        public int MapDir { get; set; }
        public int CharactPoint { get; set; }
        public int SpellPoint { get; set; }
        public int Energy { get; set; }
        public int Life { get; set; }
        public int Pods { get; set; }
        public int SaveMap { get; set; }
        public int SaveCell { get; set; }

        public long Exp { get; set; }
        public long Kamas { get; set; }

        public bool IsNewCharacter { get; set; }
        public bool IsDeletedCharacter { get; set; }
        public bool IsConnected { get; set; }

        private long quotaRecruitment;
        private long quotaTrade;
        private long quotaSmiley;

        public Fight Fight { get; set; }
        public Fighter Fighter { get; set; }

        public List<int> Zaaps { get; set; }

        public GenericStats Stats { get; set; }
        public Guilds.Guild Guild { get; set; }
        public InventaryItems ItemsInventary { get; set; }
        public InventarySpells SpellsInventary { get; set; }
        public GameClient NClient { get; set; }

        public CharacterState State { get; set; }
        public CharacterFaction Faction { get; set; }
        public CharacterChannels Channels { get; set; }
        public CharacterJobs Jobs { get; set; }

        public CharacterFriends Friends { get; set; }
        public CharacterEnemies Enemies { get; set; }

        public Character()
        {
            Zaaps = new List<int>();

            Stats = new GenericStats(this);
            ItemsInventary = new InventaryItems(this);
            SpellsInventary = new InventarySpells(this);
            Faction = new CharacterFaction(this);
            Jobs = new CharacterJobs(this);

            Channels = new CharacterChannels(this);
            Friends = new CharacterFriends(this);
            Enemies = new CharacterEnemies(this);

            Energy = 10000;
            IsConnected = false;
            IsDeletedCharacter = false;

            quotaRecruitment = 0;
            quotaTrade = 0;
        }

        #region Exp

        public void AddExp(long exp)
        {
            Exp += exp;
            LevelUp();
        }

        private void LevelUp()
        {
            if (this.Level == Entities.Requests.LevelsRequests.MaxLevel())
                return;

            if (Exp >= Entities.Requests.LevelsRequests.ReturnLevel(Level + 1).Character)
            {
                while (Exp >= Entities.Requests.LevelsRequests.ReturnLevel(Level + 1).Character)
                {
                    if (this.Level == Entities.Requests.LevelsRequests.MaxLevel())
                        break;

                    Level++;
                    SpellPoint++;
                    CharactPoint += 5;
                }

                if(IsConnected)
                    NClient.Send(string.Concat("AN", Level));

                SpellsInventary.LearnSpells();
                SendChararacterStats();
            }
        }

        #endregion

        #region ChatSpam

        public long TimeTrade()
        {
            return (long)Math.Ceiling((double)((quotaTrade - Environment.TickCount) / 1000));
        }

        public long TimeRecruitment()
        {
            return (long)Math.Ceiling((double)((quotaRecruitment - Environment.TickCount) / 1000));
        }

        public long TimeSmiley()
        {
            return (long)Math.Ceiling((double)((quotaSmiley - Environment.TickCount) / 1000));
        }

        public bool CanSendinTrade()
        {
            return (TimeTrade() <= 0 ? true : false);
        }

        public bool CanSendinRecruitment()
        {
            return (TimeRecruitment() <= 0 ? true : false);
        }

        public bool CanSendinSmiley()
        {
            return TimeSmiley() <= 0;
        }

        public void RefreshTrade()
        {
            quotaTrade = Environment.TickCount + 60000;
        }

        public void RefreshRecruitment()
        {
            quotaRecruitment = Environment.TickCount + 60000;
        }

        public void RefreshSmiley()
        {
            quotaSmiley = Environment.TickCount + 1000;
        }

        #endregion

        #region Items

        public string GetItemsPos()
        {
            var packet = "";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 1))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 1).Model.ID);

            packet += ",";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 6))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 6).Model.ID);

            packet += ",";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 7))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 7).Model.ID);

            packet += ",";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 8))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 8).Model.ID);

            packet += ",";

            if (ItemsInventary.ItemsList.Any(x => x.Position == 15))
                packet += Utilities.Basic.DeciToHex(ItemsInventary.ItemsList.First(x => x.Position == 15).Model.ID);

            return packet;
        }

        public string GetItems()
        {
            return string.Join(";", ItemsInventary.ItemsList);
        }

        public string GetItemsToSave()
        {
            return (string.Join(";", from x in ItemsInventary.ItemsList select x.SaveString()));
        }

#endregion

        #region Pattern

        public string PatternList()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Skin).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color3)).Append(";");
                builder.Append(GetItemsPos()).Append(";");
                builder.Append("0;").Append(Program.Config.GameID).Append(";;;");
            }

            return builder.ToString();
        }

        public string PatternOnParty()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Skin).Append(";");
                builder.Append(Color).Append(";");
                builder.Append(Color2).Append(";");
                builder.Append(Color3).Append(";");
                builder.Append(GetItemsPos()).Append(";");
                builder.Append(Life).Append(",").Append(Stats.GetStat(StatEnum.MaxLife).Total).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Stats.GetStat(StatEnum.Initiative).Total).Append(";");
                builder.Append(Stats.GetStat(StatEnum.Prospection).Total).Append(";0");
            }

            return builder.ToString();
        }

        public string PatternSelect()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append("|").Append(ID).Append("|");
                builder.Append(Name).Append("|");
                builder.Append(Level).Append("|");
                builder.Append(Class).Append("|");
                builder.Append(Skin).Append("|");
                builder.Append(Utilities.Basic.DeciToHex(Color)).Append("|");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append("|");
                builder.Append(Utilities.Basic.DeciToHex(Color3)).Append("||");
                builder.Append(GetItems()).Append("|");
            }

            return builder.ToString();
        }

        public string PatternGuild()
        {
            var member = Guild.Members.First(x => x.Character == this);

            StringBuilder builder = new StringBuilder();
            {
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Skin).Append(";");
                builder.Append(member.Rank).Append(";");
                builder.Append(member.ExpGaved).Append(";");
                builder.Append(member.ExpGived).Append(";");
                builder.Append(member.Rights).Append(";");
                builder.Append((IsConnected ? "1" : "0")).Append(";");
                builder.Append(Faction.ID).Append(";0");
            }

            return builder.ToString();
        }

        public string PatternDisplayChar()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(MapCell).Append(";");
                builder.Append(MapDir).Append(";0;");
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Class).Append(";");
                builder.Append(Skin).Append("^").Append(Size).Append(";");
                builder.Append(Sex).Append(";").Append(Faction.AlignementInfos).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color3)).Append(";");
                builder.Append(GetItemsPos()).Append(";"); // Items
                builder.Append("0;"); //Aura
                builder.Append(";;");

                if (Guild != null && Guild.Members.Count >= 2)
                    builder.Append(Guild.Name).Append(";").Append(Guild.Emblem);
                else
                    builder.Append(";");

                builder.Append(";0;");
                builder.Append(";"); // Mount
            }

            return builder.ToString();
        }

        public string PatternFightDisplayChar()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(Fighter.Cell).Append(";");
                builder.Append("1").Append(";0;");
                builder.Append(ID).Append(";");
                builder.Append(Name).Append(";");
                builder.Append(Class).Append(";");
                builder.Append(Skin).Append("^").Append(Size).Append(";");
                builder.Append(Sex).Append(";");
                builder.Append(Level).Append(";");
                builder.Append(Faction.AlignementInfos).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color2)).Append(";");
                builder.Append(Utilities.Basic.DeciToHex(Color3)).Append(";");
                builder.Append(GetItemsPos()).Append(";");
                builder.Append(Life).Append(";");
                builder.Append(Stats.GetStat(StatEnum.MaxPA).Total).Append(";").Append(Stats.GetStat(StatEnum.MaxPM).Total).Append(";");
                builder.Append("0;0;0;0;0;");
                builder.Append("0;0;");
                builder.Append(Fighter.Team.ID).Append(";;");
            }

            return builder.ToString();
        }

        #endregion

        #region Map

        public void LoadMap()
        {
            if (Entities.Requests.MapsRequests.MapsList.Any(x => x.Model.ID == this.MapID))
            {
                var map = Entities.Requests.MapsRequests.MapsList.First(x => x.Model.ID == this.MapID);

                NClient.Send(string.Format("GDM|{0}|{1}|{2}", map.Model.ID, map.Model.Date, map.Model.Key));

                if (this.State.IsFollow)
                {
                    foreach (var character in this.State.Followers)
                        character.NClient.Send(string.Format("IC{0}|{1}", GetMap().Model.PosX, GetMap().Model.PosY));
                }
            }
        }

        public bool IsInIncarnam
        {
            get
            {
                var map = GetMap();

                return map.Model.SubArea == 440 || map.Model.SubArea == 442 || map.Model.SubArea == 443 ||
                    map.Model.SubArea == 444 || map.Model.SubArea == 445 || map.Model.SubArea == 446 ||
                    map.Model.SubArea == 449 || map.Model.SubArea == 450;
            }
        }

        public void TeleportNewMap(int _mapID, int _cell)
        {
            NClient.Send(string.Format("GA;2;{0};", ID));

            GetMap().DelPlayer(this);
            var map = Entities.Requests.MapsRequests.MapsList.First(x => x.Model.ID == _mapID);

            MapID = map.Model.ID;
            MapCell = _cell;

            LoadMap();
        }

        public void Sit()
        {
            State.IsSitted = !State.IsSitted;

            if (State.IsSitted)
            {
                State.SitStartTime = Environment.TickCount;

                NClient.Send("ILS1000");
            }
            else
            {
                int regenerated = (int)(Environment.TickCount - State.SitStartTime) / 1000;
                int missing = Stats.GetStat(StatEnum.MaxLife).Total - Life;

                if (regenerated > missing)
                    regenerated = missing;

                NClient.Player.Life += regenerated;

                NClient.Send("ILF" + regenerated);
                NClient.Send("ILS360000");
            }
        }

        public Maps.Map GetMap()
        {
            return Entities.Requests.MapsRequests.MapsList.First(x => x.Model.ID == this.MapID);
        }

        #endregion

        #region Stats

        public void SendChararacterStats()
        {
            NClient.Send(string.Concat("As", this.ToString()));
        }

        public void SendPods()
        {
            NClient.Send(string.Format("Ow{0}|{1}", Pods, Stats.GetStat(StatEnum.MaxPods).Total));
        }

        public void AddLife(int _life)
        {
            if (Life == Stats.GetStat(StatEnum.MaxLife).Total)
                NClient.SendMessage("Im119");

            else if ((Life + _life) > Stats.GetStat(StatEnum.MaxLife).Total)
            {
                NClient.SendMessage(string.Concat("Im01;", (Stats.GetStat(StatEnum.MaxLife).Total - Life)));
                Life = Stats.GetStat(StatEnum.MaxLife).Total;
            }
            else
            {
                NClient.SendMessage(string.Concat("Im01;", _life));
                Life += _life;
            }
        }

        public void ResetVita(string datas)
        {
            if (datas == "full")
            {
                Life = Stats.GetStat(StatEnum.MaxLife).Total;
                SendChararacterStats();
            }
            else
            {
                Life = (Stats.GetStat(StatEnum.MaxLife).Total / (int.Parse(datas) / 100));
                SendChararacterStats();
            }
        }

        public string SqlStats()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(Life).Append('|');
                builder.Append(Energy).Append('|');
                builder.Append(Stats.GetStat(StatEnum.Vitalite).Base).Append('|');
                builder.Append(Stats.GetStat(StatEnum.Sagesse).Base).Append('|');
                builder.Append(Stats.GetStat(StatEnum.Force).Base).Append('|');
                builder.Append(Stats.GetStat(StatEnum.Intelligence).Base).Append('|');
                builder.Append(Stats.GetStat(StatEnum.Chance).Base).Append('|');
                builder.Append(Stats.GetStat(StatEnum.Agilite).Base).Append('|');
                builder.Append(CharactPoint).Append('|');
                builder.Append(SpellPoint).Append('|');
                builder.Append(Kamas);
            }

            return builder.ToString();
        }

        public void ParseStats(string args)
        {
            if (args == "")
                return;

            var Data = args.Split('|');

            Life = int.Parse(Data[0]);
            Energy = int.Parse(Data[1]);
            Stats.GetStat(StatEnum.Vitalite).Base = int.Parse(Data[2]);
            Stats.GetStat(StatEnum.Sagesse).Base = int.Parse(Data[3]);
            Stats.GetStat(StatEnum.Force).Base = int.Parse(Data[4]);
            Stats.GetStat(StatEnum.Intelligence).Base = int.Parse(Data[5]);
            Stats.GetStat(StatEnum.Chance).Base = int.Parse(Data[6]);
            Stats.GetStat(StatEnum.Agilite).Base = int.Parse(Data[7]);
            CharactPoint = int.Parse(Data[8]);
            SpellPoint = int.Parse(Data[9]);
            Kamas = long.Parse(Data[10]);
        }

        #endregion

        #region Params

        public string GetParam(string paramName)
        {
            switch (paramName)
            {
                case "kamas":
                    return Kamas.ToString();

                default:
                    return "";
            }
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            {
                builder.Append(Exp).Append(",");
                builder.Append(Entities.Requests.LevelsRequests.ReturnLevel(Level).Character).Append(",");
                builder.Append(Entities.Requests.LevelsRequests.ReturnLevel(Level + 1).Character).Append("|");
                builder.Append(Kamas).Append("|");
                builder.Append(CharactPoint).Append("|");
                builder.Append(SpellPoint).Append("|");
                builder.Append(Faction.ToString()).Append("|");
                builder.Append(Life).Append(",");
                builder.Append(Stats.GetStat(StatEnum.MaxLife).Total).Append("|");
                builder.Append(Energy).Append(",10000|");
                builder.Append(Stats.GetStat(StatEnum.Initiative).Total).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Prospection).Total).Append("|");

                builder.Append(Stats.GetStat(StatEnum.MaxPA).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.MaxPM).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Force).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Vitalite).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Sagesse).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Chance).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Agilite).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Intelligence).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.PO).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.InvocationMax).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Damage).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.DamagePhysic).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.DamageMagic).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.DamagePercent).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.Soins).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.DamagePiege).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.DamagePiegePercent).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReflectDamage).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.DamageCritic).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.EchecCritic).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.EsquivePA).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.EsquivePM).ToString()).Append("|");

                builder.Append(Stats.GetStat(StatEnum.ReduceDamageNeutre).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentNeutre).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePvPNeutre).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentPvPNeutre).ToString()).Append("|");

                builder.Append(Stats.GetStat(StatEnum.ReduceDamageTerre).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentTerre).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePvPTerre).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentPvPTerre).ToString()).Append("|");

                builder.Append(Stats.GetStat(StatEnum.ReduceDamageEau).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentEau).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePvPEau).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentPvPEau).ToString()).Append("|");

                builder.Append(Stats.GetStat(StatEnum.ReduceDamageAir).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentAir).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePvPAir).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentPvPAir).ToString()).Append("|");

                builder.Append(Stats.GetStat(StatEnum.ReduceDamageFeu).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentFeu).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePvPFeu).ToString()).Append("|");
                builder.Append(Stats.GetStat(StatEnum.ReduceDamagePercentPvPFeu).ToString()).Append("|");

                builder.Append("1");
            }

            return builder.ToString();
        }

        #endregion
    }
}
