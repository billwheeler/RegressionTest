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
        public int Number { get; set; }
        public int Modifier { get; set; }
        public DiceRoller Dice { get; set; }
        public int CurrentAttack { get; set; }

        public virtual bool Hits(BaseCharacter target)
        {
            CurrentAttack++;
            return (Dice.D20() + Modifier) >= target.AC ? true : false;
        }

        public abstract int Damage();
    }
}
