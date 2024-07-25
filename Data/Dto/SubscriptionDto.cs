using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;


namespace giftcard_api.Models
{
    public class SubscriptionDto
    {

        // Propriétés publiques
        public int IdPackage { get; set; }

        public int IdSubscriber { get; set; }

    }
}
