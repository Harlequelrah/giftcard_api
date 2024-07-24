using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace giftcard_api.Models
{
    public class Merchant:User
    {
        // Attributs privés
        private int _id;
        private int _idUser;
        private int _idMerchantWallet;
        private string _nom;
        private string _prenom;
        private byte[]? _profilePhoto;

        // Propriétés publiques
        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int IdUser
        {
            get => _idUser;
            set => _idUser = value;
        }

        public int IdMerchantWallet
        {
            get => _idMerchantWallet;
            set => _idMerchantWallet = value;
        }

        [ForeignKey("IdUser")]
        public User User { get; set; }

        [ForeignKey("IdMerchantWallet")]
        public MerchantWallet MerchantWallet { get; set; }

        public string Nom
        {
            get => _nom;
            set => _nom = value;
        }

        public string Prenom
        {
            get => _prenom;
            set => _prenom = value;
        }

        public byte[]? ProfilePhoto
        {
            get => _profilePhoto;
            set => _profilePhoto = value;
        }

        public ICollection<MerchantHistory> Histories { get; set; } = new HashSet<MerchantHistory>();
    }
}
