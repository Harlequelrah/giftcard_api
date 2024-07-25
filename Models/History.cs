using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace giftcard_api.Models
{
    public class History
    {
        private int _id;
        private double _montant;
        private string _date;

        // Propriétés publiques
        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public double Montant
        {
            get => _montant;
            set => _montant = value;
        }
        [StringLength(50)]
        public string Date
        {
            get => _date;
            set => _date = value;
        }
    }
    public class MerchantHistory : History
    {
        public enum MerchantActions
        {
            Initial,
            Remboursement,
            Encaissement,
        }

        private int _idMerchant;
        public int IdMerchant { get => _idMerchant; set => _idMerchant = value; }

        [ForeignKey("IdMerchant")]
        public Merchant Merchant { get; set; }

        public MerchantActions Action { get; set; }
        public MerchantHistory() : base() { }
        public MerchantHistory(MerchantActions action) : base()
        {
            Action = action;
        }
    }
    public class SubscriberHistory : History
    {
        public enum SubscriberActions
        {
            Initial,
            Enregistrement,
            Souscription,
            PackageExpiration,
        }
        private int _idSubscriber;
        public int IdSubscriber { get => _idSubscriber; set => _idSubscriber = value; }

        [ForeignKey("IdSubscriber")]
        public Subscriber Subscriber { get; set; }

        public SubscriberActions Action { get; set; }
        public SubscriberHistory() : base() { }
        public SubscriberHistory(SubscriberActions action) : base()
        {
            Action = action;
        }
    }
    public class BeneficiaryHistory : History
    {
        public enum BeneficiaryActions
        {
            Initial,
            Depense,
            Recharge,
        }
        private int _idBeneficiary;
        public int IdBeneficiary { get => _idBeneficiary; set => _idBeneficiary = value; }

        [ForeignKey("IdBeneficiary")]
        public Beneficiary Beneficiary { get; set; }

        public BeneficiaryActions Action { get; set; }
        public BeneficiaryHistory() : base() { }
        public BeneficiaryHistory(BeneficiaryActions action) : base()
        {
            Action = action;
        }
    }

}
