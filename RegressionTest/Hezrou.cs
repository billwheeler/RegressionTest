using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Herzou : BaseCharacter
    {
        public class HerzouAttack : BaseAttack
        {
            public HerzouAttack()
            {
                Desc = "Bite";
                Number = 3;
                Modifier = 7;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Claws";

                return hits;
            }

            public override int Damage()
            {
                int damage = 0;

                if (CurrentAttack > 1)
                    damage += Dice.D10(2);
                else
                    damage += Dice.D6(2);

                return damage + 4;
            }
        }

        public Herzou()
        {
            Name = "Herzou";
            AC = 16;
            InitMod = 3;
            Health = 136;
            MaxHealth = 136;
            HealingThreshold = 60;
            Group = Team.TeamTwo;
        }

        public override BaseAttack PickAttack()
        {
            return new HerzouAttack();
        }
    }

    public class SmolHerzou : BaseCharacter
    {
        public class HerzouAttack : BaseAttack
        {
            public HerzouAttack()
            {
                Desc = "Bite";
                Number = 3;
                Modifier = 5;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Claws";

                return hits;
            }

            public override int Damage()
            {
                int damage = 0;

                if (CurrentAttack > 1)
                    damage += Dice.D8(2);
                else
                    damage += Dice.D4(2);

                return damage + 4;
            }
        }

        public SmolHerzou()
        {
            Name = "Smol Herzou";
            AC = 16;
            InitMod = 3;
            Health = 68;
            MaxHealth = 68;
            HealingThreshold = 30;
            Group = Team.TeamTwo;
        }

        public override BaseAttack PickAttack()
        {
            return new HerzouAttack();
        }
    }
}
