using Genetic_algorithm.Models;
using Microsoft.EntityFrameworkCore;

namespace AG.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<OptimizationParameters> OptimizationParameters { get; set; }
       
    }
}