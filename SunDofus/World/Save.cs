using SunDofus.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SunDofus.World.World
{
    class Save
    {        
        private static Timer timer;

        public static void InitSaveThread()
        {
            timer = new Timer((e) =>
            {
                SaveWorld();
                timer.Change(600 * 1000, Timeout.Infinite);
            }, null, 600 * 1000, Timeout.Infinite);
        }

        public static void SaveWorld()
        {
            CharactersRequests.CharactersList.Where(x => x.IsConnected).ToList().ForEach(x => x.NClient.Send("Im1164"));
            Program.GameServer.State = Network.Game.GameServer.GameState.Maintenance;

            SaveChararacters();
            SaveGuilds();
            SaveCollectors();
            SaveBanks();

            SunDofus.Entities.Requests.CharactersRequests.CharactersList.Where(x => x.IsConnected).ToList().ForEach(x => x.NClient.Send("Im1165"));
            Program.GameServer.State = Network.Game.GameServer.GameState.Online;
        }

        private static void SaveChararacters()
        {
            foreach (var character in SunDofus.Entities.Requests.CharactersRequests.CharactersList)
                Entities.Requests.CharactersRequests.SaveCharacter(character);
        }

        private static void SaveGuilds()
        {
            foreach (var guild in Entities.Requests.GuildsRequest.GuildsList)
                Entities.Requests.GuildsRequest.SaveGuild(guild);
        }

        private static void SaveCollectors()
        {
            foreach (var collector in Entities.Requests.CollectorsRequests.CollectorsList)
                Entities.Requests.CollectorsRequests.SaveCollector(collector);
        }

        private static void SaveBanks()
        {
            foreach (var bank in Entities.Requests.BanksRequests.BanksList)
                Entities.Requests.BanksRequests.SaveBank(bank);
        }
    }
}
