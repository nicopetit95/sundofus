using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SunDofus.World.Characters;

namespace SunDofus.Entities.Requests
{
    class CharactersRequests
    {
        public static ConcurrentBag<Character> CharactersList = new ConcurrentBag<Character>();

        public static void LoadCharacters()
        {
            var sqlText = "SELECT * FROM characters";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlResult = sqlCommand.ExecuteReader();

            while (sqlResult.Read())
            {
                var character = new Character()
                {
                    ID = sqlResult.GetInt16("id"),
                    Name = sqlResult.GetString("name"),
                    Level = sqlResult.GetInt16("level"),
                    Class = sqlResult.GetInt16("class"),
                    Sex = sqlResult.GetInt16("sex"),
                    Size = 100,
                    Color = sqlResult.GetInt32("color"),
                    Color2 = sqlResult.GetInt32("color2"),
                    Color3 = sqlResult.GetInt32("color3"),

                    MapCell = int.Parse(sqlResult.GetString("mappos").Split(',')[1]),
                    MapID = int.Parse(sqlResult.GetString("mappos").Split(',')[0]),
                    MapDir = int.Parse(sqlResult.GetString("mappos").Split(',')[2]),

                    Exp = sqlResult.GetInt64("experience"),

                    IsNewCharacter = false,

                };

                character.Skin = int.Parse(string.Format("{0}{1}", character.Class, character.Sex));
                character.ParseStats(sqlResult.GetString("stats"));

                if (sqlResult.GetString("items") != "")
                    character.ItemsInventary.ParseItems(sqlResult.GetString("items"));

                if (sqlResult.GetString("spells") != "")
                    character.SpellsInventary.ParseSpells(sqlResult.GetString("spells"));

                var factionInfos = sqlResult.GetString("faction").Split(';');
                character.Faction.ID = int.Parse(factionInfos[0]);
                character.Faction.Honor = int.Parse(factionInfos[1]);
                character.Faction.Deshonor = int.Parse(factionInfos[2]);

                foreach (var zaap in sqlResult.GetString("zaaps").Split(';'))
                {
                    if (zaap == "")
                        continue;

                    character.Zaaps.Add(int.Parse(zaap));
                }

                var savepos = sqlResult.GetString("savepos").Split(';');
                character.SaveMap = int.Parse(savepos[0]);
                character.SaveCell = int.Parse(savepos[1]);

                if (character.Faction.Honor > Entities.Requests.LevelsRequests.LevelsList.OrderByDescending(x => x.Alignment).ToArray()[0].Alignment)
                    character.Faction.Level = 10;
                else
                    character.Faction.Level = Entities.Requests.LevelsRequests.LevelsList.Where(x => x.Alignment <= character.Faction.Honor).OrderByDescending(x => x.Alignment).ToArray()[0].ID;

                CharactersList.Add(character);
            }

            sqlResult.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' characters from the database !", CharactersList.Count));
        }

        public static void CreateCharacter(Character character)
        {
            var sqlText = "INSERT INTO characters VALUES(@id, @name, @level, @class, @sex, @color, @color2, @color3, @mapinfos, @stats, @items, @spells, @exp, @faction, @zaaps, @savepos)";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var P = sqlCommand.Parameters;

            P.Add(new MySqlParameter("@id", character.ID));
            P.Add(new MySqlParameter("@name", character.Name));
            P.Add(new MySqlParameter("@level", character.Level));
            P.Add(new MySqlParameter("@class", character.Class));
            P.Add(new MySqlParameter("@sex", character.Sex));
            P.Add(new MySqlParameter("@color", character.Color));
            P.Add(new MySqlParameter("@color2", character.Color2));
            P.Add(new MySqlParameter("@color3", character.Color3));
            P.Add(new MySqlParameter("@mapinfos", character.MapID + "," + character.MapCell + "," + character.MapDir));
            P.Add(new MySqlParameter("@stats", character.SqlStats()));
            P.Add(new MySqlParameter("@items", character.GetItemsToSave()));
            P.Add(new MySqlParameter("@spells", character.SpellsInventary.SaveSpells()));
            P.Add(new MySqlParameter("@exp", character.Exp));
            P.Add(new MySqlParameter("@faction", string.Concat(character.Faction.ID, ";",
                character.Faction.Honor, ";", character.Faction.Deshonor)));
            P.Add(new MySqlParameter("@zaaps", string.Join(";", character.Zaaps)));
            P.Add(new MySqlParameter("@savepos", string.Concat(character.SaveMap, ";", character.SaveCell)));

            sqlCommand.ExecuteNonQuery();

            character.IsNewCharacter = false;
        }

        public static void SaveCharacter(Character character)
        {
            if (character.IsNewCharacter && !character.IsDeletedCharacter)
            {
                CreateCharacter(character);
                return;
            }
            else if (character.IsDeletedCharacter)
            {
                DeleteCharacter(character.Name);
                return;
            }
            else if (!character.IsDeletedCharacter && !character.IsNewCharacter)
            {
                var sqlText = "UPDATE characters SET id=@id, name=@name, level=@level, class=@class, sex=@sex," +
                    " color=@color, color2=@color2, color3=@color3, mappos=@mapinfos, stats=@stats, items=@items, spells=@spells, experience=@exp, faction=@faction, zaaps=@zaaps, savepos=@savepos WHERE id=@id";
                var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

                var P = sqlCommand.Parameters;
                P.Add(new MySqlParameter("@id", character.ID));
                P.Add(new MySqlParameter("@name", character.Name));
                P.Add(new MySqlParameter("@level", character.Level));
                P.Add(new MySqlParameter("@class", character.Class));
                P.Add(new MySqlParameter("@sex", character.Sex));
                P.Add(new MySqlParameter("@color", character.Color));
                P.Add(new MySqlParameter("@color2", character.Color2));
                P.Add(new MySqlParameter("@color3", character.Color3));
                P.Add(new MySqlParameter("@mapinfos", character.MapID + "," + character.MapCell + "," + character.MapDir));
                P.Add(new MySqlParameter("@stats", character.SqlStats()));
                P.Add(new MySqlParameter("@items", character.GetItemsToSave()));
                P.Add(new MySqlParameter("@spells", character.SpellsInventary.SaveSpells()));
                P.Add(new MySqlParameter("@exp", character.Exp));
                P.Add(new MySqlParameter("@faction", string.Concat(character.Faction.ID, ";",
                    character.Faction.Honor, ";", character.Faction.Deshonor)));
                P.Add(new MySqlParameter("@zaaps", string.Join(";", character.Zaaps)));
                P.Add(new MySqlParameter("@savepos", string.Concat(character.SaveMap, ";", character.SaveCell)));

                sqlCommand.ExecuteNonQuery();
            }
        }

        public static void DeleteCharacter(string name)
        {
            var sqlText = "DELETE FROM characters WHERE name=@CharName";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            sqlCommand.Parameters.Add(new MySqlParameter("@CharName", name));

            sqlCommand.ExecuteNonQuery();
        }

        public static bool ExistsName(string name)
        {
            return CharactersList.Any(x => x.Name == name);
        }
    }
}
