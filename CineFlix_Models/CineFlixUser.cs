using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineFlix_Models
{
    public class CineFlixUser : IdentityUser
    {
        // Extra eigenschappen voor CineFlixUser
        [Required]
        [Display(Name = "Voornaam")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Achternaam")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Registratiedatum")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Display(Name = "Laatst ingelogd")]
        public DateTime? LastLoginDate { get; set; }

        [Display(Name = "Volledige naam")]
        public string FullName => $"{FirstName} {LastName}";

        // Soft delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }

        // Navigatie property voor films die deze gebruiker heeft toegevoegd
        public virtual ICollection<Film> AddedFilms { get; set; } = new List<Film>();

        // Static dummy voor seeding
        public static CineFlixUser? Dummy { get; set; }

        // Seeder methode
        public static void Seeder()
        {
            Dummy = new CineFlixUser
            {
                Id = "dummy-user-id",
                FirstName = "Test",
                LastName = "Gebruiker",
                Email = "test@cineflix.com",
                UserName = "test@cineflix.com",
                EmailConfirmed = true,
                RegistrationDate = DateTime.Now
            };
        }
    }
}