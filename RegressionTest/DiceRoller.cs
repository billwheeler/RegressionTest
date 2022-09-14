using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public enum AbilityRoll
    {
        Normal,
        Advantage,
        Disadvantage,
        ElvenAccuracy
    }

    public static class EnumDesc
    {
        public static string AbilityRoll(AbilityRoll abilityRoll)
        {
            switch (abilityRoll)
            {
                case RegressionTest.AbilityRoll.Normal:
                    return "Normal";
                case RegressionTest.AbilityRoll.Advantage:
                    return "Advantage";
                case RegressionTest.AbilityRoll.Disadvantage:
                    return "Disadvantage";
                case RegressionTest.AbilityRoll.ElvenAccuracy:
                    return "Elven Accuracy";
            }

            return "Normal";
        }
    }

    public class DiceRoller
    {
        protected Random Rnd { get; set; } = new Random(DateTime.Now.Millisecond);

        public int JustRandom(int min, int max)
        {
            return Rnd.Next(min, max + 1);
        }

        public int D4(int number = 1)
        {
            int total = 0;

            while (number > 0)
            {
                total += Rnd.Next(1, 5);
                number--;
            }

            return total;
        }

        public int D6(int number = 1)
        {
            int total = 0;

            while (number > 0)
            {
                total += Rnd.Next(1, 7);
                number--;
            }

            return total;
        }

        public int D8(int number = 1)
        {
            int total = 0;

            while (number > 0)
            {
                total += Rnd.Next(1, 9);
                number--;
            }

            return total;
        }

        public int D10(int number = 1)
        {
            int total = 0;

            while (number > 0)
            {
                total += Rnd.Next(1, 11);
                number--;
            }

            return total;
        }

        public int D12(int number = 1)
        {
            int total = 0;

            while (number > 0)
            {
                total += Rnd.Next(1, 13);
                number--;
            }

            return total;
        }

        public int D20(int number = 1)
        {
            int total = 0;

            while (number > 0)
            {
                total += Rnd.Next(1, 21);
                number--;
            }

            return total;
        }

        public int ElvenAccuracy()
        {
            int first = Rnd.Next(1, 21);
            int second = Rnd.Next(1, 21);
            int third = Rnd.Next(1, 21);

            if (first > second && first > third)
                return first;
            else if (second > first && second > third)
                return second;
            else
                return third;
        }

        public int Advantage()
        {
            int first = Rnd.Next(1, 21);
            int second = Rnd.Next(1, 21);

            if (first < second)
                return second;
            else
                return first;
        }

        public int Disadvantage()
        {
            int first = Rnd.Next(1, 21);
            int second = Rnd.Next(1, 21);

            if (first > second)
                return second;
            else
                return first;
        }

        public int D100(int number = 1)
        {
            int total = 0;

            while (number > 0)
            {
                total += Rnd.Next(1, 101);
                number--;
            }

            return total;
        }

        public int MakeAbilityRoll(AbilityRoll rollType = AbilityRoll.Normal)
        {
            switch (rollType)
            {
                case AbilityRoll.Advantage:
                    return Advantage();
                case AbilityRoll.Disadvantage:
                    return Disadvantage();
                case AbilityRoll.ElvenAccuracy:
                    return ElvenAccuracy();
                default:
                    return D20();
            }
        }
    }
}
