using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace SunDofus.World.Guilds
{
    class GuildCollector
    {
        public int ID { get; set; }
        public int Owner { get; set; }
        public int Cell { get; set; }
        public int Dir { get; set; }

        public int[] Name { get; set; }

        public bool IsInFight { get; set; }
        public bool IsNewCollector { get; set; }
        public bool MustDelete { get; set; }

        public Guild Guild { get; set; }
        public Maps.Map Map { get; set; }

        private Timer timer;

        public GuildCollector(Maps.Map map, Characters.Character owner, int id)
        {
            ID = id;
            IsInFight = false;
            Guild = owner.Guild;

            Map = map;
            Map.Collector = this;

            Owner = owner.ID;
            Cell = owner.MapCell;
            Dir = 3;

            Name = new int[2] { Utilities.Basic.Rand(1, 39), Utilities.Basic.Rand(1, 71) };

            timer = new Timer();
            timer.Enabled = true;
            timer.Interval = Utilities.Basic.Rand(5000, 15000);
            timer.Elapsed += new ElapsedEventHandler(this.Move);
            timer.Start();

            Map.Send(string.Concat("GM",PatternMap()));
        }

        private void Move(object sender, EventArgs e)
        {
            timer.Interval = Utilities.Basic.Rand(5000, 15000);

            var path = new Maps.Pathfinding("", Map, Cell, Dir);
            var newDir = Utilities.Basic.Rand(0, 3) * 2 + 1;
            var newCell = Maps.Pathfinding.NextCell(Map, Cell, newDir);

            if (newCell <= 0)
                return;

            path.UpdatePath(Maps.Pathfinding.GetDirChar(Dir) + Maps.Pathfinding.GetCellChars(Cell) + 
                Maps.Pathfinding.GetDirChar(newDir) + Maps.Pathfinding.GetCellChars(newCell));

            var startpath = path.GetStartPath;
            var cellpath = path.RemakePath();

            if (!Map.RushablesCells.Contains(newCell))
                return;

            if (cellpath != "")
            {
                Cell = path.destination;
                Dir = path.direction;

                var packet = string.Format("GA0;1;{0};{1}", ID, startpath + cellpath);

                Map.Send(packet);
            }
        }

        public string PatternGuild()
        {
            return string.Format("{0};{1};{2};{3};{4}", ID, string.Join(",", Name), Utilities.Basic.ToBase36(Map.Model.ID),
                Map.Model.PosX, Map.Model.PosY, ";0;0;10000;7;?,?,1,2,3,4,5|");
        }

        public string PatternMap()
        {
            return string.Format("|+{0};{1};0;{2};{3};-6;6000^100;;{4};{5}", Cell, Dir, ID, string.Join(",", Name), Guild.Name, Guild.Emblem);
        }
    }
}
