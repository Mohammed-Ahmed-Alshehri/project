using System.ComponentModel.DataAnnotations;

namespace TadarbProject.Models
{
	public class Job
	{
		[Required]
		public int JobId { get; set; }

		[Required]
		public string JobName { get; set; }
	}
}
