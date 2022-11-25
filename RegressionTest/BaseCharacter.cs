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
        public virtual int AC { get; set; } = 10;
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
        public bool HasElvenAccuracy { get; set; } = false;
        public bool HasAdvantageOnInitiative { get; set; } = false;
        public bool BonusActionFirst { get; set; } = false;

        public TargetPriority Value { get; set; } = TargetPriority.Medium;

        public bool HighValueTarget { get; set; } = false;
        public bool IsDodging { get; set; } = false;
        public bool IsHidden { get; set; } = false;
        public bool HasShieldRunning { get; set; } = false;
        public bool HasReactionSave { get; set; } = false;
        public bool GiftOfAlacrity { get; set; } = false;

        public Encounter Context { get; set; } = null;

        public CreatureType MyType { get; set; } = CreatureType.NPC;

        public BaseCharacter MySummoner { get; set; } = null;
        public bool BeenSummoned { get; set; } = false;

        public Dictionary<SpellEffectType, SpellEffect> ActiveEffects { get; set; } = new Dictionary<SpellEffectType, SpellEffect>();

        public bool DebugOutput { get; set; } = false;

        public bool IsUndead { get; set; } = false;
        public bool IsFiend { get; set; } = false;

        public bool IsObject { get; set; } = false;

        public bool ResistsNonmagical { get; set; } = false;
        public bool Incapacitated
        {
            get
            {
                if (ActiveEffects[SpellEffectType.HypnoticPattern].Active)
                    return true;

                return false;
            }
        }

        public int Rank
        {
            get
            {
                int rank = 50000;
                switch (Value)
                {
                    case TargetPriority.Highest:
                        rank = 7500;
                        break;
                    case TargetPriority.High:
                        rank = 10000;
                        break;
                    case TargetPriority.Medium:
                        rank = 25000;
                        break;
                    case TargetPriority.Low:
                        rank = 37500;
                        break;
                    case TargetPriority.Lowest:
                        rank = 45000;
                        break;
                }

                if (HasUndesirableEffect())
                    rank += 40000;

                if (Group != Team.TeamOne)
                    rank -= Initiative * 100;

                return rank;
            }
        }

        public enum ActionAvailability
        {
            Any = 0,
            None = 1,
            OnlyDodge = 2
        }

        public virtual ActionAvailability CanTakeActions()
        {
            if (ActiveEffects[SpellEffectType.HypnoticPattern].Active)
                return ActionAvailability.None;

            if (ActiveEffects[SpellEffectType.Turned].Active)
                return ActionAvailability.OnlyDodge;

            if (ActiveEffects[SpellEffectType.Stunned].Active)
                return ActionAvailability.None;

            if (ActiveEffects[SpellEffectType.PsychicLance].Active)
                return ActionAvailability.None;

            if (ActiveEffects[SpellEffectType.MindWhip].Active)
                return ActionAvailability.None;

            if (ActiveEffects[SpellEffectType.Confusion].Active)
            {
                return Dice.D10() < 8 ? 
                    ActionAvailability.None :
                    ActionAvailability.Any;
            }

            return ActionAvailability.Any;
        }

        public virtual bool CanOpportunityAttack()
        {
            if (ActiveEffects[SpellEffectType.HypnoticPattern].Active)
                return false;

            if (ActiveEffects[SpellEffectType.Turned].Active)
                return false;

            if (ActiveEffects[SpellEffectType.Stunned].Active)
                return false;

            if (ActiveEffects[SpellEffectType.PsychicLance].Active)
                return false;

            if (ActiveEffects[SpellEffectType.MindWhip].Active)
                return false;

            if (ActiveEffects[SpellEffectType.Confusion].Active)
                return false;

            return true;
        }

        public bool HasUndesirableEffect()
        {
            if (ActiveEffects[SpellEffectType.HypnoticPattern].Active)
                return true;

            if (ActiveEffects[SpellEffectType.Turned].Active)
                return true;

            return false;
        }

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
                Type = SpellEffectType.Bane,
                SaveType = SpellEffectSave.EachRound
            });

            ActiveEffects.Add(SpellEffectType.Bless, new SpellEffect
            {
                Ability = AbilityScore.Strength,
                Active = false,
                DC = 0,
                Name = "Bless",
                Type = SpellEffectType.Bless,
                SaveType = SpellEffectSave.Never
            });

            ActiveEffects.Add(SpellEffectType.HypnoticPattern, new SpellEffect
            {
                Ability = AbilityScore.Wisdom,
                Active = false,
                DC = 0,
                Name = "Hypnotic Pattern",
                Type = SpellEffectType.HypnoticPattern,
                SaveType = SpellEffectSave.Once
            });

            ActiveEffects.Add(SpellEffectType.SynapticStatic, new SpellEffect
            {
                Ability = AbilityScore.Intelligence,
                Active = false,
                DC = 0,
                Name = "Synaptic Static",
                Type = SpellEffectType.SynapticStatic,
                SaveType = SpellEffectSave.EachRound
            });

            ActiveEffects.Add(SpellEffectType.UnsettlingWords, new SpellEffect
            {
                Ability = AbilityScore.Strength,
                Active = false,
                DC = 0,
                Name = "Unsettling Words",
                Type = SpellEffectType.UnsettlingWords,
                SaveType = SpellEffectSave.EndsAfterOneRound
            });

            ActiveEffects.Add(SpellEffectType.Inspired, new SpellEffect
            {
                Ability = AbilityScore.Strength,
                Active = false,
                DC = 0,
                Name = "Inspired",
                Type = SpellEffectType.Inspired,
                SaveType = SpellEffectSave.Never
            });

            ActiveEffects.Add(SpellEffectType.BlackTentacles, new SpellEffect
            {
                Ability = AbilityScore.Dexterity,
                Active = false,
                DC = 0,
                Name = "Black Tentacles",
                Type = SpellEffectType.BlackTentacles,
                SaveType = SpellEffectSave.EachRound
            });

            ActiveEffects.Add(SpellEffectType.ConqueringPresense, new SpellEffect
            {
                Ability = AbilityScore.Wisdom,
                Active = false,
                DC = 0,
                Name = "Conquering Presense",
                Type = SpellEffectType.ConqueringPresense,
                SaveType = SpellEffectSave.EachRound
            });

            ActiveEffects.Add(SpellEffectType.Bladesong, new SpellEffect
            {
                Ability = AbilityScore.Dexterity,
                Active = false,
                DC = 0,
                Name = "Bladesong",
                Type = SpellEffectType.Bladesong,
                SaveType = SpellEffectSave.Never
            });

            ActiveEffects.Add(SpellEffectType.Turned, new SpellEffect
            {
                Ability = AbilityScore.Wisdom,
                Active = false,
                DC = 0,
                Name = "Turned",
                Type = SpellEffectType.Turned,
                SaveType = SpellEffectSave.Once
            });

            ActiveEffects.Add(SpellEffectType.Stunned, new SpellEffect
            {
                Ability = AbilityScore.Constitution,
                Active = false,
                DC = 0,
                Name = "Stunned",
                Type = SpellEffectType.Stunned,
                SaveType = SpellEffectSave.EachRound
            });

            ActiveEffects.Add(SpellEffectType.PsychicLance, new SpellEffect
            {
                Ability = AbilityScore.Intelligence,
                Active = false,
                DC = 0,
                Name = "Psychic Lance",
                Type = SpellEffectType.PsychicLance,
                SaveType = SpellEffectSave.EndsAfterOneRound
            });

            ActiveEffects.Add(SpellEffectType.MindWhip, new SpellEffect
            {
                Ability = AbilityScore.Intelligence,
                Active = false,
                DC = 0,
                Name = "Mind Whip",
                Type = SpellEffectType.MindWhip,
                SaveType = SpellEffectSave.EndsAfterOneRound
            });

            ActiveEffects.Add(SpellEffectType.Confusion, new SpellEffect
            {
                Ability = AbilityScore.Wisdom,
                Active = false,
                DC = 0,
                Name = "Confusion",
                Type = SpellEffectType.Confusion,
                SaveType = SpellEffectSave.EachRound
            });

            ActiveEffects.Add(SpellEffectType.PhantasmalKiller, new SpellEffect
            {
                Ability = AbilityScore.Wisdom,
                Active = false,
                DC = 0,
                Name = "Phantasmal Killer",
                Type = SpellEffectType.PhantasmalKiller,
                SaveType = SpellEffectSave.EachRound
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

        public virtual void ApplyEffect(SpellEffect effect, BaseCharacter owner)
        {
            ActiveEffects[effect.Type].Active = true;
            ActiveEffects[effect.Type].DC = effect.DC;
            ActiveEffects[effect.Type].Owner = owner;
            ActiveEffects[effect.Type].Ability = effect.Ability;
            ActiveEffects[effect.Type].NewRoundAction = effect.NewRoundAction;
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

                if (Context.HasWatchers)
                    bonus += Proficiency;

                Initiative = Dice.MakeAbilityRoll(HasAdvantageOnInitiative ?
                    HasElvenAccuracy ? AbilityRoll.ElvenAccuracy : AbilityRoll.Advantage :
                    AbilityRoll.Normal) + 
                    InitMod + bonus;
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

        public bool SavingThrow(AbilityScore score, int dc, AbilityRoll rollType = AbilityRoll.Normal, bool isConcentration = false)
        {
            if (Abilities.ContainsKey(score))
            {
                if (ActiveEffects[SpellEffectType.ConqueringPresense].Active)
                {
                    if (rollType == AbilityRoll.Advantage || rollType == AbilityRoll.ElvenAccuracy)
                        rollType = AbilityRoll.Normal;
                    else
                        rollType = AbilityRoll.Disadvantage;
                }

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

                if (isConcentration && ActiveEffects[SpellEffectType.Bladesong].Active)
                {
                    roll += 5;
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

            bool result = SavingThrow(AbilityScore.Constitution, dc, WarCaster ?
                HasElvenAccuracy ? AbilityRoll.ElvenAccuracy : AbilityRoll.Advantage : 
                AbilityRoll.Normal, true);
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
            if (amount > 0 && ActiveEffects[SpellEffectType.HypnoticPattern].Active)
            {
                ActiveEffects[SpellEffectType.HypnoticPattern].Active = false;
            }

            if (amount > 0 && ActiveEffects[SpellEffectType.Turned].Active)
            {
                ActiveEffects[SpellEffectType.Turned].Active = false;
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

            if (ActiveEffects[SpellEffectType.Turned].Active)
                nameDesc += " (turnt)";

            if (ActiveEffects[SpellEffectType.HypnoticPattern].Active)
                nameDesc += " (hypnotized)";

            if (ActiveEffects[SpellEffectType.SynapticStatic].Active)
                nameDesc += " (static)";

            if (ActiveEffects[SpellEffectType.ConqueringPresense].Active)
                nameDesc += " (fear)";

            if (ActiveEffects[SpellEffectType.Stunned].Active)
                nameDesc += " (stunned)";

            if (ActiveEffects[SpellEffectType.Confusion].Active)
                nameDesc += " (confused)";

            if (ActiveEffects[SpellEffectType.PhantasmalKiller].Active)
                nameDesc += " (pk)";

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

        public virtual void OnNewRound()
        {
            IsDodging = false;
            UsedAction = false;
            UsedBonusAction = false;
            UsedReaction = false;
        }

        public virtual void OnNewTurn()
        {
            HasShieldRunning = false;
            HasReactionSave = false;
        }

        public string OnEndTurn()
        {
            string output = string.Empty;

            foreach (KeyValuePair<SpellEffectType, SpellEffect> kvp in ActiveEffects)
            {
                SpellEffect effect = kvp.Value;

                if (effect.Active)
                {
                    if (effect.SaveType == SpellEffectSave.EachRound)
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
                    else if (effect.SaveType == SpellEffectSave.EndsAfterOneRound)
                    {
                        output = $"{Name} [{GetHealthDesc()}] has {effect.Name} effect end.";
                        ActiveEffects[effect.Type].Active = false;
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
            HasReactionSave = false;
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
