﻿using System;
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

        public bool ShouldTwilight { get; set; } = false;

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
                return Dice.D8(CriticalHit ? 4 : 2) + Modifier;
            }
        }

        public class Mace : BaseAction
        {
            public Mace()
            {
                Desc = "Mace";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 6;
                Modifier = 2;
            }

            public override int Amount()
            {
                return Dice.D6(CriticalHit ? 2 : 1) + Dice.D8(CriticalHit ? 2 : 1) + Modifier;
            }
        }

        public class MaceBooming : BaseAction
        {
            public MaceBooming()
            {
                Desc = "Mace w/Booming Blade";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 6;
                Modifier = 2;
            }

            public override int Amount()
            {
                int damage = Dice.D6(CriticalHit ? 2 : 1) + Dice.D8(CriticalHit ? 2 : 1);

                damage += Dice.D8(CriticalHit ? 2 : 1);
                if (Dice.D100() <= 33)
                {
                    damage += Dice.D8(CriticalHit ? 4 : 2);
                }

                return damage + Modifier;
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
                return Dice.D8(CriticalHit ? 4 : 2) + Modifier;
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
                //return Dice.D8();
                return Dice.D6() + 10;
            }
        }

        public Cleric()
        {
            Name = "Leonid";
            AC = ShouldTwilight ? 20 : 21;
            InitMod = -1;
            Health = 83;
            MaxHealth = 83;
            Group = Team.TeamOne;
            Healer = true;
            HealingThreshold = 18;
            Priority = HealPriority.High;
            PreTurnNotify = true;
            PostTurnNotify = true;
            TwilightSanctuaryRunning = false;
            SpiritGuardiansRunning = false;
            WarCaster = true;
            HasAdvantageOnInitiative = false;

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
            if (!SpiritGuardiansRunning)
            {
                SpiritGuardiansRunning = true;
                Concentrating = true;
                return new SpiritGuardiansActivate();
            }

            if (ShouldTwilight && Healer && !TwilightSanctuaryRunning)
            {
                TwilightSanctuaryRunning = true;
                return new TwilightSanctuaryActivate();
            }

            if (Dice.D100() <= 75)
            {
                return new TollOfTheDead();
            }

            if (ShouldTwilight)
                return new Warhammer();
            else
                return new MaceBooming();
        }

        public override BaseAction PickBonusAction()
        {
            if (Healer && HealTarget != null)
            {
                return new HealingWord { Modifier = 5, Level = SpellAction.SpellLevel.Three };
            }

            if (Dice.D100() <= 90)
            {
                return new SpiritualWeapon();
            }

            return new NoAction();
        }

        public override BaseAction PickPreTurn()
        {
            if (SpiritGuardiansRunning)
            {
                // we'll say that only 40% of the time an enemy is in range
                if (Dice.D100() <= 40)
                {
                    return new SpiritGuardiansPreTurn();
                }
            }

            return new NoAction();
        }

        public override BaseAction PickPostTurn()
        {
            if (TwilightSanctuaryRunning)
            {
                return new TwilightSanctuaryPostTurn();
            }

            return new NoAction();
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            SpiritGuardiansRunning = false;
        }

        public override void OnDeath()
        {
            base.OnDeath();

            SpiritGuardiansRunning = false;
            TwilightSanctuaryRunning = false;
        }
    }
}
