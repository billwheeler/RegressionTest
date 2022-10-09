using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Template : BaseCharacter
    {
        public class TemplateAttack : BaseAttack
        {
            public TemplateAttack()
            {
                Desc = "Template Attack";
                Number = 2;
                Modifier = 4;
            }

            public override int Damage()
            {
                return Dice.D6() + 2;
            }
        }

        public Template() : base()
        {
            Name = "Mr Template";
            AC = 13;
            InitMod = 3;
            Health = 30;
            MaxHealth = 30;
            Group = Team.TeamTwo;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            return new TemplateAttack();
        }
    }
}
