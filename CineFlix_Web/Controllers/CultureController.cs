using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace CineFlix_Web.Controllers
{
    public class CultureController : Controller
    {
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            // Sla de gekozen taal op in een cookie
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            // Keer terug naar de pagina waar de gebruiker was
            return LocalRedirect(returnUrl);
        }
    }
}
