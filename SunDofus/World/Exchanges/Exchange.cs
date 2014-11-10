using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Exchanges
{
    class Exchange
    {
        public ExchangePlayer memberOne { get; set; }
        public ExchangePlayer memberTwo { get; set; }

        public Exchange(ExchangePlayer playerOne, ExchangePlayer playerTwo)
        {
            memberOne = playerOne;
            memberTwo = playerTwo;
        }

        public void Reset()
        {
            if (!memberOne.IsNpc)
                memberOne.Character.State.OnExchangeAccepted = false;

            if (!memberTwo.IsNpc)
                memberTwo.Character.State.OnExchangeAccepted = false;

            memberOne.Send(string.Concat("EK0", memberOne.ID));
            memberOne.Send(string.Concat("EK0", memberTwo.ID));

            memberTwo.Send(string.Concat("EK0", memberTwo.ID));
            memberTwo.Send(string.Concat("EK0", memberOne.ID));
        }

        public void MoveGold(Characters.Character character, long kamas)
        {
            if (memberOne.Character == character)
            {
                Reset();

                memberOne.Kamas = kamas;

                memberOne.Send(string.Concat("EMKG", kamas));
                memberTwo.Send(string.Concat("EmKG", kamas));
            }
            else if (memberTwo.Character == character)
            {
                Reset();
                memberTwo.Kamas = kamas;

                memberTwo.Send(string.Concat("EMKG", kamas));
                memberOne.Send(string.Concat("EmKG", kamas));
            }
        }

        public void MoveItem(Characters.Character character, Characters.Items.CharacterItem item, int quantity, bool add)
        {
            if (memberOne.Character == character)
            {
                if (add)
                {
                    Reset();

                    lock (memberOne.Items)
                    {
                        if (memberOne.Items.Any(x => x.Item == item))
                        {
                            var item2 = memberOne.Items.First(x => x.Item == item);
                            item2.Quantity += quantity;

                            if (item2.Quantity > item.Quantity)
                                item2.Quantity = item.Quantity;

                            memberOne.Send(string.Format("EMKO+{0}|{1}", item.ID, item2.Quantity));
                            memberTwo.Send(string.Format("EmKO+{0}|{1}|{2}|{3}", item.ID, item2.Quantity, item.Model.ID, item.EffectsInfos()));
                        }
                        else
                        {
                            var newItem = new ExchangeItem(item);
                            newItem.Quantity = quantity;

                            memberOne.Items.Add(newItem);

                            memberOne.Send(string.Format("EMKO+{0}|{1}", item.ID, newItem.Quantity));
                            memberTwo.Send(string.Format("EmKO+{0}|{1}|{2}|{3}", item.ID, newItem.Quantity, item.Model.ID, item.EffectsInfos()));
                        }
                    }
                }
                else
                {
                    Reset();

                    lock (memberOne.Items)
                    {
                        var Item = memberOne.Items.First(x => x.Item == item);
                        if (Item.Quantity <= quantity)
                        {
                            memberOne.Items.Remove(Item);

                            memberOne.Send(string.Concat("EMKO-", Item.Item.ID));
                            memberTwo.Send(string.Concat("EmKO-", Item.Item.ID));
                        }
                        else
                        {
                            Item.Quantity -= quantity;

                            memberOne.Send(string.Format("EMKO+{0}|{1}", Item.Item.ID, Item.Quantity));
                            memberTwo.Send(string.Format("EmKO+{0}|{1}|{2}|{3}", Item.Item.ID, Item.Quantity, Item.Item.Model.ID, Item.Item.EffectsInfos()));
                        }
                    }
                }
            }
            else if (memberTwo.Character == character)
            {
                if (add)
                {
                    Reset();

                    lock (memberTwo.Items)
                    {
                        if (memberTwo.Items.Any(x => x.Item == item))
                        {
                            var item2 = memberTwo.Items.First(x => x.Item == item);
                            item2.Quantity += quantity;

                            if (item2.Quantity > item.Quantity)
                                item2.Quantity = item.Quantity;

                            memberTwo.Send(string.Format("EMKO+{0}|{1}", item.ID, item2.Quantity));
                            memberOne.Send(string.Format("EmKO+{0}|{1}|{2}|{3}", item.ID, item2.Quantity, item.Model.ID, item.EffectsInfos()));
                        }
                        else
                        {
                            var newItem = new ExchangeItem(item);
                            newItem.Quantity = quantity;

                            memberTwo.Items.Add(newItem);

                            memberTwo.Send(string.Format("EMKO+{0}|{1}", item.ID, newItem.Quantity));
                            memberOne.Send(string.Format("EmKO+{0}|{1}|{2}|{3}", item.ID, newItem.Quantity, item.Model.ID, item.EffectsInfos()));
                        }
                    }
                }
                else
                {
                    Reset();

                    lock (memberTwo.Items)
                    {
                        var Item = memberTwo.Items.First(x => x.Item == item);
                        if (Item.Quantity <= quantity)
                        {
                            memberTwo.Items.Remove(Item);

                            memberTwo.Send(string.Concat("EMKO-", Item.Item.ID));
                            memberOne.Send(string.Concat("EmKO-", Item.Item.ID));
                        }
                        else
                        {
                            Item.Quantity -= quantity;

                            memberTwo.Send(string.Format("EMKO+{0}|{1}", Item.Item.ID, Item.Quantity));
                            memberOne.Send(string.Format("EmKO+{0}|{1}|{2}|{3}", Item.Item.ID, Item.Quantity, Item.Item.Model.ID, Item.Item.EffectsInfos()));
                        }
                    }
                }
            }
        }

        public void ValideExchange()
        {
            memberOne.Character.Kamas = memberOne.Character.Kamas - memberOne.Kamas + memberTwo.Kamas;
            memberTwo.Character.Kamas = memberTwo.Character.Kamas - memberTwo.Kamas + memberOne.Kamas;

            foreach (var item in memberOne.Items)
            {
                var charItem = item.GetNewItem();

                memberTwo.Character.ItemsInventary.AddItem(charItem, false);
                memberOne.Character.ItemsInventary.DeleteItem(item.Item.ID, item.Quantity);
            }

            foreach (var item in memberTwo.Items)
            {
                var charItem = item.GetNewItem();

                memberOne.Character.ItemsInventary.AddItem(charItem, false);
                memberTwo.Character.ItemsInventary.DeleteItem(item.Item.ID, item.Quantity);
            }

            memberOne.Character.SendChararacterStats();
            memberTwo.Character.SendChararacterStats();

            memberOne.Send("EVa");
            memberTwo.Send("EVa");

            ExchangesManager.LeaveExchange(memberOne.Character, false);
        }
    }
}
