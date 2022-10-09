using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Liriam : BaseCharacter
    {
        public class Hooves : BaseAction
        {
            public Hooves()
            {
                Desc = "Hooves";
                Modifier = 6;
            }

            public override int Amount()
            {
                return Dice.D8(CriticalHit ? 4 : 2) + 4;
            }
        }

        public override BaseAction PickAction()
        {
            return new Hooves();
        }

        public Liriam()
        {
            Name = "Liriam";
            AC = 18;
            Health = 68;
            MaxHealth = 68;
            HealingThreshold = 1;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Dont;
        }
    }

    public class Tenraja : BaseCharacter
    {
        public class HalberdPolearmMaster : BaseAction
        {
            public Tenraja parent { get; set; }

            public HalberdPolearmMaster()
            {
                Desc = "Halberd";
                Type = ActionType.MeleeAttack;
                AttackModifier = 10;
                Modifier = 6;
            }

            public override int Amount()
            {
                int damage = 0;

                damage += (Time == ActionTime.Action) ?
                    Dice.D10(CriticalHit ? 2 : 1) :
                    Dice.D4(CriticalHit ? 2 : 1);

                if (parent.DivineFavorRunning)
                {
                    damage += Dice.D4(CriticalHit ? 2 : 1);
                }

                // divine smite
                if (Dice.D100() <= (CriticalHit ? 50 : 10))
                {
                    int number = Dice.D4();
                    if (number == 1)
                        number = 2;
                    else if (number > 6)
                        number = 6;

                    damage += Dice.D8(number);
                }

                // divine strike comes online at level 11
                damage += Dice.D8(CriticalHit ? 2 : 1);

                return damage + Modifier;
            }
        }

        public class LayOnHands : SpellAction
        {
            public Tenraja parent { get; set; }

            public LayOnHands()
            {
                Desc = "Lay On Hands";
                Type = ActionType.Heal;
                Time = ActionTime.Action;
            }

            public override int Amount()
            {
                int amount = 15;
                if (parent.LayOnHandsPool < 15)
                    amount = parent.LayOnHandsPool;
                return amount;
            }
        }

        public class DivineFavorActivate : BaseAction
        {
            public DivineFavorActivate()
            {
                Desc = "Divine Favor";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public bool CanBonusActionAttack { get; set; } = false;
        public bool DivineFavorRunning { get; set; } = false;
        public int LayOnHandsPool { get; set; } = 50;

        public Tenraja() : base()
        {
            Name = "Tenraja";
            AC = 18;
            Health = 92;
            MaxHealth = 92;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 0;
            WarCaster = false;
            MyType = CreatureType.PC;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 18, Mod = 4, Save = 7 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 11, Mod = 0, Save = 3 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 13, Mod = 2, Save = 4 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 9, Mod = -1, Save = 2 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 7 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 16, Mod = 3, Save = 10 });
        }

        public override void Init()
        {
            base.Init();
            DivineFavorRunning = false;
            LayOnHandsPool = 55;
            CanBonusActionAttack = false;
        }

        public override BaseAction PickAction()
        {
            if (HealTarget != null && LayOnHandsPool > 0 && Dice.D100() <= 33)
            {
                return new LayOnHands { parent = this };
            }

            CanBonusActionAttack = true;
            return new HalberdPolearmMaster { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (!Concentrating && !DivineFavorRunning)
            {
                DivineFavorRunning = true;
                Concentrating = true;
                return new DivineFavorActivate();
            }

            if (CanBonusActionAttack)
                return new HalberdPolearmMaster { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this };

            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override bool OnNewRound()
        {
            bool result = base.OnNewRound();
            CanBonusActionAttack = false;
            return result;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();

            if (!Concentrating && !DivineFavorRunning)
            {
                BonusActionFirst = Healer && HealTarget != null ? false : true;
            }
            else
            {
                BonusActionFirst = false;
            }
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            if (DivineFavorRunning) DivineFavorRunning = false;
        }

        public override void OnDeath()
        {
            base.OnDeath();

            DivineFavorRunning = false;
        }
    }
}
