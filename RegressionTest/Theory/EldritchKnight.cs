using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class EldritchKnight : BaseCharacter
    {
        public bool CanShadowBlade { get; set; } = true;
        public bool ShadowBladeRunning { get; set; } = false;

        public bool UsedActionSurge { get; set; } = false;
        public bool UsedSecondWind { get; set; } = false;

        public bool UsedBoomingBladeThisRound { get; set; } = false;
        public int ShieldUses { get; set; } = 0;

        public class Scimitar : BaseAction
        {
            public EldritchKnight parent { get; set; }

            public Scimitar()
            {
                Desc = "Scimitar";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D6(CriticalHit ? 2 : 1);

                bool shouldBoomBoom = false;
                if (Time == ActionTime.Reaction)
                    shouldBoomBoom = true;

                if (Time == ActionTime.Action && !parent.UsedBoomingBladeThisRound)
                {
                    parent.UsedBoomingBladeThisRound = true;
                    shouldBoomBoom = true;
                }

                if (shouldBoomBoom)
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

        public class ShadowBlade : BaseAction
        {
            public EldritchKnight parent { get; set; }

            public ShadowBlade()
            {
                Desc = "Shadow Blade";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
                IsMagical = true;
            }

            public override int Amount()
            {
                return Dice.D8(CriticalHit ? 4 : 2) + Modifier;
            }
        }

        public class ShadowBladeActivate : BaseAction
        {
            public ShadowBladeActivate()
            {
                Desc = "Shadow Blade";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public class SecondWindActivate : BaseAction
        {
            public SecondWindActivate(int amountHealed = 0)
            {
                Desc = $"Second Wind - healed {amountHealed}hp.";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public EldritchKnight() : base()
        {
            Name = "Amxikas";
            AC = 18;
            Health = 85;
            MaxHealth = 85;
            HealingThreshold = 24;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 5;
            MyType = CreatureType.PC;
            WarCaster = true;
            OpportunityAttackChance = 20;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 4 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 5 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 12, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 13, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 8, Mod = -1, Save = -1 });
        }

        public override void Init()
        {
            base.Init();
            
            CanShadowBlade = true;
            ShadowBladeRunning = false;
            UsedActionSurge = false;
            UsedSecondWind = false;
            ShieldUses = 4;
            UsedBoomingBladeThisRound = false;
        }

        public override void OnNewRound()
        {
            UsedBoomingBladeThisRound = false;
            base.OnNewRound();
        }

        public override BaseAction PickAction()
        {
            int total = 2;
            if (!UsedActionSurge)
            {
                total = 4;
                UsedActionSurge = true;
            }

            if (ShadowBladeRunning)
            {
                var rollType = Context.InBrightLight ? AbilityRoll.Normal : AbilityRoll.Advantage;
                return new ShadowBlade { Time = BaseAction.ActionTime.Action, TotalToRun = total, RollType = rollType, parent = this };
            }

            //if (Dice.D100() <= 10)
            //    return new Longbow { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this };

            return new Scimitar { Time = BaseAction.ActionTime.Action, TotalToRun = total, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (CanShadowBlade && !ShadowBladeRunning)
            {
                CanShadowBlade = true;
                ShadowBladeRunning = true;
                Concentrating = true;
                Stats.SpellsUsed++;

                return new ShadowBladeActivate();
            }

            if (!UsedSecondWind && Health <= HealingThreshold)
            {
                UsedSecondWind = true;
                int amount = Dice.D10() + 10;
                Heal(amount);
                return new SecondWindActivate(amount);
            }

            return new Scimitar { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;

            if (ShadowBladeRunning)
            {
                var rollType = Context.InBrightLight ? AbilityRoll.Normal : AbilityRoll.Advantage;
                return new ShadowBlade { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, RollType = rollType, parent = this };
            }

            return new Scimitar { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
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
                Stats.SpellsUsed++;
            }
        }

        public override void OnNewTurn()
        {
            if (CanShadowBlade && !ShadowBladeRunning)
            {
                BonusActionFirst = true;
            }
            else
            {
                BonusActionFirst = false;
            }
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            ShadowBladeRunning = false;
        }

        public override void OnDeath()
        {
            base.OnDeath();

            ShadowBladeRunning = false;
        }
    }
}
