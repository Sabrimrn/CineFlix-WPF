using CineFlix_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CineFlix_WPF
{
    public partial class MainWindow : Window
    {
        private readonly CineFlixDbContext _context;
        private readonly UserManager<CineFlixUser> _userManager;

        private readonly ObservableCollection<Film> _films = new();
        private readonly ObservableCollection<Regisseur> _regisseurs = new();
        private readonly ObservableCollection<Genre> _genres = new();
        private readonly ObservableCollection<CineFlixUser> _users = new();
        private readonly ICollectionView _filmsView;

        public MainWindow(CineFlixDbContext context, UserManager<CineFlixUser> userManager)
        {
            InitializeComponent();
            _context = context;
            _userManager = userManager;

            // Koppel de collecties aan de DataGrids
            FilmsDataGrid.ItemsSource = _films;
            RegisseursDataGrid.ItemsSource = _regisseurs;
            GenresDataGrid.ItemsSource = _genres;
            UsersDataGrid.ItemsSource = _users;

            _filmsView = CollectionViewSource.GetDefaultView(_films);

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAllDataAsync();
            UpdateUIForUser();
        }

        private async Task LoadAllDataAsync()
        {
            await LoadFilmsAsync();
            await LoadRegisseursAsync();
            await LoadGenresAsync();
            if (App.IsAdmin())
            {
                await LoadUsersAsync();
            }
        }

        private void UpdateUIForUser()
        {
            if (App.CurrentUser != null)
            {
                StatusTextBlock.Text = $"Ingelogd als: {App.CurrentUser.FullName} ({string.Join(", ", App.CurrentUserRoles ?? new System.Collections.Generic.List<string>())})";
                if (App.IsAdmin())
                {
                    // Maak de admin-specifieke UI elementen zichtbaar
                    UsersTab.Visibility = Visibility.Visible;
                    AdminMenu.Visibility = Visibility.Visible;
                }
            }
        }

        // --- Data Laad Methoden ---
        private async Task LoadFilmsAsync()
        {
            var filmsFromDb = await _context.Films.Include(f => f.Regisseur).OrderBy(f => f.Titel).ToListAsync();
            _films.Clear();
            foreach (var film in filmsFromDb) _films.Add(film);
        }

        private async Task LoadRegisseursAsync()
        {
            var regisseursFromDb = await _context.Regisseurs.OrderBy(r => r.Naam).ToListAsync();
            _regisseurs.Clear();
            foreach (var regisseur in regisseursFromDb) _regisseurs.Add(regisseur);
        }

        private async Task LoadGenresAsync()
        {
            var genresFromDb = await _context.Genres.OrderBy(g => g.GenreNaam).ToListAsync();
            _genres.Clear();
            foreach (var genre in genresFromDb) _genres.Add(genre);
        }

        private async Task LoadUsersAsync()
        {
            var usersFromDb = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
            _users.Clear();
            foreach (var user in usersFromDb)
            {
                _users.Add(user);
            }
        }

        // --- Film Acties ---
        private async void AddFilmButton_Click(object sender, RoutedEventArgs e)
        {
            var filmWindow = new FilmWindow(_context, null);
            if (filmWindow.ShowDialog() == true) await LoadFilmsAsync();
        }

        private async void EditFilmButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is Film filmToEdit)
            {
                var filmWindow = new FilmWindow(_context, filmToEdit);
                if (filmWindow.ShowDialog() == true) await LoadFilmsAsync();
            }
        }

        private async void DeleteFilmButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is Film filmToDelete)
            {
                if (MessageBox.Show($"Zeker dat je '{filmToDelete.Titel}' wilt verwijderen?", "Bevestig", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _context.Films.Remove(filmToDelete);
                    await _context.SaveChangesAsync();
                    _films.Remove(filmToDelete);
                }
            }
        }

        // --- Regisseur Acties ---
        private async void AddRegisseurButton_Click(object sender, RoutedEventArgs e)
        {
            var regisseurWindow = new RegisseurWindow(_context, null);
            if (regisseurWindow.ShowDialog() == true) await LoadRegisseursAsync();
        }

        private async void EditRegisseurButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is Regisseur regisseurToEdit)
            {
                var regisseurWindow = new RegisseurWindow(_context, regisseurToEdit);
                if (regisseurWindow.ShowDialog() == true) await LoadRegisseursAsync();
            }
        }

        private async void DeleteRegisseurButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is Regisseur regisseurToDelete)
            {
                bool isLinked = await _context.Films.AnyAsync(f => f.RegisseurId == regisseurToDelete.RegisseurId);
                if (isLinked)
                {
                    MessageBox.Show($"Kan '{regisseurToDelete.Naam}' niet verwijderen. De regisseur is nog gekoppeld aan films.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (MessageBox.Show($"Zeker dat je '{regisseurToDelete.Naam}' wilt verwijderen?", "Bevestig", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _context.Regisseurs.Remove(regisseurToDelete);
                    await _context.SaveChangesAsync();
                    _regisseurs.Remove(regisseurToDelete);
                }
            }
        }

        // --- Genre Acties ---
        private async void AddGenreButton_Click(object sender, RoutedEventArgs e)
        {
            var genreWindow = new GenreWindow(_context, null);
            if (genreWindow.ShowDialog() == true) await LoadGenresAsync();
        }

        private async void EditGenreButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is Genre genreToEdit)
            {
                var genreWindow = new GenreWindow(_context, genreToEdit);
                if (genreWindow.ShowDialog() == true) await LoadGenresAsync();
            }
        }

        private async void DeleteGenreButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is Genre genreToDelete)
            {
                bool isLinked = await _context.FilmGenres.AnyAsync(fg => fg.GenreId == genreToDelete.GenreId);
                if (isLinked)
                {
                    MessageBox.Show($"Kan '{genreToDelete.GenreNaam}' niet verwijderen. Het genre is nog gekoppeld aan films.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (MessageBox.Show($"Zeker dat je '{genreToDelete.GenreNaam}' wilt verwijderen?", "Bevestig", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _context.Genres.Remove(genreToDelete);
                    await _context.SaveChangesAsync();
                    _genres.Remove(genreToDelete);
                }
            }
        }

        // --- Gebruiker Acties ---
        private async void ToggleBlockUserButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is CineFlixUser userToToggle)
            {
                if (userToToggle.Id == App.CurrentUser?.Id)
                {
                    MessageBox.Show("Je kunt je eigen account niet blokkeren.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                userToToggle.IsDeleted = !userToToggle.IsDeleted;
                userToToggle.DeletedOn = userToToggle.IsDeleted ? DateTime.Now : null;

                var result = await _userManager.UpdateAsync(userToToggle);
                if (result.Succeeded)
                {
                    ICollectionView view = CollectionViewSource.GetDefaultView(UsersDataGrid.ItemsSource);
                    view.Refresh();
                    MessageBox.Show($"Gebruiker '{userToToggle.Email}' is {(userToToggle.IsDeleted ? "geblokkeerd" : "gedeblokkeerd")}.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private async void ManageRolesButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is CineFlixUser userToManage)
            {
                var rolesWindow = new RolesWindow();
                await rolesWindow.SetUserAsync(userToManage);

                rolesWindow.ShowDialog();
            }
        }

        // --- Filter & Navigatie Acties ---
        private void FilmSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _filmsView.Filter = item => (item as Film)?.Titel.ToLower().Contains(FilmSearchBox.Text.ToLower()) ?? false;
        }

        private void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            App.Logout();
            var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
            this.Close();
        }
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void FilmsMenuItem_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 0;
        private void RegisseursMenuItem_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 1;
        private void GenresMenuItem_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 2;
        private void UsersMenuItem_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 3;
    }
}