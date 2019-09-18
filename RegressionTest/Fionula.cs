﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Fionula : BaseCharacter
    {
        public class Witchbolt : BaseAttack
        {
            public Witchbolt()
            {
                Desc = "Witchbolt";
                Modifier = 7;
            }

            public override int Damage()
            {
                return Dice.D12() + Dice.D12() + Dice.D12();
            }
        }

        public class EldritchBlast : BaseAttack
        {
            public EldritchBlast()
            {
                Desc = "Eldritch Blast";
                Number = 2;
                Modifier = 7;
            }

            public override int Damage()
            {
                return Dice.D10() + 4;
            }
        }

        public Fionula()
        {
            Name = "Fionula";
            AC = 15;
            InitMod = 3;
            Health = 31;
            MaxHealth = 31;
            HealingThreshold = 11;
            Group = Team.TeamOne;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();
            if (rando == 10)
            {
                return new Witchbolt();
            }
            else
            {
                return new EldritchBlast();
            }
        }
    }
}