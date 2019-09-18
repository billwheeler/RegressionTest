using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Raelzegg : BaseCharacter
    {
        public class Swashbuckler : BaseAttack
        {
            public bool HadSneakAttack { get; set; } = false;

            public Swashbuckler()
            {
                Desc = "Rapier";
                Modifier = 7;
                Number = 2;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Dagger";

                return hits;
            }

            public override int Damage()
            {
                int damage = CurrentAttack == 1 ?
                    Dice.D8() + 4 :
                    Dice.D4();

                if (!HadSneakAttack)
                {
                    if (Dice.D10() < 10)
                    {
                        damage += (Dice.D6() + Dice.D6() + Dice.D6());
                        HadSneakAttack = true;
                    }
                }

                return damage;
            }
        }

        public Raelzegg()
        {
            Name = "Raelzegg";
            AC = 17;
            InitMod = 6;
            Health = 43;
            MaxHealth = 43;
            HealingThreshold = 15;
            Group = Team.TeamOne;
        }

        public override BaseAttack PickAttack()
        {
            return new Swashbuckler();
        }
    }
}
