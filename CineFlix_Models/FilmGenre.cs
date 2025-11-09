using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineFlix_Models
{
    public class FilmGenre
    {
        [Key]
        public int FilmGenreId { get; set; }

        [Required]
        public int FilmId { get; set; }

        [Required]
        public int GenreId { get; set; }

        // Navigatie properties
        [ForeignKey("FilmId")]
        public virtual Film Film { get; set; } = null!;

        [ForeignKey("GenreId")]
        public virtual Genre Genre { get; set; } = null!;

        // Extra property voor sortering
        public int SortOrder { get; set; } = 0;

        // Soft delete 
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }

        // Seeding data
        public static List<FilmGenre> SeedingData()
        {
            return new List<FilmGenre>
            {
                // The Shawshank Redemption - Drama
                new FilmGenre { FilmId = 1, GenreId = 2, SortOrder = 1 },
                
                // The Godfather - Drama, Crime/Thriller
                new FilmGenre { FilmId = 2, GenreId = 2, SortOrder = 1 },
                new FilmGenre { FilmId = 2, GenreId = 4, SortOrder = 2 },
                
                // The Dark Knight - Actie, Thriller
                new FilmGenre { FilmId = 3, GenreId = 1, SortOrder = 1 },
                new FilmGenre { FilmId = 3, GenreId = 4, SortOrder = 2 },
                
                // Pulp Fiction - Thriller, Drama
                new FilmGenre { FilmId = 4, GenreId = 4, SortOrder = 1 },
                new FilmGenre { FilmId = 4, GenreId = 2, SortOrder = 2 },
                
                // Dummy Film - Dummy Genre
                new FilmGenre { FilmId = 5, GenreId = 10, SortOrder = 1 }
            };
        }
    }
}