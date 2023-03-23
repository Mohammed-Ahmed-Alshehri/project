using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class UserAcount
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage = "ايميل")]
        public string UserEmail { get; set; }

        [Required]
        public string UserPassword { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]

        public string FullName { get; set; }

        [Required]
        public int City_CityId { get; set; }

        [ForeignKey("City_CityId")]
        [ValidateNever]
        public City City { get; set; }

        //set Defult values
        public string UserType { get; set; } = "Non_Admin";

        //set Defult values
        public string ActivationStatus { get; set; } = "Not_Active";


    }
}
