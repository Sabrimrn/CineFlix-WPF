using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CineFlix.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }
        [Required]
        public string GenreNaam { get; set; } = string.Empty;
        public string? Beschrijving { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }
        public virtual ICollection<FilmGenre> FilmGenres { get; set; } = new List<FilmGenre>();

        public static List<Genre> Seed() => new()
        {
            new Genre { GenreId = 1, GenreNaam = "-" },
            new Genre { GenreId = 2, GenreNaam = "Drama" }
        };
    }
}