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
        public bool Alive { get; set; } = true;
        public bool Healer { get; set; } = false;
        public Team Group { get; set; }
        public HealPriority Priority { get; set; } = HealPriority.Dont;
        public Saves Scores { get; set; } = new Saves();

        public DiceRoller Dice { get; set; } = new DiceRoller();
        public CharacterStats Stats { get; set; } = new CharacterStats();

        public abstract BaseAttack PickAttack();

        public virtual void Init()
        {
            Initiative = 0;
            Health = MaxHealth;
            Alive = true;
        }

        public void RollInitiative()
        {
            Init();
            Initiative = Dice.D20() + InitMod;
            Stats.Encounters++;
        }

        public bool NeedsHealing
        {
            get
            {
                if (Alive == false)
                    return false;

                if (Priority == HealPriority.Dont)
                    return false;

                return Health < HealingThreshold;
            }
        }

        public bool TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                Health = 0;
                Alive = false;
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

        public virtual int HealAmount(HealPriority priority)
        {
            return 0;
        }

        public virtual bool Saves(SavingThrow save)
        {
            switch (save.Attribute)
            {
                case AbilityScores.Strength:
                    return Dice.D20() + Scores.Strength >= save.Threshold;
                case AbilityScores.Dexterity:
                    return Dice.D20() + Scores.Dexterity >= save.Threshold;
                case AbilityScores.Constitution:
                    return Dice.D20() + Scores.Constitution >= save.Threshold;
                case AbilityScores.Intelligence:
                    return Dice.D20() + Scores.Intelligence >= save.Threshold;
                case AbilityScores.Wisdom:
                    return Dice.D20() + Scores.Wisdom >= save.Threshold;
                case AbilityScores.Charisma:
                    return Dice.D20() + Scores.Charisma >= save.Threshold;
            }

            return true;
        }
    }
}
