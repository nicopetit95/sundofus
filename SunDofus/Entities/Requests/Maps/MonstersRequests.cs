using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SunDofus.World.Characters.Spells;

namespace SunDofus.Entities.Requests
{
    class MonstersRequests
    {
        public static List<Models.Monsters.MonsterModel> MonstersList = new List<Models.Monsters.MonsterModel>();

        public static void LoadMonsters()
        {
            var sqlText = "SELECT * FROM creatures";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var newMonsters = new Models.Monsters.MonsterModel()
                {
                    ID = sqlReader.GetInt32("ID"),
                    Name = sqlReader.GetString("Name"),
                    GfxID = sqlReader.GetInt32("GfxId"),
                    Align = sqlReader.GetInt32("Alignement"),

                    Color = SunDofus.Utilities.Basic.HexToDeci(sqlReader.GetString("Colors").Split(',')[0]),
                    Color2 = SunDofus.Utilities.Basic.HexToDeci(sqlReader.GetString("Colors").Split(',')[1]),
                    Color3 = SunDofus.Utilities.Basic.HexToDeci(sqlReader.GetString("Colors").Split(',')[2]),

                    IA = sqlReader.GetInt16("AI_Type"),

                    Min_kamas = (sqlReader.GetString("Kamas_Dropped").Split(';')[0] == "" ? 0 : int.Parse(sqlReader.GetString("Kamas_Dropped").Split(';')[0])),
                    Max_kamas = (sqlReader.GetString("Kamas_Dropped").Split(';').Length <= 1 ||
                        sqlReader.GetString("Kamas_Dropped").Split(';')[1] == "" ? 0 : int.Parse(sqlReader.GetString("Kamas_Dropped").Split(';')[0])),
                };

                foreach (var newItem in sqlReader.GetString("Items_Dropped").Split('|'))
                {
                    if (newItem == "")
                        continue;

                    var infos = newItem.Split(';');

                    if (infos.Length < 3)
                        continue;

                    newMonsters.Items.Add(new Models.Monsters.MonsterModel.MonsterItem(int.Parse(infos[0]),
                        double.Parse(infos[1]), int.Parse(infos[2])));
                }

                MonstersList.Add(newMonsters);
            }

            sqlReader.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' monsters from the database !", MonstersList.Count));
        }

        public static void LoadMonstersLevels()
        {
            var sqlText = "SELECT * FROM creatures_levels";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var newLevel = new Models.Monsters.MonsterLevelModel()
                {
                    ID = sqlReader.GetInt32("Id"),
                    CreatureID = sqlReader.GetInt32("Mob_Id"),
                    GradeID = sqlReader.GetInt16("Grade"),

                    Level = sqlReader.GetInt32("Level"),
                    Exp = sqlReader.GetInt32("Experience"),
                    AP = sqlReader.GetInt16("Pa"),
                    MP = sqlReader.GetInt16("Pm"),
                    Life = sqlReader.GetInt32("Life"),

                    RNeutral = sqlReader.GetInt32("rNeutral"),
                    RStrenght = sqlReader.GetInt32("rEarth"),
                    RIntel = sqlReader.GetInt32("rFire"),
                    RLuck = sqlReader.GetInt32("rWater"),
                    RAgility = sqlReader.GetInt32("rAir"),

                    RPa = sqlReader.GetInt32("rPA"),
                    RPm = sqlReader.GetInt32("rPM"),

                    Wisdom = sqlReader.GetInt32("Sagesse"),
                    Strenght = sqlReader.GetInt32("Force"),
                    Intel = sqlReader.GetInt32("Intelligence"),
                    Luck = sqlReader.GetInt32("Chance"),
                    Agility = sqlReader.GetInt32("Agilite"),
                };

                foreach (var newSpell in sqlReader.GetString("Spells").Split(';'))
                {
                    if (newSpell == "")
                        continue;

                    var infos = newSpell.Split('@');

                    newLevel.Spells.Add(new CharacterSpell
                        (int.Parse(infos[0]), int.Parse(infos[1]), -1));
                }

                if (MonstersList.Any(x => x.ID == newLevel.CreatureID))
                {
                    var monster = MonstersList.First(x => x.ID == newLevel.CreatureID);
                    monster.Levels.Add(newLevel);
                }
            }

            sqlReader.Close();
        }
    }
}
