using SunDofus.World.Characters.Stats;
using SunDofus.World.Maps.Fights.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Fights
{
    public enum BuffActiveType
    {
        ACTIVE_STATS,
        ACTIVE_BEGINTURN,
        ACTIVE_ENDTURN,
        ACTIVE_ENDMOVE,
        ACTIVE_ATTACK_POST_JET,
        ACTIVE_ATTACK_AFTER_JET,
        ACTIVE_ATTACKED_POST_JET,
        ACTIVE_ATTACKED_AFTER_JET,
    }

    public enum BuffDecrementType
    {
        TYPE_BEGINTURN,
        TYPE_ENDTURN,
        TYPE_ENDMOVE,
    }

    class FighterBuff
    {
        private Dictionary<BuffActiveType, List<EffectCast>> myBuffActives = new Dictionary<BuffActiveType, List<EffectCast>>()
        {
            { BuffActiveType.ACTIVE_ATTACKED_AFTER_JET, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_ATTACKED_POST_JET, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_ATTACK_AFTER_JET, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_ATTACK_POST_JET, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_BEGINTURN, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_ENDTURN, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_ENDMOVE, new List<EffectCast>() },
            { BuffActiveType.ACTIVE_STATS, new List<EffectCast>() }
        };

        private Dictionary<BuffDecrementType, List<EffectCast>> myBuffDecrements = new Dictionary<BuffDecrementType, List<EffectCast>>()
        {
            { BuffDecrementType.TYPE_BEGINTURN, new List<EffectCast>() },
            { BuffDecrementType.TYPE_ENDTURN, new List<EffectCast>() },
            { BuffDecrementType.TYPE_ENDMOVE, new List<EffectCast>() }
        };

        public void AddBuff(EffectCast buff, BuffActiveType active, BuffDecrementType decrement)
        {
            buff = buff.CopyToBuff();

            if (buff.Target == buff.Target.Fight.CurrentFighter)
                buff.Duration += 1;

            myBuffActives[active].Add(buff);
            myBuffDecrements[decrement].Add(buff);

            switch (buff.Type)
            {
                case EffectEnum.ChanceEcaflip:
                    buff.Target.Fight.Send("GIE" + (int)buff.Type + ';' + buff.Target.ID + ';' + buff.Value1 + ';' + buff.Value2 + ';' + buff.Value3 + ';' + buff.Chance + ';' + buff.Duration + ';' + buff.SpellID);
                    break;

                default:
                    buff.Target.Fight.Send("GIE" + (int)buff.Type + ';' + buff.Target.ID + ';' + buff.Value1 + ';' + (buff.Value2 == -1 ? "" : buff.Value2.ToString()) + ";;" + buff.Chance + ';' + buff.Duration + ';' + buff.SpellID);
                    break;
            }
        }

        public bool HasBuff(EffectEnum type, BuffActiveType active)
        {
            return myBuffActives[active].Any(x => x.Type == type);
        }

        public void OnTurnBegin()
        {
            foreach (EffectCast buff in myBuffActives[BuffActiveType.ACTIVE_BEGINTURN])
                BuffProcessor.ApplyBuff(buff);

            foreach (EffectCast buff in myBuffDecrements[BuffDecrementType.TYPE_BEGINTURN])
            {
                buff.Duration -= 1;

                if (buff.Duration <= 0)
                {
                    BuffProcessor.RemoveBuff(buff);

                    foreach (BuffActiveType active in myBuffActives.Keys)
                    {
                        if (myBuffActives[active].Contains(buff))
                        {
                            myBuffActives[active].Remove(buff);
                            break;
                        }
                    }
                }
            }

            myBuffDecrements[BuffDecrementType.TYPE_BEGINTURN].RemoveAll(x => x.Duration <= 0);
        }

        public void OnTurnEnd()
        {
            foreach (EffectCast buff in myBuffActives[BuffActiveType.ACTIVE_ENDTURN])
                BuffProcessor.ApplyBuff(buff);

            foreach (EffectCast buff in myBuffDecrements[BuffDecrementType.TYPE_ENDTURN])
            {
                buff.Duration -= 1;

                if (buff.Duration <= 0)
                {
                    BuffProcessor.RemoveBuff(buff);

                    foreach (BuffActiveType active in myBuffActives.Keys)
                    {
                        if (myBuffActives[active].Contains(buff))
                        {
                            myBuffActives[active].Remove(buff);
                            break;
                        }
                    }
                }
            }

            myBuffDecrements[BuffDecrementType.TYPE_ENDTURN].RemoveAll(x => x.Duration <= 0);
        }

        public void OnMoveEnd()
        {
            foreach (EffectCast buff in myBuffActives[BuffActiveType.ACTIVE_ENDMOVE])
                BuffProcessor.ApplyBuff(buff);

            foreach (EffectCast buff in myBuffDecrements[BuffDecrementType.TYPE_ENDMOVE])
            {
                buff.Duration -= 1;

                if (buff.Duration <= 0)
                {
                    BuffProcessor.RemoveBuff(buff);

                    foreach (BuffActiveType active in myBuffActives.Keys)
                    {
                        if (myBuffActives[active].Contains(buff))
                        {
                            myBuffActives[active].Remove(buff);
                            return;
                        }
                    }
                }
            }

            myBuffDecrements[BuffDecrementType.TYPE_ENDMOVE].RemoveAll(x => x.Duration <= 0);
        }

        public void OnAttackPostJet(EffectCast cast)
        {
            foreach (EffectCast buff in myBuffActives[BuffActiveType.ACTIVE_ATTACK_POST_JET])
                BuffProcessor.ApplyBuff(buff, cast);
        }

        public void OnAttackAfterJet(EffectCast cast)
        {
            foreach (EffectCast buff in myBuffActives[BuffActiveType.ACTIVE_ATTACK_AFTER_JET])
                BuffProcessor.ApplyBuff(buff, cast);
        }

        public void OnAttackedPostJet(EffectCast cast)
        {
            foreach (EffectCast buff in myBuffActives[BuffActiveType.ACTIVE_ATTACKED_POST_JET])
                BuffProcessor.ApplyBuff(buff, cast);
        }

        public void OnAttackedAfterJet(EffectCast cast)
        {
            foreach (EffectCast buff in myBuffActives[BuffActiveType.ACTIVE_ATTACKED_AFTER_JET])
                BuffProcessor.ApplyBuff(buff, cast);
        }

        public void Debuff()
        {
            foreach (BuffDecrementType decrement in myBuffDecrements.Keys)
            {
                foreach (EffectCast buff in myBuffDecrements[decrement])
                    if (IsDebuffable(buff))
                        BuffProcessor.RemoveBuff(buff);

                myBuffDecrements[decrement].RemoveAll(x => IsDebuffable(x));
            }

            foreach (BuffActiveType active in myBuffActives.Keys)
                myBuffActives[active].RemoveAll(x => IsDebuffable(x));
        }

        private bool IsDebuffable(EffectCast cast)
        {
            switch (cast.Type)
            {
                case EffectEnum.AddPA:
                case EffectEnum.AddPABis:
                case EffectEnum.AddPM:
                case EffectEnum.SubPA:
                case EffectEnum.SubPAEsquive:
                    return false;
            }

            return true;
        }
    }
}
