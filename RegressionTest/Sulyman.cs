using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Sulyman : BaseCharacter
    {
        public class TollOfTheDead : BaseAttack
        {
            public TollOfTheDead()
            {
                Desc = "Toll of the Dead";
                Modifier = 7;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D12() + Dice.D12() + Dice.D12() + Dice.D12();
                return Dice.D12() + Dice.D12();
            }
        }

        public class TollOfTheDeadSpirit : BaseAttack
        {
            public TollOfTheDeadSpirit()
            {
                Desc = "Toll of the Dead";
                Number = 2;
                Modifier = 7;
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
                {
                    if (CriticalHit)
                        return Dice.D12() + Dice.D12() + Dice.D12() + Dice.D12();
                    return Dice.D12() + Dice.D12();
                }
                else
                {
                    if (CriticalHit)
                        return Dice.D8() + Dice.D8() + 4;
                    return Dice.D8() + 4;
                }
            }
        }

        public Sulyman()
        {
            Name = "Sulyman";
            AC = 15;
            InitMod = 1;
            Health = 51;
            MaxHealth = 51;
            Group = Team.TeamOne;
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
            if (Dice.D10() < 3)
                return Dice.D4() + Dice.D4() + 4;
            return Dice.D4() + 4;
        }
    }
}
