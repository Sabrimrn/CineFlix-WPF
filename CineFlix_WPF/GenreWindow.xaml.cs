using CineFlix_Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CineFlix_WPF
{
    public partial class GenreWindow : Window
    {
        private readonly CineFlixDbContext _context;
        private Genre _genre;

        public GenreWindow()
        {
            InitializeComponent();
            _context = App.ServiceProvider.GetRequiredService<CineFlixDbContext>();
        }

        public void SetGenre(Genre genre)
        {
            _genre = genre;
            if (_genre != null)
            {
                var tb = this.FindName("GenreNaamTextBox") as TextBox;
                if (tb != null)
                {
                    tb.Text = _genre.GenreNaam;
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tb = this.FindName("GenreNaamTextBox") as TextBox;
                var text = tb?.Text ?? string.Empty;

                if (string.IsNullOrWhiteSpace(text))
                {
                    MessageBox.Show("Genre naam is verplicht.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_genre == null)
                {
                    _genre = new Genre();
                    _context.Genres.Add(_genre);
                }

                _genre.GenreNaam = text.Trim();

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
