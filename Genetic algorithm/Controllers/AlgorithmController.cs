using Microsoft.AspNetCore.Mvc;
using AG.Models;
using AG.Services;
using System.Threading.Tasks;
using Genetic_algorithm.Models;
using Microsoft.EntityFrameworkCore;

namespace AG.Controllers
{
    public class AlgorithmController : Controller
    {
        private readonly AlgorithmService _algorithmService;
        private readonly ApplicationDbContext _context;

        public AlgorithmController(AlgorithmService algorithmService, ApplicationDbContext context)
        {
            _algorithmService = algorithmService;
            _context = context;
        }

        // Widok początkowy, formularz do wprowadzenia danych
        public IActionResult Run()
        {
            var model = new CombinedAlgorithmParameters
            {
                // Inicjujemy puste listy dla formularzy
                OptimizationParametersList = new List<OptimizationParameters> { new OptimizationParameters() },
                ScheduleParametersList = new List<ScheduleParameters> { new ScheduleParameters() }
            };

            return View(model);
        }

        // Metoda POST, która zapisuje parametry algorytmu w bazie danych
        [HttpPost]
        public async Task<IActionResult> SaveAlgorithmSettings(CombinedAlgorithmParameters model)
        {
            if (!ModelState.IsValid)
            {
                // Jeśli formularz zawiera błędy, zwróć go z powrotem do widoku
                return View("Run", model);
            }

            // Zapisz dane optymalizacji w bazie danych (tylko parametry z listy OptimizationParametersList)
            foreach (var optimizationParameters in model.OptimizationParametersList)
            {
                _context.OptimizationParameters.Add(optimizationParameters);
            }

            // Zapisujemy zmiany w bazie danych
            await _context.SaveChangesAsync();

            // Logowanie zapisanych danych (opcjonalne)
            var savedOptimizationParameters = await _context.OptimizationParameters.ToListAsync();
            foreach (var param in savedOptimizationParameters)
            {
                Console.WriteLine($"Zapisane parametry: OptimizationType={param.OptimizationType}, " +
                                  $"PopulationSize={param.PopulationSize}, " +
                                  $"PreferenceWeight={param.PreferenceWeight}, " +
                                  $"MutationFrequency={param.MutationFrequency}, " +
                                  $"NumberOfParents={param.NumberOfParents}, " +
                                  $"ElitePercentage={param.ElitePercentage}");
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
                return View();  // Jeśli formularz jest niepoprawny, zwróć formularz z błędami
            }

            // Rozpoczęcie przetwarzania algorytmu
            ViewBag.Status = "Algorytm w trakcie przetwarzania...";

            // Przygotowanie parametrów optymalizacji z bazy danych (jeśli zostały zapisane)
            var optimizationParameters = await _context.OptimizationParameters.FirstOrDefaultAsync();

            if (optimizationParameters == null)
            {
                optimizationParameters = new OptimizationParameters
                {
                    OptimizationType = model.OptimizationParametersList.FirstOrDefault()?.OptimizationType ?? 1,
                    PopulationSize = model.OptimizationParametersList.FirstOrDefault()?.PopulationSize ?? 100,
                    PreferenceWeight = model.OptimizationParametersList.FirstOrDefault()?.PreferenceWeight ?? 0.5,
                    MutationFrequency = model.OptimizationParametersList.FirstOrDefault()?.MutationFrequency ?? 0.1,
                    NumberOfParents = model.OptimizationParametersList.FirstOrDefault()?.NumberOfParents ?? 2,
                    ElitePercentage = model.OptimizationParametersList.FirstOrDefault()?.ElitePercentage ?? 0.1
                };
            }

            // Przygotowanie parametrów harmonogramu z formularza
            var scheduleParameters = model.ScheduleParametersList.FirstOrDefault() ?? new ScheduleParameters();

            // Przekazanie parametrów do algorytmu
            var result = await Task.Run(() => _algorithmService.RunAlgorithm(optimizationParameters, scheduleParameters, employeePreference: null));

            // Przekazanie wyniku do widoku "Result"
            return View("Result", result);
        }
    }
}
