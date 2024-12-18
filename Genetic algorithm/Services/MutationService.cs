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

                        // Jeżeli to niedziela (dzień 6), nie zmieniamy zmiany (pozostaje 0)
                        if (day == 6)
                        {
                            continue; // Pomijamy mutację w niedzielę
                        }

                        // W pozostałych dniach, losujemy dowolną zmianę (0-3)
                        schedule[worker, day] = random.Next(0, 4); // Losowanie zmiany (0-3)
                    }
                }
            }
        }
    }
}