using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Karrius : BaseCharacter
    {
        public class Greatsword : BaseAttack
        {
            public Greatsword()
            {
                Desc = "Greatsword";
                Number = 2;
                Modifier = 7;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D6() + Dice.D6() + Dice.D6() + Dice.D6() + 4;

                return Dice.D6() + Dice.D6() + 4;
            }
        }

        public class GreatswordBoom : BaseAttack
        {
            public GreatswordBoom()
            {
                Desc = "Greatsword";
                Number = 3;
                Modifier = 7;
            }

            public override int Damage()
            {
                if (CurrentAttack < 3)
                {
                    if (CriticalHit)
                        return Dice.D6() + Dice.D6() + Dice.D6() + Dice.D6() + 4 + Dice.D8() + Dice.D8();

                    return Dice.D6() + Dice.D6() + 4 + Dice.D8();
                }
                else
                {
                    if (CriticalHit)
                        return Dice.D8();
                    return Dice.D8();
                }
            }
        }


        public class EldritchBlast : BaseAttack
        {
            public EldritchBlast()
            {
                Desc = "Eldritch Blast";
                Number = 2;
                Modifier = 6;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D10() + Dice.D10() + 3;

                return Dice.D10() + 3;
            }
        }

        public Karrius()
        {
            Name = "Karrius";
            AC = 16;
            InitMod = 2;
            Health = 54;
            MaxHealth = 54;
            HealingThreshold = 22;
            Group = Team.TeamTwo;
            Priority = HealPriority.High;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();

            if (rando == 10)
                return new GreatswordBoom();
            else if (rando > 4)
                return new EldritchBlast();
            else
                return new Greatsword();
        }
    }
}
