using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.Utilities;
using SunDofus.Network;
using System.Reflection;
using System.Threading;
using SunDofus.Network.Realm;
using SunDofus.Network.Game;
using SunDofus.Entities.Requests;

namespace SunDofus
{
    class Program
    {
        public static GameServer GameServer { get; private set; }
        public static RealmServer RealmServer { get; private set; }
        public static Config Config { get; private set; }
        public static MySQLHelper DBHelper { get; private set; }

        static void Main(string[] args)
        {
            Basic.StartTime = DateTime.Now;
            
            try
            {
                Config = Config.Get();
                Loggers.InitializeLoggers();
                DBHelper = new MySQLHelper();

                AccountsRequests.ResetConnectedValue();

                Console.Title = string.Format("{0} | Server '{1}'", 
                    string.Concat("SunDofus v", Config.Version(Assembly.GetExecutingAssembly().FullName.Split(',')[1].Replace("Version=", "").Trim())),
                    Config.GameID);

                Entities.Requests.LevelsRequests.LoadLevels();

                Entities.Requests.ItemsRequests.LoadItems();
                Entities.Requests.ItemsRequests.LoadItemsSets();
                Entities.Requests.ItemsRequests.LoadUsablesItems();

                Entities.Requests.SpellsRequests.LoadSpells();
                Entities.Requests.SpellsRequests.LoadSpellsToLearn();

                Entities.Requests.MonstersRequests.LoadMonsters();
                Entities.Requests.MonstersRequests.LoadMonstersLevels();

                Entities.Requests.MapsRequests.LoadMaps();

                Entities.Requests.TriggersRequests.LoadTriggers();

                Entities.Requests.ZaapsRequests.LoadZaaps();
                Entities.Requests.ZaapisRequests.LoadZaapis();

                Entities.Requests.NoPlayerCharacterRequests.LoadNPCsAnswers();
                Entities.Requests.NoPlayerCharacterRequests.LoadNPCsQuestions();
                Entities.Requests.NoPlayerCharacterRequests.LoadNPCs();

                Entities.Requests.BanksRequests.LoadBanks();
                Entities.Requests.CharactersRequests.LoadCharacters();
                Entities.Requests.GuildsRequest.LoadGuilds();
                Entities.Requests.CollectorsRequests.LoadCollectors();

                World.World.Save.InitSaveThread();

                RealmServer = new Network.Realm.RealmServer();
                RealmServer.Start();

                GameServer = new Network.Game.GameServer();
                GameServer.Start();
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }

            Console.ReadLine();
        }
    }
}
