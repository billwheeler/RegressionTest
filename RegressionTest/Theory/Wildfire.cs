using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class WildfireDruid : BaseCharacter
    {
        public bool ConjureRunning { get; set; } = false;
        public bool ConjureUsed { get; set; } = false;


        public class ConjureWoodlandBeingsActivate : BaseAction
        {
            public ConjureWoodlandBeingsActivate()
            {
                Desc = "Conjure Woodland Beings";
                Type = ActionType.Activate;
                Time = ActionTime.Action;
            }

            public override int Amount()
            {
                return 0;
            }
        }

        public WildfireDruid()
        {
            Name = "Wildfire";
            AC = 17;
            Health = 75;
            MaxHealth = 75;
            HealingThreshold = 18;
            Group = Team.TeamOne;
            Healer = true;
            Priority = HealPriority.High;
            InitMod = 2;
            WarCaster = true;
            MyType = CreatureType.PC;

            Abilities.Add(AbilityScore.Strength, new Stat { Score = 10, Mod = 0, Save = 0 });
            Abilities.Add(AbilityScore.Dexterity, new Stat { Score = 14, Mod = 2, Save = 2 });
            Abilities.Add(AbilityScore.Constitution, new Stat { Score = 16, Mod = 3, Save = 7 });
            Abilities.Add(AbilityScore.Intelligence, new Stat { Score = 12, Mod = 1, Save = 5 });
            Abilities.Add(AbilityScore.Wisdom, new Stat { Score = 20, Mod = 5, Save = 9 });
            Abilities.Add(AbilityScore.Charisma, new Stat { Score = 9, Mod = -1, Save = -1 });
        }

        public override void Init()
        {
            base.Init();
            ConjureRunning = false;
            ConjureUsed = false;
        }

        public override void OnFailConcentration()
        {
            base.OnFailConcentration();

            if (ConjureRunning)
            {
                ConjureRunning = false;
                Context.DeactivateSummons(Group);
            }
        }

        public override void OnDeath()
        {
            base.OnDeath();

            if (ConjureRunning)
            {
                ConjureRunning = false;
                Context.DeactivateSummons(Group);
            }
        }
    }
}
