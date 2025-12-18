using Microsoft.AspNetCore.Identity.UI.Services;

namespace CineFlix_Web.Services
{
    // Deze klasse doet "net alsof" hij een mail stuurt.
    // Dit is nodig om de error weg te krijgen.
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Hier zou je echte mail-code zetten (bijv. SendGrid of SMTP).
            // Voor nu loggen we het alleen in de console van Visual Studio.
            Console.WriteLine($"--- FAKE EMAIL TO {email} ---");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine(htmlMessage);
            Console.WriteLine("-----------------------------");

            return Task.CompletedTask;
        }
    }
}
