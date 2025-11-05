using CineFlix.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CineFlix.WPF
{
    public partial class LoginWindow : Window
    {
        private readonly SignInManager<CineFlixUser> _signIn;
        private readonly UserManager<CineFlixUser> _userManager;
        private readonly CineFlixDbContext _db;

        public LoginWindow(SignInManager<CineFlixUser> signIn, UserManager<CineFlixUser> userManager, CineFlixDbContext db)
        {
            InitializeComponent();
            _signIn = signIn;
            _userManager = userManager;
            _db = db;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var email = EmailBox.Text.Trim();
                var pw = PasswordBox.Password;
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) { MessageBox.Show("User not found"); return; }

                var res = await _signIn.PasswordSignInAsync(user, pw, isPersistent: false, lockoutOnFailure: false);
                if (res.Succeeded)
                {
                    // set roles in App (simple)
                    var roles = await _userManager.GetRolesAsync(user);
                    App.Services.GetRequiredService<MainWindow>(); // ensure registered
                    var main = App.Services.GetRequiredService<MainWindow>();
                    main.Show();
                    this.Close();
                }
                else MessageBox.Show("Login failed");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}");
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var reg = App.Services.GetRequiredService<RegisterWindow>();
            reg.ShowDialog();
        }
    }
}