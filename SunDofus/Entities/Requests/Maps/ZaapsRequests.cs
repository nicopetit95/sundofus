using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SunDofus.World.Maps.Zaaps;

namespace SunDofus.Entities.Requests
{
    class ZaapsRequests
    {
        public static List<Zaap> ZaapsList = new List<Zaap>();

        public static void LoadZaaps()
        {
            var sqlText = "SELECT * FROM zaaps";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var zaap = new Zaap()
                {
                    MapID = sqlReader.GetInt32("mapID"),
                    CellID = sqlReader.GetInt32("cellID"),
                };

                if (ParseZaap(zaap))
                    ZaapsList.Add(zaap);
            }

            sqlReader.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' zaaps from the database !", ZaapsList.Count));
        }

        private static bool ParseZaap(Zaap zaap)
        {
            if (MapsRequests.MapsList.Any(x => x.Model.ID == zaap.MapID) && !ZaapsList.Any(x => x.MapID == zaap.MapID))
            {
                zaap.Map = MapsRequests.MapsList.First(x => x.Model.ID == zaap.MapID);
                return true;
            }
            else
                return false;
        }
    }
}
