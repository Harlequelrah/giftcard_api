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


    }
}
