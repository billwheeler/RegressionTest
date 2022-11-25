using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Fighter : BaseCharacter
    {
        public bool GiantsMightRunning { get; set; }
        public bool UsedGiantsMight { get; set; }

        public bool UsedActionSurge { get; set; } = false;
        public bool UsedSecondWind { get; set; } = false;

        public class OversizedMaul : BaseAction
        {
            public Fighter parent { get; set; }

            private string _desc = "Oversized Maul";
            private bool _gmThisTurn = false;
            private bool _gwmThisTurn = false;
            private readonly bool GreatWeaponMasterEnabled = true;

            public override void PreHit(BaseCharacter attacker, BaseCharacter target)
            {
                _gwmThisTurn = false;

                base.PreHit(attacker, target);

                if (GreatWeaponMasterEnabled)
                {
                    if (ShouldPowerAttack(target.AC, 14, 18))
                    {
                        _gwmThisTurn = true;
                        AttackModifier = 4;
                        parent.Stats.PowerAttacks++;
                    }
                    else
                    {
                        _gwmThisTurn = false;
                        AttackModifier = 9;
                    }
                }
                else
                {
                    _gwmThisTurn = false;
                    AttackModifier = 9;
                }
            }

            public override string Desc
            {
                get
                {
                    string output = _desc;

                    if (_gwmThisTurn)
                        output += " (GWM)";

                    if (_gmThisTurn)
                    {
                        output += " (GM)";
                        _gmThisTurn = false;
                    }

                    return output;
                }
                set { _desc = value; }
            }

            public OversizedMaul()
            {
                Desc = "Oversized Maul";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
                TotalToRun = 2;
                IsMagical = true;
            }

            public override bool Hits(BaseCharacter attacker, BaseCharacter target)
            {
                bool result = base.Hits(attacker, target);

                return result;
            }

            public override int Amount()
            {
                int damage = Dice.D6(CriticalHit ? 8 : 4);

                if (_gwmThisTurn)
                {
                    damage += 10;
                    _gwmThisTurn = false;
                }

                if (!parent.UsedGiantsMight)
                {
                    parent.UsedGiantsMight = true;
                    _gmThisTurn = true;
                    damage += Dice.D6(CriticalHit ? 2 : 1);
                }

                return damage + Modifier;
            }
        }

        public class Warhammer : BaseAction
        {
            private string _desc = "Warhammer";
            private bool _gmThisTurn = false;

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
                Modifier = 5;
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

                return damage + Modifier;
            }
        }

        public class GiantsMightActivate : BaseAction
        {
            public GiantsMightActivate()
            {
                Desc = "Giant's Might";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
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
        }

        public Fighter() : base()
        {
            Name = "Fighter";
            AC = 19;
            Health = 85;
            MaxHealth = 85;
            HealingThreshold = 24;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 0;
            MyType = CreatureType.PC;
            OpportunityAttackChance = 20;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 13, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 10, Mod = 0, Save = 0 });
        }

        public override int AC
        {
            get
            {
                if (GiantsMightRunning)
                    return 19;

                return 21;
            }
        }

        public override void Init()
        {
            base.Init();
            GiantsMightRunning = false;
            UsedGiantsMight = false;
            UsedActionSurge = false;
            UsedSecondWind = false;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();
            if (!GiantsMightRunning)
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
            int total = 2;
            if (!UsedActionSurge)
            {
                total = 4;
                UsedActionSurge = true;
            }

            if (GiantsMightRunning)
                return new OversizedMaul { Time = BaseAction.ActionTime.Action, TotalToRun = total, parent = this };

            return new Warhammer { Time = BaseAction.ActionTime.Action, TotalToRun = total, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (!GiantsMightRunning)
            {
                GiantsMightRunning = true;
                return new GiantsMightActivate();
            }

            if (!UsedSecondWind && Health <= HealingThreshold)
            {
                UsedSecondWind = true;
                int amount = Dice.D10() + 10;
                Heal(amount);
            }

            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;

            if (GiantsMightRunning)
                return new OversizedMaul { Time = BaseAction.ActionTime.Action, TotalToRun = 1, parent = this };

            return new Warhammer { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
        }

        public override void OnDeath()
        {
            base.OnDeath();

            GiantsMightRunning = false;
        }
    }
}
