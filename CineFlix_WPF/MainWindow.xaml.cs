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
        private readonly ICollectionView _filmsView;

        public MainWindow(CineFlixDbContext context, UserManager<CineFlixUser> userManager)
        {
            InitializeComponent();
            _context = context;
            _userManager = userManager;

            // Koppel de collecties aan de DataGrids
            FilmsDataGrid.ItemsSource = _films;
            RegisseursDataGrid.ItemsSource = _regisseurs;

            _filmsView = CollectionViewSource.GetDefaultView(_films);

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUIForUser();
            await LoadAllDataAsync();
        }

        private async Task LoadAllDataAsync()
        {
            await LoadFilmsAsync();
            await LoadRegisseursAsync();
            // Hier kunnen later ook andere laad-methodes komen (bv. LoadGenresAsync)
        }

        private void UpdateUIForUser()
        {
            if (App.CurrentUser != null)
            {
                StatusTextBlock.Text = $"Ingelogd als: {App.CurrentUser.FullName} ({string.Join(", ", App.CurrentUserRoles ?? new System.Collections.Generic.List<string>())})";
                if (App.IsAdmin())
                {
                    AdminMenu.Visibility = Visibility.Visible;
                }
            }
        }

        private async Task LoadFilmsAsync()
        {
            var filmsFromDb = await _context.Films
                .Include(f => f.Regisseur)
                .OrderBy(f => f.Titel)
                .ToListAsync();
            _films.Clear();
            foreach (var film in filmsFromDb) _films.Add(film);
        }

        private async Task LoadRegisseursAsync()
        {
            var regisseursFromDb = await _context.Regisseurs
                .OrderBy(r => r.Naam)
                .ToListAsync();
            _regisseurs.Clear();
            foreach (var regisseur in regisseursFromDb) _regisseurs.Add(regisseur);
        }

        // --- Film Acties ---
        private async void AddFilmButton_Click(object sender, RoutedEventArgs e)
        {
            var filmWindow = new FilmWindow(null);
            if (filmWindow.ShowDialog() == true) await LoadFilmsAsync();
        }

        private async void EditFilmButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is Film filmToEdit)
            {
                var filmWindow = new FilmWindow(filmToEdit);
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
                // Controleer of de regisseur nog aan films gekoppeld is
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

        private void FilmSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _filmsView.Filter = item => (item as Film)?.Titel.ToLower().Contains(FilmSearchBox.Text.ToLower()) ?? false;
        }

        // --- Menu & Navigatie Acties ---
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
        private void UsersMenuItem_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Gebruikersbeheer nog niet geïmplementeerd.");
    }
}