using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Marinyth : BaseCharacter
    {
        public bool ShouldHuntersMark { get; set; } = true;
        public bool HuntersMarkRunning { get; set; }
        public bool FirstRound { get; set; }
        public bool UsedFlurry { get; set; }
        public bool CanFlurry { get; set; }

        public class Longbow : BaseAction
        {
            public Marinyth parent { get; set; }

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

                    if (parent.HuntersMarkRunning)
                        output += " (HM)";

                    return output;
                }
                set { _desc = value; }
            }

            public Longbow()
            {
                Desc = "Longbow";
                Type = ActionType.RangedAttack;
                AttackModifier = 11;
                Modifier = 5;
                TotalToRun = 2;
                IsMagical = true;
            }

            public override bool Hits(BaseCharacter attacker, BaseCharacter target)
            {
                bool result = base.Hits(attacker, target);

                if (parent.CanFlurry)
                {
                    if (!result && !parent.UsedFlurry)
                    {
                        TotalToRun += 1;
                        parent.UsedFlurry = true;
                    }
                }

                return result;
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 2 : 1);

                if (CurrentRunning == 3)
                    damage += Dice.D8(CriticalHit ? 2 : 1);

                if (parent.HuntersMarkRunning)
                {
                    damage += Dice.D6(CriticalHit ? 2 : 1);
                }

                if (_ssThisTurn)
                {
                    damage += 10;
                    _ssThisTurn = false;
                }

                return damage + Modifier;
            }
        }

        public class HuntersMarkActivate : BaseAction
        {
            public HuntersMarkActivate()
            {
                Desc = "Hunter's Mark";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public Marinyth() : base()
        {
            Name = "Marinyth";
            AC = 17;
            InitMod = 8;
            Health = 76;
            MaxHealth = 76;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Priority = HealPriority.Medium;
            MyType = CreatureType.PC;
            BonusActionFirst = false;
            UsedFlurry = false;
            HuntersMarkRunning = false;
            CanFlurry = false;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 4 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 14, Mod = 2, Save = 3 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 8, Mod = -1, Save = -1 });
        }

        public override void Init()
        {
            base.Init();

            HuntersMarkRunning = false;
            FirstRound = true;
            IsHidden = false;
            UsedFlurry = false;
        }

        public override BaseAction PickAction()
        {
            int total = FirstRound ? 3 : 2;
            FirstRound = false;

            return new Longbow { Time = BaseAction.ActionTime.Action, parent = this, TotalToRun = total };
        }

        public override BaseAction PickBonusAction()
        {
            if (ShouldHuntersMark && !HuntersMarkRunning)
            {
                HuntersMarkRunning = true;
                Concentrating = true;
                Stats.SpellsUsed++;
                return new HuntersMarkActivate();
            }

            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();
            UsedFlurry = false;

            if (ShouldHuntersMark && !HuntersMarkRunning)
            {
                BonusActionFirst = true;
            }
            else
            {
                BonusActionFirst = false;
            }

            if (Context.InBrightLight)
                IsHidden = Dice.D100() <= (FirstRound ? 10 : 1);
            else
                IsHidden = Dice.D100() <= (FirstRound ? 40 : 5);
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            HuntersMarkRunning = false;
        }

        public override void OnDeath()
        {
            base.OnDeath();

            HuntersMarkRunning = false;
        }
    }
}
