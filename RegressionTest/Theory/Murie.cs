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
        public bool SpiritualWeaponRunning { get; set; }
        public bool CanTurn { get; set; } = false;
        public bool UsedChannelDivinity { get; set; } = false;
        public bool CastLevelledSpellThisTurn { get; set; } = false;

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
                IsMagical = true;
            }

            public override int Amount()
            {
                return Dice.D12(2);
            }
        }

        public class Warhammer : BaseAction
        {
            public Murie parent { get; set; }

            public Warhammer()
            {
                Desc = "Warhammer";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 7;
                Modifier = 3;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 4 : 2);

                return damage + Modifier;
            }
        }

        public class Mace : BaseAction
        {
            public Murie parent { get; set; }

            public Mace()
            {
                Desc = "Mace";
                Type = ActionType.MeleeAttack;
                Time = ActionTime.Action;
                AttackModifier = 8;
                Modifier = 4;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D6(CriticalHit ? 2 : 1);

                damage += Dice.D8(CriticalHit ? 2 : 1);

                if (parent.ShouldBoomBoom)
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

        public class TurnUndead : BaseAction
        {
            public TurnUndead()
            {
                Desc = "Turn Undead";
                Type = ActionType.SpellSave;
                Time = ActionTime.Action;
                Ability = AbilityScore.Wisdom;
                Damageless = true;
                MinTargets = 2;
                MaxTargets = 6;
                DC = 16;
                IsMagical = true;

                EffectToApply = new SpellEffect
                {
                    Ability = AbilityScore.Wisdom,
                    DC = 15,
                    Name = "Turn Undead",
                    Type = SpellEffectType.Turned,
                    SaveType = SpellEffectSave.Never
                };
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

        public Murie() : base()
        {
            Name = "Murie";
            AC = 22;
            InitMod = -1;
            Health = ShouldBoomBoom ? 75 : 84;
            MaxHealth = ShouldBoomBoom ? 75 : 84;
            Group = Team.TeamOne;
            Healer = true;
            HealingThreshold = 18;
            Priority = HealPriority.High;
            PreTurnNotify = true;
            SpiritGuardiansRunning = false;
            WarCaster = true;
            MyType = CreatureType.PC;
            OpportunityAttackChance = 10;

            if (ShouldBoomBoom)
                Abilities.Add(AbilityScore.Strength, new Stat { Score = 16, Mod = 3, Save = 3 });
            else
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
            SpiritualWeaponRunning = false;
            UsedChannelDivinity = false;
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();

            CastLevelledSpellThisTurn = false;
        }

        public override BaseAction PickAction()
        {
            if (!UsedChannelDivinity)
            {
                if (CanTurn)
                {
                    UsedChannelDivinity = true;
                    return new TurnUndead();
                }
            }

            if (!SpiritGuardiansRunning)
            {
                SpiritGuardiansRunning = true;
                Concentrating = true;
                Stats.SpellsUsed++;
                CastLevelledSpellThisTurn = true;
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
                    return new Mace { parent = this };
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
                    return new Mace { parent = this };
                }
            }

            IsDodging = true;
            return new DodgeAction { Time = BaseAction.ActionTime.Action };
        }

        public override BaseAction PickBonusAction()
        {
            if (!CastLevelledSpellThisTurn && !SpiritualWeaponRunning)
            {
                SpiritualWeaponRunning = true;
                Stats.SpellsUsed++;
                CastLevelledSpellThisTurn = true;
                return new SpiritualWeapon();
            }

            if (!CastLevelledSpellThisTurn && Healer && HealTarget != null)
            {
                Stats.SpellsUsed++;
                return new HealingWord { Modifier = 4, Level = SpellAction.SpellLevel.One };
            }

            if (SpiritualWeaponRunning && Dice.D100() <= 90)
            {
                return new SpiritualWeapon();
            }

            return new NoAction();
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;

            return new Mace { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
        }

        public override BaseAction PickPreTurn(BaseCharacter target)
        {
            if (!target.HasUndesirableEffect())
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
                    return 90;
                case 2:
                    return 81;
                case 3:
                    return 72;
                case 4:
                    return 63;
                case 5:
                    return 54;
                case 6:
                    return 45;
                case 7:
                    return 36;
                case 8:
                    return 27;
                case 9:
                    return 18;
                case 10:
                    return 9;
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
            SpiritualWeaponRunning = false;
        }
    }
}
