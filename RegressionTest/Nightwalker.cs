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
                TotalToRun = 2;
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
                return Dice.D12(4);
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
                return Dice.D6(4);
            }
        }

        public Nightwalker()
        {
            Name = "Nightwalker";
            AC = 14;
            InitMod = 4;
            Health = 297;
            MaxHealth = 297;
            Group = Team.TeamTwo;
            PreTurnNotify = true;
            HighValueTarget = true;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 22, Mod = 6, Save = 6 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 19, Mod = 4, Save = 4 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 24, Mod = 7, Save = 13 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 6, Mod = -2, Save = -2 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 9, Mod = -1, Save = -1 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 8, Mod = -1, Save = -1 });
        }

        public override BaseAction PickAction()
        {
            return new EnervatingFocus();
        }

        public override BaseAction PickBonusAction()
        {
            if (Dice.D6() == 6)
                return new FingerOfDoom();

            return new NoAction();
        }

        public override BaseAction PickPreTurn(BaseCharacter target)
        {
            // we'll say that only 67% of the time an enemy is in range
            if (Dice.D100() <= 67)
            {
                return new AnnihilatingAura();
            }

            return new NoAction();
        }
    }
}
