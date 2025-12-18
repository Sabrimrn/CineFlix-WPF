using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineFlix_Models;

namespace CineFlix_Web.Controllers
{
    // Dit is het adres van je API: https://jouwwebsite/api/films
    [Route("api/films")]
    [ApiController]
    public class FilmsApiController : ControllerBase
    {
        private readonly CineFlixDbContext _context;

        public FilmsApiController(CineFlixDbContext context)
        {
            _context = context;
        }

        // GET: api/films
        // Deze wordt aangeroepen door je mobiele app om alle films op te halen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Film>>> GetFilms()
        {
            // We halen films op INCLUSIEF de regisseur, anders is dat veld leeg in de app
            return await _context.Films
                .Include(f => f.Regisseur)
                .ToListAsync();
        }

        // GET: api/films/5
        // Voor de details van 1 film
        [HttpGet("{id}")]
        public async Task<ActionResult<Film>> GetFilm(int id)
        {
            var film = await _context.Films
                .Include(f => f.Regisseur)
                .FirstOrDefaultAsync(f => f.FilmId == id);

            if (film == null)
            {
                return NotFound();
            }

            return film;
        }
    }
}
