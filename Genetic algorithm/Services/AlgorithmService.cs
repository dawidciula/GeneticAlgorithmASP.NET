using System;
using System.Collections.Generic;
using System.Linq;
using AG.Models;

namespace AG.Services
{
    public class AlgorithmService
    {
        private double[] _slots;
        private int[,] _bestSchedule;
        private double _bestFitness = 0.0;

        public ScheduleResult RunAlgorithm(OptimizationParameters parameters)
        {
            // Parametry algorytmu
            int populationSize = parameters.PopulationSize;
            int numberOfWorkers = parameters.NumberOfWorkers;
            int daysInWeek = parameters.DaysInWeek;
            double preferenceWeight = parameters.PreferenceWeight;
            var optimizationType = (OptimizationType)parameters.OptimizationType;

            // Preferencje pracowników
            var employeePreferences = new List<int[]>
            {
                new int[] {1, 1, 1, 1, 1, 0, 0}, // Pracownik 1
                new int[] {0, 1, 1, 1, 0, 2, 0}, // Pracownik 2
                null, // Brak preferencji
                null,  // Brak preferencji
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            };

            var random = new Random();
            var population = GenerateInitialPopulation(populationSize, numberOfWorkers, daysInWeek);

            // Pętla pokoleniowa
            for (int generation = 0; generation < 1000; generation++)
            {
                // Obliczanie fitness
                var fitness = population.Select(schedule => GetFitness(schedule, employeePreferences, preferenceWeight)).ToArray();

                // Aktualizacja najlepszego wyniku
                for (int i = 0; i < fitness.Length; i++)
                {
                    if (fitness[i] > _bestFitness)
                    {
                        _bestFitness = fitness[i];
                        _bestSchedule = population[i];
                    }
                }

                // Sprawdzenie zakończenia, jeśli znaleziono maksymalny fitness
                if (_bestFitness >= 1.0) // Zakładamy, że 1.0 to maksymalny fitness
                {
                    break;
                }

                // Selekcja rodziców
                var parents = SelectParents(population, fitness, optimizationType, random);

                // Krzyżowanie
                var offspring = PerformCrossover(parents, random, numberOfWorkers, daysInWeek);

                // Mutacja
                PerformMutation(offspring, random);

                // Aktualizacja populacji
                population = offspring;
            }

            // Zwracanie wyniku
            return new ScheduleResult
            {
                BestSchedule = _bestSchedule,
                BestFitness = _bestFitness
            };
        }

        private List<int[,]> GenerateInitialPopulation(int populationSize, int workers, int days)
        {
            var random = new Random();
            var population = new List<int[,]>();

            for (int i = 0; i < populationSize; i++)
            {
                var schedule = new int[workers, days];
                for (int worker = 0; worker < workers; worker++)
                {
                    for (int day = 0; day < days; day++)
                    {
                        schedule[worker, day] = random.Next(0, 4); // Losowe zmiany (0-3)
                    }
                }
                population.Add(schedule);
            }

            return population;
        }

        private double GetFitness(int[,] schedule, List<int[]> employeePreferences, double preferenceWeight)
        {
            int numberOfWorkers = schedule.GetLength(0);
            int daysInWeek = schedule.GetLength(1);
            double fitness = 0.0;

            // Ocena preferencji pracowników
            for (int worker = 0; worker < numberOfWorkers; worker++)
            {
                if (employeePreferences[worker] != null)
                {
                    for (int day = 0; day < daysInWeek; day++)
                    {
                        if (schedule[worker, day] == employeePreferences[worker][day])
                        {
                            fitness += 10 * preferenceWeight;
                        }
                    }
                }
            }

            // Możesz dodać inne kryteria fitness (np. równomierne zmiany)
            return fitness;
        }

        private List<int[,]> SelectParents(List<int[,]> population, double[] fitness, OptimizationType optimizationType, Random random)
        {
            var parents = new List<int[,]>();
            var totalFitness = fitness.Sum();

            if (optimizationType == OptimizationType.RouletteSelection)
            {
                FillSlots(fitness);

                for (int i = 0; i < population.Count; i++)
                {
                    double point = totalFitness * random.NextDouble();
                    int selected = SearchSlot(point);
                    parents.Add(population[selected]);
                }
            }
            else if (optimizationType == OptimizationType.TournamentSelection)
            {
                for (int i = 0; i < population.Count; i++)
                {
                    int candidate1 = random.Next(population.Count);
                    int candidate2 = random.Next(population.Count);
                    parents.Add(fitness[candidate1] > fitness[candidate2] ? population[candidate1] : population[candidate2]);
                }
            }

            return parents;
        }

        private void FillSlots(double[] fitness)
        {
            _slots = new double[fitness.Length + 1];
            _slots[0] = fitness[0];

            for (int i = 1; i < fitness.Length; i++)
            {
                _slots[i] = _slots[i - 1] + fitness[i];
            }
        }

        private int SearchSlot(double point)
        {
            int low = 0, high = _slots.Length - 2;

            while (low < high)
            {
                int mid = (low + high) / 2;
                if (_slots[mid] <= point && _slots[mid + 1] > point)
                {
                    return mid;
                }
                else if (_slots[mid] > point)
                {
                    high = mid - 1;
                }
                else
                {
                    low = mid + 1;
                }
            }

            return low;
        }

        private List<int[,]> PerformCrossover(List<int[,]> parents, Random random, int workers, int days)
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

        private void PerformMutation(List<int[,]> population, Random random)
        {
            foreach (var schedule in population)
            {
                if (random.NextDouble() < 0.1) // 10% szansa na mutację
                {
                    int worker = random.Next(schedule.GetLength(0));
                    int day = random.Next(schedule.GetLength(1));
                    schedule[worker, day] = random.Next(0, 4); // Zmiana na losową zmianę
                }
            }
        }

        private enum OptimizationType
        {
            RouletteSelection = 1,
            TournamentSelection = 2
        }
    }
}
