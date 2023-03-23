using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class FieldOfSpecialtyDetails
    {
        [Key]
        public int DetailFieldId { get; set; }

        [Required]
        public string SpecializationName { get; set; }

        [Required]
        public int Field_FieldId { get; set; }

        [ForeignKey("Field_FieldId")]
        [ValidateNever]
        public FieldOfSpecialtyMaster FieldOfSpecialty { get; set; }
    }
}
