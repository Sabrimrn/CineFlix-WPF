using System.Windows.Controls;

namespace CineFlix_WPF
{
    public partial class HeaderControl : UserControl
    {
        public HeaderControl()
        {
            InitializeComponent();

            // Controleer of een gebruiker is ingelogd
            if (App.CurrentUser != null)
            {
                LoggedInUserTextBlock.Text = App.CurrentUser.Email;
            }
            else
            {
                LoggedInUserTextBlock.Text = "Niet ingelogd";
            }
        }
    }
}