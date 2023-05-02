using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class TrainingType
    {

        [Key]
        public int TrainingTypeId { get; set; }


        [Required]
        public string TypeName { get; set; }


        [Required]
        public string Description { get; set; }


    }
}
