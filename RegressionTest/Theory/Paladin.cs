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

                if (parent.SpiritShroudRunning)
                {
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                }

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

                damage += Dice.D8(CriticalHit ? 2 : 1);

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
                int amount = 25;
                if (parent.LayOnHandsPool < 25)
                    amount = parent.LayOnHandsPool;
                return amount;
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

        public bool SpiritShroudRunning { get; set; } = false;
        public int LayOnHandsPool { get; set; } = 50;

        public Paladin()
        {
            Name = "Murie";
            AC = 18;
            Health = 85;
            MaxHealth = 85;
            HealingThreshold = 30;
            Group = Team.TeamOne;
            Healer = true;
            Priority = HealPriority.Medium;
            InitMod = 0;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 20, Mod = 5, Save = 8 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 10, Mod = 0, Save = 3 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 6 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = 2 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 7 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 16, Mod = 3, Save = 10 });
        }

        public override void Init()
        {
            base.Init();
            SpiritShroudRunning = false;
        }

        public override BaseAction PickAction()
        {
            if (HealTarget != null && LayOnHandsPool > 0)
            {
                return new LayOnHands { parent = this };
            }

            return new GlaivePM { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (false && !Concentrating && !SpiritShroudRunning)
            {
                SpiritShroudRunning = true;
                return new SpiritShroudActivate();
            }

            //return new NoAction { Time = BaseAction.ActionTime.BonusAction };
            return new GlaivePM { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this };
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            SpiritShroudRunning = false;
        }

        public override void OnDeath()
        {
            base.OnDeath();
        }
    }
}
