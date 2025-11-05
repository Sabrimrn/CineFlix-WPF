using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineFlix_Models
{
    public class Film
    {
        [Key]
        public int FilmId { get; set; }

        [Required(ErrorMessage = "Titel is verplicht")]
        [StringLength(200, ErrorMessage = "Titel mag maximaal 200 karakters zijn")]
        [Display(Name = "Titel")]
        public string Titel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Releasejaar is verplicht")]
        [Display(Name = "Releasejaar")]
        [Range(1895, 2050, ErrorMessage = "Releasejaar moet tussen 1895 en 2050 liggen")]
        public int Releasejaar { get; set; }

        [Display(Name = "Duur (minuten)")]
        [Range(1, 600, ErrorMessage = "Duur moet tussen 1 en 600 minuten liggen")]
        public int DuurMinuten { get; set; }

        [Display(Name = "Beschrijving")]
        [StringLength(2000)]
        public string? Beschrijving { get; set; }

        [Display(Name = "Cover Afbeelding")]
        [StringLength(500)]
        public string? CoverAfbeelding { get; set; }

        [Display(Name = "Rating")]
        [Range(0, 5, ErrorMessage = "Rating moet tussen 0 en 5 sterren zijn")]
        public double Rating { get; set; }

        // Foreign Keys
        [Display(Name = "Regisseur")]
        public int? RegisseurId { get; set; }

        [Display(Name = "Toegevoegd door")]
        public string? AddedByUserId { get; set; }

        // Navigatie properties
        [ForeignKey("RegisseurId")]
        public virtual Regisseur? Regisseur { get; set; }

        [ForeignKey("AddedByUserId")]
        public virtual CineFlixUser? AddedByUser { get; set; }

        public virtual ICollection<FilmGenre> FilmGenres { get; set; } = new List<FilmGenre>();

        // Soft delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }

        // Extra properties voor weergave
        [NotMapped]
        public string DisplayTitel => $"{Titel} ({Releasejaar})";

        [NotMapped]
        public string DuurFormatted => $"{DuurMinuten / 60}u {DuurMinuten % 60}min";

        // Static dummy voor seeding
        public static Film? Dummy { get; set; }

        // Seeding data
        public static List<Film> SeedingData()
        {
            return new List<Film>
            {
                new Film
                {
                    Titel = "The Shawshank Redemption",
                    Releasejaar = 1994,
                    DuurMinuten = 142,
                    Beschrijving = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                    Rating = 4.9,
                    RegisseurId = 1
                },
                new Film
                {
                    Titel = "The Godfather",
                    Releasejaar = 1972,
                    DuurMinuten = 175,
                    Beschrijving = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.",
                    Rating = 4.8,
                    RegisseurId = 2
                },
                new Film
                {
                    Titel = "The Dark Knight",
                    Releasejaar = 2008,
                    DuurMinuten = 152,
                    Beschrijving = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests.",
                    Rating = 4.7,
                    RegisseurId = 3
                },
                new Film
                {
                    Titel = "Pulp Fiction",
                    Releasejaar = 1994,
                    DuurMinuten = 154,
                    Beschrijving = "The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.",
                    Rating = 4.6,
                    RegisseurId = 4
                },
                new Film
                {
                    Titel = "Dummy Film",
                    Releasejaar = 2020,
                    DuurMinuten = 90,
                    Beschrijving = "Dit is een dummy film voor testing",
                    Rating = 0,
                    RegisseurId = 1
                }
            };
        }
    }
}