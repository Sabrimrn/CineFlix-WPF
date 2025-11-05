using CineFlix_Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace CineFlix_WPF
{
    /// <summary>
    /// Interaction logic for RegisseurWindow.xaml
    /// </summary>
    public partial class RegisseurWindow : Window
    {
        private readonly CineFlixDbContext _context;
        private Regisseur _regisseur;

        public RegisseurWindow()
        {
            InitializeComponent();
            _context = App.ServiceProvider.GetRequiredService<CineFlixDbContext>();
        }

        public void SetRegisseur(Regisseur regisseur)
        {
            _regisseur = regisseur;
            if (_regisseur != null)
            {
                NaamTextBox.Text = _regisseur.Naam;
                GeboortejaarTextBox.Text = _regisseur.Geboortejaar?.ToString() ?? string.Empty;
                NationaliteitTextBox.Text = _regisseur.Nationaliteit ?? string.Empty;
                BiografieTextBox.Text = _regisseur.Biografie ?? string.Empty;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NaamTextBox.Text))
                {
                    MessageBox.Show("Naam is verplicht.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_regisseur == null)
                {
                    _regisseur = new Regisseur();
                    _context.Regisseurs.Add(_regisseur);
                }

                _regisseur.Naam = NaamTextBox.Text.Trim();

                if (int.TryParse(GeboortejaarTextBox.Text, out int jaar))
                {
                    _regisseur.Geboortejaar = jaar;
                }
                else
                {
                    _regisseur.Geboortejaar = null;
                }

                _regisseur.Nationaliteit = string.IsNullOrWhiteSpace(NationaliteitTextBox.Text) ? null : NationaliteitTextBox.Text.Trim();
                _regisseur.Biografie = string.IsNullOrWhiteSpace(BiografieTextBox.Text) ? null : BiografieTextBox.Text.Trim();

                await _context.SaveChangesAsync();
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
    }
}
