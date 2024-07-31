using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace giftcard_api.Models
{
    public class User
    {
        // Attributs privés
        private int _id;
        private int? _idRole;
        private string _email;
        private string _password;
        private string _telephone;
        private string? _refreshToken;
        private DateTime? _refreshTokenExpiryTime;
        private string? _dateInscription;
        private string _adresse;
        private bool _isActive = true;

        // Propriétés publiques
        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int? IdRole
        {
            get => _idRole;
            set => _idRole = value;
        }
        [JsonIgnore]
        [ForeignKey("IdRole")]
        public Role? Role { get; set; }

        public string Email
        {
            get => _email;
            set => _email = value;
        }
        public string Telephone
        {
            get => _telephone;
            set => _telephone = value;
        }
        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public string? RefreshToken
        {
            get => _refreshToken;
            set => _refreshToken = value;
        }

        public DateTime? RefreshTokenExpiryTime
        {
            get => _refreshTokenExpiryTime;
            set => _refreshTokenExpiryTime = value;
        }
        [StringLength(50)]
        public string? DateInscription
        {
            get => _dateInscription;
            set => _dateInscription = value;
        }

        public string Adresse
        {
            get => _adresse;
            set => _adresse = value;
        }

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }
    }
}
