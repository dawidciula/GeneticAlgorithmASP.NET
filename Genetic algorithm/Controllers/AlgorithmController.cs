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

        public AlgorithmController(AlgorithmService algorithmService, IRepository repository)
        {
            _algorithmService = algorithmService;
            _repository = repository;
        }

        // Widok początkowy, formularz do wprowadzenia danych
        public async Task<IActionResult> Run()
        {
            // Pobierz zapisane parametry optymalizacji z repozytorium
            var optimizationParameters = await _repository.GetOptimizationParameterByIdAsync(1); // Zawsze ID = 1

            var model = new CombinedAlgorithmParameters
            {
                OptimizationParametersList = optimizationParameters != null
                    ? new List<OptimizationParameters> { optimizationParameters }
                    : new List<OptimizationParameters> { new OptimizationParameters() },
                ScheduleParametersList = new List<ScheduleParameters> { new ScheduleParameters() } // ScheduleParameters nie są zapisywane w bazie
            };

            return View(model);
        }

        // Metoda POST, która dodaje lub aktualizuje parametry optymalizacji w bazie
        [HttpPost]
        public async Task<IActionResult> SaveAlgorithmSettings(CombinedAlgorithmParameters model)
        {
            if (!ModelState.IsValid)
            {
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
                    ElitePercentage = 0.1
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

        // Metoda POST, która uruchamia algorytm
        [HttpPost]
        public async Task<IActionResult> Run(CombinedAlgorithmParameters model)
        {
            if (!ModelState.IsValid)
            {
                return View(); // Jeśli formularz jest niepoprawny, zwróć formularz z błędami
            }

            ViewBag.Status = "Algorytm w trakcie przetwarzania...";

            // Pobierz parametry optymalizacji z repozytorium
            var optimizationParameters = await _repository.GetOptimizationParameterByIdAsync(1);

            // Jeśli brak zapisanych parametrów, ustaw domyślne wartości
            if (optimizationParameters == null)
            {
                optimizationParameters = new OptimizationParameters
                {
                    OptimizationType = OptimizationType.Roulette,
                    PopulationSize = 100,
                    PreferenceWeight = 0.5,
                    MutationFrequency = 0.1,
                    NumberOfParents = 2,
                    ElitePercentage = 0.1
                };
            }

            // Parametry harmonogramu z formularza (nie zapisywane w bazie)
            var scheduleParameters = model.ScheduleParametersList.FirstOrDefault() ?? new ScheduleParameters();

            // Przekazanie parametrów do algorytmu
            var result = await Task.Run(() =>
                _algorithmService.RunAlgorithm(optimizationParameters, scheduleParameters, employeePreference: null));

            // Zwracanie wyniku do widoku "Result"
            return View("Result", result);
        }
    }
}
