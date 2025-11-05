using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace CineFlix.Models
{
    public class CineFlixUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }
        public virtual ICollection<Film> AddedFilms { get; set; } = new List<Film>();
        public string FullName => $"{FirstName} {LastName}";
    }
}