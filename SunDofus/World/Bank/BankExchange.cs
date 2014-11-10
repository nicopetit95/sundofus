using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Bank
{
    class BankExchange
    {
        public Bank Bank { get; set; }
        public Characters.Character Character { get; set; }

        public BankExchange(Bank bank, Characters.Character character)
        {
            Bank = bank;
            Character = character;
        }

        public void MoveKamas(long length, bool add = true)
        {
            if (add)
            {
                if (length > Character.Kamas)
                    length = Character.Kamas;
                else if (length < 0)
                    length = 0;

                Bank.Kamas += length;
                Character.Kamas -= length;

                Character.SendChararacterStats();
                Character.NClient.Send(string.Concat("EsKG", Bank.Kamas));
            }
            else
            {
                if (length > Bank.Kamas)
                    length = Bank.Kamas;
                else if (length < 0)
                    length = 0;

                Bank.Kamas -= length;
                Character.Kamas += length;

                Character.SendChararacterStats();
                Character.NClient.Send(string.Concat("EsKG", Bank.Kamas));
            }
        }

        public void MoveItem(SunDofus.World.Characters.Items.CharacterItem item, int quantity, bool add = true)
        {
            if (add)
            {
                Character.ItemsInventary.DeleteItem(item.ID, quantity);

                if (Bank.Items.Any(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position))
                {
                    var sameitem = Bank.Items.First(x => x.EffectsInfos() == item.EffectsInfos() && x.Model.ID == item.Model.ID && x.Position == item.Position);
                    sameitem.Quantity += quantity;
                    
                    Character.NClient.Send(string.Concat("EsKO+", sameitem.StorageString()));
                    return;
                }

                var newitem = item.Copy();
                newitem.Quantity = quantity;

                Bank.Items.Add(newitem);
                Character.NClient.Send(string.Concat("EsKO+", newitem.StorageString()));
            }
            else
            {
                var pods = item.Model.Pods * quantity;

                if (pods + Character.Pods > Character.Stats.GetStat(Characters.Stats.StatEnum.MaxPods).Total)
                {
                    Character.NClient.SendMessage("Vous êtes trop lourd pour éxecuter cette action !");
                    return;
                }

                if (quantity == item.Quantity)
                {
                    Bank.Items.Remove(item);
                    Character.NClient.Send(string.Concat("EsKO-", item.StorageString()));
                    Character.ItemsInventary.AddItem(item, false);
                }
                else
                {
                    item.Quantity -= quantity;
                    Character.NClient.Send(string.Concat("EsKO+", item.StorageString()));

                    var newitem = item.Copy();
                    newitem.Quantity = quantity;
                    Character.ItemsInventary.AddItem(newitem, false);
                }
            }
        }
    }
}
