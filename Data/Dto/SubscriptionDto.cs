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

    }
}
