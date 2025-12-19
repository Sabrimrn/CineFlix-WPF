using CommunityToolkit.Mvvm.ComponentModel;
using CineFlix_Models;

namespace CineFlix_Mobile.ViewModels
{
    // Dit zorgt ervoor dat de aangeklikte film automatisch in de variabele 'Film' komt
    [QueryProperty(nameof(Film), "Film")]
    public partial class FilmDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        Film? film;
    }
}