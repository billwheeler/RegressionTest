using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Bladesinger : BaseCharacter
    {
        public class Rapier : BaseAction
        {
            public Bladesinger parent { get; set; }
            public readonly bool CanUseBoomingBlade = true;

            public Rapier()
            {
                Desc = "Rapier";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 8;
                Modifier = 4;
                TotalToRun = 2;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 2 : 1);

                bool shouldBoomBoom = false;
                if (CanUseBoomingBlade)
                {
                    if (Time == ActionTime.Action && !parent.BoomingBladeThisTurn)
                    {
                        shouldBoomBoom = true;
                    }

                    if (Time == ActionTime.Reaction)
                    {
                        shouldBoomBoom = true;
                    }
                }

                if (shouldBoomBoom)
                {
                    parent.BoomingBladeThisTurn = true;
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

        public class Shadowblade : BaseAction
        {
            public Bladesinger parent { get; set; }
            public readonly bool CanUseBoomingBlade = false;

            public Shadowblade()
            {
                Desc = "Shadow Blade";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 7;
                Modifier = 3;
                TotalToRun = 2;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 6 : 3);

                return damage + Modifier;
            }
        }

        public class BladesongActivate : BaseAction
        {
            public BladesongActivate()
            {
                Desc = "Bladesong";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
                IsMagical = true;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public class ShadowbladeActivate : BaseAction
        {
            public ShadowbladeActivate()
            {
                Desc = "Shadow Blade";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
                IsMagical = true;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public int ShieldUses { get; set; } = 0;

        public bool ShouldShadowBlade { get; set; } = false;

        public bool ShadowbladeRunning { get; set; }
        public bool HypnoticPatternRunning { get; set; }
        public bool SynapticStaticRunning { get; set; }

        public bool BladesongRunning { get; set; } = false;
        public bool BladesongUsed { get; set; } = false;

        public bool BoomingBladeThisTurn { get; set; } = false;

        public override int AC
        {
            get
            {
                if (BladesongRunning)
                    return 19;

                return 15;
            }
        }

        public Bladesinger() : base()
        {
            Name = "Bladesinger";
            InitMod = 3;
            Health = 65;
            MaxHealth = 65;
            Group = Team.TeamOne;
            HealingThreshold = 24;
            Priority = HealPriority.Medium;
            MyType = CreatureType.PC;
            Healer = false;
            WarCaster = true;
            OpportunityAttackChance = 20;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 18, Mod = 4, Save = 8 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 4 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 9, Mod = -1, Save = -1 });
        }

        public override void Init()
        {
            base.Init();

            SynapticStaticRunning = false;
            ShadowbladeRunning = false;
            HypnoticPatternRunning = false;
            BoomingBladeThisTurn = false;
            BladesongRunning = false;
            BladesongUsed = false;

            ActiveEffects[SpellEffectType.Bladesong].Active = false;
        }

        public override void OnNewEncounter()
        {
            base.OnNewEncounter();
            ShieldUses = 4;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();
            BoomingBladeThisTurn = false;

            if (!BladesongRunning)
            {
                BonusActionFirst = true;
            }
            else if (ShouldShadowBlade && !ShadowbladeRunning)
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
            if (!Context.AnyoneHaveEffect(Group, SpellEffectType.Turned))
            {
                if (!SynapticStaticRunning)
                {
                    Stats.SpellsUsed++;
                    SynapticStaticRunning = true;
                    return new SynapticStatic(16);
                }
            }

            if (!ShouldShadowBlade && !Concentrating && !HypnoticPatternRunning)
            {
                Stats.SpellsUsed++;
                Concentrating = true;
                HypnoticPatternRunning = true;
                return new HypnoticPattern(16);
            }

            if (ShadowbladeRunning)
            {
                var rollType = Context.InBrightLight ? AbilityRoll.Normal : AbilityRoll.Advantage;
                return new Shadowblade { Time = BaseAction.ActionTime.Action, TotalToRun = 2, RollType = rollType, parent = this };
            }

            return new Rapier { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (!BladesongRunning)
            {
                BladesongRunning = true;
                BladesongUsed = true;
                ActiveEffects[SpellEffectType.Bladesong].Active = true;
                return new BladesongActivate();
            }
            else if (ShouldShadowBlade && !ShadowbladeRunning && !Concentrating)
            {
                Stats.SpellsUsed++;
                ShadowbladeRunning = true;
                Concentrating = true;
                return new ShadowbladeActivate();
            }

            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;

            if (ShadowbladeRunning)
            {
                var rollType = Context.InBrightLight ? AbilityRoll.Normal : AbilityRoll.Advantage;
                return new Shadowblade { Time = BaseAction.ActionTime.Action, TotalToRun = 1, RollType = rollType, parent = this };
            }

            return new Rapier { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            if (HypnoticPatternRunning)
            {
                HypnoticPatternRunning = false;
                Concentrating = false;
            }

            if (ShadowbladeRunning)
            {
                ShadowbladeRunning = false;
                Concentrating = false;
            }
        }

        public override void OnDeath()
        {
            base.OnDeath();

            if (HypnoticPatternRunning)
            {
                HypnoticPatternRunning = false;
                Concentrating = false;
            }

            if (ShadowbladeRunning)
            {
                ShadowbladeRunning = false;
                Concentrating = false;
            }

            if (BladesongRunning)
            {
                ActiveEffects[SpellEffectType.Bladesong].Active = false;
                BladesongRunning = false;
            }
        }
    }
}

