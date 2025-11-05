using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineFlix_Models
{
    public class Regisseur
    {
        [Key]
        public int RegisseurId { get; set; }

        [Required(ErrorMessage = "Naam is verplicht")]
        [StringLength(100, ErrorMessage = "Naam mag maximaal 100 karakters zijn")]
        [Display(Name = "Naam")]
        public string Naam { get; set; } = string.Empty;

        [Display(Name = "Geboortejaar")]
        [Range(1850, 2020, ErrorMessage = "Geboortejaar moet tussen 1850 en 2020 liggen")]
        public int? Geboortejaar { get; set; }

        [StringLength(100)]
        [Display(Name = "Nationaliteit")]
        public string? Nationaliteit { get; set; }

        [Display(Name = "Biografie")]
        [StringLength(2000)]
        public string? Biografie { get; set; }

        // Navigatie property
        public virtual ICollection<Film> Films { get; set; } = new List<Film>();

        // Soft delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }

        // Extra display property
        [NotMapped]
        public string DisplayNaam
        {
            get
            {
                if (Geboortejaar.HasValue)
                    return $"{Naam} ({Geboortejaar})";
                return Naam;
            }
        }

        [NotMapped]
        public int AantalFilms => Films?.Count(f => !f.IsDeleted) ?? 0;

        // Static dummy voor seeding
        public static Regisseur? Dummy { get; set; }

        // Seeding data
        public static List<Regisseur> SeedingData()
        {
            return new List<Regisseur>
            {
                new Regisseur
                {
                    RegisseurId = 1,
                    Naam = "Frank Darabont",
                    Geboortejaar = 1959,
                    Nationaliteit = "Amerikaans",
                    Biografie = "Frank Darabont is een Amerikaanse filmmaker, scenarioschrijver en producent."
                },
                new Regisseur
                {
                    RegisseurId = 2,
                    Naam = "Francis Ford Coppola",
                    Geboortejaar = 1939,
                    Nationaliteit = "Amerikaans",
                    Biografie = "Francis Ford Coppola is een Amerikaanse filmregisseur, producent en scenarioschrijver."
                },
                new Regisseur
                {
                    RegisseurId = 3,
                    Naam = "Christopher Nolan",
                    Geboortejaar = 1970,
                    Nationaliteit = "Brits-Amerikaans",
                    Biografie = "Christopher Nolan is een Brits-Amerikaanse filmregisseur, scenarioschrijver en producent."
                },
                new Regisseur
                {
                    RegisseurId = 4,
                    Naam = "Quentin Tarantino",
                    Geboortejaar = 1963,
                    Nationaliteit = "Amerikaans",
                    Biografie = "Quentin Tarantino is een Amerikaanse regisseur, scenarioschrijver, producent en acteur."
                },
                new Regisseur
                {
                    RegisseurId = 5,
                    Naam = "Dummy Regisseur",
                    Geboortejaar = 2000,
                    Nationaliteit = "Onbekend",
                    Biografie = "Dit is een dummy regisseur voor testing"
                }
            };
        }
    }
}
