using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace giftcard_api.Models
{
    public class UserUpdateDto
    {
        [Required]
        public int Id { get; set; }

        public string? Email { get; set; }

        public string? Telephone { get; set; }

        public string? Password { get; set; }

        public string? Adresse { get; set; }

        public bool IsActive { get; set; } = true;


    }
}
