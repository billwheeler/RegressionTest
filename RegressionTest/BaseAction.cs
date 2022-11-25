using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public abstract class BaseAction: IDisposable
    {
        public virtual string Desc { get; set; }
 
        public DiceRoller Dice { get; set; } = new DiceRoller();

        public int MinTargets { get; set; } = 1;
        public int MaxTargets { get; set; } = 1;
        
        public int AttackModifier { get; set; } = 0;
        public int Modifier { get; set; } = 0;
        
        public int TotalToRun { get; set; } = 1;
        public int CurrentRunning { get; set; } = 0;
        public int CurrentHits { get; set; } = 0;
        public bool HasCritted { get; set; } = false;

        public bool CriticalHit { get; set; } = false;
        public int CriticalThreshold { get; set; } = 20;
        public bool HalfDamageOnMiss { get; set; } = false;
        public bool Damageless { get; set; } = false;

        public AbilityRoll RollType { get; set; } = AbilityRoll.Normal;

        public AbilityScore Ability { get; set; } = AbilityScore.Wisdom;
        public int DC { get; set; } = 10;

        public BaseAction ApplyEffectAfter { get; set; } = null;

        public SpellEffect EffectToApply { get; set; } = null;

        public bool IsMagical { get; set; } = false;

        public bool PotentiallyPowerful { get; set; } = false;

        public enum ActionType
        {
            None,
            Activate,
            Heal,
            GrantTempHp,
            MeleeAttack,
            RangedAttack,
            SpellAttack,
            SpellSave,
            Apply,
            NewRound
        }

        public ActionType Type { get; set; } = ActionType.MeleeAttack;
        
        public enum ActionTime
        {
            Action,
            BonusAction,
            Reaction,
            PreTurn,    // Spirit Guardians, etc
            PostTurn    // Legendary Actions, Twilight Sanctuary, etc
        }

        public ActionTime Time { get; set; } = ActionTime.Action;

        public enum DamageAmount
        {
            None,
            Half,
            Full
        }

        public DamageAmount Result { get; set; } = DamageAmount.None;

        public virtual void PreHit(BaseCharacter attacker, BaseCharacter target)
        {
        }

        public virtual bool Hits(BaseCharacter attacker, BaseCharacter target)
        {
            CriticalHit = false;
            bool hits = false;
            switch (Type)
            {
                case ActionType.MeleeAttack:
                case ActionType.RangedAttack:
                case ActionType.SpellAttack:
                    hits = AttackType(attacker, target);
                    Result = hits ? DamageAmount.Full : DamageAmount.None;
                    break;

                case ActionType.Apply:
                    hits = true;
                    if (EffectToApply != null)
                    {
                        target.ApplyEffect(EffectToApply, attacker);
                    }

                    Result = DamageAmount.None;
                    break;

                case ActionType.SpellSave:
                    hits = !target.SavingThrow(Ability, DC);
                    if (hits && EffectToApply != null)
                    {
                        target.ApplyEffect(EffectToApply, attacker);
                    }

                    if (HalfDamageOnMiss)
                    {
                        Result = hits ? DamageAmount.Full : DamageAmount.Half;
                        hits = true;
                    }
                    else
                    {
                        Result = hits ? DamageAmount.Full : DamageAmount.None;
                    }
                    break;

                case ActionType.Heal:
                    hits = true;
                    Result = DamageAmount.Full;
                    break;

                case ActionType.GrantTempHp:
                    hits = true;
                    Result = DamageAmount.Full;
                    break;

                default:
                    hits = false;
                    Result = DamageAmount.None;
                    break;
            }

            return hits;
        }

        protected virtual bool AttackType(BaseCharacter attacker, BaseCharacter target)
        {
            CriticalHit = false;
            var abilityRoll = RollType;

            if (target.IsHidden)
            {
                switch (abilityRoll)
                {
                    case AbilityRoll.Advantage:
                    case AbilityRoll.ElvenAccuracy:
                        abilityRoll = AbilityRoll.Normal;
                        break;
                    default:
                        abilityRoll = AbilityRoll.Disadvantage;
                        break;
                }
            }

            if (target.IsDodging)
            {
                switch (abilityRoll)
                {
                    case AbilityRoll.Advantage:
                    case AbilityRoll.ElvenAccuracy:
                        abilityRoll = AbilityRoll.Normal;
                        break;
                    default:
                        abilityRoll = AbilityRoll.Disadvantage;
                        break;
                }
            }

            if (attacker.ActiveEffects[SpellEffectType.ConqueringPresense].Active)
            {
                switch (abilityRoll)
                {
                    case AbilityRoll.Advantage:
                    case AbilityRoll.ElvenAccuracy:
                        abilityRoll = AbilityRoll.Normal;
                        break;
                    default:
                        abilityRoll = AbilityRoll.Disadvantage;
                        break;
                }
            }

            if (target.ActiveEffects[SpellEffectType.Stunned].Active)
            {
                switch (abilityRoll)
                {
                    case AbilityRoll.Disadvantage:
                        abilityRoll = AbilityRoll.Normal;
                        break;
                    default:
                        abilityRoll = attacker.HasElvenAccuracy ? AbilityRoll.ElvenAccuracy : AbilityRoll.Advantage;
                        break;
                }
            }

            if (attacker.IsHidden)
            {
                switch (abilityRoll)
                {
                    case AbilityRoll.Disadvantage:
                        abilityRoll = AbilityRoll.Normal;
                        break;
                    default:
                        abilityRoll = attacker.HasElvenAccuracy ? AbilityRoll.ElvenAccuracy : AbilityRoll.Advantage;
                        break;
                }
            }


            int mod = AttackModifier;
            int roll = Dice.MakeAbilityRoll(abilityRoll);

            if (attacker.ActiveEffects[SpellEffectType.Bless].Active)
            {
                roll += Dice.D4();
            }

            if (attacker.ActiveEffects[SpellEffectType.Bane].Active)
            {
                roll -= Dice.D4();
            }

            if (attacker.ActiveEffects[SpellEffectType.SynapticStatic].Active)
            {
                roll -= Dice.D6();
            }

            if (attacker.ActiveEffects[SpellEffectType.Inspired].Active)
            {
                roll += Dice.D8();
                attacker.ActiveEffects[SpellEffectType.Inspired].Active = false;
                attacker.ActiveEffects[SpellEffectType.Inspired].DC = 0;
            }

            if (attacker.DebugOutput)
            {
                Console.WriteLine($"{attacker.Name} rolled with {EnumDesc.AbilityRoll(abilityRoll)}, for a result of {roll}");
            }

            if (roll >= CriticalThreshold)
            {
                CriticalHit = true;
                HasCritted = true;
            }

            target.PreHitCalc(roll, mod, PotentiallyPowerful, CriticalHit);

            int armorClass = target.AC;
            if (target.HasShieldRunning)
                armorClass += 5;

            if (target.HasReactionSave)
            {
                armorClass += Dice.D6();
                target.HasReactionSave = false;
            }

            var result = (roll + mod) >= armorClass ? true : false;

            return result;
        }

        public virtual int Amount()
        {
            return 0;
        }

        public string HitDesc()
        {
            if (Type == ActionType.SpellSave)
            {
                return Result == DamageAmount.Full ? "failed save" : "made save";
            }

            if (CriticalHit)
                return "critical hit";

            if (Result == DamageAmount.Full)
                return "hits";

            return "misses";
        }

        public void Dispose()
        {
        }

        public bool ShouldPowerAttack(int targetAC, int lowerBound, int upperBound)
        {
            switch (RollType)
            {
                case AbilityRoll.Advantage:
                    lowerBound += 5;
                    upperBound += 5;
                    break;
                case AbilityRoll.ElvenAccuracy:
                    lowerBound += 8;
                    upperBound += 8;
                    break;
                case AbilityRoll.Disadvantage:
                    lowerBound -= 5;
                    upperBound -= 5;
                    break;
            }

            if (targetAC >= lowerBound && targetAC <= upperBound)
            {
                // we're in the questionable range, play it safe for the first attack
                if (CurrentRunning == 1)
                    return false;

                // if we had at least one hit, go for it
                if (CurrentRunning >= 2 && CurrentHits >= 1)
                    return true;

                return false;
            }

            if (targetAC < lowerBound)
            {
                return true;
            }

            return false;
        }
    }

    public abstract class SpellAction: BaseAction
    {
        public enum SpellLevel
        {
            Cantrip = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9
        }

        public SpellLevel Level { get; set; }

        public SpellLevel DesiredLevel(SpellLevel level)
        {
            if (Level == SpellLevel.Cantrip)
                return SpellLevel.Cantrip;

            if (level <= Level)
                return Level;

            return level;
        }
    }

    public class NoAction : BaseAction
    {
        public NoAction()
        {
            Desc = "(Nothing)";
            Type = ActionType.None;
        }
    }

    public class CureWounds : SpellAction
    {
        public CureWounds()
        {
            Desc = "Cure Wounds";
            Level = SpellLevel.One;
            Type = ActionType.Heal;
            Time = ActionTime.Action;
        }

        public override int Amount()
        {
            int die = (int)Level;
            return Dice.D8(die) + Modifier;
        }
    }

    public class HealingWord : SpellAction
    {
        public HealingWord()
        {
            Desc = "Healing Word";
            Level = SpellLevel.One;
            Type = ActionType.Heal;
            Time = ActionTime.BonusAction;
        }

        public override int Amount()
        {
            int die = (int)Level;
            return Dice.D4(die) + Modifier;
        }
    }

    public class GuidingBolt : SpellAction
    {
        public GuidingBolt()
        {
            Desc = "Guiding Bolt";
            Level = SpellLevel.One;
            Type = ActionType.SpellAttack;
        }

        public override int Amount()
        {
            int die = 4;

            return Dice.D6(die) + Modifier;
        }
    }

    public class DodgeAction : SpellAction
    {
        public DodgeAction()
        {
            Desc = "Dodge";
            Level = SpellLevel.One;
            Type = ActionType.Activate;
        }
    }

    public class RunsAwayAction : SpellAction
    {
        public RunsAwayAction()
        {
            Desc = "Runs Away";
            Level = SpellLevel.One;
            Type = ActionType.Activate;
        }
    }

    public class NothingAction : SpellAction
    {
        public NothingAction()
        {
            Desc = "Nothing";
            Level = SpellLevel.One;
            Type = ActionType.Activate;
        }
    }

    public class SynapticStatic : BaseAction
    {
        public SynapticStatic(int dc = 17)
        {
            Desc = "Synaptic Static";
            Type = ActionType.SpellSave;
            Time = ActionTime.Action;
            Ability = AbilityScore.Intelligence;
            HalfDamageOnMiss = true;
            MinTargets = 3;
            MaxTargets = 5;
            DC = dc;

            EffectToApply = new SpellEffect
            {
                Ability = AbilityScore.Intelligence,
                DC = dc,
                Name = "Synaptic Static",
                Type = SpellEffectType.SynapticStatic
            };
        }

        public override int Amount()
        {
            return Dice.D6(8);
        }
    }

    public class PhantasmalKiller : BaseAction
    {
        public PhantasmalKiller(BaseCharacter owner, int dc = 17)
        {
            Desc = "Phantasmal Killer";
            Type = ActionType.SpellSave;
            Time = ActionTime.Action;
            Ability = AbilityScore.Wisdom;
            HalfDamageOnMiss = false;
            MinTargets = 1;
            MaxTargets = 1;
            DC = dc;

            EffectToApply = new SpellEffect
            {
                Ability = AbilityScore.Wisdom,
                DC = dc,
                Name = "Phantasmal Killer",
                Type = SpellEffectType.PhantasmalKiller,
                SaveType = SpellEffectSave.EachRound,
                NewRoundAction = new PhantasmalKillerApply(),
                Owner = owner
            };
        }

        public override int Amount()
        {
            return Dice.D10(4);
        }
    }

    public class PhantasmalKillerApply : BaseAction
    {
        public PhantasmalKillerApply()
        {
            Desc = "Phantasmal Killer";
            Type = ActionType.NewRound;
            IsMagical = true;
        }

        public override int Amount()
        {
            return Dice.D10(4);
        }
    }

    public class ScorchingRay : BaseAction
    {
        public ScorchingRay(int attackMod = 0)
        {
            Desc = "Scorching Ray";
            Type = ActionType.SpellAttack;
            Time = ActionTime.Action;
            AttackModifier = attackMod;
            Modifier = 0;
            TotalToRun = 3;
            IsMagical = true;
        }

        public override int Amount()
        {
            int damage = Dice.D6(CriticalHit ? 4 : 2);
            return damage + Modifier;
        }
    }

    public class MindWhip : BaseAction
    {
        public MindWhip(int dc = 17)
        {
            Desc = "Mind Whip";
            Type = ActionType.SpellSave;
            Time = ActionTime.Action;
            Ability = AbilityScore.Intelligence;
            HalfDamageOnMiss = true;
            Damageless = false;
            MinTargets = 1;
            MaxTargets = 1;
            DC = dc;

            EffectToApply = new SpellEffect
            {
                Ability = AbilityScore.Intelligence,
                DC = dc,
                Name = "Mind Whip",
                Type = SpellEffectType.MindWhip,
                SaveType = SpellEffectSave.EndsAfterOneRound
            };
        }

        public override int Amount()
        {
            return Dice.D6(3);
        }
    }

    public class PsychicLance : BaseAction
    {
        public PsychicLance(int dc = 17)
        {
            Desc = "Psychic Lance";
            Type = ActionType.SpellSave;
            Time = ActionTime.Action;
            Ability = AbilityScore.Intelligence;
            HalfDamageOnMiss = true;
            Damageless = false;
            MinTargets = 1;
            MaxTargets = 1;
            DC = dc;

            EffectToApply = new SpellEffect
            {
                Ability = AbilityScore.Intelligence,
                DC = dc,
                Name = "Psychic Lance",
                Type = SpellEffectType.PsychicLance,
                SaveType = SpellEffectSave.EndsAfterOneRound
            };
        }

        public override int Amount()
        {
            return Dice.D6(7);
        }
    }

    public class Confusion : BaseAction
    {
        public Confusion(int dc = 17)
        {
            Desc = "Confusion";
            Type = ActionType.SpellSave;
            Time = ActionTime.Action;
            Ability = AbilityScore.Wisdom;
            Damageless = true;
            MinTargets = 2;
            MaxTargets = 4;
            DC = dc;

            EffectToApply = new SpellEffect
            {
                Ability = AbilityScore.Wisdom,
                DC = dc,
                Name = "Confusion",
                Type = SpellEffectType.Confusion
            };
        }
    }

    public class HypnoticPattern : BaseAction
    {
        public HypnoticPattern(int dc = 17)
        {
            Desc = "Hypnotic Pattern";
            Type = ActionType.SpellSave;
            Time = ActionTime.Action;
            Ability = AbilityScore.Wisdom;
            Damageless = true;
            MinTargets = 2;
            MaxTargets = 6;
            DC = dc;

            EffectToApply = new SpellEffect
            {
                Ability = AbilityScore.Wisdom,
                DC = dc,
                Name = "Hypnotic Pattern",
                Type = SpellEffectType.HypnoticPattern
            };
        }
    }

    public class BlackTentacles : BaseAction
    {
        public BlackTentacles(BaseCharacter owner, int dc = 17)
        {
            Desc = "Black Tentacles";
            Type = ActionType.SpellSave;
            Time = ActionTime.Action;
            Ability = AbilityScore.Dexterity;
            Damageless = false;
            MinTargets = 2;
            MaxTargets = 6;
            DC = dc;

            EffectToApply = new SpellEffect
            {
                Ability = AbilityScore.Wisdom,
                DC = dc,
                Name = "Black Tentacles",
                Type = SpellEffectType.BlackTentacles,
                SaveType = SpellEffectSave.EachRound,
                NewRoundAction = new BlackTentaclesApply(),
                Owner = owner
            };
        }

        public override int Amount()
        {
            return Dice.D6(3);
        }
    }

    public class BlackTentaclesApply : BaseAction
    {
        public BlackTentaclesApply()
        {
            Desc = "Black Tentacles";
            Type = ActionType.NewRound;
            IsMagical = true;
        }

        public override int Amount()
        {
            return Dice.D6(3);
        }
    }
}
