using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TadarbProject.Models.ViewModels
{
    public class SpecialitiesVM
    {


        [ValidateNever]
        public IEnumerable<SelectListItem> MasterFieldsListItems { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> DetailFieldsListItems { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> DepartmentListItems { get; set; }

    }
}
