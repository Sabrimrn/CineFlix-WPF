using CineFlix_Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace CineFlix_WPF
{
    public partial class FilmWindow : Window
    {
        private readonly CineFlixDbContext _context;
        private readonly Film _film;
        private readonly bool _isNew;

        // De constructor ontvangt nu de DbContext, wat beter is voor de structuur.
        public FilmWindow(CineFlixDbContext context, Film? film)
        {
            InitializeComponent();
            _context = context;

            if (film == null)
            {
                _film = new Film();
                _isNew = true;
                Title = "Nieuwe Film Toevoegen";
            }
            else
            {
                _film = _context.Films.Include(f => f.Regisseur).Single(f => f.FilmId == film.FilmId);
                _isNew = false;
                Title = "Film Bewerken";
            }

            DataContext = _film;
            Loaded += FilmWindow_Loaded;
        }

        private void FilmWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RegisseurComboBox.ItemsSource = _context.Regisseurs.OrderBy(r => r.Naam).ToList();

            TitelTextBox.Text = _film.Titel;
            ReleasejaarTextBox.Text = _film.Releasejaar > 0 ? _film.Releasejaar.ToString() : "";
            DuurMinutenTextBox.Text = _film.DuurMinuten > 0 ? _film.DuurMinuten.ToString() : "";
            BeschrijvingTextBox.Text = _film.Beschrijving;

            if (_film.Regisseur != null)
            {
                RegisseurComboBox.SelectedItem = _film.Regisseur;
            }

            // Stel de rating van de control in
            MyRatingControl.Value = _film.Rating;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _film.Titel = TitelTextBox.Text;
            if (int.TryParse(ReleasejaarTextBox.Text, out int jaar)) _film.Releasejaar = jaar;
            if (int.TryParse(DuurMinutenTextBox.Text, out int duur)) _film.DuurMinuten = duur;

            // Lees de waarde uit de RatingControl
            _film.Rating = MyRatingControl.Value;

            _film.Beschrijving = BeschrijvingTextBox.Text;

            if (RegisseurComboBox.SelectedItem is Regisseur selectedRegisseur)
            {
                _film.RegisseurId = selectedRegisseur.RegisseurId;
            }

            if (_isNew)
            {
                _context.Films.Add(_film);
            }
            else
            {
                _context.Films.Update(_film);
            }

            try
            {
                await _context.SaveChangesAsync();
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}