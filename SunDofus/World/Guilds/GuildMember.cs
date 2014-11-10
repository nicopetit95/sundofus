using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Guilds
{
    class GuildMember
    {
        public int Rank { get; set; }
        public int ExpGaved { get; set; }
        public int ExpGived { get; set; }
        public int Rights { get; set; }

        public Characters.Character Character { get; set; }

        public GuildMember(Characters.Character character)
        {
            Character = character;
        }

        public bool IsBoss
        {
            get
            {
                return (this.Rights & 1) == 1;
            }
        }

        public bool CanManageBoost
        {
            get
            {
                return IsBoss || (this.Rights & 2) == 2;
            }
        }

        public bool CanManageRights
        {
            get
            {
                return IsBoss || (this.Rights & 4) == 4;
            }
        }

        public bool CanInvite
        {
            get
            {
                return IsBoss || (this.Rights & 8) == 8;
            }
        }

        public bool CanBann
        {
            get
            {
                return IsBoss || (this.Rights & 16) == 16;
            }
        }

        public bool CanManageXPContitribution
        {
            get
            {
                return IsBoss || (this.Rights & 32) == 32;
            }
        }

        public bool CanManageRanks
        {
            get
            {
                return IsBoss || (this.Rights & 64) == 64;
            }
        }

        public bool CanHireTaxCollector
        {
            get
            {
                return IsBoss || (this.Rights & 128) == 128;
            }
        }

        public bool CanManageOwnXPContitribution
        {
            get
            {
                return IsBoss || (this.Rights & 256) == 256;
            }
        }

        public bool CanCollect
        {
            get
            {
                return IsBoss || (this.Rights & 512) == 512;
            }
        }

        public bool CanUseMountPark
        {
            get
            {
                return IsBoss || (this.Rights & 4096) == 4096;
            }
        }

        public bool CanArrangeMountPark
        {
            get
            {
                return IsBoss || (this.Rights & 8192) == 8192;
            }
        }

        public bool CanManageOtherMount
        {
            get
            {
                return IsBoss || (this.Rights & 16384) == 16384;
            }
        }

        public string SqlToString()
        {
            return string.Format("{0};{1};{2};{3};{4}", Character.ID, Rank, ExpGaved, ExpGived, Rights);
        }
    }
}
