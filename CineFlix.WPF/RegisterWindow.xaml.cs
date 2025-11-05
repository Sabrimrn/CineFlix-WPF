using CineFlix.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Windows;

namespace CineFlix.WPF
{
    public partial class RegisterWindow : Window
    {
        private readonly UserManager<CineFlixUser> _userManager;
        public RegisterWindow(UserManager<CineFlixUser> userManager)
        {
            InitializeComponent();
            _userManager = userManager;
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = new CineFlixUser
                {
                    UserName = Email.Text.Trim(),
                    Email = Email.Text.Trim(),
                    FirstName = FirstName.Text.Trim(),
                    LastName = LastName.Text.Trim()
                };
                var res = await _userManager.CreateAsync(user, Password.Password);
                if (res.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    MessageBox.Show("Registered");
                    DialogResult = true;
                }
                else MessageBox.Show(string.Join("\n", res.Errors));
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
        }

        private void Close_Click(object s, RoutedEventArgs e) => Close();
    }
}