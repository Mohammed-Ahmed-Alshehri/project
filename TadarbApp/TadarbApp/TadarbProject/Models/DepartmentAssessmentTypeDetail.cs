using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class DepartmentAssessmentTypeDetail
    {
        [Key]
        public int DepartmentAssessmentTypeDetailId { get; set; }

        [Required]
        public int DepartmentAssessmentTypeMaster_MasterId { get; set; }

        [ForeignKey("DepartmentAssessmentTypeMaster_MasterId")]
        [ValidateNever]
        public DepartmentAssessmentTypeMaster master { get; set; }


        [Required]
        public int AssessmentType_AssessmentTypeId { get; set; }

        [ForeignKey("AssessmentType_AssessmentTypeId")]
        [ValidateNever]
        public AssessmentType assessmentType { get; set; }

        [Required]
        public int RequiredMark { get; set; }

        public string ActivationStatus { get; set; } = "Active";

    }
}
