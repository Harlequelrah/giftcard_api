using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace giftcard_api.Data
{
    public class PayementDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public double Montant { get; set; }
    }
}
