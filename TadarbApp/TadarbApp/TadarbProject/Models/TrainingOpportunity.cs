using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class TrainingOpportunity
    {

        [Key]
        public int TrainingOpportunityId { get; set; }

        [Required]
        public int Branch_BranchId { get; set; }
        [ForeignKey("Branch_BranchId")]
        [ValidateNever]
        public OrganizationBranch_TrainProv Branch { get; set; }

        [Required]
        public int CreatedByEmployeeId { get; set; }
        [ForeignKey("CreatedByEmployeeId")]
        [ValidateNever]
        public Employee CreatedByEmployee { get; set; }

        [Required]
        public int TotalNumberOfSeats { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime StartRegisterDate { get; set; }

        [Required]
        public DateTime EndRegisterDate { get; set; }

        [Required]
        public string OpportunityStatus { get; set; } = "Available";

        [Required]
        public string AbilityofSubmissionStatus { get; set; } = "Available";

        [Required]
        public int SupervisorEmployeeId { get; set; }
        [ForeignKey("SupervisorEmployeeId")]
        [ValidateNever]
        public Employee SupervisorEmployee { get; set; }


        [Required]
        public int Department_DepartmentId { get; set; }
        [ForeignKey("Department_DepartmentId")]
        [ValidateNever]
        public Department Department { get; set; }

        [Required]
        public int DetailFiled_DetailFiledId { get; set; }
        [ForeignKey("DetailFiled_DetailFiledId")]
        [ValidateNever]
        public FieldOfSpecialtyDetails DetailFiled { get; set; }

        [Required]
        public string OpportunityDescription { get; set; }

        [Required]
        public int RequestedOpportunities { get; set; } = 0;

        [Required]
        public int ApprovedOpportunities { get; set; } = 0;

        [Required]
        public int RejectedOpportunities { get; set; } = 0;

        [Required]
        public int AvailableOpportunities { get; set; }

        [Required]
        public int MinimumHours { get; set; }

        [Required]
        public int MinimumWeeks { get; set; }

        [Required]
        public int TrainingType_TrainingTypeId { get; set; }

        [ForeignKey("TrainingType_TrainingTypeId")]
        [ValidateNever]
        public TrainingType trainingType { get; set; }
    }


}
