using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Rogue : BaseCharacter
    {
        public bool HuntersMarkRunning { get; set; } = false;
        public bool CanHuntersMark { get; set; } = false;

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

        public class Soulblades : BaseAction
        {
            public Rogue parent { get; set; }

            public Soulblades()
            {
                Desc = "Soul Blades";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
            }

            public override int Amount()
            {
                int damage = Time == ActionTime.Action ?
                    Dice.D4(CriticalHit ? 4 : 2) :
                    Dice.D4(CriticalHit ? 2 : 1);

                if (!parent.DidSneakAttack)
                {
                    damage += Dice.D6(CriticalHit ? 10 : 5);
                    parent.DidSneakAttack = true;
                }

                if (parent.HuntersMarkRunning)
                {
                    damage += Dice.D6(CriticalHit ? 2 : 1);
                }

                return damage += Modifier;
            }
        }

        public class Shortsword : BaseAction
        {
            public Rogue parent { get; set; }

            public Shortsword()
            {
                Desc = "Shortsword";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
            }

            public override int Amount()
            {
                int damage = Dice.D6(CriticalHit ? 2 : 1);

                if (!parent.DidSneakAttack)
                {
                    damage += Dice.D6(CriticalHit ? 10 : 5);
                    parent.DidSneakAttack = true;
                }

                if (parent.HuntersMarkRunning)
                {
                    damage += Dice.D6(CriticalHit ? 2 : 1);
                }

                if (Time == ActionTime.Action)
                {
                    damage += Modifier;
                }

                return damage;
            }
        }

        public class Dagger : BaseAction
        {
            public Rogue parent { get; set; }

            public Dagger()
            {
                Desc = "Dagger";
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
            }

            public override int Amount()
            {
                int damage = Dice.D4(CriticalHit ? 2 : 1);

                if (!parent.DidSneakAttack)
                {
                    damage += Dice.D6(CriticalHit ? 10 : 5);
                    parent.DidSneakAttack = true;
                }

                if (parent.HuntersMarkRunning)
                {
                    damage += Dice.D6(CriticalHit ? 2 : 1);
                }

                if (Time == ActionTime.Action)
                {
                    damage += Modifier;
                }

                return damage;
            }
        }

        public bool DidSneakAttack { get; set; }

        public Rogue()
        {
            Name = "Amxikas";
            AC = 18;
            InitMod = 7;
            Health = 83;
            MaxHealth = 83;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            DidSneakAttack = false;
            MyType = CreatureType.PC;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = 3 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 14, Mod = 2, Save = 2 });
        }

        public override void Init()
        {
            base.Init();
            DidSneakAttack = false;
            CanHuntersMark = true;
            HuntersMarkRunning = false;
        }

        public override void OnNewTurn()
        {
            DidSneakAttack = false;

            if (CanHuntersMark && !HuntersMarkRunning)
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

            if (HuntersMarkRunning) HuntersMarkRunning = false;
        }

        public override BaseAction PickAction()
        {
            return new Soulblades { Time = BaseAction.ActionTime.Action, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (CanHuntersMark && !HuntersMarkRunning)
            {
                CanHuntersMark = false;
                HuntersMarkRunning = true;
                Concentrating = true;
                return new HuntersMarkActivate();
            }

            return new Soulblades { Time = BaseAction.ActionTime.BonusAction, parent = this };
        }
    }
}
