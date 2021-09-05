using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Rogue : BaseCharacter
    {
        public class Longsword : BaseAction
        {
            public Rogue parent { get; set; }

            public Longsword()
            {
                Desc = "Longsword";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
            }

            public override int Amount()
            {
                int damage = (Time == ActionTime.Action) ?
                    Dice.D4(CriticalHit ? 4 : 2) : 
                    Dice.D4(CriticalHit ? 2 : 1);

                if (!parent.DidSneakAttack)
                {
                    damage += Dice.D6(CriticalHit ? 10 : 5);
                    parent.DidSneakAttack = true;
                }

                return damage + Modifier;
            }
        }

        public class Shortsword : BaseAction
        {
            public Rogue parent { get; set; }

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

                if (!parent.DidSneakAttack)
                {
                    damage += Dice.D6(CriticalHit ? 10 : 5);
                    parent.DidSneakAttack = true;
                }

                if (Time == ActionTime.Action)
                {
                    damage += Modifier;
                }

                return damage;
            }
        }

        public class Dagger : BaseAction
        {
            public Rogue parent { get; set; }

            public Dagger()
            {
                Desc = "Dagger";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
            }

            public override int Amount()
            {
                int damage = Dice.D4(CriticalHit ? 4 : 2);

                if (!parent.DidSneakAttack)
                {
                    damage += Dice.D6(CriticalHit ? 10 : 5);
                    parent.DidSneakAttack = true;
                }

                if (Time == ActionTime.Action)
                {
                    damage += Modifier;
                }

                return damage;
            }
        }

        public bool DidSneakAttack { get; set; }

        public Rogue()
        {
            Name = "Amxikas";
            AC = 18;
            InitMod = 7;
            Health = 83;
            MaxHealth = 83;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            DidSneakAttack = false;
            MyType = CreatureType.PC;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 9, Mod = -1, Save = 3 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 14, Mod = 2, Save = 2 });
        }

        public override void Init()
        {
            base.Init();
            DidSneakAttack = false;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();
            DidSneakAttack = false;
        }

        public override int OnTakeDamage(int amount, BaseAction.ActionType actionType)
        {
            if (!UsedReaction && (actionType == BaseAction.ActionType.MeleeAttack || actionType == BaseAction.ActionType.RangedAttack))
            {
                bool shouldUseEvasion = false;

                int currentHP = Health + TempHitPoints;

                // if this would kill me, evade!
                if (amount >= currentHP)
                    shouldUseEvasion = true;

                // if this takes away half my health or more, evade!
                if (amount >= (int)Math.Floor(currentHP / 2.0))
                    shouldUseEvasion = true;

                if (shouldUseEvasion)
                {
                    amount = (int)Math.Floor(amount / 2.0);
                    UsedReaction = true;
                }
            }

            return base.OnTakeDamage(amount, actionType);
        }

        public override BaseAction PickAction()
        {
            return new Longsword { Time = BaseAction.ActionTime.Action, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            return new Longsword { Time = BaseAction.ActionTime.BonusAction, parent = this };
        }
    }
}
