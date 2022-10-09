using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public enum SpellEffectType
    {
        SynapticStatic,
        Bless,
        Bane,
        UnsettlingWords,
        HypnoticPattern,
        Inspired,
        BlackTentacles
    }

    public class SpellEffect
    {
        public string Name { get; set; }
        public int DC { get; set; }
        public AbilityScore Ability { get; set; }
        public SpellEffectType Type { get; set; }
        public bool Active { get; set; } = false;
    }
}
