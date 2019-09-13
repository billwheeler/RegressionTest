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
                Number = 2;
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
                Number = 2;
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
            Health = 28;
            MaxHealth = 28;
            Group = Team.TeamTwo;
            HealingThreshold = 13;
            Alive = true;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();
            if (rando < 7)
                return new LightCrossbow { Dice = Dice };

            return new Scimitar { Dice = Dice };
        }
    }
}
