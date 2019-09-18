﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Sulyman : BaseCharacter
    {
        public class TollOfTheDead : BaseAttack
        {
            public TollOfTheDead()
            {
                Desc = "Toll of the Dead";
                Modifier = 7;
            }

            public override int Damage()
            {
                return Dice.D12() + Dice.D12();
            }
        }

        public class TollOfTheDeadSpirit : BaseAttack
        {
            public TollOfTheDeadSpirit()
            {
                Desc = "Toll of the Dead";
                Number = 2;
                Modifier = 7;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Spirtual Weapon";

                return hits;
            }

            public override int Damage()
            {
                if (CurrentAttack > 1)
                    return Dice.D12() + Dice.D12();
                else
                    return Dice.D8() + 4;
            }
        }

        public Sulyman()
        {
            Name = "Sulyman";
            AC = 15;
            InitMod = 1;
            Health = 43;
            MaxHealth = 43;
            Group = Team.TeamOne;
            Healer = true;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();
            if (rando > 6)
                return new TollOfTheDeadSpirit();

            return new TollOfTheDead();
        }
    }
}
