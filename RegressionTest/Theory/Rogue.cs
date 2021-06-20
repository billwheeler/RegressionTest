using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Rogue : BaseCharacter
    {
        public class Shortsword : BaseAction
        {
            public Shortsword()
            {
                Desc = "Shortsword";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
            }

            public override int Amount()
            {
                int damage = Dice.D6(CriticalHit ? 2 : 1);

                if (!DidSneakAttack)
                {
                    damage += Dice.D6(CriticalHit ? 10 : 5);
                    DidSneakAttack = true;
                }

                if (Time == ActionTime.Action)
                {
                    damage += Modifier;
                }

                return damage;
            }
        }

        public class SoulDagger : BaseAction
        {
            public SoulDagger()
            {
                Desc = "Soul Dagger";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
            }

            public override int Amount()
            {
                int damage = Time == ActionTime.Action ?
                    Dice.D4(CriticalHit ? 4 : 2) :
                    Dice.D4(CriticalHit ? 2 : 1);

                if (!DidSneakAttack)
                {
                    damage += Dice.D6(CriticalHit ? 10 : 5);
                    DidSneakAttack = true;
                }

                return damage + Modifier;
            }
        }

        public static bool DidSneakAttack { get; set; }

        public Rogue()
        {
            Name = "Amxikas";
            AC = 17;
            InitMod = 5;
            Health = 83;
            MaxHealth = 83;
            HealingThreshold = 30;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            DidSneakAttack = false;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 9, Mod = -1, Save = 3 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 12, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 12, Mod = 2, Save = 2 });
        }

        public override void OnNewTurn()
        {
            DidSneakAttack = false;
        }

        public override BaseAction PickAction()
        {
            return new SoulDagger { Owner = this, Time = BaseAction.ActionTime.Action };
        }

        public override BaseAction PickBonusAction()
        {
            return new SoulDagger { Owner = this, Time = BaseAction.ActionTime.BonusAction };
        }
    }
}
