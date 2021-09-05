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
        public int NumSuperiorityDice { get; set; }

        public class GlaivePM : BaseAction
        {
            public Fighter parent { get; set; }

            private string _desc = "Glaive";
            private bool _gwnThisTurn = false;
            private bool _sdThisTurn = false;

            /*public override void PreHit(BaseCharacter attacker, BaseCharacter target)
            {
                base.PreHit(attacker, target);
                double percentage = Util.Remap(target.AC, 12, 17, 100, 0);

                if (Dice.D100() < percentage)
                {
                    _gwnThisTurn = true;
                    AttackModifier = 4;
                }
                else
                {
                    _gwnThisTurn = false;
                    AttackModifier = 9;
                }
            }*/

            public override string Desc
            {
                get
                {
                    string output = _desc;

                    if (_gwnThisTurn)
                        output += " (GWN)";

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
            }

            public override int Amount()
            {
                int damage = 0;

                damage += (Time == ActionTime.Action) ?
                    Dice.D10(CriticalHit ? 2 : 1) :
                    Dice.D4(CriticalHit ? 2 : 1);

                if (parent.NumSuperiorityDice > 0 && !parent.UsedSuperiorityDice)
                {
                    parent.NumSuperiorityDice--;
                    parent.UsedSuperiorityDice = true;
                    _sdThisTurn = true;
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                }

                if (_gwnThisTurn)
                {
                    damage += 10;
                    _gwnThisTurn = false;
                }

                return damage + Modifier;
            }
        }

        public Fighter()
        {
            Name = "Murie";
            AC = 18;
            Health = 104;
            MaxHealth = 104;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 0;
            MyType = CreatureType.PC;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 12, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 14, Mod = 2, Save = 2 });
        }

        public override void Init()
        {
            base.Init();
            UsedSuperiorityDice = false;
            NumSuperiorityDice = 5;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();
            UsedSuperiorityDice = false;
        }

        public override BaseAction PickAction()
        {
            return new GlaivePM { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            return new GlaivePM { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this };
        }

        public override void OnDeath()
        {
            base.OnDeath();
        }
    }
}
