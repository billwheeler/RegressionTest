using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Fionula : BaseCharacter
    {
        public class GravitySinkhole : BaseAttack
        {
            public GravitySinkhole()
            {
                Desc = "Gravity Sinkhole";
                Modifier = 9;
            }

            public override int Damage()
            {
                return Dice.D10() + Dice.D10() + Dice.D10() + Dice.D10() + Dice.D10();
            }
        }

        public class EldritchBlast : BaseAttack
        {
            public EldritchBlast()
            {
                Desc = "Eldritch Blast";
                Number = 2;
                Modifier = 9;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D10() + Dice.D10() + 5;

                return Dice.D10() + 5;
            }
        }

        public Fionula()
        {
            Name = "Fionula";
            AC = 15;
            InitMod = 3;
            Health = 75;
            MaxHealth = 75;
            HealingThreshold = 30;
            Group = Team.TeamOne;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();
            if (rando == 10)
            {
                return new GravitySinkhole();
            }
            else
            {
                return new EldritchBlast();
            }
        }
    }
}
