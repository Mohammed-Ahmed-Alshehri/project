using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TadarbProject.Models
{
    public class Department
    {

        [Key]
        public int DepartmentId { get; set; }


        [Required]
        public string DepartmentName { get; set; }

        [Required]
        public int Organization_OrganizationId { get; set; }

        [ForeignKey("Organization_OrganizationId")]
        [ValidateNever]
        public Organization organization { get; set; }


        public int? College_CollegeId { get; set; }


        [ForeignKey("College_CollegeId")]
        [ValidateNever]
        public UniversityCollege universityCollege { get; set; }



        public int? Branch_BranchId { get; set; }

        [ForeignKey("Branch_BranchId")]
        [ValidateNever]
        public OrganizationBranch_TrainProv OrganizationBranch { get; set; }


        [Required]
        public int Responsible_UserId { get; set; }
        [ForeignKey("Responsible_UserId")]
        [ValidateNever]
        public UserAcount User { get; set; }
    }
}
