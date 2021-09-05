using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Ranger : BaseCharacter
    {
        public bool HuntersMarkRunning { get; set; }
        public bool PlanarWarriorUsed { get; set; }

        public class Longbow : BaseAction
        {
            public Ranger parent { get; set; }

            private string _desc = "Longbow";
            private bool _planarThisTurn = false;
            private bool _ssThisTurn = false;

            /*public override void PreHit(BaseCharacter attacker, BaseCharacter target)
            {
                base.PreHit(attacker, target);
                double percentage = Util.Remap(target.AC, 14, 19, 100, 0);

                if (Dice.D100() < percentage)
                {
                    _ssThisTurn = true;
                    AttackModifier = 6;
                }
                else
                {
                    _ssThisTurn = false;
                    AttackModifier = 11;
                }
            }*/
            
            public override string Desc { 
                get
                {
                    string output = _desc;

                    if (_ssThisTurn)
                        output += " (SS)";

                    if (_planarThisTurn)
                        output += " (PW)";

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
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 2 : 1);

                if (parent.HuntersMarkRunning)
                {
                    damage += Dice.D6(CriticalHit ? 2 : 1);
                }

                if (!parent.PlanarWarriorUsed && Dice.D100() <= 80)
                {
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                    _planarThisTurn = true;
                    parent.PlanarWarriorUsed = true;
                }
                else
                {
                    _planarThisTurn = false;
                }

                if (_ssThisTurn)
                {
                    damage += 10;
                    _ssThisTurn = false;
                }

                return damage + Modifier;
            }
        }

        public class PlanarWarriorActivate : BaseAction
        {
            public PlanarWarriorActivate()
            {
                Desc = "Planar Warrior";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
            }

            public override int Amount()
            {
                return 0;
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

        public Ranger()
        {
            Name = "Marinyth";
            AC = 17;
            InitMod = 5;
            Health = 84;
            MaxHealth = 84;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Priority = HealPriority.Medium;
            MyType = CreatureType.PC;
            BonusActionFirst = false;

            PlanarWarriorUsed = false;
            HuntersMarkRunning = false;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 4 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 14, Mod = 2, Save = 6 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 12, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 8, Mod = -1, Save = -1 });
        }

        public override void Init()
        {
            base.Init();
            PlanarWarriorUsed = false;
            HuntersMarkRunning = false;
        }

        public override BaseAction PickAction()
        {
            return new Longbow { Time = BaseAction.ActionTime.Action, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (!HuntersMarkRunning)
            {
                HuntersMarkRunning = true;
                Concentrating = true;
                return new HuntersMarkActivate();
            }

            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override void OnNewRound()
        {
            base.OnNewRound();

            PlanarWarriorUsed = false;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();

            if (!HuntersMarkRunning)
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

            HuntersMarkRunning = false;
        }

        public override void OnDeath()
        {
            base.OnDeath();

            HuntersMarkRunning = false;
        }
    }
}
