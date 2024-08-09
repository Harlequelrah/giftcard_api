using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;


namespace giftcard_api.Models
{
    public class SubscriptionDto
    {

        [Required]
        public int IdPackage { get; set; }
        [JsonIgnore]
        public int? NbrCarteGenere { get; set; } = 0;

        [Required]
        public int IdSubscriber { get; set; }
        public double? MontantParCarte { get; set; } = null;

    }
    public class FullSubscriptionDto
    {

        public int Id { get; set; }

        public string NomPackage { get; set; }

        public string? NomSubscriber { get; set; }

        public int? NbrCarteGenere { get; set; }

        public double BudgetRestant {  get ; set; }

        public string DateSouscription { get; set; }

        public string? DateExpiration { get; set; }

        public double? MontantParCarte { get; set; }

    }
}
