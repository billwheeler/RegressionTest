using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Soulknife : BaseCharacter
    {
        public class Soulblades : BaseAction
        {
            public Soulknife parent { get; set; }

            public Soulblades()
            {
                Desc = "Soul Blades";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
            }

            public override int Amount()
            {
                int damage = Time == ActionTime.Action ?
                    Dice.D4(CriticalHit ? 4 : 2) :
                    Dice.D4(CriticalHit ? 2 : 1);

                if (!parent.DidSneakAttack)
                {
                    damage += Dice.D6(CriticalHit ? 10 : 5);
                    parent.DidSneakAttack = true;
                }

                return damage += Modifier;
            }
        }

        public bool DidSneakAttack { get; set; }

        public Soulknife()
        {
            Name = "Amxikas";
            AC = 17;
            InitMod = 5;
            Health = 73;
            MaxHealth = 73;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            DidSneakAttack = false;
            MyType = CreatureType.PC;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 9, Mod = -1, Save = 3 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 12, Mod = 1, Save = 1 });
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
            return new Soulblades { Time = BaseAction.ActionTime.Action, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            return new Soulblades { Time = BaseAction.ActionTime.BonusAction, parent = this };
        }
    }
}
