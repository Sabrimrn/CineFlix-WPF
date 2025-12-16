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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Regisseurs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegisseurId,Naam,Geboortejaar,Nationaliteit,Biografie,IsDeleted,DeletedOn")] Regisseur regisseur)
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegisseurId,Naam,Geboortejaar,Nationaliteit,Biografie,IsDeleted,DeletedOn")] Regisseur regisseur)
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
