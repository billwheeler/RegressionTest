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
                int damage = Dice.D6(CriticalHit ? 4 : 2);
                return damage + Modifier;
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
                return Dice.D6(6) + 4;
            }
        }

        public Wight() : base()
        {
            Name = "Wight";
            AC = 14;
            InitMod = 4;
            Health = 69;
            MaxHealth = 69;
            Group = Team.TeamTwo;
            Priority = HealPriority.Medium;
            IsUndead = true;
            ResistsNonmagical = true;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 18, Mod = 4, Save = 4 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 13, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 15, Mod = 2, Save = 2 });
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
