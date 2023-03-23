using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required]
        public string CityName { get; set; }

        [Required]
        public int Country_CountryId { get; set; }

        [ForeignKey("Country_CountryId")]
        [ValidateNever]
        public Country Country { get; set; }
    }
}
