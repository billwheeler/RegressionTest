using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Ranger : BaseCharacter
    {
        public bool FavoredFoeRunning { get; set; }
        public bool PlanarWarriorRunning { get; set; }

        public class Longbow : BaseAction
        {
            public Ranger parent { get; set; }

            public Longbow()
            {
                Desc = "Longbow";
                Type = ActionType.RangedAttack;
                AttackModifier = 11;
                Modifier = 5;
                TotalToRun = 2;
            }

            public override int Amount()
            {
                int damage = 0;

                damage += Dice.D8(CriticalHit ? 2 : 1);

                if (parent.PlanarWarriorRunning)
                {
                    damage += Dice.D8(CriticalHit ? 2 : 1);
                    parent.PlanarWarriorRunning = false;
                }

                if (parent.FavoredFoeRunning)
                {
                    damage += Dice.D6(CriticalHit ? 2 : 1);
                }
                else
                {
                    parent.FavoredFoeRunning = true;
                    parent.Concentrating = true;
                }

                return damage + Modifier;
            }
        }

        public class PlanarWarriorActivate : BaseAction
        {
            public PlanarWarriorActivate()
            {
                Desc = "Planar Warrior";
                Type = ActionType.Activate;
                Time = ActionTime.BonusAction;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public Ranger()
        {
            Name = "Marinyth";
            AC = 17;
            InitMod = 5;
            Health = 84;
            MaxHealth = 84;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Medium;
            InitMod = 5;
            BonusActionFirst = true;
            MyType = CreatureType.PC;

            FavoredFoeRunning = false;
            PlanarWarriorRunning = false;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 4 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 12, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 16, Mod = 3, Save = 3 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 8, Mod = -1, Save = -1 });
        }

        public override void Init()
        {
            base.Init();
            FavoredFoeRunning = false;
            PlanarWarriorRunning = false;
        }

        public override BaseAction PickAction()
        {
            if (Healer && HealTarget != null)
            {
                return new CureWounds { Modifier = 3, Level = SpellAction.SpellLevel.Two };
            }

            return new Longbow { Time = BaseAction.ActionTime.Action, parent = this };
        }

        public override BaseAction PickBonusAction()
        {
            if (!PlanarWarriorRunning)
            {
                PlanarWarriorRunning = true;
                return new PlanarWarriorActivate();
            }

            return new NoAction { Time = BaseAction.ActionTime.BonusAction };
        }

        public override void OnNewRound()
        {
            base.OnNewRound();
            PlanarWarriorRunning = false;
        }

        public override void OnNewTurn()
        {
            if (Healer && HealTarget != null)
            {
                BonusActionFirst = false;
            }
            else
            {
                BonusActionFirst = true;
            }
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            FavoredFoeRunning = false;
        }

        public override void OnDeath()
        {
            base.OnDeath();

            PlanarWarriorRunning = false;
            FavoredFoeRunning = false;
        }
    }
}
