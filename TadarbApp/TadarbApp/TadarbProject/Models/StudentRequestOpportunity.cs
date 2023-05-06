using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class StudentRequestOpportunity
    {
        [Key]
        public int StudentRequestOpportunityId { get; set; }

        [Required]
        public int TrainingOpportunity_TrainingOpportunityId { get; set; }


        [ForeignKey("TrainingOpportunity_TrainingOpportunityId")]
        [ValidateNever]
        public TrainingOpportunity trainingOpportunity { get; set; }



        [Required]
        public int Trainee_TraineeId { get; set; }


        [ForeignKey("Trainee_TraineeId")]
        [ValidateNever]
        public UniversityTraineeStudent student { get; set; }

        [Required]
        public string? DecisionStatus { get; set; } = "waiting";

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.Now.Date;


        public DateTime? DecisionDate { get; set; } = null;


    }
}
