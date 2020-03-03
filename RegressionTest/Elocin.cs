using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Elocin : BaseCharacter
    {
        public class TollOfTheDead : BaseAttack
        {
            public TollOfTheDead()
            {
                Desc = "Vicious Mockery";
                Modifier = 7;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D4() + Dice.D4() + Dice.D4() + Dice.D4();
                return Dice.D4() + Dice.D4();
            }
        }

        public Elocin()
        {
            Name = "Elocin";
            AC = 15;
            InitMod = 1;
            Health = 51;
            MaxHealth = 51;
            Group = Team.TeamOne;
            Healer = true;
            Priority = HealPriority.High;
        }

        public override BaseAttack PickAttack()
        {
            return new TollOfTheDead();
        }

        public override int HealAmount(HealPriority priority)
        {
            if (Dice.D10() < 3)
                return Dice.D4() + Dice.D4() + 4;
            return Dice.D4() + 4;
        }
    }
}
