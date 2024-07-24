using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace giftcard_api.Models
{
    public class Subscription
    {
        // Attributs privés
        private int _idPackage;
        private int _idSubscriber;
        private DateTime _dateSouscription;

        // Propriétés publiques
        public int IdPackage
        {
            get => _idPackage;
            set => _idPackage = value;
        }

        public int IdSubscriber
        {
            get => _idSubscriber;
            set => _idSubscriber = value;
        }

        [ForeignKey("IdPackage")]
        public Package Package { get; set; }

        [ForeignKey("IdSubscriber")]
        public Subscriber Subscriber { get; set; }

        public DateTime DateSouscription
        {
            get => _dateSouscription;
            set => _dateSouscription = value;
        }
    }
}
