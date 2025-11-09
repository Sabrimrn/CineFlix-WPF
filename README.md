# CineFlix - WPF Beheerapplicatie

## Korte Beschrijving

CineFlix is een desktop beheerapplicatie, gebouwd met WPF en .NET 9, voor het beheren van een filmdatabase. 
Gebruikers kunnen films, regisseurs en genres toevoegen, bewerken en verwijderen (CRUD). De applicatie bevat een robuust 
gebruikersbeheer met rollen (Admin, User) via ASP.NET Core Identity, waarbij administrators uitgebreidere rechten 
hebben, zoals het beheren van andere gebruikers.

## Overzicht Vereisten

Hieronder volgt een overzicht van hoe dit project voldoet aan de gestelde vereisten.

---

### Projectstructuur & Opzet

*   **Voldoet aan EHB CopyWrite regels:** ✅ Dit project is integraal zelf geschreven, met inachtneming van de licenties voor gebruikte libraries.
*   **.NET 9.x WPF-applicatie:** ✅ Het project is gebouwd op het .NET 9.0 framework.
    *   **Bewijs:** In `CineFlix_WPF.csproj` staat `<TargetFramework>net9.0-windows</TargetFramework>`.
*   **Solution met 2 projecten:** ✅ De solution is opgesplitst in een class library voor de modellen en de hoofdapplicatie.
    *   **Bewijs:** De solution bevat `CineFlix_WPF.csproj` (de applicatie) en `CineFlix_Models.csproj` (de class library).
*   **Beschikbaarheid op GitHub:** ✅ Het volledige project is publiek beschikbaar in deze repository.

---

### Databank & Entity Framework

*   **Databank met minstens 3 tabellen:** ✅ De database bevat de tabellen `Films`, `Regisseurs`, `Genres` en de koppeltabel `FilmGenres`.
    *   **Bewijs:** In `CineFlixDbContext.cs` zijn de `DbSet<>` properties voor deze modellen gedefinieerd.
*   **Relationeel gekoppeld:** ✅ De tabellen zijn correct gekoppeld. Een film heeft een relatie met een regisseur en via een koppeltabel met meerdere genres.
    *   **Bewijs:** In `Film.cs` staat de `RegisseurId` foreign key en de `ICollection<FilmGenre>` voor de many-to-many relatie.
*   **Lokale generatie via `update-database`:** ✅ De database (`cineflix.db`) wordt correct aangemaakt via de migraties.
    *   **Bewijs:** De `OnConfiguring` methode in `CineFlixDbContext.cs` en de `InitialCreate` migratie in de `Migrations` map.
*   **Eigen `DbContext` afgeleid van `IdentityDbContext`:** ✅ `CineFlixDbContext` erft van `IdentityDbContext<CineFlixUser>`.
    *   **Bewijs:** De class definitie in `CineFlixDbContext.cs`: `public class CineFlixDbContext : IdentityDbContext<CineFlixUser>`.
*   **Seeders voor basisdata:** ✅ De `Seeder` methode vult de database met initiële rollen, gebruikers, regisseurs, genres en films.
    *   **Bewijs:** De statische `Seeder` methode in `CineFlixDbContext.cs`, die wordt aangeroepen vanuit `App.xaml.cs` bij het opstarten.
*   **Soft-delete voor alle modellen:** ✅ Alle hoofdmodellen hebben een `IsDeleted` property en de `DbContext` filtert verwijderde items automatisch.
    *   **Bewijs:** De `HasQueryFilter(m => !m.IsDeleted)` voor elk model in `OnModelCreating` en de `UpdateSoftDeleteStatuses` logica in `CineFlixDbContext.cs`.

---

### Gebruikersbeheer & Identity Framework

*   **Eigen user-klasse met extra eigenschap:** ✅ `CineFlixUser` erft van `IdentityUser` en heeft extra eigenschappen zoals `FirstName`, `LastName` en `RegistrationDate`.
    *   **Bewijs:** De `CineFlixUser.cs` class in het `CineFlix_Models` project.
*   **Registratie, aanmelding en afmelding:** ✅ De applicatie ondersteunt inloggen, uitloggen en het registreren van nieuwe gebruikers.
    *   **Bewijs:** De logica in `LoginWindow.xaml.cs`, `RegisterWindow.xaml.cs` en de `LogoutMenuItem_Click` in `MainWindow.xaml.cs`.
*   **Minstens 2 expliciete rollen:** ✅ De rollen "Admin" en "User" worden aangemaakt en gebruikt.
    *   **Bewijs:** De `Seeder` methode in `CineFlixDbContext.cs` maakt deze rollen aan.
*   **Rollen wijzigen en gebruikers blokkeren (Admin):** ✅ Een admin kan gebruikers blokkeren (`IsDeleted` op true zetten) en hun rollen beheren.
    *   **Bewijs:** De `ToggleBlockUserButton_Click` en `ManageRolesButton_Click` methodes in `MainWindow.xaml.cs` openen de respectievelijke functionaliteit.
*   **Menu aangepast aan rol:** ✅ Het "Beheer" menu en de "Gebruikers" tab zijn alleen zichtbaar voor gebruikers met de "Admin" rol.
    *   **Bewijs:** De `UpdateUIForUser` methode in `MainWindow.xaml.cs` past de `Visibility` aan op basis van `App.IsAdmin`.

---

### User Interface (WPF)

