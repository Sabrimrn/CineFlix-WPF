using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineFlix.Models
{
    public class Film
    {
        [Key]
        public int FilmId { get; set; }
        [Required]
        public string Titel { get; set; } = string.Empty;
        public int Releasejaar { get; set; }
        public int DuurMinuten { get; set; }
        public string? Beschrijving { get; set; }
        public string? CoverAfbeelding { get; set; }
        public double Rating { get; set; }
        public int? RegisseurId { get; set; }
        public string? AddedByUserId { get; set; }
        public virtual Regisseur? Regisseur { get; set; }
        public virtual CineFlixUser? AddedByUser { get; set; }
        public virtual ICollection<FilmGenre> FilmGenres { get; set; } = new List<FilmGenre>();
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }

        [NotMapped]
        public string DisplayTitel => $"{Titel} ({Releasejaar})";

        public static List<Film> Seed() => new()
        {
            new Film { FilmId = 1, Titel = "Dummy Film", Releasejaar = 2020, DuurMinuten = 90, Beschrijving = "Dummy", Rating = 0, RegisseurId = 1 }
        };
    }
}