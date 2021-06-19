using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Wight : BaseCharacter
    {
        public class LongswordAttack : BaseAttack
        {
            public LongswordAttack()
            {
                Desc = "Longsword";
                Number = 2;
                Modifier = 9;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D8() + Dice.D8() + 4;

                return Dice.D8() + 4;
            }
        }

        public class LongswordDrain : LongswordAttack
        {
            public LongswordDrain()
            {
                Desc = "Longsword";
                Number = 2;
                Modifier = 9;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Life Drain";

                return hits;
            }

            public override int Damage()
            {
                if (CurrentAttack > 1)
                    return Dice.D6() + Dice.D6() + Dice.D6() + Dice.D6();

                return base.Damage();
            }
        }

        public Wight()
        {
            Name = "Wight";
            AC = 14;
            InitMod = 4;
            Health = 54;
            MaxHealth = 54;
            Group = Team.TeamTwo;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            if (Dice.D6() == 1)
                return new LongswordAttack();

            return new LongswordDrain();
        }
    }
}