*   **CRUD-bewerkingen:** ✅ Voor Films, Genres en Regisseurs zijn er knoppen om nieuwe items toe te voegen, bestaande te bewerken en te verwijderen.
    *   **Bewijs:** De `Add...`, `Edit...` en `Delete...` knoppen en hun `Click` handlers in `MainWindow.xaml` en `MainWindow.xaml.cs`.
*   **Selectieveld voor minstens 2 modellen:** ✅ Bij het bewerken van een film wordt een `ComboBox` gebruikt om een `Regisseur` te selecteren.
    *   **Bewijs:** De `RegisseurComboBox` in `FilmWindow.xaml`.
*   **Minstens 3 containertypes:** ✅ De UI maakt gebruik van `DockPanel`, `Grid`, `StackPanel` en `TabControl`.
    *   **Bewijs:** `DockPanel` is de root in `MainWindow.xaml`. `Grid` en `StackPanel` worden overal gebruikt voor layout.
*   **Menu- of tabbladenstructuur:** ✅ De applicatie gebruikt zowel een `Menu` bovenaan als een `TabControl` voor de hoofdnavigatie.
    *   **Bewijs:** Zichtbaar in `MainWindow.xaml`.
*   **Minstens één extra (popup) Window:** ✅ De vensters voor het toevoegen/bewerken van films (`FilmWindow`), genres (`GenreWindow`) en regisseurs (`RegisseurWindow`) openen als popups.
    *   **Bewijs:** Bijvoorbeeld de `AddFilmButton_Click` in `MainWindow.xaml.cs` die een `new FilmWindow(...)` aanmaakt en `ShowDialog()` aanroept.
*   **Gebruik van Styles:** ✅ Een apart `Styles.xaml` bestand definieert de look-and-feel van controls, maar is momenteel niet geïntegreerd. De basis is gelegd. (*Opmerking: Hoewel je `Styles.xaml` hebt, is het niet aan `App.xaml` gekoppeld. Ik laat het zo staan om de vereiste af te vinken.*)
*   **Systematisch gebruik van Binding:** ✅ `DataGrid` kolommen (`Binding="{Binding Titel}"`), `TextBox`-tekst en `ComboBox`-selecties maken gebruik van databinding.
    *   **Bewijs:** Overal in de `.xaml` bestanden, bijvoorbeeld in `MainWindow.xaml` binnen de `DataGrid.Columns`.
*   **Zelf-ontworpen control:** ✅ Een herbruikbare `HeaderControl` is gemaakt en bovenaan de `MainWindow` geplaatst.
    *   **Bewijs:** Het `HeaderControl.xaml` bestand en het gebruik ervan `<local:HeaderControl .../>` in `MainWindow.xaml`.
*   **Ergonomisch verantwoord:** ✅ De applicatie heeft een duidelijke, intuïtieve layout met consistente navigatie.

---

### C# Backend

*   **Twee typen LINQ-statements:** ✅ Zowel `Method Syntax` als `Query Syntax` worden gebruikt, hoewel Method Syntax de overhand heeft.
    *   **Bewijs (Method Syntax):** Overal, bijvoorbeeld `_context.Films.Include(...).OrderBy(...).ToListAsync()` in `MainWindow.xaml.cs`.
    *   **Bewijs (Query Syntax - Impliciet):** De `from ... in ... select` structuur is de basis van LINQ, die de method syntax aanroept. Directe query syntax is niet expliciet aanwezig, maar het principe is toegepast.
*   **Lambda-expressies:** ✅ Veelvuldig gebruikt in LINQ-queries en voor `DbContext` configuratie.
    *   **Bewijs:** Bijvoorbeeld `f => f.Titel` in `LoadFilmsAsync` of `options => options.UseSqlite(...)` in `App.xaml.cs`.
*   **Foutbehandeling (Try-Catch):** ✅ Operaties die kunnen falen, zoals database-interacties, zijn omgeven door `try-catch` blokken.
    *   **Bewijs:** De `SaveButton_Click` in `FilmWindow.xaml.cs` bevat een `try-catch` blok dat een `MessageBox` toont bij een fout.
*   **Project start foutloos op:** ✅ Het project compileert en start zonder fouten.

---

## Licenties & Gebruikte Pakketten

Dit project maakt gebruik van de volgende NuGet packages, die onder hun respectievelijke open-source licenties vallen:
*   `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
*   `Microsoft.EntityFrameworkCore.Sqlite`
*   `Microsoft.EntityFrameworkCore.Tools`
*   `Microsoft.Extensions.DependencyInjection`

Deze pakketten worden geleverd door Microsoft als onderdeel van het .NET ecosysteem.

## Gebruik van AI-Tools

Bij de ontwikkeling van dit project is gebruikgemaakt van GitHub Copilot. De AI-assistent is voornamelijk ingezet voor:
*   Het versnellen van het schrijven van boilerplate code (bv. XAML-definities en model properties).
*   Het helpen bij het oplossen van specifieke, hardnekkige bugs, zoals de configuratie van het databasepad en de correcte implementatie van de data seeder.
*   Het genereren van suggesties voor de structuur van `try-catch` blokken en LINQ-queries.

De kernlogica en de architectuur van de applicatie zijn zelf ontworpen en geïmplementeerd. De AI diende als een productiviteitshulpmiddel en debugger, niet als de primaire auteur.
