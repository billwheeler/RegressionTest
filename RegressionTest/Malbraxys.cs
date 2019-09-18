using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Malbraxys : BaseCharacter
    {
        public class Quarterstaff : BaseAttack
        {
            public Quarterstaff()
            {
                Desc = "Quarterstaff";
                Modifier = 6;
            }

            public override int Damage()
            {
                return Dice.D8() + 3;
            }
        }

        public class QuarterstaffFire : BaseAttack
        {
            public QuarterstaffFire()
            {
                Desc = "Quarterstaff w/Fangs of the Fire Snake";
                Modifier = 6;
            }

            public override int Damage()
            {
                return Dice.D8() + 3 + Dice.D10();
            }
        }

        public class QuarterstaffFlury : BaseAttack
        {
            public QuarterstaffFlury()
            {
                Desc = "Quarterstaff";
                Number = 3;
                Modifier = 6;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Unarmed Strike";

                return hits;
            }

            public override int Damage()
            {
                int damage = CurrentAttack == 1 ?
                    Dice.D8() :
                    Dice.D4();

                return damage + 3;
            }
        }

        public class ThornWhip : BaseAttack
        {
            public ThornWhip()
            {
                Desc = "Thorn Whip";
                Modifier = 7;
            }

            public override int Damage()
            {
                return Dice.D6() + Dice.D6();
            }
        }

        public class BurningHands : BaseAttack
        {
            public BurningHands()
            {
                Desc = "Burning Hands";
                Modifier = 6;
            }

            public override int Damage()
            {
                return Dice.D6() + Dice.D6() + Dice.D6();
            }
        }

        public Malbraxys()
        {
            Name = "Malbraxys";
            AC = 17;
            InitMod = 3;
            Health = 35;
            MaxHealth = 35;
            Group = Team.TeamOne;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();
            if (rando == 10)
            {
                return new ThornWhip();
            }
            else if (rando == 9)
            {
                return new BurningHands();
            }
            else if (rando == 8 || rando == 7)
            {
                return new QuarterstaffFlury();
            }
            else if (rando == 6)
            {
                return new QuarterstaffFire();
            }
            else
            {
                return new Quarterstaff();
            }
        }
    }
}
