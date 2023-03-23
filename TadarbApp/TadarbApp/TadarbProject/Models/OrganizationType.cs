using System.ComponentModel.DataAnnotations;

namespace TadarbProject.Models
{
    public class OrganizationType
    {
        [Key]
        public int TypeId { get; set; }

        [Required]
        public string TypeName { get; set; }
    }
}
