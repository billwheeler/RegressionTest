using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Amxikas : BaseCharacter
    {
        public class Longsword : AmxikasBaseWeapon
        {
            public Longsword()
            {
                Desc = "Longsword";
            }

            public override int Amount()
            {
                int damage = (Time != ActionTime.BonusAction) ?
                    Dice.D10(CriticalHit ? 2 : 1) :
                    Dice.D4(CriticalHit ? 2 : 1);

                damage += TallyBuffs();

                return damage + Modifier;
            }
        }

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
            public Amxikas parent { get; set; }

            private string _desc = string.Empty;
            private bool _smitedThisTurn = false;

            // some heuristics for smiting
            private bool enemyIsNemesis = false;
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

                    if (enemyIsNemesis && parent.VowOfEnmityRunning)
                    {
                        output += " (enmity)";
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

                enemyIsNemesis = false;
                enemyIsUndead = target.IsUndead;
                enemyIsFiend = target.IsFiend;

                if (parent.Context.GetIndexByID(target.ID) == parent.MyNemesis)
                {
                    RollType = AbilityRoll.Advantage;
                    enemyIsNemesis = true;
                }
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

                    if (enemyIsNemesis)
                        percentToSmite = 40;
                    else if (enemyIsUndead)
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
            public Amxikas parent { get; set; }

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

        public class VowOfEnmityActivate : BaseAction
        {
            public VowOfEnmityActivate()
            {
                Desc = "Vow of Enmity";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
                IsMagical = true;
            }

            public override int Amount()
            {
                return 0;
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

        public bool CanVowOfEnmity { get; set; } = false;
        public bool VowOfEnmityRunning { get; set; } = false;
        public bool CanSpiritShroud { get; set; } = false;
        public bool SpiritShroudRunning { get; set; } = false;

        public bool ShouldUseSmites { get; set; } = false;
        public int LayOnHandsPool { get; set; } = 45;
        public int MyNemesis { get; set; } = -1;

        public Amxikas() : base()
        {
            Name = "Amxikas";
            AC = 18;
            Health = 76;
            MaxHealth = 76;
            HealingThreshold = 24;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 5;
            WarCaster = true;
            MyType = CreatureType.PC;
            OpportunityAttackChance = 60;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 3 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 8 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 14, Mod = 2, Save = 5 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 8, Mod = -1, Save = 2 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 10, Mod = 0, Save = 7 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 16, Mod = 3, Save = 10 });
        }

        public override void Init()
        {
            base.Init();
            SpiritShroudRunning = false;
            VowOfEnmityRunning = false;

            ShouldUseSmites = true;
            CanVowOfEnmity = true;
        }

        public override void OnNewEncounter()
        {
            MyNemesis = Context.PickHighValueTarget(Group);
            CanVowOfEnmity = MyNemesis != -1;
        }

        public override BaseAction PickAction()
        {
            if (HealTarget != null && LayOnHandsPool > 0 && Dice.D100() <= 33)
            {
                return new LayOnHands { parent = this };
            }

            return new RevenantBlade { Time = BaseAction.ActionTime.Action, TotalToRun = 2, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (CanVowOfEnmity && !VowOfEnmityRunning)
            {
                CanVowOfEnmity = false;
                VowOfEnmityRunning = true;
                return new VowOfEnmityActivate();
            }

            if (CanSpiritShroud && !Concentrating && !SpiritShroudRunning)
            {
                Stats.SpellsUsed++;
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

            if (CanVowOfEnmity && !VowOfEnmityRunning)
            {
                BonusActionFirst = true;
            }
            else if (CanSpiritShroud && !Concentrating && !SpiritShroudRunning)
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

            VowOfEnmityRunning = false;
            SpiritShroudRunning = false;
        }
    }
}
