using CineFlix_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace CineFlix_WPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public static CineFlixUser? CurrentUser { get; set; }
        public static IList<string>? CurrentUserRoles { get; set; }

        public static bool IsAdmin => CurrentUserRoles?.Contains("Admin") ?? false;

        public static async Task LoginAsync(CineFlixUser user)
        {
            CurrentUser = user;
            var userManager = ServiceProvider.GetRequiredService<UserManager<CineFlixUser>>();
            CurrentUserRoles = await userManager.GetRolesAsync(user);
        }

        public static void Logout()
        {
            CurrentUser = null;
            CurrentUserRoles = null;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CineFlixDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<CineFlixUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                CineFlixDbContext.Seeder(dbContext, userManager, roleManager).Wait();
            }

            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            // --- DE FIX IS HIER ---
            // De AddDbContext-regel is nu compleet en correct.
            services.AddDbContext<CineFlixDbContext>(options =>
                options.UseSqlite(@"Data Source=C:\Users\sabri\source\repos\CineFlix\CineFlix_WPF\bin\Debug\net9.0-windows\cineflix.db")
            );
            // --- EINDE FIX ---

            services.AddIdentity<CineFlixUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<CineFlixDbContext>()
            .AddDefaultTokenProviders();

            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
        }
    }
}