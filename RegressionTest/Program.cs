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
            //enc.Add(new MawDemon());

            return enc;
        }

        public static Encounter shits()
        {
            Encounter enc = new Encounter { OutputAttacks = false, AllowHealing = true };

            enc.Add(new Tenraja());
            enc.Add(new Liriam());
            enc.Add(new Raelzegg());
            enc.Add(new Fionula());
            enc.Add(new Malbraxys());
            enc.Add(new Sulyman());
            enc.Add(new Elocin());

            enc.Add(new Gnoll { Name = "Gnoll #1" });
            enc.Add(new Gnoll { Name = "Gnoll #2" });
            //enc.Add(new Gnoll { Name = "Gnoll #3" });
            enc.Add(new SeaPrinceSkirmisher { Name = "Skirmisher #1", HasDistortion = true });
            enc.Add(new SeaPrinceSkirmisher { Name = "Skirmisher #2" });
            enc.Add(new SeaPrinceRanger { Name = "Ranger #1" });
            enc.Add(new SeaPrinceRanger { Name = "Ranger #2" });
            enc.Add(new SeaPrinceRanger { Name = "Ranger #3" });
            enc.Add(new SeaPrinceCleric { Name = "Cleric" });

            return enc;
        }

        public static Encounter gnolls()
        {
            Encounter enc = new Encounter { OutputAttacks = false, AllowHealing = true };

            enc.Add(new Tenraja());
            enc.Add(new Liriam());
            enc.Add(new Raelzegg());
            enc.Add(new Fionula());
            enc.Add(new Malbraxys());
            enc.Add(new Sulyman());

            enc.Add(new Gnoll { Name = "Gnoll #1" });
            enc.Add(new Gnoll { Name = "Gnoll #2" });
            enc.Add(new Gnoll { Name = "Gnoll #5" });
            enc.Add(new Gnoll { Name = "Gnoll #6" });
            enc.Add(new GnollPackLord { Name = "Gnoll Pack Lord" });

            return enc;
        }

        public static Encounter ghasts()
        {
            Encounter enc = new Encounter { OutputAttacks = false, AllowHealing = true };

            enc.Add(new Tenraja());
            enc.Add(new Raelzegg());
            enc.Add(new Fionula());
            enc.Add(new Malbraxys());
            enc.Add(new Sulyman());

            enc.Add(new Ghast { Name = "Ghast #1" });
            enc.Add(new Ghast { Name = "Ghast #2" });
            enc.Add(new Ghast { Name = "Ghast #3" });
            enc.Add(new Ghast { Name = "Ghast #4" });
            enc.Add(new Ghast { Name = "Ghast #5" });
            enc.Add(new Ghast { Name = "Ghast #6" });

            return enc;
        }

        static void Main(string[] args)
        {
            /*
            Encounter enc = pirates();
            for (int i = 0; i < 60000; i++)
            {
                enc.RollInitiative();
                while (enc.ProcessRound()) { }
                enc.PostEncounter();
            }
        
            Console.WriteLine(enc.Output());
            */

            Encounter enc = shits();
            for (int i = 0; i < 60000; i++)
            {
                enc.RollInitiative();
                while (enc.ProcessRound()) { }
                enc.PostEncounter();
            }

            Console.WriteLine(enc.Output());

            /*
            enc = ghasts();
            for (int i = 0; i < 60000; i++)
            {
                enc.RollInitiative();
                while (enc.ProcessRound()) { }
                enc.PostEncounter();
            }

            Console.WriteLine(enc.Output());
            */

            Console.ReadLine();
        }
    }
}
