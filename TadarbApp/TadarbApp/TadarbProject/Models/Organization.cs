using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
    public class Organization
    {
        [Key]
        public int OrganizationId { get; set; }

        [Required]
        public string OrganizationName { get; set; }

        [Required]
        public int Organization_TypeId { get; set; }

        [ForeignKey("Organization_TypeId")]
        [ValidateNever]
        public OrganizationType OrganizationType { get; set; }

        //set Defult values

        public DateTime SubscriptionDate { get; set; } = DateTime.Now.Date;

        [Required]
        public string CommercialRegistrationNumber { get; set; }

        //set Defult values
        public string ActivationStatus { get; set; } = "Not_Active";

        [Required]
        public int ResponsibleUserId { get; set; }

        [ForeignKey("ResponsibleUserId")]
        [ValidateNever]
        public UserAcount user { get; set; }


        [Required]
        public int FieldOfOrganization_SpecialtiesFieldId { get; set; }

        [ForeignKey("FieldOfOrganization_SpecialtiesFieldId")]
        [ValidateNever]
        public FieldOfSpecialtyMaster FieldOfSpecialty { get; set; }

        [ValidateNever]
        public string LogoPath { get; set; }

        [Required]
        public int MainBranchCityId { get; set; }

        [ForeignKey("MainBranchCityId")]
        [ValidateNever]
        public City City { get; set; }

        public string OrganizationURL { get; set; }

        [ValidateNever]
        public string Location { get; set; }


    }
}
