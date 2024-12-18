using AG.Models;
using AG.Services;
using Genetic_algorithm.Interfaces;
using Genetic_algorithm.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Zamiast AutoDetect, podaj wersję serwera MySQL ręcznie
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"), 
        new MySqlServerVersion(new Version(8, 3, 0)) // Wersja MySQL 8.0.0, dostosuj do swojej wersji
    )
);




//Services
builder.Services.AddScoped<FitnessService>();
builder.Services.AddScoped<Population>();
builder.Services.AddScoped<FourbrigadePopulation>();
builder.Services.AddScoped<CrossoverService>();
builder.Services.AddScoped<MutationService>();
builder.Services.AddScoped<AlgorithmService>();
builder.Services.AddScoped<IRepository, Repository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();