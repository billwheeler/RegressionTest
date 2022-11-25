using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Sorcerer : BaseCharacter
    {
        public int ShieldUses { get; set; } = 0;
        public bool SynapticStaticRunning { get; set; } = false;
        public bool HypnoticPatternRunning { get; set; } = false;
        public bool BlackTentaclesRunning { get; set; } = false;

        public class Firebolt : BaseAction
        {
            public Firebolt()
            {
                Desc = "Firebolt";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 9;
                Modifier = 0;
                TotalToRun = 1;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D10(CriticalHit ? 4 : 2);
                return damage + Modifier;
            }
        }

        public Sorcerer() : base()
        {
            Name = "Sorcerer";
            AC = 15;
            InitMod = 2;
            Health = 65;
            MaxHealth = 65;
            Group = Team.TeamOne;
            HealingThreshold = 18;
            Priority = HealPriority.High;
            MyType = CreatureType.PC;
            Healer = false;
            WarCaster = false;
            OpportunityAttackChance = 0;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 9, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 20, Mod = 5, Save = 9 });
        }

        public override void Init()
        {
            base.Init();

            ShieldUses = 4;
            SynapticStaticRunning = false;
            HypnoticPatternRunning = false;
            BlackTentaclesRunning = false;
        }

        public override void OnNewEncounter()
        {
            base.OnNewEncounter();

            ShieldUses = 4;
            SynapticStaticRunning = false;
            HypnoticPatternRunning = false;
            BlackTentaclesRunning = false;
        }

        public override BaseAction PickAction()
        {
            //if (!Context.AnyoneHaveEffect(Group, SpellEffectType.Turned))
            {
                if (!SynapticStaticRunning)
                {
                    Stats.SpellsUsed++;
                    SynapticStaticRunning = true;
                    return new SynapticStatic(17);
                }
            }

            if (!HypnoticPatternRunning)
            {
                Stats.SpellsUsed++;
                Concentrating = true;
                HypnoticPatternRunning = true;
                //return new Confusion(17);
                return new HypnoticPattern(17);
            }

            if (Health < 30)
                return new DodgeAction { Time = BaseAction.ActionTime.Action };

            if (!Concentrating)
            {
                Stats.SpellsUsed++;
                Concentrating = true;
                BlackTentaclesRunning = true;
                return new BlackTentacles(this, 17);
            }

            return new ScorchingRay(9);
        }

        public override BaseAction PickBonusAction()
        {
            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            return new NoAction { Time = BaseAction.ActionTime.Reaction };
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
                Stats.SpellsUsed++;
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

            if (BlackTentaclesRunning)
            {
                BlackTentaclesRunning = false;
                Context.EndEffect(Group, SpellEffectType.BlackTentacles);
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

            if (BlackTentaclesRunning)
            {
                BlackTentaclesRunning = false;
                Context.EndEffect(Group, SpellEffectType.BlackTentacles);
            }
        }
    }
}
