using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TadarbProject.Models.ViewModels
{
    public class DepartmentVM
    {


        public Department department { get; set; }

        [ValidateNever]

        public IEnumerable<SelectListItem> UserListItems { get; set; }
    }
}
