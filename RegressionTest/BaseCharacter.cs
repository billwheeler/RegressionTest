using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
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
    }
}
