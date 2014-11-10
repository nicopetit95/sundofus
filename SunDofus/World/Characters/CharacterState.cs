using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Characters
{
    class CharacterState
    {
        Character Character { get; set; }

        public CharacterState(Character character)
        {
            Character = character;
            Created = false;

            Party = null;
            Followers = new List<Character>();
        }

        public bool Created { get; set; }

        public bool OnMove { get; set; }
        public bool OnExchange { get; set; }
        public bool OnExchangePanel { get; set; }
        public bool OnExchangeAccepted { get; set; }
        public bool OnExchangeWithBank { get; set; }

        public int MoveToCell { get; set; }
        public int ActualNPC { get; set; }
        public int CurrentPlayerTrade { get; set; }

        public long SitStartTime { get; set; }
        public bool IsSitted { get; set; }

        public bool InFight { get; set; }
        public bool IsSpectator { get; set; }

        public bool OnWaitingParty { get; set; }
        public int SenderInviteParty { get; set; }
        public int ReceiverInviteParty{ get; set; }

        public bool OnWaitingGuild { get; set; }
        public int SenderInviteGuild { get; set; }
        public int ReceiverInviteGuild { get; set; }

        public bool IsFollow { get; set; }
        public bool IsFollowing { get; set; }
        public int FollowingID { get; set; }

        public bool OnDialoging { get; set; }
        public int OnDialogingWith { get; set; }

        public bool IsChallengeAsked { get; set; }
        public bool IsChallengeAsker { get; set; }
        public int ChallengeAsked { get; set; }
        public int ChallengeAsker { get; set; }

        public CharacterParty Party { get; set; }
        public List<Character> Followers { get; set; }

        public bool Busy
        {
            get
            {
                return (OnMove || OnExchange || OnWaitingParty || OnDialoging || IsChallengeAsked || IsChallengeAsker || OnExchangeWithBank || InFight || IsSpectator);
            }
        }
    }
}
