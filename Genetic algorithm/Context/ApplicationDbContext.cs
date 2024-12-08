using Microsoft.EntityFrameworkCore;

namespace AG.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet dla OptimizationParameters
        public DbSet<OptimizationParameters> OptimizationParameters { get; set; }

       
    }
}