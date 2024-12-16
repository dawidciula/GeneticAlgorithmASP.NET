using System.Collections.Generic;

namespace Genetic_algorithm.Interfaces
{
    public interface IPopulationService
    {
        /// <summary>
        /// Generuje początkową populację harmonogramów.
        /// </summary>
        /// <param name="populationSize">Liczba populacji.</param>
        /// <param name="workers">Liczba pracowników.</param>
        /// <param name="days">Liczba dni w harmonogramie.</param>
        /// <returns>Lista populacji reprezentowanej jako tablice dwuwymiarowe.</returns>
        List<int[,]> GenerateInitialPopulation(int populationSize, int workers, int days);
    }
}