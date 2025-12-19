using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CineFlix_Models;
using CineFlix_Mobile.Services;
using System.Diagnostics;

namespace CineFlix_Mobile.ViewModels
{
    public partial class FilmsViewModel : ObservableObject
    {
        private readonly FilmsService _filmsService;

        // We bewaren hier alle films (de originele lijst)
        private List<Film> _allFilms = new();

        // Dit is de lijst die op het scherm staat (gefilterd)
        [ObservableProperty]
        ObservableCollection<Film> films;

        [ObservableProperty]
        bool isLoading;

        // NIEUW: De tekst die je intypt in de zoekbalk
        [ObservableProperty]
        string searchText;

        public FilmsViewModel(FilmsService filmsService)
        {
            _filmsService = filmsService;
            Films = new ObservableCollection<Film>();
        }

        // NIEUW: Deze functie wordt aangeroepen als je typt of op Enter drukt
        [RelayCommand]
        public void PerformSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Zoekbalk leeg? Toon alles!
                Films = new ObservableCollection<Film>(_allFilms);
            }
            else
            {
                // Filter de lijst op titel (hoofdletterongevoelig)
                var filtered = _allFilms.Where(f => f.Titel.ToLower().Contains(SearchText.ToLower())).ToList();
                Films = new ObservableCollection<Film>(filtered);
            }
        }

        [RelayCommand]
        public async Task LoadFilms()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                var filmsFromApi = await _filmsService.GetFilmsAsync();

                // Sla de films op in de "backup" lijst
                _allFilms = filmsFromApi;

                Films.Clear();
                foreach (var film in filmsFromApi)
                {
                    Films.Add(film);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}