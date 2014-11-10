using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SunDofus.World.Bank;

namespace SunDofus.Entities.Requests
{
    class BanksRequests
    {
        public static ConcurrentBag<Bank> BanksList = new ConcurrentBag<Bank>();

        public static void LoadBanks()
        {
            var sqlText = "SELECT * FROM banks";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var sqlResult = sqlCommand.ExecuteReader();

            while (sqlResult.Read())
            {
                var bank = new Bank()
                {
                    Owner = sqlResult.GetInt32("Owner"),
                    Kamas = sqlResult.GetInt64("Kamas"),
                    IsNewBank = false
                };

                bank.ParseItems(sqlResult.GetString("Items"));

                BanksList.Add(bank);
            }

            sqlResult.Close();

            Utilities.Loggers.Status.Write(string.Format("Loaded '{0}' banks from the database !", BanksList.Count));
        }

        public static void SaveBank(Bank bank)
        {
            if (bank.IsNewBank)
            {
                CreateBank(bank);
                return;
            }
            else
            {
                var sqlText = "UPDATE banks SET Owner=@Owner, Kamas=@Kamas, Items=@Items WHERE Owner=@Owner";
                var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

                var P = sqlCommand.Parameters;

                P.Add(new MySqlParameter("@Owner", bank.Owner));
                P.Add(new MySqlParameter("@Kamas", bank.Kamas));
                P.Add(new MySqlParameter("@Items", bank.GetItems()));

                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void CreateBank(Bank bank)
        {
            var sqlText = "INSERT INTO banks VALUES(@Owner, @Kamas, @Items)";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            var P = sqlCommand.Parameters;

            P.Add(new MySqlParameter("@Owner", bank.Owner));
            P.Add(new MySqlParameter("@Kamas", bank.Kamas));
            P.Add(new MySqlParameter("@Items", bank.GetItems()));

            sqlCommand.ExecuteNonQuery();

            bank.IsNewBank = false;
        }
    }
}
