using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Fighter : BaseCharacter
    {
        public int SuperiorityDice { get; set; }
        public bool UsedSuperiorityDice { get; set; }

        public bool ShouldRadiantSoul { get; set; } = true;
        public bool RadiantSoulRunning { get; set; }
        public bool RadiantSoulUsed { get; set; }

        public bool UsedActionSurge { get; set; } = false;
        public bool UsedSecondWind { get; set; } = false;

        public GlaivePM Glaive { get; set; }

        public class GlaivePM : BaseAction
        {
            public Fighter parent { get; set; }

            private string _desc = "Glaive";
            private bool _paThisTurn = false;
            private bool _sdThisTurn = false;
            private bool _rsThisTurn = false;
            private readonly bool PowerAttackEnabled = true;

            public override void PreHit(BaseCharacter attacker, BaseCharacter target)
            {
                base.PreHit(attacker, target);

                if (PowerAttackEnabled)
                {
                    if (ShouldPowerAttack(target.AC, 13, 18))
                    {
                        _paThisTurn = true;
                        AttackModifier = 4;
                    }
                    else
                    {
                        _paThisTurn = false;
                        AttackModifier = 9;
                    }
                }
                else
                {
                    _paThisTurn = false;
                    AttackModifier = 9;
                }
            }

            public override string Desc
            {
                get
                {
                    string output = _desc;

                    if (_paThisTurn)
                    {
                        output += " (GWM)";
                    }

                    if (_sdThisTurn)
                    {
                        output += " (SD)";
                        _sdThisTurn = false;
                    }

                    if (_rsThisTurn)
                    {
                        output += " (RS)";
                        _sdThisTurn = false;
                    }

                    return output;
                }
                set { _desc = value; }
            }

            public GlaivePM()
            {
                Desc = "Glaive";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Time != ActionTime.BonusAction ?
                    Dice.D10(CriticalHit ? 2 : 1) :
                    Dice.D4(CriticalHit ? 2 : 1);

                if (!parent.UsedSuperiorityDice && parent.SuperiorityDice > 0)
                {
                    parent.UsedSuperiorityDice = true;
                    parent.SuperiorityDice--;
                    _sdThisTurn = true;
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                }

                if (parent.RadiantSoulRunning && !parent.RadiantSoulUsed)
                {
                    parent.RadiantSoulUsed = true;
                    _rsThisTurn = true;
                    damage += 4;
                }

                if (_paThisTurn)
                {
                    parent.Stats.PowerAttacks++;
                    damage += 10;
                }

                return damage + Modifier;
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

        public Fighter() : base()
        {
            Name = "Fighter";
            AC = 18;
            Health = 85;
            MaxHealth = 85;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 0;
            MyType = CreatureType.PC;
            OpportunityAttackChance = 67;

            Glaive = new GlaivePM { parent = this };

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 13, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 10, Mod = 0, Save = 0 });
        }

        public override void Init()
        {
            base.Init();
            SuperiorityDice = 5;
            UsedSuperiorityDice = false;
            UsedActionSurge = false;
            UsedSecondWind = false;
            RadiantSoulRunning = false;
            RadiantSoulUsed = false;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();

            UsedSuperiorityDice = false;
            RadiantSoulUsed = false;

            Glaive.CurrentRunning = 0;
            Glaive.CurrentHits = 0;
            Glaive.HasCritted = false;
        }

        public override BaseAction PickAction()
        {
            int total = 2;
            if (!UsedActionSurge)
            {
                if (ShouldRadiantSoul)
                {
                    RadiantSoulRunning = true;
                }
                else
                {
                    total = 4;
                }
                UsedActionSurge = true;
            }

            Glaive.Time = BaseAction.ActionTime.Action;
            Glaive.TotalToRun = total;
            return Glaive;
        }

        public override BaseAction PickBonusAction()
        {
            if (!UsedSecondWind && Health <= HealingThreshold)
            {
                UsedSecondWind = true;
                int amount = Dice.D10() + 10;
                Heal(amount);
            }

            Glaive.Time = BaseAction.ActionTime.BonusAction;
            Glaive.TotalToRun = 1;
            return Glaive;
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;

            Glaive.Time = BaseAction.ActionTime.Reaction;
            Glaive.TotalToRun = 1;
            return Glaive;
        }

        public override void OnDeath()
        {
            RadiantSoulRunning = false;

            base.OnDeath();
        }
    }
}
