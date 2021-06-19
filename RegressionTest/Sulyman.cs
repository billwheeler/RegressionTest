using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Sulyman : BaseCharacter
    {
        public class GuidingBoltSpirit : GuidingBolt
        {
            public GuidingBoltSpirit()
            {
                Desc = "Guiding Bolt";
                Number = 2;
                Modifier = 9;
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
                        return Dice.D8() + Dice.D8() + 4;
                    return Dice.D8() + 4;
                }

                return base.Damage();
            }
        }

        public class GuidingBolt : BaseAttack
        {
            public int Level { get; set; }

            public GuidingBolt()
            {
                Desc = "Guiding Bolt";
                Modifier = 9;
            }

            public override int Damage()
            {
                int numDice = 3 + Level;
                if (CriticalHit) numDice *= 2;

                int damage = 0;
                for (int i = 0; i < numDice; i++)
                {
                    damage += Dice.D6();
                }

                return damage;
            }
        }

        public class TollOfTheDead : BaseAttack
        {
            public TollOfTheDead()
            {
                Desc = "Toll of the Dead";
                Modifier = 9;
            }

            public override int Damage()
            {
                return Dice.D12() + Dice.D12();
            }
        }

        public class TollOfTheDeadSpirit : TollOfTheDead
        {
            public TollOfTheDeadSpirit()
            {
                Desc = "Guiding Bolt";
                Number = 2;
                Modifier = 9;
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
                        return Dice.D8() + Dice.D8() + 4;
                    return Dice.D8() + 4;
                }

                return base.Damage();
            }
        }

        public Sulyman()
        {
            Name = "Sulyman";
            AC = 16;
            InitMod = 1;
            Health = 75;
            MaxHealth = 75;
            Group = Team.TeamOne;
            Healer = true;
            Priority = HealPriority.High;
        }

        public override BaseAttack PickAttack()
        {
            switch (Dice.D10())
            {
                case 1:
                    return new GuidingBoltSpirit { Level = Dice.D5() };
                case 2:
                    return new GuidingBoltSpirit { Level = Dice.D5() };
                case 3:
                    return new GuidingBolt { Level = Dice.D5() };
                case 4:
                    return new GuidingBolt { Level = Dice.D5() };
                case 5:
                    return new TollOfTheDeadSpirit();
                case 6:
                    return new TollOfTheDeadSpirit();
                case 7:
                    return new TollOfTheDeadSpirit();
                case 8:
                    return new TollOfTheDeadSpirit();
                case 9:
                    return new TollOfTheDead();
                case 10:
                default:
                    return new TollOfTheDead();
            }
        }

        public override int HealAmount(HealPriority priority)
        {
            if (Dice.D10() < 3)
                return Dice.D4() + Dice.D4() + 4;
            return Dice.D4() + 4;
        }
    }
}
