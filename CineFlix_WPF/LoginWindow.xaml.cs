using CineFlix_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

                // Probeer in te loggen
                var result = await _signInManager.PasswordSignInAsync(user.UserName, PasswordBox.Password, false, lockoutOnFailure: true);

                if (result.Succeeded)
                {
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
                else if (result.IsLockedOut)
                {
                    ShowError("Account is tijdelijk geblokkeerd wegens te veel mislukte pogingen.");
                }
                else if (result.IsNotAllowed)
                {
                    ShowError("Account is niet geactiveerd.");
                }
                else
                {
                    ShowError("Ongeldige inloggegevens.");
                }
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
