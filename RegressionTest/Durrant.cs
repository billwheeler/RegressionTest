using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Durrant : BaseCharacter
    {
        public class Greatsword : BaseAttack
        {
            public Greatsword()
            {
                Desc = "Greatsword";
                Number = 2;
                Modifier = 8;
            }

            public override int Damage()
            {
                return Dice.D6(CriticalHit ? 4 : 2) + 4;
            }
        }

        public class Longbow : BaseAttack
        {
            public Longbow()
            {
                Desc = "Longbow";
                Number = 2;
                Modifier = 7;
            }

            public override int Damage()
            {
                return Dice.D6(CriticalHit ? 2 : 1) + 4;
            }
        }

        public Durrant()
        {
            Name = "Durrant";
            AC = 17;
            InitMod = 3;
            Health = 80;
            MaxHealth = 80;
            HealingThreshold = 22;
            Group = Team.TeamTwo;
            Priority = HealPriority.High;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();

            if (rando > 6)
                return new Longbow();
            else
                return new Greatsword();
        }
    }
}
