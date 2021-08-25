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
            //enc.Add(new EldritchKnight());
            //enc.Add(new Soulknife());
            //enc.Add(new Cleric());
            enc.Add(new NerfedTwilight());
            //enc.Add(new DivineSorlock());
            //enc.Add(new Paladin());
            enc.Add(new Fighter());
            enc.Add(new Ranger());
            enc.Add(new FiendWarlock());
            
            /*enc.Add(new Druid());
            for (int i = 1; i <= 4; i++)
            {
                enc.Add(new Satyr { Name = $"Satyr #{i}" });
            }*/

            enc.Add(new Nightwalker());
            for (int i = 1; i <= 3; i++)
            {
                enc.Add(new Wight { Name = $"Wight #{i}" });
            }

            /*for (int i = 1; i <= 10; i++)
            {
                enc.Add(new Hellwasp { Name = $"Hellwasp #{i}" });
            }*/

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
