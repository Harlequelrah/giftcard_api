using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace giftcard_api.Data
{
    public class RoleDto
    {
        [Required]

        public int Id { get; set; }

        [Required]
        public string RoleNom  { get; set; }


    }
}
