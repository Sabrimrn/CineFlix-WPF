namespace CineFlix_Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Views.FilmDetailPage), typeof(Views.FilmDetailPage));
        }
    }
}
