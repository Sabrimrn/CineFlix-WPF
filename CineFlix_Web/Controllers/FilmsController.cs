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
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? genreId)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentGenre"] = genreId; // Nieuw: Onthoud welk genre is gekozen
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["RatingSortParm"] = sortOrder == "Rating" ? "rating_desc" : "Rating";

            // Vul de dropdown met genres
            ViewData["Genres"] = new SelectList(_context.Genres, "GenreId", "Naam");

            // Alles selecteren
            var filmsQuery = _context.Films
                .Include(f => f.Regisseur)
                .Include(f => f.FilmGenres) // Zorg dat we bij de genres kunnen
                .ThenInclude(fg => fg.Genre)
                .AsQueryable();

            // FILTEREN OP TEKST
            if (!string.IsNullOrEmpty(searchString))
            {
                filmsQuery = filmsQuery.Where(s => s.Titel.Contains(searchString));
            }

            // NIEUW: FILTEREN OP GENRE
            if (genreId.HasValue)
            {
                filmsQuery = filmsQuery.Where(x => x.FilmGenres.Any(fg => fg.GenreId == genreId));
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
                default: // Standaard
                    filmsQuery = filmsQuery.OrderBy(s => s.Titel);
                    break;
            }

            return View(await filmsQuery.ToListAsync());
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
