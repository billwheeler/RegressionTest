using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Paladin : BaseCharacter
    {
        public class Warhammer : PaladinBaseWeapon
        {
            public Warhammer()
            {
                Desc = "Warhammer";
                AttackModifier = 9;
                Modifier = 7;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D8(CriticalHit ? 2 : 1);
                damage += TallyBuffs();
                return damage + Modifier;
            }
        }

        public class ShieldBash : PaladinBaseWeapon
        {
            public ShieldBash()
            {
                Desc = "Shield Bash";
                AttackModifier = 9;
                Modifier = 5;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D4(CriticalHit ? 2 : 1);
                damage += TallyBuffs();
                return damage + Modifier;
            }
        }

        public abstract class PaladinBaseWeapon : BaseAction
        {
            public Paladin parent { get; set; }

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
                //damage += Dice.D8(CriticalHit ? 2 : 1);

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
                int amount = 20;
                if (parent.LayOnHandsPool < 20)
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

        public bool CanBonusActionAttack { get; set; } = false;
        public bool CanSpiritShroud { get; set; } = false;
        public bool SpiritShroudRunning { get; set; } = false;
        public bool ShouldUseSmites { get; set; } = false;
        public int LayOnHandsPool { get; set; } = 45;

        public Paladin() : base()
        {
            Name = "Murie";
            AC = 20;
            Health = 85;
            MaxHealth = 85;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 0;
            WarCaster = true;
            MyType = CreatureType.PC;
            ShouldUseSmites = true;
            CanSpiritShroud = true;
            OpportunityAttackChance = 10;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 20, Mod = 5, Save = 8 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 10, Mod = 0, Save = 3 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 14, Mod = 2, Save = 5 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = 2 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 7 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 16, Mod = 3, Save = 10 });
        }

        public override void Init()
        {
            base.Init();
            SpiritShroudRunning = false;
            LayOnHandsPool = 45;
            CanBonusActionAttack = false;
            ShouldUseSmites = true;
            CanSpiritShroud = true;
        }

        public override BaseAction PickAction()
        {
            return new Warhammer { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (CanSpiritShroud && !Concentrating && !SpiritShroudRunning)
            {
                SpiritShroudRunning = true;
                Concentrating = true;
                return new SpiritShroudActivate();
            }

            return new ShieldBash { Time = BaseAction.ActionTime.BonusAction, parent = this };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;
            return new Warhammer { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
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
