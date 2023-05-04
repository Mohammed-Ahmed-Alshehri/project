using System.ComponentModel.DataAnnotations;

namespace TadarbProject.Models
{
    public class TrainingType
    {

        [Key]
        public int TrainingTypeId { get; set; }


        [Required]
        public string TypeName { get; set; }


        [Required]
        public string Description { get; set; }


    }
}
