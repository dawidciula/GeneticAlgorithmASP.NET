using Microsoft.AspNetCore.Mvc;
using AG.Models;
using AG.Services;
using Genetic_algorithm.Models;
using Genetic_algorithm.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AG.Controllers
{
    public class AlgorithmController : Controller
{
    private readonly AlgorithmService _algorithmService;
    private readonly IRepository _repository;
    private readonly Population _population;

    public AlgorithmController(AlgorithmService algorithmService, IRepository repository, Population population)
    {
        _algorithmService = algorithmService;
        _repository = repository;
        _population = population;
    }

    // Widok początkowy
    public async Task<IActionResult> Run()
    {
        var optimizationParameters = await _repository.GetOptimizationParameterByIdAsync(1);

        var model = new CombinedAlgorithmParameters
        {
            OptimizationParametersList = optimizationParameters != null
                ? new List<OptimizationParameters> { optimizationParameters }
                : new List<OptimizationParameters> { new OptimizationParameters() },
            ScheduleParametersList = new List<ScheduleParameters> { new ScheduleParameters() },
        };

        return View(model);
    }

    // Zapisanie parametrów w bazie
   [HttpPost]
        public async Task<IActionResult> SaveAlgorithmSettings(CombinedAlgorithmParameters model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState nie jest prawidłowy:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                
                return View("Run", model);
            }

            // Pobierz istniejący rekord z ID = 1
            var existingParameters = await _repository.GetOptimizationParameterByIdAsync(1);

            // Logowanie danych wejściowych z formularza
            Console.WriteLine("Dane przesłane z formularza:");
            foreach (var param in model.OptimizationParametersList)
            {
                Console.WriteLine($"OptimizationType={param.OptimizationType}, PopulationSize={param.PopulationSize}, PreferenceWeight={param.PreferenceWeight}, MutationFrequency={param.MutationFrequency}, NumberOfParents={param.NumberOfParents}, ElitePercentage={param.ElitePercentage}");
            }

            // Jeśli rekord istnieje, zaktualizuj dane
            if (existingParameters != null)
            {
                var updatedParameters = model.OptimizationParametersList.FirstOrDefault();
                if (updatedParameters != null)
                {
                    existingParameters.OptimizationType = updatedParameters.OptimizationType;
                    existingParameters.PopulationSize = updatedParameters.PopulationSize;
                    existingParameters.PreferenceWeight = updatedParameters.PreferenceWeight;
                    existingParameters.MutationFrequency = updatedParameters.MutationFrequency;
                    existingParameters.NumberOfParents = updatedParameters.NumberOfParents;
                    existingParameters.ElitePercentage = updatedParameters.ElitePercentage;
                    existingParameters.MaxGenerations = updatedParameters.MaxGenerations;
                    existingParameters.MaxStagnation = updatedParameters.MaxStagnation;
                    existingParameters.CrossoverPoints = updatedParameters.CrossoverPoints;

                    // Aktualizacja w repozytorium
                    await _repository.UpdateOptimizationParameterAsync(existingParameters);

                    // Logowanie po aktualizacji
                    Console.WriteLine("Zaktualizowano parametry:");
                    Console.WriteLine($"OptimizationType={existingParameters.OptimizationType}, PopulationSize={existingParameters.PopulationSize}, PreferenceWeight={existingParameters.PreferenceWeight}, MutationFrequency={existingParameters.MutationFrequency}, NumberOfParents={existingParameters.NumberOfParents}, ElitePercentage={existingParameters.ElitePercentage}");
                }
            }
            else
            {
                // Jeśli rekord nie istnieje, dodaj nowy
                var newParameters = model.OptimizationParametersList.FirstOrDefault() ?? new OptimizationParameters
                {
                    Id = 1, // Ustaw ID na 1
                    OptimizationType = OptimizationType.Roulette,
                    PopulationSize = 100,
                    PreferenceWeight = 0.5,
                    MutationFrequency = 0.1,
                    NumberOfParents = 2,
                    ElitePercentage = 0.1,
                    MaxGenerations = 500000,
                    MaxStagnation = 250000
                };

                await _repository.AddOptimizationParameterAsync(newParameters);

                // Logowanie po dodaniu nowych danych
                Console.WriteLine("Dodano nowe parametry:");
                Console.WriteLine($"OptimizationType={newParameters.OptimizationType}, PopulationSize={newParameters.PopulationSize}, PreferenceWeight={newParameters.PreferenceWeight}, MutationFrequency={newParameters.MutationFrequency}, NumberOfParents={newParameters.NumberOfParents}, ElitePercentage={newParameters.ElitePercentage}");
            }

            // Przekierowanie na stronę główną
            TempData["Status"] = "Ustawienia zapisane!";
            return RedirectToAction("Run");
        }
    

    // Uruchomienie algorytmu
    [HttpPost]
    public async Task<IActionResult> Run(CombinedAlgorithmParameters model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Nieprawidłowe dane wejściowe.";
            return View("Run", model);
        }
        

        var optimizationParameters = await _repository.GetOptimizationParameterByIdAsync(1)
                                      ?? new OptimizationParameters
                                      {
                                          OptimizationType = OptimizationType.Roulette,
                                          PopulationSize = 100,
                                          PreferenceWeight = 0.5,
                                          MutationFrequency = 0.1,
                                          NumberOfParents = 2,
                                          ElitePercentage = 0.1,
                                          MaxGenerations = 100000,
                                          MaxStagnation = 1000,
                                          CrossoverPoints = 6
                                      };

        var scheduleParameters = model.ScheduleParametersList.FirstOrDefault()
                                 ?? new ScheduleParameters();

        var initialPopulation = _population.GenerateInitialPopulation(
            optimizationParameters.PopulationSize,
            scheduleParameters.NumberOfWorkers,
            scheduleParameters.DaysInWeek);
        
        

        var result = await Task.Run(() =>
            _algorithmService.RunAlgorithm(
                optimizationParameters,
                scheduleParameters,
                employeePreference: null // Możesz dodać obsługę preferencji, jeśli jest taka potrzeba
            ));

        return View("Result", result);
    }
}
}
