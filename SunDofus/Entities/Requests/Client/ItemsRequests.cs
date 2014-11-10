using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.Entities.Requests
{
    class ItemsRequests
    {
        public static List<Models.Items.ItemModel> ItemsList = new List<Models.Items.ItemModel>();
        public static List<Models.Items.SetModel> SetsList = new List<Models.Items.SetModel>();
        public static List<Models.Items.ItemUsableModel> UsablesList = new List<Models.Items.ItemUsableModel>();

        public static void LoadItems()
        {
            var sqlText = "SELECT * FROM items";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var item = new Models.Items.ItemModel()
                {
                    ID = sqlReader.GetInt32("ID"),
                    Pods = sqlReader.GetInt16("Weight"),
                    Price = sqlReader.GetInt32("Price"),
                    Type = sqlReader.GetInt16("Type"),
                    Level = sqlReader.GetInt16("Level"),
                    Jet = sqlReader.GetString("Stats"),
                    Condistr = sqlReader.GetString("Conditions"),
                };

                item.ParseWeaponInfos(sqlReader.GetString("WeaponInfo"));
                item.ParseRandomJet();

                ItemsList.Add(item);
            }

            sqlReader.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' items from the database !", ItemsList.Count));
        }

        public static void LoadItemsSets()
        {
            var sqlText = "SELECT * FROM items_sets";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var set = new Models.Items.SetModel()
                {
                    ID = sqlReader.GetInt16("ID"),
                };

                set.ParseBonus(sqlReader.GetString("bonus"));
                set.ParseItems(sqlReader.GetString("items"));

                SetsList.Add(set);
            }

            sqlReader.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' items sets from the database !", SetsList.Count));
        }

        public static void LoadUsablesItems()
        {
            var sqlText = "SELECT * FROM items_usables";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var item = new Models.Items.ItemUsableModel()
                {
                    Base = sqlReader.GetInt16("ID"),
                    Args = sqlReader.GetString("Args"),
                };

                if (sqlReader.GetInt16("MustDelete") == 1)
                    item.MustDelete = true;
                else
                    item.MustDelete = false;

                item.AttributeItem();

                UsablesList.Add(item);
            }

            sqlReader.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' items usables from the database !", UsablesList.Count));
        }
    }
}
