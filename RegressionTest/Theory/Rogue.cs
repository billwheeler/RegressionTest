using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Rogue : BaseCharacter
    {
        public class RevenantBlade : BaseAction
        {
            public Rogue parent { get; set; }

            private string _desc = "Longbow";
            private bool _saThisTurn = false;

            public RevenantBlade()
            {
                Desc = "Revenant Blade";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
                IsMagical = true;
            }

            public override string Desc
            {
                get
                {
                    string output = _desc;

                    if (_saThisTurn)
                        output += " (SA)";

                    return output;
                }
                set { _desc = value; }
            }

            public override int Amount()
            {
                int damage = (Time != ActionTime.BonusAction) ?
                    Dice.D4(CriticalHit ? 4 : 2) : 
                    Dice.D4(CriticalHit ? 2 : 1);

                if (!parent.DidSneakAttack)
                {
                    damage += Dice.D6(CriticalHit ? 10 : 5);
                    _saThisTurn = true;
                    parent.DidSneakAttack = true;
                }

                return damage + Modifier;
            }
        }

        public bool DidSneakAttack { get; set; }

        public Rogue() : base()
        {
            Name = "Amxikas";
            AC = 17;
            InitMod = 7 + 4;
            Health = 75;
            MaxHealth = 75;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            DidSneakAttack = false;
            MyType = CreatureType.PC;
            OpportunityAttackChance = 60;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = 3 });
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

        public override bool ShouldUncannyDodge(int amount, BaseAction.ActionType actionType)
        {
            if (!UsedReaction && (actionType == BaseAction.ActionType.MeleeAttack || actionType == BaseAction.ActionType.RangedAttack || actionType == BaseAction.ActionType.SpellAttack))
            {
                bool shouldUseEvasion = false;

                int currentHP = Health + TempHitPoints;

                // if this would kill me, evade!
                if (amount >= currentHP)
                {
                    shouldUseEvasion = true;
                }

                // if this takes away half my health or more, evade!
                if (!shouldUseEvasion && amount >= (int)Math.Floor(currentHP / 2.0))
                {
                    shouldUseEvasion = true;
                }

                // if it's above a threshold, evade!
                if (!shouldUseEvasion && amount > 24)
                {
                    shouldUseEvasion = true;
                }

                // if we're low on health, always evade!
                if (!shouldUseEvasion && currentHP <= (int)Math.Floor(MaxHealth / 3.0))
                {
                    shouldUseEvasion = true;
                }

                if (shouldUseEvasion)
                {
                    UsedReaction = true;
                    return true;
                }
            }

            return false;
        }

        public override BaseAction PickAction()
        {
            return new RevenantBlade { Time = BaseAction.ActionTime.Action, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            return new RevenantBlade { Time = BaseAction.ActionTime.BonusAction, parent = this };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;
            return new RevenantBlade { Time = BaseAction.ActionTime.Reaction, parent = this };
        }
    }
}
