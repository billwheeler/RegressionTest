using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class DiceRoller
    {
        protected Random Rnd { get; set; }

        public DiceRoller()
        {
            Rnd = new Random();
        }

        public int D4() { return Rnd.Next(1, 4); }
        public int D6() { return Rnd.Next(1, 6); }
        public int D8() { return Rnd.Next(1, 8); }
        public int D10() { return Rnd.Next(1, 10); }
        public int D12() { return Rnd.Next(1, 12); }
        public int D20() { return Rnd.Next(1, 20); }
    }
}
