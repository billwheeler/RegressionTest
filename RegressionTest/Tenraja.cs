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
        public class Halberd : PaladinBaseWeapon
        {
            public Halberd()
            {
                Desc = "Halberd";
                AttackModifier = 11;
                Modifier = 6;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Time != ActionTime.BonusAction ?
                    Dice.D10(CriticalHit ? 2 : 1) :
                    Dice.D4(CriticalHit ? 2 : 1);

                damage += TallyBuffs();
                return damage + Modifier;
            }
        }

        public abstract class PaladinBaseWeapon : BaseAction
        {
            public Tenraja parent { get; set; }

            private string _desc = string.Empty;
            private bool _smitedThisTurn = false;

            // some heuristics for smiting
            private bool enemyIsUndead = false;
            private bool enemyIsHVT = false;

            public override string Desc
            {
                get
                {
                    string output = _desc;

                    if (_smitedThisTurn)
                    {
                        _smitedThisTurn = false;
                        output += " (smite)";
                    }

                    return output;
                }
                set { _desc = value; }
            }

            public PaladinBaseWeapon()
            {
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
            }

            public override void PreHit(BaseCharacter attacker, BaseCharacter target)
            {
                base.PreHit(attacker, target);

                enemyIsUndead = target.IsUndead;
                enemyIsHVT = target.HighValueTarget;
            }

            public int TallyBuffs()
            {
                int damage = 0;

                if (parent.SpiritShroudRunning)
                {
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                }

                if (parent.ShouldUseSmites)
                {
                    int percentToSmite = enemyIsHVT ? 40 : 20;

                    if (CriticalHit)
                        percentToSmite = 100;

                    // divine smite
                    if (Dice.D100() <= percentToSmite)
                    {
                        parent.Stats.Smites++;
                        _smitedThisTurn = true;
                        damage += Dice.D8(DiceNumberForSmite(CriticalHit));
                    }
                }

                // divine strike comes online at level 11
                damage += Dice.D8(CriticalHit ? 2 : 1);

                return damage;
            }

            private int DiceNumberForSmite(bool isCrit)
            {
                int count = 2;
                if (enemyIsUndead)
                    count += 1;

                if (isCrit)
                    count *= 2;

                if (enemyIsUndead && count > 6)
                    count = 6;

                if (!enemyIsUndead && count > 5)
                    count = 5;

                return count;
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
                int amount = 65;
                if (parent.LayOnHandsPool < 65)
                    amount = parent.LayOnHandsPool;
                return amount;
            }
        }

        public bool CanSpiritShroud { get; set; } = false;
        public bool SpiritShroudRunning { get; set; } = false;
        public bool ShouldUseSmites { get; set; } = false;
        public int LayOnHandsPool { get; set; } = 65;

        public Tenraja() : base()
        {
            Name = "Tenraja";
            AC = 19;
            Health = 147;
            MaxHealth = 147;
            HealingThreshold = 27;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 5;
            WarCaster = false;
            MyType = CreatureType.PC;
            ShouldUseSmites = true;
            OpportunityAttackChance = 50;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 18, Mod = 4, Save = 7 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 11, Mod = 0, Save = 3 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 19, Mod = 4, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 9, Mod = -1, Save = 2 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 8 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 16, Mod = 3, Save = 11 });
        }

        public override void Init()
        {
            base.Init();
            LayOnHandsPool = 65;
            ShouldUseSmites = true;
            CanSpiritShroud = false;
            SpiritShroudRunning = false;
        }

        public override BaseAction PickAction()
        {
            return new Halberd { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (CanSpiritShroud && !Concentrating && !SpiritShroudRunning)
            {
                SpiritShroudRunning = true;
                Concentrating = true;
                return new SpiritShroudActivate();
            }

            return new Halberd { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;
            return new Halberd { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();
            if (CanSpiritShroud && !Concentrating && !SpiritShroudRunning)
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

            if (SpiritShroudRunning) SpiritShroudRunning = false;
        }

        public override void OnDeath()
        {
            base.OnDeath();

            SpiritShroudRunning = false;
        }
    }
}
