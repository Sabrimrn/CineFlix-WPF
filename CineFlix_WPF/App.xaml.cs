using CineFlix_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace CineFlix_WPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Voer de database seeder uit bij het opstarten
            // We halen de benodigde services op uit de DI container
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CineFlixDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<CineFlixUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Roep de Seeder aan
                CineFlixDbContext.Seeder(dbContext, userManager, roleManager).Wait();
            }


            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // --- DE FIX IS HIER ---
            // Voeg logging services toe. Dit is nodig voor UserManager.
            services.AddLogging();

            // Voeg de DbContext toe
            services.AddDbContext<CineFlixDbContext>(options =>
                options.UseSqlite("Data Source=cineflix.db"));

            // Voeg Identity services toe
            services.AddIdentity<CineFlixUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<CineFlixDbContext>()
            .AddDefaultTokenProviders();

            // Voeg vensters toe aan de DI container
            services.AddTransient<MainWindow>();
            // services.AddTransient<LoginWindow>(); // etc.
        }
    }
}