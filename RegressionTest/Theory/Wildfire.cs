using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class WildfireDruid : BaseCharacter
    {
        public bool DidExtraDamage { get; set; } = false;

        public bool ShouldConjure { get; set; } = true;
        public bool ConjureRunning { get; set; } = false;
        public bool ConjureUsed { get; set; } = false;

        public bool ConfusionRunning { get; set; } = false;

        public bool WildfireSummoned { get; set; } = false;
        public bool WildfireSummonedThisTurn { get; set; } = false;

        public int ScorchingRayUses { get; set; } = 0;

        public class Firebolt : BaseAction
        {
            public Firebolt()
            {
                Desc = "Firebolt";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 8;
                Modifier = 0;
                TotalToRun = 1;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D10(CriticalHit ? 4 : 2);
                damage += Dice.D8(CriticalHit ? 2 : 1);
                return damage + Modifier;
            }
        }

        public class ScorchingRayWildfire : BaseAction
        {
            public WildfireDruid parent { get; set; }

            public ScorchingRayWildfire()
            {
                Desc = "Scorching Ray";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 8;
                Modifier = 0;
                TotalToRun = 3;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D6(CriticalHit ? 4 : 2);

                if (parent.DidExtraDamage == false)
                {
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                    parent.DidExtraDamage = true;
                }

                return damage + Modifier;
            }
        }

        public class ConjureWoodlandBeingsActivate : BaseAction
        {
            public ConjureWoodlandBeingsActivate()
            {
                Desc = "Conjure Minor Elementals";
                Type = ActionType.Activate;
                Time = ActionTime.Action;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public class CureWoundsWildfire : SpellAction
        {
            public CureWoundsWildfire()
            {
                Desc = "Cure Wounds";
                Level = SpellLevel.One;
                Type = ActionType.Heal;
                Time = ActionTime.Action;
            }

            public override int Amount()
            {
                int die = (int)Level;
                return Dice.D8(die) + Dice.D8(1) + Modifier;
            }
        }

        public class HealingWordWildfire : SpellAction
        {
            public HealingWordWildfire()
            {
                Desc = "Healing Word";
                Level = SpellLevel.One;
                Type = ActionType.Heal;
                Time = ActionTime.BonusAction;
            }

            public override int Amount()
            {
                int die = (int)Level;
                return Dice.D4(die) + Dice.D8(1) + Modifier;
            }
        }

        public class SummonWildfireSpirit : BaseAction
        {
            public SummonWildfireSpirit()
            {
                Desc = "Summon Wildfire Spirit";
                Type = ActionType.SpellSave;
                Time = ActionTime.Action;
                Ability = AbilityScore.Dexterity;
                HalfDamageOnMiss = false;
                MinTargets = 3;
                MaxTargets = 6;
                DC = 16;
            }

            public override int Amount()
            {
                return Dice.D6(2);
            }
        }

        public class FieryTeleport : BaseAction
        {
            public FieryTeleport()
            {
                Desc = "Fiery Teleport";
                Type = ActionType.SpellSave;
                Time = ActionTime.BonusAction;
                Ability = AbilityScore.Dexterity;
                HalfDamageOnMiss = false;
                MinTargets = 1;
                MaxTargets = 4;
                DC = 16;
            }

            public override int Amount()
            {
                return Dice.D6(1) + 4;
            }
        }

        public class FlameSeed : BaseAction
        {
            public FlameSeed()
            {
                Desc = "Flame Seed";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 8;
                Modifier = 4;
                TotalToRun = 1;
                IsMagical = true;
            }

            public override int Amount()
            {
                return Dice.D6(CriticalHit ? 2 : 1) + Modifier;
            }
        }

        public WildfireDruid() : base()
        {
            Name = "Wildfire";
            AC = 17;
            Health = 75;
            MaxHealth = 75;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = true;
            Priority = HealPriority.High;
            InitMod = 2;
            WarCaster = true;
            MyType = CreatureType.PC;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 12, Mod = 1, Save = 5 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 18, Mod = 4, Save = 8 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 9, Mod = -1, Save = -1 });
        }

        public override void Init()
        {
            base.Init();
            DidExtraDamage = false;
            ConjureRunning = false;
            ConjureUsed = false;
            ConfusionRunning = false;
            WildfireSummoned = false;
            WildfireSummonedThisTurn = false;
            ScorchingRayUses = 2;
        }

        public override void OnNewRound()
        {
            base.OnNewRound();

            DidExtraDamage = false;
            WildfireSummonedThisTurn = false;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();
        }

        public override BaseAction PickAction()
        {
            if (ShouldConjure)
            {
                if (!ConjureUsed && !ConjureRunning)
                {
                    ConjureRunning = true;
                    Concentrating = true;
                    ConjureUsed = true;
                    Stats.SpellsUsed++;
                    Context.ActivateSummons(Group);

                    return new ConjureWoodlandBeingsActivate();
                }
            }
            
            if (!Concentrating && !ConfusionRunning)
            {
                ConfusionRunning = true;
                Stats.SpellsUsed++;
                Concentrating = true;
                return new Confusion(16);
            }

            if (!WildfireSummoned)
            {
                WildfireSummoned = true;
                WildfireSummonedThisTurn = true;
                return new SummonWildfireSpirit();
            }

            if (ScorchingRayUses > 0)
            {
                ScorchingRayUses--;
                Stats.SpellsUsed++;
                return new ScorchingRayWildfire { parent = this, Time = BaseAction.ActionTime.Action };
            }
            
            return new Firebolt { Time = BaseAction.ActionTime.Action };
        }

        public override BaseAction PickBonusAction()
        {
            if (Healer && HealTarget != null)
            {
                Stats.SpellsUsed++;
                return new HealingWordWildfire { Modifier = 4, Level = SpellAction.SpellLevel.One };
            }

            if (WildfireSummoned && !WildfireSummonedThisTurn)
            {
                if (Dice.D100() <= 67)
                    return new FieryTeleport();
                
                return new FlameSeed();
            }

            return new NoAction();
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            if (ConjureRunning)
            {
                ConjureRunning = false;
                Context.DeactivateSummons(Group);
            }

            if (ConfusionRunning)
            {
                ConfusionRunning = false;
                Context.EndEffect(Group, SpellEffectType.Confusion);
            }
        }

        public override void OnDeath()
        {
            base.OnDeath();

            if (ConjureRunning)
            {
                ConjureRunning = false;
                Context.DeactivateSummons(Group);
            }

            if (ConfusionRunning)
            {
                ConfusionRunning = false;
                Context.EndEffect(Group, SpellEffectType.Confusion);
            }
        }
    }
}
