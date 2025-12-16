using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CineFlix_Models; // Zorg dat de referentie bestaat!

namespace CineFlix_Mobile.ViewModels
{
    public class FilmsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Film> Films { get; set; } = new ObservableCollection<Film>();

        // Event handler nullable maken met '?'
        public event PropertyChangedEventHandler? PropertyChanged;

        public FilmsViewModel()
        {
            LoadDummyData();
        }

        private void LoadDummyData()
        {
            // Gebruik de static data uit je model als die bestaat, anders handmatig
            Films.Add(new Film { Titel = "Inception", Releasejaar = 2010, Rating = 4.8 });
            Films.Add(new Film { Titel = "Interstellar", Releasejaar = 2014, Rating = 4.7 });
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}