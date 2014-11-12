using SunDofus.Entities.Requests;
using SunDofus.World;
using SunDofus.World.Bank;
using SunDofus.World.Characters;
using SunDofus.World.Characters.Spells;
using SunDofus.World.Characters.Stats;
using SunDofus.World.Conditions;
using SunDofus.World.Guilds;
using SunDofus.World.Maps;
using SunDofus.World.Maps.Fights;
using SunDofus.World.Maps.Zaapis;
using SunDofus.World.Maps.Zaaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Network.Game
{
    class GameParser
    {
        public GameClient Client { get; set; }

        private delegate void Packets(string datas);
        private Dictionary<string, Packets> RegisteredPackets;

        public GameParser(GameClient client)
        {
            Client = client;

            RegisteredPackets = new Dictionary<string, Packets>();
            RegisterPackets();
        }

        #region Packets

        private void RegisterPackets()
        {
            RegisteredPackets["AA"] = CreateCharacter;
            RegisteredPackets["AB"] = StatsBoosts;
            RegisteredPackets["AD"] = DeleteCharacter;
            //RegisteredPackets["Af"] = RefreshQueue;
            RegisteredPackets["Ag"] = SendGifts;
            RegisteredPackets["AG"] = AcceptGift;
            RegisteredPackets["AL"] = SendCharacterList;
            RegisteredPackets["AP"] = SendRandomName;
            RegisteredPackets["AS"] = SelectCharacter;
            RegisteredPackets["AT"] = ParseTicket;
            RegisteredPackets["AV"] = SendCommunauty;
            RegisteredPackets["BA"] = ParseConsoleMessage;
            RegisteredPackets["Ba"] = TeleportByPos;
            RegisteredPackets["BD"] = SendDate;
            RegisteredPackets["BM"] = ParseChatMessage;
            RegisteredPackets["BS"] = UseSmiley;
            RegisteredPackets["cC"] = ChangeChannel;
            RegisteredPackets["DC"] = DialogCreate;
            RegisteredPackets["DR"] = DialogReply;
            RegisteredPackets["DV"] = DialogExit;
            RegisteredPackets["eD"] = ChangeDirection;
            RegisteredPackets["eU"] = UseEmote;
            RegisteredPackets["EA"] = ExchangeAccept;
            RegisteredPackets["EB"] = ExchangeBuy;
            RegisteredPackets["EK"] = ExchangeValidate;
            RegisteredPackets["EM"] = ExchangeMove;
            RegisteredPackets["ER"] = ExchangeRequest;
            RegisteredPackets["ES"] = ExchangeSell;
            RegisteredPackets["EV"] = CancelExchange;
            RegisteredPackets["FA"] = FriendAdd;
            RegisteredPackets["FD"] = FriendDelete;
            RegisteredPackets["FL"] = FriendsList;
            RegisteredPackets["FO"] = FriendsFollow;
            RegisteredPackets["fD"] = FightDetails;
            RegisteredPackets["fL"] = FightList;
            RegisteredPackets["fN"] = ToggleFightLock;
            RegisteredPackets["fH"] = ToggleFightHelp;
            RegisteredPackets["fP"] = ToggleFightParty;
            RegisteredPackets["fS"] = ToggleFightSpectator;
            RegisteredPackets["GA"] = GameAction;
            RegisteredPackets["GC"] = CreateGame;
            RegisteredPackets["GI"] = GameInformations;
            RegisteredPackets["GK"] = EndAction;
            RegisteredPackets["GP"] = ChangeAlignmentEnable;
            RegisteredPackets["GR"] = FightReady;
            RegisteredPackets["GT"] = FightTurnReady;
            RegisteredPackets["Gt"] = FightTurnPass;
            RegisteredPackets["GQ"] = FightLeave;
            RegisteredPackets["Gp"] = FightPlacement;
            RegisteredPackets["gB"] = UpgradeStatsGuild;
            RegisteredPackets["gb"] = UpgradeSpellsGuild;
            RegisteredPackets["gC"] = CreateGuild;
            RegisteredPackets["gH"] = LetCollectorGuild;
            RegisteredPackets["gI"] = GetGuildInfos;
            RegisteredPackets["gJ"] = GetGuildJoinRequest;
            RegisteredPackets["gK"] = ExitGuild;
            RegisteredPackets["gP"] = ModifyRightGuild;
            RegisteredPackets["gV"] = CloseGuildPanel;
            RegisteredPackets["iA"] = EnemyAdd;
            RegisteredPackets["iD"] = EnemyDelete;
            RegisteredPackets["iL"] = EnemiesList;
            RegisteredPackets["Od"] = DeleteItem;
            RegisteredPackets["OM"] = MoveItem;
            RegisteredPackets["OU"] = UseItem;
            RegisteredPackets["PA"] = PartyAccept;
            RegisteredPackets["PG"] = PartyGroupFollow;
            RegisteredPackets["PF"] = PartyFollow;
            RegisteredPackets["PI"] = PartyInvite;
            RegisteredPackets["PR"] = PartyRefuse;
            RegisteredPackets["PV"] = PartyLeave;
            RegisteredPackets["SB"] = SpellBoost;
            RegisteredPackets["SM"] = SpellMove;
            RegisteredPackets["WU"] = UseZaaps;
            RegisteredPackets["Wu"] = UseZaapis;
            RegisteredPackets["WV"] = ExitZaap;
            RegisteredPackets["Wv"] = ExitZaapis;
        }

        public void Parse(string datas)
        {
            if (datas == "ping")
                Client.Send("pong");
            else if (datas == "qping")
                Client.Send("qpong");

            if (datas.Length < 2) 
                return;

            string header = datas.Substring(0, 2);

            if (!RegisteredPackets.ContainsKey(header))
            {
                Client.Send("BN");
                return;
            }

            RegisteredPackets[header](datas.Substring(2));
        }

        #endregion

        #region Ticket & Queue

        private void ParseTicket(string datas)
        {
            lock (Program.GameServer.Tickets)
            {
                if (Program.GameServer.Tickets.Any(x => x.Key == datas))
                {
                    var key = Program.GameServer.Tickets.First(x => x.Key == datas);

                    if (Program.GameServer.Clients.Any(x => x.Authentified == true && x.Infos.Pseudo == key.Model.Pseudo))
                        Program.GameServer.Clients.First(x => x.Authentified == true && x.Infos.Pseudo == key.Model.Pseudo).Disconnect();

                    Client.Infos = key.Model;
                    Client.Infos.ParseCharacters();
                    Client.ParseCharacters();

                    Client.Authentified = true;

                    foreach (var friend in Client.Infos.StrFriends.Split('+'))
                    {
                        if (!Client.Friends.Contains(friend))
                            Client.Friends.Add(friend);
                    }

                    foreach (var enemy in Client.Infos.StrEnemies.Split('+'))
                    {
                        if (!Client.Enemies.Contains(enemy))
                            Client.Enemies.Add(enemy);
                    }

                    Program.GameServer.Tickets.Remove(key);

                    AccountsRequests.UpdateConnectedValue(key.Model.ID, true);

                    Client.Send("ATK0");

                    SendCharacterList("");
                }
                else
                    Client.Send("ATE");
            }
        }

        #endregion
        
        #region Character

        private void SendRandomName(string datas)
        {
            Client.Send(string.Concat("APK", Utilities.Basic.RandomName()));
        }

        private void SendCommunauty(string datas)
        {
            Client.Send("AV0");
        }

        public void SendCharacterList(string datas)
        {
            if (!Client.IsInQueue)
            {
                string packet = string.Format("ALK{0}|{1}", Client.Infos.Subscription, Client.Characters.Count);

                if (Client.Characters.Count != 0)
                {
                    foreach (Character character in Client.Characters)
                        packet += string.Concat("|", character.PatternList());
                }

                Client.Send(packet);
            }
        }

        private void CreateCharacter(string datas)
        {
            try
            {
                var characterDatas = datas.Split('|');

                if (characterDatas[0] != "" | Entities.Requests.CharactersRequests.ExistsName(characterDatas[0]) == false)
                {
                    var character = new Character();

                    if (Entities.Requests.CharactersRequests.CharactersList.Count > 0)
                        character.ID = (Entities.Requests.CharactersRequests.CharactersList.OrderByDescending(x => x.ID).ToArray()[0].ID) + 1;
                    else
                        character.ID = 1;

                    character.Name = characterDatas[0];
                    character.Level = 1;
                    character.Class = int.Parse(characterDatas[1]);
                    character.Sex = int.Parse(characterDatas[2]);
                    character.Skin = int.Parse(character.Class + "" + character.Sex);
                    character.Size = 100;
                    character.Color = int.Parse(characterDatas[3]);
                    character.Color2 = int.Parse(characterDatas[4]);
                    character.Color3 = int.Parse(characterDatas[5]);

                    #region MapInfos

                    switch (character.Class)
                    {
                        case 1:
                            character.MapID = 10300;
                            character.MapCell = 337;
                            character.MapDir = 3;
                            break;
                        case 2:
                            character.MapID = 10258;
                            character.MapCell = 210;
                            character.MapDir = 3;
                            break;
                        case 3:
                            character.MapID = 10299;
                            character.MapCell = 300;
                            character.MapDir = 3;
                            break;
                        case 4:
                            character.MapID = 10285;
                            character.MapCell = 263;
                            character.MapDir = 3;
                            break;
                        case 5:
                            character.MapID = 10298;
                            character.MapCell = 301;
                            character.MapDir = 3;
                            break;
                        case 6:
                            character.MapID = 10276;
                            character.MapCell = 296;
                            character.MapDir = 3;
                            break;
                        case 7:
                            character.MapID = 10283;
                            character.MapCell = 299;
                            character.MapDir = 3;
                            break;
                        case 8:
                            character.MapID = 10294;
                            character.MapCell = 263;
                            character.MapDir = 3;
                            break;
                        case 9:
                            character.MapID = 10292;
                            character.MapCell = 299;
                            character.MapDir = 3;
                            break;
                        case 10:
                            character.MapID = 10279;
                            character.MapCell = 269;
                            character.MapDir = 3;
                            break;
                        case 11:
                            character.MapID = 10296;
                            character.MapCell = 244;
                            character.MapDir = 3;
                            break;
                        case 12:
                            character.MapID = 10289;
                            character.MapCell = 264;
                            character.MapDir = 3;
                            break;
                    }

                    #endregion

                    character.CharactPoint = (character.Level - 1) * 5;
                    character.SpellPoint = (character.Level - 1);
                    character.Exp = Entities.Requests.LevelsRequests.ReturnLevel(character.Level).Character;
                    character.Kamas = 0;

                    if (character.Class < 1 | character.Class > 12 | character.Sex < 0 | character.Sex > 1)
                    {
                        Client.Send("AAE");
                        return;
                    }

                    character.SpellsInventary.LearnSpells();
                    character.IsNewCharacter = true;

                    lock (Entities.Requests.CharactersRequests.CharactersList)
                        Entities.Requests.CharactersRequests.CharactersList.Add(character);

                    lock(Client.Characters)
                        Client.Characters.Add(character);

                    AccountsRequests.UpdateCharacters(Client.Infos.ID, character.Name, Program.Config.GameID, true);

                    Client.Send("AAK");
                    Client.Send("TB");
                    SendCharacterList("");
                }
                else
                {
                    Client.Send("AAE");
                }
            }
            catch (Exception e)
            {
                Utilities.Loggers.Errors.Write(e.ToString());
            }
        }

        private void DeleteCharacter(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas.Split('|')[0], out id))
                return;

            lock (Entities.Requests.CharactersRequests.CharactersList)
            {
                if (!Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == id))
                    return;

                var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == id);

                if (datas.Split('|')[1] != Client.Infos.Answer && character.Level >= 20)
                {
                    Client.Send("ADE");
                    return;
                }

                lock(Client.Characters)
                    Client.Characters.Remove(character);

                AccountsRequests.UpdateCharacters(Client.Infos.ID, character.Name, Program.Config.GameID, false); 
                character.IsDeletedCharacter = true;

                SendCharacterList("");
            }
        }

        private void SelectCharacter(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            lock (Entities.Requests.CharactersRequests.CharactersList)
            {
                if (!Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == id))
                    return;

                var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == id);

                lock (Client.Characters)
                {
                    if (Client.Characters.Contains(character))
                    {
                        Client.Player = character;
                        Client.Player.State = new CharacterState(Client.Player);
                        Client.Player.NClient = Client;

                        Client.Player.IsConnected = true;

                        foreach (var client in Program.GameServer.Clients.Where(x => x.Characters.Any(c => c.IsConnected == true) && x.Friends.Contains(Client.Infos.Pseudo) && x.Player.Friends.WillNotifyWhenConnected))
                            client.Send(string.Concat("Im0143;", Client.Player.Name));

                        Client.Send(string.Concat("ASK", Client.Player.PatternSelect()));
                    }
                    else
                        Client.Send("ASE");
                }
            }
        }

        #endregion

        #region Gift

        private void SendGifts(string datas)
        {
            Client.SendGifts();
        }

        private void AcceptGift(string datas)
        {
            var infos = datas.Split('|');

            var idGift = 0;
            var idChar = 0;

            if (!int.TryParse(infos[0], out idGift) || !int.TryParse(infos[1], out idChar))
                return;

            if (Client.Characters.Any(x => x.ID == idChar))
            {
                lock (Client.Infos.Gifts)
                {
                    if (Client.Infos.Gifts.Any(x => x.ID == idGift))
                    {
                        var myGift = Client.Infos.Gifts.First(e => e.ID == idGift);
                        Client.Characters.First(x => x.ID == idChar).ItemsInventary.AddItem(myGift.Item, true);

                        Client.Send("AG0");

                        GiftsRequests.DeleteGift(myGift.ID, Client.Infos.ID);
                        
                        lock(Client.Infos.Gifts)
                            Client.Infos.Gifts.Remove(myGift);

                    }
                    else
                        Client.Send("AGE");
                }
            }
            else
                Client.Send("AGE");
        }

        #endregion

        #region World

        #region Date

        private void SendDate(string datas)
        {
            Client.Send(string.Concat("BD", Utilities.Basic.GetDofusDate()));
        }

        #endregion

        #region Channels

        private void ChangeChannel(string channel)
        {
            char head;

            if (!char.TryParse(channel.Substring(1), out head))
                return;
            
            var state = (channel.Substring(0, 1) == "+" ? true : false);
            Client.Player.Channels.ChangeChannelState(head, state);
        }

        #endregion

        #region Faction

        private void ChangeAlignmentEnable(string enable)
        {
            if (enable == "+")
            {
                if (Client.Player.Faction.ID != 0)
                    Client.Player.Faction.IsEnabled = true;
            }
            else if (enable == "*")
            {
                var hloose = Client.Player.Faction.Honor / 100;
                Client.Send(string.Concat("GIP", hloose.ToString()));

                return;
            }
            else if (enable == "-")
            {
                var hloose = Client.Player.Faction.Honor / 100;

                if (Client.Player.Faction.ID != 0)
                {
                    Client.Player.Faction.IsEnabled = false;
                    Client.Player.Faction.Honor -= hloose;
                }
            }

            Client.Player.SendChararacterStats();
        }

        #endregion

        #region Friends - Enemies

        private void FriendsList(string datas)
        {
            Client.Player.Friends.SendFriends();
        }

        private void FriendAdd(string datas)
        {
            Client.Player.Friends.AddFriend(datas);
        }

        private void FriendDelete(string datas)
        {
            Client.Player.Friends.RemoveFriend(datas);
        }

        private void FriendsFollow(string datas)
        {
            if (datas.Substring(0, 1) == "+")
                Client.Player.Friends.WillNotifyWhenConnected = true;
            else
                Client.Player.Friends.WillNotifyWhenConnected = false;

            Client.Send(string.Concat("FO", (Client.Player.Friends.WillNotifyWhenConnected ? "+" : "-")));
        }

        private void EnemiesList(string datas)
        {
            Client.Player.Enemies.SendEnemies();
        }

        private void EnemyAdd(string datas)
        {
            Client.Player.Enemies.AddEnemy(datas);
        }

        private void EnemyDelete(string datas)
        {
            Client.Player.Enemies.RemoveEnemy(datas);
        }

        #endregion

        #region Zaaps - Zaapis

        private void ExitZaap(string datas)
        {
            Client.Send("WV");
        }

        private void ExitZaapis(string datas)
        {
            Client.Send("Wv");
        }

        private void UseZaapis(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            ZaapisManager.OnMove(Client.Player, id);
        }

        private void UseZaaps(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            ZaapsManager.OnMove(Client.Player, id);
        }

        #endregion

        #region Chat

        private void ParseChatMessage(string datas)
        {
            var infos = datas.Split('|');

            var channel = infos[0];
            var message = infos[1];

            switch (channel)
            {
                case "*":
                    Chat.SendGeneralMessage(Client, message);
                    return;

                case "^":
                    Chat.SendIncarnamMessage(Client, message);
                    return;

                case "$":
                    Chat.SendPartyMessage(Client, message);
                    return;

                case "%":
                    Chat.SendGuildMessage(Client, message);
                    return;

                case "#":
                    //TeamMessage
                    return;

                case "?":
                    Chat.SendRecruitmentMessage(Client, message);
                    return;

                case "!":
                    Chat.SendFactionMessage(Client, message);
                    return;

                case ":":
                    Chat.SendTradeMessage(Client, message);
                    return;

                case "@":
                    Chat.SendAdminMessage(Client, message);
                    return;

                case "¤":
                    //No idea
                    return;

                default:
                    if (channel.Length > 1)
                        Chat.SendPrivateMessage(Client, channel, message);
                    return;
            }
        }

        private void ParseConsoleMessage(string datas)
        {
            Client.Commander.ParseAdminCommand(datas);
        }

        #endregion

        #region Guilds

        private void CloseGuildPanel(string datas)
        {
            Client.Send(string.Concat("gV", Client.Player.Name));
        }

        private void CreateGuild(string datas)
        {
            //TODO VERIF IF HAST THE CLIENT A GILDAOGEME

            if (Client.Player.Guild != null)
            {
                Client.Player.NClient.Send("Ea");
                return;
            }

            var infos = datas.Split('|');

            if (infos.Length < 5)
            {
                Client.Send("BN");
                return;
            }

            if (infos[0].Contains("-"))
                infos[0] = "1";
            if (infos[1].Contains("-"))
                infos[1] = "0";
            if (infos[2].Contains("-"))
                infos[2] = "1";
            if (infos[3].Contains("-"))
                infos[3] = "0";

            var bgID = 0;
            var bgColor = 0;
            var embID = 0;
            var embColor = 0;

            if (!int.TryParse(infos[0], out bgID) || !int.TryParse(infos[1], out bgColor) ||
                !int.TryParse(infos[2], out embID) || !int.TryParse(infos[3], out embColor))
            {
                Client.Send("BN");
                return;
            }

            if (infos[4].Length > 15 || Entities.Requests.GuildsRequest.GuildsList.Any(x => x.Name == infos[4]))
            {
                Client.Player.NClient.Send("Ean");
                return;
            }

            var ID = (Entities.Requests.GuildsRequest.GuildsList.Count < 1 ? 1 : Entities.Requests.GuildsRequest.GuildsList.OrderByDescending(x => x.ID).ToArray()[0].ID + 1);

            var guild = new Guild()
            {
                ID = ID,
                Name = infos[4],
                BgID = bgID,
                BgColor = bgColor,
                EmbID = embID,
                EmbColor = embColor,
                Exp = 0,
                Level = 1,
                CollectorMax = 1,
                CollectorProspection = 0,
                CollectorWisdom = 0,
                CollectorPods = 0,
                IsNewGuild = true
            };

            guild.AddMember(new GuildMember(Client.Player));

            guild.Spells.Add(462, 1);
            guild.Spells.Add(461, 1);
            guild.Spells.Add(460, 1);
            guild.Spells.Add(459, 1);
            guild.Spells.Add(458, 1);
            guild.Spells.Add(457, 1);
            guild.Spells.Add(456, 1);
            guild.Spells.Add(455, 1);
            guild.Spells.Add(454, 1);
            guild.Spells.Add(453, 1);
            guild.Spells.Add(452, 1);
            guild.Spells.Add(451, 1);

            Client.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(guild.Members[0].Rights)));
            Client.Send("gV");

            Entities.Requests.GuildsRequest.GuildsList.Add(guild);

            //REMOVE GILDAOGEME
        }

        private void ExitGuild(string datas)
        {
            if (!Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == datas))
            {
                Client.Send("BN");
                return;
            }            

            if (datas == Client.Player.Name)
            {
                if (Client.Player.Guild == null)
                {
                    Client.Send("BN");
                    return;
                }

                var guild = Client.Player.Guild;

                if (guild.Members.Count < 2)
                {
                    Client.Player.NClient.Send("Im1101");
                    return;
                }

                var member = guild.Members.First(x => x.Character == Client.Player);

                if (member.Rank == 1)
                {
                    Client.Player.NClient.Send("Im1101");
                    return;
                }

                Client.Send(string.Format("gKK{0}|{1}", Client.Player.Name, Client.Player.Name));

                member.Rank = 0;
                member.Rights = 0;
                member.ExpGived = 0;
                member.ExpGaved = 0;

                guild.Members.Remove(member);
                Client.Player.Guild = null;
                Client.Player.NClient.Send("Im0176");
            }
            else
            {
                var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == datas);

                if (character.Guild == null || Client.Player.Guild == null || (Client.Player.Guild != character.Guild))
                {
                    Client.Send("BN");
                    return;
                }
                
                var guild = Client.Player.Guild;

                if (!guild.Members.First(x => x.Character == Client.Player).CanBann)
                {
                    Client.Send("Im1101");
                    return;
                }

                var member = guild.Members.First(x => x.Character == character);

                if (member.Rank == 1)
                {
                    Client.Player.NClient.Send("Im1101");
                    return;
                }
                
                if(character.IsConnected)
                    character.NClient.Send(string.Format("gKK{0}|{1}", Client.Player.Name, Client.Player.Name));

                member.Rank = 0;
                member.Rights = 0;
                member.ExpGived = 0;
                member.ExpGaved = 0;

                Client.Player.NClient.Send(string.Concat("Im0177;", character.Name));

                guild.Members.Remove(member);
                character.Guild = null;
            }
        }

        private void ModifyRightGuild(string datas)
        {
            if (Client.Player.Guild == null)
            {
                Client.Send("BN");
                return;
            }

            var guild = Client.Player.Guild;

            if (!guild.Members.First(x => x.Character == Client.Player).CanManageRights)
            {
                Client.Send("Im1101");
                return;
            }

            var memember = guild.Members.First(x => x.Character == Client.Player);
            var infos = datas.Split('|');

            var ID = 0;
            var rank = 0;
            var rights = 0;
            var expgived = 0;

            if (!int.TryParse(infos[0], out ID) || !int.TryParse(infos[1], out rank) || !int.TryParse(infos[3], out rights) ||
                !int.TryParse(infos[2], out expgived) || !Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == ID))
            {
                Client.Send("BN");
                return;
            }

            var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == ID);

            if(character.Name == Client.Player.Name && Client.Player.Guild.Members.First
                (x => x.Character.Name == Client.Player.Name).Rights != rights)
            {
                Client.Send("BN");
                return;
            }

            var member = guild.Members.First(x => x.Character == character);

            if (member.Rank == 1 && (member.Rights != rights || member.Rank != rank))
            {
                Client.Player.NClient.Send("Im1101");
                return;
            }

            if (expgived > 90)
                expgived = 90;
            else if (expgived < 0)
                expgived = 0;

            if (rank == 1 && member.Rank != 1 && memember.Rank == 1)
            {
                memember.Rank = member.Rank;
                memember.Rights = member.Rights;


                Client.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(memember.Rights)));

                member.Rank = 1;
                member.Rights = 29695;
                member.ExpGived = expgived;

                if (member.Character.IsConnected)
                    member.Character.NClient.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(member.Rights)));

                return;
            }

            member.ExpGived = expgived;
            member.Rights = rights;
            member.Rank = rank;

            if (member.Character.IsConnected)
                member.Character.NClient.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(member.Rights)));
        }

        private void UpgradeStatsGuild(string datas)
        {
            if (Client.Player.Guild == null)
            {
                Client.Send("BN");
                return;
            }

            var guild = Client.Player.Guild;

            if (!guild.Members.First(x => x.Character == Client.Player).CanManageBoost)
            {
                Client.Send("Im1101");
                return;
            }

            switch (datas[0])
            {
                case 'p':

                    if (guild.BoostPoints > 0)
                    {
                        guild.BoostPoints -= 1;
                        guild.CollectorProspection += 1;
                        return;
                    }
                    break;

                case 'x':
                    
                    if (guild.BoostPoints > 0)
                    {
                        guild.BoostPoints -= 1;
                        guild.CollectorWisdom += 1;
                        return;
                    }
                    break;

                case 'o':
                    
                    if (guild.BoostPoints > 0)
                    {
                        guild.BoostPoints -= 1;
                        guild.CollectorPods += 20;
                        return;
                    }
                    break;

                case 'k':

                    if (guild.BoostPoints > 19)
                    {
                        guild.BoostPoints -= 20;
                        guild.CollectorMax += 1;
                        return;
                    }
                    break;
            }

            Client.Send("BN");
        }

        private void UpgradeSpellsGuild(string datas)
        {
            if (Client.Player.Guild == null)
            {
                Client.Send("BN");
                return;
            }

            var guild = Client.Player.Guild;

            if (!guild.Members.First(x => x.Character == Client.Player).CanManageBoost)
            {
                Client.Send("Im1101");
                return;
            }

            var spellID = 0;

            if(!int.TryParse(datas, out spellID) || !guild.Spells.ContainsKey(spellID) || guild.BoostPoints < 5)
            {
                Client.Send("BN");
                return;
            }

            guild.Spells[spellID]++;
            guild.BoostPoints -= 5;

            GetGuildInfos("B");
        }

        private void LetCollectorGuild(string datas)
        {
            var map = Client.Player.GetMap();

            if (Client.Player.Guild == null || map == null)
            {
                Client.Send("BN");
                return;
            }

            var guild = Client.Player.Guild;

            if (!guild.Members.First(x => x.Character == Client.Player).CanHireTaxCollector)
            {
                Client.Send("Im1101");
                return;
            }

            if (guild.CollectorMax <= guild.Collectors.Count)
            {
                Client.Player.NClient.SendMessage("Vous avez trop de percepteurs !");
                return;
            }

            if (map.Collector != null)
            {
                Client.Player.NClient.SendMessage("Un percepteur est déjà présent sur la map !");
                return;
            }

            var ID = Client.Player.GetMap().NextNpcID();

            var collector = new GuildCollector(map, Client.Player, ID)
            {
                IsNewCollector = true
            };

            guild.Collectors.Add(collector);
            Entities.Requests.CollectorsRequests.CollectorsList.Add(collector);

            Client.Player.Guild.SendMessage(string.Format("Un percepteur vient d'être posé par <b>{0}</b> en [{1},{2}] !", Client.Player.Name, Client.Player.GetMap().Model.PosX, Client.Player.GetMap().Model.PosY));
            GetGuildInfos("B");
        }

        private void GetGuildInfos(string datas)
        {
            if (Client.Player.Guild == null)
            {
                Client.Send("BN");
                return;
            }

            var packet = string.Empty;
            var guild = Client.Player.Guild;

            switch (datas[0])
            {
                case 'B':

                    packet = string.Format("gIB{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", guild.CollectorMax, guild.Collectors.Count, (guild.Level * 100), guild.Level,
                          guild.CollectorPods, guild.CollectorProspection, guild.CollectorWisdom, guild.CollectorMax, guild.BoostPoints, (1000 + (10 * guild.Level)), guild.GetSpells());

                    Client.Send(packet);
                    return;

                case 'G':

                    var lastLevel = Entities.Requests.LevelsRequests.LevelsList.OrderByDescending(x => x.Guild).Where(x => x.Guild <= guild.Exp).ToArray()[0].Guild;
                    var nextLevel = Entities.Requests.LevelsRequests.LevelsList.OrderBy(x => x.Guild).Where(x => x.Guild > guild.Exp).ToArray()[0].Guild;

                    packet = string.Format("gIG1|{0}|{1}|{2}|{3}", guild.Level, lastLevel, guild.Exp, nextLevel);

                    Client.Send(packet);
                    return;

                case 'M':

                    Client.Send(string.Concat("gIM+", string.Join("|", from c in guild.Members select c.Character.PatternGuild())));
                    return;

                case 'T':

                    packet = string.Concat("gITM+", string.Join("|", from c in guild.Collectors select c.PatternGuild()));
                    Client.Send(packet.Substring(0, packet.Length - 1));
                    return;
            }
        }

        private void GetGuildJoinRequest(string datas)
        {
            switch (datas[0])
            {
                case 'R':

                    if (!Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == datas.Substring(1)))
                    {
                        Client.Send("BN");
                        return;
                    }

                    var receiverCharacter = Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == datas.Substring(1));

                    if (receiverCharacter.Guild != null || Client.Player.Guild == null || !receiverCharacter.IsConnected)
                    {
                        if (receiverCharacter.Guild != null)
                        {
                            Client.Player.NClient.Send("Im134");
                            return;
                        }

                        Client.Send("BN");
                        return;
                    }

                    if (!Client.Player.Guild.Members.First(x => x.Character == Client.Player).CanInvite)
                    {
                        Client.Send("Im1101");
                        return;
                    }

                    Client.Player.State.ReceiverInviteGuild = receiverCharacter.ID;
                    receiverCharacter.State.SenderInviteGuild = Client.Player.ID;

                    Client.Player.State.OnWaitingGuild = true;
                    receiverCharacter.State.OnWaitingGuild = true;

                    Client.Send(string.Concat("gJR", receiverCharacter.Name));
                    receiverCharacter.NClient.Send(string.Format("gJr{0}|{1}|{2}", Client.Player.ID, Client.Player.Name, Client.Player.Guild.Name));

                    break;

                case 'K':

                    var ID = 0;

                    if (!int.TryParse(datas.Substring(1), out ID) || !Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == ID))
                    {
                        Client.Send("BN");
                        return;
                    }

                    var accepttoCharacter = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == ID);

                    if (!accepttoCharacter.IsConnected || accepttoCharacter.State.ReceiverInviteGuild != Client.Player.ID)
                    {
                        Client.Send("BN");
                        return;
                    }

                    Client.Player.State.SenderInviteGuild = -1;
                    accepttoCharacter.State.ReceiverInviteGuild = -1;

                    Client.Player.State.OnWaitingGuild = false;
                    accepttoCharacter.State.OnWaitingGuild = false;
                    
                    Client.Player.Guild = accepttoCharacter.Guild;
                    var member = new GuildMember(Client.Player);
                    var guild = Client.Player.Guild;

                    accepttoCharacter.Guild.Members.Add(member);

                    Client.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(member.Rights)));
                    accepttoCharacter.NClient.Send(string.Concat("gJKa", Client.Player.Name));

                    break;

                case 'E':

                    if (!Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == datas.Substring(1)))
                    {
                        Client.Send("BN");
                        return;
                    }

                    var refusetoCharacter = Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == datas.Substring(1));

                    if (!refusetoCharacter.IsConnected || refusetoCharacter.State.ReceiverInviteGuild == Client.Player.ID)
                    {
                        Client.Send("BN");
                        return;
                    }

                    refusetoCharacter.NClient.Send("gJEc");

                    break;
            }
        }

        #endregion

        #region Game

        private void CreateGame(string datas)
        {
            if (Client.Player.SaveMap == 0)
            {
                Client.Player.SaveMap = Client.Player.MapID;
                Client.Player.SaveCell = Client.Player.MapCell;

                Client.Send("Im06");
            }

            Client.Send(string.Concat("GCK|1|", Client.Player.Name));
            Client.Send("AR6bk");

            Client.Player.Channels.SendChannels();
            Client.Send("SLo+");
            Client.Player.SpellsInventary.SendAllSpells();
            Client.Send(string.Concat("BT", Utilities.Basic.GetActuelTime()));

            if (Client.Player.Life == 0)
                Client.Player.Life = Client.Player.Stats.GetStat(StatEnum.MaxLife).Total;

            Client.Player.ItemsInventary.RefreshBonus();
            Client.Player.SendPods();
            Client.Player.SendChararacterStats();

            Client.Player.LoadMap();

            Client.Send(string.Concat("FO", (Client.Player.Friends.WillNotifyWhenConnected ? "+" : "-")));

            if (Client.Player.Guild != null)
            {
                var guild = Client.Player.Guild;
                Client.Send(string.Format("gS{0}|{1}|{2}", guild.Name, guild.Emblem.Replace(",", "|"), Utilities.Basic.ToBase36(guild.Members.First(x => x.Character == Client.Player).Rights)));
            }
        }

        private void TeleportByPos(string packet)
        {
            if (packet[0] != 'M' || !packet.Contains(','))
                return;

            if (Client.Infos.Level < 1)
                return;

            var pos = packet.Substring(1).Split(',');
            var posx = 0;
            var posy = 0;

            if (!int.TryParse(pos[0], out posx) || !int.TryParse(pos[1], out posy))
                return;

            var maps = Entities.Requests.MapsRequests.MapsList.Where(x => x.Model.PosX == posx && x.Model.PosY == posy).ToArray();

            if (maps.Length > 0)
            {
                Client.Player.TeleportNewMap(maps[0].Model.ID, Client.Player.MapCell);
                Client.SendConsoleMessage("Character Teleported !", 0);
            }
        }

        private void GameInformations(string datas)
        {
            Client.Player.GetMap().AddPlayer(Client.Player);
            Client.Send("GDK");
        }

        private void GameAction(string datas)
        {
            var packet = 0;

            if (!int.TryParse(datas.Substring(0,3), out packet))
                return;

            switch (packet)
            {
                case 1:
                    GameMove(datas.Substring(3));
                    return;

                case 500:
                    ParseGameAction(datas.Substring(3));
                    return;

                case 300:
                    FightLaunchSpell(datas.Substring(3));
                    return;

                case 900:
                    AskChallenge(datas.Substring(3));
                    return;

                case 901:
                    AcceptChallenge(datas.Substring(3));
                    return;

                case 902:
                    RefuseChallenge(datas.Substring(3));
                    return;

                case 903:
                    FightJoin(datas.Substring(3));
                    return;
            }
        }

        private void ParseGameAction(string packet)
        {
            var infos = packet.Split(';'); 
            
            var id = 0;

            if (!int.TryParse(infos[1], out id))
                return;

            switch (id)
            {
                case 44:
                    ZaapsManager.SaveZaap(Client.Player);
                    return;

                case 114:
                    ZaapsManager.SendZaaps(Client.Player);
                    return;

                case 157:
                    ZaapisManager.SendZaapis(Client.Player);
                    return;

                default:
                    Client.Send("BN");
                    return;
            }
        }

        private void GameMove(string packet)
        {
            if (!Client.Player.State.InFight & Client.Player.State.Busy)
                return;

            var path = new Pathfinding(packet, Client.Player.GetMap(), (Client.Player.State.InFight ? Client.Player.Fighter.Cell : Client.Player.MapCell), Client.Player.MapDir, true);
            var newPath = path.RemakePath();

            if (Client.Player.State.InFight && !Client.Player.Fight.TryMove(Client.Player.Fighter, path))
                return;

            Client.Player.MapDir = path.direction;
            Client.Player.State.MoveToCell = path.destination;
            Client.Player.State.OnMove = true;

            if (Client.Player.State.InFight)
                Client.Player.Fighter.Fight.Send(string.Format("GA0;1;{0};{1}", Client.Player.ID, newPath));
            else
                Client.Player.GetMap().Send(string.Format("GA0;1;{0};{1}", Client.Player.ID, newPath));
        }

        private void EndAction(string datas)
        {
            switch (datas[0])
            {
                case 'K':

                    if (Client.Player.State.OnMove == true)
                    {
                        if (Client.Player.State.InFight)
                            Client.Player.Fighter.Cell = Client.Player.State.MoveToCell;
                        else
                        {
                            if (Client.Player.GetMap().MonstersGroups.Any(x => x.MapCell == Client.Player.State.MoveToCell))
                            {
                                var group = Client.Player.GetMap().MonstersGroups.First(x => x.MapCell == Client.Player.State.MoveToCell);
                                var fight = new MonsterFight(Client.Player, group, Client.Player.GetMap());

                                Client.Player.GetMap().AddFight(fight);
                                
                                foreach (var mob in group.Monsters)
                                {
                                    var otherMonster = new MonsterFighter(mob, fight, Client.Player.GetMap().NextNpcID());
                                    fight.FighterJoin(otherMonster, 1);
                                }           

                                Client.Player.State.OnMove = false;
                                Client.Player.State.MoveToCell = -1;
                                return;
                            }

                            Client.Player.MapCell = Client.Player.State.MoveToCell;
                        }

                        Client.Player.State.OnMove = false;
                        Client.Player.State.MoveToCell = -1;

                        Client.Send("BN");

                        if (Client.Player.GetMap().Triggers.Any(x => x.CellID == Client.Player.MapCell))
                        {
                            var trigger = Client.Player.GetMap().Triggers.First(x => x.CellID == Client.Player.MapCell);

                            if (TriggerCondition.HasConditions(Client.Player, trigger.Conditions))
                                SunDofus.World.Effects.EffectAction.ParseEffect(Client.Player, trigger.ActionID, trigger.Args);
                            else
                                Client.SendMessage("Im11");
                        }                        
                    }

                    return;

                case 'E':

                    var cell = 0;

                    if (!int.TryParse(datas.Split('|')[1], out cell))
                        return;

                    Client.Player.State.OnMove = false;
                    Client.Player.MapCell = cell;

                    return;
            }
        }

        private void UseSmiley(string packet)
        {
            int smiley;

            if (!int.TryParse(packet, out smiley))
                return;

            if (smiley < 1 | smiley > 15)
                return;

            if (!Client.Player.CanSendinSmiley())
                return;

            Client.Player.RefreshSmiley();

            if (Client.Player.State.InFight)
                Client.Player.Fight.Send("cS" + Client.Player.ID + '|' + smiley);
            else
                Client.Player.GetMap().Send("cS" + Client.Player.ID + '|' + smiley);
        }

        private void ChangeDirection(string packet)
        {
            int direction;

            if (!int.TryParse(packet, out direction))
                return;

            if (direction < 0 | direction > 7)
                return;

            Client.Player.GetMap().Send("eD" + Client.Player.ID + "|" + direction);
        }

        private void UseEmote(string packet)
        {
            int emote;

            if (!int.TryParse(packet, out emote))
                return;

            if (emote == 1)
            {
                Client.Player.Sit();
                Client.Player.GetMap().Send("eUK" + Client.Player.ID + '|' + (Client.Player.State.IsSitted ? 1 : 0));
            }
        }

        #endregion

        #region Challenge

        private void AskChallenge(string datas)
        {
            var charid = 0;

            if (!int.TryParse(datas, out charid))
                return;

            if (Client.Player.State.Busy)
                return;

            if (Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == charid))
            {
                var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

                if (Client.Player.State.Busy || character.State.Busy || Client.Player.GetMap().Model.ID != character.GetMap().Model.ID)
                {
                    Client.SendMessage("Personnage actuellement occupé ou indisponible !");
                    return;
                }

                Client.Player.State.ChallengeAsked = character.ID;
                Client.Player.State.ChallengeAsker = Client.Player.ID;
                Client.Player.State.IsChallengeAsker = true;

                character.State.ChallengeAsker = Client.Player.ID;
                character.State.ChallengeAsked = character.ID;
                character.State.IsChallengeAsked = true;

                Client.Player.GetMap().Send(string.Format("GA;900;{0};{1}", Client.Player.ID, character.ID));
            }
        }

        private void AcceptChallenge(string datas)
        {
            var charid = 0;

            if (!int.TryParse(datas, out charid))
                return;

            if (Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == charid) && Client.Player.State.ChallengeAsker == charid)
            {
                var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

                Client.Player.State.ChallengeAsker = -1;
                Client.Player.State.ChallengeAsked = -1;
                Client.Player.State.IsChallengeAsked = false;

                character.State.ChallengeAsked = -1;
                character.State.ChallengeAsker = -1;
                character.State.IsChallengeAsker = false;

                Client.Send(string.Format("GA;901;{0};{1}", Client.Player.ID, character.ID));
                character.NClient.Send(string.Format("GA;901;{0};{1}", character.ID, Client.Player.ID));

                Client.Player.GetMap().AddFight(new ChallengeFight
                    (Client.Player, character, Client.Player.GetMap()));
            }
        }

        private void RefuseChallenge(string datas)
        {
            var charid = 0;

            if (!int.TryParse(datas, out charid))
                return;

            if (Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == charid) && Client.Player.State.ChallengeAsker == charid)
            {
                Character asker = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Client.Player.State.ChallengeAsker);
                Character asked = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Client.Player.State.ChallengeAsked);

                asker.State.ChallengeAsked = -1;
                asker.State.ChallengeAsker = -1;
                asker.State.IsChallengeAsker = false;

                asked.State.ChallengeAsker = -1;
                asked.State.ChallengeAsked = -1;
                asked.State.IsChallengeAsked = false;

                asker.NClient.Send(string.Format("GA;902;{0};{1}", asker.ID, asked.ID));
                asked.NClient.Send(string.Format("GA;902;{0};{1}", asked.ID, asker.ID));
            }
        }

        #endregion

        #region Items

        private void DeleteItem(string datas)
        {
            var allDatas = datas.Split('|');
            var ID = 0;
            var quantity = 0;

            if (!int.TryParse(allDatas[0], out ID) || !int.TryParse(allDatas[1], out quantity) || quantity <= 0)
                return;

            Client.Player.ItemsInventary.DeleteItem(ID, quantity);
        }

        private void MoveItem(string datas)
        {
            var allDatas = datas.Split('|');

            var ID = 0;
            var pos = 0;
            var quantity = 1;

            if (allDatas.Length >= 3)
            {
                if (!int.TryParse(allDatas[2], out quantity))
                    return;
            }

            if (!int.TryParse(allDatas[0], out ID) || !int.TryParse(allDatas[1], out pos))
                return;

            Client.Player.ItemsInventary.MoveItem(ID, pos, quantity);
        }

        private void UseItem(string datas)
        {
            Client.Player.ItemsInventary.UseItem(datas);
        }

        #endregion

        #region StatsBoosts

        private void StatsBoosts(string datas)
        {
            var caract = 0;

            if (!int.TryParse(datas, out caract))
                return;

            var count = 0;

            switch (caract)
            {
                case 11:

                    if (Client.Player.CharactPoint < 1)
                        return;

                    if (Client.Player.Class == 11)
                    {
                        Client.Player.Stats.GetStat(StatEnum.Vitalite).Base += 2;
                        Client.Player.Life += 2;
                    }
                    else
                    {
                        Client.Player.Stats.GetStat(StatEnum.Vitalite).Base += 1;
                        Client.Player.Life += 1;
                    }

                    Client.Player.CharactPoint -= 1;
                    Client.Player.SendChararacterStats();

                    break;

                case 12:

                    if (Client.Player.CharactPoint < 3)
                        return;

                    Client.Player.Stats.GetStat(StatEnum.Sagesse).Base += 1;
                    Client.Player.CharactPoint -= 3;
                    Client.Player.SendChararacterStats();

                    break;

                case 10:

                    if (Client.Player.Class == 1 | Client.Player.Class == 7 | Client.Player.Class == 2 | Client.Player.Class == 5)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base < 51) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 50) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 150) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 250) count = 5;
                    }

                    else if (Client.Player.Class == 3 | Client.Player.Class == 9)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base < 51) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 50) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 150) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 250) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 350) count = 5;
                    }

                    else if (Client.Player.Class == 4 | Client.Player.Class == 6 | Client.Player.Class == 8 | Client.Player.Class == 10)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base < 101) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 100) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 200) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 300) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 400) count = 5;
                    }

                    else if (Client.Player.Class == 11)
                    {
                        count = 3;
                    }

                    else if (Client.Player.Class == 12)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base < 51) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 50) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Force).Base > 200) count = 3;
                    }

                    if (Client.Player.CharactPoint >= count)
                    {
                        Client.Player.Stats.GetStat(StatEnum.Force).Base += 1;
                        Client.Player.CharactPoint -= count;
                        Client.Player.SendChararacterStats();
                    }
                    else
                        Client.Send("ABE");

                    break;

                case 15:

                    if (Client.Player.Class == 1 | Client.Player.Class == 2 | Client.Player.Class == 5 | Client.Player.Class == 7 | Client.Player.Class == 10)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base < 101) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 100) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 200) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 300) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 400) count = 5;
                    }

                    else if (Client.Player.Class == 3)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base < 21) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 20) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 60) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 100) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 140) count = 5;
                    }

                    else if (Client.Player.Class == 4)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base < 51) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 50) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 150) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 250) count = 4;
                    }

                    else if (Client.Player.Class == 6 | Client.Player.Class == 8)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base < 21) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 20) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 40) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 60) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 80) count = 5;
                    }

                    else if (Client.Player.Class == 9)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base < 51) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 50) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 150) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 250) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 350) count = 5;
                    }

                    else if (Client.Player.Class == 11)
                    {
                        count = 3;
                    }

                    else if (Client.Player.Class == 12)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base < 51) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 50) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Intelligence).Base > 200) count = 3;
                    }

                    if (Client.Player.CharactPoint >= count)
                    {
                        Client.Player.Stats.GetStat(StatEnum.Intelligence).Base += 1;
                        Client.Player.CharactPoint -= count;
                        Client.Player.SendChararacterStats();
                    }
                    else
                        Client.Send("ABE");

                    break;

                case 13:

                    if (Client.Player.Class == 1 | Client.Player.Class == 4 | Client.Player.Class == 5
                        | Client.Player.Class == 6 | Client.Player.Class == 7 | Client.Player.Class == 8 | Client.Player.Class == 9)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base < 21) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 20) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 40) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 60) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 80) count = 5;
                    }

                    else if (Client.Player.Class == 2 | Client.Player.Class == 10)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base < 101) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 100) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 200) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 300) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 400) count = 5;
                    }

                    else if (Client.Player.Class == 3)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base < 101) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 100) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 150) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 230) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 330) count = 5;
                    }

                    else if (Client.Player.Class == 11)
                    {
                        count = 3;
                    }

                    else if (Client.Player.Class == 12)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base < 51) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 50) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Chance).Base > 200) count = 3;
                    }

                    if (Client.Player.CharactPoint >= count)
                    {
                        Client.Player.Stats.GetStat(StatEnum.Chance).Base += 1;
                        Client.Player.CharactPoint -= count;
                        Client.Player.SendChararacterStats();
                    }
                    else
                        Client.Send("ABE");

                    break;

                case 14:

                    if (Client.Player.Class == 1 | Client.Player.Class == 2 | Client.Player.Class == 3 | Client.Player.Class == 5
                        | Client.Player.Class == 7 | Client.Player.Class == 8 | Client.Player.Class == 10)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base < 21) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 20) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 40) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 60) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 80) count = 5;
                    }

                    else if (Client.Player.Class == 4)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base < 101) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 100) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 200) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 300) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 400) count = 5;
                    }

                    else if (Client.Player.Class == 6 | Client.Player.Class == 9)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base < 51) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 50) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 100) count = 3;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 150) count = 4;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 200) count = 5;
                    }

                    else if (Client.Player.Class == 11)
                    {
                        count = 3;
                    }

                    else if (Client.Player.Class == 12)
                    {
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base < 51) count = 1;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 50) count = 2;
                        if (Client.Player.Stats.GetStat(StatEnum.Agilite).Base > 200) count = 3;
                    }

                    if (Client.Player.CharactPoint >= count)
                    {
                        Client.Player.Stats.GetStat(StatEnum.Agilite).Base += 1;
                        Client.Player.CharactPoint -= count;
                        Client.Player.SendChararacterStats();
                    }
                    else
                        Client.Send("ABE");

                    break;
            }
        }

        #endregion

        #region Spells

        private void SpellBoost(string datas)
        {
            var spellID = 0;

            if (!int.TryParse(datas, out spellID))
                return;

            if (!Client.Player.SpellsInventary.Spells.Any(x => x.ID == spellID))
            {
                Client.Send("SUE");
                return;
            }

            var level = Client.Player.SpellsInventary.Spells.First(x => x.ID == spellID).Level;

            if (Client.Player.SpellPoint < level || level >= 6)
            {
                Client.Send("SUE");
                return;
            }

            Client.Player.SpellPoint -= level;

            var spell = Client.Player.SpellsInventary.Spells.First(x => x.ID == spellID);
            spell.ChangeLevel(spell.Level + 1);

            Client.Send(string.Format("SUK{0}~{1}", spellID, level + 1));
            Client.Player.SendChararacterStats();
        }

        private void SpellMove(string _datas)
        {
            Client.Send("BN");

            var datas = _datas.Split('|');
            var spellID = 0;
            var newPos = 0;

            if (!int.TryParse(datas[0], out spellID) || !int.TryParse(datas[1], out newPos))
                return;

            if (!Client.Player.SpellsInventary.Spells.Any(x => x.ID == spellID))
                return;

            if (Client.Player.SpellsInventary.Spells.Any(x => x.Position == newPos))
            {
                Client.Player.SpellsInventary.Spells.First(x => x.Position == newPos).Position = 25;
                Client.Player.SpellsInventary.Spells.First(x => x.ID == spellID).Position = newPos;
            }
            else
                Client.Player.SpellsInventary.Spells.First(x => x.ID == spellID).Position = newPos;
        }

        #endregion

        #region Exchange

        private void ExchangeRequest(string datas)
        {
            if (Client.Player == null || Client.Player.State.Busy)
            {
                Client.Send("BN");
                return;
            }

            var packet = datas.Split('|');
            var ID = 0;
            var receiverID = 0;

            if (!int.TryParse(packet[0],out ID) || !int.TryParse(packet[1],out receiverID))
                return;

            switch (ID)
            {
                case 0://NPC BUY/SELL
                {
                    var NPC = Client.Player.GetMap().Npcs.First(x => x.ID == receiverID);

                    if (NPC.Model.SellingList.Count == 0)
                    {
                        Client.Send("BN");
                        return;
                    }

                    Client.Player.State.OnExchange = true;
                    Client.Player.State.ActualNPC = NPC.ID;

                    Client.Send(string.Concat("ECK0|", NPC.ID));

                    var newPacket = "EL";

                    foreach (var i in NPC.Model.SellingList)
                    {
                        var item = Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == i);
                        newPacket += string.Format("{0};{1}|", i, item.EffectInfos());
                    }

                    Client.Send(newPacket.Substring(0, newPacket.Length - 1));

                    break;
                }
                case 1://Player
                {
                    if (Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == receiverID))
                    {
                        var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == receiverID);

                        if (!character.IsConnected == true && !character.State.Busy)
                        {
                            Client.Send("BN");
                            return;
                        }

                        character.NClient.Send(string.Format("ERK{0}|{1}|1", Client.Player.ID, character.ID));
                        Client.Send(string.Format("ERK{0}|{1}|1", Client.Player.ID, character.ID));

                        character.State.CurrentPlayerTrade = Client.Player.ID;
                        character.State.OnExchange = true;

                        Client.Player.State.CurrentPlayerTrade = character.ID;
                        Client.Player.State.OnExchange = true;
                    }

                    break;
                }
                case 2: // Trade with NPC
                {

                    break;
                }
            }
        }

        private void CancelExchange(string t)
        {
            Client.Send("EV");

            if (Client.Player.State.OnExchange)
                SunDofus.World.Exchanges.ExchangesManager.LeaveExchange(Client.Player);
            else if (Client.Player.State.OnExchangeWithBank)
                Client.Player.State.OnExchangeWithBank = false;
        }

        private void ExchangeBuy(string packet)
        {
            if (!Client.Player.State.OnExchange)
            {
                Client.Send("EBE");
                return;
            }

            var datas = packet.Split('|');
            var itemID = 0;
            var quantity = 1;

            if (!int.TryParse(datas[0], out itemID) || !int.TryParse(datas[1], out quantity))
                return;

            var item = Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == itemID);
            var NPC = Client.Player.GetMap().Npcs.First(x => x.ID == Client.Player.State.ActualNPC);

            if (quantity <= 0 || !NPC.Model.SellingList.Contains(itemID))
            {
                Client.Send("EBE");
                return;
            }

            var price = item.Price * quantity;

            if (Client.Player.Kamas >= price)
            {
                var newItem = new SunDofus.World.Characters.Items.CharacterItem(item);
                newItem.GeneratItem(4);
                newItem.Quantity = quantity;


                Client.Player.Kamas -= price;
                Client.Send("EBK");
                Client.Player.ItemsInventary.AddItem(newItem, false);
            }
            else
                Client.Send("EBE");
        }

        private void ExchangeSell(string datas)
        {
            if (!Client.Player.State.Busy)
            {
                Client.Send("OSE");
                return;
            }

            var packet = datas.Split('|');

            var itemID = 0;
            var quantity = 1;

            if (!int.TryParse(packet[0], out itemID) || !int.TryParse(packet[1], out quantity))
                return;

            if (!Client.Player.ItemsInventary.ItemsList.Any(x => x.ID == itemID) || quantity <= 0)
            {
                Client.Send("OSE");
                return;
            }

            var item = Client.Player.ItemsInventary.ItemsList.First(x => x.ID == itemID);

            if (item.Quantity < quantity)
                quantity = item.Quantity;

            var price = Math.Floor((double)item.Model.Price / 10) * quantity;

            if (price < 1)
                price = 1;

            Client.Player.Kamas += (int)price;
            Client.Player.ItemsInventary.DeleteItem(item.ID, quantity);
            Client.Send("ESK");
        }

        private void ExchangeMove(string datas)
        {
            switch (datas[0])
            {
                case 'G': //kamas

                    if (Client.Player.State.OnExchangeWithBank)
                    {
                        var length = (long)0;
                        var addkamas = true;

                        if (!long.TryParse(datas.Substring(1), out length))
                            return;

                        if (datas[1] == '-')
                        {
                            addkamas = false;
                            if (!long.TryParse(datas.Substring(2), out length))
                                return;
                        }

                        World.Bank.BanksManager.FindExchange(Client.Player).MoveKamas(length, addkamas);
                        return;
                    }

                    var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Client.Player.State.CurrentPlayerTrade);

                    if (!Client.Player.State.OnExchangePanel || !character.State.OnExchangePanel || character.State.CurrentPlayerTrade != Client.Player.ID)
                    {
                        Client.Send("EME");
                        return;
                    }

                    var actualExchange = SunDofus.World.Exchanges.ExchangesManager.Exchanges.First(x => (x.memberOne.Character.ID == Client.Player.ID &&
                        x.memberTwo.Character.ID == character.ID) || (x.memberTwo.Character.ID == Client.Player.ID && x.memberOne.Character.ID == character.ID));

                    long kamas = 0;

                    if (!long.TryParse(datas.Substring(1), out kamas))
                        return;

                    if (kamas > Client.Player.Kamas)
                        kamas = Client.Player.Kamas;
                    else if (kamas < 0)
                        kamas = 0;

                    actualExchange.MoveGold(Client.Player, kamas);

                    break;

                case 'O': //Items

                    var itemID = 0;
                    var quantity = 0;
                    var infos = new string[0];

                    if (Client.Player.State.OnExchangeWithBank)
                    {
                        var additem = true;
                        infos = datas.Substring(2).Split('|');

                        if (datas[1] == '-')
                            additem = false;

                        itemID = 0;
                        quantity = 0;
                        SunDofus.World.Characters.Items.CharacterItem item = null;

                        if (!int.TryParse(infos[0], out itemID) || !int.TryParse(infos[1], out quantity))
                        {
                            Client.Send("EME");
                            return;
                        }

                        if (additem)
                        {
                            if (Client.Player.ItemsInventary.ItemsList.Any(x => x.ID == itemID))
                                item = Client.Player.ItemsInventary.ItemsList.First(x => x.ID == itemID);
                            else
                                return;
                        }
                        else
                        {
                            var bank = BanksManager.FindExchange(Client.Player).Bank;

                            if (bank.Items.Any(x => x.ID == itemID))
                                item = bank.Items.First(x => x.ID == itemID);
                            else
                                return;
                        }

                        if (quantity <= 0)
                            quantity = 1;
                        else if (quantity > item.Quantity)
                            quantity = item.Quantity;

                        BanksManager.FindExchange(Client.Player).MoveItem(item, quantity, additem);
                        return;
                    }

                    var character2 = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Client.Player.State.CurrentPlayerTrade);

                    if (!Client.Player.State.OnExchangePanel || !character2.State.OnExchangePanel || character2.State.CurrentPlayerTrade != Client.Player.ID)
                    {
                        Client.Send("EME");
                        return;
                    }

                    var actualExchange2 = SunDofus.World.Exchanges.ExchangesManager.Exchanges.First(x => (x.memberOne.Character.ID == Client.Player.ID &&
                        x.memberTwo.Character.ID == character2.ID) || (x.memberTwo.Character.ID == Client.Player.ID && x.memberOne.Character.ID == character2.ID));

                    var add = (datas.Substring(1, 1) == "+" ? true : false);
                    infos = datas.Substring(2).Split('|');

                    itemID = 0;
                    quantity = 0;

                    if (!int.TryParse(infos[0], out itemID) || !int.TryParse(infos[1], out quantity))
                        return;

                    var charItem = Client.Player.ItemsInventary.ItemsList.First(x => x.ID == itemID);
                    if (charItem.Quantity < quantity)
                        quantity = charItem.Quantity;
                    if (quantity < 1)
                        return;

                    actualExchange2.MoveItem(Client.Player, charItem, quantity, add);

                    break;
            }
        }

        private void ExchangeAccept(string datas)
        {
            if (Client.Player.State.OnExchange && Client.Player.State.CurrentPlayerTrade != -1)
            {
                var character = SunDofus.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Client.Player.State.CurrentPlayerTrade);
                if (character.State.CurrentPlayerTrade == Client.Player.ID)
                {
                    SunDofus.World.Exchanges.ExchangesManager.AddExchange(character, Client.Player);
                    return;
                }
            }
            Client.Send("BN");
        }

        private void ExchangeValidate(string datas)
        {
            if (!Client.Player.State.OnExchange)
            {
                Client.Send("BN");
                return;
            }

            Client.Player.State.OnExchangeAccepted = true;

            var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Client.Player.State.CurrentPlayerTrade);

            if (!Client.Player.State.OnExchangePanel || !character.State.OnExchangePanel || character.State.CurrentPlayerTrade != Client.Player.ID)
            {
                Client.Send("EME");
                return;
            }

            var actualExchange = SunDofus.World.Exchanges.ExchangesManager.Exchanges.First(x => (x.memberOne.Character.ID == Client.Player.ID &&
                x.memberTwo.Character.ID == character.ID) || (x.memberTwo.Character.ID == Client.Player.ID && x.memberOne.Character.ID == character.ID));

            Client.Send(string.Concat("EK1", Client.Player.ID));
            character.NClient.Send(string.Concat("EK1", Client.Player.ID));

            if (character.State.OnExchangeAccepted)
                actualExchange.ValideExchange();
        }

        #endregion

        #region Party

        private void PartyInvite(string datas)
        {
            if (Entities.Requests.CharactersRequests.CharactersList.Any(x => x.Name == datas && x.IsConnected))
            {
                var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.Name == datas);
                if (character.State.Party != null || character.State.Busy)
                {
                    Client.Send(string.Concat("PIEa", datas));
                    return;
                }

                if (Client.Player.State.Party != null)
                {
                    if (Client.Player.State.Party.Members.Count < 8)
                    {
                        character.State.SenderInviteParty = Client.Player.ID;
                        character.State.OnWaitingParty = true;
                        Client.Player.State.ReceiverInviteParty = character.ID;
                        Client.Player.State.OnWaitingParty = true;

                        Client.Send(string.Format("PIK{0}|{1}", Client.Player.Name, character.Name));
                        character.NClient.Send(string.Format("PIK{0}|{1}", Client.Player.Name, character.Name));
                    }
                    else
                    {
                        Client.Send(string.Concat("PIEf", datas));
                        return;
                    }
                }
                else
                {
                    character.State.SenderInviteParty = Client.Player.ID;
                    character.State.OnWaitingParty = true;
                    Client.Player.State.ReceiverInviteParty = character.ID;
                    Client.Player.State.OnWaitingParty = true;

                    Client.Send(string.Format("PIK{0}|{1}", Client.Player.Name, character.Name));
                    character.NClient.Send(string.Format("PIK{0}|{1}", Client.Player.Name, character.Name));
                }
            }
            else
                Client.Send(string.Concat("PIEn", datas));
        }

        private void PartyRefuse(string datas)
        {
            if (Client.Player.State.SenderInviteParty == -1)
            {
                Client.Send("BN");
                return;
            }

            var character = Entities.Requests.CharactersRequests.CharactersList.First
                (x => x.ID == Client.Player.State.SenderInviteParty);

            if (character.IsConnected == false || character.State.ReceiverInviteParty != Client.Player.ID)
            {
                Client.Send("BN");
                return;
            }

            character.State.ReceiverInviteParty = -1;
            character.State.OnWaitingParty = false;

            Client.Player.State.SenderInviteParty = -1;
            Client.Player.State.OnWaitingParty = false;

            character.NClient.Send("PR");
        }

        private void PartyAccept(string datas)
        {
            if (Client.Player.State.SenderInviteParty != -1 && Client.Player.State.OnWaitingParty)
            {
                var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == Client.Player.State.SenderInviteParty);

                if (character.IsConnected == false || character.State.ReceiverInviteParty != Client.Player.ID)
                {
                    Client.Player.State.SenderInviteParty = -1;
                    Client.Player.State.OnWaitingParty = false;
                    Client.Send("BN");
                    return;
                }

                Client.Player.State.SenderInviteParty = -1;
                Client.Player.State.OnWaitingParty = false;

                character.State.ReceiverInviteParty = -1;
                character.State.OnWaitingParty = false;

                if (character.State.Party == null)
                {
                    character.State.Party = new CharacterParty(character);
                    character.State.Party.AddMember(Client.Player);
                }
                else
                {
                    if (character.State.Party.Members.Count > 7)
                    {
                        Client.Send("BN");
                        character.NClient.Send("PR");
                        return;
                    }
                    character.State.Party.AddMember(Client.Player);
                }

                character.NClient.Send("PR");
            }
            else
            {
                Client.Player.State.SenderInviteParty = -1;
                Client.Player.State.OnWaitingParty = false;
                Client.Send("BN");
            }
        }

        private void PartyLeave(string datas)
        {
            if (Client.Player.State.Party == null || !Client.Player.State.Party.Members.Keys.Contains(Client.Player))
            {
                Client.Send("BN");
                return;
            }

            if (datas == "")
                Client.Player.State.Party.LeaveParty(Client.Player.Name);
            else
            {
                var character = Client.Player.State.Party.Members.Keys.ToList().First(x => x.ID == int.Parse(datas));
                Client.Player.State.Party.LeaveParty(character.Name, Client.Player.ID.ToString());
            }
        }

        private void PartyFollow(string datas)
        {
            var add = (datas.Substring(0, 1) == "+" ? true : false);
            var charid = 0;

            if (!int.TryParse(datas.Substring(1, datas.Length - 1), out charid))
                return;

            var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

            if (add)
            {
                if (!character.IsConnected || Client.Player.State.IsFollowing)
                {
                    Client.Send("BN");
                    return;
                }

                if (character.State.Party == null || !character.State.Party.Members.ContainsKey(Client.Player)
                    || character.State.Followers.Contains(Client.Player))
                {
                    Client.Send("BN");
                    return;
                }

                lock(character.State.Followers)
                    character.State.Followers.Add(Client.Player);

                character.State.IsFollow = true;
                character.NClient.Send(string.Concat("Im052;", Client.Player.Name));

                Client.Player.State.FollowingID = character.ID;
                Client.Player.State.IsFollowing = true;

                Client.Send(string.Format("IC{0}|{1}", character.GetMap().Model.PosX, character.GetMap().Model.PosY));
                Client.Send(string.Concat("PF+", character.ID));
            }
            else
            {
                if (character.State.Party == null || !character.State.Party.Members.ContainsKey(Client.Player)
                    || !character.State.Followers.Contains(Client.Player) || character.ID != Client.Player.State.FollowingID)
                {
                    Client.Send("BN");
                    return;
                }

                lock (character.State.Followers)
                    character.State.Followers.Remove(Client.Player);

                character.State.IsFollow = false;
                character.NClient.Send(string.Concat("Im053;", Client.Player.Name));

                Client.Player.State.FollowingID = -1;
                Client.Player.State.IsFollowing = false;

                Client.Send("IC|");
                Client.Send("PF-");
            }
        }

        private void PartyGroupFollow(string datas)
        {
            var add = (datas.Substring(0, 1) == "+" ? true : false);
            var charid = 0;

            if (!int.TryParse(datas.Substring(1, datas.Length - 1), out charid))
                return;

            var character = Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == charid);

            if (add)
            {
                if (!character.IsConnected || character.State.Party == null || !character.State.Party.Members.ContainsKey(Client.Player))
                {
                    Client.Send("BN");
                    return;
                }

                foreach (var charinparty in character.State.Party.Members.Keys.Where(x => x != character))
                {
                    if (charinparty.State.IsFollowing)
                        charinparty.NClient.Send("PF-");

                    lock (character.State.Followers)
                        character.State.Followers.Add(Client.Player);

                    character.NClient.Send(string.Concat("Im052;", Client.Player.Name));

                    charinparty.State.FollowingID = character.ID;
                    charinparty.State.IsFollowing = true;

                    charinparty.NClient.Send(string.Format("IC{0}|{1}", character.GetMap().Model.PosX, character.GetMap().Model.PosY));
                    charinparty.NClient.Send(string.Concat("PF+", character.ID));
                }

                character.State.IsFollow = true;
            }
            else
            {
                if (character.State.Party == null || !character.State.Party.Members.ContainsKey(Client.Player))
                {
                    Client.Send("BN");
                    return;
                }

                foreach (var charinparty in character.State.Party.Members.Keys.Where(x => x != character))
                {
                    lock (character.State.Followers)
                        character.State.Followers.Remove(Client.Player);

                    character.NClient.Send(string.Concat("Im053;", Client.Player.Name));

                    charinparty.State.FollowingID = -1;
                    charinparty.State.IsFollowing = false;

                    charinparty.NClient.Send("IC|");
                    charinparty.NClient.Send("PF-");
                }

                character.State.IsFollow = false;
            }
        }

        #endregion

        #region Dialogs

        private void DialogCreate(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas, out id))
                return;

            if ((!Client.Player.GetMap().Npcs.Any(x => x.ID == id) && Client.Player.GetMap().Collector.ID != id) || Client.Player.State.Busy)
            {
                Client.Send("BN");
                return;
            }

            if (Client.Player.GetMap().Npcs.Any(x => x.ID == id)) //Is also a NPC
            {
                var npc = Client.Player.GetMap().Npcs.First(x => x.ID == id);

                if (npc.Model.Question == null)
                {
                    Client.Send("BN");
                    Client.SendMessage("Dialogue inexistant !");
                    return;
                }

                Client.Player.State.OnDialoging = true;
                Client.Player.State.OnDialogingWith = npc.ID;

                Client.Send(string.Concat("DCK", npc.ID));

                var packet = string.Concat("DQ", npc.Model.Question.QuestionID);

                if(npc.Model.Question.Params.Count > 0)
                {
                    packet = string.Concat(packet, ";");

                    foreach (var param in npc.Model.Question.Params)
                        packet = string.Concat(Client.Player.GetParam(param), ",");

                    packet = packet.Substring(0, packet.Length - 1);
                }

                packet = string.Concat(packet, "|");

                if (npc.Model.Question.Answers.Count(x => x.HasConditions(Client.Player)) != 0)
                {
                    foreach (var answer in npc.Model.Question.Answers)
                    {
                        if (answer.HasConditions(Client.Player))
                            packet += string.Concat(answer.AnswerID, ";");
                    }
                }

                Client.Send(packet.Substring(0, packet.Length - 1));
            }
            else //Is also a collector
            {
                var collector = Client.Player.GetMap().Collector;

                Client.Player.State.OnDialoging = true;
                Client.Player.State.OnDialogingWith = collector.ID;
                Client.Send(string.Concat("DCK", collector.ID));

                var packet = string.Format("DQ1;{0},{1},{2},{3},{4}", 
                    collector.Guild.Name, collector.Guild.CollectorPods, collector.Guild.CollectorProspection, collector.Guild.CollectorWisdom, collector.Guild.Collectors.Count);

                Client.Send(packet);
            }
        }

        private void DialogReply(string datas)
        {
            var id = 0;

            if (!int.TryParse(datas.Split('|')[1], out id))
                return;

            if (!Client.Player.GetMap().Npcs.Any(x => x.ID == Client.Player.State.OnDialogingWith))
            {
                Client.Send("BN");
                return;
            }

            var npc = Client.Player.GetMap().Npcs.First(x => x.ID == Client.Player.State.OnDialogingWith);

            if (!npc.Model.Question.Answers.Any(x => x.AnswerID == id))
            {
                Client.Send("BN");
                return;
            }

            var answer = npc.Model.Question.Answers.First(x => x.AnswerID == id);

            if (!answer.HasConditions(Client.Player))
            {
                Client.Send("BN");
                return;
            }

            answer.ApplyEffects(Client.Player);
            DialogExit("");
        }

        private void DialogExit(string datas)
        {
            Client.Send("DV");

            Client.Player.State.OnDialogingWith = -1;
            Client.Player.State.OnDialoging = false;
        }

        #endregion

        #region Fights (out)

        private void FightDetails(string packet)
        {
            int ID = 0;

            if (!int.TryParse(packet, out ID))
                return;

            Fight fight = Client.Player.GetMap().Fights.Find(x => x.ID == ID);

            if (fight != null)
            {
                StringBuilder builder = new StringBuilder("fD").Append(fight.ID).Append('|');

                foreach (Fighter fighter in fight.Team1.GetAliveFighters())
                    builder.Append(fighter.Name).Append('~').Append(fighter.Level).Append(';');

                builder.Append('|');

                foreach (Fighter fighter in fight.Team2.GetAliveFighters())
                    builder.Append(fighter.Name).Append('~').Append(fighter.Level).Append(';');

                Client.Send(builder.ToString());
            }
        }

        private void FightList(string packet)
        {
            StringBuilder builder;
            List<String> fights = new List<String>();

            foreach (Fight fight in Client.Player.GetMap().Fights)
            {
                builder = new StringBuilder();

                builder.Append(fight.ID).Append(';').Append(0).Append(';');
                builder.Append("0,0,").Append(fight.Team1.GetAliveFighters().Length).Append(';');
                builder.Append("0,0,").Append(fight.Team2.GetAliveFighters().Length).Append(';');
                builder.Append('|');

                fights.Add(builder.ToString());
            }

            Client.Send("fL" + string.Join("|", fights));
        }

        private void FightJoin(string packet)
        {
            if (Client.Player.State.Busy)
                return;

            if (!packet.Contains(";"))
            {
                int fightID;

                if (!int.TryParse(packet, out fightID))
                    return;

                Fight fight = Client.Player.GetMap().Fights.First(x => x.ID == fightID);

                if (fight == null)
                    return;

                if (fight.CanJoinSpectator())
                    fight.PlayerJoinSpectator(Client.Player);
            }
            else
            {
                string[] data = packet.Split(';');

                int fightID;

                if (!int.TryParse(data[0], out fightID))
                    return;

                Fight fight = Client.Player.GetMap().Fights.First(x => x.ID == fightID);

                if (fight == null)
                    return;

                int leaderID;

                if (!int.TryParse(data[1], out leaderID))
                    return;

                FightTeam team = fight.GetTeam(leaderID);

                if (fight.CanJoin(Client.Player, team))
                    fight.PlayerJoin(Client.Player, team.ID);
            }
        }

        #endregion

        #region Fights (toggle)

        private void ToggleFightLock(string packet)
        {
            if (!Client.Player.State.InFight)
                return;

            Client.Player.Fight.Toggle(Client.Player.Fighter, ToggleType.LOCK);
        }

        private void ToggleFightHelp(string packet)
        {
            if (!Client.Player.State.InFight)
                return;

            Client.Player.Fight.Toggle(Client.Player.Fighter, ToggleType.HELP);
        }

        private void ToggleFightParty(string packet)
        {
            if (!Client.Player.State.InFight)
                return;

            Client.Player.Fight.Toggle(Client.Player.Fighter, ToggleType.PARTY);
        }

        private void ToggleFightSpectator(string packet)
        {
            if (!Client.Player.State.InFight)
                return;

            Client.Player.Fight.Toggle(Client.Player.Fighter, ToggleType.SPECTATOR);
        }

        #endregion

        #region Fights (in)

        private void FightReady(string packet)
        {
            if (!Client.Player.State.InFight)
                return;

            Client.Player.Fight.PlayerFightReady(Client.Player.Fighter, packet[0] == '0' ? false : true);
        }

        private void FightTurnReady(string packet)
        {
            if (!Client.Player.State.InFight)
                return;

            Client.Player.Fight.State = FightState.WAITING;
            Client.Player.Fight.PlayerTurnReady(Client.Player.Fighter);
        }

        private void FightTurnPass(string packet)
        {
            if (!Client.Player.State.InFight)
                return;

            Client.Player.Fight.PlayerTurnPass(Client.Player.Fighter);
        }

        private void FightLeave(string packet)
        {
            if (!Client.Player.State.InFight & !Client.Player.State.IsSpectator)
                return;

            Fighter fighter = Client.Player.Fighter;

            if (packet.Length > 0)
            {
                if (Client.Player.Fighter.Fight.State != FightState.STARTING)
                    return;

                if (Client.Player.Fighter != Client.Player.Fighter.Team.Leader)
                    return;

                int fighterID;

                if (!int.TryParse(packet, out fighterID))
                    return;

                fighter = Client.Player.Fight.GetFighter(fighterID);

                if (fighter.Team != Client.Player.Fighter.Team)
                    return;
            }

            if (fighter == null)
                Client.Player.Fight.SpectatorLeave(Client.Player);
            else
                Client.Player.Fight.PlayerLeave(fighter);
        }

        private void FightPlacement(string packet)
        {
            if (!Client.Player.State.InFight)
                return;

            int cell;

            if (!int.TryParse(packet, out cell))
                return;

            Client.Player.Fight.PlayerPlace(Client.Player.Fighter, cell);
        }

        private void FightLaunchSpell(string datas)
        {
            if (!Client.Player.State.InFight)
                return;

            if (!datas.Contains(';'))
                return;

            string[] data = datas.Split(';');
            int spellID;
            int cell;

            if (!int.TryParse(data[0], out spellID))
                return;

            if (!int.TryParse(data[1], out cell))
                return;

            CharacterSpell spell = Client.Player.SpellsInventary.Spells.Find(x => x.ID == spellID);

            Client.Player.Fight.LaunchSpell(Client.Player.Fighter, spell, cell);
        }

        #endregion

        #endregion
    }
}
