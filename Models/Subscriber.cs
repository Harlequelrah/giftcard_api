using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace giftcard_api.Models
{
    public class Subscriber
    {
        private int _id;
        private int _idUser;
        private int _idSubscriberWallet;

        private string _subscriberName;

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

        public int IdSubscriberWallet
        {
            get => _idSubscriberWallet;
            set => _idSubscriberWallet = value;
        }

        public string SubscriberName
        {
            get => _subscriberName;
            set => _subscriberName = value;
        }
        [JsonIgnore]
        [ForeignKey("IdUser")]
        public User User { get; set; }
        
        [ForeignKey("IdSubscriberWallet")]
        public SubscriberWallet SubscriberWallet { get; set; }
        [JsonIgnore]
        public ICollection<Subscription> SubscriberSubscriptions { get; set; }  = new HashSet<Subscription>();
        [JsonIgnore]
        public ICollection<SubscriberHistory> Histories { get; set; } = new HashSet<SubscriberHistory>();

    }
}
