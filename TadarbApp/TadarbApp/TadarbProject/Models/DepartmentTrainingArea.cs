using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace TadarbProject.Models
{
	public class DepartmentTrainingArea
	{

		[Required]
		public int DepartmentTrainingAreaId { get; set; }


		[Required]
		public int Department_DepartmenId { get; set; }


		[ForeignKey("Department_DepartmenId")]
		[ValidateNever]
		public Department department { get; set; }

		[Required]
		public int TrainArea_DetailFiledId { get; set; }


		[ForeignKey("TrainArea_DetailFiledId")]
		[ValidateNever]
		public FieldOfSpecialtyDetails fieldOfSpecialtyDetails { get; set; }

	}
}
