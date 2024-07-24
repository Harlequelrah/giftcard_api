using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace giftcard_api.Data
{
    public class BeneficiaryDto
    {
        [JsonIgnore]
        public int IdRole { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Adresse { get; set; }
        [Required]
        public string Nom { get; set; }
        [Required]
        public string Prenom { get; set; }
        [Required]
        public bool Has_gochap { get; set; }
    }
}
