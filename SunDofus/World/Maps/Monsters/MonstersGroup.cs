using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace SunDofus.World.Maps.Monsters
{
    class MonstersGroup
    {
        public List<Monster> Monsters { get; set; }

        public int ID { get; set; }
        public int MaxSize { get; set; }

        private Map map;
        private int cell;
        private int dir;

        private Timer timer;
        private Dictionary<int, List<int>> mbase;

        public MonstersGroup(Dictionary<int, List<int>> monsters, Map map)
        {

            Monsters = new List<Monster>();
            mbase = monsters;

            this.map = map;
            MaxSize = map.Model.MaxGroupSize;

            ID = map.NextNpcID();

            RefreshMappos();
            RefreshMonsters();

            timer = new Timer();
            timer.Enabled = true;
            timer.Interval = Utilities.Basic.Rand(10000, 15000);
            timer.Elapsed += new ElapsedEventHandler(this.Move);
        }

        private void Move(object e, EventArgs e2)
        {
            timer.Interval = Utilities.Basic.Rand(10000, 15000);

            var path = new Maps.Pathfinding("", map, cell, dir);
            var newDir = Utilities.Basic.Rand(0, 3) * 2 + 1;
            var newCell = Pathfinding.NextCell(map, cell, newDir);

            if (newCell <= 0)
                return;

            path.UpdatePath(Maps.Pathfinding.GetDirChar(dir) + Maps.Pathfinding.GetCellChars(cell) + Maps.Pathfinding.GetDirChar(newDir) +
                Maps.Pathfinding.GetCellChars(newCell));

            var startpath = path.GetStartPath;
            var cellpath = path.RemakePath();

            if (!map.RushablesCells.Contains(newCell))
                return;

            if (cellpath != "")
            {
                cell = path.destination;
                dir = path.direction;

                var packet = string.Format("GA0;1;{0};{1}", ID, startpath + cellpath);

                map.Send(packet);
            }
        }

        private void RefreshMonsters()
        {
            var i = Utilities.Basic.Rand(1, MaxSize);
            for (int size = 1; size <= i; size++)
            {
                var mob = ReturnNewMonster();

                if (mob == null)
                    continue;

                lock(Monsters)
                    Monsters.Add(mob);
            }
        }

        private Monster ReturnNewMonster()
        {
            var key = mbase.Keys.ToList()[Utilities.Basic.Rand(0, mbase.Count - 1)];
            var value = mbase[key][Utilities.Basic.Rand(0, mbase[key].Count - 1)];

            if (!Entities.Requests.MonstersRequests.MonstersList.Any(x => x.ID == key))
                return null;

            return new Monster(Entities.Requests.MonstersRequests.MonstersList.First(x => x.ID == key), value);
        }

        private void RefreshMappos()
        {
            dir = Utilities.Basic.Rand(0, 3) * 2 + 1;
            cell = map.RushablesCells[Utilities.Basic.Rand(0, map.RushablesCells.Count - 1)];
        }

        public string PatternOnMap()
        {
            var packet = string.Format("|+{0};{1};0;{2};", cell, dir, ID);

            var ids = "";
            var skins = "";
            var lvls = "";
            var colors = "";

            var first = true;
            foreach (var monster in Monsters)
            {
                if (first)
                    first = false;

                else
                {
                    ids += ",";
                    skins += ",";
                    lvls += ",";
                    colors += ",";
                }

                var model = monster.Model;
                ids += model.ID;
                skins += model.GfxID + "^100";
                lvls += monster.Level;

                colors += string.Format("{0},{1},{2};0,0,0,0", Utilities.Basic.DeciToHex(model.Color),
                    Utilities.Basic.DeciToHex(model.Color2), Utilities.Basic.DeciToHex(model.Color3));
            }

            packet += string.Format("{0};-3;{1};{2};{3}", ids, skins, lvls, colors);

            return packet;
        }
    }
}
