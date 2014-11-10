using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SunDofus.World.Guilds;

namespace SunDofus.Entities.Requests
{
    class GuildsRequest
    {
        public static ConcurrentBag<Guild> GuildsList = new ConcurrentBag<Guild>();

        public static void LoadGuilds()
        {
            var sqlText = "SELECT * FROM guilds";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlResult = sqlCommand.ExecuteReader();

            while (sqlResult.Read())
            {
                var guild = new Guild
                {
                    ID = sqlResult.GetInt32("ID"),
                    Name = sqlResult.GetString("Name"),
                    Level = sqlResult.GetInt32("Level"),
                    Exp = sqlResult.GetInt64("Exp"),

                    IsNewGuild = false,
                };

                guild.ParseEmblem(sqlResult.GetString("Emblem"));
                guild.ParseSqlMembers(sqlResult.GetString("Members"));
                guild.ParseSqlStats(sqlResult.GetString("Stats"));

                GuildsList.Add(guild);
            }

            sqlResult.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' guilds from the database !", GuildsList.Count));
        }

        public static void SaveGuild(Guild guild)
        {
            if (guild.IsNewGuild && !guild.MustDelete)
            {
                CreateGuild(guild);
                return;
            }
            else if (guild.MustDelete)
            {
                DeleteGuild(guild.ID);
                return;
            }
            else if (!guild.MustDelete && !guild.IsNewGuild)
            {
                var sqlText = "UPDATE guilds SET ID=@ID, Name=@Name, Level=@Level, Exp=@Exp, Stats=@Stats," +
                    " Members=@Members, Emblem=@Emblem WHERE ID=@ID";
                var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

                var P = sqlCommand.Parameters;
                P.Add(new MySqlParameter("@ID", guild.ID));
                P.Add(new MySqlParameter("@Name", guild.Name));
                P.Add(new MySqlParameter("@Level", guild.Level));
                P.Add(new MySqlParameter("@Exp", guild.Exp));
                P.Add(new MySqlParameter("@Stats", guild.GetSqlStats()));
                P.Add(new MySqlParameter("@Members", guild.GetSqlMembers()));
                P.Add(new MySqlParameter("@Emblem", guild.GetSqlEmblem()));

                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void DeleteGuild(int guildID)
        {
            var sqlText = "DELETE FROM guilds WHERE ID=@ID";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            sqlCommand.Parameters.Add(new MySqlParameter("@ID", guildID));

            sqlCommand.ExecuteNonQuery();
        }

        private static void CreateGuild(Guild guild)
        {
            var sqlText = "INSERT INTO guilds VALUES(@ID, @Name, @Emblem, @Level, @Exp, @Members, @Stats)";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var P = sqlCommand.Parameters;

            P.Add(new MySqlParameter("@ID", guild.ID));
            P.Add(new MySqlParameter("@Name", guild.Name));
            P.Add(new MySqlParameter("@Level", guild.Level));
            P.Add(new MySqlParameter("@Exp", guild.Exp));
            P.Add(new MySqlParameter("@Stats", guild.GetSqlStats()));
            P.Add(new MySqlParameter("@Members", guild.GetSqlMembers()));
            P.Add(new MySqlParameter("@Emblem", guild.GetSqlEmblem()));

            sqlCommand.ExecuteNonQuery();

            guild.IsNewGuild = false;
        }
    }
}
