using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Satyr : BaseCharacter
    {
        public class Ram : BaseAction
        {
            public Ram()
            {
                Desc = "Ram";
                Type = ActionType.MeleeAttack;
                AttackModifier = 3;
                Modifier = 1;
            }

            public override int Amount()
            {
                return Dice.D4(CriticalHit ? 4 : 2) + Modifier;
            }
        }

        public class Shortbow : BaseAction
        {
            public Shortbow()
            {
                Desc = "Shortbow";
                Type = ActionType.MeleeAttack;
                AttackModifier = 5;
                Modifier = 3;
            }

            public override int Amount()
            {
                return Dice.D6(CriticalHit ? 2 : 1) + Modifier;
            }
        }

        public Satyr()
        {
            Name = "Erven";
            AC = 14;
            Health = 45;
            MaxHealth = 45;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Priority = HealPriority.Low;
            InitMod = 2;
            MyType = CreatureType.Summon;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 12, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 11, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 12, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 14, Mod = 2, Save = 2 });
        }

        public override BaseAction PickAction()
        {
            if (Dice.D4() == 1)
                return new Ram { Time = BaseAction.ActionTime.Action };

            return new Shortbow { Time = BaseAction.ActionTime.Action };
        }
    }
}
