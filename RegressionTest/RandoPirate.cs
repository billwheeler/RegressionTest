using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class RandoPirate : BaseCharacter
    {
        public class Scimitar : BaseAttack
        {
            public Scimitar()
            {
                Desc = "Scimitar";
                Modifier = 3;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D6() + Dice.D6() + 1;

                return Dice.D6() + 1;
            }
        }

        public class LightCrossbow : BaseAttack
        {
            public LightCrossbow()
            {
                Desc = "Light Crossbow";
                Modifier = 3;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D8() + Dice.D8() + 1;

                return Dice.D8() + 1;
            }
        }

        public RandoPirate()
        {
            Name = "Rando Pirate";
            AC = 14;
            InitMod = 4;
            Health = 11;
            MaxHealth = 11;
            Group = Team.TeamTwo;
            HealingThreshold = 7;
            Priority = HealPriority.Low;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();
            if (rando < 7)
                return new LightCrossbow();

            return new Scimitar();
        }
    }
}
