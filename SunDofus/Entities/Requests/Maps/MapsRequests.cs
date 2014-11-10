using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SunDofus.World.Maps;

namespace SunDofus.Entities.Requests
{
    class MapsRequests
    {
        public static List<Map> MapsList = new List<Map>();

        public static void LoadMaps()
        {
            var sqlText = "SELECT * FROM maps";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var map = new Models.Maps.MapModel()
                {
                    ID = sqlReader.GetInt32("id"),
                    Date = sqlReader.GetString("date"),
                    Width = sqlReader.GetInt16("width"),
                    Height = sqlReader.GetInt16("heigth"),
                    Capabilities = sqlReader.GetInt16("capabilities"),
                    Mappos = sqlReader.GetString("mappos"),
                    MapData = sqlReader.GetString("mapData"),
                    Key = sqlReader.GetString("key"),
                    MaxMonstersGroup = sqlReader.GetInt16("numgroup"),
                    MaxGroupSize = sqlReader.GetInt16("groupsize"),
                };

                foreach (var newMonster in sqlReader.GetString("monsters").Split('|'))
                {
                    if (newMonster == "")
                        continue;

                    var infos = newMonster.Split(',');

                    if (infos.Length < 2)
                        continue;
                    if (infos[1].Length < 1)
                        continue;

                    int creature = int.Parse(infos[0]);
                    if (!map.Monsters.ContainsKey(creature))
                        map.Monsters.Add(creature, new List<int>());

                    map.Monsters[creature].Add(int.Parse(infos[1]));
                }

                map.ParsePos();

                MapsList.Add(new Map(map));
            }

            sqlReader.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' maps from the database !", MapsList.Count));
        }
    }
}
