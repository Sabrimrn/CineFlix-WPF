using CineFlix_Models;
using CineFlix_Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Razor; // Nodig voor ViewLocalization
using System.Globalization; // Nodig voor CultureInfo
using Microsoft.AspNetCore.Localization; // Nodig voor RequestLocalization

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURATIE & SERVICES ---

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<CineFlixDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentity<CineFlixUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<CineFlixDbContext>()
.AddDefaultTokenProviders();

// NIEUW: Voeg Localization toe
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    // NIEUW: Support voor vertalingen in Views en Data Annotations (Models)
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// --- 2. DATABASE SEEDING ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CineFlixDbContext>();
        var userManager = services.GetRequiredService<UserManager<CineFlixUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();
        await CineFlixDbContext.Seeder(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
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
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<CineFlix_Web.Middleware.RequestLoggingMiddleware>();

// NIEUW: Configureer welke talen we ondersteunen
var supportedCultures = new[]
{
    new CultureInfo("nl"),
    new CultureInfo("en"),
    new CultureInfo("fr")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("nl"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();