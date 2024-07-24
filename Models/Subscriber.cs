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
        private int _idSubscriberHistory;
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

        [ForeignKey("IdUser")]
        public User User { get; set; }

        [ForeignKey("IdSubscriberWallet")]
        public SubscriberWallet SubscriberWallet { get; set; }

        public ICollection<Subscription> Packages { get; set; }
        public ICollection<SubscriberHistory> Histories { get; set; } = new HashSet<SubscriberHistory>();

    }
}
