using AG.Models;
using Genetic_algorithm.Interfaces;
using Genetic_algorithm.Models;

namespace AG.Services
{
    public class AlgorithmService
    {
        private readonly FitnessService _fitnessService;
        private readonly Population _population;
        private readonly CrossoverService _crossoverService;
        private readonly MutationService _mutationService;

        private int[,] _bestSchedule;
        private double _bestFitness;

        public AlgorithmService(
            FitnessService fitnessService,
            Population population,
            CrossoverService crossoverService,
            MutationService mutationService)
        {
            _fitnessService = fitnessService;
            _population = population;
            _crossoverService = crossoverService;
            _mutationService = mutationService;
            _bestFitness = 0.0;
        }

        public ScheduleResult RunAlgorithm(OptimizationParameters optimizationParameters, ScheduleParameters scheduleParameters, EmployeePreference employeePreference)
        {
            // Parametry ustawione na sztywno
            int populationSize = optimizationParameters.PopulationSize;
            int numberOfWorkers = scheduleParameters.NumberOfWorkers;
            int daysInWeek = 7;
            double preferenceWeight = 0;
            var optimizationType = OptimizationType.RouletteSelection;
            double mutationFrequency = optimizationParameters.MutationFrequency;
            int numberOfParents = optimizationParameters.NumberOfParents;
            int eliteCount = (int)(populationSize * optimizationParameters.ElitePercentage);
            int numberOfCrossoverPoints = optimizationParameters.CrossoverPoints;
            int maxGenerations = optimizationParameters.MaxGenerations;
            int maxStagnation = optimizationParameters.MaxStagnation;

            Console.WriteLine("Uruchomiono algorytm z następującymi parametrami:");
            Console.WriteLine($"OptimizationType: {optimizationType}");
            Console.WriteLine($"PopulationSize: {populationSize}");
            Console.WriteLine($"PreferenceWeight: {preferenceWeight}");
            Console.WriteLine($"MutationFrequency: {mutationFrequency}");
            Console.WriteLine($"NumberOfParents: {numberOfParents}");
            Console.WriteLine($"ElitePercentage: 0.2");
            Console.WriteLine($"NumberOfWorkers: {numberOfWorkers}");
            Console.WriteLine($"DaysInWeek: {daysInWeek}");

            var employeePreferences = new List<int[]>
            {
                new int[] {1, 1, 1, 1, 1, 0, 0}, // Pracownik 1
                new int[] {0, 1, 1, 1, 0, 2, 0}, // Pracownik 2
                new int[] {3, 3, 0, 2, 1, 0, 0}, // Pracownik 2
                null,
                null,
                null,
                null,
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
            population = _population.GenerateInitialPopulation(populationSize, numberOfWorkers, daysInWeek);
            
            
          
            int generationsWithoutImprovement = 0;

            for (int generation = 0; generation < maxGenerations; generation++)
            {
                // Obliczanie fitness
                var fitness = population.Select(schedule => _fitnessService.CalculateFitness(schedule, employeePreferences)).ToArray();


                // Sprawdzenie poprawności tablicy fitness przed jej użyciem
                if (fitness == null || fitness.Length == 0)
                {
                    throw new InvalidOperationException("Tablica fitness jest pusta lub null.");
                }

                // Znalezienie maksymalnego fitness w obecnej generacji
                double currentBestFitness = fitness.Max();

                // Jeśli nowy maksymalny fitness jest lepszy niż poprzedni
                if (currentBestFitness > _bestFitness)
                {
                    _bestFitness = currentBestFitness;
                    _bestSchedule = population[Array.IndexOf(fitness, currentBestFitness)];
                    generationsWithoutImprovement = 0; // Reset licznika stagnacji
                }
                else
                {
                    generationsWithoutImprovement++; // Zwiększenie licznika stagnacji
                }
                
                // Logowanie najlepszej wartości fitness oraz liczby wygenerowanych harmonogramów
                Console.WriteLine($"Generacja {generation + 1}:");
                Console.WriteLine($"  Najlepsza wartość fitness: {_bestFitness}");
                Console.WriteLine($"  Liczba wygenerowanych harmonogramów: {population.Count}");
                Console.WriteLine($"  Liczba osobników w populacji: {population.Count}");
                Console.WriteLine($"  Liczba rodziców: {numberOfParents}");

                // Sprawdzenie warunku stagnacji
                if (generationsWithoutImprovement >= maxStagnation)
                {
                    Console.WriteLine($"Algorytm zatrzymany z powodu stagnacji po {generation} generacjach.");
                    break;
                }
                

                // Jeśli maksymalny fitness został znaleziony, zakończ pętlę
                if (_bestFitness >= 1000.0)
                {
                    break;
                }

                // Logowanie liczby osobników w populacji i liczby rodziców w każdej iteracji
                Console.WriteLine($"Generacja {generation + 1}:");

                // Sortowanie populacji według fitness (od najwyższego do najniższego)
                var sortedPopulation = population.Zip(fitness, (schedule, fit) => new { Schedule = schedule, Fitness = fit })
                    .OrderByDescending(x => x.Fitness)
                    .ToList();

                // Pobranie elitarnych osobników
                double fitnessThreshold = 0.8 * _bestFitness; // Próg fitness jako 80% najlepszego
                var eliteIndividuals = sortedPopulation
                    .Where(x => x.Fitness >= fitnessThreshold)
                    .Select(x => x.Schedule)
                    .ToList();
                
                // Selekcja rodziców z reszty populacji
                List<int[,]> parents;

                // Selekcja rodziców z reszty populacji
                    parents = SelectParents(
                        sortedPopulation.Skip(eliteCount).Select(x => x.Schedule).ToList(),
                        sortedPopulation.Skip(eliteCount).Select(x => x.Fitness).ToArray(),
                        optimizationType, // Przekazanie odpowiedniego typu
                        random,
                        numberOfParents,
                        mutationFrequency);

                // Wykonanie krzyżowania
                var offspring = _crossoverService.PerformCrossover(parents, random, numberOfWorkers, daysInWeek, numberOfCrossoverPoints);

                // Wykonanie mutacji
                _mutationService.PerformMutation(offspring, random, numberOfWorkers, daysInWeek, mutationFrequency);

                // Teraz zapewniamy, że liczba osobników w populacji nie przekroczy rozmiaru
                // Populacja powinna składać się z elitarnych osobników i nowego pokolenia
                offspring.AddRange(eliteIndividuals);

                // Upewnij się, że populacja nie przekroczy ustalonego rozmiaru
                population = offspring.Take(populationSize).ToList();
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

                // Normalizacja wartości fitness
                for (int i = 0; i < fitness.Length; i++)
                {
                    if (fitness[i] > threshold)
                        fitness[i] = (fitness[i] - threshold) / (maxFitness - threshold);
                    else
                        fitness[i] = 0; // Przycina niskie wartości fitness
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
                int tournamentSize = Math.Max(2, numberOfParents); // Domyślny rozmiar turnieju

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
                if (slots[mid] < point) 
                    low = mid + 1;
                else 
                    high = mid;
            }
            // Sprawdzenie, aby zwrócić poprawny indeks
            return Math.Min(low, slots.Length - 1);
        }


        private enum OptimizationType
        {
            RouletteSelection,
            TournamentSelection
        }
    }
}
