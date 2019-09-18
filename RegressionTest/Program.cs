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

    class Program
    {
        static void Main(string[] args)
        {
            DiceRoller dice = new DiceRoller();
            Encounter enc = new Encounter { OutputAttacks = false };

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

            for (int i = 0; i < 80000; i++)
            {
                enc.RollInitiative();
                while (enc.ProcessRound())
                {
                }
                enc.PostEncounter();
            }

            Console.WriteLine(enc.ToString());
            Console.ReadLine();
        }
    }
}
