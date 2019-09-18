﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Rosara : BaseCharacter
    {
        public class HandCrossbow : BaseAttack
        {
            public bool HadSneakAttack { get; set; } = false;

            public HandCrossbow()
            {
                Desc = "Hand Crossbow";
                Modifier = 4;
            }

            public override int Damage()
            {
                int dmg = Dice.D6() + 2;

                if (!HadSneakAttack)
                {
                    if (Dice.D10() < 9)
                        dmg += (Dice.D6() + Dice.D6());
                    HadSneakAttack = true;
                }

                return dmg;
            }
        }

        public Rosara()
        {
            Name = "Rosara";
            AC = 13;
            InitMod = 2;
            Health = 26;
            MaxHealth = 26;
            Group = Team.TeamTwo;
        }

        public override BaseAttack PickAttack()
        {
            return new HandCrossbow();
        }
    }
}