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

                // Wybór wielu punktów krzyżowania
                int numberOfCrossoverPoints = random.Next(2, days / 2);
                var crossoverPoints = new List<int>();
                while (crossoverPoints.Count < numberOfCrossoverPoints)
                {
                    int point = random.Next(1, days - 1);
                    if (!crossoverPoints.Contains(point))
                        crossoverPoints.Add(point);
                }
                crossoverPoints.Sort();

                // Krzyżowanie z uwzględnieniem wielopunktowego podziału
                bool takeFromParent1 = true;
                int lastCrossoverPoint = 0;

                foreach (var point in crossoverPoints)
                {
                    for (int worker = 0; worker < workers; worker++)
                    {
                        for (int day = lastCrossoverPoint; day < point; day++)
                        {
                            child[worker, day] = takeFromParent1 ? parent1[worker, day] : parent2[worker, day];
                        }
                    }
                    takeFromParent1 = !takeFromParent1;
                    lastCrossoverPoint = point;
                }

                // Dodanie pozostałych dni
                for (int worker = 0; worker < workers; worker++)
                {
                    for (int day = lastCrossoverPoint; day < days; day++)
                    {
                        child[worker, day] = takeFromParent1 ? parent1[worker, day] : parent2[worker, day];
                    }
                }

                // Walidacja i korekcja dziecka
                ValidateAndCorrectChild(child, workers, days);
                offspring.Add(child);
            }

            return offspring;
        }

        private void ValidateAndCorrectChild(int[,] child, int workers, int days)
        {
            for (int worker = 0; worker < workers; worker++)
            {
                int workDaysCount = 0;
                bool hadNightShiftBefore = false;

                for (int day = 0; day < days; day++)
                {
                    int shift = child[worker, day];

                    // Sprawdzenie zmian nocnych
                    if (shift == 3)
                    {
                        if (day > 0 && child[worker, day - 1] != 0)
                        {
                            child[worker, day] = 0;
                        }
                        hadNightShiftBefore = true;
                    }
                    else if (shift != 0)
                    {
                        // Dzienna lub popołudniowa zmiana po nocnej
                        if (day > 0 && child[worker, day - 1] == 3)
                        {
                            child[worker, day] = 0;
                        }
                        workDaysCount++;
                    }

                    // Zapewnienie minimum dni pracy i dni wolnych
                    if (workDaysCount > 5)
                    {
                        child[worker, day] = 0;
                        workDaysCount--;
                    }
                }

                // Korekcja liczby dni wolnych
                EnsureMinimumWorkDays(child, worker, days);
            }
        }

        private void EnsureMinimumWorkDays(int[,] schedule, int worker, int days)
        {
            int workDaysCount = 0;
            var freeDays = new List<int>();

            for (int day = 0; day < days; day++)
            {
                if (schedule[worker, day] == 0)
                {
                    freeDays.Add(day);
                }
                else
                {
                    workDaysCount++;
                }
            }

            var random = new Random();

            // Uzupełnienie dni pracy jeśli za mało
            while (workDaysCount < 5 && freeDays.Count > 0)
            {
                int dayToWork = freeDays[random.Next(freeDays.Count)];
                freeDays.Remove(dayToWork);
                schedule[worker, dayToWork] = random.Next(1, 4);
                workDaysCount++;
            }

            // Zapewnienie minimum dwóch dni wolnych
            while (freeDays.Count < 2)
            {
                int randomDay;
                do
                {
                    randomDay = random.Next(days);
                } while (freeDays.Contains(randomDay));

                freeDays.Add(randomDay);
                schedule[worker, randomDay] = 0;
            }
        }
    }
}
