using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public enum Team
    {
        TeamOne = 1,
        TeamTwo = 2,
        TeamThree = 3,
        TeamFour = 4
    }

    public enum HealPriority
    {
        Dont = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

    class Program
    {
        public static Encounter shits()
        {
            bool nightwalker = true;
            bool useDruid = false;
            bool useBard = true;

            Encounter enc = new Encounter { OutputAttacks = false, AllowHealing = true };

            enc.Add(new Amxikas());

            if (useBard)
            {
                enc.Add(new Bard());
            }
            else
            {
                enc.Add(new Fighter());
            }

            enc.Add(new Marinyth { ShouldHuntersMark = true });
            enc.Add(new GenieWarlock());

            if (useDruid)
            {
                enc.Add(new Druid());
                for (int i = 1; i <= 4; i++)
                {
                    enc.Add(new Satyr { Name = $"Satyr #{i}", ShepherdSummons = true });
                }
            }
            else
            {
                enc.Add(new Murie());
            }

            if (nightwalker)
            {
                enc.Add(new Nightwalker());
                for (int i = 1; i <= 4; i++)
                {
                    enc.Add(new Wight { Name = $"Wight #{i}" });
                }
            }
            else
            {
                for (int i = 1; i <= 12; i++)
                {
                    enc.Add(new Hellwasp { Name = $"Hellwasp #{i}" });
                }
            }

            return enc;
        }

        static void Main(string[] args)
        {
            Encounter enc = shits();
            for (int i = 0; i < 60000; i++)
            {
                enc.RollInitiative();
                while (enc.ProcessRound()) { }
                enc.PostEncounter();
            }

            Console.WriteLine(enc.Output());

            Console.ReadLine();
        }
    }
}
