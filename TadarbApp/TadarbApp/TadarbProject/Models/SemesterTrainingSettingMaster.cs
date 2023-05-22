using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class SemesterTrainingSettingMaster
    {
        [Key]
        public int SemesterTrainingSettingMasterId { get; set; }

        [Required]
        public int Department_DepartmenId { get; set; }

        [ForeignKey("Department_DepartmenId")]
        [ValidateNever]
        public Department departmet { get; set; }

        [Required]
        public int AcademicYear { get; set; }

        [Required]
        public string SemesterType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }


        [Required]
        public string ActivationStatus { get; set; }


        [Required]
        public int TrainingType_TrainingTypeId { get; set; }

        [ForeignKey("TrainingType_TrainingTypeId")]
        [ValidateNever]
        public TrainingType trainingType { get; set; }


        [Required]
        public int RequiredWeeks { get; set; }

        [Required]
        public int MinimumRequiredHours { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now.Date;


        [Required]
        public int CreatedByEmployee_EmployeeId { get; set; }

        [ForeignKey("CreatedByEmployee_EmployeeId")]
        [ValidateNever]
        public Employee CreatedByEmployee { get; set; }



        public string EvaluationFileToTrainingSupervisor { get; set; }
    }
}
