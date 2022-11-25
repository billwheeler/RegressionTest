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

    public enum TargetPriority
    {
        Lowest = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Highest = 4
    }

    class Program
    {
        public static bool output = false;
        public static int passes = 30000;

        public static Encounter shits()
        {
            bool nightwalker = true;
            bool useDruid = true;
            bool shepherd = false;

            Encounter enc = new Encounter
            {
                InBrightLight = !nightwalker,
                HasWatchers = false,
                OutputAttacks = output,
                AllowHealing = true
            };

            //enc.Add(new Tenraja());

            //enc.Add(new Amxikas { CanSpiritShroud = !nightwalker });
            //enc.Add(new AmxikasRi());
            enc.Add(new Witchblade());
            //enc.Add(new Conquest { CanSpiritShroud = !nightwalker, CanConqueringPresense = true });
            //enc.Add(new Rogue());
            //enc.Add(new EldritchKnight());
            //enc.Add(new HexBard { ShouldPhantasmalKiller = nightwalker, ShouldHex = !nightwalker });
            //enc.Add(new Bard { ShouldPhantasmalKiller = nightwalker });

            //enc.Add(new Fighter());
            enc.Add(new Paladin
            {
                IsWatchers = enc.HasWatchers,
                CanTurn = enc.HasWatchers && nightwalker ? false : true,
                CanSpiritShroud = !nightwalker
            });

            enc.Add(new Marinyth { ShouldHuntersMark = true });
            //enc.Add(new Sorcerer());
            //enc.Add(new GenieWarlock());
            enc.Add(new Bladesinger { ShouldShadowBlade = !enc.InBrightLight });

            if (useDruid)
            {
                bool woodland = false;
                BaseCharacter druid;

                if (shepherd)
                {
                    druid = new Druid();
                }
                else
                {
                    druid = new WildfireDruid();
                }

                enc.Add(druid);
                if (woodland)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        enc.Add(new Satyr { Name = $"Satyr #{i}", ShepherdSummons = shepherd });
                    }
                }
                else
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        enc.Add(new SteamMephit { Name = $"Steam Mephit #{i}", MySummoner = druid, ShepherdSummons = shepherd });
                    }
                }
            }
            else
            {
                //enc.Add(new NerfedTwilight { CanTurn = nightwalker });
                enc.Add(new Murie { CanTurn = nightwalker, ShouldBoomBoom = false });
                //enc.Add(new Paladin());
                //enc.Add(new Fighter());
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
                for (int i = 1; i <= 10; i++)
                {
                    enc.Add(new Hellwasp { Name = $"Hellwasp #{i}" });
                }
            }

            return enc;
        }

        static void Main(string[] args)
        {
            if (!output) WriteProgress("0%", 1);

            int x = 0;
            int y = 0;
            int z;

            int increment = (int)Math.Ceiling((double)passes / 100.0f);

            Encounter enc = shits();
            for (int i = 0; i < passes; i++)
            {
                z = 0;

                enc.RollInitiative();
                while (enc.ProcessRound())
                {
                    z++;
                    if (z > 30)
                        break;
                }
                enc.PostEncounter();

                if (x == increment)
                {
                    y++;
                    x = 0;
                    if (!output) WriteProgress($"{y}%", 1);
                }

                x++;
            }

            Console.WriteLine(enc.Output());

            Console.ReadLine();
        }

        /// <summary>
        /// Writes a string at the x position, y position = 1;
        /// Tries to catch all exceptions, will not throw any exceptions.  
        /// </summary>
        /// <param name="s">String to print usually "*" or "@"</param>
        /// <param name="x">The x postion,  This is modulo divided by the window.width, 
        /// which allows large numbers, ie feel free to call with large loop counters</param>
        protected static void WriteProgress(string s, int x)
        {
            int origRow = Console.CursorTop;
            int origCol = Console.CursorLeft;
            int width = Console.WindowWidth;
            x %= width;
            try
            {
                Console.SetCursorPosition(x, 1);
                Console.Write(s);
            }
            catch (ArgumentOutOfRangeException e)
            {

            }
            finally
            {
                try
                {
                    Console.SetCursorPosition(origCol, origRow);
                }
                catch (ArgumentOutOfRangeException e)
                {
                }
            }
        }
    }
}
