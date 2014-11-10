using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Entities.Models.Clients
{
    class AccountModel
    {
        public int ID { get; set; }
        public int Level { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Pseudo { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }

        public Dictionary<int, List<string>> Characters { get; set; }
        public List<string> CharactersNames { get; set; }

        public string Strgifts { get; set; }
        public List<GiftModel> Gifts { get; set; }

        public string StrFriends { get; set; }
        public List<string> Friends { get; set; }

        public string StrEnemies { get; set; }
        public List<string> Enemies { get; set; }

        public DateTime SubscriptionDate { get; set; }
        public long Subscription { get; set; }

        public AccountModel()
        {
            Strgifts = "";
            StrFriends = "";
            StrEnemies = "";

            CharactersNames = new List<string>();

            Characters = new Dictionary<int, List<string>>();
            Gifts = new List<GiftModel>();

            Enemies = new List<string>();
            Friends = new List<string>();
        }

        public long SubscriptionTime()
        {
            var time = SubscriptionDate.Subtract(DateTime.Now).TotalMilliseconds;

            if (SubscriptionDate.Subtract(DateTime.Now).TotalMilliseconds <= 1)
                return 0;

            return (long)time;
        }

        public void ParseCharacters()
        {
            if (Characters[Program.Config.GameID].Count < 1) 
                return;

            foreach (var datas in Characters[Program.Config.GameID])
            {
                lock (Characters)
                {
                    if (!CharactersNames.Contains(datas))
                        CharactersNames.Add(datas);
                }
            }
        }

        public void ParseGifts()
        {
            if (Strgifts == "") 
                return;

            foreach (var datas in Strgifts.Split('+'))
            {
                var giftDatas = datas.Split('~');
                var gift = new GiftModel();

                gift.ID = int.Parse(giftDatas[0]);
                gift.Title = giftDatas[1];
                gift.Message = giftDatas[2];
                gift.ItemID = int.Parse(giftDatas[3]);
                gift.Image = giftDatas[4];

                Gifts.Add(gift);
            }
        }
    }
}
