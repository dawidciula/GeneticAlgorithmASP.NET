using System;
using System.Collections.Generic;

namespace AG.Services
{
    public class CrossoverService
    {
        public List<int[,]> PerformCrossover(List<int[,]> parents, Random random, int workers, int days)
        {
            var offspring = new List<int[,]>();
            for (int i = 0; i < parents.Count / 2; i++)
            {
                var parent1 = parents[2 * i];
                var parent2 = parents[2 * i + 1];
                var child = new int[workers, days];

                int crossoverPoint = days / 2;
                for (int worker = 0; worker < workers; worker++)
                {
                    for (int day = 0; day < crossoverPoint; day++)
                    {
                        child[worker, day] = parent1[worker, day];
                    }
                    for (int day = crossoverPoint; day < days; day++)
                    {
                        child[worker, day] = parent2[worker, day];
                    }
                }
                offspring.Add(child);
            }
            return offspring;
        }
    }
}