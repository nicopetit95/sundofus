using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.Entities.Requests
{
    class LevelsRequests
    {
        public static List<Models.Levels.LevelModel> LevelsList = new List<Models.Levels.LevelModel>();

        public static void LoadLevels()
        {
            var sqlText = "SELECT * FROM levels";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var level = new Models.Levels.LevelModel()
                {
                    ID = sqlReader.GetInt16("Level"),
                    Character = sqlReader.GetInt64("Character"),
                    Job = sqlReader.GetInt64("Job"),
                    Mount = sqlReader.GetInt64("Mount"),
                    Alignment = sqlReader.GetInt64("Pvp"),
                    Guild = sqlReader.GetInt64("Guild"),
                };

                LevelsList.Add(level);
            }

            sqlReader.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' levels from the database !", LevelsList.Count));
        }

        public static Models.Levels.LevelModel ReturnLevel(int _level)
        {
            if (LevelsList.Any(x => x.ID == _level))
                return LevelsList.First(x => x.ID == _level);
            else
                return new Models.Levels.LevelModel(long.MaxValue);
        }

        public static int MaxLevel()
        {
            return LevelsList.First(x => x.ID > 0 && LevelsList.Any(y => y.ID > x.ID) == false).ID - 1;
        }
    }
}
