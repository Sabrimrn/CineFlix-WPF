using CineFlix_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CineFlix_WPF
{
    public partial class LoginWindow : Window
    {
        private readonly UserManager<CineFlixUser> _userManager;
        private readonly SignInManager<CineFlixUser> _signInManager;

        public LoginWindow()
        {
            InitializeComponent();
            _userManager = App.ServiceProvider.GetRequiredService<UserManager<CineFlixUser>>();
            _signInManager = App.ServiceProvider.GetRequiredService<SignInManager<CineFlixUser>>();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ErrorMessage.Visibility = Visibility.Collapsed;

                // Validatie
                if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    ShowError("Vul alle velden in.");
                    return;
                }

                // Zoek gebruiker
                var user = await _userManager.FindByEmailAsync(EmailTextBox.Text);
                if (user == null)
                {
                    ShowError("Ongeldige inloggegevens.");
                    return;
                }

                // Check of account niet geblokkeerd is
                if (user.IsDeleted)
                {
                    ShowError("Dit account is geblokkeerd.");
                    return;
                }

                // Use UserManager to validate password and handle lockout without HttpContext
                // Controleer of gebruiker al gelocked is
                if (await _userManager.IsLockedOutAsync(user))
                {
                    ShowError("Account is tijdelijk geblokkeerd wegens te veel mislukte pogingen.");
                    return;
                }

                // Controleer wachtwoord
                bool passwordValid = await _userManager.CheckPasswordAsync(user, PasswordBox.Password);

                if (!passwordValid)
                {
                    // Verhoog failed count en controleer lockout
                    await _userManager.AccessFailedAsync(user);
                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        ShowError("Account is tijdelijk geblokkeerd wegens te veel mislukte pogingen.");
                    }
                    else
                    {
                        ShowError("Ongeldige inloggegevens.");
                    }

                    return;
                }

                // Succesvolle login: reset failed count
                await _userManager.ResetAccessFailedCountAsync(user);

                // Update laatste login datum
                user.LastLoginDate = DateTime.Now;
                await _userManager.UpdateAsync(user);

                // Haal rollen op
                var roles = await _userManager.GetRolesAsync(user);

                // Sla gebruiker en rollen op in App
                App.CurrentUser = user;
                App.CurrentUserRoles = roles.ToList();

                // Open hoofdvenster
                var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Er is een fout opgetreden: {ex.Message}");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = App.ServiceProvider.GetRequiredService<RegisterWindow>();
            registerWindow.Show();
            this.Close();
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}
