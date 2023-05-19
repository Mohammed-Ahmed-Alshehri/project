using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class DepartmentAssessmentTypeMaster
    {
        [Key]
        public int DepartmentAssessmentTypeMasterId { get; set; }

        [Required]
        public int Department_DepartmentId { get; set; }

        [ForeignKey("Department_DepartmentId")]
        [ValidateNever]
        public Department department { get; set; }


        [Required]
        public int Employee_EmployeeId { get; set; }

        [ForeignKey("Employee_EmployeeId")]
        [ValidateNever]
        public Employee employee { get; set; }

        [Required]
        public DateTime StartActivationDate { get; set; }


        [Required]
        public int RequireCompletionHours { get; set; }


        [Required]
        public int AcademicSupervisorMarks { get; set; }


        [Required]
        public int TrainingSupervisorMarks { get; set; }

        public string ActivationStatus { get; set; } = "Active";

    }
}
