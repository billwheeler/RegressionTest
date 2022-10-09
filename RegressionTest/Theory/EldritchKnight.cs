using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class EldritchKnight : BaseCharacter
    {
        public bool CanShadowBlade { get; set; } = true;
        public bool ShadowBladeRunning { get; set; } = false;

        public bool UsedActionSurge { get; set; } = false;
        public bool UsedSecondWind { get; set; } = false;

        public class Scimitar : BaseAction
        {
            public EldritchKnight parent { get; set; }

            public Scimitar()
            {
                Desc = "Scimitar";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
                IsMagical = true;
            }

            public override int Amount()
            {
                return Dice.D6(CriticalHit ? 2 : 1) + Modifier;
            }
        }

        public class ShadowBlade : BaseAction
        {
            public EldritchKnight parent { get; set; }

            public ShadowBlade()
            {
                Desc = "Shadow Blade";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
                IsMagical = true;
            }

            public override int Amount()
            {
                return Dice.D8(CriticalHit ? 4 : 2) + Modifier;
            }
        }

        public class Longbow : BaseAction
        {
            public EldritchKnight parent { get; set; }

            public Longbow()
            {
                Desc = "Longbow";
                Type = ActionType.RangedAttack;
                AttackModifier = 9;
                Modifier = 5;
            }

            public override int Amount()
            {
                return Dice.D8(CriticalHit ? 2 : 1) + Modifier;
            }
        }

        public class ShadowBladeActivate : BaseAction
        {
            public ShadowBladeActivate()
            {
                Desc = "Shadow Blade";
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

        public EldritchKnight() : base()
        {
            Name = "Amxikas";
            AC = 17;
            Health = 85;
            MaxHealth = 85;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 5;
            MyType = CreatureType.PC;
            WarCaster = true;
            OpportunityAttackChance = 40;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 4 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 5 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 12, Mod = 1, Save = 1 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 13, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 8, Mod = -1, Save = -1 });
        }

        public override void Init()
        {
            base.Init();
            CanShadowBlade = true;
            ShadowBladeRunning = false;
            UsedActionSurge = false;
            UsedSecondWind = false;
        }

        public override BaseAction PickAction()
        {
            int total = 2;
            if (!UsedActionSurge)
            {
                total = 4;
                UsedActionSurge = true;
            }

            if (ShadowBladeRunning)
            {
                return new ShadowBlade { Time = BaseAction.ActionTime.Action, TotalToRun = total, parent = this };
            }

            //if (Dice.D100() <= 10)
            //    return new Longbow { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this };

            return new Scimitar { Time = BaseAction.ActionTime.Action, TotalToRun = total, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (CanShadowBlade && !ShadowBladeRunning)
            {
                CanShadowBlade = false;
                ShadowBladeRunning = true;
                Concentrating = true;

                return new ShadowBladeActivate();
            }

            if (!UsedSecondWind && Health <= HealingThreshold)
            {
                UsedSecondWind = true;
                int amount = Dice.D10() + 10;
                Heal(amount);
                return new SecondWindActivate(amount);
            }

            return new Scimitar { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this };
        }

        public override void OnNewTurn()
        {
            if (CanShadowBlade && !ShadowBladeRunning)
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

            ShadowBladeRunning = false;
        }

        public override void OnDeath()
        {
            base.OnDeath();

            ShadowBladeRunning = false;
        }
    }
}
