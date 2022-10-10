using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Fighter : BaseCharacter
    {
        public bool UsedGiantsMight { get; set; }

        public bool ShouldRadiantSoul { get; set; } = true;
        public bool RadiantSoulRunning { get; set; }
        public bool RadiantSoulUsed { get; set; }

        public bool UsedActionSurge { get; set; } = false;
        public bool UsedSecondWind { get; set; } = false;

        public class Warhammer : BaseAction
        {
            private string _desc = "Warhammer";
            private bool _gmThisTurn = false;
            private bool _rsThisTurn = false;

            public Fighter parent { get; set; }

            public override string Desc
            {
                get
                {
                    string output = _desc;

                    if (_gmThisTurn)
                    {
                        output += " (GM)";
                        _gmThisTurn = false;
                    }

                    if (_rsThisTurn)
                    {
                        output += " (RS)";
                        _rsThisTurn = false;
                    }

                    return output;
                }
                set { _desc = value; }
            }

            public Warhammer()
            {
                Desc = "Warhammer";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 9;
                Modifier = 7;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 2 : 1);

                if (!parent.UsedGiantsMight)
                {
                    parent.UsedGiantsMight = true;
                    _gmThisTurn = true;
                    damage += Dice.D6(CriticalHit ? 2 : 1);
                }

                if (parent.RadiantSoulRunning && !parent.RadiantSoulUsed)
                {
                    parent.RadiantSoulUsed = true;
                    _rsThisTurn = true;
                    damage += 4;
                }

                return damage + Modifier;
            }
        }

        public class ShieldBash : BaseAction
        {
            private string _desc = "Shield Bash";
            private bool _gmThisTurn = false;
            private bool _rsThisTurn = false;

            public Fighter parent { get; set; }

            public override string Desc
            {
                get
                {
                    string output = _desc;

                    if (_gmThisTurn)
                    {
                        output += " (GM)";
                        _gmThisTurn = false;
                    }

                    if (_rsThisTurn)
                    {
                        output += " (RS)";
                        _rsThisTurn = false;
                    }

                    return output;
                }
                set { _desc = value; }
            }

            public ShieldBash()
            {
                Desc = "Shield Bash";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 9;
                Modifier = 7;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D4(CriticalHit ? 2 : 1);

                if (!parent.UsedGiantsMight)
                {
                    parent.UsedGiantsMight = true;
                    _gmThisTurn = true;
                    damage += Dice.D6(CriticalHit ? 2 : 1);
                }

                if (parent.RadiantSoulRunning && !parent.RadiantSoulUsed)
                {
                    parent.RadiantSoulUsed = true;
                    _rsThisTurn = true;
                    damage += 4;
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
            AC = 20;
            Health = 85;
            MaxHealth = 85;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 0;
            MyType = CreatureType.PC;
            OpportunityAttackChance = 10;

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
            UsedGiantsMight = false;
            UsedActionSurge = false;
            UsedSecondWind = false;
            RadiantSoulRunning = false;
            RadiantSoulUsed = false;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();

            UsedGiantsMight = false;
            RadiantSoulUsed = false;
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

            return new Warhammer { Time = BaseAction.ActionTime.Action, TotalToRun = total, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (!UsedSecondWind && Health <= HealingThreshold)
            {
                UsedSecondWind = true;
                int amount = Dice.D10() + 10;
                Heal(amount);
            }

            return new ShieldBash { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;

            return new Warhammer { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
        }

        public override void OnDeath()
        {
            RadiantSoulRunning = false;

            base.OnDeath();
        }
    }
}
