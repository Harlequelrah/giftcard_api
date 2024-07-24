using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace giftcard_api.Data
{
    public class SubscriberDto
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
        public string SubscriberName { get; set; }
    }
}
