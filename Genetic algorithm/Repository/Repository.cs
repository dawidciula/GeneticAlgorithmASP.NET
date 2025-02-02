using Microsoft.EntityFrameworkCore;
using AG.Models;


namespace Genetic_algorithm.Repository
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OptimizationParameters>> GetOptimizationParametersAsync()
        {
            return await _context.OptimizationParameters.ToListAsync();
        }

        public async Task<OptimizationParameters> GetOptimizationParameterByIdAsync(int id)
        {
            return await _context.OptimizationParameters
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddOptimizationParameterAsync(OptimizationParameters optimizationParameters)
        {
            _context.OptimizationParameters.Add(optimizationParameters);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOptimizationParameterAsync(OptimizationParameters optimizationParameters)
        {
            _context.OptimizationParameters.Update(optimizationParameters);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOptimizationParameterAsync(int id)
        {
            var optimizationParameter = await GetOptimizationParameterByIdAsync(id);
            if (optimizationParameter != null)
            {
                _context.OptimizationParameters.Remove(optimizationParameter);
                await _context.SaveChangesAsync();
            }
        }
    }
}
