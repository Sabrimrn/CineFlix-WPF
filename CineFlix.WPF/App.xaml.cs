using CineFlix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Windows;

namespace CineFlix.WPF
{
    public partial class App : Application
    {
        private IHost? _host;
        public static IServiceProvider Services { get; private set; } = null!;

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((ctx, cfg) =>
                {
                    cfg.SetBasePath(Directory.GetCurrentDirectory());
                    cfg.AddJsonFile("appsettings.json", optional: true);
                    cfg.AddUserSecrets<App>(optional: true);
                    cfg.AddEnvironmentVariables();
                })
                .ConfigureServices((ctx, services) =>
                {
                    var conn = ctx.Configuration.GetConnectionString("DefaultConnection")
                               ?? "Server=(localdb)\\mssqllocaldb;Database=CineFlixDb;Trusted_Connection=true;TrustServerCertificate=true";

                    services.AddDbContext<CineFlixDbContext>(opt => opt.UseSqlServer(conn));
                    services.AddIdentity<CineFlixUser, IdentityRole>(opts => { opts.User.RequireUniqueEmail = true; })
                            .AddEntityFrameworkStores<CineFlixDbContext>()
                            .AddDefaultTokenProviders();

                    services.AddSingleton<MainWindow>();
                    services.AddTransient<LoginWindow>();
                    services.AddTransient<RegisterWindow>();
                    services.AddTransient<FilmWindow>();
                    services.AddTransient<RegisseurWindow>();
                    services.AddTransient<GenreWindow>();
                    services.AddTransient<RolesWindow>();
                })
                .Build();

            await _host.StartAsync();
            Services = _host.Services;

            // Migrate + seed
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CineFlixDbContext>();
            var um = scope.ServiceProvider.GetRequiredService<UserManager<CineFlixUser>>();
            var rm = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                await db.Database.MigrateAsync();
                await CineFlixDbContext.SeedAsync(db, um, rm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"DB init failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Show login window
            var login = Services.GetRequiredService<LoginWindow>();
            login.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null) { await _host.StopAsync(); _host.Dispose(); }
            base.OnExit(e);
        }
    }
}