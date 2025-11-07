using CineFlix_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace CineFlix_WPF
{
    public partial class App : Application
    {
        private IHost? _host;
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static CineFlixUser? CurrentUser { get; set; }
        public static List<string>? CurrentUserRoles { get; set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Registreer DbContext
                    services.AddDbContext<CineFlixDbContext>();

                    // Configureer Identity
                    services.AddIdentity<CineFlixUser, IdentityRole>(options =>
                    {
                        // Basis wachtwoordinstellingen voor testen
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequiredLength = 4;
                    })
                    .AddEntityFrameworkStores<CineFlixDbContext>()
                    .AddDefaultTokenProviders();

                    // Registreer alle vensters voor Dependency Injection
                    services.AddTransient<MainWindow>();
                    services.AddTransient<LoginWindow>();
                    services.AddTransient<RegisterWindow>();
                    services.AddTransient<FilmWindow>();
                    // Voeg hier andere vensters toe als je ze maakt (GenreWindow, etc.)
                })
                .Build();

            await _host.StartAsync();
            ServiceProvider = _host.Services;

            // Database initialiseren en seeden
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CineFlixDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<CineFlixUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                try
                {
                    // Zorgt dat de database bestaat. In productie gebruik je liever MigrateAsync().
                    await dbContext.Database.EnsureCreatedAsync();

                    // Seed de database met basisdata
                    await CineFlixDbContext.Seeder(dbContext, userManager, roleManager);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Databasefout: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Start de applicatie met het Login venster
            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            base.OnExit(e);
        }

        public static bool IsInRole(string role) => CurrentUserRoles?.Contains(role) ?? false;
        public static bool IsAdmin() => IsInRole("Administrator");
        public static void Logout()
        {
            CurrentUser = null;
            CurrentUserRoles = null;
        }
    }
}