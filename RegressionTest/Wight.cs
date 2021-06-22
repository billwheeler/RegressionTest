using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Wight : BaseCharacter
    {
        public class Longsword : BaseAction
        {
            public Longsword()
            {
                Desc = "Longsword";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 9;
                Modifier = 4;
                TotalToRun = 2;
            }

            public override int Amount()
            {
                return Dice.D8(CriticalHit ? 2 : 1) + Modifier;

            }
        }

        public class LifeDrain : BaseAction
        {
            public LifeDrain()
            {
                Desc = "Life Drain";
                Type = ActionType.SpellAttack;
                Time = ActionTime.BonusAction;
                Ability = AbilityScore.Constitution;
                DC = 17;
            }

            public override int Amount()
            {
                return Dice.D6(4);
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

        public override BaseAction PickAction()
        {
            return new Longsword();
        }

        public override BaseAction PickBonusAction()
        {
            return new LifeDrain();
        }

    }
}
