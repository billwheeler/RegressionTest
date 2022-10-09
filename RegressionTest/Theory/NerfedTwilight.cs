﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class NerfedTwilight : BaseCharacter
    {
        public bool SpiritGuardiansRunning { get; set; }
        public bool TwilightSanctuaryRunning { get; set; }

        public bool ShouldBoomBoom { get; set; } = false;

        public class TollOfTheDead : BaseAction
        {
            public TollOfTheDead()
            {
                Desc = "Toll of the Dead";
                Type = ActionType.SpellSave;
                Time = ActionTime.Action;
                Ability = AbilityScore.Wisdom;
                DC = 16;
            }

            public override int Amount()
            {
                return Dice.D12(2);
            }
        }

        public class Warhammer : BaseAction
        {
            public NerfedTwilight parent { get; set; }

            public Warhammer()
            {
                Desc = "Warhammer";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 7;
                Modifier = 3;
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 4 : 2);

                if (parent.ShouldBoomBoom)
                {
                    if (Time != ActionTime.Reaction || parent.WarCaster)
                    {
                        damage += Dice.D8(CriticalHit ? 2 : 1);
                        if (Dice.D100() <= 33)
                        {
                            damage += Dice.D8(CriticalHit ? 4 : 2);
                        }
                    }
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
                AttackModifier = 8;
                Modifier = 4;
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
                DC = 16;
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

        public NerfedTwilight() : base()
        {
            Name = "Leonid";
            AC = 20;
            InitMod = -1;
            Health = 75;
            MaxHealth = 75;
            Group = Team.TeamOne;
            Healer = true;
            HealingThreshold = 18;
            Priority = HealPriority.High;
            PreTurnNotify = true;
            PostTurnNotify = true;
            TwilightSanctuaryRunning = false;
            SpiritGuardiansRunning = false;
            WarCaster = true;
            HasAdvantageOnInitiative = true;
            MyType = CreatureType.PC;
            OpportunityAttackChance = 10;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 15, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 8, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 18, Mod = 4, Save = 8 });
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
            if (Healer && !TwilightSanctuaryRunning)
            {
                TwilightSanctuaryRunning = true;
                Context.GiveTempHP(Group, this, Dice.D6() + 9);
                return new TwilightSanctuaryActivate();
            }

            if (!SpiritGuardiansRunning)
            {
                SpiritGuardiansRunning = true;
                Concentrating = true;
                return new SpiritGuardiansActivate();
            }

            int rando = Dice.D100();
            if (ShouldBoomBoom)
            {
                if (rando >= 95)
                {
                    return new TollOfTheDead();
                }
                else if (rando < 95 && rando >= 35)
                {
                    return new Warhammer { parent = this };
                }
            }
            else
            {
                if (rando >= 90)
                {
                    return new TollOfTheDead();
                }
                else if (rando <= 35)
                {
                    return new Warhammer { parent = this };
                }
            }

            IsDodging = true;
            return new DodgeAction();
        }

        public override BaseAction PickBonusAction()
        {
            if (Healer && HealTarget != null)
            {
                return new HealingWord { Modifier = 4, Level = SpellAction.SpellLevel.One };
            }

            if (Dice.D100() <= 90)
            {
                return new SpiritualWeapon();
            }

            return new NoAction();
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;

            return new Warhammer { Time = BaseAction.ActionTime.Reaction, parent = this };
        }

        public override BaseAction PickPreTurn(BaseCharacter target)
        {
            if (!target.Incapacitated)
            {
                if (SpiritGuardiansRunning)
                {
                    if (Dice.D100() <= GetSpiritGuardiansChance())
                        return new SpiritGuardiansPreTurn();
                }
            }

            return new NoAction();
        }

        private int GetSpiritGuardiansChance()
        {
            switch (Context.GetLivingEnemyCount(Group, false))
            {
                case 1:
                    return 70;
                case 2:
                    return 63;
                case 3:
                    return 56;
                case 4:
                    return 49;
                case 5:
                    return 42;
                case 6:
                    return 35;
                case 7:
                    return 28;
                case 8:
                    return 21;
                case 9:
                    return 14;
                case 10:
                    return 7;
                default:
                    return 3;
            }
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
