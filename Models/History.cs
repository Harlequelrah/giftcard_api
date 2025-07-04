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
            MaintenanceSolde,
        }

        private int _idMerchant;
        public int IdMerchant { get => _idMerchant; set => _idMerchant = value; }

        [JsonIgnore]
        [ForeignKey("IdMerchant")]
        public Merchant Merchant { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
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
            MaintenanceSolde,
            Enregistrement,
            Souscription,
            PackageExpiration,
        }
        private int _idSubscriber;
        public int IdSubscriber { get => _idSubscriber; set => _idSubscriber = value; }

        [JsonIgnore]
        [ForeignKey("IdSubscriber")]
        public Subscriber Subscriber { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
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
            MaintenanceSolde,
        }
        private int _idBeneficiary;
        public int IdBeneficiary { get => _idBeneficiary; set => _idBeneficiary = value; }
        [JsonIgnore]
        [ForeignKey("IdBeneficiary")]
        public Beneficiary Beneficiary { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BeneficiaryActions Action { get; set; }
        public BeneficiaryHistory() : base() { }
        public BeneficiaryHistory(BeneficiaryActions action) : base()
        {
            Action = action;
        }
    }

}
