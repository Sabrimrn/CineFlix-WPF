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
        // Initialiseer _name om waarschuwing CS8618 te voorkomen.
        private string _name = string.Empty;
        private bool _isChecked;
        private bool _isEnabled = true;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    public partial class RolesWindow : Window
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<CineFlix_Models.CineFlixUser> _userManager;

        // HIER IS DE VERANDERING:
        // We voegen een vraagteken (?) toe om aan te geven dat _user null mag zijn.
        private CineFlix_Models.CineFlixUser? _user;

        public RolesWindow()
        {
            InitializeComponent();
            // We halen de services op via de App ServiceProvider
            _roleManager = App.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = App.ServiceProvider.GetRequiredService<UserManager<CineFlix_Models.CineFlixUser>>();
        }

        // Deze methode wordt aangeroepen vanuit het hoofdvenster om de gebruiker in te stellen.
        public async Task SetUserAsync(CineFlix_Models.CineFlixUser user)
        {
            _user = user;
            await LoadRolesAsync();
        }

        private async Task LoadRolesAsync()
        {
            if (_user == null) return;

            RolesListBox.Items.Clear();
            var roles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(_user);

            var list = new List<RoleItem>();
            foreach (var role in roles)
            {
                // Controleer of de rolnaam niet null is
                if (role.Name != null)
                {
                    // Omdat we nu in de if-statement zitten, weet de compiler dat role.Name niet null is.
                    list.Add(new RoleItem { Name = role.Name, IsChecked = userRoles.Contains(role.Name) });
                }
            }

            RolesListBox.ItemsSource = list;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Voeg een controle toe om een fout te voorkomen.
            if (_user == null || RolesListBox.ItemsSource == null) return;

            try
            {
                var items = RolesListBox.ItemsSource as IEnumerable<RoleItem>;
                // Controleer of 'items' niet null is
                if (items == null) return;

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
            // Deze methode is niet nodig, je kunt hem leeg laten.
        }
    }
}