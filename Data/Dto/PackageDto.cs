using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace giftcard_api.Models
{
    public class PackageDto
    {
        [Required]
        public string NomPackage { get; set; }
        [Required]
        public string Description { get; set; }

        public int? NbrJour { get; set; } = null ;
        [Required]
        public double Budget { get; set; }
        [Required]
        public double Prix { get; set; }
        [Required]
        public double MontantBase { get; set; }

}
}
