using AG.Models;
using AG.Services;
using Genetic_algorithm.Interfaces;
using Genetic_algorithm.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Konfiguracja bazy danych
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();


//Services
builder.Services.AddScoped<FitnessService>();
builder.Services.AddScoped<Population>();
builder.Services.AddScoped<FourbrigadePopulation>();
builder.Services.AddScoped<CrossoverService>();
builder.Services.AddScoped<MutationService>();
builder.Services.AddScoped<AlgorithmService>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapRazorPages();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();