using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class SteamMephit : BaseCharacter
    {
        public bool ShepherdSummons { get; set; } = false;

        public class SteamBreath : BaseAction
        {
            public SteamBreath()
            {
                Desc = "Steam Breath";
                Type = ActionType.SpellSave;
                Time = ActionTime.Action;
                Ability = AbilityScore.Dexterity;
                HalfDamageOnMiss = true;
                MinTargets = 1;
                MaxTargets = 3;
                DC = 10;
                IsMagical = true;
            }

            public override int Amount()
            {
                return Dice.D4(CriticalHit ? 4 : 2) + Modifier;
            }
        }

        public class Claws : BaseAction
        {
            public Claws()
            {
                Desc = "Claws";
                Type = ActionType.MeleeAttack;
                AttackModifier = 2;
                Modifier = 1;
                IsMagical = true;
            }

            public override int Amount()
            {
                return Dice.D4(CriticalHit ? 4 : 2) + Modifier;
            }
        }

        public SteamMephit() : base()
        {
            Name = "Elly";
            AC = 11;
            Health = ShepherdSummons ? 33 : 21;
            MaxHealth = ShepherdSummons ? 33 : 21;
            HealingThreshold = 6;
            Group = Team.TeamOne;
            Priority = HealPriority.Dont;
            InitMod = -1;
            MyType = CreatureType.Summon;
            OpportunityAttackChance = 10;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 5, Mod = -3, Save = -3 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 11, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 11, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 12, Mod = 1, Save = 1 });
        }

        public override BaseAction PickAction()
        {
            if (Dice.D6() == 6)
                return new SteamBreath();

            return new Claws { Time = BaseAction.ActionTime.Action, TotalToRun = 1 };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;

            return new Claws { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1 };
        }
    }
 
    public class ArtElementalMascot : BaseCharacter
    {
        public bool ShepherdSummons { get; set; } = false;

        public class Flare : BaseAction
        {
            public Flare()
            {
                Desc = "Flare";
                Type = ActionType.RangedAttack;
                AttackModifier = 3;
                Modifier = 1;
            }

            public override int Amount()
            {
                return Dice.D4(CriticalHit ? 4 : 2) + Modifier;
            }
        }

        public ArtElementalMascot() : base()
        {
            Name = "Elly";
            AC = 11;
            Health = ShepherdSummons ? 26 : 18;
            MaxHealth = ShepherdSummons ? 26 : 18;
            HealingThreshold = 6;
            Group = Team.TeamOne;
            Priority = HealPriority.Dont;
            InitMod = -1;
            MyType = CreatureType.Summon;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 6, Mod = -2, Save = -2 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 13, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 12, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 11, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 15, Mod = 2, Save = 2 });
        }

        public override BaseAction PickAction()
        {
            return new Flare { Time = BaseAction.ActionTime.Action, TotalToRun = 1, IsMagical = ShepherdSummons };
        }
    }
}
