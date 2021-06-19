using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Nightwalker : BaseCharacter
    {
        public class EnervatingFocus : BaseAttack
        {
            public EnervatingFocus()
            {
                Desc = "Enervating Focus";
                Number = 3;
                Modifier = 12;
            }

            public override int Damage()
            {
                if (CurrentAttack == 1)
                {
                    return Dice.D6() + Dice.D6() + Dice.D6() + Dice.D6();
                }

                int numDice = 5;
                if (CriticalHit) numDice *= 2;

                int damage = 0;
                for (int i = 0; i < numDice; i++)
                {
                    damage += Dice.D8();
                }

                return damage + 6;
            }
        }

        public class EnervatingFocusFinger : EnervatingFocus
        {
            public EnervatingFocusFinger()
            {
                Desc = "Enervating Focus";
                Number = 3;
                Modifier = 12;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Finger of Death";

                return hits;
            }

            public override int Damage()
            {
                if (CurrentAttack > 2)
                {
                    return Dice.D12() + Dice.D12() + Dice.D12() + Dice.D12();
                }

                return base.Damage();
            }
        }

        public Nightwalker()
        {
            Name = "Nightwalker";
            AC = 14;
            InitMod = 4;
            Health = 297;
            MaxHealth = 297;
            Group = Team.TeamTwo;
        }

        public override BaseAttack PickAttack()
        {
            if (Dice.D6() == 6)
                return new EnervatingFocusFinger();

            return new EnervatingFocus();
        }
    }
}
