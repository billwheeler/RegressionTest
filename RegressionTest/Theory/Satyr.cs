using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Satyr : BaseCharacter
    {
        public bool ShepherdSummons { get; set; } = false;

        public class Shortbow : BaseAction
        {
            public Shortbow()
            {
                Desc = "Shortbow";
                Type = ActionType.MeleeAttack;
                AttackModifier = 5;
                Modifier = 3;
            }

            public override int Amount()
            {
                return Dice.D6(CriticalHit ? 2 : 1) + Modifier;
            }
        }

        public Satyr()
        {
            Name = "Erven";
            AC = 14;
            Health = 31;
            MaxHealth = 31;
            HealingThreshold = 12;
            Group = Team.TeamOne;
            Priority = HealPriority.Low;
            InitMod = 3;
            MyType = CreatureType.Summon;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 12, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 11, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 12, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 14, Mod = 2, Save = 2 });
        }

        public override void Init()
        {
            base.Init();
            
            if (ShepherdSummons)
            {
                Health = 45;
                MaxHealth = 45;
            }
            else
            {
                Health = 31;
                MaxHealth = 31;
            }
        }

        public override BaseAction PickAction()
        {
            return new Shortbow { Time = BaseAction.ActionTime.Action, IsMagical = ShepherdSummons ? true : false };
        }
    }
}
