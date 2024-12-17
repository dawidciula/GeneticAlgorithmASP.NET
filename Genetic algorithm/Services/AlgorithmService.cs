using AG.Models;
using Genetic_algorithm.Models;

namespace AG.Services
{
    public class AlgorithmService
    {
        private readonly FitnessService _fitnessService;
        private readonly FlexibleWorkTimePopulation _flexibleWorkTimePopulation;
        private readonly FourbrigadePopulation _fourBrigadePopulation;
        private readonly CrossoverService _crossoverService;
        private readonly MutationService _mutationService;

        private int[,] _bestSchedule;
        private double _bestFitness;

        public AlgorithmService(
            FitnessService fitnessService,
            FlexibleWorkTimePopulation flexibleWorkTimePopulation,
            FourbrigadePopulation fourBrigadePopulation,
            CrossoverService crossoverService,
            MutationService mutationService)
        {
            _fitnessService = fitnessService;
            _flexibleWorkTimePopulation = flexibleWorkTimePopulation;
            _fourBrigadePopulation = fourBrigadePopulation;
            _crossoverService = crossoverService;
            _mutationService = mutationService;
            _bestFitness = 0.0;
        }

        public ScheduleResult RunAlgorithm(OptimizationParameters optimizationParameters, ScheduleParameters scheduleParameters, WorkRegime workRegime, EmployeePreference employeePreference)
        {
            // Parametry ustawione na sztywno
            int populationSize = 80; // Rozmiar populacji
            int numberOfWorkers = 12;  // Liczba pracowników
            int daysInWeek = 7; // Liczba dni w tygodniu
            double preferenceWeight = 0.7; // Waga preferencji
            var optimizationType = OptimizationType.RouletteSelection; // Typ optymalizacji
            double mutationFrequency = 0.3; // Częstotliwość mutacji
            int numberOfParents = 4; // Liczba rodziców
            int eliteCount = (int)(populationSize * 0.2); // Procent elitarnych osobników

            Console.WriteLine("Uruchomiono algorytm z następującymi parametrami:");
            Console.WriteLine($"OptimizationType: {optimizationType}");
            Console.WriteLine($"PopulationSize: {populationSize}");
            Console.WriteLine($"PreferenceWeight: {preferenceWeight}");
            Console.WriteLine($"MutationFrequency: {mutationFrequency}");
            Console.WriteLine($"NumberOfParents: {numberOfParents}");
            Console.WriteLine($"ElitePercentage: 0.2");
            Console.WriteLine($"NumberOfWorkers: {numberOfWorkers}");
            Console.WriteLine($"DaysInWeek: {daysInWeek}");
            Console.WriteLine($"WorkRegime: {workRegime}");

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
            // Wybór populacji na podstawie WorkRegime
            List<int[,]> population;
            if (workRegime == WorkRegime.FlexibleWorkTime)
            {
                population = _flexibleWorkTimePopulation.GenerateInitialPopulation(populationSize, numberOfWorkers, daysInWeek);
            }
            else if (workRegime == WorkRegime.Fourbrigade)
            {
                population = _fourBrigadePopulation.GenerateInitialPopulation(populationSize, numberOfWorkers, daysInWeek);
            }
            else
            {
                throw new ArgumentException("Nieznany tryb pracy populacji.");
            }

            int generationsWithoutImprovement = 0;
            const int maxStagnation = 500;

            for (int generation = 0; generation < 100000; generation++)
            {
                // Obliczanie fitness
                var fitness = population.Select(schedule =>
                    _fitnessService.CalculateFitness(schedule)).ToArray();
                
                // Validate fitness array before using it
                if (fitness == null || fitness.Length == 0)
                {
                    throw new InvalidOperationException("Fitness array is empty or null.");
                }

                // Znajdź maksymalny fitness w obecnej generacji
                double currentBestFitness = fitness.Max();
                
                // If new max fitness is better than the previous one
                if (currentBestFitness > _bestFitness)
                {
                    _bestFitness = currentBestFitness;
                    _bestSchedule = population[Array.IndexOf(fitness, currentBestFitness)];
                    generationsWithoutImprovement = 0; // Reset stagnation counter
                }
                else
                {
                    generationsWithoutImprovement++; // Increase stagnation counter
                }
                
                // Check stagnation condition
                if (generationsWithoutImprovement >= maxStagnation)
                {
                    Console.WriteLine($"Algorytm zatrzymany z powodu stagnacji po {generation} generacjach.");
                    break;
                }

                // Aktualizacja najlepszego wyniku
                for (int i = 0; i < fitness.Length; i++)
                {
                    if (fitness[i] > _bestFitness)
                    {
                        _bestFitness = fitness[i];
                        _bestSchedule = population[i];
                    }
                }

                // If maximum fitness is found, break the loop
                if (_bestFitness >= 1000.0)
                {
                    break;
                }
                
                // Sort population by fitness (highest to lowest)
                var sortedPopulation = population.Zip(fitness, (schedule, fit) => new { Schedule = schedule, Fitness = fit })
                    .OrderByDescending(x => x.Fitness)
                    .ToList();
                
                // Get elite individuals
                double fitnessThreshold = 0.8 * _bestFitness; // Fitness threshold as 80% of best
                var eliteIndividuals = sortedPopulation
                    .Where(x => x.Fitness >= fitnessThreshold)
                    .Select(x => x.Schedule)
                    .ToList();

                // Select parents from the rest of the population
                var parents = SelectParents(
                    sortedPopulation.Skip(eliteCount).Select(x => x.Schedule).ToList(),
                    sortedPopulation.Skip(eliteCount).Select(x => x.Fitness).ToArray(),
                    optimizationType,
                    random,
                    numberOfParents,
                    mutationFrequency);

                // Perform crossover
                var offspring = _crossoverService.PerformCrossover(parents, random, numberOfWorkers, daysInWeek);

                // Perform mutation
                _mutationService.PerformMutation(offspring, random, numberOfWorkers, daysInWeek, mutationFrequency);
                
                // Add elite individuals to offspring
                offspring.AddRange(eliteIndividuals);

                // Update population
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

            if (optimizationType == OptimizationType.RouletteSelection)
            {
                
                double maxFitness = fitness.Max();
                double minFitness = fitness.Min();
                double threshold = maxFitness * 0.1; // Próg skalowania

                // Normalize fitness values and ensure no negative or invalid values
                for (int i = 0; i < fitness.Length; i++)
                {
                    if (fitness[i] > threshold)
                        fitness[i] = (fitness[i] - threshold) / (maxFitness - threshold);
                    else
                        fitness[i] = 0; // Truncate low fitness values
                }

                var slots = FillSlots(fitness);
                for (int i = 0; i < numberOfParents; i++)
                {
                    double point = random.NextDouble() * fitness.Sum();
                    parents.Add(population[SearchSlot(slots, point)]);
                }
            }
            else if (optimizationType == OptimizationType.TournamentSelection)
            {
                int tournamentSize = Math.Max(2, numberOfParents); // Default tournament size

                for (int i = 0; i < numberOfParents; i++)
                {
                    int bestIndex = -1;
                    double bestFitness = double.MinValue;

                    for (int j = 0; j < tournamentSize; j++)
                    {
                        int candidate = random.Next(population.Count);
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
