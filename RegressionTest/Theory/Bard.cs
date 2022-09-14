using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Bard : BaseCharacter
    {
        public class Rapier : BaseAction
        {
            public readonly bool CanUseBoomingBlade = true;

            public Rapier()
            {
                Desc = "Rapier";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 7;
                Modifier = 3;
                TotalToRun = 1;
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
                        damage += Dice.D8(CriticalHit ? 4 : 2);
                    }
                }

                return damage + Modifier;
            }
        }

        public class RayOfFrost : BaseAction
        {
            public RayOfFrost()
            {
                Desc = "Ray of Frost";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 8;
                Modifier = 0;
                TotalToRun = 1;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 4 : 2);

                return damage + Modifier;
            }
        }

        public class UnsettlingWords : BaseAction
        {
            public UnsettlingWords()
            {
                Desc = "Unsettling Words";
                Type = ActionType.Apply;
                Time = ActionTime.BonusAction;
                MinTargets = 1;
                MaxTargets = 1;

                EffectToApply = new SpellEffect
                {
                    Name = "Unsettling Words",
                    Type = SpellEffectType.UnsettlingWords
                };
            }
        }

        public bool DidBigSpell { get; set; }
        public bool HypnoticPatternRunning { get; set; }

        public int Inspirations { get; set; }

        public int ShieldUses { get; set; } = 3;

        public Bard()
        {
            Name = "Orianna";
            AC = 15;
            InitMod = 4;
            Health = 66;
            MaxHealth = 66;
            Group = Team.TeamOne;
            HealingThreshold = 18;
            Priority = HealPriority.Medium;
            MyType = CreatureType.PC;
            WarCaster = true;
            OpportunityAttackChance = 5;
            GiftOfAlacrity = true;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 14, Mod = 2, Save = 6 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 18, Mod = 4, Save = 8 });
        }

        public override void PreHitCalc(int attackRoll, int modifier, bool potentiallyPowerful, bool criticalHit)
        {
            bool shouldCastShield = false;

            if (!HasShieldRunning && ShieldUses > 0)
            {
                if (attackRoll + modifier > AC)
                {
                    if (Health < 20)
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
        }

        public override void OnNewRound()
        {
            base.OnNewRound();
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
                    return new HypnoticPattern(16);
                }
                else
                {
                    DidBigSpell = true;
                    return new SynapticStatic(16);
                }
            }

            bool shouldDodge = false;
            if (Dice.D100() > Health)
                shouldDodge = true;

            if (shouldDodge)
            {
                IsDodging = true;
                return new DodgeAction { Time = BaseAction.ActionTime.Action };
            }

            if (Dice.D100() <= 20)
                return new Rapier();

            return new RayOfFrost();
        }

        public override BaseAction PickBonusAction()
        {
            if (!DidBigSpell)
            {
                return new UnsettlingWords();
            }

            if (Healer && HealTarget != null)
            {
                return new HealingWord { Modifier = 4, Level = SpellAction.SpellLevel.Three };
            }

            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            return new Rapier { Time = BaseAction.ActionTime.Reaction };
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            if (HypnoticPatternRunning)
            {
                HypnoticPatternRunning = false;
                Context.EndHypnoticPattern(Group);
            }
        }

        public override void OnDeath()
        {
            base.OnDeath();

            if (HypnoticPatternRunning)
            {
                HypnoticPatternRunning = false;
                Context.EndHypnoticPattern(Group);
            }
        }
    }
}

