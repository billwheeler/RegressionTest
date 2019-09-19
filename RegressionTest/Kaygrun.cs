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
            public Scimitar()
            {
                Desc = "Scimitar";
                Number = 2;
                Modifier = 4;
            }

            public override int Damage()
            {
                int damage = Dice.D6() + 4;
                if (CriticalHit)
                    damage += Dice.D6();

                return damage;
            }
        }

        public Kaygrun()
        {
            Name = "Kaygrun";
            AC = 15;
            InitMod = 3;
            Health = 26;
            MaxHealth = 26;
            HealingThreshold = 14;
            Group = Team.TeamTwo;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            return new Scimitar();
        }
    }
}
