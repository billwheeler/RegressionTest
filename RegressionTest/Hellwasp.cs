using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Hellwasp : BaseCharacter
    {
        public class HellwaspAttack : BaseAttack
        {
            public HellwaspAttack()
            {
                Desc = "Sting";
                Number = 2;
                Modifier = 7;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Sword Talons";

                return hits;
            }

            public override int Damage()
            {
                int damage = 0;

                if (CurrentAttack > 1)
                {
                    damage += Dice.D6() + Dice.D6();
                    if (CriticalHit) damage += Dice.D6() + Dice.D6();
                }
                else
                {
                    damage += Dice.D8() + Dice.D6() + Dice.D6();
                    if (CriticalHit) damage += Dice.D8() + Dice.D6() + Dice.D6();
                }

                return damage + 4;
            }
        }

        public Hellwasp()
        {
            Name = "Hellwasp";
            AC = 19;
            InitMod = 2;
            Health = 52;
            MaxHealth = 52;
            Group = Team.TeamTwo;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            return new HellwaspAttack();
        }
    }
}
