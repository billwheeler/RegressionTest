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
            public HexBard parent = null;
            public bool UseBoomingBlade = true;
            protected string _desc = string.Empty;

            public Rapier()
            {
                Desc = "Rapier";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 8;
                Modifier = 6;
                TotalToRun = UseBoomingBlade ? 1 : 2;
                IsMagical = true;
            }

            public override string Desc
            {
                get
                {
                    string output = _desc;

                    if (UseBoomingBlade)
                    {
                        output += " (BB)";
                    }

                    return output;
                }
                set { _desc = value; }
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 2 : 1);

                if (parent.HexRunning)
                {
                    damage += Dice.D6(CriticalHit ? 2 : 1);
                }

                if (Time == ActionTime.Action && UseBoomingBlade)
                {
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                    int percentageEnemyMoves = (Time == ActionTime.Reaction) ? 95 : 25;
                    if (Dice.D100() <= percentageEnemyMoves)
                    {
                        damage += Dice.D8(2);
                    }
                }
                else if (Time == ActionTime.Action && parent.InspiriationUses > 0)
                {
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                    parent.InspiriationUses--;
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
                AttackModifier = 8;
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

        public class HexActivate : BaseAction
        {
            public HexActivate()
            {
                Desc = "Hex";
                Type = ActionType.Activate;
                Time = ActionTime.Action;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public int InspiriationUses { get; set; } = 0;
        public int ShieldUses { get; set; } = 0;

        public bool ShouldPhantasmalKiller { get; set; } = false;
        public bool DidPhantasmalKiller { get; set; }
        public bool PhantasmalKillerRunning { get; set; }

        public bool ShouldHex { get; set; } = false;
        public bool HexRunning { get; set; }

        public bool DidPsychicLance { get; set; }
        public bool DidHypnoticPattern { get; set; }
        public bool HypnoticPatternRunning { get; set; }
        public bool CastLevelledSpellThisTurn { get; set; } = false;

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
            WarCaster = true;
            OpportunityAttackChance = 20;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 8, Mod = -1, Save = 3 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 18, Mod = 4, Save = 8 });
        }

        public override void Init()
        {
            base.Init();

            DidPsychicLance = false;
            DidPhantasmalKiller = false;
            PhantasmalKillerRunning = false;
            DidHypnoticPattern = false;
            HypnoticPatternRunning = false;
            CastLevelledSpellThisTurn = false;
            ShieldUses = 4;
            InspiriationUses = 4;
        }

        public override void OnNewEncounter()
        {
            base.OnNewEncounter();
            
            // armor of agathys
            SetTempHitPoints(20);
            Stats.SpellsUsed++;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();
            CastLevelledSpellThisTurn = false;
        }

        public override BaseAction PickAction()
        {
            if (!Concentrating && ShouldPhantasmalKiller && !DidPhantasmalKiller)
            {
                List<int> targets = Context.PickEnemies(Group);
                if (targets.Count > 0)
                {
                    int index = targets.First();
                    var enemy = Context.Characters[index];
                    if (enemy != null && enemy.HighValueTarget && !enemy.HasUndesirableEffect())
                    {
                        DidPhantasmalKiller = true;
                        Concentrating = true;
                        PhantasmalKillerRunning = true;
                        Stats.SpellsUsed++;
                        CastLevelledSpellThisTurn = true;
                        return new PhantasmalKiller(this, 16);
                    }
                }
            }

            if (!Concentrating && !ShouldHex && !HexRunning)
            {
                Concentrating = true;
                HexRunning = true;
                Stats.SpellsUsed++;
                CastLevelledSpellThisTurn = true;
                return new HexActivate();
            }

            if (!Concentrating && !DidHypnoticPattern)
            {
                DidHypnoticPattern = true;
                Concentrating = true;
                HypnoticPatternRunning = true;
                Stats.SpellsUsed++;
                CastLevelledSpellThisTurn = true;
                return new HypnoticPattern(16);
            }

            if (!DidPhantasmalKiller && !DidPsychicLance)
            {
                DidPsychicLance = true;
                Stats.SpellsUsed++;
                CastLevelledSpellThisTurn = true;
                return new PsychicLance(16);
            }

            bool shouldDodge = false;
            if (Health < 20)
                shouldDodge = true;

            if (shouldDodge)
            {
                IsDodging = true;
                return new DodgeAction { Time = BaseAction.ActionTime.Action };
            }

            return new Rapier { UseBoomingBlade = Dice.D100() <= 67, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (!CastLevelledSpellThisTurn && Healer && HealTarget != null)
            {
                Stats.SpellsUsed++;
                return new HealingWord { Modifier = 4, Level = SpellAction.SpellLevel.One };
            }

            /*if (InspiriationUses > 0)
            {
                BaseCharacter ally = Context.PickRandomTeammate(Group, ID, false);
                if (ally != null)
                {
                    ally.ApplyEffect(new SpellEffect
                    {
                        DC = 0,
                        Type = SpellEffectType.Inspired,
                        Active = true
                    }, this);

                    return new Inspiration(ally.Name) { Time = BaseAction.ActionTime.BonusAction };
                }
            }*/

            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;
            return new Rapier { Time = BaseAction.ActionTime.Reaction, UseBoomingBlade = true, parent = this };
        }

        public override void PreHitCalc(int attackRoll, int modifier, bool potentiallyPowerful, bool criticalHit)
        {
            bool shouldCastShield = false;

            if (!HasShieldRunning && ShieldUses > 0)
            {
                if (attackRoll + modifier > AC)
                {
                    if (Health < 25)
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

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            if (PhantasmalKillerRunning)
            {
                PhantasmalKillerRunning = false;
                Context.EndEffect(Group, SpellEffectType.PhantasmalKiller);
            }

            if (HypnoticPatternRunning)
            {
                HypnoticPatternRunning = false;
                Context.EndEffect(Group, SpellEffectType.HypnoticPattern);
            }
        }

        public override void OnDeath()
        {
            base.OnDeath();

            if (PhantasmalKillerRunning)
            {
                PhantasmalKillerRunning = false;
                Context.EndEffect(Group, SpellEffectType.PhantasmalKiller);
            }

            if (HypnoticPatternRunning)
            {
                HypnoticPatternRunning = false;
                Context.EndEffect(Group, SpellEffectType.HypnoticPattern);
            }
        }
    }
}

