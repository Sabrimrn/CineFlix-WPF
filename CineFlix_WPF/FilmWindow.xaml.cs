using CineFlix_Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;

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
            // Minimal UI initialization — keep empty to avoid runtime errors
        }
    }
}
