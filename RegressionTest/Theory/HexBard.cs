using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class HexBard : BaseCharacter
    {
        public class Rapier : BaseAction
        {
            public readonly bool CanUseBoomingBlade = true;

            public Rapier()
            {
                Desc = "Rapier";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 9;
                Modifier = 5;
                TotalToRun = 2;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 2 : 1);

                if (CanUseBoomingBlade)
                {
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                    int percentageEnemyMoves = (Time == ActionTime.Reaction) ? 90 : 25;
                    if (Dice.D100() <= percentageEnemyMoves)
                    {
                        damage += Dice.D8(2);
                    }
                }

                return damage + Modifier;
            }
        }

        public class EldritchBlast : BaseAction
        {
            public EldritchBlast()
            {
                Desc = "Eldritch Blast";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 9;
                Modifier = 0;
                TotalToRun = 2;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D10(CriticalHit ? 2 : 1);

                return damage + Modifier;
            }
        }

        public class Inspiration : BaseAction
        {
            public Inspiration(string target)
            {
                Desc = $"Inspiration, targets {target}";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public int ShieldUses { get; set; } = 0;
        public bool DidBigSpell { get; set; }
        public bool HypnoticPatternRunning { get; set; }

        public HexBard() : base()
        {
            Name = "Orianna";
            AC = 18;
            InitMod = 4;
            Health = 75;
            MaxHealth = 75;
            Group = Team.TeamOne;
            HealingThreshold = 18;
            Priority = HealPriority.Medium;
            MyType = CreatureType.PC;
            Healer = true;
            WarCaster = false;
            OpportunityAttackChance = 10;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 12, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 8, Mod = -1, Save = 3 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 20, Mod = 5, Save = 9 });
        }

        public override void Init()
        {
            base.Init();

            DidBigSpell = false;
            HypnoticPatternRunning = false;
        }

        public override void OnNewEncounter()
        {
            base.OnNewEncounter();
            SetTempHitPoints(15);
            ShieldUses = 4;
        }

        public override bool OnNewRound()
        {
            return base.OnNewRound();
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();

            if (!DidBigSpell)
            {
                BonusActionFirst = true;
            }
            else
            {
                BonusActionFirst = false;
            }
        }

        public override BaseAction PickAction()
        {
            if (!DidBigSpell)
            {
                if (Context.AnyoneHaveEffect(Group, SpellEffectType.SynapticStatic))
                {
                    DidBigSpell = true;
                    Concentrating = true;
                    HypnoticPatternRunning = true;
                    return new HypnoticPattern(17);
                }
                else
                {
                    DidBigSpell = true;
                    return new SynapticStatic(17);
                }
            }

            bool shouldDodge = false;
            if (Health < 25)
                shouldDodge = true;

            if (shouldDodge)
            {
                IsDodging = true;
                return new DodgeAction { Time = BaseAction.ActionTime.Action };
            }

            if (Dice.D100() <= 80)
                return new Rapier();

            return new EldritchBlast();
        }

        public override BaseAction PickBonusAction()
        {
            if (Healer && HealTarget != null)
            {
                return new HealingWord { Modifier = 4, Level = SpellAction.SpellLevel.One };
            }

            BaseCharacter ally = Context.PickRandomTeammate(Group, ID);
            if (ally != null)
            {
                ally.ApplyEffect(new SpellEffect
                {
                    DC = 0,
                    Type = SpellEffectType.Inspired,
                    Active = true
                });

                return new Inspiration(ally.Name) { Time = BaseAction.ActionTime.BonusAction };
            }

            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;
            return new Rapier { Time = BaseAction.ActionTime.Reaction };
        }

        public override void PreHitCalc(int attackRoll, int modifier, bool potentiallyPowerful, bool criticalHit)
        {
            bool shouldCastShield = false;

            if (!HasShieldRunning && ShieldUses > 0)
            {
                if (attackRoll + modifier > AC)
                {
                    if (Health < 30)
                    {
                        shouldCastShield = true;
                    }
                    else if (potentiallyPowerful)
                    {
                        shouldCastShield = true;
                    }
                    else if (criticalHit)
                    {
                        shouldCastShield = true;
                    }
                }
            }

            if (shouldCastShield)
            {
                UsedReaction = true;
                HasShieldRunning = true;
                ShieldUses--;
            }
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            if (HypnoticPatternRunning)
            {
                HypnoticPatternRunning = false;
                Context.EndEffect(Group, SpellEffectType.HypnoticPattern);
            }
        }

        public override void OnDeath()
        {
            base.OnDeath();

            if (HypnoticPatternRunning)
            {
                HypnoticPatternRunning = false;
                Context.EndEffect(Group, SpellEffectType.HypnoticPattern);
            }
        }
    }
}

