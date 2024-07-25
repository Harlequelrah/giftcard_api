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
        private string _dateSouscription;
        private int? _nbrCarteGenere=0;
        private DateTime? _dateExpiration;

        // Propriétés publiques
        public int IdPackage
        {
            get => _idPackage;
            set => _idPackage = value;
        }
        public int? NbrCarteGenere
        {
            get => _nbrCarteGenere;
            set => _nbrCarteGenere = value;
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


        [StringLength(50)]
        public string DateSouscription
        {
            get => _dateSouscription;
            set => _dateSouscription = value;
        }
        public DateTime? DateExpiration
        {
            get => _dateExpiration;
            set => _dateExpiration = value;
        }
    }
}
