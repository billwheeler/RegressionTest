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
    }

    public class TeamStats
    {
        public Team Group { get; set; }
        public string Name { get; set; }
        public int Encounters { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public float TotalDPR { get; set; }

        public float AverageDPR
        {
            get
            {
                return (float)TotalDPR / (float)Encounters;
            }
        }

        public float Success
        {
            get
            {
                return (float)Wins / (float)Encounters * 100.0f;
            }
        }

        public override string ToString()
        {
            string output = string.Empty;

            output = string.Format("{0} - DPR: {1}hp, Wins: {2}% \n",
                Name, AverageDPR.ToString("#.##"), Success.ToString("#.##")
            );

            return output;
        }
    }
}
