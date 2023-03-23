using System.ComponentModel.DataAnnotations;

namespace TadarbProject.Models
{
    public class FieldOfSpecialtyMaster
    {
        [Key]
        public int FieldId { get; set; }

        [Required]
        public string FieldName { get; set; }
    }
}
