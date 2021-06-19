using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class FireElemental : BaseCharacter
    {
        public class FireElementalAttack : BaseAttack
        {
            public FireElementalAttack()
            {
                Desc = "Touch";
                Number = 2;
                Modifier = 8;
            }

            public override int Damage()
            {
                return Dice.D8(CriticalHit ? 4 : 2) + 3;
            }
        }

        public FireElemental()
        {
            Name = "Fire Elemental";
            AC = 16;
            InitMod = 3;
            Health = 68;
            MaxHealth = 68;
            HealingThreshold = 60;
            Group = Team.TeamTwo;
        }

        public override BaseAttack PickAttack()
        {
            return new FireElementalAttack();
        }
    }
}
