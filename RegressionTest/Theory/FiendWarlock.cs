using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class FiendWarlock : BaseCharacter
    {
        public class EldritchBlast : BaseAction
        {
            public FiendWarlock parent { get; set; }

            public EldritchBlast()
            {
                Desc = "Eldritch Blast";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 9;
                Modifier = 5;
                TotalToRun = 2;
            }

            public override int Amount()
            {
                int damage = Dice.D10(CriticalHit ? 2 : 1);

                if (parent.HexRunning)
                {
                    damage += Dice.D6(CriticalHit ? 2 : 1);
                }
                
                return damage + Modifier;
            }
        }

        public class HexActivate : BaseAction
        {
            public HexActivate()
            {
                Desc = "Hex";
                Type = ActionType.Activate;
                Time = ActionTime.Action;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public class SynapticStatic : BaseAction
        {
            public SynapticStatic()
            {
                Desc = "Synaptic Static";
                Type = ActionType.SpellSave;
                Time = ActionTime.Action;
                Ability = AbilityScore.Intelligence;
                HalfDamageOnMiss = true;
                MaxTargets = 4;
                DC = 17;

                EffectToApply = new SpellEffect
                {
                    Ability = AbilityScore.Intelligence,
                    DC = 17,
                    Name = "Synaptic Static",
                    Type = SpellEffectType.SynapticStatic
                };
            }

            public override int Amount()
            {
                return Dice.D6(8);
            }
        }

        public bool HexRunning { get; set; } = false;
        public bool CanHex { get; set; } = true;
        public bool CanSynapticStatic { get; set; } = true;
        public bool CastedLeveledSpell { get; set; } = false;

        public FiendWarlock()
        {
            Name = "Lakrissa";
            AC = 18;
            InitMod = 2;
            Health = 83;
            MaxHealth = 83;
            Group = Team.TeamOne;
            HealingThreshold = 18;
            Priority = HealPriority.Medium;
            MyType = CreatureType.PC;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 8, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 12, Mod = 1, Save = 5 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 20, Mod = 5, Save = 9 });
        }

        public override void Init()
        {
            base.Init();

            CanSynapticStatic = true;
            CanHex = true;
            HexRunning = false;
            CastedLeveledSpell = false;
        }

        public override void OnNewRound()
        {
            base.OnNewRound();

            CastedLeveledSpell = false;
        }

        public override BaseAction PickAction()
        {
            if (CanSynapticStatic)
            {
                CanSynapticStatic = false;
                CastedLeveledSpell = true;
                return new SynapticStatic();
            }

            return new EldritchBlast { parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (!CastedLeveledSpell && CanHex && !HexRunning)
            {
                CanHex = false;
                HexRunning = true;
                Concentrating = true;
                return new HexActivate();
            }

            return new NoAction();
        }

        public override void OnNewTurn()
        {
            if (CanSynapticStatic)
            {
                BonusActionFirst = false;
            }
            else if (CanHex && !HexRunning)
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

            HexRunning = false;
        }

        public override void OnDeath()
        {
            base.OnDeath();

            HexRunning = false;
        }
    }

}
