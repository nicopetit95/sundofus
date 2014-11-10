using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Entities.Models.Levels
{
    class LevelModel
    {
        public int ID { get; set; }

        public long Character { get; set; }
        public long Job { get; set; }
        public long Alignment { get; set; }
        public long Guild { get; set; }
        public long Mount { get; set; }

        public LevelModel(long max = 0)
        {
            Character = max;
            Job = max;
            Mount = max;
            Alignment = max;
            Guild = max;
        }
    }
}
