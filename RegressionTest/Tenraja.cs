using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Liriam : BaseCharacter
    {
        public class Hooves : BaseAttack
        {
            public Hooves()
            {
                Desc = "Hooves";
                Number = 1;
                Modifier = 6;
            }

            public override int Damage()
            {
                int dmg = 0;
                dmg += (Dice.D8() + Dice.D8()) + 4;
                if (CriticalHit) dmg += (Dice.D8() + Dice.D8());
                return dmg;
            }
        }

        public override BaseAttack PickAttack()
        {
            return new Hooves();
        }

        public Liriam()
        {
            Name = "Liriam";
            AC = 18;
            Health = 68;
            MaxHealth = 68;
            HealingThreshold = 1;
            Group = Team.TeamOne;
            Healer = false;
            Priority = HealPriority.Dont;
        }
    }

    public class Tenraja : BaseCharacter
    {
        public class GorningHorns : BaseAttack
        {
            public GorningHorns()
            {
                Desc = "Goring Horns";
                Modifier = 9;
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
                Desc = "Halberd";
                Number = 3;
                Modifier = 10;
                DivineSmite = false;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 2)
                    Desc = "Halberd Alt";

                return hits;
            }

            public override int Damage()
            {
                int baseDamage = CurrentAttack > 2 ? Dice.D4() : Dice.D10();
                if (CriticalHit) baseDamage += CurrentAttack > 2 ? Dice.D4() : Dice.D10();

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

                return baseDamage + 4;
            }
        }

        public Tenraja()
        {
            Name = "Tenraja";
            AC = 18;
            Health = 76;
            MaxHealth = 76;
            HealingThreshold = 33;
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
