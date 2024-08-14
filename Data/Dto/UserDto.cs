using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace giftcard_api.Models
{
    public class UserDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Telephone { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Adresse { get; set; }
        [Required]
        public string NomComplet { get; set; }


    }
    public class UserUpdateDto
    {
        [Required]
        public int Id { get; set; }

        public string? Email { get; set; }

        public string? NomComplet { get; set; }

        public string? Telephone { get; set; }

        public string? Password { get; set; }

        public string? Adresse { get; set; }

        public bool IsActive { get; set; } = true;


    }
        public class FullUser
    {
        public int Id { get; set; }

        public string NomComplet { get; set; }
        public string NomRole { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string DateInscription { get; set; }
        public string Adresse { get; set; }
        public bool IsActive { get; set; }

    }
    public class AppUser
    {
        public int SpecialId{ get; set; }
        public string NomComplet { get; set; }
        public string Email { get; set; }
        public string Solde { get; set; }
    }
}
