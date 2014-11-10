using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.Entities.Requests
{
    class SpellsRequests
    {
        public static List<Entities.Models.Spells.SpellModel> SpellsList = new List<Entities.Models.Spells.SpellModel>();
        public static List<Entities.Models.Spells.SpellToLearnModel> SpellsToLearnList = new List<Entities.Models.Spells.SpellToLearnModel>();

        public static void LoadSpells()
        {
            var sqlText = "SELECT * FROM spells";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var spell = new Entities.Models.Spells.SpellModel()
                {
                    ID = sqlReader.GetInt16("id"),
                    Sprite = sqlReader.GetInt16("sprite"),
                    SpriteInfos = sqlReader.GetString("spriteInfos"),
                };

                for (int i = 1; i <= 6; i++)
                    spell.ParseLevel(sqlReader.GetString("lvl" + i), i);

                SpellsList.Add(spell);
            }

            sqlReader.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' spells from the database !", SpellsList.Count));
        }

        public static void LoadSpellsToLearn()
        {
            var sqlText = "SELECT * FROM spells_learn";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var spell = new Entities.Models.Spells.SpellToLearnModel()
                {
                    Race = sqlReader.GetInt16("Classe"),
                    Level = sqlReader.GetInt16("Level"),
                    SpellID = sqlReader.GetInt16("SpellId"),
                    Pos = sqlReader.GetInt16("Position"),
                };

                SpellsToLearnList.Add(spell);
            }

            sqlReader.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' spells to learn from the database !", SpellsToLearnList.Count));
        }
    }
}
