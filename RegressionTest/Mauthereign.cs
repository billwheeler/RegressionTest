using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Mauthereign : BaseCharacter
    {
        public class ChainLightning : BaseAttack
        {
            public ChainLightning()
            {
                Desc = "Chain Lightning";
                Number = 1;
                Modifier = 11;
            }

            public override int Damage()
            {
                return Dice.D8(10);
            }
        }

        public class SynapticStatic : BaseAttack
        {
            public SynapticStatic()
            {
                Desc = "Synaptic Static";
                Number = 1;
                Modifier = 11;
            }

            public override int Damage()
            {
                return Dice.D6(8);
            }
        }

        public class Shatter : BaseAttack
        {
            public Shatter()
            {
                Desc = "Shatter";
                Number = 1;
                Modifier = 11;
            }

            public override int Damage()
            {
                return Dice.D8(6);
            }
        }

        public class RayOfFrost : BaseAttack
        {
            public RayOfFrost()
            {
                Desc = "Ray of Frost";
                Number = 1;
                Modifier = 11;
            }

            public override int Damage()
            {
                return Dice.D8(4);
            }
        }

        public Mauthereign()
        {
            Name = "Mauthereign Vance";
            AC = 16;
            Health = 121;
            MaxHealth = 121;
            HealingThreshold = 60;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            if (CurrentRound == 1)
                return new ChainLightning();

            if (CurrentRound == 2)
                return new SynapticStatic();

            if (CurrentRound == 3)
                return new Shatter();

            return new RayOfFrost();
        }
    }
}
