using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Nightwalker : BaseCharacter
    {
        public class EnervatingFocus : BaseAction
        {
            public EnervatingFocus()
            {
                Desc = "Enervating Focus";
                Type = ActionType.MeleeAttack;
                AttackModifier = 12;
                Modifier = 6;
            }

            public override int Amount()
            {
                return Dice.D8(CriticalHit ? 10 : 5) + Modifier;
            }
        }

        public class FingerOfDoom : BaseAction
        {
            public FingerOfDoom()
            {
                Desc = "Finger of Doom";
                Type = ActionType.SpellSave;
                Ability = AbilityScore.Wisdom;
                DC = 21;
            }

            public override int Amount()
            {
                return Dice.D12(6);
            }
        }

        public class AnnihilatingAura : BaseAction
        {
            public AnnihilatingAura()
            {
                Desc = "Annihilating Aura";
                Type = ActionType.SpellSave;
                Ability = AbilityScore.Constitution;
                DC = 21;
            }

            public override int Amount()
            {
                return Dice.D6(6);
            }
        }

        public Nightwalker() : base()
        {
            Name = "Nightwalker";
            AC = 14;
            InitMod = 4;
            Health = 337;
            MaxHealth = 337;
            Group = Team.TeamTwo;
            PreTurnNotify = true;
            HighValueTarget = true;
            IsUndead = true;
            ResistsNonmagical = true;
            Value = TargetPriority.Highest;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 22, Mod = 6, Save = 6 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 19, Mod = 4, Save = 4 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 24, Mod = 7, Save = 13 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 6, Mod = -2, Save = -2 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 9, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 8, Mod = -1, Save = -1 });
        }

        public override BaseAction PickAction()
        {
            return new EnervatingFocus { TotalToRun = 1 };
        }

        public override BaseAction PickBonusAction()
        {
            if (Dice.D6() == 6)
                return new FingerOfDoom { TotalToRun = 1 };

            return new EnervatingFocus { TotalToRun = 1 };
        }

        public override BaseAction PickPreTurn(BaseCharacter target)
        {
            if (Alive)
            {
                bool doAura;
                int chance = 0;

                if (ActiveEffects[SpellEffectType.Turned].Active)
                    chance = 20;

                if (target.IsObject)
                    chance = 100;

                if (target.MyType == CreatureType.Summon)
                    chance = 98;

                if (chance == 100)
                    doAura = true;
                else if (chance == 0)
                    doAura = false;
                else
                    doAura = Dice.D100() <= GetAnnihilatingAuraChance();

                if (doAura)
                {
                    return new AnnihilatingAura();
                }
            }

            return new NoAction();
        }

        private int GetAnnihilatingAuraChance()
        {
            switch (Context.GetLivingEnemyCount(Group, false))
            {
                case 1:
                    return 70;
                case 2:
                    return 63;
                case 3:
                    return 56;
                case 4:
                    return 49;
                case 5:
                    return 42;
                case 6:
                    return 35;
                case 7:
                    return 28;
                case 8:
                    return 21;
                case 9:
                    return 14;
                case 10:
                    return 7;
                default:
                    return 3;
            }
        }
    }
}
