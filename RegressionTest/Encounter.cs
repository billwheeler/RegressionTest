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

        public List<int> PreTurnChars = new List<int>();
        public List<int> PostTurnChars = new List<int>();

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
            if (!AllowHealing)
                character.Healer = false;

            currentId++;
            character.ID = currentId;
            Characters.Add(character);

            if (character.PreTurnNotify)
                PreTurnChars.Add(character.ID);

            if (character.PostTurnNotify)
                PostTurnChars.Add(character.ID);
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

        public int GetIndexByID(int id)
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].ID == id)
                    return i;
            }

            return -1;
        }

        public BaseCharacter GetByID(int id)
        {
            return Characters.Where(c => c.ID == id).First();
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
            BaseCharacter target = Characters.Where(c => c.Group == group && c.NeedsHealing).OrderByDescending(c => c.Priority).FirstOrDefault();
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

        public bool ProcessAction(BaseAction action, int me, int target = -1)
        {
            if (action.Type == BaseAction.ActionType.Heal)
            {
                int amount = action.Amount();
                Characters[me].HealTarget.Heal(amount);
                Characters[me].Stats.Healed += amount;

                if (Characters[me].Group == Team.TeamOne)
                    Players.TotalHealing += amount;
                else if (Characters[me].Group == Team.TeamTwo)
                    Baddies.TotalHealing += amount;

                if (OutputAttacks) Console.WriteLine(string.Format("{0} [{1}] heals {2} for {3}hp.",
                    Characters[me].Name,
                    Characters[me].GetHealthDesc(),
                    Characters[me].HealTarget.Name,
                    amount
                ));
            }
            else if (action.Type == BaseAction.ActionType.GrantTempHp)
            {
                if (target > -1)
                {
                    // don't give temp hit points to enemies
                    if (Characters[me].Group == Characters[target].Group)
                    {
                        int amount = action.Amount();

                        // only grant temp hp if it's most than we have
                        if (Characters[target].TempHitPoints < amount)
                        {
                            Characters[target].SetTempHitPoints(amount);

                            if (Characters[me].Group == Team.TeamOne)
                                Players.TotalTempHP += amount;
                            else if (Characters[me].Group == Team.TeamTwo)
                                Baddies.TotalTempHP += amount;

                            if (OutputAttacks) Console.WriteLine(string.Format("{0} [{1}] grants {2} temp hp to {3}.",
                                Characters[me].Name,
                                Characters[me].GetHealthDesc(),
                                amount,
                                Characters[target].Name
                            ));
                        }
                    }
                }
            }
            else if (action.Type == BaseAction.ActionType.Activate)
            {
                if (OutputAttacks) Console.WriteLine(string.Format("{0} [{1}] activates {2}.",
                    Characters[me].Name,
                    Characters[me].GetHealthDesc(),
                    action.Desc
                ));
            }
            else if (action.Type == BaseAction.ActionType.SpellSave)
            {
                if (target > -1)
                {
                    // don't attact teammates
                    if (Characters[me].Group == Characters[target].Group)
                        return true;
                }

                int enemy = target > -1 ? target : PickEnemy(Characters[me].Group);
                if (enemy == -1)
                {
                    return false;
                }

                bool hits = action.Hits(Characters[enemy]);
                int damage = 0;
                bool survives = true;
                string description = "no damage";
                string concentration = ".";

                if (hits)
                {
                    damage = action.Amount();
                    Characters[me].Stats.DamageGiven += damage;
                    Characters[enemy].Stats.DamageTaken += damage;
                    survives = Characters[enemy].TakeDamage(damage);

                    if (Characters[enemy].Concentrating)
                    {
                        concentration = Characters[enemy].ConcentrationCheck(damage) ?
                            ", maintains concentration." :
                            ", loses concentration.";
                    }

                    description = survives ?
                        string.Format("{0}hp damage", damage) :
                        string.Format("{0}hp damage, and dies", damage);
                }

                if (OutputAttacks) Console.WriteLine(string.Format("{0} [{1}] - {2}, {3}. {4} takes {5}{6}",
                        Characters[me].Name,
                        Characters[me].GetHealthDesc(),
                        action.Desc,
                        action.HitDesc(),
                        Characters[enemy].Name,
                        description,
                        concentration
                ));

                if (!survives)
                {
                    Characters[enemy].Stats.Deaths++;
                    enemy = PickEnemy(Characters[me].Group);
                    if (enemy == -1)
                    {
                        return false;
                    }
                }
            }
            else if (action.Type != BaseAction.ActionType.None)
            {
                if (target > -1)
                {
                    // don't attact teammates
                    if (Characters[me].Group == Characters[target].Group)
                        return true;
                }

                int enemy = target > -1 ? target : PickEnemy(Characters[me].Group);
                if (enemy == -1)
                {
                    return false;
                }

                for (int i = 0; i < action.TotalToRun; i++)
                {
                    bool hits = action.Hits(Characters[enemy]);
                    Characters[me].Stats.Attacks++;
                    int damage = 0;
                    bool survives = true;
                    string description = "no damage";
                    string concentration = ".";

                    if (hits)
                    {
                        Characters[me].Stats.Hits++;
                        damage = action.Amount();
                        Characters[me].Stats.DamageGiven += damage;
                        Characters[enemy].Stats.DamageTaken += damage;
                        survives = Characters[enemy].TakeDamage(damage);

                        if (Characters[enemy].Concentrating)
                        {
                            concentration = Characters[enemy].ConcentrationCheck(damage) ?
                                ", maintains concentration." :
                                ", loses concentration.";
                        }

                        description = survives ?
                            string.Format("{0}hp damage", damage) :
                            string.Format("{0}hp damage, and dies", damage);
                    }

                    if (OutputAttacks) Console.WriteLine(string.Format("{0} [{1}] - {2}, {3}. {4} takes {5}{6}",
                         Characters[me].Name,
                         Characters[me].GetHealthDesc(),
                         action.Desc,
                         action.HitDesc(),
                         Characters[enemy].Name,
                         description,
                         concentration
                    ));

                    if (!survives)
                    {
                        Characters[enemy].Stats.Deaths++;
                        enemy = PickEnemy(Characters[me].Group);
                        if (enemy == -1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool ProcessRound()
        {
            bool result = true;
            if (OutputAttacks) Console.WriteLine(string.Format("\n--- Encounter Round {0} --- ", Round));

            for (int me = 0; me < Characters.Count; me++)
            {
                // am dead, am big cat no more
                if (!Characters[me].Alive)
                {
                    continue;
                }

                // has a team prevailed?
                if (CurrentEnemies(Team.TeamOne).Count == 0 || CurrentEnemies(Team.TeamTwo).Count == 0)
                {
                    if (result && OutputAttacks) Console.WriteLine("\n*** Encounter ended *** \n");
                    result = false;
                    break;
                }

                Characters[me].Stats.Rounds++;
                Characters[me].OnNewRound();

                // are we a healer? does anyone need it?
                if (AllowHealing && Characters[me].Healer)
                {
                    int target = PickHealTarget(Characters[me].Group);
                    if (target > -1)
                    {
                        Characters[me].HealTarget = Characters[target];
                    }
                }

                Characters[me].OnNewTurn();

                foreach (int id in PreTurnChars)
                {
                    int idx = GetIndexByID(id);
                    BaseAction preAction = Characters[idx].PickPreTurn();
                    if (!ProcessAction(preAction, idx, me))
                    {
                        if (result && OutputAttacks) Console.WriteLine("\n*** Encounter ended *** \n");
                        result = false;
                        break;
                    }
                }

                // am dead, am big cat no more
                if (!Characters[me].Alive)
                {
                    continue;
                }

                // pick action
                BaseAction mainAction = Characters[me].PickAction();
                if (!ProcessAction(mainAction, me))
                {
                    if (result && OutputAttacks) Console.WriteLine("\n*** Encounter ended *** \n");
                    result = false;
                    break;
                }


                // pick bonus action
                BaseAction bonusAction = Characters[me].PickBonusAction();
                if (!ProcessAction(bonusAction, me))
                {
                    if (result && OutputAttacks) Console.WriteLine("\n*** Encounter ended *** \n");
                    result = false;
                    break;
                }

                Characters[me].OnEndTurn();

                // post turn reactions
                if (result)
                {
                    foreach (int id in PostTurnChars)
                    {
                        int idx = GetIndexByID(id);
                        BaseAction postAction = Characters[idx].PickPostTurn();
                        if (!ProcessAction(postAction, idx, me))
                        {
                            if (result && OutputAttacks) Console.WriteLine("\n*** Encounter ended *** \n");
                            result = false;
                            break;
                        }
                    }
                }
            }

            // has a team prevailed?
            if (CurrentEnemies(Team.TeamOne).Count == 0 || CurrentEnemies(Team.TeamTwo).Count == 0)
                result = false;

            if (OutputAttacks && !result) Console.WriteLine(string.Empty);

            Round++;
            Players.Rounds++;
            Baddies.Rounds++;

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

            foreach (BaseCharacter c in Characters.OrderBy(c => c.Name))
            {
                output += string.Format("{0} - DPR: {1}hp, Accuracy: {2}%, Mortality: {3}%, Average Rounds: {4} \n",
                    c.Name, 
                    c.Stats.DPR.ToString("0.##"),
                    c.Stats.Accuracy.ToString("0.##"),
                    c.Stats.Mortality.ToString("0.##"),
                    c.Stats.AverageRoundsActed.ToString("0.##")
                );
            }

            output += "\n";

            output += Players.Output(AllowHealing);
            output += Baddies.Output(AllowHealing);

            output += "\n";

            return output;
        }
    }
}
