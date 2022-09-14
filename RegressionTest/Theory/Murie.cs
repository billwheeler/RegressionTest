using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Murie : BaseCharacter
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
                DC = 16;
                IsMagical = true;
            }

            public override int Amount()
            {
                return Dice.D12(2);
            }
        }

        public class Warhammer : BaseAction
        {
            public readonly bool CanUseBoomingBlade = true;

            public Murie parent { get; set; }

            public Warhammer()
            {
                Desc = "Warhammer";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 6;
                Modifier = 2;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 4 : 2);

                if (CanUseBoomingBlade)
                {
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                    int percentageEnemyMoves = (Time == ActionTime.Reaction) ? 90 : 25;
                    if (Dice.D100() <= percentageEnemyMoves)
                    {
                        damage += Dice.D8(CriticalHit ? 4 : 2);
                    }
                }

                return damage + Modifier;
            }
        }

        public class Mace : BaseAction
        {
            public readonly bool CanUseBoomingBlade = true;

            public Murie parent { get; set; }

            public Mace()
            {
                Desc = "Mace";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 7;
                Modifier = 3;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D6(CriticalHit ? 4 : 2);

                if (CanUseBoomingBlade)
                {
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                    int percentageEnemyMoves = (Time == ActionTime.Reaction) ? 90 : 25;
                    if (Dice.D100() <= percentageEnemyMoves)
                    {
                        damage += Dice.D8(CriticalHit ? 4 : 2);
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
                IsMagical = true;
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
                IsMagical = true;
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

        public Murie()
        {
            Name = "Murie";
            AC = 22;
            InitMod = -1;
            Health = 75;
            MaxHealth = 75;
            Group = Team.TeamOne;
            Healer = true;
            HealingThreshold = 18;
            Priority = HealPriority.High;
            PreTurnNotify = true;
            SpiritGuardiansRunning = false;
            WarCaster = true;
            MyType = CreatureType.PC;
            OpportunityAttackChance = 10;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 16, Mod = 3, Save = 3 });
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
            if (rando >= 90)
            {
                return new TollOfTheDead();
            }
            else if (rando < 90 && rando >= 30)
            {
                return new Mace { parent = this };
            }

            IsDodging = true;
            return new DodgeAction { Time = BaseAction.ActionTime.Action };
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

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            return new Mace { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
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
        }
    }
}
