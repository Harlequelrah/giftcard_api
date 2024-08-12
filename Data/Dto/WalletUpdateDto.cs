using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace giftcard_api.Models
{
    public class WalletUpdateDto
    {
        public int Id { get; set; }
        public double Montant { get; set; }
    }
}
