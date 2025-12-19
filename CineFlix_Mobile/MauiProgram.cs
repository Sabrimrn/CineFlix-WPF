using Microsoft.Extensions.Logging;
using CineFlix_Mobile.Services;   
using CineFlix_Mobile.ViewModels; 
using CineFlix_Mobile.Views;      

namespace CineFlix_Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // Registreer de API Service
            builder.Services.AddSingleton<FilmsService>();

            // Registreer ViewModel
            builder.Services.AddTransient<FilmsViewModel>();

            // Registreer de Pagina
            builder.Services.AddTransient<FilmsPage>();

            return builder.Build();
        }
    }
}
