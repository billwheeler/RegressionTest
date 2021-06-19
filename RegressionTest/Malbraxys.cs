using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Malbraxys : BaseCharacter
    {


        public class ShortswordFlury : BaseAttack
        {
            public ShortswordFlury()
            {
                Desc = "Shortsword";
                Number = 4;
                Modifier = 11;
            }

            public override bool Hits(BaseCharacter target)
            {
                if (CurrentAttack > 2)
                    Modifier = 8;

                bool hits = base.Hits(target);
                if (CurrentAttack > 2)
                    Desc = "Unarmed Strike";

                return hits;
            }

            public override int Damage()
            {
                if (CurrentAttack < 3)
                {
                    if (CriticalHit)
                        return Dice.D6() + Dice.D6() + 7;
                    return Dice.D6() + 7;
                }
                else
                {
                    if (CriticalHit)
                        return Dice.D6() + Dice.D6() + 4;
                    return Dice.D6() + 4;

                }
            }
        }

        public Malbraxys()
        {
            Name = "Malbraxys";
            AC = 20;
            InitMod = 4;
            Health = 77;
            MaxHealth = 77;
            HealingThreshold = 33;
            Group = Team.TeamOne;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            return new ShortswordFlury();
        }
    }
}
