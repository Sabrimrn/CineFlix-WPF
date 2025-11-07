using CineFlix_Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;

namespace CineFlix_WPF
{
    public partial class FilmWindow : Window
    {
        private readonly CineFlixDbContext _context;
        private readonly Film _film;
        private readonly bool _isNew;

        public FilmWindow(Film? film)
        {
            InitializeComponent();
            _context = App.ServiceProvider.GetRequiredService<CineFlixDbContext>();

            if (film == null)
            {
                _film = new Film();
                _isNew = true;
                Title = "Nieuwe Film Toevoegen";
            }
            else
            {
                // Laad de film opnieuw vanuit de context om tracking te garanderen
                _film = _context.Films.Include(f => f.Regisseur).Single(f => f.FilmId == film.FilmId);
                _isNew = false;
                Title = "Film Bewerken";
            }

            // Koppel data aan het venster
            DataContext = _film;
            Loaded += FilmWindow_Loaded;
        }

        private void FilmWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Vul de ComboBox voor regisseurs
            RegisseurComboBox.ItemsSource = _context.Regisseurs.OrderBy(r => r.Naam).ToList();

            // Koppel de UI aan de data
            TitelTextBox.Text = _film.Titel;
            ReleasejaarTextBox.Text = _film.Releasejaar.ToString();
            DuurMinutenTextBox.Text = _film.DuurMinuten.ToString();
            BeschrijvingTextBox.Text = _film.Beschrijving;
            RatingTextBox.Text = _film.Rating.ToString();
            if (_film.Regisseur != null)
            {
                RegisseurComboBox.SelectedItem = _film.Regisseur;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Update het film object met data uit de UI
            _film.Titel = TitelTextBox.Text;
            if (int.TryParse(ReleasejaarTextBox.Text, out int jaar)) _film.Releasejaar = jaar;
            if (int.TryParse(DuurMinutenTextBox.Text, out int duur)) _film.DuurMinuten = duur;
            if (double.TryParse(RatingTextBox.Text, out double rating)) _film.Rating = rating;

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
                DialogResult = true; // Signaleert succes aan het hoofdvenster
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}