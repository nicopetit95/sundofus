using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SunDofus.World.Maps.Zaapis;

namespace SunDofus.Entities.Requests
{
    class ZaapisRequests
    {
        public static List<Zaapis> ZaapisList = new List<Zaapis>();

        public static void LoadZaapis()
        {
            var sqlText = "SELECT * FROM zaapis";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var zaapis = new Zaapis()
                {
                    MapID = sqlReader.GetInt32("mapid"),
                    CellID = sqlReader.GetInt32("cellid"),
                    Faction = sqlReader.GetInt32("zone"),
                };

                if (ParseZaapis(zaapis))
                    ZaapisList.Add(zaapis);
            }

            sqlReader.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' zaapis from the database !", ZaapisList.Count));
        }

        private static bool ParseZaapis(Zaapis zaapis)
        {
            if (MapsRequests.MapsList.Any(x => x.Model.ID == zaapis.MapID) && !ZaapisList.Any(x => x.MapID == zaapis.MapID))
            {
                zaapis.Map = MapsRequests.MapsList.First(x => x.Model.ID == zaapis.MapID);
                return true;
            }
            else
                return false;
        }
    }
}
