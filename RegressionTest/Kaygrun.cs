using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Kaygrun : BaseCharacter
    {
        public class Swashbuckler : BaseAttack
        {
            public bool FirstAttack { get; set; }
            public bool HadSneakAttack { get; set; }

            public Swashbuckler()
            {
                Desc = "Shortsword";
                Modifier = 4;
                Number = 2;
                FirstAttack = true;
                HadSneakAttack = false;
            }

            public override int Damage()
            {
                int damage = Dice.D6() + 4;

                if (!HadSneakAttack)
                {
                    if (Dice.D10() < 10)
                    {
                        damage += (Dice.D6() + Dice.D6());
                        HadSneakAttack = true;
                    }
                }

                return damage;
            }
        }

        public Kaygrun()
        {
            Name = "Kaygrun";
            AC = 15;
            InitMod = 3;
            Health = 28;
            MaxHealth = 28;
            HealingThreshold = 13;
            Group = Team.TeamTwo;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            return new Swashbuckler();
        }
    }
}
