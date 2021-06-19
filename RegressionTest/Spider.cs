using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Spider : BaseCharacter
    {
        public class SpiderAttack : BaseAttack
        {
            public SpiderAttack()
            {
                Desc = "Bite";
                Modifier = 7;
            }

            public override int Damage()
            {
                int damage = 0;

                damage += (Dice.D12() + Dice.D12());
                if (CriticalHit) damage += (Dice.D12() + Dice.D12());

                int rando = Dice.D10();
                if (rando < 4)
                    damage += Dice.D6() + Dice.D6() + Dice.D6() + Dice.D6() + Dice.D6() + Dice.D6();

                return damage + 5;
            }
        }

        public Spider()
        {
            Name = "Spider";
            AC = 15;
            InitMod = 5;
            Health = 34;
            MaxHealth = 34;
            HealingThreshold = 12;
            Group = Team.TeamTwo;
        }

        public override BaseAttack PickAttack()
        {
            return new SpiderAttack();
        }
    }
}
