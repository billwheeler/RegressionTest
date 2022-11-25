using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Conquest : BaseCharacter
    {
        public class RevenantBlade : AmxikasBaseWeapon
        {
            public RevenantBlade()
            {
                Desc = "Revenant Blade";
            }

            public override int Amount()
            {
                int damage = (Time != ActionTime.BonusAction) ?
                    Dice.D4(CriticalHit ? 4 : 2) :
                    Dice.D4(CriticalHit ? 2 : 1);

                damage += TallyBuffs();

                return damage + Modifier;
            }
        }

        public abstract class AmxikasBaseWeapon : BaseAction
        {
            public Conquest parent { get; set; }

            private string _desc = string.Empty;
            private bool _smitedThisTurn = false;

            // some heuristics for smiting
            private bool enemyIsUndead = false;
            private bool enemyIsFiend = false;

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

            public AmxikasBaseWeapon()
            {
                Type = ActionType.MeleeAttack;
                AttackModifier = 9;
                Modifier = 5;
                IsMagical = true;
            }

            public override void PreHit(BaseCharacter attacker, BaseCharacter target)
            {
                base.PreHit(attacker, target);

                enemyIsUndead = target.IsUndead;
                enemyIsFiend = target.IsFiend;
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
                    int percentToSmite = 5;

                    if (enemyIsUndead)
                        percentToSmite = 20;
                    else if (enemyIsFiend)
                        percentToSmite = 20;

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
                else if (enemyIsFiend)
                    count += 1;

                if (isCrit)
                    count *= 2;

                if ((enemyIsUndead || enemyIsFiend) && count > 6)
                    count = 6;

                if (!(enemyIsUndead || enemyIsFiend) && count > 5)
                    count = 5;

                return count;
            }
        }

        public class LayOnHands : SpellAction
        {
            public Conquest parent { get; set; }

            public LayOnHands()
            {
                Desc = "Lay On Hands";
                Type = ActionType.Heal;
                Time = ActionTime.Action;
                IsMagical = true;
            }

            public override int Amount()
            {
                int amount = 20;
                if (parent.LayOnHandsPool < 20)
                    amount = parent.LayOnHandsPool;
                return amount;
            }
        }

        public class ConqueringPresenseApply : BaseAction
        {
            public ConqueringPresenseApply()
            {
                Desc = "Conquering Presense";
                Type = ActionType.NewRound;
                IsMagical = true;
            }

            public override int Amount()
            {
                return 4;
            }
        }

        public class ConqueringPresense : BaseAction
        {
            public ConqueringPresense(BaseCharacter owner)
            {
                Desc = "Conquering Presense";
                Type = ActionType.SpellSave;
                Time = ActionTime.Action;
                Ability = AbilityScore.Wisdom;
                Damageless = true;
                MinTargets = 3;
                MaxTargets = 8;
                DC = 16;

                EffectToApply = new SpellEffect
                {
                    Ability = AbilityScore.Wisdom,
                    DC = 16,
                    Name = "Conquering Presense",
                    Type = SpellEffectType.ConqueringPresense,
                    NewRoundAction = new ConqueringPresenseApply(),
                    Owner = owner
                };
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

        public bool CanConqueringPresense { get; set; } = false;
        public bool UsedChannelDivinity { get; set; } = false;
        public bool ConqueringPresenseRunning { get; set; } = false;

        public bool CanSpiritShroud { get; set; } = false;
        public bool SpiritShroudRunning { get; set; } = false;

        public bool ShouldUseSmites { get; set; } = false;
        public int LayOnHandsPool { get; set; } = 45;

        public Conquest() : base()
        {
            Name = "Amxikas";
            AC = 18;
            Health = 76;
            MaxHealth = 76;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = true;
            Priority = HealPriority.Medium;
            InitMod = 5;
            WarCaster = false;
            MyType = CreatureType.PC;
            OpportunityAttackChance = 20;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 3 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 18, Mod = 4, Save = 7 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 14, Mod = 2, Save = 9 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = 2 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 7 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 18, Mod = 4, Save = 11 });
        }

        public override void Init()
        {
            base.Init();
            ShouldUseSmites = true;
            SpiritShroudRunning = false;
            UsedChannelDivinity = false;
            ConqueringPresenseRunning = false;
        }

        public override BaseAction PickAction()
        {
            if (HealTarget != null && LayOnHandsPool > 0 && Dice.D100() <= 33)
            {
                return new LayOnHands { parent = this };
            }

            if (CanConqueringPresense && !UsedChannelDivinity && !ConqueringPresenseRunning)
            {
                UsedChannelDivinity = true;
                ConqueringPresenseRunning = true;

                return new ConqueringPresense(this);
            }

            return new RevenantBlade { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (CanSpiritShroud && !Concentrating && !SpiritShroudRunning)
            {
                SpiritShroudRunning = true;
                Concentrating = true;
                return new SpiritShroudActivate();
            }

            return new RevenantBlade { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 1, parent = this };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            Stats.OpportunityAttacks++;
            return new RevenantBlade { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
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

