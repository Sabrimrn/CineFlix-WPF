using CineFlix_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CineFlix_WPF
{
    public partial class MainWindow : Window
    {
        private readonly CineFlixDbContext _context;
        private readonly UserManager<CineFlixUser> _userManager;
        private DispatcherTimer _timer;

        private ObservableCollection<Film> _films;
        private ObservableCollection<Regisseur> _regisseurs;
        private ObservableCollection<Genre> _genres;
        private ObservableCollection<CineFlixUser> _users;

        public MainWindow()
        {
            InitializeComponent();
            _context = App.ServiceProvider.GetRequiredService<CineFlixDbContext>();
            _userManager = App.ServiceProvider.GetRequiredService<UserManager<CineFlixUser>>();


            _films = new ObservableCollection<Film>();
            _regisseurs = new ObservableCollection<Regisseur>();
            _genres = new ObservableCollection<Genre>();
            _users = new ObservableCollection<CineFlixUser>();

            InitializeWindow();
            LoadData();
            SetupTimer();
        }

        private void InitializeWindow()
        {
            // Set user info in status bar
            if (App.CurrentUser != null)
            {
                StatusUserText.Text = $"Gebruiker: {App.CurrentUser.FullName}";
                StatusRoleText.Text = $"Rol: {string.Join(", ", App.CurrentUserRoles ?? new List<string>())}";
            }

            // Show/hide admin features based on role
            if (App.IsAdmin())
            {
                AdminTabItem.Visibility = Visibility.Visible;
                RolesMenuItem.Visibility = Visibility.Visible;
            }

            // Add converter to resources
            // this.Resources.Add("GenresConverter", new GenresConverter());
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => StatusTimeText.Text = DateTime.Now.ToString("HH:mm:ss");
            _timer.Start();
        }

        private async void LoadData()
        {
            try
            {
                await LoadFilms();
                await LoadRegisseurs();
                await LoadGenres();
                await LoadDashboardData();

                if (App.IsAdmin())
                {
                    await LoadUsers();
                }

                // Setup filter comboboxes
                SetupFilterComboBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden van data: {ex.Message}", "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadFilms()
        {
            var films = await _context.Films
                .Include(f => f.Regisseur)
                .Include(f => f.FilmGenres)
                    .ThenInclude(fg => fg.Genre)
                .OrderBy(f => f.Titel)
                .ToListAsync();

            _films.Clear();
            foreach (var film in films)
            {
                _films.Add(film);
            }

            FilmsDataGrid.ItemsSource = _films;
        }

        private async Task LoadRegisseurs()
        {
            var regisseurs = await _context.Regisseurs
                .Include(r => r.Films)
                .OrderBy(r => r.Naam)
                .ToListAsync();

            _regisseurs.Clear();
            foreach (var regisseur in regisseurs)
            {
                _regisseurs.Add(regisseur);
            }

            RegisseursDataGrid.ItemsSource = _regisseurs;
        }

        private async Task LoadGenres()
        {
            var genres = await _context.Genres
                .Include(g => g.FilmGenres)
                    .ThenInclude(fg => fg.Film)
                .OrderBy(g => g.GenreNaam)
                .ToListAsync();

            _genres.Clear();
            foreach (var genre in genres)
            {
                _genres.Add(genre);
            }

            GenresDataGrid.ItemsSource = _genres;
        }

        private async Task LoadUsers()
        {
            var users = await _context.Users.ToListAsync();

            _users.Clear();
            foreach (var user in users)
            {
                _users.Add(user);
            }

            UsersDataGrid.ItemsSource = _users;
        }

        private async Task LoadDashboardData()
        {
            // Load statistics
            var totalFilms = await _context.Films.CountAsync();
            var totalRegisseurs = await _context.Regisseurs.CountAsync();
            var totalGenres = await _context.Genres.CountAsync();
            var avgRating = await _context.Films.AverageAsync(f => (double?)f.Rating) ?? 0;

            TotalFilmsText.Text = totalFilms.ToString();
            TotalRegisseursText.Text = totalRegisseurs.ToString();
            TotalGenresText.Text = totalGenres.ToString();
            AverageRatingText.Text = avgRating.ToString("F1");

            // Load recent films
            var recentFilms = await _context.Films
                .Include(f => f.Regisseur)
                .OrderByDescending(f => f.FilmId)
                .Take(5)
                .ToListAsync();

            RecentFilmsDataGrid.ItemsSource = recentFilms;
        }

        private void SetupFilterComboBoxes()
        {
            // Genre filter
            var genreList = new List<Genre> { new Genre { GenreNaam = "Alle Genres" } };
            genreList.AddRange(_genres);
            GenreFilterComboBox.ItemsSource = genreList;
            GenreFilterComboBox.SelectedIndex = 0;

            // Regisseur filter
            var regisseurList = new List<Regisseur> { new Regisseur { Naam = "Alle Regisseurs" } };
            regisseurList.AddRange(_regisseurs);
            RegisseurFilterComboBox.ItemsSource = regisseurList;
            RegisseurFilterComboBox.SelectedIndex = 0;
        }

        // Film Methods
        private void AddFilmButton_Click(object sender, RoutedEventArgs e)
        {
            var filmWindow = new FilmWindow(null);
            if (filmWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditFilmButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var film = button?.Tag as Film;
            if (film != null)
            {
                var filmWindow = new FilmWindow(film);
                if (filmWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
        }

        private async void DeleteFilmButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var film = button?.Tag as Film;
            if (film != null)
            {
                var result = MessageBox.Show($"Weet je zeker dat je '{film.Titel}' wilt verwijderen?",
                    "Bevestig verwijderen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Films.Remove(film);
                        await _context.SaveChangesAsync();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fout bij verwijderen: {ex.Message}", "Fout",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void FilmsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var film = FilmsDataGrid.SelectedItem as Film;
            if (film != null)
            {
                var filmWindow = new FilmWindow(film);
                if (filmWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
        }

        // Regisseur Methods
        private void AddRegisseurButton_Click(object sender, RoutedEventArgs e)
        {
            var regisseurWindow = new RegisseurWindow(null);
            if (regisseurWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditRegisseurButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var regisseur = button?.Tag as Regisseur;
            if (regisseur != null)
            {
                var regisseurWindow = new RegisseurWindow(regisseur);
                if (regisseurWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
        }

        private async void DeleteRegisseurButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var regisseur = button?.Tag as Regisseur;
            if (regisseur != null)
            {
                var result = MessageBox.Show($"Weet je zeker dat je '{regisseur.Naam}' wilt verwijderen?",
                    "Bevestig verwijderen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Regisseurs.Remove(regisseur);
                        await _context.SaveChangesAsync();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fout bij verwijderen: {ex.Message}", "Fout",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Genre Methods
        private void AddGenreButton_Click(object sender, RoutedEventArgs e)
        {
            var genreWindow = new GenreWindow(null);
            if (genreWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditGenreButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var genre = button?.Tag as Genre;
            if (genre != null)
            {
                var genreWindow = new GenreWindow(genre);
                if (genreWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
        }

        private async void DeleteGenreButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var genre = button?.Tag as Genre;
            if (genre != null)
            {
                var result = MessageBox.Show($"Weet je zeker dat je '{genre.GenreNaam}' wilt verwijderen?",
                    "Bevestig verwijderen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Genres.Remove(genre);
                        await _context.SaveChangesAsync();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fout bij verwijderen: {ex.Message}", "Fout",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Search and Filter Methods
        private void FilmSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterFilms();
        }

        private void GenreFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterFilms();
        }

        private void RegisseurFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterFilms();
        }

        private void SearchFilmsButton_Click(object sender, RoutedEventArgs e)
        {
            FilterFilms();
        }

        private void FilterFilms()
        {
            if (FilmsDataGrid == null) return;

            var view = CollectionViewSource.GetDefaultView(FilmsDataGrid.ItemsSource);
            if (view == null) return;

            view.Filter = item =>
            {
                var film = item as Film;
                if (film == null) return false;

                // Text search
                var searchText = FilmSearchBox?.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    if (!film.Titel.ToLower().Contains(searchText) &&
                        !(film.Beschrijving?.ToLower().Contains(searchText) ?? false))
                    {
                        return false;
                    }
                }

                // Genre filter
                var selectedGenre = GenreFilterComboBox?.SelectedItem as Genre;
                if (selectedGenre != null && selectedGenre.GenreNaam != "Alle Genres")
                {
                    if (!film.FilmGenres.Any(fg => fg.GenreId == selectedGenre.GenreId))
                    {
                        return false;
                    }
                }

                // Regisseur filter
                var selectedRegisseur = RegisseurFilterComboBox?.SelectedItem as Regisseur;
                if (selectedRegisseur != null && selectedRegisseur.Naam != "Alle Regisseurs")
                {
                    if (film.RegisseurId != selectedRegisseur.RegisseurId)
                    {
                        return false;
                    }
                }

                return true;
            };
        }

        // User Management Methods
        private void ManageRolesButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button?.Tag as CineFlixUser;
            if (user != null)
            {
                var rolesWindow = new RolesWindow(user);
                if (rolesWindow.ShowDialog() == true)
                {
                    LoadUsers();
                }
            }
        }

        private async void ToggleBlockUserButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button?.Tag as CineFlixUser;
            if (user != null)
            {
                try
                {
                    user.IsDeleted = !user.IsDeleted;
                    user.DeletedOn = user.IsDeleted ? DateTime.Now : null;

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    await LoadUsers();

                    MessageBox.Show($"Gebruiker {(user.IsDeleted ? "geblokkeerd" : "gedeblokkeerd")}.",
                        "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fout bij wijzigen status: {ex.Message}", "Fout",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Menu Methods
        private void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            App.Logout();
            var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
            this.Close();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FilmsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 1;
        }

        private void RegisseursMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 2;
        }

        private void GenresMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 3;
        }

        private void RolesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsAdmin())
            {
                MainTabControl.SelectedIndex = 4;
            }
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("CineFlix Film Bibliotheek Beheer\nVersie 1.0\n\n© 2025 CineFlix",
                "Over CineFlix", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer?.Stop();
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}