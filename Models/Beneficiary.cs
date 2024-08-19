using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace giftcard_api.Models
{
    public class Beneficiary
    {
        // Attributs privés
        private int _id;
        private int? _idUser = null;
        private int _idSubscriber;
        private int _idBeneficiaryWallet;
        private string _nom;
        private string _prenom;

        private string _email;
        private bool _has_gochap;
        private string _telephoneNumero;


        // Propriétés publiques
        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }
        public bool Has_gochap
        {
            get => _has_gochap;
            set => _has_gochap = value;
        }

        public string Email
        {
            get =>_email;
            set => _email = value;
        }

        public int? IdUser
        {
            get => _idUser;
            set => _idUser = value;
        }
        public int IdSubscriber
        {
            get => _idSubscriber;
            set => _idSubscriber = value;
        }

        public int IdBeneficiaryWallet
        {
            get => _idBeneficiaryWallet;
            set => _idBeneficiaryWallet = value;
        }

        [JsonIgnore]
        [ForeignKey("IdUser")]
        public User User { get; set; }


        [ForeignKey("IdBeneficiaryWallet")]
        public BeneficiaryWallet BeneficiaryWallet { get; set; }

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
        public string TelephoneNumero
        {
            get => _telephoneNumero;
            set => _telephoneNumero = value;
        }


        public Beneficiary()
        {

        }
        public Beneficiary(int idUser, string nom, string prenom, bool has_gochap)
        {
            _idUser = idUser;
            _nom = nom;
            _prenom = prenom;
            _has_gochap = has_gochap;
        }

        [JsonIgnore]
        public ICollection<BeneficiaryHistory> Histories { get; set; } = new HashSet<BeneficiaryHistory>();
    }
}
