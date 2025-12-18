using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineFlix_Models;

namespace CineFlix_Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GebruikersController : Controller
    {
        private readonly UserManager<CineFlixUser> _userManager;

        public GebruikersController(UserManager<CineFlixUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Gebruikers
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<GebruikerViewModel>();

            foreach (var user in users)
            {
                var thisViewModel = new GebruikerViewModel();
                thisViewModel.UserId = user.Id;
                thisViewModel.Email = user.Email;


                thisViewModel.Naam = user.UserName;

                thisViewModel.IsGeblokkeerd = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now;

                // Haal de rollen op
                thisViewModel.Rollen = await _userManager.GetRolesAsync(user);

                userViewModels.Add(thisViewModel);
            }

            return View(userViewModels);
        }

        public async Task<IActionResult> ToggleAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleBlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now)
            {
                user.LockoutEnd = null;
            }
            else
            {
                user.LockoutEnd = DateTimeOffset.Now.AddYears(100);
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
    public class GebruikerViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Naam { get; set; } 
        public IList<string> Rollen { get; set; }
        public bool IsGeblokkeerd { get; set; } 
    }
}