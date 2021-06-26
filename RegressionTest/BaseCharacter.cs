using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Saves
    {
        public int Strength { get; set; } = 0;
        public int Dexterity { get; set; } = 0;
        public int Constitution { get; set; } = 0;
        public int Intelligence { get; set; } = 0;
        public int Wisdom { get; set; } = 0;
        public int Charisma { get; set; } = 0;
    }

    public enum AbilityScore
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma
    }

    public class Stat
    {
        public int Score { get; set; } = 10;
        public int Mod { get; set; } = 0;
        public int Save { get; set; } = 0;
    }

    public abstract class BaseCharacter
    {
        public int ID { get; set; } = 0;
        public string Name { get; set; }
        public int AC { get; set; } = 10;
        public int InitMod { get; set; } = 0;
        public int Initiative { get; set; } = 0;
        public int HealingThreshold { get; set; } = 0;
        public int Health { get; set; } = 5;
        public int MaxHealth { get; set; } = 5;
        public int TempHitPoints { get; set; } = 0;
        public bool Alive { get; set; } = true;
        public bool Healer { get; set; } = false;
        public BaseCharacter HealTarget { get; set; } = null;
        public Team Group { get; set; }
        public HealPriority Priority { get; set; } = HealPriority.Dont;
        public bool Concentrating { get; set; } = false;
        
        public Saves Scores { get; set; } = new Saves();

        public DiceRoller Dice { get; set; } = new DiceRoller();
        public CharacterStats Stats { get; set; } = new CharacterStats();

        public bool PreTurnNotify { get; set; } = false;
        public bool PostTurnNotify { get; set; } = false;

        public Dictionary<AbilityScore, Stat> Abilities = new Dictionary<AbilityScore, Stat>();

        public bool WarCaster { get; set; } = false;
        public bool HasAdvantageOnInitiative { get; set; } = false;
        public bool BonusActionFirst { get; set; } = false;

        public bool HasBless { get; set; } = false;

        public Encounter Context { get; set; } = null;

        public virtual BaseAttack PickAttack()
        {
            return null;
        }

        public virtual void Init()
        {
            Initiative = 0;
            Health = MaxHealth;
            Alive = true;
            TempHitPoints = 0;
            HealTarget = null;
            HasAdvantageOnInitiative = false;
        }

        public void RollInitiative()
        {
            Init();
            Initiative = Dice.MakeAbilityRoll(HasAdvantageOnInitiative ? AbilityRoll.Advantage : AbilityRoll.Normal) + InitMod;
            Stats.Encounters++;
        }

        public bool NeedsHealing
        {
            get
            {
                if (Priority == HealPriority.Dont)
                    return false;

                return Health < HealingThreshold;
            }
        }

        public bool SavingThrow(AbilityScore score, int dc, AbilityRoll rollType = AbilityRoll.Normal)
        {
            if (Abilities.ContainsKey(score))
            {
                int roll = Dice.MakeAbilityRoll(rollType) + Abilities[score].Save;
                if (HasBless)
                    roll += Dice.D4();

                return roll >= dc;
            }

            return false;
        }

        public bool ConcentrationCheck(int amount)
        {
            if (Alive == false)
                return false;

            if (!Concentrating)
                return true;

            int dc = (int)Math.Floor(amount / 2.0f);
            if (dc < 10) dc = 10;

            bool result = SavingThrow(AbilityScore.Constitution, dc, WarCaster ? AbilityRoll.Advantage : AbilityRoll.Normal);
            if (!result)
            {
                OnFailConcentration();
            }

            return result;
        }

        public bool TakeDamage(int amount)
        {
            if (TempHitPoints > 0)
            {
                if (amount >= TempHitPoints)
                {
                    amount -= TempHitPoints;
                    TempHitPoints = 0;
                }
                else
                {
                    TempHitPoints -= amount;
                    return true;
                }
            }

            Health -= amount;
            if (Health <= 0)
            {
                Health = 0;
                Alive = false;
                OnDeath();
                return false;
            }

            return true;
        }

        public void Heal(int amount)
        {
            Alive = true;
            Health += amount;
            if (Health > MaxHealth)
                Health = MaxHealth;
        }

        public void SetTempHitPoints(int amount)
        {
            if (!Alive) return;
            TempHitPoints = amount;
        }

        public string GetHealthDesc()
        {
            if (Alive)
            {
                if (TempHitPoints > 0)
                    return string.Format("{0}/{1}hp", Health, TempHitPoints);

                return string.Format("{0}hp", Health);
            }

            return "dead";
        }

        public virtual int HealAmount(HealPriority priority)
        {
            return 0;
        }

        public virtual void OnNewRound()
        {
        }

        public virtual void OnNewTurn()
        {
        }

        public virtual void OnEndTurn()
        {
        }

        public virtual void OnFailConcentration()
        {
            Concentrating = false;
        }

        public virtual void OnDeath()
        {
            Concentrating = false;
        }

        public virtual void OnBeforeHitCalc(int roll)
        {
        }

        public virtual BaseAction PickAction()
        {
            return new NoAction { Time = BaseAction.ActionTime.Action };
        }

        public virtual BaseAction PickBonusAction()
        {
            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public virtual BaseAction PickReaction()
        {
            return new NoAction { Time = BaseAction.ActionTime.Reaction };
        }

        public virtual BaseAction PickPreTurn()
        {
            return new NoAction { Time = BaseAction.ActionTime.PreTurn };
        }

        public virtual BaseAction PickPostTurn()
        {
            return new NoAction { Time = BaseAction.ActionTime.PostTurn };
        }
    }
}
