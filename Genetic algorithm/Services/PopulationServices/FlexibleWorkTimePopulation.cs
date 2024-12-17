using System;
using System.Collections.Generic;

public class FlexibleWorkTimePopulation
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
                    // Losowanie zmiany: 0 - dzień wolny, 1 - zmiana poranna, 2 - zmiana popołudniowa, 3 - zmiana nocna
                    schedule[worker, day] = random.Next(0, 4); // Losowanie wartości z zakresu 0 do 3
                }
            }

            population.Add(schedule);
        }

        return population;
    }

    
}