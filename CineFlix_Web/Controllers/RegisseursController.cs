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
    public class RegisseursController : Controller
    {
        private readonly CineFlixDbContext _context;

        public RegisseursController(CineFlixDbContext context)
        {
            _context = context;
        }

        // GET: Regisseurs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Regisseurs.ToListAsync());
        }

        // GET: Regisseurs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regisseur = await _context.Regisseurs
                .FirstOrDefaultAsync(m => m.RegisseurId == id);
            if (regisseur == null)
            {
                return NotFound();
            }

            return View(regisseur);
        }

        // GET: Regisseurs/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Regisseurs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("RegisseurId,Naam,Geboortejaar,Nationaliteit,Biografie")] Regisseur regisseur)
        {
            if (ModelState.IsValid)
            {
                _context.Add(regisseur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(regisseur);
        }

        // GET: Regisseurs/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regisseur = await _context.Regisseurs.FindAsync(id);
            if (regisseur == null)
            {
                return NotFound();
            }
            return View(regisseur);
        }

        // POST: Regisseurs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("RegisseurId,Naam,Geboortejaar,Nationaliteit,Biografie")] Regisseur regisseur)
        {
            if (id != regisseur.RegisseurId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(regisseur);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegisseurExists(regisseur.RegisseurId))
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
            return View(regisseur);
        }

        // GET: Regisseurs/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regisseur = await _context.Regisseurs
                .FirstOrDefaultAsync(m => m.RegisseurId == id);
            if (regisseur == null)
            {
                return NotFound();
            }

            return View(regisseur);
        }

        // POST: Regisseurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var regisseur = await _context.Regisseurs.FindAsync(id);
            if (regisseur != null)
            {
                _context.Regisseurs.Remove(regisseur);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegisseurExists(int id)
        {
            return _context.Regisseurs.Any(e => e.RegisseurId == id);
        }
    }
}
