using CineFlix.Models;
using System;
using System.Linq;
using System.Windows;

namespace CineFlix.WPF
{
    public partial class FilmWindow : Window
    {
        private readonly CineFlixDbContext _db;
        private Film? _film;

        public FilmWindow(Film? film, CineFlixDbContext db)
        {
            InitializeComponent();
            _db = db;
            _film = film;
            LoadData();
        }

        private void LoadData()
        {
            RegisseurBox.ItemsSource = _db.Regisseurs.ToList();
            GenresList.ItemsSource = _db.Genres.ToList();

            if (_film != null)
            {
                Titel.Text = _film.Titel;
                Year.Text = _film.Releasejaar.ToString();
                Duration.Text = _film.DuurMinuten.ToString();
                RegisseurBox.SelectedItem = _film.Regisseur;
                // select genres
                foreach (var g in _film.FilmGenres.Select(fg => fg.Genre))
                {
                    GenresList.SelectedItems.Add(g);
                }
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_film == null) _film = new Film();

                _film.Titel = Titel.Text.Trim();
                _film.Releasejaar = int.TryParse(Year.Text, out var y) ? y : 0;
                _film.DuurMinuten = int.TryParse(Duration.Text, out var d) ? d : 0;
                _film.Regisseur = RegisseurBox.SelectedItem as Regisseur;

                // manage genres (simple)
                _film.FilmGenres.Clear();
                foreach (Genre g in GenresList.SelectedItems)
                {
                    _film.FilmGenres.Add(new FilmGenre { Film = _film, Genre = g, GenreId = g.GenreId });
                }

                if (_film.FilmId == 0) _db.Films.Add(_film);
                else _db.Films.Update(_film);

                await _db.SaveChangesAsync();
                DialogResult = true;
            }
            catch (Exception ex) { MessageBox.Show($"Save error: {ex.Message}"); }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}