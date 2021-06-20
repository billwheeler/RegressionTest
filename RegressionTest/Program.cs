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
            Encounter enc = new Encounter { OutputAttacks = false, AllowHealing = true };

            /*
            enc.Add(new Tenraja());
            enc.Add(new Liriam());
            enc.Add(new Fionula());
            enc.Add(new Malbraxys());
            enc.Add(new Sulyman());
            enc.Add(new Lorax());
            */

            enc.Add(new Rogue());
            enc.Add(new Cleric());
            enc.Add(new Paladin());

            /*
            enc.Add(new Nightwalker());
            enc.Add(new Wight());
            enc.Add(new Wight());
            enc.Add(new Wight());
            */

            enc.Add(new Hellwasp { Name = "Hellwasp #1" });
            enc.Add(new Hellwasp { Name = "Hellwasp #2" });
            enc.Add(new Hellwasp { Name = "Hellwasp #3" });
            enc.Add(new Hellwasp { Name = "Hellwasp #4" });
            enc.Add(new Hellwasp { Name = "Hellwasp #5" });
            enc.Add(new Hellwasp { Name = "Hellwasp #6" });
            //enc.Add(new Hellwasp { Name = "Hellwasp #7" });
            //enc.Add(new Hellwasp { Name = "Hellwasp #8" });
            //enc.Add(new Hellwasp { Name = "Hellwasp #9" });
            //enc.Add(new Hellwasp { Name = "Hellwasp #10" });

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
