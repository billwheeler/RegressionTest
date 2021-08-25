using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Util
    {
        public static double Remap(int compare, int xMin, int xMax, int yMin, int yMax)
        {
            if (xMin == yMin)
            {
                return compare >= xMax ? yMax : yMin;
            }

            return yMin + (yMax - yMin) * (compare - xMin) / (xMax - xMin);
        }
    }
}
