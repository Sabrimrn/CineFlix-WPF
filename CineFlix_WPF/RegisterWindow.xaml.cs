using CineFlix_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CineFlix_WPF
{
    public partial class RegisterWindow : Window
    {
        private readonly UserManager<CineFlixUser> _userManager;

        public RegisterWindow()
        {
            InitializeComponent();
            _userManager = App.ServiceProvider.GetRequiredService<UserManager<CineFlixUser>>();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var emailBox = this.FindName("Email") as TextBox;
                var firstBox = this.FindName("FirstName") as TextBox;
                var lastBox = this.FindName("LastName") as TextBox;
                var passwordBox = this.FindName("Password") as PasswordBox;

                var emailText = emailBox?.Text?.Trim() ?? string.Empty;
                var firstText = firstBox?.Text?.Trim() ?? string.Empty;
                var lastText = lastBox?.Text?.Trim() ?? string.Empty;
                var password = passwordBox?.Password ?? string.Empty;

                var user = new CineFlixUser
                {
                    UserName = emailText,
                    Email = emailText,
                    FirstName = firstText,
                    LastName = lastText
                };

                var res = await _userManager.CreateAsync(user, password);
                if (res.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    MessageBox.Show("Registered successfully", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show(string.Join("\n", res.Errors));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
