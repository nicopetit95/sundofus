using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Utilities
{
    class BooleanByte
    {
        public static int SetFlag(int Number, int Flag, bool Value)
        {
            switch (Flag)
            {
                case 0:
                    if (Value)
                        return Number | 1;
                    else
                        return Number & (255 - 1);

                case 1:
                    if (Value)
                        return Number | 2;
                    else
                        return Number & (255 - 2);

                case 2:
                    if (Value)
                        return Number | 4;
                    else
                        return Number & (255 - 4);

                case 3:
                    if (Value)
                        return Number | 8;
                    else
                        return Number & (255 - 8);
                case 4:
                    if (Value)
                        return Number | 16;
                    else
                        return Number & (255 - 16);

                case 5:
                    if (Value)
                        return Number | 32;
                    else
                        return Number & (255 - 32);

                case 6:
                    if (Value)
                        return Number | 64;
                    else
                        return Number & (255 - 64);

                case 7:
                    if (Value)
                        return Number | 128;
                    else
                        return Number & (255 - 128);
            }

            return -1;
        }

        public static bool GetFlag(int Number, int Flag)
        {
            switch (Flag)
            {
                case 0:
                    return (Number & 1) != 0;

                case 1:
                    return (Number & 2) != 0;

                case 2:
                    return (Number & 4) != 0;

                case 3:
                    return (Number & 8) != 0;

                case 4:
                    return (Number & 16) != 0;

                case 5:
                    return (Number & 32) != 0;

                case 6:
                    return (Number & 64) != 0;

                case 7:
                    return (Number & 128) != 0;
            }

            return false;
        }
    }
}