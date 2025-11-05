using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineFlix_Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Genre naam is verplicht")]
        [StringLength(50, ErrorMessage = "Genre naam mag maximaal 50 karakters zijn")]
        [Display(Name = "Genre")]
        public string GenreNaam { get; set; } = string.Empty;

        [Display(Name = "Beschrijving")]
        [StringLength(500)]
        public string? Beschrijving { get; set; }

        [Display(Name = "Kleur")]
        [StringLength(7)] // Voor hex color codes zoals #FF0000
        public string? Kleur { get; set; }

        // Navigatie property
        public virtual ICollection<FilmGenre> FilmGenres { get; set; } = new List<FilmGenre>();

        // Soft delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }

        // Extra display property
        [NotMapped]
        public int AantalFilms => FilmGenres?.Count(fg => !fg.Film.IsDeleted) ?? 0;

        // Static dummy voor seeding
        public static Genre? Dummy { get; set; }

        // Seeding data
        public static List<Genre> SeedingData()
        {
            return new List<Genre>
            {
                new Genre
                {
                    GenreId = 1,
                    GenreNaam = "Actie",
                    Beschrijving = "Films met veel actie, gevechten en stunts",
                    Kleur = "#FF0000"
                },
                new Genre
                {
                    GenreId = 2,
                    GenreNaam = "Drama",
                    Beschrijving = "Serieuze films met nadruk op karakterontwikkeling",
                    Kleur = "#0000FF"
                },
                new Genre
                {
                    GenreId = 3,
                    GenreNaam = "Komedie",
                    Beschrijving = "Films bedoeld om te lachen",
                    Kleur = "#FFFF00"
                },
                new Genre
                {
                    GenreId = 4,
                    GenreNaam = "Thriller",
                    Beschrijving = "Spannende films met suspense",
                    Kleur = "#800080"
                },
                new Genre
                {
                    GenreId = 5,
                    GenreNaam = "Science Fiction",
                    Beschrijving = "Films met futuristische of wetenschappelijke thema's",
                    Kleur = "#00FF00"
                },
                new Genre
                {
                    GenreId = 6,
                    GenreNaam = "Horror",
                    Beschrijving = "Enge films bedoeld om angst op te wekken",
                    Kleur = "#8B0000"
                },
                new Genre
                {
                    GenreId = 7,
                    GenreNaam = "Romantiek",
                    Beschrijving = "Films over liefde en relaties",
                    Kleur = "#FFC0CB"
                },
                new Genre
                {
                    GenreId = 8,
                    GenreNaam = "Animatie",
                    Beschrijving = "Geanimeerde films",
                    Kleur = "#FFA500"
                },
                new Genre
                {
                    GenreId = 9,
                    GenreNaam = "Documentaire",
                    Beschrijving = "Non-fictie films over werkelijke onderwerpen",
                    Kleur = "#808080"
                },
                new Genre
                {
                    GenreId = 10,
                    GenreNaam = "-",
                    Beschrijving = "Dummy genre voor testing",
                    Kleur = "#FFFFFF"
                }
            };
        }
    }
}