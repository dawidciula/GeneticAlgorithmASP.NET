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

        public IActionResult Run()
        {
            return View(new OptimizationParameters());
        }

        [HttpPost]
        public async Task<IActionResult> Run(OptimizationParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return View(parameters);
            }

            // Rozpoczęcie przetwarzania algorytmu
            ViewBag.Status = "Algorytm w trakcie przetwarzania...";
            var result = await Task.Run(() => _algorithmService.RunAlgorithm(parameters));

            // Przekazanie wyniku do widoku
            return View("Result", result);
        }
    }
}