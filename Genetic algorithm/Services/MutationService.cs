using System;
using System.Collections.Generic;

namespace AG.Services
{
    public class MutationService
    {
        public void PerformMutation(List<int[,]> population, Random random, int workers, int days)
        {
            foreach (var schedule in population)
            {
                if (random.NextDouble() < 0.1) // 10% szansa na mutację
                {
                    int worker = random.Next(workers);
                    int day = random.Next(days);
                    schedule[worker, day] = random.Next(0, 4);
                }
            }
        }
    }
}