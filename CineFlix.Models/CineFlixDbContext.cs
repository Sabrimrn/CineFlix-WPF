using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CineFlix.Models
{
    public class CineFlixDbContext : IdentityDbContext<CineFlixUser>
    {
        public CineFlixDbContext(DbContextOptions<CineFlixDbContext> options) : base(options) { }

        public DbSet<Film> Films { get; set; } = null!;
        public DbSet<Regisseur> Regisseurs { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<FilmGenre> FilmGenres { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Soft-delete query filters
            modelBuilder.Entity<Film>().HasQueryFilter(f => !f.IsDeleted);
            modelBuilder.Entity<Regisseur>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<Genre>().HasQueryFilter(g => !g.IsDeleted);
            modelBuilder.Entity<FilmGenre>().HasQueryFilter(fg => !fg.IsDeleted);
            modelBuilder.Entity<CineFlixUser>().HasQueryFilter(u => !u.IsDeleted);

            // Seed basic data
            modelBuilder.Entity<Regisseur>().HasData(Regisseur.Seed());
            modelBuilder.Entity<Genre>().HasData(Genre.Seed());
            modelBuilder.Entity<Film>().HasData(Film.Seed());
            modelBuilder.Entity<FilmGenre>().HasData(FilmGenre.Seed());
        }

        public override int SaveChanges()
        {
            HandleSoftDelete();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleSoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void HandleSoftDelete()
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).ToList();
            foreach (var e in entries)
            {
                if (e.Entity is Film f) { e.State = EntityState.Modified; f.IsDeleted = true; f.DeletedOn = DateTime.UtcNow; }
                if (e.Entity is Regisseur r) { e.State = EntityState.Modified; r.IsDeleted = true; r.DeletedOn = DateTime.UtcNow; }
                if (e.Entity is Genre g) { e.State = EntityState.Modified; g.IsDeleted = true; g.DeletedOn = DateTime.UtcNow; }
                if (e.Entity is FilmGenre fg) { e.State = EntityState.Modified; fg.IsDeleted = true; fg.DeletedOn = DateTime.UtcNow; }
                if (e.Entity is CineFlixUser u) { e.State = EntityState.Modified; u.IsDeleted = true; u.DeletedOn = DateTime.UtcNow; }
            }
        }

        // Seeder helper for runtime seeding (creates roles and users)
        public static async Task SeedAsync(CineFlixDbContext db, UserManager<CineFlixUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Administrator")) await roleManager.CreateAsync(new IdentityRole("Administrator"));
            if (!await roleManager.RoleExistsAsync("User")) await roleManager.CreateAsync(new IdentityRole("User"));

            var adminEmail = "admin@cineflix.local";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new CineFlixUser { UserName = adminEmail, Email = adminEmail, FirstName = "Admin", LastName = "Cine" };
                var r = await userManager.CreateAsync(admin, "Admin123!");
                if (r.Succeeded) await userManager.AddToRoleAsync(admin, "Administrator");
            }
        }
    }
}