using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Cleric : BaseCharacter
    {
        public bool SpiritGuardiansRunning { get; set; }
        public bool TwilightSanctuaryRunning { get; set; }

        public class Warhammer : BaseAction
        {
            public Warhammer()
            {
                Desc = "Warhammer";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 6;
                Modifier = 2;
            }

            public override int Amount()
            {
                return Dice.D8(CriticalHit ? 2 : 1) + 2;
            }
        }

        public class SpiritualWeapon : BaseAction
        {
            public SpiritualWeapon()
            {
                Desc = "Spiritual Weapon";
                Type = ActionType.SpellAttack;
                Time = ActionTime.BonusAction;
                AttackModifier = 9;
                Modifier = 5;
            }

            public override int Amount()
            {
                return Dice.D8(CriticalHit ? 2 : 1) + 2;
            }
        }

        public class SpiritGuardiansActivate : BaseAction
        {
            public SpiritGuardiansActivate()
            {
                Desc = "Spirit Guardians";
                Type = ActionType.Activate;
                Time = ActionTime.Action;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public class SpiritGuardiansPreTurn : BaseAction
        {
            public SpiritGuardiansPreTurn()
            {
                Desc = "Spirit Guardians";
                Type = ActionType.SpellSave;
                Time = ActionTime.PreTurn;
                HalfDamageOnMiss = true;
                Ability = AbilityScore.Wisdom;
                DC = 17;
            }

            public override int Amount()
            {
                int damage = Dice.D8(5);
                if (Result == DamageAmount.Half)
                {
                    damage = (int)Math.Floor(damage / 2.0f);
                }

                return damage;
            }
        }

        public class TwilightSanctuaryActivate : BaseAction
        {
            public TwilightSanctuaryActivate()
            {
                Desc = "Twilight Sanctuary";
                Type = ActionType.Activate;
                Time = ActionTime.Action;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public class TwilightSanctuaryPostTurn : BaseAction
        {
            public TwilightSanctuaryPostTurn()
            {
                Desc = "Twilight Sanctuary";
                Type = ActionType.GrantTempHp;
                Time = ActionTime.PostTurn;
            }

            public override int Amount()
            {
                return Dice.D6() + 10;
            }
        }

        public Cleric()
        {
            Name = "Leonid";
            AC = 20;
            InitMod = -1;
            Health = 83;
            MaxHealth = 83;
            Group = Team.TeamOne;
            Healer = true;
            HealingThreshold = 30;
            Priority = HealPriority.High;
            PreTurnNotify = true;
            PostTurnNotify = true;
            TwilightSanctuaryRunning = false;
            SpiritGuardiansRunning = false;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 8, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 10, Mod = 0, Save = 4 });
        }

        public override void Init()
        {
            base.Init();
            SpiritGuardiansRunning = false;
            TwilightSanctuaryRunning = false;
        }

        public override BaseAction PickAction()
        {
            if (Healer && !SpiritGuardiansRunning)
            {
                SpiritGuardiansRunning = true;
                Concentrating = true;
                return new SpiritGuardiansActivate { Owner = this };
            }

            if (Healer && !TwilightSanctuaryRunning)
            {
                TwilightSanctuaryRunning = true;
                return new TwilightSanctuaryActivate { Owner = this };
            }

            return new Warhammer { Owner = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (HealTarget != null)
            {
                return new HealingWord { Owner = this, Modifier = 5 };
            }

            if (Dice.D100() <= 90)
            {
                return new SpiritualWeapon { Owner = this };
            }

            return new NoAction { Owner = this };
        }

        public override BaseAction PickPreTurn()
        {
            if (SpiritGuardiansRunning)
            {
                // we'll say that only 40% of the time an enemy is in range
                if (Dice.D100() <= 40)
                {
                    return new SpiritGuardiansPreTurn { Owner = this };
                }
            }

            return new NoAction { Owner = this };
        }

        public override BaseAction PickPostTurn()
        {
            if (TwilightSanctuaryRunning)
            {
                return new TwilightSanctuaryPostTurn { Owner = this };
            }

            return new NoAction { Owner = this };
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            SpiritGuardiansRunning = false;
        }
    }
}
