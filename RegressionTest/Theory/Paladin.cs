using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Paladin : BaseCharacter
    {
        public class GlaivePM : BaseAction
        {
            public GlaivePM()
            {
                Desc = "Glaive";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
            }

            public override int Amount()
            {
                int damage = 0;

                damage += (Time == ActionTime.Action) ?
                    Dice.D10(CriticalHit ? 2 : 1) :
                    Dice.D4(CriticalHit ? 2 : 1);

                damage += Dice.D8(CriticalHit ? 2 : 1);

                return damage + Modifier;
            }
        }

        public class LayOnHands : SpellAction
        {
            public LayOnHands()
            {
                Desc = "Lay On Hands";
                Type = ActionType.Heal;
                Time = ActionTime.Action;
            }

            public override int Amount()
            {
                return 10;
            }
        }

        public Paladin()
        {
            Name = "Murie";
            AC = 18;
            Health = 85;
            MaxHealth = 85;
            HealingThreshold = 30;
            Group = Team.TeamOne;
            Healer = true;
            Priority = HealPriority.Medium;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 20, Mod = 5, Save = 8 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 10, Mod = 0, Save = 3 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 6 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = 2 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 7 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 16, Mod = 3, Save = 10 });
        }

        public override BaseAction PickAction()
        {
            if (HealTarget != null)
            {
                return new LayOnHands { Owner = this };
            }

            return new GlaivePM { Owner = this, Time = BaseAction.ActionTime.Action, TotalToRun = 2 };
        }

        public override BaseAction PickBonusAction()
        {
            return new GlaivePM { Owner = this, Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1 };
        }
    }
}
