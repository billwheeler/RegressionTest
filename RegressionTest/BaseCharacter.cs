using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public abstract class BaseCharacter
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int AC { get; set; }
        public int InitMod { get; set; }
        public int Initiative { get; set; }
        public int HealingThreshold { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public DiceRoller Dice { get; set; }
        public bool Alive { get; set; }
        public Team Group { get; set; }

        public int Attacks = 0;
        public int Hits = 0;
        public int DamageGiven = 0;
        public int DamageTaken = 0;

        public abstract BaseAttack PickAttack();

        public bool NeedsHealing()
        {
            return Health < HealingThreshold;
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
    }
}
