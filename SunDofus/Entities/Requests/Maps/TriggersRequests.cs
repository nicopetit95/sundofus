using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SunDofus.Entities.Requests
{
    class TriggersRequests
    {
        public static List<Entities.Models.Maps.TriggerModel> TriggersList = new List<Entities.Models.Maps.TriggerModel>();

        public static void LoadTriggers()
        {
            var sqlText = "SELECT * FROM triggers";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var trigger = new Entities.Models.Maps.TriggerModel()
                {
                    MapID = sqlReader.GetInt32("MapID"),
                    CellID = sqlReader.GetInt32("CellID"),
                    ActionID = sqlReader.GetInt16("ActionID"),
                    Args = sqlReader.GetString("Args"),
                    Conditions = sqlReader.GetString("Conditions"),
                };

                if (ParseTrigger(trigger))
                    TriggersList.Add(trigger);
            }

            sqlReader.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' triggers from the database !", TriggersList.Count));
        }

        public static bool ParseTrigger(Entities.Models.Maps.TriggerModel trigger)
        {
            if (MapsRequests.MapsList.Any(x => x.Model.ID == trigger.MapID))
            {
                MapsRequests.MapsList.First(x => x.Model.ID == trigger.MapID).Triggers.Add(trigger);
                return true;
            }
            else
                return false;
        }
    }
}
