using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    /// <summary>Used to shuffle collections via Fisher-Yates shuffle.</summary>

    public class Shuffler
    {
        private Random random;

        public Shuffler()
        {
            random = new Random();
        }

        /// <summary>Shuffles the specified array.</summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="array">The array to shuffle.</param>

        public void Shuffle<T>(IList<T> array)
        {
            for (int n = array.Count; n > 1;)
            {
                int k = random.Next(n);
                --n;
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
