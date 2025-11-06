using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CineFlix_WPF
{
    public class RoleItem : INotifyPropertyChanged
    {
        private string _name;
        private bool _isChecked;
        private bool _isEnabled = true;

        public string Name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public bool IsChecked {
            get => _isChecked;
            set {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    /// <summary>
    /// Interaction logic for RolesWindow.xaml
    /// </summary>
    public partial class RolesWindow : Window
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<CineFlix_Models.CineFlixUser> _userManager;
        private CineFlix_Models.CineFlixUser _user;

        public RolesWindow()
        {
            InitializeComponent();
            _roleManager = App.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = App.ServiceProvider.GetRequiredService<UserManager<CineFlix_Models.CineFlixUser>>();
        }

        public async Task SetUserAsync(CineFlix_Models.CineFlixUser user)
        {
            _user = user;
            await LoadRolesAsync();
        }

        private async Task LoadRolesAsync()
        {
            RolesListBox.Items.Clear();
            var roles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(_user);

            var list = new List<RoleItem>();
            foreach (var role in roles)
            {
                list.Add(new RoleItem { Name = role.Name ?? string.Empty, IsChecked = userRoles.Contains(role.Name) });
            }

            RolesListBox.ItemsSource = list;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var items = RolesListBox.ItemsSource as IEnumerable<RoleItem>;
                var selected = items.Where(i => i.IsChecked).Select(i => i.Name).ToList();

                var current = await _userManager.GetRolesAsync(_user);

                var toAdd = selected.Except(current).ToList();
                var toRemove = current.Except(selected).ToList();

                if (toAdd.Any())
                {
                    var res = await _userManager.AddToRolesAsync(_user, toAdd);
                    if (!res.Succeeded) throw new Exception(string.Join(';', res.Errors.Select(er => er.Description)));
                }

                if (toRemove.Any())
                {
                    var res = await _userManager.RemoveFromRolesAsync(_user, toRemove);
                    if (!res.Succeeded) throw new Exception(string.Join(';', res.Errors.Select(er => er.Description)));
                }

                MessageBox.Show("Rollen bijgewerkt.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void RolesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
