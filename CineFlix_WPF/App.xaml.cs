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
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddUserSecrets<App>(optional: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    // Registreer DbContext
                    services.AddDbContext<CineFlixDbContext>(options =>
                    {
                        var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
                            ?? "Server=(localdb)\\mssqllocaldb;Database=CineFlixDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";
                        options.UseSqlServer(connectionString);
                    });

                    // Configureer Identity
                    services.AddIdentity<CineFlixUser, IdentityRole>(options =>
                    {
                        // Wachtwoord instellingen
                        options.Password.RequireDigit = true;
                        options.Password.RequireLowercase = true;
                        options.Password.RequireUppercase = true;
                        options.Password.RequireNonAlphanumeric = true;
                        options.Password.RequiredLength = 6;

                        // Lockout instellingen
                        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                        options.Lockout.MaxFailedAccessAttempts = 5;
                        options.Lockout.AllowedForNewUsers = true;

                        // User instellingen
                        options.User.RequireUniqueEmail = true;
                    })
                    .AddEntityFrameworkStores<CineFlixDbContext>()
                    .AddDefaultTokenProviders();

                    // Registreer MainWindow expliciet
                    services.AddSingleton<MainWindow>();

                    // Registreer overige Windows automatisch
                    var windowTypes = typeof(App).Assembly.GetTypes()
                        .Where(t => typeof(Window).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && t != typeof(MainWindow));
                    foreach (var wt in windowTypes)
                    {
                        services.AddTransient(wt);
                    }
                })
                .Build();

            await _host.StartAsync();

            ServiceProvider = _host.Services;

            // Migreer de database en seed data
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CineFlixDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<CineFlixUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                try
                {
                    // Voer migraties uit
                    await context.Database.MigrateAsync();

                    // Seed de database
                    await CineFlixDbContext.Seeder(context, userManager, roleManager);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Er is een fout opgetreden bij het initialiseren van de database:\n{ex.Message}",
                        "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Show login window as startup window (minimal required flow)
            try
            {
                var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kan het login venster niet openen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            base.OnStartup(e);
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

        public static bool IsInRole(string role)
        {
            return CurrentUserRoles?.Contains(role) ?? false;
        }

        public static bool IsAdmin()
        {
            return IsInRole("Administrator");
        }

        public static void Logout()
        {
            CurrentUser = null;
            CurrentUserRoles = null;
        }
    }
}
