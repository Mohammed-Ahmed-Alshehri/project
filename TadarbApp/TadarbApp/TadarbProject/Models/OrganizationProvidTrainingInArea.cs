using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class OrganizationProvidTrainingInArea
    {
        [Key]
        public int OPTAreaId { get; set; }

        [Required]
        public int Organization_OrganizationId { get; set; }

        [ForeignKey("Organization_OrganizationId")]
        [ValidateNever]
        public Organization organization { get; set; }


        [Required]
        public int DetailField_DetailFieldId { get; set; }

        [ForeignKey("DetailField_DetailFieldId")]
        [ValidateNever]
        public FieldOfSpecialtyDetails fieldOfSpecialtyDetails { get; set; }


    }
}
