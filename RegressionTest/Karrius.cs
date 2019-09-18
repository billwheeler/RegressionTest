using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Karrius : BaseCharacter
    {
        public class Greatsword : BaseAttack
        {
            public Greatsword()
            {
                Desc = "Greatsword";
                Number = 2;
                Modifier = 6;
            }

            public override int Damage()
            {
                return Dice.D6() + Dice.D6() + 3;
            }
        }

        public class GreatswordBoom : BaseAttack
        {
            public GreatswordBoom()
            {
                Desc = "Greatsword";
                Number = 3;
                Modifier = 6;
            }

            public override int Damage()
            {
                return CurrentAttack < 3 ?
                    (Dice.D6() + Dice.D6() + 3 + Dice.D8()) :
                    Dice.D8();
            }
        }


        public class EldritchBlast : BaseAttack
        {
            public EldritchBlast()
            {
                Desc = "Eldritch Blast";
                Number = 2;
                Modifier = 6;
            }

            public override int Damage()
            {
                return Dice.D10() + 3;
            }
        }

        public Karrius()
        {
            Name = "Karrius";
            AC = 16;
            InitMod = 2;
            Health = 48;
            MaxHealth = 48;
            Group = Team.TeamTwo;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();

            if (rando == 10)
                return new GreatswordBoom();
            else if (rando > 4)
                return new EldritchBlast();
            else
                return new Greatsword();
        }
    }
}
