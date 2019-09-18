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
                Number = 1;
                Modifier = 4;
            }

            public override int Damage()
            {
                return Dice.D6() + 2;
            }
        }

        public class LightCrossbow : BaseAttack
        {
            public LightCrossbow()
            {
                Desc = "Light Crossbow";
                Number = 1;
                Modifier = 4;
            }

            public override int Damage()
            {
                return Dice.D8() + 2;
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
            HealingThreshold = 6;
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
