using System;
using System.Collections.Generic;

namespace AG.Services
{
    public class Population
    {
        // Metoda generująca początkową populację
        public List<int[,]> GenerateInitialPopulation(int populationSize, int workers, int days)
        {
            var random = new Random();
            var population = new List<int[,]>();

            for (int i = 0; i < populationSize; i++)
            {
                var schedule = new int[workers, days];

                // Losowanie zmian dla każdego pracownika
                for (int worker = 0; worker < workers; worker++)
                {
                    for (int day = 0; day < days; day++)
                    {
                        // Niedziela (dzień 6) - każdy pracownik musi mieć dzień wolny (0)
                        if (day == 6)
                        {
                            schedule[worker, day] = 0; // Dzień wolny w niedzielę
                        }
                        else
                        {
                            // Losowanie zmiany dla innych dni (mogą to być wszystkie zmiany: 0-3)
                            schedule[worker, day] = random.Next(0, 4); // Dopuszczamy wszystkie zmiany (0-3)
                        }
                    }
                }

                population.Add(schedule);
            }

            return population;
        }
    }
}