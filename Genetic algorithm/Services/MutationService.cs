using System;
using System.Collections.Generic;

namespace AG.Services
{
    public class MutationService
    {
        public void PerformMutation(List<int[,]> population, Random random, int workers, int days, double mutationFrequency)
        {
            foreach (var schedule in population)
            {
                // Prawdopodobieństwo mutacji zależy od mutationFrequency
                if (random.NextDouble() < mutationFrequency)
                {
                    // Liczba mutacji na harmonogram zależna od mutationFrequency
                    int numberOfMutations = (int)Math.Max(1, mutationFrequency * workers * days);

                    for (int i = 0; i < numberOfMutations; i++)
                    {
                        int worker = random.Next(workers);
                        int day = random.Next(days);

                        // Przypisanie losowej zmiany (0-3)
                        schedule[worker, day] = random.Next(0, 4);
                    }
                }
            }
        }
    }
}