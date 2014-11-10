using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Effects;

namespace SunDofus.Entities.Models.Items
{
    class SetModel
    {
        public int ID { get; set; }

        public Dictionary<int, List<EffectItem>> BonusList { get; set; }
        public List<int> ItemsList { get; set; }

        public SetModel()
        {
            ID = -1;
            ItemsList = new List<int>();
            BonusList = new Dictionary<int, List<EffectItem>>();
        }

        public void ParseItems(string _datas)
        {
            if (_datas == "") 
                return;

            foreach (var infos in _datas.Split(','))
            {
                var id = int.Parse(infos.Trim());

                if (Entities.Requests.ItemsRequests.ItemsList.Any(x => x.ID == id))
                    Entities.Requests.ItemsRequests.ItemsList.First(x => x.ID == id).Set = this.ID;

                lock(ItemsList)
                    ItemsList.Add(ID);
            }
        }

        public void ParseBonus(string _datas)
        {
            var num = 1;

            if (_datas == "") 
                return;

            foreach (var infos in _datas.Split(';'))
            {
                if (infos == "") 
                    continue;

                lock(BonusList)
                    BonusList.Add(++num, new List<EffectItem>());

                foreach (var datas in infos.Split(','))
                {
                    if (datas == "") 
                        continue;

                    var bonus = new EffectItem();
                    bonus.ID = int.Parse(datas.Split(':')[0]);
                    bonus.Value = int.Parse(datas.Split(':')[1]);

                    lock(BonusList[num])
                        BonusList[num].Add(bonus);
                }
            }
        }
    }
}
