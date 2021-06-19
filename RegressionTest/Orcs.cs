using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class LordOrc : BaseCharacter
    {
        public class OrcAttack : BaseAttack
        {
            public OrcAttack()
            {
                Desc = "Glaive";
                Number = 3;
                Modifier = 7;
            }

            public override int Damage()
            {
                int damage = 0;

                damage += Dice.D10(CriticalHit ? 2 : 1);

                return damage + 4;
            }
        }

        public LordOrc()
        {
            Name = "Lord Orc";
            AC = 17;
            InitMod = 4;
            Health = 153;
            MaxHealth = 153;
            HealingThreshold = 60;
            Group = Team.TeamTwo;
            Priority = HealPriority.High;
        }

        public override BaseAttack PickAttack()
        {
            return new OrcAttack();
        }
    }

    public class ShamanOrc : BaseCharacter
    {
        public class OrcAttack : BaseAttack
        {
            public OrcAttack()
            {
                Desc = "War Hammer";
                Number = Dice.D10() < 3 ? 3 : 2;
                Modifier = 7;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack == 2)
                    Desc = "Spirtual Weapon";
                if (CurrentAttack == 3)
                    Desc = "Spirit Guardians";

                return hits;
            }

            public override int Damage()
            {
                if (CurrentAttack == 2)
                {
                    return Dice.D8(CriticalHit ? 4 : 2) + 4;
                }
                else if (CurrentAttack == 3)
                {
                    return Dice.D8(3);
                }
                else
                {
                    return Dice.D12(CriticalHit ? 2 : 1) + 4;
                }
            }
        }

        public ShamanOrc()
        {
            Name = "Shaman Orc";
            AC = 18;
            InitMod = 0;
            Health = 71;
            MaxHealth = 71;
            HealingThreshold = 20;
            Group = Team.TeamTwo;
            Healer = true;
        }

        public override BaseAttack PickAttack()
        {
            return new OrcAttack();
        }

        public override int HealAmount(HealPriority priority)
        {
            int dice = 1;
            if (Dice.D10() < 3)
                dice++;
            if (Dice.D10() < 2)
                dice++;

            return Dice.D10(dice) + 4;
        }
    }

    public class BaseOrc : BaseCharacter
    {
        public class OrcAttack : BaseAttack
        {
            public OrcAttack()
            {
                Desc = "Great Axe";
                Number = 2;
                Modifier = 7;
            }

            public override int Damage()
            {
                return Dice.D8(CriticalHit ? 2 : 1) + 4;
            }
        }

        public BaseOrc()
        {
            Name = "Basic Orc";
            AC = 15;
            InitMod = 1;
            Health = 51;
            MaxHealth = 51;
            HealingThreshold = 40;
            Group = Team.TeamTwo;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            return new OrcAttack();
        }
    }
}
