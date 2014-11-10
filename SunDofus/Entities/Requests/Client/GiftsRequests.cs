using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Timers;
using SunDofus.Entities.Models.Clients;

namespace SunDofus.Entities.Requests
{
    class GiftsRequests
    {
        public static List<GiftModel> GetGiftsByAccountID(int accID)
        {
            var list = new List<GiftModel>();

            var sqlText = "SELECT * FROM gifts WHERE Target=@target";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            sqlCommand.Parameters.Add(new MySqlParameter("@target", accID));

            var sqlReader = sqlCommand.ExecuteReader();

            while (sqlReader.Read())
            {
                var gift = new GiftModel()
                {
                    ID = sqlReader.GetInt16("Id"),
                    Target = sqlReader.GetInt16("Target"),
                    ItemID = sqlReader.GetInt16("ItemID"),
                    Title = sqlReader.GetString("Title"),
                    Message = sqlReader.GetString("Message"),
                    Image = sqlReader.GetString("Image"),
                };

                list.Add(gift);
            }

            sqlReader.Close();

            return list;
        }

        public static void DeleteGift(int giftID, int accountID)
        {
            if (accountID == -1)
                return;

            var sqlText = "DELETE FROM gifts WHERE id=@id";
            var sqlCommand = new MySqlCommand(sqlText, Program.DBHelper.Use());

            sqlCommand.Parameters.Add(new MySqlParameter("@id", giftID));

            sqlCommand.ExecuteNonQuery();
        }
    }
}
