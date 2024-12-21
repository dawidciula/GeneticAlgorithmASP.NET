using Genetic_algorithm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AG.Models
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<OptimizationParameters> OptimizationParameters { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracja enum jako string w bazie danych
            modelBuilder.Entity<OptimizationParameters>()
                .Property(p => p.OptimizationType)
                .HasConversion(
                    v => v.ToString(), // Zapisuje nazwę enum (np. "Roulette" lub "Tournament")
                    v => (OptimizationType)Enum.Parse(typeof(OptimizationType), v) // Konwertuje string na enum
                );
        }
       
    }
}