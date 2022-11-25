using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class AmxikasRi : BaseCharacter
    {
        public class StunningStrike : BaseAction
        {
            public StunningStrike()
            {
                Desc = "Stunning Strike";
                Type = ActionType.SpellSave;
                Time = ActionTime.Action;
                MinTargets = 1;
                MaxTargets = 1;
                Damageless = true;

                EffectToApply = new SpellEffect
                {
                    Name = "Stunning Strike",
                    Type = SpellEffectType.Stunned,
                    DC = 15,
                    Ability = AbilityScore.Constitution
                };
            }
        }

        public class Longsword : AmxikasBaseWeapon
        {
            public Longsword()
            {
                Desc = "Longsword";
            }

            public override int Amount()
            {
                return Dice.D10(CriticalHit ? 2 : 1) + Modifier;
            }
        }

        public class LongswordBrace : AmxikasBaseWeapon
        {
            public LongswordBrace()
            {
                Desc = "Longsword";
            }

            public override int Amount()
            {
                int damage = Dice.D10(CriticalHit ? 2 : 1);

                damage += Dice.D8(CriticalHit ? 2 : 1);
                _bracedThisTurn = true;

                return damage + Modifier;
            }
        }

        public class FlurryOfBlows : AmxikasBaseWeapon
        {
            public FlurryOfBlows()
            {
                Desc = "Flurry of Cuts";
            }

            public override int Amount()
            {
                return Dice.D6(CriticalHit ? 2 : 1) + Modifier;
            }
        }

        public abstract class AmxikasBaseWeapon : BaseAction
        {
            public AmxikasRi parent { get; set; }

            protected string _desc = string.Empty;
            protected bool _stunnedThisTurn = false;
            protected bool _bracedThisTurn = false;

            public AmxikasBaseWeapon()
            {
                Type = ActionType.SpellAttack;
                AttackModifier = 9;
                Modifier = 5;
                IsMagical = true;
            }

            public override string Desc
            {
                get
                {
                    string output = _desc;
                    
                    if (_bracedThisTurn)
                    {
                        _bracedThisTurn = false;
                        output += " (brace)";
                    }

                    return output;
                }
                set { _desc = value; }
            }

            public override void PreHit(BaseCharacter attacker, BaseCharacter target)
            {
                base.PreHit(attacker, target);

                ApplyEffectAfter = null;

                if (parent.KiPoints > 0 && !target.ActiveEffects[SpellEffectType.Stunned].Active)
                {
                    int percentToStun = 25;

                    if (target.HighValueTarget)
                        percentToStun = 100;

                    if (Dice.D100() <= percentToStun)
                        ApplyEffectAfter = new StunningStrike();
                }
            }
        }

        public class SecondWindActivate : BaseAction
        {
            public SecondWindActivate(int amountHealed = 0)
            {
                Desc = $"Second Wind - healed {amountHealed}hp";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public int KiPoints { get; set; } = 0;
        public int SuperiorityDice { get; set; } = 0;

        public bool UsedActionSurge { get; set; } = false;
        public bool UsedSecondWind { get; set; } = false;

        public int LastStunTarget { get; set; } = -1;

        public AmxikasRi() : base()
        {
            Name = "Amxikas";
            AC = 18;
            Health = 80;
            MaxHealth = 80;
            HealingThreshold = 24;
            Group = Team.TeamOne;
            Healer = true;
            Priority = HealPriority.Medium;
            InitMod = 5;
            WarCaster = false;
            MyType = CreatureType.PC;
            OpportunityAttackChance = 100;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 4 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 5 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = 1 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 16, Mod = 0, Save = 3 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 10, Mod = 0, Save = 0 });
        }

        public override void Init()
        {
            base.Init();

            KiPoints = 5;
            SuperiorityDice = 4;

            UsedActionSurge = false;
            UsedSecondWind = false;
        }

        public override void OnNewEncounter()
        {
        }

        public override BaseAction PickAction()
        {
            int total = 2;
            if (!UsedActionSurge)
            {
                total = 4;
                UsedActionSurge = true;
            }

            return new Longsword { Time = BaseAction.ActionTime.Action, TotalToRun = total, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (!UsedSecondWind && Health <= HealingThreshold)
            {
                UsedSecondWind = true;
                int amount = Dice.D10() + 4;
                Heal(amount);
                return new SecondWindActivate(amount);
            }

            if (KiPoints > 0)
            {
                KiPoints--;
                Stats.KiUsed++;
                return new FlurryOfBlows { Time = BaseAction.ActionTime.BonusAction, TotalToRun = 2, parent = this };
            }

            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override BaseAction PickReaction(bool opportunityAttack)
        {
            if (Dice.D100() <= 20)
            {
                Stats.OpportunityAttacks++;
                return new Longsword { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
            }

            if (SuperiorityDice > 0 && Dice.D100() <= 40)
            {
                SuperiorityDice--;
                Stats.OpportunityAttacks++;
                Stats.FeatureDiceUsed++;
                return new LongswordBrace { Time = BaseAction.ActionTime.Reaction, TotalToRun = 1, parent = this };
            }

            return new NoAction { Time = BaseAction.ActionTime.Reaction };
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();

            IsHidden = false;
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();
        }

        public override void OnDeath()
        {
            base.OnDeath();
        }
    }
}
