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
        public static Encounter pirates()
        {
            Encounter enc = new Encounter { OutputAttacks = false, AllowHealing = true };

            enc.Add(new Tenraja());
            enc.Add(new Raelzegg());
            enc.Add(new Fionula());
            enc.Add(new Malbraxys());
            enc.Add(new Sulyman());

            enc.Add(new RandoPirate { Name = "Radclyf" });
            enc.Add(new RandoPirate { Name = "Telfour" });
            enc.Add(new Kaygrun());
            enc.Add(new Karrius());
            enc.Add(new Haltran());
            enc.Add(new Rosara());
            enc.Add(new MawDemon());

            return enc;
        }

        public static Encounter gnolls()
        {
            Encounter enc = new Encounter { OutputAttacks = false, AllowHealing = true };

            enc.Add(new Tenraja());
            enc.Add(new Raelzegg());
            enc.Add(new Fionula());
            enc.Add(new Malbraxys());
            enc.Add(new Sulyman());

            enc.Add(new Gnoll { Name = "Gnoll #1" });
            enc.Add(new Gnoll { Name = "Gnoll #2" });
            enc.Add(new Gnoll { Name = "Gnoll #3" });
            enc.Add(new Gnoll { Name = "Gnoll #4" });
            enc.Add(new Gnoll { Name = "Gnoll #5" });
            enc.Add(new Gnoll { Name = "Gnoll #6" });
            enc.Add(new Gnoll { Name = "Gnoll #7" });
            enc.Add(new Gnoll { Name = "Gnoll #8" });

            return enc;
        }

        static void Main(string[] args)
        {
            Encounter enc = pirates();
            for (int i = 0; i < 60000; i++)
            {
                enc.RollInitiative();
                while (enc.ProcessRound()) { }
                enc.PostEncounter();
            }

            Console.WriteLine(enc.Output());

            enc = gnolls();
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
