using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class LawCleric : BaseCharacter
    {
        public bool SpiritGuardiansRunning { get; set; }

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
                Desc = "Warhammer w/Booming Blade";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 6;
                Modifier = 2;
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 4 : 2);

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

        public LawCleric()
        {
            Name = "Leonid";
            AC = 20;
            InitMod = -1;
            Health = 83;
            MaxHealth = 83;
            Group = Team.TeamOne;
            Healer = true;
            HealingThreshold = 18;
            Priority = HealPriority.High;
            PreTurnNotify = true;
            PostTurnNotify = true;
            SpiritGuardiansRunning = false;
            MyType = CreatureType.PC;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 9, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 10, Mod = 0, Save = 4 });
        }

        public override void Init()
        {
            base.Init();
            SpiritGuardiansRunning = false;
        }

        public override BaseAction PickAction()
        {
            if (!SpiritGuardiansRunning)
            {
                SpiritGuardiansRunning = true;
                Concentrating = true;
                return new SpiritGuardiansActivate();
            }

            int rando = Dice.D100();
            if (rando >= 67)
            {
                return new TollOfTheDead();
            }
            else if (rando < 67 && rando >= 33)
            {
                IsDodging = true;
                return new DodgeAction();
            }

            return new Warhammer();
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

        public override BaseAction PickPreTurn(BaseCharacter target)
        {
            if (SpiritGuardiansRunning)
            {
                if (target.HighValueTarget)
                    return new SpiritGuardiansPreTurn();

                // we'll say that only 40% of the time an enemy is in range
                if (Dice.D100() <= 40)
                    return new SpiritGuardiansPreTurn();
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
        }
    }
}
