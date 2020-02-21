using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Fionula : BaseCharacter
    {
        public class Witchbolt : BaseAttack
        {
            public Witchbolt()
            {
                Desc = "Witchbolt";
                Modifier = 7;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D12() + Dice.D12() + Dice.D12() + Dice.D12() + Dice.D12() + Dice.D12();

                return Dice.D12() + Dice.D12() + Dice.D12();
            }
        }

        public class EldritchBlast : BaseAttack
        {
            public EldritchBlast()
            {
                Desc = "Eldritch Blast";
                Number = 2;
                Modifier = 7;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D10() + Dice.D10() + 4;

                return Dice.D10() + 4;
            }
        }

        public Fionula()
        {
            Name = "Fionula";
            AC = 15;
            InitMod = 3;
            Health = 44;
            MaxHealth = 44;
            HealingThreshold = 11;
            Group = Team.TeamOne;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();
            if (rando == 10)
            {
                return new Witchbolt();
            }
            else
            {
                return new EldritchBlast();
            }
        }
    }
}
