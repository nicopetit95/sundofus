using SunDofus.Entities.Models.Items;
using SunDofus.World.Characters.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Characters.Items
{
    class ItemsHandler
    {
        private static int lastID = 0;

        public static int GetNewID()
        {
            return ++lastID;
        }

        public static bool PositionAvaliable(int itemType, bool usable, int position)
        {

            return true;

        }

        public static bool ConditionsAvaliable(ItemModel item, Character character)
        {
            var condi = item.Condistr;
            var avaliable = false;

            if (condi == "")
                return true;

            foreach (var cond in condi.Split('&'))
            {
                var spliter = cond.Substring(2, 1);
                long value = -1;
                var toCompare = int.Parse(cond.Substring(3));

                switch (cond.Substring(0, 1))
                {
                    case "C":

                        switch (cond.Substring(1, 1))
                        {
                            case "a":
                                value = character.Stats.GetStat(StatEnum.Agilite).Base;
                                break;

                            case "i":
                                value = character.Stats.GetStat(StatEnum.Intelligence).Base;
                                break;

                            case "c":
                                value = character.Stats.GetStat(StatEnum.Chance).Base;
                                break;

                            case "s":
                                value = character.Stats.GetStat(StatEnum.Force).Base;
                                break;

                            case "v":
                                value = character.Stats.GetStat(StatEnum.Vitalite).Base;
                                break;

                            case "w":
                                value = character.Stats.GetStat(StatEnum.Sagesse).Base;
                                break;

                            case "A":
                                value = character.Stats.GetStat(StatEnum.Agilite).Total;
                                break;

                            case "I":
                                value = character.Stats.GetStat(StatEnum.Intelligence).Total;
                                break;

                            case "C":
                                value = character.Stats.GetStat(StatEnum.Chance).Total;
                                break;

                            case "S":
                                value = character.Stats.GetStat(StatEnum.Force).Total;
                                break;

                            case "V":
                                value = character.Stats.GetStat(StatEnum.Vitalite).Total;
                                break;

                            case "W":
                                value = character.Stats.GetStat(StatEnum.Sagesse).Total;
                                break;

                            default:
                                avaliable = true;
                                break;
                        }

                        break;

                    case "P":

                        switch (cond.Substring(1, 1))
                        {
                            case "G":
                                value = character.Class;
                                break;

                            case "L":
                                value = character.Level;
                                break;

                            case "K":
                                value = character.Kamas;
                                break;

                            default:
                                avaliable = true;
                                break;
                        }

                        break;

                    default:
                        avaliable = true;
                        break;
                }

                if (avaliable == true)
                    return true;

                if (spliter != "")
                {
                    switch (spliter)
                    {
                        case "<":
                            avaliable = (value < toCompare ? true : false);
                            break;

                        case ">":
                            avaliable = (value > toCompare ? true : false);
                            break;

                        case "=":
                            avaliable = (value == toCompare ? true : false);
                            break;

                        case "~":
                            avaliable = (value == toCompare ? true : false);
                            break;

                        case "!":
                            avaliable = (value != toCompare ? true : false);
                            break;

                        default:
                            avaliable = true;
                            break;
                    }

                    if (avaliable == false)
                        return false;
                }
            }

            return avaliable;
        }
    }
}
