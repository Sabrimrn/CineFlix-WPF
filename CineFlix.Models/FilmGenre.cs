using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineFlix.Models
{
    public class FilmGenre
    {
        [Key]
        public int FilmGenreId { get; set; }
        public int FilmId { get; set; }
        public int GenreId { get; set; }
        [ForeignKey(nameof(FilmId))]
        public virtual Film? Film { get; set; }
        [ForeignKey(nameof(GenreId))]
        public virtual Genre? Genre { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }
        public static System.Collections.Generic.List<FilmGenre> Seed() => new()
        {
            new FilmGenre { FilmGenreId = 1, FilmId = 1, GenreId = 1 }
        };
    }
}