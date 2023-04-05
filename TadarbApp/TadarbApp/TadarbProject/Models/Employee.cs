using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class Employee
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int Department_DepartmentId { get; set; }
        [ForeignKey("Department_DepartmentId")]
        [ValidateNever]
        public Department department { get; set; }


        [Required]
        public int Job_JobId { get; set; }
        [ForeignKey("Job_JobId")]
        [ValidateNever]
        public Job job { get; set; }

        [Required]
        public int UserAccount_UserId { get; set; }
        [ForeignKey("UserAccount_UserId")]
        [ValidateNever]
        public UserAcount userAcount { get; set; }

        [Required]
        public string SSN { get; set; }


        [Required]
        public string ActivationStatus { get; set; } = "Active";


    }

}
