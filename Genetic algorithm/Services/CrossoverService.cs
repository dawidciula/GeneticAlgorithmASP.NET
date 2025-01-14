using System;
using System.Collections.Generic;

namespace AG.Services
{
    public class CrossoverService
    {
        public List<int[,]> PerformCrossover(List<int[,]> parents, Random random, int workers, int days, int numberOfCrossoverPoints)
        {
            var offspring = new List<int[,]>();

            for (int i = 0; i < parents.Count / 2; i++)
            {
                var parent1 = parents[2 * i];
                var parent2 = parents[2 * i + 1];
                var child = new int[workers, days];

                // Generowanie punktów krzyżowania
                var crossoverPoints = GenerateCrossoverPoints(random, numberOfCrossoverPoints, days);

                // Przechodzenie przez wszystkich pracowników
                for (int worker = 0; worker < workers; worker++)
                {
                    int currentCrossoverParent = 1;  // Zaczynamy od parent1
                    int currentPointIndex = 0;
                    
                    // Przechodzenie przez wszystkie dni
                    for (int day = 0; day < days; day++)
                    {
                        // Sprawdzenie, czy mamy przejście przez punkt krzyżowania
                        if (currentPointIndex < crossoverPoints.Count && day == crossoverPoints[currentPointIndex])
                        {
                            // Zmiana rodzica
                            currentCrossoverParent = (currentCrossoverParent == 1) ? 2 : 1;
                            currentPointIndex++;
                        }

                        // Przypisanie odpowiedniego dnia od rodzica
                        if (currentCrossoverParent == 1)
                        {
                            child[worker, day] = parent1[worker, day];
                        }
                        else
                        {
                            child[worker, day] = parent2[worker, day];
                        }
                    }
                }

                // Dodanie dziecka do listy potomków
                offspring.Add(child);
            }

            return offspring;
        }

        // Funkcja generująca punkty krzyżowania
        private List<int> GenerateCrossoverPoints(Random random, int numberOfPoints, int days)
        {
            var points = new List<int>();
            
            // Losowanie unikalnych punktów krzyżowania
            while (points.Count < numberOfPoints)
            {
                int point = random.Next(1, days);
                if (!points.Contains(point))
                {
                    points.Add(point);
                }
            }

            // Posortowanie punktów krzyżowania w porządku rosnącym
            points.Sort();
            return points;
        }
    }
}