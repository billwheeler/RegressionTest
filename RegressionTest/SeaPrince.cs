using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class SeaPrince : BaseCharacter
    {
        public class Scimitar : BaseAttack
        {
            public bool HadSneakAttack { get; set; } = false;

            public Scimitar()
            {
                Desc = "Scimitar";
                Number = 2;
                Modifier = 6;
            }

            public override int Damage()
            {
                int dmg = Dice.D6();
                if (CriticalHit)
                    dmg += Dice.D6();

                return dmg + 4;
            }
        }

        public SeaPrince()
        {
            Name = "Sea Prince";
            AC = 15;
            InitMod = 3;
            Health = 39;
            MaxHealth = 39;
            HealingThreshold = 17;
            Group = Team.TeamTwo;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            return new Scimitar();
        }
    }

}
