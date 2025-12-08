using System.Collections.ObjectModel;
using CineFlix_Models; // Je gedeelde modellen!
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CineFlix_Mobile.ViewModels
{
    // Dit is een basis MVVM ViewModel.
    // In de toekomst kun je de CommunityToolkit.Mvvm gebruiken om dit korter te maken.
    public class FilmsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Film> Films { get; set; } = new ObservableCollection<Film>();

        public FilmsViewModel()
        {
            // Tijdelijke dummy data om te testen of de layout werkt
            LoadDummyData();
        }

        private void LoadDummyData()
        {
            Films.Add(new Film { Titel = "Inception", Releasejaar = 2010, Rating = 4.8 });
            Films.Add(new Film { Titel = "Interstellar", Releasejaar = 2014, Rating = 4.7 });
            Films.Add(new Film { Titel = "The Dark Knight", Releasejaar = 2008, Rating = 4.9 });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
