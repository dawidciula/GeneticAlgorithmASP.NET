using AG.Models;
using Genetic_algorithm.Models;

namespace Genetic_algorithm.Repository
{
    public interface IRepository
    {
        Task<List<OptimizationParameters>> GetOptimizationParametersAsync();
        Task<OptimizationParameters> GetOptimizationParameterByIdAsync(int id);
        Task AddOptimizationParameterAsync(OptimizationParameters optimizationParameters);
        Task UpdateOptimizationParameterAsync(OptimizationParameters optimizationParameters);
        Task DeleteOptimizationParameterAsync(int id);
        
    }
}