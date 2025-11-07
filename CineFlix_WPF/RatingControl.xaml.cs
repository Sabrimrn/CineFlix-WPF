using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CineFlix_WPF
{
    public partial class RatingControl : UserControl
    {
        // Dit is een Dependency Property. Het is de standaard manier in WPF om eigenschappen
        // te maken die je vanuit andere vensters wilt kunnen aanpassen.
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                name: "Value",
                propertyType: typeof(double),
                ownerType: typeof(RatingControl),
                new FrameworkPropertyMetadata(0.0, OnValueChanged));

        // Deze 'Value' property is nu een "wrapper" voor de Dependency Property.
        // De get en set zijn nu publiek.
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public RatingControl()
        {
            InitializeComponent();
        }

        // Deze methode wordt automatisch aangeroepen als de 'Value' verandert.
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RatingControl control)
            {
                // Update de sterren op basis van de nieuwe waarde.
                control.UpdateStars((double)e.NewValue);
            }
        }

        private void Star_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button && button.Tag != null)
            {
                int rating = int.Parse(button.Tag.ToString() ?? "0");
                // Als je op een ster klikt, passen we de 'Value' aan.
                // Dit zorgt ervoor dat OnValueChanged wordt aangeroepen en de sterren geüpdatet worden.
                this.Value = rating;
            }
        }

        // Deze methode vult de sterren visueel.
        private void UpdateStars(double rating)
        {
            Star1.IsChecked = rating >= 1;
            Star2.IsChecked = rating >= 2;
            Star3.IsChecked = rating >= 3;
            Star4.IsChecked = rating >= 4;
            Star5.IsChecked = rating >= 5;
        }

        // We hebben SetRating() niet meer nodig, omdat we direct de Value property kunnen gebruiken.
        // public void SetRating(int rating) { ... }
    }
}