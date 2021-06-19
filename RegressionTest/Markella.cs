using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Markella : BaseCharacter
    {
        public class ChainedLightning : BaseAttack
        {
            public ChainedLightning()
            {
                Desc = "Chained Lightning";
                Targets = 3;
                Type = AttackType.SpellSave;
            }

            public override int Damage()
            {
                return Dice.D8(10);
            }
        }

        public Markella()
        {
            Name = "Markella";
            AC = 20;
            InitMod = 2;
            Health = 126;
            MaxHealth = 126;
            HealingThreshold = 50;
            Group = Team.TeamTwo;
            Priority = HealPriority.High;
            SaveDC = 21;
        }

        public override BaseAttack PickAttack()
        {
            return new ChainedLightning { SaveThreshold = SaveDC };
        }
    }
}
