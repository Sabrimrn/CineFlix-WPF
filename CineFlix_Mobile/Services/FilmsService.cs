using System.Diagnostics; // Nodig voor debug logs
using System.Net.Http.Json;
using CineFlix_Models;

namespace CineFlix_Mobile.Services
{
    public class FilmsService
    {
        private readonly HttpClient _httpClient;

        // Jouw poortnummer 7191 staat hier nu goed
        private const string BaseUrl = "https://localhost:7191/api/films";

        public FilmsService()
        {
            // 1. Maak de handler aan die SSL certificaten negeert (voor Android emulator)
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            // 2. Maak de HttpClient aan MET die handler
            _httpClient = new HttpClient(handler);
        }

        public async Task<List<Film>> GetFilmsAsync()
        {
            try
            {
                string url = BaseUrl;

                // Check: Zitten we op Android? Dan localhost vervangen door 10.0.2.2
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    url = url.Replace("localhost", "10.0.2.2");
                }

                // Haal de data op
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // JSON omzetten naar lijst met films
                    return await response.Content.ReadFromJsonAsync<List<Film>>() ?? new List<Film>();
                }
                else
                {
                    // Foutmelding loggen als de statuscode niet 200 OK is
                    Debug.WriteLine($"API Fout: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Hier zien we wat er écht misgaat
                await Shell.Current.DisplayAlert("Fout!", ex.Message, "OK");
            }

            // Bij fouten: geef lege lijst terug
            return new List<Film>();
        }
    }
}