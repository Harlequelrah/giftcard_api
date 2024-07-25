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
        private string _nomPackage;

        private int _id;
        private int _nbrJour;
        private double _budget;
        private double _prix;
        private int?  _maxCarte;
        private int _montantBase;
        private string _description;




        // Propriété statique


        // Propriétés publiques


        public int NbrJour
        {
            get => _nbrJour;
            set => _nbrJour = value;
        }

        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }
        public string NomPackage
        {
            get => _nomPackage;
            set => _nomPackage = value;
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

        public int? MaxCarte
        {
            get => _maxCarte;
            set => _maxCarte = value;
        }

        public int MontantBase
        {
            get => _montantBase;
            set => _montantBase = value;
        }
        public string Description
        {
            get => _description;
            set => _description = value;
        }



        public ICollection<Subscription> PackageSubscriptions { get; set; } = new HashSet<Subscription>();
    }
}
