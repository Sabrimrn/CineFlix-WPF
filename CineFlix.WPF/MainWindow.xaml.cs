using CineFlix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CineFlix.WPF
{
    public partial class MainWindow : Window
    {
        private readonly CineFlixDbContext _db;
        private readonly UserManager<CineFlixUser> _userManager;

        public MainWindow(CineFlixDbContext db, UserManager<CineFlixUser> userManager)
        {
            InitializeComponent();
            _db = db;
            _userManager = userManager;

            // Converters in resources
            Resources.Add("GenresConverter", new GenresConverter());

            LoadAll();
        }

        private async void LoadAll()
        {
            try
            {
                var films = await _db.Films.Include(f => f.Regisseur).Include(f => f.FilmGenres).ThenInclude(fg => fg.Genre).OrderBy(f => f.Titel).ToListAsync();
                FilmsGrid.ItemsSource = films;

                var regs = await _db.Regisseurs.OrderBy(r => r.Naam).ToListAsync();
                RegisseursGrid.ItemsSource = regs;

                var genres = await _db.Genres.OrderBy(g => g.GenreNaam).ToListAsync();
                GenresGrid.ItemsSource = genres;

                var users = await _db.Users.ToListAsync();
                UsersGrid.ItemsSource = users;

                // Fill filters
                GenreFilter.ItemsSource = genres.Prepend(new Genre { GenreNaam = "Alle Genres", GenreId = 0 });
                GenreFilter.SelectedIndex = 0;
                RegisseurFilter.ItemsSource = regs.Prepend(new Regisseur { Naam = "Alle Regisseurs", RegisseurId = 0 });
                RegisseurFilter.SelectedIndex = 0;

                StatText.Text = $"Films: {films.Count}, Regisseurs: {regs.Count}, Genres: {genres.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Load error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Simple handlers call popup windows (FilmWindow etc.)
        private void AddFilm_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new FilmWindow(null, _db);
            if (wnd.ShowDialog() == true) LoadAll();
        }

        private void EditFilm_Click(object sender, RoutedEventArgs e)
        {
            if (FilmsGrid.SelectedItem is Film f)
            {
                var wnd = new FilmWindow(f, _db);
                if (wnd.ShowDialog() == true) LoadAll();
            }
        }

        private async void DeleteFilm_Click(object sender, RoutedEventArgs e)
        {
            if (FilmsGrid.SelectedItem is Film f)
            {
                if (MessageBox.Show($"Delete {f.Titel}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _db.Films.Remove(f);
                        await _db.SaveChangesAsync();
                        LoadAll();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Delete failed: {ex.Message}");
                    }
                }
            }
        }

        private void FilmsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (FilmsGrid.SelectedItem is Film f)
            {
                var wnd = new FilmWindow(f, _db);
                wnd.ShowDialog();
                LoadAll();
            }
        }

        private void AddRegisseur_Click(object s, RoutedEventArgs e)
        {
            var wnd = new RegisseurWindow(null, _db);
            if (wnd.ShowDialog() == true) LoadAll();
        }

        private void EditRegisseur_Click(object s, RoutedEventArgs e)
        {
            if (RegisseursGrid.SelectedItem is Regisseur r)
            {
                var wnd = new RegisseurWindow(r, _db);
                if (wnd.ShowDialog() == true) LoadAll();
            }
        }

        private async void DeleteRegisseur_Click(object s, RoutedEventArgs e)
        {
            if (RegisseursGrid.SelectedItem is Regisseur r)
            {
                if (MessageBox.Show($"Delete {r.Naam}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _db.Regisseurs.Remove(r);
                        await _db.SaveChangesAsync();
                        LoadAll();
                    }
                    catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
                }
            }
        }

        private void AddGenre_Click(object s, RoutedEventArgs e)
        {
            var wnd = new GenreWindow(null, _db);
            if (wnd.ShowDialog() == true) LoadAll();
        }

        private void EditGenre_Click(object s, RoutedEventArgs e)
        {
            if (GenresGrid.SelectedItem is Genre g)
            {
                var wnd = new GenreWindow(g, _db);
                if (wnd.ShowDialog() == true) LoadAll();
            }
        }

        private async void DeleteGenre_Click(object s, RoutedEventArgs e)
        {
            if (GenresGrid.SelectedItem is Genre g)
            {
                if (MessageBox.Show($"Delete {g.GenreNaam}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _db.Genres.Remove(g);
                        await _db.SaveChangesAsync();
                        LoadAll();
                    }
                    catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
                }
            }
        }

        private void Films_Click(object s, RoutedEventArgs e) => MainTabs.SelectedIndex = 1;
        private void Regisseurs_Click(object s, RoutedEventArgs e) => MainTabs.SelectedIndex = 2;
        private void Genres_Click(object s, RoutedEventArgs e) => MainTabs.SelectedIndex = 3;
        private void Roles_Click(object s, RoutedEventArgs e) => MainTabs.SelectedIndex = 4;
        private void About_Click(object s, RoutedEventArgs e) => MessageBox.Show("CineFlix - Minimal App", "About");

        private void Logout_Click(object s, RoutedEventArgs e)
        {
            var login = App.Services.GetRequiredService<LoginWindow>();
            login.Show();
            this.Close();
        }

        private void Exit_Click(object s, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Filter_Changed(object s, SelectionChangedEventArgs e) => ApplyFilter();
        private void FilmSearchBox_TextChanged(object s, TextChangedEventArgs e) => ApplyFilter();

        private void ApplyFilter()
        {
            var view = CollectionViewSource.GetDefaultView(FilmsGrid.ItemsSource);
            if (view == null) return;

            view.Filter = obj =>
            {
                var film = obj as Film;
                if (film == null) return false;

                var search = FilmSearchBox.Text?.ToLower() ?? "";
                if (!string.IsNullOrWhiteSpace(search))
                {
                    // demonstration of LINQ method syntax (lambda)
                    if (!film.Titel.ToLower().Contains(search) && !(film.Beschrijving?.ToLower().Contains(search) ?? false))
                        return false;
                }

                if (GenreFilter.SelectedItem is Genre g && g.GenreId != 0)
                {
                    // demonstration of LINQ query syntax
                    var has = from fg in film.FilmGenres
                              where fg.GenreId == g.GenreId
                              select fg;
                    if (!has.Any()) return false;
                }

                if (RegisseurFilter.SelectedItem is Regisseur r && r.RegisseurId != 0)
                {
                    if (film.RegisseurId != r.RegisseurId) return false;
                }

                return true;
            };
        }

        private void ManageRoles_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is CineFlixUser user)
            {
                var wnd = new RolesWindow(user, _userManager);
                if (wnd.ShowDialog() == true) LoadAll();
            }
        }

        private async void ToggleBlock_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is CineFlixUser user)
            {
                try
                {
                    user.IsDeleted = !user.IsDeleted;
                    _db.Users.Update(user);
                    await _db.SaveChangesAsync();
                    LoadAll();
                    MessageBox.Show($"User {(user.IsDeleted ? "blocked" : "unblocked")}");
                }
                catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
            }
        }
    }

    // Converter to show genre names
    public class GenresConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is System.Collections.Generic.ICollection<FilmGenre> list && list.Count > 0)
                return string.Join(", ", list.Select(fg => fg.Genre?.GenreNaam ?? ""));
            return "None";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    }
}