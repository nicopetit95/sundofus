using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Exchanges
{
    class ExchangesManager
    {
        public static List<Exchange> Exchanges = new List<Exchange>();

        public static void AddExchange(Characters.Character trader, Characters.Character traded)
        {
            lock(Exchanges)
                Exchanges.Add(new Exchange(new ExchangePlayer(trader), new ExchangePlayer(traded)));

            trader.State.OnExchangePanel = true;
            traded.State.OnExchangePanel = true;

            trader.State.CurrentPlayerTrade = traded.ID;
            traded.State.CurrentPlayerTrade = trader.ID;

            trader.NClient.Send("ECK1");
            traded.NClient.Send("ECK1");
        }

        public static void LeaveExchange(Characters.Character canceler, bool must = true)
        {
            if (canceler.State.ActualNPC != -1)
            {
                canceler.State.ActualNPC = -1;
                canceler.State.OnExchange = false;
            }

            if (canceler.State.CurrentPlayerTrade != -1)
            {
                if (SunDofus.Entities.Requests.CharactersRequests.CharactersList.Any(x => x.ID == canceler.State.CurrentPlayerTrade))
                {
                    var character = SunDofus.Entities.Requests.CharactersRequests.CharactersList.First(x => x.ID == canceler.State.CurrentPlayerTrade);

                    if (character != null)
                    {
                        if (character.IsConnected && must)
                            character.NClient.Send("EV");

                        canceler.State.CurrentPlayerTrade = -1;
                        canceler.State.OnExchange = false;
                        canceler.State.OnExchangePanel = false;
                        canceler.State.OnExchangeAccepted = false;

                        character.State.CurrentPlayerTrade = -1;
                        character.State.OnExchange = false;
                        character.State.OnExchangePanel = false;
                        character.State.OnExchangeAccepted = false;

                        lock (Exchanges)
                        {
                            if (Exchanges.Any(x => (x.memberOne.Character == canceler && x.memberTwo.Character == character) || (x.memberTwo.Character == canceler && x.memberOne.Character == character)))
                                Exchanges.Remove(Exchanges.First(x => (x.memberOne.Character == canceler && x.memberTwo.Character == character) || (x.memberTwo.Character == canceler && x.memberOne.Character == character)));
                        }
                    }
                }
            }
        }
    }
}
