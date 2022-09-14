using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Fighter : BaseCharacter
    {
        public bool UsedSuperiorityDice { get; set; }
        public bool RadiantSoulRunning { get; set; }
        public bool UsesShield { get; set; } = false;

        public class GlaivePM : BaseAction
        {
            public Fighter parent { get; set; }

            private string _desc = "Glaive";
            private bool _paThisTurn = false;
            private bool _sdThisTurn = false;
            private readonly bool PowerAttackEnabled = true;

            public override void PreHit(BaseCharacter attacker, BaseCharacter target)
            {
                base.PreHit(attacker, target);

                if (PowerAttackEnabled)
                {
                    if (ShouldPowerAttack(target.AC, 12, 17))
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
                        output += " (GWM)";

                    if (_sdThisTurn)
                    {
                        output += " (SD)";
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
                int damage = CurrentRunning < 4 ?
                    Dice.D10(CriticalHit ? 2 : 1) :
                    Dice.D4(CriticalHit ? 2 : 1);

                if (parent.RadiantSoulRunning && !parent.UsedSuperiorityDice)
                {
                    parent.UsedSuperiorityDice = true;
                    _sdThisTurn = true;
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                }

                if (_paThisTurn)
                {
                    damage += 10;
                }

                return damage + Modifier;
            }
        }

        public class Warhammer : BaseAction
        {
            public Fighter parent { get; set; }

            private string _desc = "Warhammer";
            private bool _sdThisTurn = false;

            public override string Desc
            {
                get
                {
                    string output = _desc;

                    if (_sdThisTurn)
                    {
                        output += " (SD)";
                        _sdThisTurn = false;
                    }

                    return output;
                }
                set { _desc = value; }
            }

            public Warhammer()
            {
                Desc = "Warhammer";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 7;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 2 : 1);

                if (parent.RadiantSoulRunning && !parent.UsedSuperiorityDice)
                {
                    parent.UsedSuperiorityDice = true;
                    _sdThisTurn = true;
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                }

                return damage + Modifier + 4;
            }
        }

        public class ShieldBash : BaseAction
        {
            public Fighter parent { get; set; }

            public ShieldBash()
            {
                Desc = "Shield Bash";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
            }
            
            public override int Amount()
            {
                return Dice.D4(CriticalHit ? 2 : 1) + Modifier + 4;
            }
        }

        public class RadiantSoulActivate : BaseAction
        {
            public RadiantSoulActivate()
            {
                Desc = "Radiant Soul";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public Fighter()
        {
            Name = "Angela";
            AC = UsesShield ? 20 : 18;
            Health = 85;
            MaxHealth = 85;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 0;
            MyType = CreatureType.PC;
            OpportunityAttackChance = 40;

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
            RadiantSoulRunning = false;
            UsedSuperiorityDice = false;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();

            UsedSuperiorityDice = false;

            if (!RadiantSoulRunning)
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
            if (UsesShield)
                return new Warhammer { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this };            
            else
                return new GlaivePM { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (!RadiantSoulRunning)
            {
                RadiantSoulRunning = true;
                return new RadiantSoulActivate { Time = BaseAction.ActionTime.BonusAction };
            }

            if (UsesShield)
            {
                return new ShieldBash { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this };
            }

            return new GlaivePM { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            if (UsesShield)
                return new Warhammer { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
            else
                return new GlaivePM { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
        }

        public override void OnDeath()
        {
            RadiantSoulRunning = false;
            base.OnDeath();
        }
    }
}
