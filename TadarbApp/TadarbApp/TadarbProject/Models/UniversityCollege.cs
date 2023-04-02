using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
	public class UniversityCollege
	{


		[Key]
		public int CollegeId { get; set; }

		[Required]
		public int Organization_OrganizationId { get; set; }

		[ForeignKey("Organization_OrganizationId")]
		[ValidateNever]
		public Organization organization { get; set; }


		[Required]
		public string CollegeName { get; set; }

		[Required]
		public int City_CityId { get; set; }
		[ForeignKey("City_CityId")]
		[ValidateNever]
		public City city { get; set; }


		[Required]
		public int Responsible_UserId { get; set; }
		[ForeignKey("Responsible_UserId")]
		[ValidateNever]
		public UserAcount User { get; set; }

		[Required]

		public int FieldOfOrganization_SpecialtiesField { get; set; }
		[ForeignKey("FieldOfOrganization_SpecialtiesField")]
		[ValidateNever]
		public FieldOfSpecialtyMaster fieldOfSpecialtyMaster { get; set; }


		public string Zoon { get; set; }

		public string Location { get; set; }
	}
}
