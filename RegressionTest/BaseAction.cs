using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public abstract class BaseAction: IDisposable
    {
        public string Desc { get; set; }
        public BaseCharacter Owner { get; set; } = null;
 
        public DiceRoller Dice { get; set; } = new DiceRoller();
        
        public List<BaseCharacter> Targets { get; set; } = new List<BaseCharacter>();
        public int MaxTargets { get; set; } = 1;
        
        public int AttackModifier { get; set; } = 0;
        public int Modifier { get; set; } = 0;
        
        public int TotalToRun { get; set; } = 1;
        
        public bool CriticalHit { get; set; } = false;
        public int CriticalThreshold { get; set; } = 20;
        public bool HalfDamageOnMiss { get; set; } = false;

        public AbilityScore Ability { get; set; } = AbilityScore.Wisdom;
        public int DC { get; set; } = 10;

        public enum ActionType
        {
            None,
            Activate,
            Heal,
            GrantTempHp,
            MeleeAttack,
            RangedAttack,
            SpellAttack,
            SpellSave
        }

        public ActionType Type { get; set; } = ActionType.MeleeAttack;
        
        public enum ActionTime
        {
            Action,
            BonusAction,
            Reaction,
            PreTurn,    // Spirit Guardians, etc
            PostTurn    // Legendary Actions, Twilight Sanctuary, etc
        }

        public ActionTime Time { get; set; } = ActionTime.Action;

        public enum DamageAmount
        {
            None,
            Half,
            Full
        }

        public DamageAmount Result { get; set; } = DamageAmount.None;

        public virtual bool Hits(BaseCharacter target)
        {
            CriticalHit = false;
            bool hits = false;
            switch (Type)
            {
                case ActionType.MeleeAttack:
                case ActionType.RangedAttack:
                case ActionType.SpellAttack:
                    hits = AttackType(target);
                    Result = hits ? DamageAmount.Full : DamageAmount.None;
                    break;

                case ActionType.SpellSave:
                    hits = target.SavingThrow(Ability, DC);
                    if (HalfDamageOnMiss)
                    {
                        Result = hits ? DamageAmount.Full : DamageAmount.Half;
                        hits = true;
                    }
                    break;

                case ActionType.Heal:
                    hits = true;
                    Result = DamageAmount.Full;
                    break;

                case ActionType.GrantTempHp:
                    hits = true;
                    Result = DamageAmount.Full;
                    break;

                default:
                    hits = false;
                    Result = DamageAmount.None;
                    break;
            }

            return hits;
        }

        protected virtual bool AttackType(BaseCharacter target)
        {
            CriticalHit = false;
            int roll = Dice.D20();

            if (roll >= CriticalThreshold)
                CriticalHit = true;

            return (roll + AttackModifier) >= target.AC ? true : false;
        }

        public abstract int Amount();

        public string HitDesc()
        {
            if (Type == ActionType.SpellSave)
            {
                return Result == DamageAmount.Full ? "made save" : "failed save";
            }

            if (CriticalHit)
                return "critical hit";

            if (Result == DamageAmount.Full)
                return "hits";

            return "misses";
        }

        public void Dispose()
        {
        }
    }

    public abstract class SpellAction: BaseAction
    {
        public enum SpellLevel
        {
            Cantrip = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9
        }

        public SpellLevel Level { get; set; }

        public SpellLevel DesiredLevel(SpellLevel level)
        {
            if (Level == SpellLevel.Cantrip)
                return SpellLevel.Cantrip;

            if (level <= Level)
                return Level;

            return level;
        }
    }

    public class NoAction : BaseAction
    {
        public NoAction()
        {
            Desc = "(Nothing)";
            Type = ActionType.None;
        }

        public override int Amount() { return 0; }
    }

    public class CureWounds : SpellAction
    {
        public CureWounds()
        {
            Desc = "Cure Wounds";
            Level = SpellLevel.One;
            Type = ActionType.Heal;
            Time = ActionTime.Action;
        }

        public override int Amount()
        {
            int die = 1;

            return Dice.D8(die) + Modifier;
        }
    }

    public class HealingWord : SpellAction
    {
        public HealingWord()
        {
            Desc = "Healing Word";
            Level = SpellLevel.One;
            Type = ActionType.Heal;
            Time = ActionTime.BonusAction;
        }

        public override int Amount()
        {
            int die = 1;

            return Dice.D4(die) + Modifier;
        }
    }

    public class EldritchBlast : SpellAction
    {
        public EldritchBlast()
        {
            Desc = "Eldritch Blast";
            Level = SpellLevel.Cantrip;
            Type = ActionType.SpellAttack;
        }

        public override int Amount()
        {
            return 0;
        }
    }

    public class GuidingBolt : SpellAction
    {
        public GuidingBolt()
        {
            Desc = "Guiding Bolt";
            Level = SpellLevel.One;
            Type = ActionType.SpellAttack;
        }

        public override int Amount()
        {
            int die = 4;

            return Dice.D6(die) + Modifier;
        }
    }
}
