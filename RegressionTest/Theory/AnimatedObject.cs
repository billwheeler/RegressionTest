using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class AnimatedObject : BaseCharacter
    {
        public class Attack : BaseAction
        {
            public Attack()
            {
                Desc = "Attack";
                Type = ActionType.MeleeAttack;
                AttackModifier = 8;
                Modifier = 4;
            }

            public override int Amount()
            {
                return Dice.D4(CriticalHit ? 2 : 1) + Modifier;
            }
        }

        public AnimatedObject()
        {
            Name = "Animated Object (Tiny)";
            AC = 18;
            Health = 20;
            MaxHealth = 20;
            HealingThreshold = 0;
            Group = Team.TeamOne;
            Priority = HealPriority.Dont;
            InitMod = 4;
            MyType = CreatureType.Summon;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 4, Mod = -3, Save = -3 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 18, Mod = 4, Save = 4 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 3, Mod = -4, Save = -4 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 3, Mod = -4, Save = -4 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 1, Mod = -5, Save = -5 });
        }

        public override BaseAction PickAction()
        {
            return new Attack { Time = BaseAction.ActionTime.Action };
        }
    }
}
