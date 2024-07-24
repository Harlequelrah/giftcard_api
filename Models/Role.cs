using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace giftcard_api.Models
{
    public class Role
    {
        // Attribut privé
        private int _id;
        private string _roleNom;

        // Propriétés publiques
        [JsonIgnore]
        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [Required]
        public string RoleNom
        {
            get => _roleNom;
            set => _roleNom = value;
        }

    }
}
