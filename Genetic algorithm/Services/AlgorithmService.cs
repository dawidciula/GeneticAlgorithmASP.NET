using System;
using System.Collections.Generic;
using System.Linq;
using AG.Models;

namespace AG.Services
{
    public class AlgorithmService
    {
        private readonly FitnessService _fitnessService;
        private readonly PopulationService _populationService;
        private readonly CrossoverService _crossoverService;
        private readonly MutationService _mutationService;

        private int[,] _bestSchedule;
        private double _bestFitness;

        public AlgorithmService(
            FitnessService fitnessService,
            PopulationService populationService,
            CrossoverService crossoverService,
            MutationService mutationService)
        {
            _fitnessService = fitnessService;
            _populationService = populationService;
            _crossoverService = crossoverService;
            _mutationService = mutationService;
            _bestFitness = 0.0;
        }

        public ScheduleResult RunAlgorithm(OptimizationParameters parameters, EmployeePreference employeePreference)
        {
            int populationSize = parameters.PopulationSize;
            int numberOfWorkers = parameters.NumberOfWorkers;
            int daysInWeek = parameters.DaysInWeek;
            double preferenceWeight = parameters.PreferenceWeight;
            var optimizationType = (OptimizationType)parameters.OptimizationType;
            double mutationFrequency = parameters.MutationFrequency;
            int numberOfParents = parameters.NumberOfParents;
            int eliteCount = (int)(parameters.PopulationSize * parameters.ElitePercentage);

            
            var employeePreferences = new List<int[]>
            {
                new int[] {1, 1, 1, 1, 1, 0, 0}, // Pracownik 1
                new int[] {0, 1, 1, 1, 0, 2, 0}, // Pracownik 2
                null,
                null,
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
            var population = _populationService.GenerateInitialPopulation(populationSize, numberOfWorkers, daysInWeek);
            

            for (int generation = 0; generation < 1000; generation++)
            {
                // Obliczanie fitness
                var fitness = population.Select(schedule =>
                    _fitnessService.CalculateFitness(schedule, employeePreferences, preferenceWeight)).ToArray();

                // Aktualizacja najlepszego wyniku
                for (int i = 0; i < fitness.Length; i++)
                {
                    if (fitness[i] > _bestFitness)
                    {
                        _bestFitness = fitness[i];
                        _bestSchedule = population[i];
                    }
                }

                // Jeśli znaleziono maksymalny fitness, zakończ pętlę
                if (_bestFitness >= 1.0)
                {
                    break;
                }
                
                // Sortowanie populacji według fitness (od najlepszego do najgorszego)
                var sortedPopulation = population.Zip(fitness, (schedule, fit) => new { Schedule = schedule, Fitness = fit })
                    .OrderByDescending(x => x.Fitness)
                    .ToList();
                
                // Pobierz elitarne osobniki
                var eliteIndividuals = sortedPopulation.Take(eliteCount).Select(x => x.Schedule).ToList();
                
                // Selekcja rodziców z reszty populacji
                var parents = SelectParents(
                    sortedPopulation.Skip(eliteCount).Select(x => x.Schedule).ToList(),
                    sortedPopulation.Skip(eliteCount).Select(x => x.Fitness).ToArray(),
                    optimizationType,
                    random,
                    numberOfParents,
                    mutationFrequency);

                // Krzyżowanie
                var offspring = _crossoverService.PerformCrossover(parents, random, numberOfWorkers, daysInWeek);

                // Mutacja
                _mutationService.PerformMutation(offspring, random, numberOfWorkers, daysInWeek, mutationFrequency);
                
                // Dodaj elitarne osobniki do potomstwa
                offspring.AddRange(eliteIndividuals);

                // Aktualizacja populacji
                population = offspring;
            }

            return new ScheduleResult
            {
                BestSchedule = _bestSchedule,
                BestFitness = _bestFitness
            };
        }

        private List<int[,]> SelectParents(List<int[,]> population, double[] fitness, OptimizationType optimizationType, Random random, int numberOfParents, double mutationFrequency)
        {
            var parents = new List<int[,]>();
            int populationSize = population.Count;

            if (optimizationType == OptimizationType.RouletteSelection)
            {
                // Modyfikacja fitness dla ruletki
                if (mutationFrequency > 0)
                {
                    double maxFitness = fitness.Max();
                    double minFitness = fitness.Min();
                    for (int i = 0; i < fitness.Length; i++)
                    {
                        fitness[i] = (fitness[i] - minFitness) / (maxFitness - minFitness);
                    }
                }

                // Wypełnienie slotów
                var slots = FillSlots(fitness);
                for (int i = 0; i < numberOfParents; i++)
                {
                    double point = random.NextDouble() * fitness.Sum();
                    parents.Add(population[SearchSlot(slots, point)]);
                }
            }
            else if (optimizationType == OptimizationType.TournamentSelection)
            {
                // Selekcja turniejowa
                for (int i = 0; i < numberOfParents; i++)
                {
                    int bestIndex = -1;
                    double bestFitness = double.MinValue;

                    // Turniej z liczbą kandydatów = numberOfParents
                    for (int j = 0; j < numberOfParents; j++)
                    {
                        int candidate = random.Next(populationSize);
                        if (fitness[candidate] > bestFitness)
                        {
                            bestFitness = fitness[candidate];
                            bestIndex = candidate;
                        }
                    }

                    parents.Add(population[bestIndex]);
                }
            }

            return parents;
        }


        private double[] FillSlots(double[] fitness)
        {
            var slots = new double[fitness.Length + 1];
            slots[0] = fitness[0];
            for (int i = 1; i < fitness.Length; i++)
            {
                slots[i] = slots[i - 1] + fitness[i];
            }
            return slots;
        }

        private int SearchSlot(double[] slots, double point)
        {
            int low = 0, high = slots.Length - 1;
            while (low < high)
            {
                int mid = (low + high) / 2;
                if (slots[mid] < point) low = mid + 1;
                else high = mid;
            }
            return low - 1;
        }

        private enum OptimizationType
        {
            RouletteSelection,
            TournamentSelection
        }
    }
}