using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public abstract class BaseAttack
    {
        public string Desc { get; set; }
        public int Number { get; set; } = 1;
        public int Modifier { get; set; } = 0;
        public DiceRoller Dice { get; set; } = new DiceRoller();
        public int CurrentAttack { get; set; } = 0;

        public virtual bool Hits(BaseCharacter target)
        {
            CurrentAttack++;
            return (Dice.D20() + Modifier) >= target.AC ? true : false;
        }

        public abstract int Damage();
    }
}
