using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Saves
    {
        public int Strength { get; set; } = 0;
        public int Dexterity { get; set; } = 0;
        public int Constitution { get; set; } = 0;
        public int Intelligence { get; set; } = 0;
        public int Wisdom { get; set; } = 0;
        public int Charisma { get; set; } = 0;
    }

    public enum AbilityScore
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma
    }

    public class Stat
    {
        public int Score { get; set; } = 10;
        public int Mod { get; set; } = 0;
        public int Save { get; set; } = 0;
    }

    public enum CreatureType
    {
        PC,
        NPC,
        Summon
    }

    public abstract class BaseCharacter
    {
        public int ID { get; set; } = 0;
        public string Name { get; set; }
        public int AC { get; set; } = 10;
        public int InitMod { get; set; } = 0;
        public int Initiative { get; set; } = 0;
        public int HealingThreshold { get; set; } = 0;
        public int Health { get; set; } = 5;
        public int MaxHealth { get; set; } = 5;
        public int TempHitPoints { get; set; } = 0;
        public bool Alive { get; set; } = true;
        public bool Healer { get; set; } = false;
        public BaseCharacter HealTarget { get; set; } = null;
        public Team Group { get; set; }
        public HealPriority Priority { get; set; } = HealPriority.Dont;
        public bool Concentrating { get; set; } = false;
        public int Proficiency { get; set; } = 4;

        public int OpportunityAttackChance { get; set; } = 0;

        public bool UsedReaction { get; set; } = false;
        public bool UsedAction { get; set; } = false;
        public bool UsedBonusAction { get; set; } = false;

        public Saves Scores { get; set; } = new Saves();

        public DiceRoller Dice { get; set; } = new DiceRoller();
        public CharacterStats Stats { get; set; } = new CharacterStats();

        public bool PreTurnNotify { get; set; } = false;
        public bool PostTurnNotify { get; set; } = false;

        public Dictionary<AbilityScore, Stat> Abilities = new Dictionary<AbilityScore, Stat>();

        public bool WarCaster { get; set; } = false;
        public bool HasAdvantageOnInitiative { get; set; } = false;
        public bool BonusActionFirst { get; set; } = false;

        public bool HighValueTarget { get; set; } = false;
        public bool IsDodging { get; set; } = false;
        public bool IsHidden { get; set; } = false;
        public bool HasShieldRunning { get; set; } = false;

        public bool GiftOfAlacrity { get; set; } = false;

        public Encounter Context { get; set; } = null;

        public CreatureType MyType { get; set; } = CreatureType.NPC;

        public BaseCharacter MySummoner { get; set; } = null;
        public bool BeenSummoned { get; set; } = false;

        public Dictionary<SpellEffectType, SpellEffect> ActiveEffects { get; set; } = new Dictionary<SpellEffectType, SpellEffect>();

        public bool DebugOutput { get; set; } = false;

        public bool IsUndead { get; set; } = false;
        public bool IsFiend { get; set; } = false;

        public bool ResistsNonmagical { get; set; } = false;
        public bool Incapacitated { get; set; } = false;

        public BaseCharacter()
        {
            AddBaseEffects();
        }

        public void AddBaseEffects()
        {
            ActiveEffects.Add(SpellEffectType.Bane, new SpellEffect
            {
                Ability = AbilityScore.Charisma,
                Active = false,
                DC = 0,
                Name = "Bane",
                Type = SpellEffectType.Bane
            });

            ActiveEffects.Add(SpellEffectType.Bless, new SpellEffect
            {
                Ability = AbilityScore.Strength,
                Active = false,
                DC = 0,
                Name = "Bless",
                Type = SpellEffectType.Bless
            });

            ActiveEffects.Add(SpellEffectType.HypnoticPattern, new SpellEffect
            {
                Ability = AbilityScore.Wisdom,
                Active = false,
                DC = 0,
                Name = "Hypnotic Pattern",
                Type = SpellEffectType.HypnoticPattern
            });

            ActiveEffects.Add(SpellEffectType.SynapticStatic, new SpellEffect
            {
                Ability = AbilityScore.Intelligence,
                Active = false,
                DC = 0,
                Name = "Synaptic Static",
                Type = SpellEffectType.SynapticStatic
            });

            ActiveEffects.Add(SpellEffectType.UnsettlingWords, new SpellEffect
            {
                Ability = AbilityScore.Strength,
                Active = false,
                DC = 0,
                Name = "Unsettling Words",
                Type = SpellEffectType.UnsettlingWords
            });

            ActiveEffects.Add(SpellEffectType.Inspired, new SpellEffect
            {
                Ability = AbilityScore.Strength,
                Active = false,
                DC = 0,
                Name = "Inspired",
                Type = SpellEffectType.Inspired
            });

            ActiveEffects.Add(SpellEffectType.BlackTentacles, new SpellEffect
            {
                Ability = AbilityScore.Dexterity,
                Active = false,
                DC = 0,
                Name = "Black Tentacles",
                Type = SpellEffectType.BlackTentacles
            });
        }

        public virtual BaseAttack PickAttack()
        {
            return null;
        }

        public virtual void Init()
        {
            Initiative = 0;
            Health = MaxHealth;
            Alive = true;
            TempHitPoints = 0;
            HealTarget = null;
            HasAdvantageOnInitiative = false;
            Concentrating = false;
            IsDodging = false;

            ResetEffects();
        }

        protected void ResetEffects()
        {
            foreach (KeyValuePair<SpellEffectType, SpellEffect> kvp in ActiveEffects)
            {
                SpellEffect effect = kvp.Value;
                ActiveEffects[effect.Type].Active = false;
                ActiveEffects[effect.Type].DC = 0;
            }
        }

        public void ApplyEffect(SpellEffect effect)
        {
            ActiveEffects[effect.Type].Active = true;
            ActiveEffects[effect.Type].DC = effect.DC;

            if (effect.Type == SpellEffectType.HypnoticPattern)
                Incapacitated = true;

            if (effect.Type == SpellEffectType.BlackTentacles)
                Incapacitated = true;
        }

        public virtual void RollInitiative()
        {
            Init();

            if (MySummoner != null)
            {
                Initiative = MySummoner.Initiative;
            }
            else
            {
                int bonus = 0;
                if (GiftOfAlacrity)
                    bonus += Dice.D8(1);

                Initiative = Dice.MakeAbilityRoll(HasAdvantageOnInitiative ? AbilityRoll.Advantage : AbilityRoll.Normal) + InitMod + bonus;
            }
            Stats.Encounters++;
        }

        public virtual void OnNewEncounter()
        {
        }

        public bool NeedsHealing
        {
            get
            {
                if (MyType == CreatureType.Summon)
                    return false;

                if (Priority == HealPriority.Dont)
                    return false;

                return Health < HealingThreshold;
            }
        }

        public bool SavingThrow(AbilityScore score, int dc, AbilityRoll rollType = AbilityRoll.Normal)
        {
            if (Abilities.ContainsKey(score))
            {
                int roll = Dice.MakeAbilityRoll(rollType) + Abilities[score].Save;

                if (ActiveEffects[SpellEffectType.UnsettlingWords].Active)
                {
                    roll -= Dice.D8();
                    ActiveEffects[SpellEffectType.UnsettlingWords].Active = false;
                }

                if (ActiveEffects[SpellEffectType.Bless].Active)
                {
                    roll += Dice.D4();
                }
                
                if (ActiveEffects[SpellEffectType.Bane].Active)
                {
                    roll -= Dice.D4();
                }

                if (ActiveEffects[SpellEffectType.SynapticStatic].Active)
                {
                    roll -= Dice.D6();
                }

                if (ActiveEffects[SpellEffectType.Inspired].Active)
                {
                    roll += Dice.D8();
                }

                return roll >= dc;
            }

            return false;
        }

        public bool ConcentrationCheck(int amount)
        {
            if (Alive == false)
                return false;

            if (!Concentrating)
                return true;

            int dc = (int)Math.Floor(amount / 2.0f);
            if (dc < 10) dc = 10;

            bool result = SavingThrow(AbilityScore.Constitution, dc, WarCaster ? AbilityRoll.Advantage : AbilityRoll.Normal);
            if (!result)
            {
                OnFailConcentration();
            }

            return result;
        }

        public virtual void PreHitCalc(int attackRoll, int modifier, bool potentiallyPowerful, bool criticalHit)
        {
        }

        public virtual bool ShouldUncannyDodge(int amount, BaseAction.ActionType actionType)
        {
            return false;
        }

        public virtual int CalculateResistences(int amount, BaseAction action)
        {
            if (action.Type == BaseAction.ActionType.MeleeAttack || action.Type == BaseAction.ActionType.RangedAttack)
            {
                if (ResistsNonmagical && action.IsMagical == false)
                {
                    amount = (int)Math.Floor((double)amount / 2.0f);
                }
            }

            return amount;
        }

        public bool TakeDamage(int amount)
        {
            if (amount > 0 && Incapacitated)
            {
                Incapacitated = false;
            }

            if (TempHitPoints > 0)
            {
                if (amount >= TempHitPoints)
                {
                    amount -= TempHitPoints;
                    TempHitPoints = 0;
                }
                else
                {
                    TempHitPoints -= amount;
                    return true;
                }
            }

            Health -= amount;
            if (Health <= 0)
            {
                Health = 0;
                Alive = false;
                OnDeath();

                if (MyType == CreatureType.Summon)
                {
                    BeenSummoned = false;
                }

                return false;
            }

            return true;
        }

        public void Heal(int amount)
        {
            Alive = true;
            Health += amount;
            if (Health > MaxHealth)
                Health = MaxHealth;
        }

        public void SetTempHitPoints(int amount)
        {
            if (!Alive) return;
            TempHitPoints = amount;
        }

        public string GetNameDesc()
        {
            string nameDesc = Name;

            if (Concentrating)
                nameDesc += "*";

            if (IsDodging)
                nameDesc += " (dodging)";

            if (IsHidden)
                nameDesc += " (hidden)";

            if (Incapacitated)
                nameDesc += " (hypnotized)";

            return nameDesc;
        }

        public string GetHealthDesc()
        {
            if (Alive)
            {
                if (TempHitPoints > 0)
                    return string.Format("{0}/{1}hp", Health, TempHitPoints);

                return string.Format("{0}hp", Health);
            }

            return "dead";
        }

        public virtual int HealAmount(HealPriority priority)
        {
            return 0;
        }

        public virtual bool OnNewRound()
        {
            IsDodging = false;
            UsedAction = false;
            UsedBonusAction = false;
            UsedReaction = false;

            if (ActiveEffects[SpellEffectType.BlackTentacles].Active)
            {
                return TakeDamage(Dice.D6(3));
            }

            return true;
        }

        public virtual void OnNewTurn()
        {
            HasShieldRunning = false;
        }

        public string OnEndTurn()
        {
            string output = string.Empty;

            foreach (KeyValuePair<SpellEffectType, SpellEffect> kvp in ActiveEffects)
            {
                SpellEffect effect = kvp.Value;

                if ((effect.Type == SpellEffectType.Bane || effect.Type == SpellEffectType.SynapticStatic || effect.Type == SpellEffectType.BlackTentacles) && effect.Active)
                {
                    if (SavingThrow(effect.Ability, effect.DC))
                    {
                        output = $"{Name} [{GetHealthDesc()}] made save against {effect.Name}, effect ended.";
                        ActiveEffects[effect.Type].Active = false;
                    }
                    else
                    {
                        output = $"{Name} [{GetHealthDesc()}] failed save against {effect.Name}, effect remains.";
                    }
                }
            }

            return output;
        }

        public virtual void OnFailConcentration()
        {
            Concentrating = false;
        }

        public virtual void OnDeath()
        {
            Concentrating = false;
            HasShieldRunning = false;
        }

        public virtual BaseAction PickAction()
        {
            return new NoAction { Time = BaseAction.ActionTime.Action };
        }

        public virtual BaseAction PickBonusAction()
        {
            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public virtual BaseAction PickReaction(bool opportunityAttack)
        {
            return new NoAction { Time = BaseAction.ActionTime.Reaction };
        }

        public virtual BaseAction PickPreTurn(BaseCharacter target)
        {
            return new NoAction { Time = BaseAction.ActionTime.PreTurn };
        }

        public virtual BaseAction PickPostTurn(BaseCharacter target)
        {
            return new NoAction { Time = BaseAction.ActionTime.PostTurn };
        }
    }
}
