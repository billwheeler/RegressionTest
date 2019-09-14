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
        }

        public void Add(BaseCharacter character)
        {
            currentId++;
            character.ID = currentId;
            Characters.Add(character);
        }

        public void RollInitiative()
        {
            Characters = Characters.Select(c => { c.Initiative = Dice.D20() + c.InitMod; return c; }).ToList();
            Characters.Sort(new InitSort());
        }

        public List<BaseCharacter> CurrentEnemies(Team group)
        {
            return Characters.Where(c => c.Group != group && c.Alive).ToList();
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

        public bool RunTurn()
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

                BaseAttack attack = Characters[me].PickAttack();

                for (int i = 0; i < attack.Number; i++)
                {
                    bool hits = attack.Hits(Characters[enemy]);
                    Characters[me].Attacks++;
                    int damage = 0;
                    bool survives = true;
                    string description = "no damage.";

                    if (hits)
                    {
                        Characters[me].Hits++;
                        damage = attack.Damage();
                        Characters[me].DamageGiven += damage;
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

            Round++;
            return result;
        }
    }
}
