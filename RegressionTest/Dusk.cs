using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class DuskRogue : BaseCharacter
    {
        public class Crossbow : BaseAttack
        {
            public bool HadSneakAttack { get; set; } = false;
            public bool IsPoisoned { get; set; } = false;

            public Crossbow(bool poisoned = false)
            {
                Desc = poisoned ? "Hand Crossbow (POISONED)" : "Hand Crossbow";
                Number = 1;
                Modifier = 7;
                IsPoisoned = poisoned;
            }

            public override int Damage()
            {
                int dmg = Dice.D6(CriticalHit ? 2 : 1);

                if (!HadSneakAttack)
                {
                    if (Dice.D10(1) < 10)
                    {
                        dmg += Dice.D6(CriticalHit ? 8 : 4);
                    }
                }

                if (IsPoisoned)
                {
                    int poisonDamage = Dice.D6(CriticalHit ? 14 : 7);
                    if (Dice.D10(1) < 6)
                    {
                        poisonDamage = (int)Math.Floor((double)(poisonDamage / 2));
                    }

                    dmg += poisonDamage;
                    IsPoisoned = false;
                }

                return dmg + 4;
            }
        }

        public int Poisons { get; set; }

        public DuskRogue()
        {
            Name = "Dusk Rogue";
            AC = 16;
            InitMod = 4;
            Health = 42;
            MaxHealth = 42;
            HealingThreshold = 42;
            Group = Team.TeamTwo;
            Priority = HealPriority.Medium;
            Poisons = 3;
        }

        public override BaseAttack PickAttack()
        {
            bool poisoned = false;
            if (Poisons > 0)
            {
                poisoned = true;
                Poisons--;
            }

            return new Crossbow(poisoned);
        }
    }
}
