﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class MawDemon : BaseCharacter
    {
        public class Bite : BaseAttack
        {
            public Bite()
            {
                Desc = "Bite";
                Number = 1;
                Modifier = 4;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D8() + Dice.D8() + Dice.D8() + Dice.D8() + 2;

                return Dice.D8() + Dice.D8() + 2;
            }
        }

        public MawDemon() : base()
        {
            Name = "Maw Demon";
            AC = 13;
            InitMod = -1;
            Health = 33;
            MaxHealth = 33;
            Group = Team.TeamTwo;
        }

        public override BaseAttack PickAttack()
        {
            return new Bite();
        }
    }
}
