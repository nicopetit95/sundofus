using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Entities.Models.Maps
{
    class MapModel
    {
        public int ID { get; set; }
        public string Date { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Capabilities { get; set; }

        public int PosX { get; set; }
        public int PosY { get; set; }
        public int SubArea { get; set; }

        public string MapData { get; set; }
        public string Key { get; set; }
        public string Mappos { get; set; }

        public int MaxMonstersGroup { get; set; }
        public int MaxGroupSize { get; set; }

        public Dictionary<int, List<int>> Monsters { get; set; }

        public MapModel()
        {
            Monsters = new Dictionary<int, List<int>>();
        }

        public void ParsePos()
        {
            var datas = Mappos.Split(',');
            if (datas.Length < 3)
            {
                Utilities.Loggers.Errors.Write(String.Format("Map {0} has invalid position", ID));
                return;
            }
            PosX = int.Parse(datas[0]);
            PosY = int.Parse(datas[1]);
            SubArea = int.Parse(datas[2]);
        }
    }
}
