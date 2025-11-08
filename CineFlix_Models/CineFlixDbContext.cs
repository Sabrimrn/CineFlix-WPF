using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Nodig voor ConfigurationBuilder
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CineFlix_Models
{
    public class CineFlixDbContext : IdentityDbContext<CineFlixUser>
    {
        public DbSet<Film> Films { get; set; }
        public DbSet<Regisseur> Regisseurs { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<FilmGenre> FilmGenres { get; set; }

        public CineFlixDbContext(DbContextOptions<CineFlixDbContext> options) : base(options) { }

        public CineFlixDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@"Data Source=C:\Users\sabri\source\repos\CineFlix\CineFlix_WPF\cineflix.db");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<FilmGenre>().HasKey(fg => new { fg.FilmId, fg.GenreId });
        }

        // --- Seeder ---
        public static async Task Seeder(CineFlixDbContext context, UserManager<CineFlixUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            var adminUser = await userManager.FindByEmailAsync("admin@cineflix.com");
            if (adminUser == null)
            {
                adminUser = new CineFlixUser { UserName = "admin@cineflix.com", Email = "admin@cineflix.com", FirstName = "Admin", LastName = "Istrator", RegistrationDate = DateTime.Now, EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            var normalUser = await userManager.FindByEmailAsync("user@cineflix.com");
            if (normalUser == null)
            {
                normalUser = new CineFlixUser { UserName = "user@cineflix.com", Email = "user@cineflix.com", FirstName = "Normal", LastName = "User", RegistrationDate = DateTime.Now, EmailConfirmed = true };
                await userManager.CreateAsync(normalUser, "User123!");
                await userManager.AddToRoleAsync(normalUser, "User");
            }
        }
    }
}