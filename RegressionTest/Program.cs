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
            Encounter enc = new Encounter { Dice = dice };

            enc.Add(new Tenraja { Dice = dice });
            enc.Add(new RandoPirate { Dice = dice, Name = "Bob Dole" });
            enc.Add(new RandoPirate { Dice = dice, Name = "Jack Kemp" });

            enc.RollInitiative();

            while (enc.RunTurn())
            {
            }

            Console.ReadLine();
        }
    }
}
