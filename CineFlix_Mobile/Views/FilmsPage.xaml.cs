using CineFlix_Mobile.ViewModels;

namespace CineFlix_Mobile.Views
{
    public partial class FilmsPage : ContentPage
    {
        // We vragen de ViewModel hier op via de haakjes (Dependency Injection)
        public FilmsPage(FilmsViewModel vm)
        {
            InitializeComponent();

            // HIER gebeurt de magie: We vertellen de pagina wie zijn data levert
            BindingContext = vm;
        }

        // Dit zorgt dat de data geladen wordt elke keer dat je naar deze pagina gaat
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Als de BindingContext goed is ingesteld, kunnen we de LoadFilms functie aanroepen
            if (BindingContext is FilmsViewModel vm)
            {
                // Roep het commando aan om te laden
                await vm.LoadFilmsCommand.ExecuteAsync(null);
            }
        }
    }
}