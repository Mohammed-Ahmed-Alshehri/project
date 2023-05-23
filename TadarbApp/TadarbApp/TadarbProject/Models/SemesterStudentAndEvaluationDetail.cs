using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class SemesterStudentAndEvaluationDetail
    {
        [Key]
        public int SemesterStudentAndEvaluationDetailId { get; set; }

        [Required]
        public int SemesterMaster_SemesterMasterId { get; set; }

        [ForeignKey("SemesterMaster_SemesterMasterId")]
        [ValidateNever]
        public SemesterTrainingSettingMaster semesterMaster { get; set; }


        [Required]
        public int StudentRequest_StudentRequestId { get; set; }

        [ForeignKey("StudentRequest_StudentRequestId")]
        [ValidateNever]
        public StudentRequestOpportunity studentRequest { get; set; }



        [Required]
        public int AcademicSupervisor_EmployeeId { get; set; }

        [ForeignKey("AcademicSupervisor_EmployeeId")]
        [ValidateNever]
        public Employee EmployeeAcademicSupervisor { get; set; }


        [Required]
        public int TrainingSupervisor_EmployeeId { get; set; }

        [ForeignKey("TrainingSupervisor_EmployeeId")]
        [ValidateNever]
        public Employee EmployeeTrainingSupervisor { get; set; }


    
        public int? TrainingSupervisorEvaluationMark { get; set; }

        public int? CompletedStudyHour { get; set; }


        public int? AcademicSupervisorEvaluationMark { get; set; }


        [Required]
        public string GeneralTrainingStatus { get; set; } = "Company Approved";


        public string? TrainingSupervisorEvaluationFilePath { get; set; }

        public string? Notes { get; set; }
    }
}
