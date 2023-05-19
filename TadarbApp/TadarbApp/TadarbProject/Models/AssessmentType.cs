using System.ComponentModel.DataAnnotations;

namespace TadarbProject.Models
{
    public class AssessmentType
    {
        [Key]
        public int AssessmentTypeId { get; set; }

        [Required]

        public string AssessmentTypeName { get; set; }


    }
}
