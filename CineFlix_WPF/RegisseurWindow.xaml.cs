using CineFlix_Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace CineFlix_WPF
{
    public partial class RegisseurWindow : Window
    {
        private readonly CineFlixDbContext _context;
        private readonly Regisseur _regisseur;
        private readonly bool _isNew;

        public RegisseurWindow(CineFlixDbContext context, Regisseur? regisseur)
        {
            InitializeComponent();
            _context = context;

            if (regisseur == null)
            {
                // Dit is een nieuwe regisseur
                _regisseur = new Regisseur();
                _isNew = true;
                Title = "Nieuwe Regisseur Toevoegen";
            }
            else
            {
                // Dit is een bestaande regisseur. We laden hem opnieuw om zeker te zijn dat hij gevolgd wordt door de DbContext.
                _regisseur = _context.Regisseurs.Single(r => r.RegisseurId == regisseur.RegisseurId);
                _isNew = false;
                Title = "Regisseur Bewerken";
            }

            // Koppel de data aan het venster
            DataContext = _regisseur;
            Loaded += RegisseurWindow_Loaded;
        }

        private void RegisseurWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Vul de UI velden met de data van de regisseur
            NaamTextBox.Text = _regisseur.Naam;
            GeboortejaarTextBox.Text = _regisseur.Geboortejaar?.ToString();
            NationaliteitTextBox.Text = _regisseur.Nationaliteit;
            BiografieTextBox.Text = _regisseur.Biografie;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validatie
            if (string.IsNullOrWhiteSpace(NaamTextBox.Text))
            {
                MessageBox.Show("De naam van de regisseur is een verplicht veld.", "Validatiefout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update het object met de data uit de UI
            _regisseur.Naam = NaamTextBox.Text;
            _regisseur.Nationaliteit = NationaliteitTextBox.Text;
            _regisseur.Biografie = BiografieTextBox.Text;

            if (int.TryParse(GeboortejaarTextBox.Text, out int jaar))
            {
                _regisseur.Geboortejaar = jaar;
            }
            else
            {
                _regisseur.Geboortejaar = null; // Maak het veld leeg als de input ongeldig is
            }

            if (_isNew)
            {
                _context.Regisseurs.Add(_regisseur);
            }
            else
            {
                _context.Regisseurs.Update(_regisseur);
            }

            try
            {
                await _context.SaveChangesAsync();
                DialogResult = true; // Dit signaleert aan het hoofdvenster dat de opslagactie is gelukt.
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het opslaan van de regisseur: {ex.Message}", "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}