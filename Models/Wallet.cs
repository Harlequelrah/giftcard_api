using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace giftcard_api.Models
{
    public class Wallet
    {
        private int _id;
        private double _solde = 0.0;
        private string _devise = "XOF";

        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }


        public double Solde
        {
            get => _solde;
            set => _solde = value;
        }


        public string Devise
        {
            get => _devise;
            set => _devise = value;
        }
    }
    public class MerchantWallet : Wallet
    {
        public MerchantWallet() : base() { }
    }
    public class SubscriberWallet : Wallet
    {
        public SubscriberWallet() : base() { }
    }
    public class BeneficiaryWallet : Wallet
    {
        public BeneficiaryWallet() : base() { }
    }

}
