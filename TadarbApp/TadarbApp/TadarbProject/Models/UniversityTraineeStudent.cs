using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class UniversityTraineeStudent
    {
        [Key]
        public int TraineeId { get; set; }

        [Required]
        public int Department_DepartmentId { get; set; }


        [ForeignKey("Department_DepartmentId")]
        [ValidateNever]
        public Department department { get; set; }



        [Required]
        public int UserAccount_UserId { get; set; }


        [ForeignKey("UserAccount_UserId")]
        [ValidateNever]
        public UserAcount user { get; set; }

        [Required]
        public string UniversityStudentNumber { get; set; }

        [Required]
        public int CompletedHours { get; set; }

        public string? CV_Path { get; set; }

        public string? ExtraDocuments_Path { get; set; }


        public string? SkillsDescription { get; set; }

    }
}
