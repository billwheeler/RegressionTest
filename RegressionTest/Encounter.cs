using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTest
{
    public class Encounter
    {
        public DiceRoller Dice { get; set; }
        public List<BaseCharacter> Characters { get; set; }
        public int Round { get; set; }
        public bool OutputAttacks { get; set; }
        public TeamStats Players { get; set; }
        public TeamStats Baddies { get; set; }
        public EncounterStats Stats { get; set; }
        public bool AllowHealing { get; set; }

        private int currentId = 0;

        public class InitSort : IComparer<BaseCharacter>
        {
            public int Compare(BaseCharacter a, BaseCharacter b)
            {
                if (a.Initiative < b.Initiative)
                    return 1;
                else if (a.Initiative > b.Initiative)
                    return -1;
                return 0;
            }
        }

        public Encounter()
        {
            Characters = new List<BaseCharacter>();
            Round = 1;
            OutputAttacks = true;
            AllowHealing = true;

            Players = new TeamStats
            {
                Name = "Players",
                Group = Team.TeamOne
            };

            Baddies = new TeamStats
            {
                Name = "Baddies",
                Group = Team.TeamTwo
            };

            Stats = new EncounterStats();
        }

        public void Add(BaseCharacter character)
        {
            currentId++;
            character.ID = currentId;
            Characters.Add(character);
        }

        public void RollInitiative()
        {
            Round = 1;
            Characters = Characters.Select(c => { c.RollInitiative(); return c; }).ToList();
            Characters.Sort(new InitSort());

            Players.Encounters++;
            Baddies.Encounters++;
        }

        public List<BaseCharacter> CurrentEnemies(Team group)
        {
            return Characters.Where(c => c.Group != group && c.Alive).ToList();
        }

        public List<BaseCharacter> TeamMembers(Team group)
        {
            return Characters.Where(c => c.Group == group).ToList();
        }

        public int PickEnemy(Team group)
        {
            List<BaseCharacter> enemies = CurrentEnemies(group);
            if (enemies.Count > 0)
            {
                int id = enemies[new Random().Next(enemies.Count)].ID;
                for (int i = 0; i < Characters.Count; i++)
                {
                    if (Characters[i].ID == id)
                        return i;
                }
            }

            return -1;
        }

        public int PickHealTarget(Team group)
        {
            BaseCharacter target = Characters.Where(c => c.Group == group && c.Alive && c.NeedsHealing).OrderByDescending(c => c.Priority).FirstOrDefault();
            if (target != null)
            {
                for (int i = 0; i < Characters.Count; i++)
                {
                    if (Characters[i].ID == target.ID)
                        return i;
                }
            }

            return -1;
        }

        public bool ProcessRound()
        {
            bool result = true;
            if (OutputAttacks) Console.WriteLine(string.Format("--- Encounter Round {0} --- ", Round));

            for (int me = 0; me < Characters.Count; me++)
            {
                if (!Characters[me].Alive)
                    continue;

                int enemy = PickEnemy(Characters[me].Group);
                if (enemy == -1)
                {
                    result = false;
                    break;
                }

                if (AllowHealing && Characters[me].Healer)
                {
                    int target = PickHealTarget(Characters[me].Group);
                    if (target > -1)
                    {
                        int amount = Characters[me].HealAmount(Characters[target].Priority);
                        Characters[target].Heal(amount);
                        Characters[me].Stats.Healed += amount;

                        if (Characters[me].Group == Team.TeamOne)
                            Players.TotalHealing += amount;
                        else if (Characters[me].Group == Team.TeamTwo)
                            Baddies.TotalHealing += amount;

                        if (OutputAttacks) Console.WriteLine(string.Format("{0} [{1}hp] heals {2} for {3}hp.",
                            Characters[me].Name,
                            Characters[me].Health,
                            Characters[target].Name, 
                            amount
                        ));

                        break;
                    }
                }

                Characters[me].Stats.Rounds++;

                BaseAttack attack = Characters[me].PickAttack();

                for (int i = 0; i < attack.Number; i++)
                {
                    bool hits = attack.Hits(Characters[enemy]);
                    Characters[me].Stats.Attacks++;
                    int damage = 0;
                    bool survives = true;
                    string description = "no damage.";

                    if (hits)
                    {
                        Characters[me].Stats.Hits++;
                        damage = attack.Damage();
                        Characters[me].Stats.DamageGiven += damage;
                        Characters[enemy].Stats.DamageTaken += damage;
                        survives = Characters[enemy].TakeDamage(damage);
                        description = survives ?
                            string.Format("{0}hp damage.", damage) :
                            string.Format("{0}hp damage, and dies!", damage);
                    }

                    if (OutputAttacks) Console.WriteLine(string.Format("{0} [{1}hp] - {2}, {3}. {4} takes {5}",
                         Characters[me].Name,
                         Characters[me].Health,
                         attack.Desc,
                         hits ? "hits" : "misses",
                         Characters[enemy].Name,
                         description
                    ));

                    if (!survives)
                    {
                        Characters[enemy].Stats.Deaths++;
                        enemy = PickEnemy(Characters[me].Group);
                        if (enemy == -1)
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }

            if (CurrentEnemies(Team.TeamOne).Count == 0 || CurrentEnemies(Team.TeamTwo).Count == 0)
                result = false;

            if (OutputAttacks && !result) Console.WriteLine(string.Empty);

            Round++;
            return result;
        }

        public void PostEncounter()
        {
            if (CurrentEnemies(Team.TeamOne).Count > 0)
                Baddies.Wins++;
            else
                Players.Wins++;


            var pcs = Characters.Where(c => c.Group == Players.Group).ToList();
            foreach (BaseCharacter c in pcs)
            {
                if (c.Stats.DPR > 0) Players.TotalDPR += c.Stats.DPR;
            }

            var bads = Characters.Where(c => c.Group == Baddies.Group).ToList();
            foreach (BaseCharacter c in bads)
            {
                if (c.Stats.DPR > 0) Baddies.TotalDPR += c.Stats.DPR;
            }
        }

        public string Output()
        {
            string output = string.Empty;

            foreach (BaseCharacter c in Characters)
            {
                output += string.Format("{0} - DPR: {1}hp, Accuracy: {2}%, Mortality: {3}% \n",
                    c.Name, 
                    c.Stats.DPR.ToString("0.##"),
                    c.Stats.Accuracy.ToString("0.##"),
                    c.Stats.Mortality.ToString("0.##")
                );
            }

            output += "\n";

            output += Players.Output(AllowHealing);
            output += Baddies.Output(AllowHealing);

            return output;
        }
    }
}
