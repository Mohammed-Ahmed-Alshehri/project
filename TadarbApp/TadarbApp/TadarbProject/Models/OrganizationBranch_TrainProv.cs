using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TadarbProject.Models
{
	public class OrganizationBranch_TrainProv
	{

		[Key]
		public int BranchId { get; set; }

		[Required]
		public int Organization_OrganizationId { get; set; }

		[ForeignKey("Organization_OrganizationId")]
		[ValidateNever]
		public Organization organization { get; set; }

		[Required]
		public string BranchName { get; set; }

		[Required]

		public int City_CityId { get; set; }

		[ForeignKey("City_CityId")]
		[ValidateNever]
		public City city { get; set; }


		[Required]

		public int Responsible_UserId { get; set; }

		[ForeignKey("Responsible_UserId")]
		[ValidateNever]
		public UserAcount user { get; set; }


		public string Zoon { get; set; }

		public string Location { get; set; }


	}
}
