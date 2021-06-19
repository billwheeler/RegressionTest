using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Behir : BaseCharacter
    {
        public class Bite : WeaponAttack
        {
            public Bite()
            {
                Desc = "Bite";
                Modifier = 10;
            }

            public override int Damage()
            {
                int damage = Dice.D10() + Dice.D10() + Dice.D10();

                if (CriticalHit)
                    damage += Dice.D10() + Dice.D10() + Dice.D10();

                return (damage) + 6;
            }
        }

        public class BiteAndConstrict : WeaponAttack
        {
            public BiteAndConstrict()
            {
                Desc = "Bite";
                Modifier = 10;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Constrict";

                return hits;
            }

            public override int Damage()
            {
                int damage;

                if (CurrentAttack > 1)
                {
                    damage = Dice.D10() + Dice.D10() + Dice.D10() + Dice.D10() + 6;
                    if (CriticalHit)
                        damage += Dice.D10() + Dice.D10() + Dice.D10() + Dice.D10();
                }
                else
                {
                    damage = Dice.D10() + Dice.D10() + Dice.D10();
                    if (CriticalHit)
                        damage += Dice.D10() + Dice.D10() + Dice.D10();
                }

                return (damage) + 6;
            }
        }

        public Behir()
        {
            Name = "Behir";
            AC = 17;
            InitMod = 3;
            Health = 168;
            MaxHealth = 168;
            Group = Team.TeamTwo;
            HealingThreshold = 25;
            Priority = HealPriority.Low;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();
            if (rando > 6)
                return new BiteAndConstrict();

            return new Bite();
        }
    }
}
