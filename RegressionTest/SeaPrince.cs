using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class SeaPrinceSkirmisher : BaseCharacter
    {
        public class Scimitar : BaseAttack
        {
            public bool HasDistortion { get; set; } = false;

            public Scimitar(bool lightning = false)
            {
                Desc = "Scimitar";
                Number = 3;
                Modifier = 5;
                HasDistortion = lightning;
            }

            public override int Damage()
            {
                int dmg = 0;

                if (CurrentAttack < 3)
                {
                    dmg += Dice.D6();


                    if (HasDistortion) dmg += Dice.D4() + Dice.D4();

                    if (CriticalHit)
                    {
                        dmg += Dice.D6();
                        if (HasDistortion) dmg += Dice.D4() + Dice.D4();
                    }
                }
                else
                {
                    Desc = "Dagger";
                    dmg += Dice.D4();
                    if (CriticalHit) dmg += Dice.D4();
                }

                return dmg + 3;
            }
        }

        public bool HasDistortion { get; set; } = false;

        public SeaPrinceSkirmisher()
        {
            Name = "Sea Prince Skimisher";
            AC = 15;
            InitMod = 3;
            Health = 71;
            MaxHealth = 71;
            HealingThreshold = 52;
            Group = Team.TeamTwo;
            Priority = HealPriority.High;
            HasDistortion = false;
        }

        public override BaseAttack PickAttack()
        {
            return new Scimitar(HasDistortion);
        }
    }


    public class SeaPrinceRanger : BaseCharacter
    {
        public class Crossbow : BaseAttack
        {
            public bool HadSneakAttack { get; set; } = false;

            public Crossbow()
            {
                Desc = "Large Crossbow";
                Number = 2;
                Modifier = 5;
            }

            public override int Damage()
            {
                int dmg = Dice.D10();
                if (CriticalHit)
                    dmg += Dice.D10();

                if (!HadSneakAttack)
                {
                    if (Dice.D10() < 8)
                    {
                        dmg += (Dice.D6() + Dice.D6() + Dice.D6() + Dice.D6());
                        if (CriticalHit) dmg += (Dice.D6() + Dice.D6() + Dice.D6() + Dice.D6());
                        HadSneakAttack = true;
                    }
                }

                return dmg + 3;
            }
        }

        public SeaPrinceRanger()
        {
            Name = "Sea Prince Ranger";
            AC = 15;
            InitMod = 3;
            Health = 39;
            MaxHealth = 39;
            HealingThreshold = 23;
            Group = Team.TeamTwo;
            Priority = HealPriority.Medium;
        }

        public override BaseAttack PickAttack()
        {
            return new Crossbow();
        }
    }

    public class SeaPrinceCleric : BaseCharacter
    {
        public class TollOfTheDead : BaseAttack
        {
            public TollOfTheDead()
            {
                Desc = "Toll of the Dead";
                Modifier = 7;
            }

            public override int Damage()
            {
                if (CriticalHit)
                    return Dice.D12() + Dice.D12();

                return Dice.D12();
            }
        }

        public class TollOfTheDeadSpirit : BaseAttack
        {
            public TollOfTheDeadSpirit()
            {
                Desc = "Toll of the Dead";
                Number = 2;
                Modifier = 7;
            }

            public override bool Hits(BaseCharacter target)
            {
                bool hits = base.Hits(target);
                if (CurrentAttack > 1)
                    Desc = "Spirtual Weapon";

                return hits;
            }

            public override int Damage()
            {
                if (CurrentAttack > 1)
                {
                    if (CriticalHit)
                        return Dice.D12() + Dice.D12();
                    return Dice.D12();
                }
                else
                {
                    if (CriticalHit)
                        return Dice.D8() + Dice.D8() + 4;
                    return Dice.D8() + 4;
                }
            }
        }

        public SeaPrinceCleric()
        {
            Name = "Sea Prince Cleric";
            AC = 18;
            InitMod = 2;
            Health = 44;
            MaxHealth = 44;
            Group = Team.TeamTwo;
            Healer = true;
            Priority = HealPriority.High;
        }

        public override BaseAttack PickAttack()
        {
            int rando = Dice.D10();
            if (rando > 6)
                return new TollOfTheDeadSpirit();

            return new TollOfTheDead();
        }

        public override int HealAmount(HealPriority priority)
        {
            if (priority == HealPriority.High && Dice.D4() > 1)
                return Dice.D4() + Dice.D4() + 3;

            return Dice.D4() + 3;
        }
    }
}
