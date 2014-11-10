using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Entities.Models.Spells
{
    class SpellModel
    {
        public int ID { get; set; }
        public int Sprite { get; set; }

        public string SpriteInfos { get; set; }

        public List<SpellLevelModel> Levels { get; set; }

        public SpellModel()
        {
            Levels = new List<SpellLevelModel>();
            ID = -1;
            Sprite = -1;
            SpriteInfos = "";
        }

        public void ParseLevel(string datas, int lvl)
        {
            if (datas == "-1")
                return;

            var level = new SpellLevelModel();
            var stats = datas.Split(',');

            var effect = stats[0];
            var effectCC = stats[1];

            if (stats[2] != "" && stats[2] != "-1")
                level.Cost = int.Parse(stats[2]);
            else
                level.Cost = 6;

            level.Level = lvl;

            level.MinRP = int.Parse(stats[3]);
            level.MaxRP = int.Parse(stats[4]);
            level.CC = int.Parse(stats[5]);
            level.EC = int.Parse(stats[6]);

            level.isOnlyLine = (stats[7] == "true" ? true : false);
            level.isOnlyViewLine = (stats[8] == "true" ? true : false);
            level.isEmptyCell = (stats[9] == "true" ? true : false);
            level.isAlterablePO = (stats[10] == "true" ? true : false);

            level.MaxPerTurn = int.Parse(stats[12]);
            level.MaxPerPlayer = int.Parse(stats[13]);
            level.TurnNumber = int.Parse(stats[14]);
            level.Type = stats[15].Substring(1);
            level.isECEndTurn = (stats[19] == "true" ? true : false);

            level.ParseEffect(effect, false);
            level.ParseEffect(effectCC, true);

            Levels.Add(level);
        }
    }
}
