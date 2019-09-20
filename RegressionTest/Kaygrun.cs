using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Kaygrun : BaseCharacter
    {
        public class Scimitar : BaseAttack
        {
            public bool HadSneakAttack { get; set; } = false;

            public Scimitar()
            {
                Desc = "Scimitar";
                Number = 2;
                Modifier = 4;
            }

            public override int Damage()
            {
                int dmg = Dice.D6();
                if (CriticalHit)
                    dmg += Dice.D6();

                if (!HadSneakAttack)
                {
                    if (Dice.D10() < 9)
                    {
                        dmg += (Dice.D6() + Dice.D6());
                        if (CriticalHit)
                            dmg += (Dice.D6() + Dice.D6());
                    }
                    HadSneakAttack = true;
                }

                return dmg + 4;
            }
        }

        public Kaygrun()
        {
            Name = "Kaygrun";
            AC = 15;
            InitMod = 3;
            Health = 39;
            MaxHealth = 39;
            HealingThreshold = 17;
            Group = Team.TeamTwo;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            return new Scimitar();
        }
    }
}
