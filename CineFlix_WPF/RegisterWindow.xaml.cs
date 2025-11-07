using CineFlix_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;

namespace CineFlix_WPF
{
    public partial class RegisterWindow : Window
    {
        private readonly UserManager<CineFlixUser> _userManager;

        public RegisterWindow(UserManager<CineFlixUser> userManager)
        {
            InitializeComponent();
            _userManager = userManager;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ErrorMessage.Text = "De wachtwoorden komen niet overeen.";
                return;
            }

            var user = new CineFlixUser
            {
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Email = EmailTextBox.Text,
                UserName = EmailTextBox.Text, // Gebruik e-mail als gebruikersnaam
                RegistrationDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, PasswordBox.Password);

            if (result.Succeeded)
            {
                // Voeg de nieuwe gebruiker standaard toe aan de 'User' rol
                await _userManager.AddToRoleAsync(user, "User");
                MessageBox.Show("Registratie succesvol! Je kunt nu inloggen.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                // Ga terug naar het login venster
                var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                ErrorMessage.Text = string.Join("\n", result.Errors.Select(e => e.Description));
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
            this.Close();
        }
    }
}