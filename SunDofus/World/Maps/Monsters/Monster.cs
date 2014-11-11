using SunDofus.Entities.Models.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Monsters
{
    class Monster
    {
        public MonsterModel Model { get; set; }
        public int Level { get; set; }
        public int Life { get; set; }
        public MonsterLevelModel MonsterLevel { get; set; }

        public Monster(MonsterModel model, int grade)
        {
            Model = model;
            Level = grade;

            if(model.Levels.Any(x => x.Level == grade))
                MonsterLevel = model.Levels.First(x => x.Level == grade);
            else
                MonsterLevel = model.Levels[0];

            Life = MonsterLevel.Life;
        }
    }
}
