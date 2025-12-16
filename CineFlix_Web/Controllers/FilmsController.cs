using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CineFlix_Models;

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
        public async Task<IActionResult> Index()
        {
            var cineFlixDbContext = _context.Films.Include(f => f.AddedByUser).Include(f => f.Regisseur);
            return View(await cineFlixDbContext.ToListAsync());
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.AddedByUser)
                .Include(f => f.Regisseur)
                .FirstOrDefaultAsync(m => m.FilmId == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // GET: Films/Create
        public IActionResult Create()
        {
            ViewData["AddedByUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["RegisseurId"] = new SelectList(_context.Regisseurs, "RegisseurId", "Naam");
            return View();
        }

        // POST: Films/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FilmId,Titel,Releasejaar,DuurMinuten,Beschrijving,CoverAfbeelding,Rating,RegisseurId,AddedByUserId,IsDeleted,DeletedOn")] Film film)
        {
            if (ModelState.IsValid)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddedByUserId"] = new SelectList(_context.Users, "Id", "Id", film.AddedByUserId);
            ViewData["RegisseurId"] = new SelectList(_context.Regisseurs, "RegisseurId", "Naam", film.RegisseurId);
            return View(film);
        }

        // GET: Films/Edit/5
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
            ViewData["AddedByUserId"] = new SelectList(_context.Users, "Id", "Id", film.AddedByUserId);
            ViewData["RegisseurId"] = new SelectList(_context.Regisseurs, "RegisseurId", "Naam", film.RegisseurId);
            return View(film);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FilmId,Titel,Releasejaar,DuurMinuten,Beschrijving,CoverAfbeelding,Rating,RegisseurId,AddedByUserId,IsDeleted,DeletedOn")] Film film)
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
            ViewData["AddedByUserId"] = new SelectList(_context.Users, "Id", "Id", film.AddedByUserId);
            ViewData["RegisseurId"] = new SelectList(_context.Regisseurs, "RegisseurId", "Naam", film.RegisseurId);
            return View(film);
        }

        // GET: Films/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.AddedByUser)
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
