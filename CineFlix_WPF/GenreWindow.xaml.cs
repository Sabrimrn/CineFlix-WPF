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
        private readonly Genre _genre; // Wordt nu in constructor gevuld
        private readonly bool _isNew;

        // Constructor voor DI
        public GenreWindow(CineFlixDbContext context, Genre? genre)
        {
            InitializeComponent();
            _context = context;

            if (genre == null)
            {
                _genre = new Genre();
                _isNew = true;
                Title = "Nieuw Genre";
            }
            else
            {
                _genre = _context.Genres.Single(g => g.GenreId == genre.GenreId);
                _isNew = false;
                Title = "Genre Bewerken";
            }

            // Vul UI
            GenreNaamTextBox.Text = _genre.GenreNaam;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _genre.GenreNaam = GenreNaamTextBox.Text;

            if (_isNew) _context.Genres.Add(_genre);
            else _context.Genres.Update(_genre);

            await _context.SaveChangesAsync();
            DialogResult = true;
        }
    }
}