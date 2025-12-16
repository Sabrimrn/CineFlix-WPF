using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 
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
                optionsBuilder.UseSqlite("Data Source=cineflix.db");

            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Film>().HasQueryFilter(m => !m.IsDeleted);
            builder.Entity<Regisseur>().HasQueryFilter(m => !m.IsDeleted);
            builder.Entity<Genre>().HasQueryFilter(m => !m.IsDeleted);
            builder.Entity<FilmGenre>().HasQueryFilter(m => !m.IsDeleted);
        }

        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Deleted)
                {
                    
                    if (entry.Entity.GetType().GetProperty("isDeleted") != null)
                    {
                        entry.State = EntityState.Modified;
                        entry.Property("isDeleted").CurrentValue = true;
                    }
                }
            }
        }

        // --- Seeder ---
        public static async Task Seeder(CineFlixDbContext context, UserManager<CineFlixUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // ROLLEN EN USERS 
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
            if (!await roleManager.RoleExistsAsync("Manager"))
            {
                await roleManager.CreateAsync(new IdentityRole("Manager"));
            }

            var adminUser = await userManager.FindByEmailAsync("admin@cineflix.com");
            if (adminUser == null)
            {
                adminUser = new CineFlixUser { UserName = "admin@cineflix.com", Email = "admin@cineflix.com", FirstName = "Admin", LastName = "Istrator", RegistrationDate = DateTime.Now, EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            var managerUser = await userManager.FindByEmailAsync("manager@cineflix.com");
            if (managerUser == null)
            {
                managerUser = new CineFlixUser { UserName = "manager@cineflix.com", Email = "manager@cineflix.com", FirstName = "Manager", LastName = "User", RegistrationDate = DateTime.Now, EmailConfirmed = true };
                await userManager.CreateAsync(managerUser, "Manager123!");
                await userManager.AddToRoleAsync(managerUser, "Manager");
            }

            var normalUser = await userManager.FindByEmailAsync("user@cineflix.com");
            if (normalUser == null)
            {
                normalUser = new CineFlixUser { UserName = "user@cineflix.com", Email = "user@cineflix.com", FirstName = "Normal", LastName = "User", RegistrationDate = DateTime.Now, EmailConfirmed = true };
                await userManager.CreateAsync(normalUser, "User123!");
                await userManager.AddToRoleAsync(normalUser, "User");
            }

            // Controleer of er al films zijn. Zo ja, stop de seeder hier.
            if (context.Films.Any())
            {
                return;
            }

            // Maak Regisseurs aan (omdat films een RegisseurId nodig hebben)
            // De ID's (1, 2, 3, 4) komen overeen met de ID's in de Film.SeedingData()
            var regisseurs = new List<Regisseur>
            {
                new Regisseur { RegisseurId = 1, Naam = "Frank Darabont" },
                new Regisseur { RegisseurId = 2, Naam = "Francis Ford Coppola" },
                new Regisseur { RegisseurId = 3, Naam = "Christopher Nolan" },
                new Regisseur { RegisseurId = 4, Naam = "Quentin Tarantino" }
            };
            await context.Regisseurs.AddRangeAsync(regisseurs);

            // Maak Genres aan
                    var genres = new List<Genre>
            {
                new Genre { GenreId = 1, GenreNaam = "Drama" },
                new Genre { GenreId = 2, GenreNaam = "Crime" },
                new Genre { GenreId = 3, GenreNaam = "Action" }
            };
            await context.Genres.AddRangeAsync(genres);

            // Sla de regisseurs en genres eerst op, zodat ze een ID hebben
            await context.SaveChangesAsync();

            // Haal de lijst met films op uit jouw Film.cs model
            var filmsToSeed = Film.SeedingData();
            await context.Films.AddRangeAsync(filmsToSeed);

            // Sla de films op, zodat die ook een ID krijgen 
            await context.SaveChangesAsync();

            // Koppel de films aan de genres (via de FilmGenre tussentabel)
            var filmGenres = new List<FilmGenre>
            {
                // Koppel The Shawshank Redemption (FilmId 1) aan Drama (GenreId 1)
                new FilmGenre { FilmId = 1, GenreId = 1 },
                // Koppel The Godfather (FilmId 2) aan Drama (GenreId 1) en Crime (GenreId 2)
                new FilmGenre { FilmId = 2, GenreId = 1 },
                new FilmGenre { FilmId = 2, GenreId = 2 },
                // Koppel The Dark Knight (FilmId 3) aan Action (GenreId 3)
                new FilmGenre { FilmId = 3, GenreId = 3 },
                // Koppel Pulp Fiction (FilmId 4) aan Crime (GenreId 2)
                new FilmGenre { FilmId = 4, GenreId = 2 }
            };
            await context.FilmGenres.AddRangeAsync(filmGenres);

            // Sla de laatste koppelingen op
            await context.SaveChangesAsync();
        }
    }
}