using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Druid : BaseCharacter
    {
        public bool BearTotemRunning { get; set; } = false;
        public bool ConjureRunning { get; set; } = false;

        public bool BearTotemUsed { get; set; } = false;
        public bool ConjureUsed { get; set; } = false;

        public class TollOfTheDead : BaseAction
        {
            public TollOfTheDead()
            {
                Desc = "Toll of the Dead";
                Type = ActionType.SpellSave;
                Time = ActionTime.Action;
                Ability = AbilityScore.Wisdom;
                DC = 17;
            }

            public override int Amount()
            {
                return Dice.D12(2);
            }
        }

        public class Scimitar : BaseAction
        {
            public Scimitar()
            {
                Desc = "Scimitar";
                Type = ActionType.MeleeAttack;
                AttackModifier = 6;
                Modifier = 2;
            }

            public override int Amount()
            {
                return Dice.D6(CriticalHit ? 2 : 1) + Modifier;
            }
        }

        public class ConjureWoodlandBeingsActivate : BaseAction
        {
            public ConjureWoodlandBeingsActivate()
            {
                Desc = "Conjure Woodland Beings";
                Type = ActionType.Activate;
                Time = ActionTime.Action;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public class BearTotemActivate : BaseAction
        {
            public BearTotemActivate()
            {
                Desc = "Bear Totem";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public class BearTotemSummonPostTurn : BaseAction
        {
            public BearTotemSummonPostTurn()
            {
                Desc = "Bear Totem";
                Type = ActionType.Heal;
                Time = ActionTime.PostTurn;
            }

            public override int Amount()
            {
                return 5;
            }
        }

        public Druid()
        {
            Name = "Cralamir";
            AC = 17;
            Health = 83;
            MaxHealth = 83;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = true;
            Priority = HealPriority.High;
            InitMod = 2;
            WarCaster = true;
            MyType = CreatureType.PC;
            PostTurnNotify = true;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 8, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 12, Mod = 1, Save = 5 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 10, Mod = 0, Save = 0 });
        }

        public override void Init()
        {
            base.Init();
            BearTotemRunning = false;
            ConjureRunning = false;
            BearTotemUsed = false;
            ConjureUsed = false;
        }

        public override BaseAction PickAction()
        {
            if (!ConjureUsed && !ConjureRunning)
            {
                ConjureRunning = true;
                Concentrating = true;
                ConjureUsed = true;
                Context.ActivateSummons(Group);

                return new ConjureWoodlandBeingsActivate();
            }

            return new TollOfTheDead { Time = BaseAction.ActionTime.Action };
        }

        public override BaseAction PickBonusAction()
        {
            if (!BearTotemUsed && !BearTotemRunning)
            {
                Context.GiveTempHP(Group, this, 15);
                BearTotemRunning = true;
                BearTotemUsed = true;
                return new BearTotemActivate();
            }

            if (Healer && HealTarget != null)
            {
                return new HealingWord { Modifier = 5, Level = SpellAction.SpellLevel.Three };
            }

            return new NoAction();
        }

        public override BaseAction PickPostTurn(BaseCharacter target)
        {
            if (BearTotemRunning)
            {
                if (target.MyType == CreatureType.Summon)
                    return new BearTotemSummonPostTurn();
            }

            return new NoAction();
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            ConjureRunning = false;
            Context.DeactivateSummons(Group);
        }

        public override void OnDeath()
        {
            base.OnDeath();

            ConjureRunning = false;
            BearTotemRunning = false;
            Context.DeactivateSummons(Group);
        }
    }
}
