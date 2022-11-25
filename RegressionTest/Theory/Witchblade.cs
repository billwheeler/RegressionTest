using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Witchblade : BaseCharacter
    {
        public bool UsedActionSurge { get; set; } = false;
        public bool UsedSecondWind { get; set; } = false;

        public bool DamageFeatureRunning { get; set; } = false;
        public bool OncePerTurnDamage { get; set; } = false;

        public class HandCrossbow : BaseAction
        {
            public Witchblade parent { get; set; }

            private string _desc = "Longbow";
            private bool _ssThisTurn = false;
            private readonly bool SharpshooterEnabled = true;

            public override void PreHit(BaseCharacter attacker, BaseCharacter target)
            {
                _ssThisTurn = false;

                base.PreHit(attacker, target);

                if (SharpshooterEnabled)
                {
                    if (ShouldPowerAttack(target.AC, 16, 20))
                    {
                        _ssThisTurn = true;
                        AttackModifier = 6;
                        parent.Stats.PowerAttacks++;
                    }
                    else
                    {
                        _ssThisTurn = false;
                        AttackModifier = 11;
                    }
                }
                else
                {
                    _ssThisTurn = false;
                    AttackModifier = 11;
                }
            }

            public override string Desc
            {
                get
                {
                    string output = _desc;

                    if (_ssThisTurn)
                        output += " (SS)";

                    return output;
                }
                set { _desc = value; }
            }

            public HandCrossbow()
            {
                Desc = "Hand Crossbow";
                Type = ActionType.RangedAttack;
                AttackModifier = 11;
                Modifier = 5;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D6(CriticalHit ? 2 : 1);

                if (_ssThisTurn)
                {
                    damage += 10;
                    _ssThisTurn = false;
                }

                return damage + Modifier;
            }
        }

        public class Longsword : BaseAction
        {
            public Witchblade parent { get; set; }
            public string Description { get; set; }

            public bool UsedFeatureDamage { get; set; } = false;

            public Longsword()
            {
                Desc = "Longsword";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
                IsMagical = true;
            }

            public override string Desc
            {
                get
                {
                    string output = Description;

                    if (UsedFeatureDamage)
                    {
                        output += " (BW)";
                        UsedFeatureDamage = false;
                    }

                    return output;
                }
                set { Description = value; }
            }

            public override int Amount()
            {
                int damage = Dice.D10(CriticalHit ? 2 : 1);

                if (parent.DamageFeatureRunning)
                {
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                    UsedFeatureDamage = true;
                }

                return damage + Modifier;
            }
        }

        public class DamageFeatureActivate : BaseAction
        {
            public DamageFeatureActivate()
            {
                Desc = "Damage Feature";
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

        public Witchblade() : base()
        {
            Name = "Amxikas";
            AC = 17;
            Health = 85;
            MaxHealth = 85;
            HealingThreshold = 24;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 5;
            MyType = CreatureType.PC;
            WarCaster = false;
            OpportunityAttackChance = 60;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 4 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 5 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 14, Mod = 2, Save = 2 });
        }

        public override void Init()
        {
            base.Init();

            UsedActionSurge = false;
            UsedSecondWind = false;
            DamageFeatureRunning = false;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();

            if (!DamageFeatureRunning)
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
            int total = 3;
            if (!UsedActionSurge)
            {
                total = 6;
                UsedActionSurge = true;
            }

            //return new HandCrossbow { Time = BaseAction.ActionTime.Action, TotalToRun = total, parent = this };
            return new Longsword { Time = BaseAction.ActionTime.Action, TotalToRun = total, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (!DamageFeatureRunning)
            {
                DamageFeatureRunning = true;
                Concentrating = true;
                return new DamageFeatureActivate();
            }

            if (!UsedSecondWind && Health <= HealingThreshold)
            {
                UsedSecondWind = true;
                int amount = Dice.D10() + 10;
                Heal(amount);
                return new SecondWindActivate(amount);
            }

            //return new HandCrossbow { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this };
            return new Longsword { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;
            return new Longsword { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };                    
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();
            DamageFeatureRunning = false;
        }

        public override void OnDeath()
        {
            base.OnDeath();
            DamageFeatureRunning = false;
        }
    }
}
