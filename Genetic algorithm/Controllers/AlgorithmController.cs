using Microsoft.AspNetCore.Mvc;
using AG.Models;
using AG.Services;
using System.Threading.Tasks;

namespace AG.Controllers
{
    public class AlgorithmController : Controller
    {
        private readonly AlgorithmService _algorithmService;

        public AlgorithmController(AlgorithmService algorithmService)
        {
            _algorithmService = algorithmService;
        }

        // Widok początkowy, formularz do wprowadzenia danych
        public IActionResult Run()
        {
            return View();
        }

        // Metoda POST, która uruchamia algorytm
        [HttpPost]
        public async Task<IActionResult> Run(int optimizationType, int populationSize, int numberOfWorkers, int daysInWeek, double preferenceWeight)
        {
            if (!ModelState.IsValid)
            {
                // Jeśli formularz jest niepoprawny, zwróć formularz z błędami
                return View();
            }

            // Rozpoczęcie przetwarzania algorytmu
            ViewBag.Status = "Algorytm w trakcie przetwarzania...";

            // Przygotowanie parametrów optymalizacji
            var parameters = new OptimizationParameters
            {
                OptimizationType = optimizationType,
                PopulationSize = populationSize,
                NumberOfWorkers = numberOfWorkers,
                DaysInWeek = daysInWeek,
                PreferenceWeight = preferenceWeight
            };

            // Przekazanie parametrów do algorytmu, z pominięciem preferencji
            var result = await Task.Run(() => _algorithmService.RunAlgorithm(parameters, null));

            // Przekazanie wyniku do widoku "Result"
            return View("Result", result);
        }
    }
}