using CineFlix_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CineFlix_Web.Controllers
{
    public class FilmsController : Controller
    {
        private readonly CineFlixDbContext _context;

        public FilmsController(CineFlixDbContext context)
        {
            _context = context;
        }

        // GET: Films
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["RatingSortParm"] = sortOrder == "Rating" ? "rating_desc" : "Rating";

            // We gebruiken nu de helper methode, zodat we geen dubbele code hebben
            var films = await GetFilteredFilms(searchString, sortOrder);

            return View(films);
        }

        // NIEUW: AJAX Zoek Functie 🚀
        // Deze wordt aangeroepen door het Javascript in je Index.cshtml als je typt
        public async Task<IActionResult> AjaxSearch(string searchString)
        {
            // We geven "" mee als sortOrder om de warning over 'null' te voorkomen
            var films = await GetFilteredFilms(searchString, "");

            // We sturen alleen het tabel-gedeelte terug
            return PartialView("_FilmTableBody", films);
        }

        // HULP METHODE: Hier zit alle logica voor zoeken en sorteren
        private async Task<List<Film>> GetFilteredFilms(string searchString, string sortOrder)
        {
            var filmsQuery = _context.Films
                .Include(f => f.Regisseur)
                .Include(f => f.FilmGenres)
                .ThenInclude(fg => fg.Genre)
                .AsQueryable();

            // FILTEREN
            if (!string.IsNullOrEmpty(searchString))
            {
                filmsQuery = filmsQuery.Where(s => s.Titel.Contains(searchString));
            }

            // SORTEREN
            switch (sortOrder)
            {
                case "date_desc":
                    filmsQuery = filmsQuery.OrderByDescending(s => s.Releasejaar);
                    break;
                case "Rating":
                    filmsQuery = filmsQuery.OrderBy(s => s.Rating);
                    break;
                case "rating_desc":
                    filmsQuery = filmsQuery.OrderByDescending(s => s.Rating);
                    break;
                default:
                    filmsQuery = filmsQuery.OrderBy(s => s.Titel);
                    break;
            }

            return await filmsQuery.ToListAsync();
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.Regisseur)
                .Include(f => f.FilmGenres)
                .ThenInclude(fg => fg.Genre)
                .FirstOrDefaultAsync(m => m.FilmId == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // GET: Films/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["RegisseurId"] = new SelectList(_context.Regisseurs, "RegisseurId", "Naam");
            return View();
        }

        // POST: Films/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("FilmId,Titel,Releasejaar,DuurMinuten,Beschrijving,CoverAfbeelding,Rating,RegisseurId")] Film film)
        {
            if (ModelState.IsValid)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RegisseurId"] = new SelectList(_context.Regisseurs, "RegisseurId", "Naam", film.RegisseurId);
            return View(film);
        }

        // GET: Films/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            ViewData["RegisseurId"] = new SelectList(_context.Regisseurs, "RegisseurId", "Naam", film.RegisseurId);
            return View(film);
        }

        // POST: Films/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("FilmId,Titel,Releasejaar,DuurMinuten,Beschrijving,CoverAfbeelding,Rating,RegisseurId")] Film film)
        {
            if (id != film.FilmId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.FilmId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RegisseurId"] = new SelectList(_context.Regisseurs, "RegisseurId", "Naam", film.RegisseurId);
            return View(film);
        }

        // GET: Films/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.Regisseur)
                .FirstOrDefaultAsync(m => m.FilmId == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film != null)
            {
                _context.Films.Remove(film);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.FilmId == id);
        }
    }
}