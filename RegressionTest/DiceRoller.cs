using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class DiceRoller
    {
        protected Random Rnd { get; set; } = new Random(DateTime.Now.Millisecond);

        public int D4() { return Rnd.Next(1, 5); }
        public int D6() { return Rnd.Next(1, 7); }
        public int D8() { return Rnd.Next(1, 9); }
        public int D10() { return Rnd.Next(1, 11); }
        public int D12() { return Rnd.Next(1, 13); }
        public int D20() { return Rnd.Next(1, 21); }
    }
}
