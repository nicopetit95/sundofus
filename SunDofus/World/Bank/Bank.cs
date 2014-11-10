using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Bank
{
    class Bank
    {
        public int Owner { get; set; }
        public long Kamas { get; set; }

        public bool IsNewBank { get; set; }

        public List<Characters.Items.CharacterItem> Items { get; set; }

        public Bank()
        {
            Items = new List<Characters.Items.CharacterItem>();
        }

        public void ParseItems(string items)
        {
            if (items == "")
                return;

            var splited = items.Split(';');

            foreach (var infos in splited)
            {
                var allInfos = infos.Split('~');
                var item = new Characters.Items.CharacterItem(Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == Convert.ToInt32(allInfos[0], 16)));
                item.EffectsList.Clear();

                item.ID = Characters.Items.ItemsHandler.GetNewID();
                item.Quantity = Convert.ToInt32(allInfos[1], 16);

                if (allInfos[2] != "")
                    item.Position = Convert.ToInt32(allInfos[2], 16);
                else
                    item.Position = -1;

                if (allInfos[3] != "")
                {
                    var effectsList = allInfos[3].Split(',');

                    foreach (var effect in effectsList)
                    {
                        var NewEffect = new Effects.EffectItem();
                        string[] EffectInfos = effect.Split('#');

                        NewEffect.ID = Convert.ToInt32(EffectInfos[0], 16);

                        if (EffectInfos[1] != "")
                            NewEffect.Value = Convert.ToInt32(EffectInfos[1], 16);

                        if (EffectInfos[2] != "")
                            NewEffect.Value2 = Convert.ToInt32(EffectInfos[2], 16);

                        if (EffectInfos[3] != "")
                            NewEffect.Value3 = Convert.ToInt32(EffectInfos[3], 16);

                        NewEffect.Effect = EffectInfos[4];

                        lock (item.EffectsList)
                            item.EffectsList.Add(NewEffect);
                    }

                }

                Items.Add(item);
            }
        }

        public string GetItems()
        {
            return string.Join("|", Items); ;
        }
        public string GetExchangeItems()
        {
            return string.Join(";", from i in Items select string.Concat("O", i.ToString()));
        }
    }
}
