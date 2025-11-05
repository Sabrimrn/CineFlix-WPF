using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CineFlix_WPF
{
    /// <summary>
    /// Interaction logic for RatingControl.xaml
    /// </summary>
    public partial class RatingControl : UserControl
    {
        public RatingControl()
        {
            InitializeComponent();
        }

        public double Value { get; private set; }

        private void Star_Click(object sender, RoutedEventArgs e)
        {
            var clicked = sender as ToggleButton;
            if (clicked == null) return;

            int index = 0;
            if (clicked == Star1) index = 1;
            else if (clicked == Star2) index = 2;
            else if (clicked == Star3) index = 3;
            else if (clicked == Star4) index = 4;
            else if (clicked == Star5) index = 5;

            SetRating(index);
        }

        public void SetRating(int rating)
        {
            Star1.IsChecked = rating >= 1;
            Star2.IsChecked = rating >= 2;
            Star3.IsChecked = rating >= 3;
            Star4.IsChecked = rating >= 4;
            Star5.IsChecked = rating >= 5;
            Value = rating;
        }
    }
}
