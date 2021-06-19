using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class GreyRender : BaseCharacter
    {
        public class GreyRenderAttack : BaseAttack
        {
            public GreyRenderAttack()
            {
                Desc = "Bite";
                Number = 3;
                Modifier = 7;

                int rando = Dice.D10();
                if (rando < 8)
                    Number = 4;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Claw";

                return hits;
            }

            public override int Damage()
            {
                int damage = 0;

                if (CurrentAttack == 1)
                {
                    damage += Dice.D12(CriticalHit ? 4 : 2);
                }
                else
                {
                    damage += Dice.D8(CriticalHit ? 4 : 2);
                }

                return damage + 4;
            }
        }

        public GreyRender()
        {
            Name = "Grey Render";
            AC = 18;
            InitMod = 1;
            Health = 189;
            MaxHealth = 189;
            HealingThreshold = 12;
            Group = Team.TeamTwo;
        }

        public override BaseAttack PickAttack()
        {
            return new GreyRenderAttack();
        }
    }
}
