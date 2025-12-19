using CineFlix_Mobile.ViewModels;

namespace CineFlix_Mobile.Views
{
    public partial class FilmDetailPage : ContentPage
    {
        public FilmDetailPage(FilmDetailViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}