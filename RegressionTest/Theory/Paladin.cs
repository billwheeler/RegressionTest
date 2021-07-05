using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Paladin : BaseCharacter
    {
        public class GlaivePM : BaseAction
        {
            public Paladin parent { get; set; }

            public GlaivePM()
            {
                Desc = "Spear";
                Type = ActionType.MeleeAttack;
                Modifier = 7;
            }

            public override int Amount()
            {
                int damage = 0;

                damage += (Time == ActionTime.Action) ?
                    Dice.D6(CriticalHit ? 2 : 1) :
                    Dice.D4(CriticalHit ? 2 : 1);

                if (parent.SpiritShroudRunning)
                {
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                }

                //Console.WriteLine($"Sacred Weapon: {parent.SacredBladeRunning}, Attack: {AttackModifier}");

                // divine smite
                if (Dice.D100() <= (CriticalHit ? 50 : 15))
                {
                    int number = Dice.D4();
                    if (number == 1)
                        number = 2;
                    else if (number > 6)
                        number = 6;

                    damage += Dice.D8(number);
                }

                // divine strike comes online at level 11
                //damage += Dice.D8(CriticalHit ? 2 : 1);

                return damage + Modifier;
            }
        }

        public class LayOnHands : SpellAction
        {
            public Paladin parent { get; set; }

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

        public class SacredBladeActivate : BaseAction
        {
            public SacredBladeActivate()
            {
                Desc = "Sacred Weapon";
                Type = ActionType.Activate;
                Time = ActionTime.Action;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public class SpiritShroudActivate : BaseAction
        {
            public SpiritShroudActivate()
            {
                Desc = "Spirit Shroud";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public bool CanBonusActionAttack { get; set; } = false;
        public bool SacredBladeRunning { get; set; } = false;
        public bool SpiritShroudRunning { get; set; } = false;
        public int LayOnHandsPool { get; set; } = 50;

        public Paladin()
        {
            Name = "Murie";
            AC = 20;
            Health = 94;
            MaxHealth = 94;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 0;
            WarCaster = false;
            MyType = CreatureType.PC;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 20, Mod = 5, Save = 8 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 10, Mod = 0, Save = 3 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 14, Mod = 2, Save = 6 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = 2 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 7 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 16, Mod = 3, Save = 10 });
        }

        public override void Init()
        {
            base.Init();
            SacredBladeRunning = false;
            SpiritShroudRunning = false;
            LayOnHandsPool = 50;
            CanBonusActionAttack = false;
        }

        public override BaseAction PickAction()
        {
            if (!SacredBladeRunning)
            {
                SacredBladeRunning = true;
                return new SacredBladeActivate();
            }

            if (HealTarget != null && LayOnHandsPool > 0 && Dice.D100() <= 33)
            {
                return new LayOnHands { parent = this };
            }

            CanBonusActionAttack = true;
            return new GlaivePM { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this, AttackModifier = SacredBladeRunning ? 12 : 9 };
        }

        public override BaseAction PickBonusAction()
        {
            if (!Concentrating && !SpiritShroudRunning)
            {
                SpiritShroudRunning = true;
                Concentrating = true;
                return new SpiritShroudActivate();
            }

            if (CanBonusActionAttack)
                return new GlaivePM { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this, AttackModifier = SacredBladeRunning ? 12 : 9 };

            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override void OnNewRound()
        {
            base.OnNewRound();
            CanBonusActionAttack = false;
        }

        public override void OnNewTurn()
        {
            if (!SacredBladeRunning)
            {
                BonusActionFirst = false;
            }
            else if (!Concentrating && !SpiritShroudRunning)
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

            if (SpiritShroudRunning) SpiritShroudRunning = false;
        }

        public override void OnDeath()
        {
            base.OnDeath();

            SacredBladeRunning = false;
            SpiritShroudRunning = false;
        }
    }
}
