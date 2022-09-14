﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weighted_Randomizer;

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
        public List<int> OpportunityAttackers { get; set; } = new List<int>();

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
            Dice = new DiceRoller();

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

            //if (character.Group == Team.TeamOne)
            //    character.HasBless = true;

            currentId++;
            character.ID = currentId;
            character.Context = this;
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

            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].OnNewEncounter();
            }
        }

        public void EndHypnoticPattern(Team group)
        {
            int count = 0;
            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].Group == group)
                    continue;

                if (!Characters[i].Alive)
                    continue;

                if (Characters[i].Incapacitated)
                {
                    count++;
                    Characters[i].Incapacitated = false;
                }
            }
        }

        public int GetLivingEnemyCount(Team group, bool allowIncapacitated = true)
        {
            int count = 0;

            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].Group == group)
                    continue;

                if (!Characters[i].Alive)
                    continue;

                if (Characters[i].MyType == CreatureType.Summon && !Characters[i].BeenSummoned)
                    continue;

                if (!allowIncapacitated && Characters[i].Incapacitated)
                    continue;

                count++;
            }

            return count;
        }

        public bool AnyoneHaveEffect(Team group, SpellEffectType type)
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].Group == group)
                    continue;

                if (!Characters[i].Alive)
                    continue;

                if (Characters[i].MyType == CreatureType.Summon && !Characters[i].BeenSummoned)
                    continue;

                if (Characters[i].ActiveEffects.ContainsKey(type))
                    return true;
            }

            return false;
        }

        public List<BaseCharacter> CurrentEnemies(Team group)
        {
            List<BaseCharacter> result = new List<BaseCharacter>();

            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].Group == group)
                    continue;

                if (!Characters[i].Alive)
                    continue;

                if (Characters[i].MyType == CreatureType.Summon && !Characters[i].BeenSummoned)
                    continue;

                result.Add(Characters[i]);
            }

            return result;
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

        public void GiveTempHP(Team group, BaseCharacter giver, int amount = 0)
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].Group == group && Characters[i].Alive && Characters[i].MyType != CreatureType.Summon)
                {
                    if (OutputAttacks) Console.WriteLine(string.Format("{0} [{1}] grants {2} temp hp to {3}.",
                        giver.Name,
                        giver.GetHealthDesc(),
                        amount,
                        Characters[i].Name
                    ));

                    if (giver.Group == Team.TeamOne)
                        Players.TotalTempHP += amount;
                    else if (giver.Group == Team.TeamTwo)
                        Baddies.TotalTempHP += amount;

                    Characters[i].SetTempHitPoints(amount);
                }
            }
        }

        public void ActivateSummons(Team group)
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].Group == group && Characters[i].MyType == CreatureType.Summon)
                    Characters[i].BeenSummoned = true;
            }
        }

        public void DeactivateSummons(Team group)
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].Group == group && Characters[i].MyType == CreatureType.Summon)
                    Characters[i].BeenSummoned = false;
            }
        }

        private int ClampMax(int max)
        {
            IWeightedRandomizer<int> randomizer = new DynamicWeightedRandomizer<int>();
            for (int i = 0; i < max; i++)
            {
                randomizer.Add(i + 1, max - i);
            }

            return randomizer.NextWithRemoval();
        }

        public List<int> PickEnemies(Team group, int min = 1, int max = 1)
        {
            if (min > max) min = max;
            if (min < 1) min = 1;

            IWeightedRandomizer<int> randomizer = new DynamicWeightedRandomizer<int>();

            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].Group != group && Characters[i].Alive)
                {
                    int weight = 5000;
                    if (Characters[i].HighValueTarget)
                    {
                        if (!Characters[i].Incapacitated)
                            weight = 500000;
                        else
                            weight = 1;
                    }
                    else if (Characters[i].Incapacitated)
                    {
                        weight = 2;
                    }

                    if (Characters[i].IsHidden)
                    {
                        weight = 1;
                    }

                    randomizer.Add(i, weight);
                }
            }

            List<int> result = new List<int>();

            int total = randomizer.Count;

            if (total > 0)
            {
                if (max > total) max = total;
                //if (max > 1) ClampMax(max);
                if (max > 1) Dice.JustRandom(min, max);

                for (int j = 0; j < max; j++)
                {
                    result.Add(randomizer.NextWithRemoval());
                }
            }

            return result;
        }

        public int PickHealTarget(Team group)
        {
            IWeightedRandomizer<int> randomizer = new DynamicWeightedRandomizer<int>();

            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].Group == group && Characters[i].Alive && Characters[i].Priority != HealPriority.Dont)
                {
                    if (Characters[i].Health <= Characters[i].HealingThreshold)
                    {
                        randomizer.Add(i, (int)Characters[i].Priority);
                    }
                }
            }

            if (randomizer.Count > 0)
            {
                return randomizer.NextWithRemoval();
            }

            return -1;
        }

        public int PickHighValueTarget(Team group)
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].Group != group && Characters[i].Alive && Characters[i].HighValueTarget == true)
                {
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

                if (target > -1)
                {
                    Characters[target].Heal(amount);
                    if (OutputAttacks) Console.WriteLine(string.Format("{0} [{1}] heals {2} for {3}hp.",
                        Characters[me].GetNameDesc(),
                        Characters[me].GetHealthDesc(),
                        Characters[target].GetNameDesc(),
                        amount
                    ));
                }
                else
                {
                    if (!Characters[me].HealTarget.Alive)
                        Characters[me].HealTarget.Stats.Deaths--;

                    Characters[me].HealTarget.Heal(amount);
                    Characters[me].Stats.Healed += amount;

                    if (Characters[me].Group == Team.TeamOne)
                        Players.TotalHealing += amount;
                    else if (Characters[me].Group == Team.TeamTwo)
                        Baddies.TotalHealing += amount;

                    if (OutputAttacks) Console.WriteLine(string.Format("{0} [{1}] heals {2} for {3}hp.",
                        Characters[me].GetNameDesc(),
                        Characters[me].GetHealthDesc(),
                        Characters[me].HealTarget.GetNameDesc(),
                        amount
                    ));
                }
            }
            else if (action.Type == BaseAction.ActionType.GrantTempHp)
            {
                if (target > -1 && Characters[me].Group == Characters[target].Group)
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
                            Characters[me].GetNameDesc(),
                            Characters[me].GetHealthDesc(),
                            amount,
                            Characters[target].GetNameDesc()
                        ));
                    }
                }
            }
            else if (action.Type == BaseAction.ActionType.Activate)
            {
                if (OutputAttacks) Console.WriteLine(string.Format("{0} [{1}] activates {2}.",
                    Characters[me].GetNameDesc(),
                    Characters[me].GetHealthDesc(),
                    action.Desc
                ));
            }
            else if (action.Type == BaseAction.ActionType.SpellSave || action.Type == BaseAction.ActionType.Apply)
            {
                List<int> targets;
                if (target > -1)
                {
                    // don't attack allies!
                    if (Characters[target].Group == Characters[me].Group)
                        return true;

                    targets = new List<int>();
                    targets.Add(target);
                }
                else
                {
                    targets = PickEnemies(Characters[me].Group, action.MinTargets, action.MaxTargets);
                    if (targets.Count == 0)
                        return false;
                }

                int damage = 0;
                foreach (int enemy in targets)
                {
                    action.PreHit(Characters[me], Characters[enemy]);
                    bool hits = action.Type == BaseAction.ActionType.SpellSave ? action.Hits(Characters[me], Characters[enemy]) : true;
                    bool survives = true;

                    string damagePart = ".";

                    if (hits)
                    {
                        if (action.Type == BaseAction.ActionType.SpellSave && !action.Damageless)
                        {
                            string description = "no damage";
                            string concentration = ".";

                            damage = action.Amount();
                            damage = Characters[enemy].CalculateResistences(damage, action);
                            Characters[me].Stats.DamageGiven += damage;
                            Characters[enemy].Stats.DamageTaken += damage;
                            survives = Characters[enemy].TakeDamage(damage);

                            Characters[me].Stats.Attacks++;
                            if (action.Result == BaseAction.DamageAmount.Full)
                                Characters[me].Stats.Hits++;

                            if (Characters[enemy].Concentrating)
                            {
                                concentration = Characters[enemy].ConcentrationCheck(damage) ?
                                    ", maintains concentration." :
                                    ", loses concentration.";
                            }

                            description = survives ?
                                string.Format("{0}hp damage", damage) :
                                string.Format("{0}hp damage, and dies", damage);

                            damagePart = string.Format(" for {0}{1}", description, concentration);
                        }
                    }

                    if (OutputAttacks)
                    {
                        if (action.Type == BaseAction.ActionType.Apply)
                        {
                            Console.WriteLine(string.Format("{0} [{1}] - {2} against {3}{4}",
                                Characters[me].GetNameDesc(),
                                Characters[me].GetHealthDesc(),
                                action.Desc,
                                Characters[enemy].Name,
                                damagePart
                            ));
                        }
                        else
                        {
                            Console.WriteLine(string.Format("{0} [{1}] - {2} against {3}, {4}{5}",
                                Characters[me].GetNameDesc(),
                                Characters[me].GetHealthDesc(),
                                action.Desc,
                                Characters[enemy].Name,
                                action.HitDesc(),
                                damagePart
                            ));
                        }
                    }

                    if (!survives)
                    {
                        Characters[enemy].Stats.Deaths++;
                    }
                }
            }
            else if (action.Type != BaseAction.ActionType.None)
            {
                int enemy;
                if (target > -1)
                {
                    // don't attack allies!
                    if (Characters[target].Group == Characters[me].Group)
                        return true;

                    enemy = target;
                }
                else
                {
                    List<int> targets = PickEnemies(Characters[me].Group, action.MinTargets, action.MaxTargets);
                    if (targets.Count == 0)
                        return false;
                    enemy = targets.First();
                }

                do
                {
                    action.CurrentRunning++;
                    action.PreHit(Characters[me], Characters[enemy]);
                    bool hits = action.Hits(Characters[me], Characters[enemy]);
                    Characters[me].Stats.Attacks++;
                    int damage;
                    bool survives = true;
                    string damagePart = ".";

                    action.TotalToRun -= 1;

                    if (hits)
                    {
                        Characters[me].Stats.Hits++;
                        action.CurrentHits++;
                        damage = action.Amount();

                        string description = "no damage";
                        string concentration = ".";

                        if (damage > 0)
                        {
                            damage = Characters[enemy].CalculateResistences(damage, action);

                            bool uncannyDodge = Characters[enemy].ShouldUncannyDodge(damage, action.Type);
                            if (uncannyDodge)
                            {
                                damage = (int)Math.Floor(damage / 2.0);
                            }

                            Characters[me].Stats.DamageGiven += damage;
                            Characters[enemy].Stats.DamageTaken += damage;
                            survives = Characters[enemy].TakeDamage(damage);

                            if (Characters[enemy].Concentrating)
                            {
                                Characters[enemy].Stats.ConcentrationChecksTotal++;

                                bool continuesConcentration = Characters[enemy].ConcentrationCheck(damage);
                                if (continuesConcentration)
                                    Characters[enemy].Stats.ConcentrationChecksSuccess++;

                                concentration = continuesConcentration ?
                                    ", maintains concentration." :
                                    ", loses concentration.";
                            }

                            if (uncannyDodge)
                            {
                                description = survives ?
                                    string.Format("{0}hp damage, used Uncanny Dodge", damage) :
                                    string.Format("{0}hp damage, used Uncanny Dodge, but still dies", damage);
                            }
                            else
                            {
                                description = survives ?
                                    string.Format("{0}hp damage", damage) :
                                    string.Format("{0}hp damage, and dies", damage);
                            }

                            damagePart = string.Format(" for {0}{1}", description, concentration);
                        }
                    }

                    if (OutputAttacks)
                    {
                        Console.WriteLine(string.Format("{0} [{1}] - {2} against {3}, {4}{5}",
                            Characters[me].GetNameDesc(),
                            Characters[me].GetHealthDesc(),
                            action.Desc,
                            Characters[enemy].Name,
                            action.HitDesc(),
                            damagePart
                        ));
                    }

                    if (!survives)
                    {
                        Characters[enemy].Stats.Deaths++;
                        List<int> targets = PickEnemies(Characters[me].Group, action.MinTargets, action.MaxTargets);
                        if (targets.Count == 0)
                            return false;
                        enemy = targets.First();
                    }
                }
                while (action.TotalToRun > 0);
            }

            return true;
        }

        public bool ProcessRound()
        {
            // make sure any new summons end up in the right place
            Characters.Sort(new InitSort());

            OpportunityAttackers.Clear();

            bool result = true;
            if (OutputAttacks) Console.WriteLine(string.Format("\n--- Encounter Round {0} --- ", Round));

            for (int me = 0; me < Characters.Count; me++)
            {
                // am dead, am big cat no more
                if (!Characters[me].Alive)
                {
                    continue;
                }

                if (Characters[me].MyType == CreatureType.Summon && Characters[me].BeenSummoned == false)
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

                if (Characters[me].OpportunityAttackChance > 0)
                {
                    OpportunityAttackers.Add(me);
                }

                if (Characters[me].Incapacitated)
                {
                    continue;
                }

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
                    BaseAction preAction = Characters[idx].PickPreTurn(Characters[me]);
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

                if (Characters[me].BonusActionFirst)
                {
                    // pick bonus action
                    BaseAction bonusAction = Characters[me].PickBonusAction();
                    if (bonusAction.Type != BaseAction.ActionType.None)
                    {
                        Characters[me].UsedBonusAction = true;
                    }

                    if (!ProcessAction(bonusAction, me))
                    {
                        if (result && OutputAttacks) Console.WriteLine("\n*** Encounter ended *** \n");
                        result = false;
                        break;
                    }

                    // pick action
                    BaseAction mainAction = Characters[me].PickAction();
                    if (mainAction.Type != BaseAction.ActionType.None)
                    {
                        Characters[me].UsedAction = true;
                    }

                    if (!ProcessAction(mainAction, me))
                    {
                        if (result && OutputAttacks) Console.WriteLine("\n*** Encounter ended *** \n");
                        result = false;
                        break;
                    }
                }
                else
                {
                    // pick action
                    BaseAction mainAction = Characters[me].PickAction();
                    if (mainAction.Type != BaseAction.ActionType.None)
                    {
                        Characters[me].UsedAction = true;
                    }

                    if (!ProcessAction(mainAction, me))
                    {
                        if (result && OutputAttacks) Console.WriteLine("\n*** Encounter ended *** \n");
                        result = false;
                        break;
                    }

                    // pick bonus action
                    BaseAction bonusAction = Characters[me].PickBonusAction();
                    if (bonusAction.Type != BaseAction.ActionType.None)
                    {
                        Characters[me].UsedBonusAction = true;
                    }

                    if (!ProcessAction(bonusAction, me))
                    {
                        if (result && OutputAttacks) Console.WriteLine("\n*** Encounter ended *** \n");
                        result = false;
                        break;
                    }
                }

                string endTurn = Characters[me].OnEndTurn();
                if (OutputAttacks)
                {
                    if (!string.IsNullOrEmpty(endTurn))
                        Console.WriteLine(endTurn);
                }

                // post turn reactions
                if (result)
                {
                    foreach (int id in PostTurnChars)
                    {
                        int idx = GetIndexByID(id);
                        BaseAction postAction = Characters[idx].PickPostTurn(Characters[me]);
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

            // process opportunity attacks
            if (OpportunityAttackers.Count > 0)
            {
                for (int i = 0; i < OpportunityAttackers.Count; i++)
                {
                    int me = OpportunityAttackers[i];

                    // am dead, am big cat no more
                    if (!Characters[me].Alive)
                    {
                        continue;
                    }

                    if (Characters[me].Incapacitated)
                    {
                        continue;
                    }

                    if (Characters[me].UsedReaction)
                    {
                        continue;
                    }

                    if (Dice.D100() > Characters[me].OpportunityAttackChance)
                    {
                        continue;
                    }

                    Characters[me].OnNewTurn();

                    // pick action
                    BaseAction reAction = Characters[me].PickReaction(true);
                    if (reAction.Type != BaseAction.ActionType.None)
                    {
                        Characters[me].UsedReaction = true;
                    }

                    if (!ProcessAction(reAction, me))
                    {
                        if (result && OutputAttacks) Console.WriteLine("\n*** Encounter ended *** \n");
                        result = false;
                        break;
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
                if (c.Stats.ConcentrationChecksTotal > 0)
                {
                    output += string.Format("{0} - DPR: {1}hp, Accuracy: {2}%, Mortality: {3}%, Average Rounds: {4}, Con. Success: {5}% \n",
                        c.Name,
                        c.Stats.DPR.ToString("0.##"),
                        c.Stats.Accuracy.ToString("0.##"),
                        c.Stats.Mortality.ToString("0.##"),
                        c.Stats.AverageRoundsActed.ToString("0.##"),
                        c.Stats.Concentration.ToString("0.##")
                    );
                }
                else
                {
                    output += string.Format("{0} - DPR: {1}hp, Accuracy: {2}%, Mortality: {3}%, Average Rounds: {4} \n",
                        c.Name,
                        c.Stats.DPR.ToString("0.##"),
                        c.Stats.Accuracy.ToString("0.##"),
                        c.Stats.Mortality.ToString("0.##"),
                        c.Stats.AverageRoundsActed.ToString("0.##")
                    );
                }
            }

            output += "\n";

            output += Players.Output(AllowHealing);
            output += Baddies.Output(AllowHealing);

            output += "\n";

            return output;
        }
    }
}
