using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Tenraja : BaseCharacter
    {
        public class GorningHorns : BaseAttack
        {
            public GorningHorns()
            {
                Desc = "Goring Horns";
                Modifier = 7;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D6() + Dice.D6() + 4;
                return Dice.D6() + 4;
            }
        }

        public class GlaiveAttack : BaseAttack
        {
            public bool HuntersMark { get; set; }
            public bool DivineSmite { get; set; }

            public GlaiveAttack()
            {
                Desc = "Glaive";
                Number = 2;
                Modifier = 7;
                DivineSmite = false;
            }

            public override int Damage()
            {
                int baseDamage = Dice.D10() + 4;
                if (CriticalHit) baseDamage += Dice.D10();

                if (HuntersMark)
                {
                    baseDamage += Dice.D6();
                    if (CriticalHit) baseDamage += Dice.D6();
                }

                if (!DivineSmite && Dice.D20() == 20)
                {
                    DivineSmite = true;
                    baseDamage += (Dice.D8() + Dice.D8());
                    if (CriticalHit) baseDamage += (Dice.D8() + Dice.D8());
                    if (Dice.D20() == 20)
                    {
                        baseDamage += Dice.D8();
                        if (CriticalHit) baseDamage += Dice.D8();
                    }
                }

                if (CriticalHit) baseDamage += 7;

                return baseDamage;
            }
        }

        public Tenraja()
        {
            Name = "Tenraja";
            AC = 16;
            Health = 44;
            MaxHealth = 44;
            HealingThreshold = 15;
            Group = Team.TeamOne;
            Healer = true;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();
            if (rando == 10)
            {
                return new GorningHorns() { Dice = Dice };
            }
            else if (rando > 3 && rando < 10)
            {
                return new GlaiveAttack { Dice = Dice, HuntersMark = true };
            }

            return new GlaiveAttack { Dice = Dice, HuntersMark = false };
        }

        public override int HealAmount(HealPriority priority)
        {
            Random rnd = new Random();

            switch (priority)
            {
                case HealPriority.High:
                    return rnd.Next(8, 12);
                case HealPriority.Medium:
                    return rnd.Next(5, 10);
                case HealPriority.Low:
                    return rnd.Next(2, 5);
            }

            return base.HealAmount(priority);
        }
    }
}
