using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Gnoll : BaseCharacter
    {
        public class GnollAttack : BaseAttack
        {
            public GnollAttack()
            {
                Desc = "Spear";
                Number = 2;
                Modifier = 5;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Bite";

                return hits;
            }

            public override int Damage()
            {
                int damage = 0;

                if (CurrentAttack == 1)
                {
                    damage += Dice.D8();
                    if (CriticalHit) damage += Dice.D8();
                }
                else
                {
                    damage += Dice.D4();
                    if (CriticalHit) damage += Dice.D4();
                }

                return damage + 2;
            }
        }

        public Gnoll()
        {
            Name = "Gnoll";
            AC = 15;
            InitMod = 1;
            Health = 22;
            MaxHealth = 22;
            HealingThreshold = 12;
            Group = Team.TeamTwo;
        }

        public override BaseAttack PickAttack()
        {
            return new GnollAttack();
        }
    }

    public class GnollPackLord : BaseCharacter
    {
        public class GnollAttack : BaseAttack
        {
            public GnollAttack()
            {
                Desc = "Glaive";
                Number = 3;
                Modifier = 6;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 2)
                    Desc = "Bite";

                return hits;
            }

            public override int Damage()
            {
                int damage = 0;

                if (CurrentAttack < 3)
                {
                    damage += Dice.D10();
                    if (CriticalHit) damage += Dice.D10();
                }
                else
                {
                    damage += Dice.D4();
                    if (CriticalHit) damage += Dice.D4();
                }

                return damage + 3;
            }
        }

        public GnollPackLord()
        {
            Name = "Gnoll Pack Lord";
            AC = 15;
            InitMod = 2;
            Health = 71;
            MaxHealth = 71;
            HealingThreshold = 12;
            Group = Team.TeamTwo;
        }

        public override BaseAttack PickAttack()
        {
            return new GnollAttack();
        }
    }
}
