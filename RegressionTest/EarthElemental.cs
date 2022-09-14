using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class EarthElemental : BaseCharacter
    {
        public bool ShepherdSummons { get; set; } = false;

        public class Slam : BaseAction
        {
            public Slam()
            {
                Desc = "Slam";
                Type = ActionType.MeleeAttack;
                AttackModifier = 8;
                Modifier = 5;
            }

            public override int Amount()
            {
                return Dice.D8(CriticalHit ? 4 : 2) + Modifier;
            }
        }

        public EarthElemental()
        {
            Name = "Elly";
            AC = 17;
            Health = ShepherdSummons ? 150 : 126;
            MaxHealth = ShepherdSummons ? 150 : 126;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Priority = HealPriority.Dont;
            InitMod = -1;
            MyType = CreatureType.Summon;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 20, Mod = 5, Save = 5 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 8, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 20, Mod = 5, Save = 5 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 5, Mod = -3, Save = -3 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 5, Mod = -3, Save = -3 });
        }

        public override BaseAction PickAction()
        {
            return new Slam { Time = BaseAction.ActionTime.Action, TotalToRun = 2, IsMagical = ShepherdSummons ? true : false };
        }
    }
}
