using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Haltran : BaseCharacter
    {
        public class TollOfTheDead : BaseAttack
        {
            public TollOfTheDead()
            {
                Desc = "Toll of the Dead";
                Modifier = 5;
            }

            public override int Damage()
            {
                return Dice.D12();
            }
        }

        public class TollOfTheDeadSpirit : BaseAttack
        {
            public TollOfTheDeadSpirit()
            {
                Desc = "Toll of the Dead";
                Number = 2;
                Modifier = 5;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Spirtual Weapon";

                return hits;
            }

            public override int Damage()
            {
                if (CurrentAttack > 1)
                    return Dice.D12();
                else
                    return Dice.D8() + 4;
            }
        }

        public Haltran()
        {
            Name = "Haltran";
            AC = 15;
            InitMod = 2;
            Health = 28;
            MaxHealth = 28;
            Group = Team.TeamTwo;
            Healer = true;
            Priority = HealPriority.High;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();
            if (rando > 6)
                return new TollOfTheDeadSpirit();

            return new TollOfTheDead();
        }

        public override int HealAmount(HealPriority priority)
        {
            if (priority == HealPriority.High && Dice.D4() > 1)
                return Dice.D4() + Dice.D4() + 3;

            return Dice.D4() + 3;
        }
    }
}
