using CineFlix_Models;
using System.Linq;
using System.Windows;

namespace CineFlix_WPF
{
    public partial class GenreWindow : Window
    {
        private readonly CineFlixDbContext _context;
        private readonly Genre _genre;
        private readonly bool _isNew;

        public GenreWindow(CineFlixDbContext context, Genre? genre)
        {
            InitializeComponent();
            _context = context;

            if (genre == null)
            {
                _genre = new Genre();
                _isNew = true;
                Title = "Nieuw Genre Toevoegen";
            }
            else
            {
                _genre = _context.Genres.Single(g => g.GenreId == genre.GenreId);
                _isNew = false;
                Title = "Genre Bewerken";
            }

            // Vul de UI
            GenreNaamTextBox.Text = _genre.GenreNaam;
            BeschrijvingTextBox.Text = _genre.Beschrijving;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(GenreNaamTextBox.Text))
            {
                MessageBox.Show("Genre naam is verplicht.", "Validatiefout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _genre.GenreNaam = GenreNaamTextBox.Text;
            _genre.Beschrijving = BeschrijvingTextBox.Text;

            if (_isNew) _context.Genres.Add(_genre);
            else _context.Genres.Update(_genre);

            await _context.SaveChangesAsync();
            DialogResult = true;
            this.Close();
        }
    }
}