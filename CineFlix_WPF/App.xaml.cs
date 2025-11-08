using CineFlix_Models;
using Microsoft.AspNetCore.Identity; 
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CineFlix_WPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static CineFlixUser? CurrentUser { get; set; }
        public static IList<string>? CurrentUserRoles { get; set; }

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CineFlixDbContext>();

            // De AddIdentity methode wordt nu herkend dankzij de nieuwe 'using' statement
            services.AddIdentity<CineFlixUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<CineFlixDbContext>()
            .AddDefaultTokenProviders();

            // Voeg de vensters toe aan de DI container
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<FilmWindow>();
            services.AddTransient<RegisseurWindow>();
            services.AddTransient<GenreWindow>();
            services.AddTransient<RolesWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CineFlixDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<CineFlixUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                try
                {
                    // Deze regel passen we straks aan!
                    await dbContext.Database.EnsureCreatedAsync();
                    await CineFlixDbContext.Seeder(dbContext, userManager, roleManager);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Databasefout: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        public static bool IsAdmin() => CurrentUserRoles?.Contains("Admin") ?? false;

        public static void Logout()
        {
            CurrentUser = null;
            CurrentUserRoles = null;
        }
    }
}