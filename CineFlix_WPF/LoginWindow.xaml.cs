using CineFlix_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace CineFlix_WPF
{
    public partial class LoginWindow : Window
    {
        // We halen de UserManager op om gebruikers te beheren en wachtwoorden te controleren.
        private readonly UserManager<CineFlixUser> _userManager;

        public LoginWindow()
        {
            InitializeComponent();

            // Haal de UserManager op uit de ServiceProvider die ik in App.xaml.cs hebben gemaakt.
            _userManager = App.ServiceProvider.GetRequiredService<UserManager<CineFlixUser>>();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vul alstublieft zowel gebruikersnaam als wachtwoord in.", "Invoerfout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Zoek de gebruiker op basis van de ingevoerde gebruikersnaam (e-mail).
            var user = await _userManager.FindByEmailAsync(username);

            // Controleer of de gebruiker bestaat en of het wachtwoord correct is.
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                // De login is succesvol

                // 1. Stel de ingelogde gebruiker in voor de hele applicatie via App.xaml.cs
                await App.LoginAsync(user);

                // 2. Haal een nieuwe MainWindow op uit de ServiceProvider.
                var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();

                // 3. Toon de MainWindow.
                mainWindow.Show();

                // 4. Sluit dit login-venster.
                this.Close();
            }
            else
            {
                // De login is mislukt. Toon een foutmelding.
                MessageBox.Show("De combinatie van gebruikersnaam en wachtwoord is onjuist.", "Login Mislukt", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Implementeer logica om het registratievenster te openen.
            MessageBox.Show("Registratie-functionaliteit is nog niet geïmplementeerd.", "In Ontwikkeling");
        }
    }
}