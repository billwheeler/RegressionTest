using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class GenieWarlock : BaseCharacter
    {
        public class EldritchBlast : BaseAction
        {
            public GenieWarlock parent { get; set; }

            public EldritchBlast()
            {
                Desc = "Eldritch Blast";
                Type = ActionType.SpellAttack;
                Time = ActionTime.Action;
                AttackModifier = 9;
                Modifier = 5;
                TotalToRun = 2;
                IsMagical = true;
            }

            public override int Amount()
            {
                int damage = Dice.D10(CriticalHit ? 2 : 1);

                if (parent.DidExtraDamage == false)
                {
                    damage += 4;
                    parent.DidExtraDamage = true;
                }
                
                return damage + Modifier;
            }
        }

        public bool DidBigSpell { get; set; } = false;
        public bool HypnoticPatternRunning { get; set; } = false;
        public bool DidExtraDamage { get; set; } = false;

        public GenieWarlock() : base()
        {
            //Name = "Ketrick";
            Name = "Lakrissa";
            //Name = "Tolson";
            AC = 18;
            InitMod = 2;
            Health = 75;
            MaxHealth = 75;
            Group = Team.TeamOne;
            HealingThreshold = 18;
            Priority = HealPriority.Medium;
            MyType = CreatureType.PC;
            WarCaster = true;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 9, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 12, Mod = 1, Save = 5 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 20, Mod = 5, Save = 9 });
        }

        public override void Init()
        {
            base.Init();

            HypnoticPatternRunning = false;
            DidExtraDamage = false;
            DidBigSpell = false;
        }

        public override bool OnNewRound()
        {
            bool result = base.OnNewRound();

            DidExtraDamage = false;

            return result;
        }

        public override BaseAction PickAction()
        {
            if (!DidBigSpell)
            {
                if (Context.AnyoneHaveEffect(Group, SpellEffectType.SynapticStatic))
                {
                    DidBigSpell = true;
                    Concentrating = true;
                    HypnoticPatternRunning = true;
                    return new HypnoticPattern(17);
                }
                else
                {
                    //DidBigSpell = true;
                    return new SynapticStatic(17);
                }
            }

            return new EldritchBlast { parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            return new NoAction();
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();

            if (!DidBigSpell)
            {
                BonusActionFirst = false;
            }
            else
            {
                BonusActionFirst = false;
            }
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            if (HypnoticPatternRunning)
            {
                HypnoticPatternRunning = false;
                Context.EndHypnoticPattern(Group);
            }
        }

        public override void OnDeath()
        {
            base.OnDeath();

            if (HypnoticPatternRunning)
            {
                HypnoticPatternRunning = false;
                Context.EndHypnoticPattern(Group);
            }
        }
    }
}
