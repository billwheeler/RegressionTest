using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Lorax : BaseCharacter
    {
        public class Whip : BaseAttack
        {
            public bool SuperiorityDice { get; set; } = false;

            public Whip()
            {
                Desc = "Whip";
                Modifier = 9;
                Number = 3;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 2)
                    Desc = "Sword";

                return hits;
            }

            public override int Damage()
            {
                int damage = 0;

                if (CurrentAttack > 2)
                {
                    if (CriticalHit)
                        return Dice.D8() + Dice.D8();

                    return Dice.D8();
                }

                if (SuperiorityDice && CurrentAttack == 1)
                {
                    damage += Dice.D8();
                    if (CriticalHit)
                        damage += Dice.D8();
                }

                damage += Dice.D4();
                if (CriticalHit)
                    damage += Dice.D4();

                return damage + 5;
            }
        }

        public int SuperiorityDice { get; set; } = 5;

        public Lorax() : base()
        {
            Name = "Lorax";
            AC = 17;
            Health = 63;
            MaxHealth = 63;
            HealingThreshold = 15;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            //ActionSurge = 1;
            Scores.Strength = 7;
            Scores.Dexterity = 3;
            Scores.Constitution = 4;
            Scores.Intelligence = 0;
            Scores.Wisdom = -1;
            Scores.Charisma = 2;
        }

        public override BaseAttack PickAttack()
        {
            if (SuperiorityDice > 0)
            {
                SuperiorityDice--;
                return new Whip { SuperiorityDice = true };
            }

            return new Whip { SuperiorityDice = false };
        }
    }
}
