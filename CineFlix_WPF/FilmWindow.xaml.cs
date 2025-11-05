using CineFlix_Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CineFlix_WPF
{
    /// <summary>
    /// Interaction logic for FilmWindow.xaml
    /// </summary>
    public partial class FilmWindow : Window
    {
        private readonly CineFlixDbContext _context;
        private Film? _film;

        public FilmWindow(Film? film)
        {
            InitializeComponent();
            _context = App.ServiceProvider.GetRequiredService<CineFlixDbContext>();
            _film = film;

            Loaded += FilmWindow_Loaded;
        }

        private async void FilmWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadLookups();

            if (_film != null)
            {
                TitelTextBox.Text = _film.Titel;
                ReleasejaarTextBox.Text = _film.Releasejaar.ToString();
                DuurTextBox.Text = _film.DuurMinuten.ToString();
                BeschrijvingTextBox.Text = _film.Beschrijving;
                RatingControl.SetRating((int)Math.Round(_film.Rating));

                if (_film.RegisseurId.HasValue)
                {
                    RegisseurComboBox.SelectedItem = _context.Regisseurs.FirstOrDefault(r => r.RegisseurId == _film.RegisseurId.Value);
                }

                foreach (var fg in _film.FilmGenres)
                {
                    var item = _context.Genres.FirstOrDefault(g => g.GenreId == fg.GenreId);
                    if (item != null)
                    {
                        GenresListBox.SelectedItems.Add(item);
                    }
                }
            }
        }

        private async Task LoadLookups()
        {
            var regisseurs = _context.Regisseurs.OrderBy(r => r.Naam).ToList();
            RegisseurComboBox.ItemsSource = regisseurs;

            var genres = _context.Genres.OrderBy(g => g.GenreNaam).ToList();
            GenresListBox.ItemsSource = genres;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TitelTextBox.Text))
                {
                    MessageBox.Show("Titel is verplicht.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var isNew = false;
                if (_film == null)
                {
                    _film = new Film();
                    _context.Films.Add(_film);
                    isNew = true;
                }

                _film.Titel = TitelTextBox.Text.Trim();

                if (int.TryParse(ReleasejaarTextBox.Text, out int jaar)) _film.Releasejaar = jaar;
                if (int.TryParse(DuurTextBox.Text, out int duur)) _film.DuurMinuten = duur;

                _film.Beschrijving = string.IsNullOrWhiteSpace(BeschrijvingTextBox.Text) ? null : BeschrijvingTextBox.Text.Trim();
                _film.Rating = RatingControl.Value;

                var selectedRegisseur = RegisseurComboBox.SelectedItem as Regisseur;
                _film.RegisseurId = selectedRegisseur?.RegisseurId;

                var selectedGenres = GenresListBox.SelectedItems.Cast<Genre>().ToList();

                // If new film, save once to get FilmId
                if (isNew)
                {
                    await _context.SaveChangesAsync();
                }

                // Soft-delete existing FilmGenre links instead of removing
                var existing = _context.FilmGenres.Where(fg => fg.FilmId == _film.FilmId && !fg.IsDeleted).ToList();
                foreach (var ex in existing)
                {
                    ex.IsDeleted = true;
                    ex.DeletedOn = DateTime.Now;
                    // ensure EF tracks the modification
                    _context.FilmGenres.Update(ex);
                }

                // Add new links for selected genres
                foreach (var g in selectedGenres)
                {
                    var link = new FilmGenre { FilmId = _film.FilmId, GenreId = g.GenreId };
                    _context.FilmGenres.Add(link);
                }

                await _context.SaveChangesAsync();

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
