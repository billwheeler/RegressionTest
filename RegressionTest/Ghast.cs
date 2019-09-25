using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Ghast : BaseCharacter
    {
        public class GhastClawsAttack : BaseAttack
        {
            public GhastClawsAttack()
            {
                Desc = "Claws";
                Modifier = 5;
            }

            public override int Damage()
            {
                int damage = Dice.D6() + Dice.D6();

                if (CriticalHit) damage += (Dice.D6() + Dice.D6());

                return damage + 3;
            }
        }

        public class GhastBiteAttack : BaseAttack
        {
            public GhastBiteAttack()
            {
                Desc = "Bite";
                Modifier = 3;
            }

            public override int Damage()
            {
                int damage = Dice.D8() + Dice.D8();

                if (CriticalHit) damage += (Dice.D8() + Dice.D8());

                return damage + 3;
            }
        }

        public Ghast()
        {
            Name = "Ghast";
            AC = 13;
            InitMod = 3;
            Health = 36;
            MaxHealth = 36;
            HealingThreshold = 20;
            Group = Team.TeamTwo;
        }

        public override BaseAttack PickAttack()
        {
            return new GhastClawsAttack();
        }

    }
}
