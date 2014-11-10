using SunDofus.Entities.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Characters.Items
{
    class CharacterItem
    {
        public int ID { get; set; }
        public int Position { get; set; }
        public int Quantity { get; set; }

        public ItemModel Model { get; set; }
        public List<Effects.EffectItem> EffectsList { get; set; }

        public CharacterItem(ItemModel model)
        {
            Model = model;

            EffectsList = new List<Effects.EffectItem>();

            lock(EffectsList)
                Model.EffectsList.ForEach(x => EffectsList.Add(new Effects.EffectItem(x)));

            Position = -1;
        }

        public string EffectsInfos()
        {
            return string.Join(",", EffectsList);
        }

        public void GeneratItem(int jet = 4)
        {
            this.Quantity = 1;
            this.Position = -1;

            EffectsList.ForEach(x => GetNewJet(x, jet));
        }

        public void GetNewJet(Effects.EffectItem effect, int jet = 3)
        {
            if (effect.ID == 91 | effect.ID == 92 | effect.ID == 93 | effect.ID == 94 | effect.ID == 95 | effect.ID == 96 | effect.ID == 97 | effect.ID == 98 | effect.ID == 99 | effect.ID == 100 | effect.ID == 101) { }
            else if (effect.ID == 800)
            {
                effect.Value3 = 10; // PDV Des familiers !
            }
            else
            {
                effect.Value = Utilities.Basic.GetRandomJet(effect.Effect, jet);
                effect.Value2 = -1;
            }
        }

        public CharacterItem Copy()
        {
            var item = new CharacterItem(Model);
            item.EffectsList.Clear();

            lock(item.EffectsList)
                EffectsList.ForEach(x => item.EffectsList.Add(new Effects.EffectItem(x)));

            item.ID = ItemsHandler.GetNewID();
            item.Position = Position;
            item.Quantity = Quantity;

            return item;
        }

        public string StorageString()
        {
            return string.Format("{0}|{1}|{2}|{3}", ID, Utilities.Basic.DeciToHex(Quantity), Model.ID, EffectsInfos());
        }

        public string SaveString()
        {
            return string.Format("{0}~{1}~{2}~{3}", Utilities.Basic.DeciToHex(Model.ID), Utilities.Basic.DeciToHex(Quantity),
                (Position == -1 ? "" : Utilities.Basic.DeciToHex(Position)), EffectsInfos());
        }

        public override string ToString()
        {
            return string.Format("{0}~{1}~{2}~{3}~{4}",Utilities.Basic.DeciToHex(ID), Utilities.Basic.DeciToHex(Model.ID),
                Utilities.Basic.DeciToHex(Quantity), (Position == -1 ? "" : Utilities.Basic.DeciToHex(Position)), EffectsInfos());
        }
    }
}
