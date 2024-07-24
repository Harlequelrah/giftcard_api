using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace giftcard_api.Models
{
    public class Package
    {
        // Attributs privés
        private int _id;
        private double _budget;
        private double _prix;
        private int _maxCarte;
        private int _montantBase;
        private DateTime _dateExpirations;

        // Propriété statique
        public static int NbrCarteGenere { get; set; }

        // Propriétés publiques
        [JsonIgnore]
        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public double Budget
        {
            get => _budget;
            set => _budget = value;
        }

        public double Prix
        {
            get => _prix;
            set => _prix = value;
        }

        public int MaxCarte
        {
            get => _maxCarte;
            set => _maxCarte = value;
        }

        public int MontantBase
        {
            get => _montantBase;
            set => _montantBase = value;
        }

        public DateTime DateExpirations
        {
            get => _dateExpirations;
            set => _dateExpirations = value;
        }

        public ICollection<Subscription> Subscribers { get; set; } = new HashSet<Subscription>();
    }
}
