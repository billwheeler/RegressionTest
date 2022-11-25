using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class CharacterStats
    {
        public int Attacks { get; set; } = 0;
        public int Hits { get; set; } = 0;
        public int DamageGiven { get; set; } = 0;
        public int DamageTaken { get; set; } = 0;
        public int Rounds { get; set; } = 0;
        public int Encounters { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public int Healed { get; set; } = 0;
        public int ConcentrationChecksTotal { get; set; } = 0;
        public int ConcentrationChecksSuccess { get; set; } = 0;
        public int Smites { get; set; } = 0;
        public int OpportunityAttacks { get; set; } = 0;
        public int PowerAttacks { get; set; } = 0;
        public int SpellsUsed { get; set; } = 0;
        public int KiUsed { get; set; } = 0;
        public int FeatureDiceUsed { get; set; } = 0;

        public float DPR
        {
            get
            {
                return (float)DamageGiven / (float)Rounds;
            }
        }

        public float Accuracy
        {
            get
            {
                return (float)Hits / (float)Attacks * 100.0f;
            }
        }

        public float Mortality
        {
            get
            {
                return (float)Deaths / (float)Encounters * 100.0f;
            }
        }

        public float AverageRoundsActed
        {
            get
            {
                return (float)Rounds / (float)Encounters;
            }
        }

        public float Concentration
        {
            get
            {
                return (float)ConcentrationChecksSuccess / (float)ConcentrationChecksTotal * 100.0f;
            }
        }

        public float SmitesPerEncounter
        {
            get
            {
                return (float)Smites / (float)Encounters;
            }
        }

        public float OpportunityAttacksPerEncounter
        {
            get
            {
                return (float)OpportunityAttacks / (float)Encounters;
            }
        }

        public float PowerAttacksPercentage
        {
            get
            {
                return (float)PowerAttacks / (float)Attacks * 100.0f;
            }
        }

        public float SpellsPerEncounter
        {
            get
            {
                return (float)SpellsUsed / (float)Encounters;
            }
        }

        public float KiPerEncounter
        {
            get
            {
                return (float)KiUsed / (float)Encounters;
            }
        }

        public float FeatureDicePerEncounter
        {
            get
            {
                return (float)FeatureDiceUsed / (float)Encounters;
            }
        }
    }

    public class EncounterStats
    {
        public int Encounters { get; set; } = 0;
        public int Rounds { get; set; } = 0;

        public float AverageRounds
        {
            get
            {
                return (float)Rounds / (float)Encounters;
            }
        }
    }

    public class TeamStats
    {
        public Team Group { get; set; }
        public string Name { get; set; }
        public int Encounters { get; set; } = 0;
        public int Rounds { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public float TotalDPR { get; set; }
        public int TotalHealing { get; set; }
        public int TotalTempHP { get; set; }

        public float AverageDPR
        {
            get
            {
                return (float)TotalDPR / (float)Encounters;
            }
        }

        public float AverageHealing
        {
            get
            {
                return (float)TotalHealing / (float)Encounters;
            }
        }

        public float AverageTempHP
        {
            get
            {
                return (float)TotalTempHP / (float)Encounters;
            }
        }


        public float AverageRounds
        {
            get
            {
                return (float)Rounds / (float)Encounters;
            }
        }

        public float Success
        {
            get
            {
                return (float)Wins / (float)Encounters * 100.0f;
            }
        }

        public string Output(bool showHealing = true)
        {
            string output = string.Empty;

            if (showHealing)
            {
                output = string.Format("{0} - DPR: {1:0.00}hp, HPE: {2:0.00}hp, THPE: {3:0.00}hp, Wins: {4:0.00}%, Rounds {5:0.00} \n",
                    Name, AverageDPR, AverageHealing, AverageTempHP, Success, AverageRounds
                );
            }
            else
            {
                output = string.Format("{0} - DPR: {1:0.00}hp, Wins: {2:0.00}%, Rounds {3:0.00} \n",
                    Name, AverageDPR, Success, AverageRounds
                );
            }

            return output;
        }
    }
}
