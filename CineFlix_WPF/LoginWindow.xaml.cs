using CineFlix_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;

namespace CineFlix_WPF
{
    public partial class LoginWindow : Window
    {
        private readonly UserManager<CineFlixUser> _userManager;

        // Constructor wordt aangeroepen door Dependency Injection
        public LoginWindow(UserManager<CineFlixUser> userManager)
        {
            InitializeComponent();
            _userManager = userManager;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            var user = await _userManager.FindByEmailAsync(EmailTextBox.Text);

            if (user != null && await _userManager.CheckPasswordAsync(user, PasswordBox.Password))
            {
                if (user.IsDeleted)
                {
                    ErrorMessage.Text = "Dit account is geblokkeerd.";
                    return;
                }

                // Sla de gebruiker en zijn rollen op in de App
                App.CurrentUser = user;
                App.CurrentUserRoles = (await _userManager.GetRolesAsync(user)).ToList();

                // Open het hoofdvenster
                var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ErrorMessage.Text = "Ongeldige e-mail of wachtwoord.";
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = App.ServiceProvider.GetRequiredService<RegisterWindow>();
            registerWindow.Show();
            this.Close();
        }
    }
}