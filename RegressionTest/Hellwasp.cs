using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Hellwasp : BaseCharacter
    {
        public class HellwaspSting : BaseAction
        {
            public HellwaspSting()
            {
                Desc = "Sting";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 7;
                Modifier = 4;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = 0;

                damage += Dice.D8(CriticalHit ? 2 : 1);
                damage += Dice.D6(CriticalHit ? 4 : 2);

                return damage;
            }
        }

        public class HellwaspTalons : BaseAction
        {
            public HellwaspTalons()
            {
                Desc = "Sword Talons";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 7;
                Modifier = 4;
            }

            public override int Amount()
            {
                return Dice.D6(CriticalHit ? 4 : 2);
            }
        }

        public Hellwasp() : base()
        {
            Name = "Hellwasp";
            AC = 19;
            InitMod = 2;
            Health = 52;
            MaxHealth = 52;
            Group = Team.TeamTwo;
            Priority = HealPriority.Medium;
            IsFiend = true;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 18, Mod = 4, Save = 4 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 15, Mod = 2, Save = 5 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 12, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 3 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 7, Mod = -2, Save = -2 });
        }

        public override BaseAction PickAction()
        {
            return new HellwaspSting();
        }

        public override BaseAction PickBonusAction()
        {
            return new HellwaspTalons();
        }
    }
}
