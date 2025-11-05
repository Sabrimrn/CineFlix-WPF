using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CineFlix.Models
{
    public class Regisseur
    {
        [Key]
        public int RegisseurId { get; set; }
        [Required]
        public string Naam { get; set; } = string.Empty;
        public int? Geboortejaar { get; set; }
        public string? Nationaliteit { get; set; }
        public string? Biografie { get; set; }
        public virtual ICollection<Film> Films { get; set; } = new List<Film>();
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }

        public string DisplayNaam => Geboortejaar.HasValue ? $"{Naam} ({Geboortejaar})" : Naam;

        public static List<Regisseur> Seed() => new()
        {
            new Regisseur { RegisseurId = 1, Naam = "Dummy Regisseur", Geboortejaar = 1970 }
        };
    }
}