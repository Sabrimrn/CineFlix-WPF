using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineFlix_Models
{
    public class CineFlixDbContext : IdentityDbContext<CineFlixUser>
    {
        // DbSets voor onze entiteiten
        public DbSet<Film> Films { get; set; }
        public DbSet<Regisseur> Regisseurs { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<FilmGenre> FilmGenres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Default connection string voor lokale ontwikkeling
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=CineFlixDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";

            if (!optionsBuilder.IsConfigured)
            {
                try
                {
                    // Probeer connection string uit user secrets te halen
                    var config = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddUserSecrets<CineFlixDbContext>(optional: true)
                        .AddEnvironmentVariables()
                        .Build();

                    string? configConnection = config.GetConnectionString("DefaultConnection");
                    if (!string.IsNullOrEmpty(configConnection))
                        connectionString = configConnection;
                }
                catch (Exception ex)
                {
                    // Als er een fout is, gebruik de default connection string
                    Console.WriteLine($"Error loading configuration: {ex.Message}");
                }
            }

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configureer soft delete query filters
            modelBuilder.Entity<Film>().HasQueryFilter(f => !f.IsDeleted);
            modelBuilder.Entity<Regisseur>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<Genre>().HasQueryFilter(g => !g.IsDeleted);
            modelBuilder.Entity<FilmGenre>().HasQueryFilter(fg => !fg.IsDeleted);
            modelBuilder.Entity<CineFlixUser>().HasQueryFilter(u => !u.IsDeleted);

            // Configureer relaties
            modelBuilder.Entity<Film>()
                .HasOne(f => f.Regisseur)
                .WithMany(r => r.Films)
                .HasForeignKey(f => f.RegisseurId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Film>()
                .HasOne(f => f.AddedByUser)
                .WithMany(u => u.AddedFilms)
                .HasForeignKey(f => f.AddedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<FilmGenre>()
                .HasOne(fg => fg.Film)
                .WithMany(f => f.FilmGenres)
                .HasForeignKey(fg => fg.FilmId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FilmGenre>()
                .HasOne(fg => fg.Genre)
                .WithMany(g => g.FilmGenres)
                .HasForeignKey(fg => fg.GenreId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index voor performance
            modelBuilder.Entity<Film>()
                .HasIndex(f => f.Titel);

            modelBuilder.Entity<Film>()
                .HasIndex(f => f.Releasejaar);

            modelBuilder.Entity<Regisseur>()
                .HasIndex(r => r.Naam);

            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.GenreNaam)
                .IsUnique();

            // Seed data voor Regisseurs
            modelBuilder.Entity<Regisseur>().HasData(Regisseur.SeedingData());

            // Seed data voor Genres
            modelBuilder.Entity<Genre>().HasData(Genre.SeedingData());
        }

        // Seeder methode om data toe te voegen na migratie
        public static async Task Seeder(CineFlixDbContext context, UserManager<CineFlixUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed rollen
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            if (!await roleManager.RoleExistsAsync("Manager"))
            {
                await roleManager.CreateAsync(new IdentityRole("Manager"));
            }

            // Seed admin gebruiker
            var adminEmail = "admin@cineflix.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new CineFlixUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "CineFlix",
                    RegistrationDate = DateTime.Now
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }

            // Seed test gebruiker
            var testEmail = "user@cineflix.com";
            var testUser = await userManager.FindByEmailAsync(testEmail);

            if (testUser == null)
            {
                testUser = new CineFlixUser
                {
                    UserName = testEmail,
                    Email = testEmail,
                    EmailConfirmed = true,
                    FirstName = "Test",
                    LastName = "Gebruiker",
                    RegistrationDate = DateTime.Now
                };

                var result = await userManager.CreateAsync(testUser, "User123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(testUser, "User");
                }
            }

            // Seed films als ze nog niet bestaan
            if (!context.Films.Any())
            {
                context.Films.AddRange(Film.SeedingData());
                await context.SaveChangesAsync();
            }

            // Seed film-genre koppelingen
            if (!context.FilmGenres.Any())
            {
                context.FilmGenres.AddRange(FilmGenre.SeedingData());
                await context.SaveChangesAsync();
            }

            // Set dummy objects
            Genre.Dummy = context.Genres.FirstOrDefault(g => g.GenreNaam == "-");
            Regisseur.Dummy = context.Regisseurs.FirstOrDefault(r => r.Naam == "Dummy Regisseur");
            Film.Dummy = context.Films.FirstOrDefault(f => f.Titel == "Dummy Film");
            CineFlixUser.Dummy = testUser;
        }

        // Override SaveChangesAsync voor soft delete
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleSoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        // Override SaveChanges voor soft delete
        public override int SaveChanges()
        {
            HandleSoftDelete();
            return base.SaveChanges();
        }

        private void HandleSoftDelete()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                if (entry.Entity is Film film)
                {
                    entry.State = EntityState.Modified;
                    film.IsDeleted = true;
                    film.DeletedOn = DateTime.Now;
                }
                else if (entry.Entity is Regisseur regisseur)
                {
                    entry.State = EntityState.Modified;
                    regisseur.IsDeleted = true;
                    regisseur.DeletedOn = DateTime.Now;
                }
                else if (entry.Entity is Genre genre)
                {
                    entry.State = EntityState.Modified;
                    genre.IsDeleted = true;
                    genre.DeletedOn = DateTime.Now;
                }
                else if (entry.Entity is FilmGenre filmGenre)
                {
                    entry.State = EntityState.Modified;
                    filmGenre.IsDeleted = true;
                    filmGenre.DeletedOn = DateTime.Now;
                }
                else if (entry.Entity is CineFlixUser user)
                {
                    entry.State = EntityState.Modified;
                    user.IsDeleted = true;
                    user.DeletedOn = DateTime.Now;
                }
            }
        }
    }
}