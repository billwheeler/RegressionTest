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
        BlackTentacles,
        ConqueringPresense,
        Bladesong,
        Turned,
        Stunned,
        PsychicLance,
        MindWhip,
        Confusion,
        PhantasmalKiller
    }

    public enum SpellEffectSave
    {
        Never,
        Once,
        EachRound,
        EndsAfterOneRound
    }

    public class SpellEffect
    {
        public string Name { get; set; }
        public int DC { get; set; }
        public AbilityScore Ability { get; set; }
        public SpellEffectType Type { get; set; }
        public SpellEffectSave SaveType { get; set; }
        public bool Active { get; set; } = false;
        public BaseAction NewRoundAction { get; set; } = null;
        public BaseCharacter Owner { get; set; } = null;
    }
}
