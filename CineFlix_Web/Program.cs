using CineFlix_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURATIE & SERVICES ---

// Database connectie (SQLite)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<CineFlixDbContext>(options =>
    options.UseSqlite(connectionString));

// Identity System (Gebruikers, Rollen, Login eisen)
builder.Services.AddIdentity<CineFlixUser, IdentityRole>(options =>
{
    // Eis: Wachtwoord instellingen
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;

    // Eis: Unieke email
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false; // Zet op true als je email verificatie echt werkend maakt
})
.AddEntityFrameworkStores<CineFlixDbContext>()
.AddDefaultTokenProviders();

// Voeg MVC en Session support toe
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Eis: Middleware / Cookies

var app = builder.Build();

// --- 2. DATABASE SEEDING BIJ OPSTARTEN ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CineFlixDbContext>();
        var userManager = services.GetRequiredService<UserManager<CineFlixUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Zorg dat de database up-to-date is
        await context.Database.MigrateAsync();

        // Run de seeder
        await CineFlixDbContext.Seeder(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        // Eis: Logging
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Fout bij het seeden van de database.");
    }
}

// --- 3. MIDDLEWARE PIPELINE ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Voor CSS/JS (Bootstrap)

app.UseRouting();

app.UseSession(); // Eis: Eigen middleware (Session/Cookies)

app.UseAuthentication(); // Wie ben je?
app.UseAuthorization();  // Wat mag je?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
