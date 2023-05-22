using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class StudentSemesterEvaluationMark
    {

        [Key]
        public int StudentSemesterEvaluationMarkId { get; set; }



        [Required]
        public int SemesterStudentAndEvaluationDetail_DetailId { get; set; }

        [ForeignKey("SemesterStudentAndEvaluationDetail_DetailId")]
        [ValidateNever]
        public SemesterStudentAndEvaluationDetail semesterStudentAndEvaluationDetail { get; set; }


        public int DepartmentAssessmentTypeDetail_DetailId { get; set; }


        [ForeignKey("DepartmentAssessmentTypeDetail_DetailId")]
        [ValidateNever]
        public DepartmentAssessmentTypeDetail assessmentTypeDetail { get; set; }


        public int? StudentMark { get; set; }

        public string? SupportiveDocumentsPath { get; set; }



    }
}
