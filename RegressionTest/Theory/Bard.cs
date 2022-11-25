using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Bard : BaseCharacter
    {
        public bool PhantasmalKillerRunning { get; set; } = false;

        public class Rapier : BaseAction
        {
            public readonly bool CanUseBoomingBlade = false;

            public Rapier()
            {
                Desc = "Rapier";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 6;
                Modifier = 2;
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
                        damage += Dice.D8(2);
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
                return Dice.D8(CriticalHit ? 4 : 2) + Modifier;
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
                Damageless = true;

                EffectToApply = new SpellEffect
                {
                    Name = "Unsettling Words",
                    Type = SpellEffectType.UnsettlingWords
                };
            }
        }

        public class Inspiration : BaseAction
        {
            public Inspiration(string target)
            {
                Desc = $"Inspiration, targets {target}";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
                Damageless = true;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public class AnimateObjectActivate : BaseAction
        {
            public AnimateObjectActivate()
            {
                Desc = "Animate Object";
                Type = ActionType.Activate;
                Time = ActionTime.Action;
                Damageless = true;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public bool ShouldPhantasmalKiller { get; set; } = false;

        public int ShieldUses { get; set; } = 0;
        public bool DidBigSpell { get; set; }
        public bool HypnoticPatternRunning { get; set; }

        public Bard() : base()
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
            OpportunityAttackChance = 5;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 12, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 8, Mod = -1, Save = 3 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 18, Mod = 4, Save = 8 });
        }

        public override void Init()
        {
            base.Init();

            DidBigSpell = false;
            HypnoticPatternRunning = false;
            PhantasmalKillerRunning = false;
        }

        public override void OnNewEncounter()
        {
            base.OnNewEncounter();
            SetTempHitPoints(10);
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
                if (ShouldPhantasmalKiller)
                {
                    DidBigSpell = true;
                    PhantasmalKillerRunning = true;
                    Concentrating = true;
                    Stats.SpellsUsed++;
                    return new PhantasmalKiller(this, 16);
                }
                else
                {
                    DidBigSpell = true;
                    Stats.SpellsUsed++;
                    return new SynapticStatic(16);
                }
            }
            
            if (!Concentrating)
            {
                Concentrating = true;
                HypnoticPatternRunning = true;
                Stats.SpellsUsed++;
                return new HypnoticPattern(16);
            }

            bool shouldDodge = false;
            if (Health < 25)
                shouldDodge = true;

            if (shouldDodge)
            {
                IsDodging = true;
                return new DodgeAction { Time = BaseAction.ActionTime.Action };
            }

            if (Dice.D100() <= 10)
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
                Stats.SpellsUsed++;
                return new HealingWord { Modifier = 4, Level = SpellAction.SpellLevel.One };
            }

            BaseCharacter ally = Context.PickRandomTeammate(Group, ID, false);
            if (ally != null)
            {
                ally.ApplyEffect(new SpellEffect
                {
                    DC = 0,
                    Type = SpellEffectType.Inspired,
                    Active = true,
                }, this);

                return new Inspiration(ally.Name) { Time = BaseAction.ActionTime.BonusAction };
            }

            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;
            return new Rapier { Time = BaseAction.ActionTime.Reaction };
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            if (HypnoticPatternRunning)
            {
                HypnoticPatternRunning = false;
                Context.EndEffect(Group, SpellEffectType.HypnoticPattern);
            }

            if (PhantasmalKillerRunning)
            {
                PhantasmalKillerRunning = false;
                Context.EndEffect(Group, SpellEffectType.PhantasmalKiller);
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

            if (PhantasmalKillerRunning)
            {
                PhantasmalKillerRunning = false;
                Context.EndEffect(Group, SpellEffectType.PhantasmalKiller);
            }
        }
    }
}

