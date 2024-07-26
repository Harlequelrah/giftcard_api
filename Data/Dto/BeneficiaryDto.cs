using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace giftcard_api.Data
{
    public class BeneficiaryDto
    {
        [Required]
        public int IdSubscription { get; set; }
        [Required]
        public int IdPackage { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }


        [Required]
        public string Nom { get; set; }
        [Required]
        public string Prenom { get; set; }
        [Required]
        public bool Has_gochap { get; set; }

    }
}
