using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TadarbProject.Models.ViewModels
{
    public class TrainingOpportunityVM
    {
        public TrainingOpportunity TrainingOpportunity { get; set; }

        [ValidateNever]

        public IEnumerable<SelectListItem> UserListItems { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> DepartmentListItems { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> DetailFieldsListItems { get; set; }
        public IEnumerable<SelectListItem> TrainingTypeListItems { get; set; }



    }
}
