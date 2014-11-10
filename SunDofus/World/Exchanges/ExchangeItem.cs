using SunDofus.World.Characters.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Exchanges
{
    class ExchangeItem
    {
        public CharacterItem Item { get; set; }

        public int Quantity { get; set; }

        public ExchangeItem(CharacterItem item)
        {
            Item = item;
        }

        public CharacterItem GetNewItem()
        {
            var item = new CharacterItem(Item.Model);

            item.EffectsList.Clear();

            lock(Item.EffectsList)
                Item.EffectsList.ForEach(x => item.EffectsList.Add(new Effects.EffectItem(x)));

            item.ID = ItemsHandler.GetNewID();
            item.Position = Item.Position;

            item.Quantity = Quantity;

            return item;
        }
    }
}
